using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    public bool isMovingObjects = false;

    private float xRotation;
    private float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float currentSensX = isMovingObjects ? sensX / 2 : sensX;
        float currentSensY = isMovingObjects ? sensY / 2 : sensY;

        //mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * currentSensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * currentSensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // cam rotation and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
