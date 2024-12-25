using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float senstivity;

    float xRotation = 0f;

    public Transform playerBody;

    public float TopClamp = -60f;
    public float BottomClamp = 60f;

    public PauseMenu pauseMenu;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        if (pauseMenu.isPaused) {
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
        }

        float mouseX = Input.GetAxis("Mouse X") * senstivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * senstivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, TopClamp, BottomClamp);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
