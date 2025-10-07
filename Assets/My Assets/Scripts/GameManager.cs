using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public AudioManager audiomanager;
    public LevelLoader levelLoader;
    public GridManager gridManager;
    public GameObject loadingImage;
    public float levelWaitTime = 1.5f; // slightly shorter wait
    public int currentLevelIndex;
    public TextMeshProUGUI currentLevelText;
    public GameObject pausePanel,startPanel,pauseMenuParent;
    internal bool playerCanMove = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void NewGame()
    {
        playerCanMove = true;
        currentLevelIndex = 0;
        StartCoroutine(WaitAndMoveToNextLevel());
        startPanel.SetActive(false);
    }

    public void Continue()
    {
        playerCanMove = true;
        currentLevelIndex = PlayerPrefs.GetInt("SavedLevel", 0);
        StartCoroutine(WaitAndMoveToNextLevel());
        startPanel.SetActive(false);
    }

    public void PauseGame()
    {
        playerCanMove = false;
        pausePanel.SetActive(true);
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        playerCanMove = true;
        pausePanel.SetActive(false);
    }

    private IEnumerator WaitAndMoveToNextLevel()
    {
        currentLevelText.text = $"Level {currentLevelIndex + 1}";
        loadingImage.SetActive(true);
        audiomanager.StopBackgroundMusic();
        yield return new WaitForSeconds(levelWaitTime);
        loadingImage.SetActive(false);
        pauseMenuParent.SetActive(true);
        levelLoader.LoadLevelFromText(currentLevelIndex);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("SavedLevel", currentLevelIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        PlayerPrefs.Save();
    }
}