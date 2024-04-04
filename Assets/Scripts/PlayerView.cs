using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Orientation Settings")]
    //[SerializeField] private float orientationSpeedLimitMouse = 1.0f;
    [SerializeField, Range(10, 80)] private float verticalUpViewLimit = 80.0f;
    [SerializeField, Range(10, 80)] private float verticalDownViewLimit = 80.0f;
    [SerializeField, Range(0, 160)] private float horizontalViewLimit = 160.0f;
    [Header("Aiming Settings")]
    [SerializeField] private float orientationSpeed = 1.0f;
    [SerializeField] private float zoom = 0.5f;
    [SerializeField] private int zoomTime = 10;
    [SerializeField] private Camera thisCamera;
    [SerializeField, Range(0, 100)] private int fieldOfViewZoom = 30;
    private bool isZoomed = false;
    private readonly int halfCircleDegrees = 180;
    private float upperViewLimit;
    private float lowerViewLimit;
    private float rightViewLimit;
    private float leftViewLimit;
    private readonly int standardFieldOfView = 60;
    private int finalLayerMask;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        int fullCircleDegrees = 360;
        upperViewLimit = fullCircleDegrees - verticalUpViewLimit;
        lowerViewLimit = verticalDownViewLimit;
        rightViewLimit = fullCircleDegrees - horizontalViewLimit;
        leftViewLimit = horizontalViewLimit;
        int aimHelperLayerMask = 1 << LayerMask.NameToLayer("AimHelper");
        finalLayerMask = ~aimHelperLayerMask;
    }

    void Update()
    {
        HandleOrientation();
        isZoomed = Input.GetMouseButton(1);
        if (Input.GetMouseButtonDown(0))
        {
            HandleAttack();
        }
    }

    private void HandleOrientation()
    {
        Vector3 currentRotation = transform.localEulerAngles;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector2 orientationVector = new Vector2(mouseX, mouseY).normalized * orientationSpeed;
        orientationVector = isZoomed ? orientationVector * zoom : orientationVector;
        int zoomedFieldOfView = isZoomed ? standardFieldOfView - fieldOfViewZoom : standardFieldOfView;
        thisCamera.fieldOfView = Mathf.Lerp(thisCamera.fieldOfView, zoomedFieldOfView, Time.deltaTime * zoomTime);
        /*if (orientationVector.magnitude > orientationSpeedLimitMouse) The system this will be build for, will always either give max or non input, so this is not needed
        {
            orientationVector = orientationVector.normalized * orientationSpeedLimitMouse;
        }*/

        float newRotationX = currentRotation.x - orientationVector.y;
        if (newRotationX < upperViewLimit && newRotationX > halfCircleDegrees)
        {
            newRotationX = upperViewLimit;
        }
        else if (newRotationX > lowerViewLimit && newRotationX < halfCircleDegrees)
        {
            newRotationX = lowerViewLimit;
        }

        float newRotationY = currentRotation.y + orientationVector.x;
        if (newRotationY > leftViewLimit && newRotationY < halfCircleDegrees)
        {
            newRotationY = leftViewLimit;
        }
        else if (newRotationY < rightViewLimit && newRotationY > halfCircleDegrees)
        {
            newRotationY = rightViewLimit;
        }

        transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, 0);
    }

    private void HandleAttack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, finalLayerMask) && hit.collider.CompareTag("Enemy") && hit.collider.GetComponent<ForceHitDetector>() != null) //TODO: This looks a bit ugly, maybe just focusing on the component is fine?
        {
            hit.collider.GetComponent<ForceHitDetector>().HitWithForce(ray.direction);
        }
    }
}
