using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 200f;

    // Update is called once per frame
    void Update()
    {
        float moveDirection = Input.GetAxis("Vertical");
        float turnDirection = Input.GetAxis("Horizontal");

        Vector3 move = transform.forward * moveDirection * moveSpeed * Time.deltaTime;
        transform.position += move;

        transform.Rotate(0,turnDirection * turnSpeed * Time.deltaTime, 0);
    }
}
