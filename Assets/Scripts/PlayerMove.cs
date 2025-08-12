using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour
{
    public CharacterController controller;
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private float gravity = -9.81f;

    private List<Collider> picked = new List<Collider>();
    public float pickableRadius = 2f;
    public LayerMask pickableLayerMask;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        Collider[] pickableObjects = Physics.OverlapSphere(groundCheck.position, pickableRadius, pickableLayerMask);
        if (picked.Count != 0)
        {
            foreach (var gameObject in picked)
            {
                if (pickableObjects.Contains(gameObject.GetComponent<Collider>()))
                {
                    picked.Remove(gameObject);
                }
            }
        } else
        {
            picked = pickableObjects.ToList();
        }

        foreach (var gameObject in picked)
        {
            ItemManager.instance.PickUpItem(gameObject.GetComponent<Collider>());
            Debug.Log(gameObject.name);
        }

    }
}
