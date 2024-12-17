using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour {

    public float sensitivity;

    float xRotation = 0f;

    public Transform playerBody;

    public float topClamp;
    public float bottomClamp;
    void Start() {

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        float mouseX = Input.GetAxisRaw("MouseX") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("MouseY") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
