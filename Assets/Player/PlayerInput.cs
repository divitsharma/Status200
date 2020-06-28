using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Default,
    Turning,
    OnTrigger,
    TriggerExit
}

public class PlayerInput : MonoBehaviour
{
    [SerializeField] float lateralSpeed;
    [SerializeField] float forwardSpeed;
    [SerializeField] float lateralRange;

    CharacterController controller;

    [SerializeField] int turnsPerSecond = 2;
    float degreesTurned = 0.0f;
    float targetAngle;

    State state;
    Router intersectingRouter;
    public Transform railOrigin;

    ScoreManager scoreManager;
    float elapsedTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger");
        state = State.OnTrigger;
        intersectingRouter = other.gameObject.GetComponent<Router>();
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exited");
        // if we've exited the trigger, we're going straight
        state = State.TriggerExit;
        //intersectingRouter = null;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        state = State.Default;
        //railOrigin.position = transform.position;
        //railOrigin.rotation = transform.rotation;
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        forwardSpeed += elapsedTime * 0.005f;


        Vector3 deltaMove = transform.forward * forwardSpeed;
        Vector3 lateralMove = Vector3.zero;

        switch (state)
        {
            case State.Default:
                {
                    float horizontal = Input.GetAxisRaw("Horizontal");
                    //float vertical = Input.GetAxisRaw("Vertical");

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
                }
                break;

            case State.Turning:
                // move according to target forward
                deltaMove = railOrigin.forward * forwardSpeed;

                float angle = turnsPerSecond * targetAngle * Time.deltaTime;
                if (Mathf.Abs(degreesTurned + angle) >= 90f)
                {
                    // rotate the leftover amount and exit
                    angle = targetAngle - degreesTurned;
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
                {
                    // keep moving towards center
                    Vector3 toCenter = Vector3.Project(transform.position - railOrigin.position, railOrigin.right);
                    lateralMove = -toCenter;

                    TurnDirection direction = TurnDirection.None;

                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        direction = TurnDirection.Left;
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        direction = TurnDirection.Right;
                    }

                    if (direction != TurnDirection.None)
                    {
                        bool success = Turn(intersectingRouter, direction);
                        state = State.Turning;
                        
                        if (success)
                        {
                            scoreManager.PlayerScored();
                        }
                        else
                        {
                            scoreManager.FailedTurn();
                        }
                    }
                }
                break;
            case State.TriggerExit:
                {
                    TurnDirection direction = TurnDirection.Straight;
                    bool success = Turn(intersectingRouter, direction);
                    state = State.Default;
                    if (success)
                    {
                        scoreManager.PlayerScored();
                    }
                    else
                    {
                        scoreManager.FailedTurn();
                    }
                }
                break;
            default:
                break;
        }

        deltaMove += lateralMove * lateralSpeed;
        controller.Move(deltaMove * Time.deltaTime);

    }

    bool Turn(Router router, TurnDirection direction)
    {
        Transform target = router.GetSocket(direction);

        // determine sign
        targetAngle = Vector3.Dot(transform.right, target.forward) * 90f;

        controller.enabled = false;
        transform.position = target.position;
        //transform.rotation = target.rotation;
        railOrigin = target;
        controller.enabled = true;

        return router.CorrectTurn == direction;
    }
}
