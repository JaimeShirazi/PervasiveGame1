using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        new RealObjectSequence("your mouse"),
    };

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
            string networkQuestion = networkManager.ReceiveQuestion(i);
            BaseObjectSequence.ObjectSequenceOperation operation = sequences[i].Begin(this, i, networkQuestion);
            yield return operation;
            networkManager.SendQuestion(i, networkQuestion);
        }
    }
}
