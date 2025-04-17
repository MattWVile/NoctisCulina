using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    private GameObject player;
    private Enemy enemy;
    private Transform rootTransform;
    private Rigidbody2D rootRigidbody2D;

    void Start()
    {
        player = GameObject.Find("Player");
        rootTransform = transform.root;
        enemy = rootTransform.gameObject.GetComponent<Enemy>();
        rootRigidbody2D = rootTransform.GetComponent<Rigidbody2D>();

        if (rootRigidbody2D == null)
        {
            Debug.LogError("Rigidbody2D component not found on the root object.");
        }
    }

    void Update() => MoveTowardsPlayer();

    private void MoveTowardsPlayer()
    {
        if (player != null && enemy != null && rootRigidbody2D != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rootRigidbody2D.velocity = direction * enemy.CurrentSpeed;
        }
    }
}
