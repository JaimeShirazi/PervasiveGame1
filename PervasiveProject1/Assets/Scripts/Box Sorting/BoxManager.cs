using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class BoxManager : MonoBehaviour
{
    private static BoxManager instance;

    [SerializeField] private CameraManager cam;
    [SerializeField] private List<BoxLogic> boxes = new();

    private int hoveringIndex = -1;
    public void SetHovering(BoxLogic box, bool value)
    {
        int index = boxes.IndexOf(box);
        if (index < 0) return;

        if (value) hoveringIndex = index;
        else if (hoveringIndex == index) hoveringIndex = -1;
    }
    private int selectedIndex = -1;
    public void SetSelected(BoxLogic box)
    {
        int index = boxes.IndexOf(box);
        if (index < 0) return;

        selectedIndex = index;
    }

    void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GameStateManager.OnSequenceEnd += ResetState;
    }
    private void OnDisable()
    {
        GameStateManager.OnSequenceEnd -= ResetState;
    }

    private BoxSortingOperation current;
    public static BoxSortingOperation BeginSelection(GameObject target)
    {
        instance.current = new();
        instance.StartCoroutine(instance.SelectionInternal(target));
        return instance.current;
    }
    IEnumerator SelectionInternal(GameObject target)
    {
        cam.SetParams(new()
        {
            worldPos = new Vector3(0, 5f, -6.5f),
            eulerAngles = new Vector3(20f, 0, 0),
            fov = 46f,
            easeTime = 1f,
            index = CameraManager.CameraInfluencer.BoxManager
        });

        float xRange = (boxes.Count - 1) * 8f;
        float GetPos(int index)
        {
            return ((index / (float)(boxes.Count - 1)) * xRange) - (xRange * 0.5f);
        }

        selectedIndex = -1;
        hoveringIndex = -1;

        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].Initialise(
                GetPos(i),
                SetHovering,
                SetSelected
                );
        }

        target.transform.DOMove(new Vector3(0, 4f, 11.78f), 0.4f);
        yield return new WaitForSeconds(0.4f);

        while (selectedIndex < 0)
        {
            if (hoveringIndex >= 0)
            {
                target.transform.position = Vector3.Lerp(target.transform.position, new Vector3(GetPos(hoveringIndex), 4f, 11.78f), Time.deltaTime * 10f);
            }
            yield return null;
        }
        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].FreezeInput();
        }

        target.transform.DOMove(new Vector3(GetPos(hoveringIndex), -5f, 11.78f), 1f);
        Sequence shrink = DOTween.Sequence();
        shrink.AppendInterval(0.2f);
        shrink.Append(target.transform.DOScale(0, 0.8f)).SetEase(Ease.InSine);
        shrink.Play();
        yield return new WaitForSeconds(0.7f);
        boxes[selectedIndex].TrapObject();

        current.Finish(selectedIndex);
    }
    void ResetState()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].ResetState();
        }
    }
}
