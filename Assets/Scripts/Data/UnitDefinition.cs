using UnityEngine;

[CreateAssetMenu(menuName = "Biotonic/Unit Definition", fileName = "NewUnit")]
public class UnitDefinition : ScriptableObject
{
    public string   UnitType;     // "Light", "Ranged", … kept in sync with backend enum
    public int      EnergyCost;
    public int      BiomassCost;
    public int      GeneSeedCost;
    public int      Attack;
    public int      HP;
    public GameObject Prefab;     // visual prefab loaded at runtime
}