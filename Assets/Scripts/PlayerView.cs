using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Orientation Settings")]
    [SerializeField] private float MouseLimit = 1.0f;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
        if (orientationVector.magnitude > MouseLimit)
        {
            orientationVector = orientationVector.normalized * MouseLimit;
        }
        float newRotationX = currentRotation.x - orientationVector.y;
        float newRotationY = currentRotation.y + orientationVector.x;
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
