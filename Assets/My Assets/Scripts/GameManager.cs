using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Dependencies")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GridManager gridManager;

    [Header("UI Elements")]
    [SerializeField] private GameObject loadingImage;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject pauseMenuParent;

    [Header("Settings")]
    [SerializeField] private float levelWaitTime = 1.5f;

    public int CurrentLevelIndex { get; set; }
    public bool IsPlayerInputEnabled { get; private set; } = true;

    private const string SAVED_LEVEL_KEY = "SavedLevel";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGame()
    {
        IsPlayerInputEnabled = true;
        // CurrentLevelIndex = 0; // Uncomment if new game should always start at 0
        PlayerPrefs.SetInt(SAVED_LEVEL_KEY, CurrentLevelIndex);
        StartCoroutine(WaitAndMoveToNextLevel());
        startPanel.SetActive(false);
    }

    public void Continue()
    {
        IsPlayerInputEnabled = true;
        CurrentLevelIndex = PlayerPrefs.GetInt(SAVED_LEVEL_KEY, 0);
        StartCoroutine(WaitAndMoveToNextLevel());
        startPanel.SetActive(false);
    }

    public void PauseGame()
    {
        IsPlayerInputEnabled = false;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Optional: Pause physics/time
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        IsPlayerInputEnabled = true;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void LevelComplete()
    {
        if (!IsPlayerInputEnabled) return;

        IsPlayerInputEnabled = false;
        // Logic to advance level
        CurrentLevelIndex++; 
        // Note: Logic in PlayerController was: Wait -> Show Loading -> Load Next.
        // We will centralize it here.
        StartCoroutine(LevelCompleteSequence());
    }

    private IEnumerator LevelCompleteSequence()
    {
        yield return new WaitForSeconds(0.5f); // Small delay after win
        
        // Update UI for NEXT level
        currentLevelText.text = $"Level {CurrentLevelIndex + 1}";
        
        loadingImage.SetActive(true);
        audioManager.StopBackgroundMusic();
        
        yield return new WaitForSeconds(levelWaitTime);
        
        loadingImage.SetActive(false);
        pauseMenuParent.SetActive(true);
        
        // Load the next level
        levelLoader.LoadLevelFromText(CurrentLevelIndex);
        
        IsPlayerInputEnabled = true;
        SaveGame();
    }

    private IEnumerator WaitAndMoveToNextLevel()
    {
        currentLevelText.text = $"Level {CurrentLevelIndex + 1}";
        loadingImage.SetActive(true);
        audioManager.StopBackgroundMusic();
        
        yield return new WaitForSeconds(levelWaitTime);
        
        loadingImage.SetActive(false);
        pauseMenuParent.SetActive(true);
        
        levelLoader.LoadLevelFromText(CurrentLevelIndex);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt(SAVED_LEVEL_KEY, CurrentLevelIndex);
        PlayerPrefs.Save();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
