using SpriteSlicer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CutterController : MonoBehaviour
{
    [SerializeField] private bool isSelected = false;

    public Transform circleCenter;
    public float circleRadius = 1f;

    public float[] allowedAngles = new float[] { 0, 45, 90, 135, 180, 225, 270, 315 };

    public float cutSpeed = 5f;
    private bool cutting = false;

    [SerializeField] private Slicer slicer;

    private Vector3 targetPosition;


    [Header("RawImage / RenderTexture")]
    public RawImage rawImage;
    Camera uiCameraCached = null;   
    Camera worldCamera = null; 

    public bool IsSelected { get { return isSelected; } set { isSelected = value; } }

    public static CutterController Instance { get { return instance; } }

    static CutterController instance;
    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        CutterUi.OnClicked += HandleCutterUIClicked;
    }
    void OnDisable()
    {
        CutterUi.OnClicked -= HandleCutterUIClicked;
    }

    void Start()
    {
        worldCamera = Camera.main;

        if (rawImage != null && rawImage.canvas != null)
        {
            if (rawImage.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                uiCameraCached = null;
            else
                uiCameraCached = rawImage.canvas.worldCamera;
        }
    }

    void Update()
    {
        if(!isSelected)
            return;

        if (cutting)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                cutSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                StopCut();
            }

            return;
        }

        Move();
    }

    void HandleCutterUIClicked(GameObject hitObject)
    {
        if(!isSelected)
            return;

        StartCut();
    }

    void Move()
    {
        Vector3 mousePosWorld = Utils.GetWorldPositionFromRawImage(
            rawImage, 
            uiCameraCached, 
            worldCamera, 
            Input.mousePosition);

        if (mousePosWorld == Vector3.positiveInfinity)
            return;

        mousePosWorld.z = 0f;

        Vector3 dir = (mousePosWorld - circleCenter.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float snappedAngle = FindClosestAngle(angle);
        float rad = snappedAngle * Mathf.Deg2Rad;
        Vector3 snappedDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);

        transform.position = circleCenter.position + snappedDir * circleRadius;
        transform.up = -snappedDir; 
    }
    void StartCut()
    {
        cutting = true;
        targetPosition = circleCenter.position + transform.up;
        slicer.Cutting = true;
    }

    void StopCut()
    {
        slicer.Cutting = false;
        cutting = false;
    }

    float FindClosestAngle(float currentAngle)
    {
        float closest = allowedAngles[0];
        float minDistance = Mathf.Abs(Mathf.DeltaAngle(currentAngle, closest));

        foreach (float a in allowedAngles)
        {
            float dist = Mathf.Abs(Mathf.DeltaAngle(currentAngle, a));
            if (dist < minDistance)
            {
                closest = a;
                minDistance = dist;
            }
        }

        return closest;
    }
}
