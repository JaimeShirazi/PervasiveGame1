using UnityEngine;
using System.Collections.Generic;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    public enum CameraInfluencer // Higher = higher priority (overrides lower)
    {
        ObjectFreeInspector = 0,
        BoxManager = 1
    };
    [Serializable]
    public struct CameraParams
    {
        public Vector3 worldPos;
        public Vector3 eulerAngles;
        public float fov;
        public float easeTime;
        public CameraInfluencer index;
        public static float InOutSine(float t) => (float)(Mathf.Cos(t * Mathf.PI) - 1) / -2;
        public static CameraParams Lerp(CameraParams a, CameraParams b, float currentTime, float startedTime)
        {
            if (b.easeTime <= 0) return b;

            float t = (currentTime - startedTime) / b.easeTime;
            if (t >= 1) return b;
            else if (t <= 0) return a;

            t = InOutSine(t);
            return new CameraParams
            {
                worldPos = Vector3.Lerp(a.worldPos, b.worldPos, t),
                eulerAngles = Vector3.Lerp(a.eulerAngles, b.eulerAngles, t),
                fov = Mathf.Lerp(a.fov, b.fov, t),
                easeTime = b.easeTime,
                index = b.index
            };
        }
        public void Apply(Camera target)
        {
            target.transform.position = worldPos;
            target.transform.localEulerAngles = eulerAngles;
            target.fieldOfView = fov;
        }
        public bool IsEqualTo(CameraParams other)
        {
            return worldPos == other.worldPos
                && eulerAngles == other.eulerAngles
                && fov == other.fov
                && index == other.index
                && easeTime == other.easeTime;
        }
    }
    private CameraParams previous;
    private Dictionary<CameraInfluencer, CameraParams> targets = new();
    private CameraInfluencer currentHighest;
    private float timeStarted;
    public void SetParams(CameraParams target)
    {
        if (targets.Count <= 0)
        {
            targets.Add(target.index, target);
            currentHighest = target.index;
            previous = target;
            timeStarted = Time.time - target.easeTime;
            Update();
            return;
        }

        if ((int)target.index >= (int)currentHighest)
        {
            if (target.IsEqualTo(targets[currentHighest])) return;

            previous = CameraParams.Lerp(previous, targets[currentHighest], Time.time, timeStarted);
            timeStarted = Time.time;
            currentHighest = target.index;
        }

        if (!targets.ContainsKey(target.index))
            targets.Add(target.index, target);
        else
            targets[target.index] = target;
    }
    public void Release(CameraInfluencer source)
    {
        if (!targets.ContainsKey(source)) return;

        bool wasActive = source == currentHighest;

        if (wasActive)
        {
            previous = CameraParams.Lerp(previous, targets[currentHighest], Time.time, timeStarted);
        }

        targets.Remove(source);

        if (wasActive)
        {
            CameraInfluencer highestPriority = (CameraInfluencer)0;
            foreach (CameraInfluencer key in targets.Keys)
            {
                if ((int)key > (int)highestPriority)
                {
                    highestPriority = key;
                }
            }

            timeStarted = Time.time;
            currentHighest = highestPriority;
        }
    }
    public void ReleaseAll()
    {
        targets.Clear();
        timeStarted = 0;
        currentHighest = (CameraInfluencer)0;
    }
    void Update()
    {
        if (targets.Count < 1) return;

        CameraParams.Lerp(previous, targets[currentHighest], Time.time, timeStarted).Apply(cam);
    }
}
