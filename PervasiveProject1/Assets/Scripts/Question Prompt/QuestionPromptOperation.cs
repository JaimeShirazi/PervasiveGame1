using UnityEngine;

public class QuestionPromptOperation : CustomYieldInstruction
{
    private string question;
    public string Question => question;
    internal bool done = false;
    internal void Finish(string question)
    {
        done = true;
        this.question = question;
    }
    public override bool keepWaiting => !done;
}