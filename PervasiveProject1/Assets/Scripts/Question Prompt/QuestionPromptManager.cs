using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class QuestionPromptManager : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_InputField inputField;
    private static QuestionPromptManager instance;

    private QuestionPromptOperation current;

    void Awake()
    {
        instance = this;
    }
    public static QuestionPromptOperation Ask(string formattedPrompt) => instance.AskInternal(formattedPrompt);
    private QuestionPromptOperation AskInternal(string formattedPrompt)
    {
        group.blocksRaycasts = true;
        title.text = "What " + formattedPrompt + " make you wonder?";
        group.alpha = 0;
        inputField.text = "";
        group.DOFade(1, 0.5f);
        current = new();
        return current;
    }
    public void Submit()
    {
        if (current == null) return;
        current.Finish(inputField.text);
        group.DOFade(0, 0.5f);
        group.blocksRaycasts = false;
    }
}
