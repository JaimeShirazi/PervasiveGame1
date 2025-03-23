using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxLogic : MonoBehaviour
{
    const float OFFSCREEN_HEIGHT = -10f;

    [SerializeField] private BoxAnimator animator;

    private bool interactable;
    private bool selected;
    private UnityAction<BoxLogic, bool> setHover;
    private UnityAction<BoxLogic> setSelected;

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
        if (selected || !interactable) return;

        MouseState newState = new(detectors);
        if (MouseState.StartedHovering(currentMouseState, newState))
        {
            animator.OpenLid();
            setHover.Invoke(this, true);
        }
        else if (MouseState.StoppedHovering(currentMouseState, newState))
        {
            animator.CloseLid();
            setHover.Invoke(this, false);
        }
        if (MouseState.StartedClicking(currentMouseState, newState))
        {
            animator.OpenLid();
            selected = true;
            setSelected.Invoke(this);
        }
        currentMouseState = newState;
    }
    #endregion

    public void Initialise(float targetX, UnityAction<BoxLogic, bool> setHover, UnityAction<BoxLogic> setSelected)
    {
        interactable = true;
        animator.CloseLidInstantly();
        selected = false;
        gameObject.SetActive(true);
        this.setHover = setHover;
        this.setSelected = setSelected;
        transform.position = new Vector3(targetX, OFFSCREEN_HEIGHT, 11.78f);
        transform.DOMove(new Vector3(targetX, -5.34f, 11.78f), 1f).SetEase(Ease.OutQuad);
    }
    public void ResetState()
    {
        transform.position = new Vector3(0, OFFSCREEN_HEIGHT, 11.78f);
        gameObject.SetActive(false);
    }
    public void FreezeInput()
    {
        interactable = false;
        currentMouseState = new()
        {
            hovering = false,
            clicked = false
        };
    }
    /// <summary>
    /// Closes the lid, intended to be called after the object has been moved inside the box
    /// </summary>
    public void TrapObject()
    {
        animator.CloseLid();
    }
}
