using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] float lateralSpeed;
    [SerializeField] float forwardSpeed;

    CharacterController controller;

    bool turning = false;
    float timeTurning = 0.0f;
    [SerializeField] float turnOverSeconds = 1f;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (turning)
        {
            timeTurning += Time.deltaTime;
            //float angle = Mathf.LerpAngle(0f, 90f, timeTurning / turnOverSeconds);
            //Debug.Log(angle);
            transform.Rotate(Vector3.up, 0.1f);
            if (timeTurning >= turnOverSeconds)
            {
                timeTurning = 0f;
                turning = false;
            }
        }
        else
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 moveDir = transform.right * horizontal + transform.up * vertical;// new Vector3(horizontal, vertical, 0f);
            Vector3 deltaMove = moveDir * lateralSpeed + transform.forward * forwardSpeed;
            controller.Move(deltaMove * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                turning = true;
            }
        }
    }
}
