using UnityEngine;

public class RealObjectSequence : BaseObjectSequence
{
    public RealObjectSequence(string name) : base(name) { }
    protected override TextDisplayer.TextPosition IntroductionMessagePosition => TextDisplayer.TextPosition.Middle;
    protected override float Length => 60f;
    protected override bool Sortable => false;
}
