using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeOutUI : MonoBehaviour
{
    const float FADE_LENGTH = 1f;

    private static FadeOutUI instance;

    private float timeStarted = -FADE_LENGTH;
    private bool targetVisible;

    private Image self;

    float GetNormalisedTime()
    {
        return (Time.time - timeStarted) / FADE_LENGTH;
    }
    float GetRemainingTime()
    {
        return FADE_LENGTH - (Time.time - timeStarted);
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
    public static float FadeIn()
    {
        if (!instance.targetVisible)
        {
            instance.BeginToggleFade();
        }
        return instance.GetRemainingTime();
    }
    /// <returns>How long the fade will take</returns>
    public static float FadeOut()
    {
        if (instance.targetVisible)
        {
            instance.BeginToggleFade();
        }
        return instance.GetRemainingTime();
    }

    void BeginToggleFade()
    {
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

        if (!targetVisible)
        {
            gameObject.SetActive(false);
        }
    }
}
