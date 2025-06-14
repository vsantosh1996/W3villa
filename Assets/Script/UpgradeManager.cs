using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("Upgrade Configuration")]
    public UpgradeData[] availableUpgrades; // Drag multiple UpgradeData assets here

    [Header("Runtime Values")]
    public int[] upgradeLevels; // Track level for each upgrade
    public int[] upgradeCosts;  // Track cost for each upgrade

    void Start()
    {
        if (availableUpgrades != null && availableUpgrades.Length > 0)
        {
            // Initialize arrays to match upgrade count
            upgradeLevels = new int[availableUpgrades.Length];
            upgradeCosts = new int[availableUpgrades.Length];

            // Calculate initial costs for all upgrades
            for (int i = 0; i < availableUpgrades.Length; i++)
            {
                if (availableUpgrades[i] != null)
                {
                    upgradeLevels[i] = 0;
                    upgradeCosts[i] = Mathf.RoundToInt(availableUpgrades[i].baseCost *
                        Mathf.Pow(availableUpgrades[i].costMultiplier, upgradeLevels[i]));
                }
            }
        }
    }

    public bool CanAffordUpgrade(int upgradeIndex, int playerMoney)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgradeCosts.Length) return false;
        return playerMoney >= upgradeCosts[upgradeIndex];
    }

    public void PurchaseUpgrade(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= availableUpgrades.Length) return;
        if (availableUpgrades[upgradeIndex] == null) return;

        upgradeLevels[upgradeIndex]++;
        // Recalculate cost for next level
        upgradeCosts[upgradeIndex] = Mathf.RoundToInt(availableUpgrades[upgradeIndex].baseCost *
            Mathf.Pow(availableUpgrades[upgradeIndex].costMultiplier, upgradeLevels[upgradeIndex]));
    }

    public int GetUpgradeEffect(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= availableUpgrades.Length) return 0;
        if (availableUpgrades[upgradeIndex] == null) return 0;

        return availableUpgrades[upgradeIndex].effectValue * upgradeLevels[upgradeIndex];
    }

    public int GetUpgradeCost(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgradeCosts.Length) return 0;
        return upgradeCosts[upgradeIndex];
    }

    public int GetUpgradeLevel(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgradeLevels.Length) return 0;
        return upgradeLevels[upgradeIndex];
    }
}