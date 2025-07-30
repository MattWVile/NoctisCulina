using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RangeController : MonoBehaviour
{
    [SerializeField] private bool autoScaleVisual = true;

    private SpriteRenderer visual;
    private Tower parentTower;

    private void Awake()
    {
        visual = GetComponent<SpriteRenderer>();
        parentTower = GetComponentInParent<Tower>();
    }

    public void SetRange(float range)
    {
        if (autoScaleVisual)
        {
            float diameter = range * 2f;
            transform.localScale = new Vector3(diameter, diameter, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parentTower is TeslaTower teslaTower)
        {
            teslaTower.OnRangeTriggerEnter(other);
        }
        else if (parentTower is PhotonCannonTower photonCannonTower)
        {
            photonCannonTower.OnRangeTriggerEnter(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (parentTower is TeslaTower teslaTower)
        {
            teslaTower.OnRangeTriggerExit(other);
        }
        else if (parentTower is PhotonCannonTower photonCannonTower)
        {
            photonCannonTower.OnRangeTriggerExit(other);
        }
    }
}