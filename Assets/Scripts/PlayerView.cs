using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Orientation Settings")]
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
    private int aimHelperLayerMask;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        int fullCircleDegrees = 360;
        upperViewLimit = fullCircleDegrees - verticalUpViewLimit;
        lowerViewLimit = verticalDownViewLimit;
        rightViewLimit = fullCircleDegrees - horizontalViewLimit;
        leftViewLimit = horizontalViewLimit;
        aimHelperLayerMask = 1 << LayerMask.NameToLayer("AimHelper");
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
        Vector2 orientationVector = new Vector2(mouseX, mouseY);

        // Very beautiful code incoming
        if (orientationVector.magnitude < 0.1f)
        {
            return;
        }

        int horizontalSign = orientationVector.x > 0 ? 1 : -1;
        int verticalSign = orientationVector.y > 0 ? 1 : -1;

        float biggestDot = orientationVector.x * horizontalSign; //horizontalDot but just naming it biggest since i start with it
        float verticalDot = orientationVector.y * verticalSign;
        float diagonalDot = 0.7f * (orientationVector.x * horizontalSign) + 0.7f * (orientationVector.y * verticalSign);

        Vector2 bestVector = new Vector2(horizontalSign, 0);

        if (verticalDot > biggestDot)
        {
            biggestDot = verticalDot;
            bestVector = new Vector2(0, verticalSign);
        }

        if (diagonalDot > biggestDot)
        {
            bestVector = new Vector2(0.7f * horizontalSign, 0.7f * verticalSign);
        }

        orientationVector = bestVector * orientationSpeed;
        // Very beautiful code done

        orientationVector = isZoomed ? orientationVector * zoom : orientationVector;
        int zoomedFieldOfView = isZoomed ? standardFieldOfView - fieldOfViewZoom : standardFieldOfView;
        thisCamera.fieldOfView = Mathf.Lerp(thisCamera.fieldOfView, zoomedFieldOfView, Time.deltaTime * zoomTime);

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
