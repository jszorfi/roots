using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 2;
    private bool downThisFrame = false;
    private float speed = 0.01f;

    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0, speed, 0), Space.World);
            downThisFrame = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-speed, 0, 0), Space.World);
            downThisFrame = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, -speed, 0), Space.World);
            downThisFrame = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(speed, 0, 0), Space.World);
            downThisFrame = true;
        }

        if (downThisFrame && speed < 0.7f)
        {
            speed += 0.0005f;
        }
        else
        {
            speed = 0.01f;
        }

        downThisFrame = false;
    }
}