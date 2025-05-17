using System.Collections;
using UnityEngine;

public class PlayerMeleeAttackController : MonoBehaviour
{
    public GameObject topCollider;
    public GameObject leftCollider;
    public GameObject downCollider;
    public GameObject rightCollider;

    private bool canAttack = true;
    private float attackCooldown = 2f;

    void Update()
    {
        HandleMeleeAttack();
    }

    private void HandleMeleeAttack()
    {
        if (canAttack)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(PerformAttack(topCollider));
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(PerformAttack(leftCollider));
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(PerformAttack(downCollider));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(PerformAttack(rightCollider));
            }
        }
    }

    private IEnumerator PerformAttack(GameObject colliderGameObject)
    {
        canAttack = false;
        Collider2D attackCollider = colliderGameObject.GetComponent<PolygonCollider2D>(); // Get the collider component
        attackCollider.enabled = true; // Enable the collider for the attack
        yield return new WaitForSeconds(0.1f); // Keep the collider active for a short duration
        attackCollider.enabled = false; // Disable the collider after the attack
        yield return new WaitForSeconds(attackCooldown - 0.1f); // Wait for the cooldown
        canAttack = true;
    }
}
