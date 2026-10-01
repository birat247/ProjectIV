using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Supports TextMeshPro if you are using it

public class ResultManager : MonoBehaviour
{
    [Header("UI Panel Setup")]
    public GameObject resultUI;          // Drag ResultUI here
    public Text legacyResultText;         // Assign if using standard UI Text
    public TextMeshProUGUI tmpResultText;// Assign if using TextMeshPro Text

    [Header("Fighters")]
    public FightingController playerController;
    public OpponentAI opponentAI;

    private bool isGameOver = false;

    void Start()
    {
        // Make sure ResultUI panel is hidden at game start
        if (resultUI != null)
        {
            resultUI.SetActive(false);
        }

        Time.timeScale = 1f; // Ensure normal game speed on start
    }

    void Update()
    {
        if (isGameOver) return;

        // Player Lost
        if (playerController != null && playerController.currentHealth <= 0)
        {
            ShowResultScreen("YOU LOSE!");
        }
        // Player Won
        else if (opponentAI != null && opponentAI.currentHealth <= 0)
        {
            ShowResultScreen("YOU WIN!");
        }
    }

    void ShowResultScreen(string textMessage)
    {
        isGameOver = true;

        // Set Text
        if (tmpResultText != null)
            tmpResultText.text = textMessage;

        if (legacyResultText != null)
            legacyResultText.text = textMessage;

        // Show Result Panel
        if (resultUI != null)
            resultUI.SetActive(true);

        // Stop/End previous scene gameplay logic
        Time.timeScale = 0f;
    }

    // Call this from the MAIN MENU button OnClick listener
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Unfreeze time before scene change
        SceneManager.LoadScene("MainMenu");
    }
}