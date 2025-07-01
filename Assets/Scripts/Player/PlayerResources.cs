using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public static PlayerResources Instance { get; private set; }
    [SerializeField] private int resources = 0;

    public int Resources => resources;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddResources(int amount)
    {
        resources += amount;
        // TODO: Update UI if needed
    }

    public bool SpendResources(int amount)
    {
        if (resources >= amount)
        {
            resources -= amount;
            // TODO: Update UI if needed
            return true;
        }
        return false;
    }
}
