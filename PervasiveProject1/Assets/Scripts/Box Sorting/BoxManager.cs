using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BoxManager : MonoBehaviour
{
    private static BoxManager instance;

    [SerializeField] private CameraManager cam;
    [SerializeField] private List<BoxLogic> boxes = new();

    void Awake()
    {
        instance = this;
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
        for (int i = 0; i < boxes.Count; i++)
        {

        }
        yield return new WaitForSeconds(2);
        cam.Release(CameraManager.CameraInfluencer.BoxManager);
        current.Finish(0);
    }
}
