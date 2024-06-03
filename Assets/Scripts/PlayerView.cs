using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Orientation Settings")]
    [SerializeField, Range(10, 80)] private float verticalUpViewLimit = 80.0f;
    [SerializeField, Range(10, 80)] private float verticalDownViewLimit = 80.0f;
    [SerializeField, Range(0, 160)] private float horizontalViewLimit = 160.0f;
    [SerializeField] private GameObject limiterCube;
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
    public static List<Vector3>[] InBoundsVectors { get; private set; } //Listen, I know all these classes are messy, if i have time I'll clean up
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
        InBoundsVectors = LimiterCalculation();
    }

    void Update()
    {
        //TODO: There should be a aiming timer, so you can't zoom in and out all the time using the helper to find the best spot. Should not be a long timer though
        isZoomed = Input.GetMouseButton(1);
        HandleOrientation();
        if (Input.GetMouseButtonDown(0))
        {
            HandleAttack();
        }
        wasAimingLastFrame = (wasAimingLastFrame && isZoomed) || (isZoomed && !wasAimingLastFrame);
    }

    private void HandleOrientation()
    {
        Vector3 currentForward = transform.forward;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector2 orientation = new Vector2(mouseX, mouseY);
        // This zoom logic should probably be in a separate method / or class, yea... wuhuu ma dudes :D
        int zoomedFieldOfView = isZoomed ? standardFieldOfView - fieldOfViewZoom : standardFieldOfView;
        thisCamera.fieldOfView = Mathf.Lerp(thisCamera.fieldOfView, zoomedFieldOfView, Time.deltaTime * zoomTime);

        orientation = DirectionalVector8(orientation);
        if (isZoomed)
        {
            Vector2 zoomedVector = AimingOrientationHelper(orientation);
            orientation = orientationSpeed * Time.deltaTime * zoom * zoomedVector;
        }
        else
        {
            orientation = orientationSpeed * Time.deltaTime * orientation;
        }

        Vector3 currentRotation = transform.localEulerAngles;
        float newRotationX = currentRotation.x - orientation.y;
        if (newRotationX < upperViewLimit && newRotationX > halfCircleDegrees)
        {
            newRotationX = upperViewLimit;
        }
        else if (newRotationX > lowerViewLimit && newRotationX < halfCircleDegrees)
        {
            newRotationX = lowerViewLimit;
        }

        float newRotationY = currentRotation.y + orientation.x;
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

    private Vector2 AimingOrientationHelper(Vector2 orientationVector) //Might have been better do use the newer/next orientation instead of last frame, but... I've come this far
    {
        Vector3 currentPosition = transform.position; //Should alway be the same, so maybe this should be in the start method
        Vector3 direction = transform.forward;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, shootingDistance, aimHelperLayerMask);

        if (hits.Length == 0)
        {
            return orientationVector;
        }

        Vector3 bestPerpendicularVector = Vector3.zero;
        Vector3 bestReferencePoint = Vector3.zero;
        //Vector3 enemyHitPosition = Vector3.zero;
        float length = -1;
        foreach (RaycastHit hit in hits)
        {
            Vector3 colliderHitPosition = hit.collider.transform.position;
            Vector3 projection = Vector3.Project(colliderHitPosition - currentPosition, direction);
            Vector3 perpendicularVector = colliderHitPosition - currentPosition - projection;
            float perpendicularVectorLength = perpendicularVector.magnitude;

            Debug.DrawLine(colliderHitPosition, colliderHitPosition - perpendicularVector, Color.black); //Just wanna se the perpendicular line

            Vector3 oldProjection = Vector3.Project(colliderHitPosition - currentPosition, lastAimVector);
            Vector3 perpendicularVectorOld = colliderHitPosition - currentPosition - oldProjection;
            float perpendicularVectorLengthOld = perpendicularVectorOld.magnitude;
            if (perpendicularVectorLengthOld >= perpendicularVectorLength && (length < 0 || length > perpendicularVectorLength))
            {
                bestPerpendicularVector = perpendicularVector;
                bestReferencePoint = currentPosition + projection;
                length = perpendicularVectorLength;
                //enemyHitPosition = colliderHitPosition;
            }
        }
        if (bestReferencePoint != Vector3.zero && !wasAimingLastFrame)
        {
            if (length > distanceStage1)
            {
            }
            else if (length < distanceStage1 && length > distanceStage2)
            {
                FigureNameOutLater(bestPerpendicularVector, bestReferencePoint, 0.10f); //Stage percentage should be a editable value
            }
            else if (length < distanceStage2 && length > distanceStage3)
            {
                FigureNameOutLater(bestPerpendicularVector, bestReferencePoint, 0.25f);
            }
            else if (length < distanceStage3 && length > distanceStage4)
            {
                FigureNameOutLater(bestPerpendicularVector, bestReferencePoint, 0.50f);
            }
            else
            {
                FigureNameOutLater(bestPerpendicularVector, bestReferencePoint, 0.80f);
            }
        }

        Vector2 targetDirection2D = TransformTo2dSpace(bestReferencePoint, bestPerpendicularVector);
        float orientationDot = Vector2.Dot(targetDirection2D.normalized, orientationVector);
        if (orientationDot <= 0)
            return orientationVector;

        Vector2 vectorBetween = targetDirection2D - orientationVector;
        return (orientationVector + vectorBetween * orientationDot).normalized;
    }

    private void FigureNameOutLater(Vector3 direction, Vector3 startingPosition, float directionTravelPercentage)
    {
        Vector3 targetPosition = startingPosition + direction * directionTravelPercentage;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.localRotation = targetRotation;
    }

    private Vector2 TransformTo2dSpace(Vector3 referencePoint, Vector3 toTransform)
    {
        //Making a right handed coordinate system
        /* Decided to go with another approach, but saving in case i need inspiration later in life... like surely I will check this out again... right?
        Vector3 zDirection = transform.forward * -1 + referencePoint;
        Vector3 xDirection = transform.right + referencePoint;
        Vector3 yDirection = transform.up + referencePoint;
        Debug.DrawLine(referencePoint, zDirection, Color.blue);
        Debug.DrawLine(referencePoint, xDirection, Color.red);
        Debug.DrawLine(referencePoint, yDirection, Color.green);
        */
        Vector3 ex = Vector3.Cross(transform.up, transform.forward).normalized;
        Vector3 ey = Vector3.Cross(transform.forward, ex).normalized;
        Debug.DrawLine(referencePoint, referencePoint + ex, Color.red);
        Debug.DrawLine(referencePoint, referencePoint + ey, Color.green);
        return new Vector2(Vector3.Dot(toTransform, ex), Vector3.Dot(toTransform, ey));
    }

    private Vector2 DirectionalVector8(Vector2 orientation)
    {
        if (orientation.magnitude < 0.1f)
        {
            return Vector2.zero;
        }

        int horizontalSign = orientation.x > 0 ? 1 : -1;
        int verticalSign = orientation.y > 0 ? 1 : -1;

        float biggestDot = orientation.x * horizontalSign; //horizontalDot but just naming it biggest since I start with it
        float verticalDot = orientation.y * verticalSign;
        float diagonalDot = 0.7f * (orientation.x * horizontalSign) + 0.7f * (orientation.y * verticalSign);

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

        return bestVector;
    }

    private void HandleAttack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, shootingDistance, finalLayerMask) && hit.collider.CompareTag("Enemy") && hit.collider.GetComponent<ForceHitDetector>() != null) //TODO: This looks a bit ugly, maybe just focusing on the component is fine?
        {
            hit.collider.GetComponent<ForceHitDetector>().HitWithForce(ray.direction);
        }
    }

    private List<Vector3>[] LimiterCalculation()
    {
        List<Vector3> limiterForwards = new List<Vector3>();
        List<Vector3> limiterVector = new List<Vector3>();
        int fullCircleDegrees = 360; //Jep, right now I'm doing this calc again for the gizmos... I know, I know... I'll fix it later
        float upperViewLimitTemp = fullCircleDegrees - verticalUpViewLimit;
        float lowerViewLimitTemp = verticalDownViewLimit;
        float rightViewLimitTemp = fullCircleDegrees - horizontalViewLimit;
        float leftViewLimitTemp = horizontalViewLimit;

        //The forwardvector is just for show, was what i thought! haha, me genius little man. This is why we dont clean code boooois and gurls
        limiterCube.transform.localRotation = Quaternion.Euler(0, rightViewLimitTemp, 0);
        limiterForwards.Add(limiterCube.transform.forward);
        limiterVector.Add(limiterCube.transform.right);
        limiterCube.transform.localRotation = Quaternion.Euler(0, leftViewLimitTemp, 0);
        limiterForwards.Add(limiterCube.transform.forward);
        limiterVector.Add(-limiterCube.transform.right);
        limiterCube.transform.localRotation = Quaternion.Euler(upperViewLimitTemp, 0, 0);
        limiterForwards.Add(limiterCube.transform.forward);
        limiterVector.Add(-limiterCube.transform.up);
        limiterCube.transform.localRotation = Quaternion.Euler(lowerViewLimitTemp, 0, 0);
        limiterForwards.Add(limiterCube.transform.forward);
        limiterVector.Add(limiterCube.transform.up);

        return new List<Vector3>[] { limiterForwards, limiterVector };
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 endPoint = transform.position + transform.forward * shootingDistance;
        Gizmos.DrawLine(transform.position, endPoint);

        List<Vector3>[] vectors = LimiterCalculation();
        List<Vector3> limiterForwards = vectors[0];
        foreach (Vector3 forward in limiterForwards)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + forward * shootingDistance);
        }

        List<Vector3> limiterVector = vectors[1];
        foreach (Vector3 vector in limiterVector)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + vector * shootingDistance);
        }
    }
}
