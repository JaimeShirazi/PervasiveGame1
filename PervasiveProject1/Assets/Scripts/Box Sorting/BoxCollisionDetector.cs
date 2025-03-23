using UnityEngine;
using UnityEngine.EventSystems;

public class BoxCollisionDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private BoxLogic logic;
    public bool Hovering { get; private set; }
    public bool Clicking { get; private set; }
    private void OnEnable()
    {
        logic.RegisterDetector(this);
    }
    private void OnDisable()
    {
        logic.DeregisterDetector(this);
    }
    public void OnPointerEnter(PointerEventData _)
    {
        Hovering = true;
        logic.OnDetectorUpdate();
    }
    public void OnPointerExit(PointerEventData _)
    {
        Hovering = false;
        logic.OnDetectorUpdate();
    }
    public void OnPointerDown(PointerEventData _)
    {
        Clicking = true;
        logic.OnDetectorUpdate();
    }
    public void OnPointerUp(PointerEventData _)
    {
        Clicking = false;
        logic.OnDetectorUpdate();
    }
}
