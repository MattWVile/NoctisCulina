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
    }
}
