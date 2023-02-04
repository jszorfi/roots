using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool downThisFrame  = false;
    private float speed         = 0.03f;
    private Vector3 dir;
    private readonly Vector3 up    = new Vector3( 0,  1,  0);
    private readonly Vector3 down  = new Vector3( 0, -1,  0);
    private readonly Vector3 left  = new Vector3(-1,  0,  0);
    private readonly Vector3 right = new Vector3( 1,  0,  0);

    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.W))
        {
            dir = up;
            downThisFrame = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir = left;
            downThisFrame = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir = down;
            downThisFrame = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            
            dir = right;
            downThisFrame = true;
        }

        if (downThisFrame)
        {
            transform.Translate(speed*dir, Space.World);

            if(speed < 0.7f)
            {
                speed += 0.01f;
            }
        }
        else
        {
            speed = 0.03f;
        }

        downThisFrame = false;
    }
}