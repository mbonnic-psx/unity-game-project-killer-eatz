using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamLook : MonoBehaviour
{
    [Header("Camera Settings")]
    public float sensitivity = 2.0f;
    public float smoothing = 2.0f;
    public float lookXLimit = 45.0f;
    public float rotationX = 0;
    public Transform cameraTransform;
    public Camera playerCamera;
    private Vector2 smoothMouseDelta;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MouseCameraLook();
    }

    public void MouseCameraLook()
    {
        // Get the mouse input
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Smooth the mouse input
        smoothMouseDelta.x = Mathf.Lerp(smoothMouseDelta.x, mouseDelta.x, 1f / smoothing);
        smoothMouseDelta.y = Mathf.Lerp(smoothMouseDelta.y, mouseDelta.y, 1f / smoothing);

        // Update the camera and player rotations
        transform.Rotate(Vector3.left * smoothMouseDelta.y * sensitivity);
        player.transform.Rotate(Vector3.up * smoothMouseDelta.x * sensitivity);

        // Clamp the vertical rotation to prevent upside-down camera
        float verticalRotation = transform.localEulerAngles.x;

        if (verticalRotation > 180)
        {
            verticalRotation -= 360;
        }

        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        transform.localEulerAngles = new Vector3(verticalRotation, transform.localEulerAngles.y, 0);

        //unlock the cursor by pressing Escape
        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }
}
