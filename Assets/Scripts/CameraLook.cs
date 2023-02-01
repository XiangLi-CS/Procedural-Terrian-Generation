using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField]
    public float sensitivity;       //camera sensitivity

    public Transform cameraBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;       //Lock mouse in the scene
    }


    void Update()
    {
        //Looking around by controlling monuse
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        //Field of view 180 degrees
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        cameraBody.Rotate(Vector3.up * mouseX);

    }
}
