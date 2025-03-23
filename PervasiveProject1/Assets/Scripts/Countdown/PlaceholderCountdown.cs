using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlaceholderCountdown : MonoBehaviour, ICountdown
{
    private Image image;
    private float timeStarted;
    private float length;
    private bool animating;

    void Awake()
    {
        image = GetComponent<Image>();
    }
    void Start()
    {
        GameStateManager.SetCountdown(this);
    }
    public void Begin(float length)
    {
        this.length = length;
        timeStarted = Time.time;
        if (!animating)
            StartCoroutine(Countdown());
    }
    IEnumerator Countdown()
    {
        animating = true;
        while (Time.time - timeStarted < length)
        {
            image.fillAmount = 1f - ((Time.time - timeStarted) / length);
            yield return null;
        }
        image.fillAmount = 0;
        animating = false;
    }
}
