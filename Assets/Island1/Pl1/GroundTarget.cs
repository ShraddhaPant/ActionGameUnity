using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTarget : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        // Raycast downward from player to find exact ground position
        RaycastHit hit;
        if (Physics.Raycast(player.position, Vector3.down, out hit, 100f))
        {
            transform.position = new Vector3(
                player.position.x,
                hit.point.y,        // exact ground height under player
                player.position.z
            );
        }
    }
}