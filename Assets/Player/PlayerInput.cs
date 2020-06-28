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

    [SerializeField] float degreesPerSecond = 180f;
    float degreesTurned = 0.0f;

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

        switch (state)
        {
            case State.Default:
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
                break;

            case State.Turning:
                deltaMove = railOrigin.forward * forwardSpeed;

                float angle = degreesPerSecond * Time.deltaTime;
                if (degreesTurned + angle >= 90f)
                {
                    // rotate the leftover amount and exit
                    angle = 90f - degreesTurned;
                    degreesTurned = 0f;
                    state = State.Default;
                }
                else
                {
                    // rotate full amount
                    degreesTurned += angle;
                }
                transform.Rotate(Vector3.up, angle);
                break;

            case State.OnTrigger:

                if (Input.GetKeyDown(KeyCode.A))
                {
                    Turn(intersectingRouter.leftSocket);
                    state = State.Turning;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    Turn(intersectingRouter.rightSocket);
                    state = State.Turning;
                }
                break;
            default:
                break;
        }

        controller.Move(deltaMove * Time.deltaTime);

    }

    void Turn(Transform target)
    {
        // determine sign
        Debug.Log(Vector3.Dot(transform.right, target.right));

        controller.enabled = false;
        transform.position = target.position;
        //transform.rotation = target.rotation;
        railOrigin = target;
        controller.enabled = true;
    }
}
