using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject leaderboardPanel;
    public Button leaderboardButton;
    //public Button closeButton;
    public Transform contentParent;
    public GameObject entryPrefab;
    public TMP_Text loadingText;

    void Start()
    {
        leaderboardButton.onClick.AddListener(ShowLeaderboard);
        //closeButton.onClick.AddListener(HideLeaderboard);
        leaderboardPanel.SetActive(false);
    }

    void ShowLeaderboard()
    {
        leaderboardPanel.SetActive(true);
        leaderboardPanel.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
        StartCoroutine(LoadMockLeaderboard());
    }

    void HideLeaderboard()
    {
        leaderboardPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).OnComplete(() => leaderboardPanel.SetActive(false));
    }

    IEnumerator LoadMockLeaderboard()
    {
        loadingText.text = "Loading...";
        loadingText.gameObject.SetActive(true);

        // Clear old entries
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        yield return new WaitForSeconds(1f); // Simulate network delay

        // Mock data
        string[] names = { "CoinMaster", "TapKing", "ClickHero", "Player", "Newbie" };
        int[] scores = { 999999, 888888, 777777, 50000, 1000 };

        for (int i = 0; i < names.Length; i++)
        {
            GameObject entry = Instantiate(entryPrefab, contentParent);
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = $"#{i + 1}";
            texts[1].text = names[i];
            texts[2].text = scores[i].ToString("N0");
        }

        loadingText.gameObject.SetActive(false);
    }
}