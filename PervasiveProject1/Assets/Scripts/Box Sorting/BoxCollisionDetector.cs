using UnityEngine;

public class BoxCollisionDetector : MonoBehaviour
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
    private void OnMouseEnter()
    {
        Hovering = true;
        logic.OnDetectorUpdate();
    }
    private void OnMouseExit()
    {
        Hovering = false;
        logic.OnDetectorUpdate();
    }
    private void OnMouseDown()
    {
        Clicking = true;
        logic.OnDetectorUpdate();
    }
    private void OnMouseUp()
    {
        Clicking = false;
        logic.OnDetectorUpdate();
    }
}
