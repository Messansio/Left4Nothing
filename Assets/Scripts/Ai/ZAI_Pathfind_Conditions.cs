using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZAI_Pathfind_Conditions : MonoBehaviour
{
    private bool isTargetOnObstacle = false;
    private bool CheckIfPlayerOnObstacle()  //returns true if player is on top of a possible obstacle ( like a wall )
    {
        return isTargetOnObstacle;
    }

    private bool isTargetBehindDoor = false;
    private bool CheckIfPlayerBehindDoor()  //returns true if player is behind a door
    {
        return isTargetBehindDoor;
    }


    private bool isTargetOverPit = false;
    private bool CheckIfPlayerOverPit() //returns true if player is on the other side of a possible pit
    {
        return isTargetOverPit;
    }
}
