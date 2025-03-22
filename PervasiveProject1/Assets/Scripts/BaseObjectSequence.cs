using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class BaseObjectSequence
{
    public class ObjectSequenceOperation : CustomYieldInstruction
    {
        private string question;
        private int box;
        public string Question => question;
        public int Box => box;
        internal bool done = false;
        internal void Finish(string question, int box)
        {
            done = true;
            this.question = question;
            this.box = box;
        }
        public override bool keepWaiting => !done;
    }
    private readonly string name;
    public BaseObjectSequence(string name)
    {
        this.name = name;
    }
    private ObjectSequenceOperation current;
    public ObjectSequenceOperation Begin(MonoBehaviour caller, int thisIndex, string networkQuestion)
    {
        if (current != null) return current;

        current = new();
        caller.StartCoroutine(MainSequence(thisIndex, networkQuestion));
        return current;
    }
    const float MESSAGE_TIME = 5f;
    public IEnumerator MainSequence(int thisIndex, string networkQuestion)
    {
        OnBegin();
        bool isEven = thisIndex % 2 == 0;

        TextDisplayer.Display("Consider this " + name + ".", IntroductionMessagePosition, MESSAGE_TIME);
        yield return new WaitForSeconds(Length * (10f / 60f));
        TextDisplayer.Display("What is this object's history?", isEven ? TextDisplayer.TextPosition.LeftHigher : TextDisplayer.TextPosition.RightHigher, MESSAGE_TIME);
        yield return new WaitForSeconds(Length * (20f / 60f));
        TextDisplayer.Display(networkQuestion, isEven ? TextDisplayer.TextPosition.RightLower : TextDisplayer.TextPosition.LeftLower, MESSAGE_TIME);
        yield return new WaitForSeconds(Length * (20f / 60f));

        QuestionPromptOperation promptOperation = QuestionPromptManager.Ask(name);
        yield return promptOperation;

        BoxSortingOperation sortOperation = BoxManager.BeginSelection(null);
        yield return sortOperation;

        OnEnd();
        current.Finish(promptOperation.Question, sortOperation.Box);
    }
    protected virtual void OnBegin() { }
    protected virtual void OnEnd() { }
    /// <summary>
    /// Where the "consider this [thing]" text should be placed
    /// </summary>
    protected abstract TextDisplayer.TextPosition IntroductionMessagePosition { get; }
    protected abstract float Length { get; }
    protected abstract bool Sortable { get; }
}
