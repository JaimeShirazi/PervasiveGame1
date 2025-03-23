using UnityEngine;

public class ObjectSequenceOperation : CustomYieldInstruction
{
    private readonly float expectedLength;
    public float ExpectedLength => expectedLength;
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
    public ObjectSequenceOperation(float expectedLength)
    {
        this.expectedLength = expectedLength;
    }
}