using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Default,
    Turning,
    OnTrigger
}

public class PlayerInput : MonoBehaviour
{
    [SerializeField] float lateralSpeed;
    [SerializeField] float forwardSpeed;
    [SerializeField] float lateralRange;

    CharacterController controller;

    [SerializeField] float turnOverSeconds = 1f;
    float timeTurning = 0.0f;

    State state;
    Router intersectingRouter;
    public Transform railOrigin;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        state = State.OnTrigger;
        intersectingRouter = other.gameObject.GetComponent<Router>();
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exited");
        state = State.Default;
        intersectingRouter = null;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        state = State.Default;
        //railOrigin.position = transform.position;
        //railOrigin.rotation = transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 deltaMove = transform.forward * forwardSpeed;
        if (state == State.Turning)
        {
            timeTurning += Time.deltaTime;
            //float angle = Mathf.LerpAngle(0f, 90f, timeTurning / turnOverSeconds);
            //Debug.Log(angle);
            transform.Rotate(Vector3.up, 0.1f);
            if (timeTurning >= turnOverSeconds)
            {
                timeTurning = 0f;
            }
        }
        else if (state == State.Default)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            //float vertical = Input.GetAxisRaw("Vertical");

            Vector3 lateralMove = Vector3.zero;
            Vector3 toCenter = Vector3.Project(transform.position - railOrigin.position, railOrigin.right);
            // go back to origin if not holding down any keys
            if (horizontal == 0f)
            {
                lateralMove = -toCenter;// new Vector3(-transform.localPosition.x, 0.0f, 0.0f);
            }
            else
            {
                // clamp position
                if (toCenter.magnitude < lateralRange)
                {
                    lateralMove = transform.right * horizontal;
                }
            }

            deltaMove += lateralMove * lateralSpeed;
        }
        else if (state == State.OnTrigger)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Turn(intersectingRouter.leftSocket);
                state = State.Default;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Turn(intersectingRouter.rightSocket);
                state = State.Default;
            }
        }

        controller.Move(deltaMove * Time.deltaTime);

    }

    void Turn(Transform target)
    {
        controller.enabled = false;
        transform.position = target.position;
        transform.rotation = target.rotation;
        railOrigin = target;
        controller.enabled = true;
    }
}
