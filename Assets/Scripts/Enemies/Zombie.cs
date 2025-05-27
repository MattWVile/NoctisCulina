using UnityEngine;

public class Zombie : Enemy
{
    private void Awake()
    {
        TotalHealth = 11f;
        Damage = 1f;
        MaxSpeed = 1f;
        CurrentSpeed = MaxSpeed;
        GetComponent<SpriteRenderer>().enabled = false;
        ScoreWhenColurChanged = 300;
        ScoreWhenKilled = 100;
    }
    public void SetSpeedToZero()
    {
        if (CurrentSpeed == 0f)
        {
            return; // Speed is already zero, no need to set it again
        }

        CurrentSpeed = 0f; // Set this zombie's speed to zero
    }
}
