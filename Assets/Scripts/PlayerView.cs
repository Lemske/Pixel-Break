using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Orientation Settings")]
    [SerializeField] private float mouseLimit = 1.0f;
    [SerializeField, Range(10, 80)] private float verticalUpViewLimit = 80.0f;
    [SerializeField, Range(10, 80)] private float verticalDownViewLimit = 80.0f;
    [SerializeField, Range(0, 160)] private float horizontalViewLimit = 160.0f;
    private int halfCircleDegrees = 180;
    private float upperViewLimit;
    private float lowerViewLimit;
    private float rightViewLimit;
    private float leftViewLimit;


    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        int circleDegrees = 360;
        upperViewLimit = circleDegrees - verticalUpViewLimit;
        lowerViewLimit = verticalDownViewLimit;
        rightViewLimit = circleDegrees - horizontalViewLimit;
        leftViewLimit = horizontalViewLimit;
    }

    void Update()
    {
        HandleLook();
        if (Input.GetMouseButtonDown(0))
        {
            HandleAttack();
        }
    }

    private void HandleLook()
    {
        Vector3 currentRotation = transform.localEulerAngles;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector2 orientationVector = new Vector2(mouseX, mouseY);
        if (orientationVector.magnitude > mouseLimit)
        {
            orientationVector = orientationVector.normalized * mouseLimit;
        }

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

        Debug.Log(newRotationY);
        transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, 0);
    }

    private void HandleAttack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Enemy") && hit.collider.GetComponent<ForceHitDetector>() != null) //TODO: This looks a bit ugly, maybe just focusing on the component is fine?
        {
            hit.collider.GetComponent<ForceHitDetector>().HitWithForce(ray.direction);
        }
        else
        {
            Debug.Log("Missed!");
        }
    }
}
