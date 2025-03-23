using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeOutUI : MonoBehaviour
{
    public class FadeOpertaion : CustomYieldInstruction
    {
        private bool done;
        public void Done() => done = true;
        public override bool keepWaiting => !done;
        internal static FadeOpertaion GetCompleteOperation()
        {
            FadeOpertaion op = new();
            op.Done();
            return op;
        }
    }
    private FadeOpertaion current = FadeOpertaion.GetCompleteOperation();

    const float FADE_LENGTH = 1f;

    private static FadeOutUI instance;

    private float timeStarted = -FADE_LENGTH;
    private bool targetVisible;

    private Image self;

    float GetNormalisedTime()
    {
        return (Time.time - timeStarted) / FADE_LENGTH;
    }
    float GetReflectedTime()
    {
        return Time.time - (FADE_LENGTH - (Time.time - timeStarted));
    }
    void SetAlpha(float progress)
    {
        if (!targetVisible)
            progress = 1 - progress;
        self.color = new Color(0, 0, 0, progress);
    }

    void Awake()
    {
        instance = this;
        self = GetComponent<Image>();
        gameObject.SetActive(false);
    }

    /// <returns>How long the fade will take</returns>
    public static FadeOpertaion FadeIn()
    {
        if (!instance.targetVisible)
        {
            instance.BeginToggleFade();
        }
        return instance.current;
    }
    /// <returns>How long the fade will take</returns>
    public static FadeOpertaion FadeOut()
    {
        if (instance.targetVisible)
        {
            instance.BeginToggleFade();
        }
        return instance.current;
    }

    void BeginToggleFade()
    {
        current = new();

        gameObject.SetActive(true);

        if (GetNormalisedTime() > 1)
        {
            timeStarted = Time.time;
        }
        else
        {
            timeStarted = GetReflectedTime();
        }

        targetVisible = !targetVisible;

        StartCoroutine(ToggleFade());
    }
    IEnumerator ToggleFade()
    {
        while (GetNormalisedTime() < 1)
        {
            SetAlpha(GetNormalisedTime());
            yield return null;
        }
        SetAlpha(1);

        current.Done();

        if (!targetVisible)
        {
            gameObject.SetActive(false);
        }
    }
}
