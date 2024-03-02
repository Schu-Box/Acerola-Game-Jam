using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    public UpgradeCard positiveUpgradeCard;
    public UpgradeCard negativeUpgradeCard;

    public void Setup(UpgradeConfig positiveConfig, UpgradeConfig negativeConfig)
    {
        positiveUpgradeCard.Setup(positiveConfig);
        negativeUpgradeCard.Setup(negativeConfig);
    }
}
