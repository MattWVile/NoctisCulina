using UnityEngine;
using Pathfinding;

public class Zombie : Enemy
{
    private void Awake()
    {
        TotalHealth = 11f;
        Damage = 1f;
        MaxSpeed = 1f;
        colourChangeDuration  =1.5f; // Set the color change duration for the zombie
        GetComponent<SpriteRenderer>().enabled = false;
        ScoreWhenColurChanged = 300;
        ScoreWhenKilled = 100;
        ResourceWhenKilled = 15; // Set resource reward for killing
        GetComponent<AIPath>().maxSpeed = MaxSpeed; // Set the AIPath max speed to match the zombie's max speed
        originalMaxSpeed = MaxSpeed; // Store the original max speed for slow effects
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            var destinationSetter = GetComponent<AIDestinationSetter>();
            if (destinationSetter != null)
            {
                destinationSetter.target = playerObj.transform;
            }
        }
    }
}
