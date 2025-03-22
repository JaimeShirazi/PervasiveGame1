using UnityEngine;

public class BoxSortingOperation : CustomYieldInstruction
{
    private int box;
    public int Box => box;
    internal bool done = false;
    internal void Finish(int box)
    {
        done = true;
        this.box = box;
    }
    public override bool keepWaiting => !done;
}
