using System.Collections;
using UnityEngine;

public class PlayerMeleeAttackController : MonoBehaviour
{
    public GameObject topCollider;
    public GameObject leftCollider;
    public GameObject downCollider;
    public GameObject rightCollider;

    private bool canAttack = true;
    private float attackCooldown = 3.6f;

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
    Collider2D attackCollider = colliderGameObject.GetComponent<PolygonCollider2D>();

    // Get the SpriteRenderer from the parent GameObject (the player)
    SpriteRenderer playerRenderer = transform.parent.GetComponent<SpriteRenderer>();
    if (playerRenderer == null)
    {
        Debug.LogError("SpriteRenderer not found on the parent GameObject.");
        yield break;
    }

    // Enable the collider for the attack
    attackCollider.enabled = true;
    yield return new WaitForSeconds(0.1f);
    attackCollider.enabled = false;

    // Pulse the player's sprite to indicate cooldown
    yield return ColorChangeUtility.PulseAlpha(this, playerRenderer, attackCooldown - 0.1f, 2f);

    canAttack = true;
}


}
