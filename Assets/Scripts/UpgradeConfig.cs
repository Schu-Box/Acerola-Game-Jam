using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade Config")]
public class UpgradeConfig : ScriptableObject
{
   public string title;
   public string description;

   public UpgradeEffect upgradeEffect;
}

[System.Serializable]
public class UpgradeEffect
{
   public UpgradeType type;
   public float value;
}

[System.Serializable]
public enum UpgradeType
{
   JumpHeight = 10,
   
   MovementSpeed = 20,
   
   //Debuffs
   BrickSpawnRate = 1000,
   
   BrickFallSpeed = 1010,
}
