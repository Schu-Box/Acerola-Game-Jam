using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Debug")]
    public TextMeshProUGUI groundedText;
    public TextMeshProUGUI jumpFallingText;
    
    [Header("UI")]
    public GameObject gameOverUI;
    
    public TextMeshProUGUI experienceText;

    public GameObject levelUpUI;
    public UpgradeButton firstUpgradeButton;
    public UpgradeButton secondUpgradeButton;

    [Header("Shockwave")]
    public Transform shockwaveHolder;
    public GameObject shockwavePrefab;

    [Header("Carrot")]
    public Transform carrotHolder;
    public Carrot carrotPrefab;

    [Header("Upgrades")]
    public List<UpgradeConfig> positiveUpgrades;
    public List<UpgradeConfig> negativeUpgrades;
    
    private void Awake()
    {
        Instance = this;
        
        gameOverUI.SetActive(false);
        levelUpUI.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        
        gameOverUI.SetActive(true);
    }

    public void UpdateExperienceUI(int newExperience)
    {
        experienceText.text = newExperience.ToString();
    }

    public void DisplayLevelUp()
    {
        //TODO: Display dis

        Time.timeScale = 0f;
        
        levelUpUI.SetActive(true);
        
        UpgradeConfig positiveUpgrade = positiveUpgrades[Random.Range(0, positiveUpgrades.Count)];
        UpgradeConfig negativeUpgrade = negativeUpgrades[Random.Range(0, negativeUpgrades.Count)];

        firstUpgradeButton.Setup(positiveUpgrade, negativeUpgrade);
        
        UpgradeConfig secondPositiveUpgrade = positiveUpgrades[Random.Range(0, positiveUpgrades.Count)];
        while (secondPositiveUpgrade == positiveUpgrade)
        {
            secondPositiveUpgrade = positiveUpgrades[Random.Range(0, positiveUpgrades.Count)];
        }
        
        UpgradeConfig secondNegativeUpgrade = negativeUpgrades[Random.Range(0, negativeUpgrades.Count)];
        while (secondNegativeUpgrade == negativeUpgrade)
        {
            secondNegativeUpgrade = negativeUpgrades[Random.Range(0, negativeUpgrades.Count)];
        }
        
        secondUpgradeButton.Setup(secondPositiveUpgrade, secondNegativeUpgrade);
    }

    public void ChooseUpgrade(UpgradeButton upgradeButton)
    {
        Debug.Log("Upgrade Chosen");
        
        ApplyUpgrade(upgradeButton.positiveUpgradeCard.upgradeConfig);
        ApplyUpgrade(upgradeButton.negativeUpgradeCard.upgradeConfig);

        Time.timeScale = 1f;

        levelUpUI.SetActive(false);
    }

    public void ApplyUpgrade(UpgradeConfig upgradeConfig)
    {
        Debug.Log("Applying upgrade: " + upgradeConfig.title);

        switch (upgradeConfig.upgradeEffect.type)
        {
            case UpgradeType.JumpHeight:
                PlayerController.Instance.movementData.jumpForce *= upgradeConfig.upgradeEffect.value;
                break;
            
            case UpgradeType.MovementSpeed:
                PlayerController.Instance.movementData.movementSpeed *= upgradeConfig.upgradeEffect.value;
                break;
            
            case UpgradeType.BrickSpawnRate:
                BrickSpawner.Instance.timeBetweenSpawns /= upgradeConfig.upgradeEffect.value;
                break;
            
            case UpgradeType.BrickFallSpeed:
                BrickSpawner.Instance.brickGravityScale *= upgradeConfig.upgradeEffect.value;
                break;
            
            default:
                Debug.LogError("UPgrade type not found!");
                break;
        }
    }

    public void ApplyShockwave(Vector2 in_position)
    {
        Shockwave newShockwave = Instantiate(shockwavePrefab, shockwaveHolder).GetComponentInChildren<Shockwave>();
        newShockwave.Spawn(in_position);
    }

    public void SpawnCarrot(Vector2 in_position)
    {
        Carrot newCarrot = Instantiate(carrotPrefab, carrotHolder);
        newCarrot.Spawn(in_position);
    }
}
