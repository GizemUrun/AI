using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    float turnDirection = 2f;
    public float turnSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveLR = Input.GetAxis("Vertical");
        float moveUD = Input.GetAxis("Horizontal");

        
        
        Vector3 moveX = transform.forward * moveLR * moveSpeed * Time.deltaTime;
        Vector3 moveY = transform.right * moveUD * moveSpeed * Time.deltaTime;

        transform.position += moveX;
        transform.position += moveY;
        
        transform.Rotate(0, turnDirection * turnSpeed * Time.deltaTime, 0);
    }
}
