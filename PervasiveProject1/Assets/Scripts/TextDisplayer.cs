using UnityEngine;

public class TextDisplayer : MonoBehaviour
{
    [SerializeField] private TextDisplayPosition top, leftHigher, rightHigher, middle, leftLower, rightLower, bottom;
    public enum TextPosition
    {
        Top, LeftHigher, RightHigher, Middle, LeftLower, RightLower, Bottom
    }
    private TextDisplayPosition GetTextAtPosition(TextPosition position) => position switch
    {
        TextPosition.LeftHigher => leftHigher,
        TextPosition.RightHigher => rightHigher,
        TextPosition.Middle => middle,
        TextPosition.LeftLower => leftLower,
        TextPosition.RightLower => rightLower,
        TextPosition.Bottom => bottom,
        TextPosition.Top or _ => top
    };

    private static TextDisplayer instance;
    void Awake()
    {
        instance = this;
    }
    public static void Display(string text, TextPosition position, float time) => instance.DisplayInternal(text, position, time);
    private void DisplayInternal(string text, TextPosition position, float time)
    {
        TextDisplayPosition displayer = GetTextAtPosition(position);
        displayer.Display(text, time);
    }
}