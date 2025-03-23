using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectFreeInspector : MonoBehaviour
{
    private static ObjectFreeInspector instance;
    public static void BeginInspecting(GameObject target)
    {
        target.transform.position = new Vector3(0, 4f, 0); //A point high enough offscreen
        target.transform.DOMove(Vector3.zero, 2f).SetEase(Ease.OutQuad);
        instance.target = target.transform;
    }
    public static void EndInspection()
    {
        instance.target = null;
    }

    private Transform target;
    [SerializeField] private CameraManager cam;

    float zoomBuffer = 0.5f;

    class AverageBuffer
    {
        public Vector2[] lastPositions;
        const int COUNT = 5;
        int pos;
        public AverageBuffer()
        {
            lastPositions = new Vector2[COUNT];
        }
        public void SetLatest(Vector2 newPos)
        {
            lastPositions[pos] = newPos;
            pos++;
            if (pos >= COUNT) pos = 0;
        }
        public Vector2 Average
        {
            get
            {
                Vector2 average = Vector2.zero;
                for (int i = 0; i < COUNT; i++)
                {
                    average += lastPositions[i];
                }
                average /= COUNT;
                return average;
            }
        }
    }

    private AverageBuffer buffer = new();

    private bool click;
    private Vector2 pos;
    private Vector2 lastPos;
    private Vector2 smoothed;
    void Awake()
    {
        instance = this;

        InputAction moveAction = InputSystem.actions.FindActionMap("Player").FindAction("Move");
        InputAction clickAction = InputSystem.actions.FindActionMap("Player").FindAction("Click");
        InputAction scrollAction = InputSystem.actions.FindActionMap("Player").FindAction("Scroll");

        moveAction.performed += OnMovePerformed;

        clickAction.started += OnClickStarted;
        clickAction.canceled += OnClickCancelled;

        scrollAction.performed += OnScrollPerformed;
        scrollAction.canceled += OnScrollCancelled;
    }
    void OnMovePerformed(InputAction.CallbackContext context)
    {
        pos = context.ReadValue<Vector2>();
    }
    void OnClickStarted(InputAction.CallbackContext context)
    {
        click = true;
    }
    void OnClickCancelled(InputAction.CallbackContext context)
    {
        click = false;
        smoothed = buffer.Average;
    }
    void OnScrollPerformed(InputAction.CallbackContext context)
    {
        zoomBuffer -= context.ReadValue<float>() * 0.05f;
        zoomBuffer = Mathf.Clamp01(zoomBuffer);
    }
    void OnScrollCancelled(InputAction.CallbackContext context)
    {
        
    }
    private void Update()
    {
        if (target == null) return;

        Vector2 realTarget = new Vector2(pos.y - lastPos.y, -pos.x + lastPos.x) * 0.4f;

        buffer.SetLatest(realTarget);

        if (click)
        {
            smoothed = realTarget;
        }
        else
        {
            smoothed = smoothed.normalized * Mathf.Lerp(Mathf.Max(0, smoothed.magnitude - 0.01f), 0, Time.deltaTime * 10f);
        }

        target.Rotate(new Vector3(smoothed.x, smoothed.y, 0), Space.World);

        lastPos = pos;

        cam.SetParams(new CameraManager.CameraParams()
        {
            worldPos = new Vector3(0, 0, Mathf.Lerp(-6f, -5f, zoomBuffer)),
            eulerAngles = Vector3.zero,
            fov = Mathf.Lerp(20f, 50f, zoomBuffer),
            easeTime = 0.04f,
            index = CameraManager.CameraInfluencer.ObjectFreeInspector
        });
    }
}
