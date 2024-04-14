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
    [SerializeField] private float shootingDistance = 100.0f;
    //TODO: don't really like this way of doing it, so Ill revisit if time allows ----------
    [SerializeField] private float distanceStage1 = 1.0f;
    [SerializeField] private float distanceStage2 = 0.60f;
    [SerializeField] private float distanceStage3 = 0.40f;
    [SerializeField] private float distanceStage4 = 0.20f;
    // ----------------------------------------------------------------------------
    private bool isZoomed = false;
    private bool wasAimingLastFrame = false;
    private Vector3 lastAimVector;
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
        //TODO: There should be a aiming timer, so you can't zoom in and out all the time using the helper to find the best spot. Should not be a long timer though
        isZoomed = Input.GetMouseButton(1);
        wasAimingLastFrame = (wasAimingLastFrame && isZoomed) || (isZoomed && !wasAimingLastFrame);
        HandleOrientation();
        if (Input.GetMouseButtonDown(0))
        {
            HandleAttack();
        }
    }

    private void HandleOrientation()
    {
        Vector3 currentForward = transform.forward;
        Vector3 currentRotation = transform.localEulerAngles;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector2 orientationVector = new Vector2(mouseX, mouseY);

        // This zoom logic should probably be in a separate method / or class, yea... wuhuu ma dudes :D
        int zoomedFieldOfView = isZoomed ? standardFieldOfView - fieldOfViewZoom : standardFieldOfView;
        thisCamera.fieldOfView = Mathf.Lerp(thisCamera.fieldOfView, zoomedFieldOfView, Time.deltaTime * zoomTime);

        // Very beautiful code incoming
        if (orientationVector.magnitude < 0.1f)
        {
            return;
        }

        int horizontalSign = orientationVector.x > 0 ? 1 : -1;
        int verticalSign = orientationVector.y > 0 ? 1 : -1;

        float biggestDot = orientationVector.x * horizontalSign; //horizontalDot but just naming it biggest since I start with it
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

        if (isZoomed)
        {
            ;
            AimingOrientationHelper(bestVector);
        }

        orientationVector = bestVector * orientationSpeed;
        // Very beautiful code done

        orientationVector = isZoomed ? orientationVector * zoom : orientationVector;

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
        lastAimVector = currentForward;
    }

    private void AimingOrientationHelper(Vector2 orientationVector) //TODO: might not need orientationVector
    {
        Vector3 currentPosition = transform.position; //Should alway be the same, so maybe this should be in the start method
        Vector3 direction = transform.forward;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, shootingDistance, aimHelperLayerMask);

        if (hits.Length == 0)
        {
            return;
        }

        Vector3 bestPerpendicularVector = Vector3.zero;
        Vector3 bestPerpendicularPoint = Vector3.zero;
        float length = -42069;
        foreach (RaycastHit hit in hits)
        {
            Vector3 colliderHitPosition = hit.collider.transform.position;
            Vector3 projection = Vector3.Project(colliderHitPosition - currentPosition, direction);
            Vector3 perpendicularVector = colliderHitPosition - currentPosition - projection;
            float perpendicularVectorLength = perpendicularVector.magnitude;

            Debug.DrawLine(colliderHitPosition, colliderHitPosition - perpendicularVector, Color.red); //Just wanna se the perpendicular line


            Vector3 oldProjection = Vector3.Project(colliderHitPosition - currentPosition, lastAimVector);
            Vector3 perpendicularVectorOld = colliderHitPosition - currentPosition - oldProjection;
            float perpendicularVectorLengthOld = perpendicularVectorOld.magnitude;
            if (perpendicularVectorLengthOld > perpendicularVectorLength && (length == -42069 || length > perpendicularVectorLength))
            {
                bestPerpendicularVector = perpendicularVector;
                bestPerpendicularPoint = currentPosition + projection;
                length = perpendicularVectorLength;
            }
        }
        if (bestPerpendicularPoint != Vector3.zero)
        {
            Debug.DrawLine(bestPerpendicularPoint, Vector3.up * 10, Color.green);
            Debug.Log("Length: " + length);
            if (length > distanceStage1)
            {
                Debug.Log("Stage 0");
            }
            else if (length < distanceStage1 && length > distanceStage2)
            {
                Debug.Log("Stage 1");
            }
            else if (length < distanceStage2 && length > distanceStage3)
            {
                Debug.Log("Stage 2");
            }
            else if (length < distanceStage3 && length > distanceStage4)
            {
                Debug.Log("Stage 3");
            }
            else
            {
                Debug.Log("Stage 4");
            }

        }
    }

    private void FigureNameOutLater(Vector3 direction, Vector3 startingPosition)
    {

    }


    /*Remember this for aim helper, might be useful

        Vector3 aimingDirection = // your aiming direction vector
        Quaternion targetRotation = Quaternion.LookRotation(aimingDirection);
        transform.rotation = targetRotation; 

    */

    private void HandleAttack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, shootingDistance, finalLayerMask) && hit.collider.CompareTag("Enemy") && hit.collider.GetComponent<ForceHitDetector>() != null) //TODO: This looks a bit ugly, maybe just focusing on the component is fine?
        {
            hit.collider.GetComponent<ForceHitDetector>().HitWithForce(ray.direction);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 endPoint = transform.position + transform.forward * shootingDistance;
        Gizmos.DrawLine(transform.position, endPoint);
    }
}
