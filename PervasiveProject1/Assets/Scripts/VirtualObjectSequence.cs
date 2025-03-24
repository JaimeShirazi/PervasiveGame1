using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class VirtualObjectSequence : BaseObjectSequence
{
    public VirtualObjectSequence(string name, string prefabPath) : base(name)
    {
        this.prefabPath = prefabPath;
    }
    private string prefabPath;
    private GameObject prefabInstance;
    protected override void OnBegin()
    {
        GameObject prefabResource = Resources.Load(prefabPath) as GameObject;
        prefabInstance = GameObject.Instantiate(prefabResource);
        ObjectFreeInspector.BeginInspecting(prefabInstance);
    }
    protected override IEnumerator OnEnd(UnityAction<string> question, UnityAction<int> targetBox)
    {
        ObjectFreeInspector.EndInspection();

        QuestionPromptOperation promptOperation = QuestionPromptManager.Ask("does this " + name);
        yield return promptOperation;
        question.Invoke(promptOperation.Question);

        BoxSortingOperation sortOperation = BoxManager.BeginSelection(prefabInstance);
        yield return sortOperation;
        targetBox.Invoke(sortOperation.Box);
    }
    public override void OnGameStateReset()
    {
        GameObject.Destroy(prefabInstance);
        prefabInstance = null;
    }
    protected override TextDisplayer.TextPosition IntroductionMessagePosition => TextDisplayer.TextPosition.Top;
    protected override float Length => 40f;
}
