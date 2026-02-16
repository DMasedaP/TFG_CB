using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float speed = 20f;
    private CharacterController controller;
    public Transform cameraTrf; // Asignar en editor

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
    }
    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Direcciones relativas a la cámara
        Vector3 forward = Vector3.ProjectOnPlane(cameraTrf.forward, Vector3.up).normalized;
        Vector3 right = cameraTrf.right;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveZ + right * moveX;

        controller.Move(move * speed * Time.deltaTime);
    }
}
