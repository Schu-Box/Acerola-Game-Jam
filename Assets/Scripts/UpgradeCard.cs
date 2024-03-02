using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI cardDescription;

    public UpgradeConfig upgradeConfig;
    public void Setup(UpgradeConfig config)
    {
        upgradeConfig = config;
        
        // cardImage.sprite = config.sprite;
        cardDescription.text = config.description;
    }
}
