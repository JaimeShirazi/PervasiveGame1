using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TMP_Text))]
public class TextDisplayPosition : MonoBehaviour
{
    private TMP_Text text;
    private RectTransform RectTrans => (RectTransform)transform;

    private float fontSizeOrigin;
    private Vector2 anchoredOrigin;
    [SerializeField] private float fontSizeDelta = 2f;
    [SerializeField] private Vector2 anchoredDelta;
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        fontSizeOrigin = text.fontSize;
        anchoredOrigin = RectTrans.anchoredPosition;
    }
    public void Display(string target, float time)
    {
        text.alpha = 0;
        text.text = target;
        text.fontSize = fontSizeOrigin;
        RectTrans.anchoredPosition = anchoredOrigin;

        float fadeInTime = Mathf.Min(0.5f, time / 2f);
        float fadeOutTime = fadeInTime;
        float fadeHoldTime = time - fadeInTime - fadeOutTime;
        Sequence fadeSeq = DOTween.Sequence(text.DOFade(1, fadeInTime));
        if (fadeHoldTime > 0)
            fadeSeq.AppendInterval(fadeHoldTime);
        fadeSeq.Append(text.DOFade(0, fadeOutTime));

        Sequence seq = DOTween.Sequence(RectTrans.DOAnchorPos(anchoredOrigin + anchoredDelta, time));
        seq.Join(DOTween.To(() => text.fontSize, x => text.fontSize = x, fontSizeOrigin + fontSizeDelta, time));
    }
}
