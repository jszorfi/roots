using UnityEngine;

public class CameraController : MonoBehaviour
{
    // The movement speed of the camera
    public float cameraSpeed;

    // The smoothness of the camera movement
    public float smoothness;

    // Internal state of the camera controller
    private Camera m_Camera;
    private Vector3 m_TargetPosition;
    private Vector3 m_CameraVelocity;

    void Start()
    {
        m_Camera = Camera.main;
        m_TargetPosition = m_Camera.transform.position;
    }

    void FixedUpdate()
    {
        float heightFactor = 1.0f; //transform.position.z / 3.0f;

        if(Input.GetKey(KeyCode.W))
        {
            Debug.Log("Before set: " + m_TargetPosition);
            m_TargetPosition += new Vector3(0.0f, 1.0f, 0.0f) * cameraSpeed * heightFactor;
            Debug.Log("After set: " + m_TargetPosition);
        }

        if(Input.GetKey(KeyCode.S))
        {
            m_TargetPosition += new Vector3(0.0f, 1.0f, 0.0f) * -cameraSpeed * heightFactor;
        }

        if(Input.GetKey(KeyCode.A))
        {
            m_TargetPosition += new Vector3(1.0f, 0.0f, 0.0f) * -cameraSpeed * heightFactor;
        }

        if(Input.GetKey(KeyCode.D))
        {
            m_TargetPosition += new Vector3(1.0f, 0.0f, 0.0f) * cameraSpeed * heightFactor;
        }

        //Debug.Log("Before move: " + m_Camera.transform.position);
        m_Camera.transform.position = Vector3.SmoothDamp(m_Camera.transform.position, m_TargetPosition, ref m_CameraVelocity, smoothness);     
        //Debug.Log("After move: " + m_Camera.transform.position);
    }
}
