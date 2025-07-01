using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Towers/TowerData", order = 1)]
public class TowerData : ScriptableObject
{
    public string towerName;
    public GameObject towerPrefab;
    public int cost;
    // Add more fields as needed (icon, description, etc)
}
