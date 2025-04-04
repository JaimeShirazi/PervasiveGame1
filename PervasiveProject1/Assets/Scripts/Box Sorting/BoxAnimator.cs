using UnityEngine;

public class BoxAnimator : MonoBehaviour
{
    [SerializeField] private Transform lidAnchor;
    [SerializeField] private MeshRenderer topRender, bottomRender;
    private enum AnimationType
    {
        Open, Close
    }
    const float ANIMATION_LENGTH = 0.3f;
    private float timeStartedAnimate = -ANIMATION_LENGTH;
    private AnimationType type = AnimationType.Close;
    private void SetAnimProgress(float t)
    {
        float smoothed = InOutSine(t);
        void SetEulerX(float normValue)
        {
            lidAnchor.localEulerAngles = Vector3.right * 90f * normValue;
        }
        void SetMat(float normValue)
        {
            topRender.material.SetFloat("_AO_Blend", Mathf.Min(normValue * 7f, 1));
            bottomRender.material.SetFloat("_AO_Blend", Mathf.Min(normValue * 7f, 1));
        }
        switch (type)
        {
            case AnimationType.Open:
                SetEulerX(smoothed);
                SetMat(smoothed);
                break;
            case AnimationType.Close:
                SetEulerX(1f - smoothed);
                SetMat(1f - smoothed);
                break;
        }
    }
    private bool IsAnimating() => Time.time - timeStartedAnimate < ANIMATION_LENGTH;
    public static float InOutSine(float t) => (float)(Mathf.Cos(t * Mathf.PI) - 1) / -2;
    private void ReflectTime()
    {
        timeStartedAnimate = Time.time - (ANIMATION_LENGTH - (Time.time - timeStartedAnimate));
    }
    public void OpenLid()
    {
        if (type == AnimationType.Open) return;

        if (IsAnimating())
        {
            switch (type)
            {
                case AnimationType.Close:
                    ReflectTime();
                    break;
                default:
                    timeStartedAnimate = Time.time;
                    break;
            }
        }
        else timeStartedAnimate = Time.time;
        type = AnimationType.Open;
    }
    public void CloseLid()
    {
        if (type == AnimationType.Close) return;

        if (IsAnimating())
        {
            switch (type)
            {
                case AnimationType.Open:
                    ReflectTime();
                    break;
                default:
                    timeStartedAnimate = Time.time;
                    break;
            }
        }
        else timeStartedAnimate = Time.time;
        type = AnimationType.Close;
    }
    public void CloseLidInstantly()
    {
        timeStartedAnimate = Time.time - ANIMATION_LENGTH;
        type = AnimationType.Close;
        SetAnimProgress(1);
    }
    private void Update()
    {
        if (IsAnimating())
        {
            SetAnimProgress((Time.time - timeStartedAnimate) / ANIMATION_LENGTH);
        }
        else
        {
            SetAnimProgress(1);
        }
    }
}
