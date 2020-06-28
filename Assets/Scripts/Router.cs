using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnDirection
{
    Left = 0,
    Right,
    Straight,
    Up,
    Down
}

public class Router : MonoBehaviour
{
    public Transform leftSocket;
    public Transform rightSocket;

    public TurnDirection CorrectTurn;
    // Start is called before the first frame update
    void Start()
    {
        //int r = Random.Range(0, 1);
    }

}
