using UnityEngine;
using Pathfinding;

public class Zombie : Enemy
{
    private void Awake()
    {
        TotalHealth = 11f;
        Damage = 1f;
        MaxSpeed = 1f;
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
    public void SetSpeedToZero()
    {
        if (GetComponent<AIPath>().maxSpeed == 0f)
        {
            return; // Speed is already zero, no need to set it again
        }

        GetComponent<AIPath>().maxSpeed = 0f; // Set this zombie's speed to zero
    }
}
