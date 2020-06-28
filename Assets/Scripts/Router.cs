using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnDirection
{
    None = -1,
    Left = 0,
    Right,
    Straight,
}

public class Router : MonoBehaviour
{
    public Transform leftSocket;
    public Transform rightSocket;
    public Transform straightSocket;

    public Transform[] pipes = new Transform[3];
    public Material correctColor;

    public TurnDirection CorrectTurn;
    
    void Start()
    {
        int r = Random.Range(0, 1);
        CorrectTurn = (TurnDirection)r;
        pipes[r].GetComponent<MeshRenderer>().material = correctColor;
    }

    public Transform GetSocket(TurnDirection direction)
    {
        Transform ret = null;
        switch (direction)
        {
            case TurnDirection.Left:
                ret = leftSocket;
                break;
            case TurnDirection.Right:
                ret = rightSocket;
                break;
            case TurnDirection.Straight:
                ret = straightSocket;
                break;
            default:
                break;
        }
        return ret;
    }

}
