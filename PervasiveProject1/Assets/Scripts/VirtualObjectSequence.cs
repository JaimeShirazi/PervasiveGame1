using System;
using UnityEngine;

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
    protected override void OnEnd()
    {
        ObjectFreeInspector.EndInspection();
    }
    protected override TextDisplayer.TextPosition IntroductionMessagePosition => TextDisplayer.TextPosition.Top;
    protected override float Length => 60f;
    protected override bool Sortable => true;
}
