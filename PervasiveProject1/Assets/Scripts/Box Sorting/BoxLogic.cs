using System;
using System.Collections.Generic;
using UnityEngine;

public class BoxLogic : MonoBehaviour
{
    [SerializeField] private BoxAnimator animator;

    private bool selected;

    #region Collisions listener
    [Serializable]
    public struct MouseState
    {
        public bool hovering;
        public bool clicked;
        public MouseState(List<BoxCollisionDetector> detectors)
        {
            hovering = false;
            clicked = false;
            foreach (BoxCollisionDetector detector in detectors)
            {
                if (detector.Hovering) hovering = true;
                if (detector.Clicking) clicked = true;
            }
        }
        public static bool StartedHovering(MouseState prev, MouseState current) => !prev.hovering && current.hovering;
        public static bool StoppedHovering(MouseState prev, MouseState current) => prev.hovering && !current.hovering;
        public static bool StartedClicking(MouseState prev, MouseState current) => !prev.clicked && current.clicked;
    }
    private List<BoxCollisionDetector> detectors = new();
    public MouseState currentMouseState = new()
    {
        hovering = false,
        clicked = false
    };
    public void RegisterDetector(BoxCollisionDetector detector)
    {
        detectors.Add(detector);
    }
    public void DeregisterDetector(BoxCollisionDetector detector)
    {
        detectors.Remove(detector);
    }
    public void OnDetectorUpdate()
    {
        MouseState newState = new(detectors);
        if (MouseState.StartedHovering(currentMouseState, newState))
        {
            animator.OpenLid();
        }
        else if (MouseState.StoppedHovering(currentMouseState, newState))
        {
            if (!selected)
            {
                animator.CloseLid();
            }
        }
        if (MouseState.StartedClicking(currentMouseState, newState))
        {
            animator.OpenLid();
            selected = true;
        }
        currentMouseState = newState;
    }
    #endregion

    public void Initialise()
    {
        selected = false;
    }
}
