using UnityEngine;

public enum UpgradeType
{
    ShootRate,
    Damage,
    ProjectileSpeed,
    Penetration,
    MultiShot,
    PlayerSpeed,
    MaxHP,
    Regen,
    SlowEnemies,
    XPAttraction
}

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrades/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public UpgradeType type;
    public float value;
}
