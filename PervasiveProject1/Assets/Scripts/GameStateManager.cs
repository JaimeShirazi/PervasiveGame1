using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;

    private static List<BaseObjectSequence> sequences = new()
    {
        new VirtualObjectSequence("this nail", "Nail"),
        new VirtualObjectSequence("this clown", "Clown"),
        new VirtualObjectSequence("this rose", "Rose"),
        new VirtualObjectSequence("this shell", "Shell"),
        new VirtualObjectSequence("this watch", "Watch"),
        new VirtualObjectSequence("this phone", "Phone"),
        new RealObjectSequence("your mouse"),
    };

    private static UnityEvent onSequenceEnd = new();
    public static event UnityAction OnSequenceEnd
    {
        add => onSequenceEnd.AddListener(value);
        remove => onSequenceEnd.RemoveListener(value);
    }

    [SerializeField] private CameraManager cameraManager;

    private INetworkManager networkManager;
    /// <remarks>
    /// Do not call this function during Awake (call it later, like from Start or OnEnable)
    /// </remarks>
    public static void SetNetworkManager(INetworkManager networkManager)
    {
        instance.networkManager = networkManager;
        instance.TryBegin();
    }

    private void Awake()
    {
        instance = this;
    }

    void TryBegin()
    {
        if (begun) return;
        if (networkManager == null) return;

        StartCoroutine(Begin());
    }
    private bool begun;
    IEnumerator Begin()
    {
        begun = true;
        for (int i = 0; i < sequences.Count; i++)
        {
            OnSequenceEnd += sequences[i].OnGameStateReset;

            FadeOutUI.FadeOut();
            string networkQuestion = networkManager.ReceiveQuestion(i);

            ObjectSequenceOperation operation = sequences[i].Begin(this, i, networkQuestion);
            yield return operation;

            networkManager.SendQuestion(i, networkQuestion);

            float fadeTime = FadeOutUI.FadeIn();
            yield return new WaitForSeconds(fadeTime);
            cameraManager.ReleaseAll();
            onSequenceEnd.Invoke();

            OnSequenceEnd -= sequences[i].OnGameStateReset;
        }
    }
}
