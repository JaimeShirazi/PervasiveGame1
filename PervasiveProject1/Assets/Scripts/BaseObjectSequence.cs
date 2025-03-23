using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class BaseObjectSequence
{
    protected readonly string name;
    protected virtual string ConsiderFormattedName => "this " + name;
    public BaseObjectSequence(string name)
    {
        this.name = name;
    }
    private ObjectSequenceOperation current;
    public ObjectSequenceOperation Begin(MonoBehaviour caller, int thisIndex, string networkQuestion)
    {
        if (current != null) return current;

        current = new(Length);
        caller.StartCoroutine(MainSequence(thisIndex, networkQuestion));
        return current;
    }
    const float MESSAGE_TIME = 5f;
    const string PROMPT_ONE = "Consider {0}.";
    const float DELAY_NORM_START_TO_PROMPT_ONE = 0;
    const string PROMPT_TWO = "What is this object's history?";
    const float DELAY_NORM_PROMPT_ONE_TO_PROMPT_TWO = 10f / 60f;
    const float DELAY_NORM_PROMPT_TWO_TO_PROMPT_THREE = 20f / 60f;
    const float REMAINING_TIME = 1f - (DELAY_NORM_START_TO_PROMPT_ONE + DELAY_NORM_PROMPT_ONE_TO_PROMPT_TWO + DELAY_NORM_PROMPT_TWO_TO_PROMPT_THREE);
    public IEnumerator MainSequence(int thisIndex, string networkQuestion)
    {
        OnBegin();
        bool isEven = thisIndex % 2 == 0;

        if (DELAY_NORM_START_TO_PROMPT_ONE > 0) yield return new WaitForSeconds(Length * DELAY_NORM_START_TO_PROMPT_ONE);

        TextDisplayer.Display(string.Format(PROMPT_ONE, ConsiderFormattedName),
            IntroductionMessagePosition,
            MESSAGE_TIME);

        if (DELAY_NORM_PROMPT_ONE_TO_PROMPT_TWO > 0) yield return new WaitForSeconds(Length * DELAY_NORM_PROMPT_ONE_TO_PROMPT_TWO);

        TextDisplayer.Display(PROMPT_TWO,
            isEven ? TextDisplayer.TextPosition.LeftHigher : TextDisplayer.TextPosition.RightHigher,
            MESSAGE_TIME);

        if (DELAY_NORM_PROMPT_TWO_TO_PROMPT_THREE > 0) yield return new WaitForSeconds(Length * DELAY_NORM_PROMPT_TWO_TO_PROMPT_THREE);

        TextDisplayer.Display(networkQuestion,
            isEven ? TextDisplayer.TextPosition.RightLower : TextDisplayer.TextPosition.LeftLower,
            MESSAGE_TIME);

        if (REMAINING_TIME > 0) yield return new WaitForSeconds(Length * REMAINING_TIME);

        string question = "";
        int box = -1;
        void OnRecieveQuestion(string value)
        {
            question = value;
        }
        void OnRecieveBox(int index)
        {
            box = index;
        }

        yield return OnEnd(OnRecieveQuestion, OnRecieveBox);

        current.Finish(question, box);
    }
    protected virtual void OnBegin() { }
    protected virtual IEnumerator OnEnd(UnityAction<string> question, UnityAction<int> targetBox)
    {
        question.Invoke(""); targetBox.Invoke(-1);
        yield return null;
    }
    public virtual void OnGameStateReset() { }
    /// <summary>
    /// Where the "consider this [thing]" text should be placed
    /// </summary>
    protected abstract TextDisplayer.TextPosition IntroductionMessagePosition { get; }
    protected abstract float Length { get; }
}
