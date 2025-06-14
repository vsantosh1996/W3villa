using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace ClickerGame
{
    public class GameManager : MonoBehaviour
    {
        [Header("UI")]
        public Button tapButton;
        public TMP_Text coinText;
        public Button upgradeButton;
        public GameObject upgradePanel;

        [Header("Button Movement Settings")]
        public RectTransform tapButtonRect; // Assign the tap button's RectTransform
        public RectTransform movementArea; // Area where button can move (usually the Canvas or a panel)
        public float moveSpeed = 0.3f; // Animation duration
        public bool enableRandomMovement = true;

        [Header("Upgrades")]
        public Button autoCollectorBtn;
        public Button tapMultiplierBtn;
        public TMP_Text autoCollectorText;
        public TMP_Text tapMultiplierText;

        // Game Data
        int coins = 0;
        int tapMultiplier = 1;
        int autoCollectorLevel = 0;
        int tapMultiplierLevel = 0;
        float autoTimer = 0.1f;

        // Costs
        int autoCollectorCost = 50;
        int tapMultiplierCost = 25;

        // Button movement
        Vector2 originalButtonPosition;

        void Start()
        {
            LoadGame();

            // Store original button position
            if (tapButtonRect == null)
                tapButtonRect = tapButton.GetComponent<RectTransform>();

            originalButtonPosition = tapButtonRect.anchoredPosition;

            // If no movement area is set, use the canvas
            if (movementArea == null)
                movementArea = GameObject.FindObjectOfType<Canvas>().GetComponent<RectTransform>();

            tapButton.onClick.AddListener(TapCoin);
            upgradeButton.onClick.AddListener(ToggleUpgrades);
            autoCollectorBtn.onClick.AddListener(BuyAutoCollector);
            tapMultiplierBtn.onClick.AddListener(BuyTapMultiplier);

            UpdateUI();
            upgradePanel.SetActive(false);
        }

        void Update()
        {
            // Auto collector
            if (autoCollectorLevel > 0)
            {
                autoTimer += Time.deltaTime;
                if (autoTimer >= 1f)
                {
                    autoTimer = 0f;
                    AddCoins(autoCollectorLevel);
                }
            }
        }

        void TapCoin()
        {
            // Bounce animation
            tapButton.transform.DOScale(1.2f, 0.1f)
                .OnComplete(() => tapButton.transform.DOScale(1f, 0.5f));

            AddCoins(1 * tapMultiplier);

            // Move button to new position
            if (enableRandomMovement)
                MoveButtonToRandomPosition();
        }

        void MoveButtonToRandomPosition()
        {
            // Get button dimensions
            Vector2 buttonSize = tapButtonRect.sizeDelta;

            // Calculate safe movement bounds (keeping button fully visible)
            float halfWidth = buttonSize.x * 0.5f;
            float halfHeight = buttonSize.y * 0.5f;

            // Get movement area bounds
            Vector2 areaSize = movementArea.sizeDelta;

            // Calculate random position within safe bounds
            float randomX = Random.Range(-areaSize.x * 0.5f + halfWidth, areaSize.x * 0.5f - halfWidth);
            float randomY = Random.Range(-areaSize.y * 0.5f + halfHeight, areaSize.y * 0.5f - halfHeight);

            Vector2 newPosition = new Vector2(randomX, randomY);

            // Animate to new position
            tapButtonRect.DOAnchorPos(newPosition, moveSpeed).SetEase(Ease.OutQuart);
        }

        // Alternative: Move in specific patterns
        void MoveButtonInCircle()
        {
            float angle = Time.time * 2f; // Speed of circle movement
            float radius = 80f; // Circle radius

            Vector2 newPosition = new Vector2(
                originalButtonPosition.x + Mathf.Cos(angle) * radius,
                originalButtonPosition.y + Mathf.Sin(angle) * radius
            );

            tapButtonRect.DOAnchorPos(newPosition, moveSpeed);
        }

        // Alternative: Move to corners in sequence
        int cornerIndex = 0;
        void MoveButtonToCorners()
        {
            Vector2 areaSize = movementArea.sizeDelta;
            Vector2 buttonSize = tapButtonRect.sizeDelta;

            Vector2[] corners = new Vector2[]
            {
                new Vector2(-areaSize.x * 0.4f, areaSize.y * 0.4f),   // Top-left
                new Vector2(areaSize.x * 0.4f, areaSize.y * 0.4f),    // Top-right
                new Vector2(areaSize.x * 0.4f, -areaSize.y * 0.4f),   // Bottom-right
                new Vector2(-areaSize.x * 0.4f, -areaSize.y * 0.4f)   // Bottom-left
            };

            tapButtonRect.DOAnchorPos(corners[cornerIndex], moveSpeed);
            cornerIndex = (cornerIndex + 1) % corners.Length;
        }

        // Method to reset button to original position
        public void ResetButtonPosition()
        {
            tapButtonRect.DOAnchorPos(originalButtonPosition, moveSpeed);
        }

        void AddCoins(int amount)
        {
            coins += amount;
            UpdateUI();

            // Coin text animation
            coinText.transform.DOScale(1.1f, 0.1f)
                .OnComplete(() => coinText.transform.DOScale(1f, 0.1f));
        }

        void ToggleUpgrades()
        {
            bool isActive = upgradePanel.activeInHierarchy;
            upgradePanel.SetActive(!isActive);

            if (!isActive)
                upgradePanel.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
        }

        void BuyAutoCollector()
        {
            if (coins >= autoCollectorCost)
            {
                coins -= autoCollectorCost;
                autoCollectorLevel++;
                autoCollectorCost = Mathf.RoundToInt(autoCollectorCost * 1.5f);
                UpdateUI();
                SaveGame();
            }
        }

        void BuyTapMultiplier()
        {
            if (coins >= tapMultiplierCost)
            {
                coins -= tapMultiplierCost;
                tapMultiplierLevel++;
                tapMultiplier = 1 + tapMultiplierLevel;
                tapMultiplierCost = Mathf.RoundToInt(tapMultiplierCost * 1.8f);
                UpdateUI();
                SaveGame();
            }
        }

        void UpdateUI()
        {
            coinText.text = "Coins: " + coins;

            autoCollectorText.text = $"Auto Collector Lv.{autoCollectorLevel}\nCost: {autoCollectorCost}";
            tapMultiplierText.text = $"Tap Power Lv.{tapMultiplierLevel}\nCost: {tapMultiplierCost}";

            autoCollectorBtn.interactable = coins >= autoCollectorCost;
            tapMultiplierBtn.interactable = coins >= tapMultiplierCost;
        }

        void SaveGame()
        {
            PlayerPrefs.SetInt("Coins", coins);
            PlayerPrefs.SetInt("TapMultiplier", tapMultiplierLevel);
            PlayerPrefs.SetInt("AutoCollector", autoCollectorLevel);
            PlayerPrefs.SetInt("AutoCost", autoCollectorCost);
            PlayerPrefs.SetInt("TapCost", tapMultiplierCost);
            PlayerPrefs.SetString("LastSave", System.DateTime.Now.ToBinary().ToString());
        }

        void LoadGame()
        {
            coins = PlayerPrefs.GetInt("Coins", 0);
            tapMultiplierLevel = PlayerPrefs.GetInt("TapMultiplier", 0);
            autoCollectorLevel = PlayerPrefs.GetInt("AutoCollector", 0);
            autoCollectorCost = PlayerPrefs.GetInt("AutoCost", 50);
            tapMultiplierCost = PlayerPrefs.GetInt("TapCost", 25);

            tapMultiplier = 1 + tapMultiplierLevel;            
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveGame();
        }

        void OnDestroy()
        {
            SaveGame();
        }
    }
}