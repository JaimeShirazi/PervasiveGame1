using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;

    private static List<BaseObjectSequence> sequences = new()
    {
        new VirtualObjectSequence("nail", "Nail"),
        new VirtualObjectSequence("clown", "Clown"),
        new VirtualObjectSequence("rose", "Rose"),
        new VirtualObjectSequence("shell", "Shell"),
        new VirtualObjectSequence("watch", "Watch"),
        new VirtualObjectSequence("phone", "Phone"),
        new RealObjectSequence("mouse"),
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

    private ICountdown countdown;
    /// <remarks>
    /// Do not call this function during Awake (call it later, like from Start or OnEnable)
    /// </remarks>
    public static void SetCountdown(ICountdown countdown)
    {
        instance.countdown = countdown;
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
        if (countdown == null) return;

        StartCoroutine(Begin());
    }
    private bool begun;
    IEnumerator Begin()
    {
        begun = true;
        for (int i = 0; i < sequences.Count; i++)
        {
            OnSequenceEnd += sequences[i].OnGameStateReset;

            yield return FadeOutUI.FadeOut();

            string networkQuestion = networkManager.ReceiveQuestion(i);

            ObjectSequenceOperation operation = sequences[i].Begin(this, i, networkQuestion);
            countdown.Begin(operation.ExpectedLength);
            yield return operation;

            networkManager.SendQuestion(i, operation.Question);

            yield return FadeOutUI.FadeIn();

            cameraManager.ReleaseAll();
            onSequenceEnd.Invoke();

            OnSequenceEnd -= sequences[i].OnGameStateReset;
        }

        TextDisplayer.Display("Every object has a story.", TextDisplayer.TextPosition.Middle, 4f);
        yield return new WaitForSeconds(6f);

        TextDisplayer.Display("A Hugh Craig and Jaime Shirazi Game", TextDisplayer.TextPosition.Middle, 4f);
        yield return new WaitForSeconds(4f);
    }
}
