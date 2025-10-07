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
    public bool playerCanMove = true;
    public TextMeshProUGUI currentLevelText;

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
        currentLevelIndex = 0;
        StartCoroutine(WaitAndMoveToNextLevel());
    }

    public void Continue()
    {
        currentLevelIndex = PlayerPrefs.GetInt("SavedLevel", 0);
        StartCoroutine(WaitAndMoveToNextLevel());
    }

    private IEnumerator WaitAndMoveToNextLevel()
    {
        currentLevelText.text = $"Level {currentLevelIndex + 1}";
        loadingImage.SetActive(true);
        audiomanager.StopBackgroundMusic();
        yield return new WaitForSeconds(levelWaitTime);
        loadingImage.SetActive(false);
        levelLoader.LoadLevelFromText(currentLevelIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("SavedLevel", currentLevelIndex);
        PlayerPrefs.Save();
    }
}