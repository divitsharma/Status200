using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LeaderboardData", menuName = "ScriptableObjects/LeaderboardData", order = 1)]
public class LeaderboardData : ScriptableObject
{
    public float best = 0f;
}
