using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public CharacterController controller;

    [SerializeField]
    public float speed;

    [SerializeField]
    public float climbSpeed;

    Vector3 velocity;

    void Update()
    {
        

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Moving camera
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += transform.up * climbSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position -= transform.up * climbSpeed * Time.deltaTime;
        }


    }
}

