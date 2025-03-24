using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RealObjectSequence : BaseObjectSequence
{
    public RealObjectSequence(string name) : base(name) { }
    protected override string ConsiderFormattedName => "your " + name;
    protected override TextDisplayer.TextPosition IntroductionMessagePosition => TextDisplayer.TextPosition.Middle;
    protected override IEnumerator OnEnd(UnityAction<string> question, UnityAction<int> targetBox)
    {
        ObjectFreeInspector.EndInspection();

        targetBox.Invoke(0);

        QuestionPromptOperation promptOperation = QuestionPromptManager.Ask("would someone's " + name);
        yield return promptOperation;
        question.Invoke(promptOperation.Question);
    }

    protected override float Length => 40f;
}
