using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public LevelLoader levelLoader;
    public GridManager gridManager;
    public GameObject loadingImage;
    public float levelWaitTime = 2f;
    public int currentLevelIndex;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGame()
    {
        currentLevelIndex = -1;
        StartCoroutine(ShowLevelLoadScreenAndLoadLevel());
    }

    public void Continue()
    {
        currentLevelIndex = PlayerPrefs.GetInt("SavedLevel", 0);
        StartCoroutine(ShowLevelLoadScreenAndLoadLevel());
    }

    private IEnumerator ShowLevelLoadScreenAndLoadLevel()
    {
        loadingImage.SetActive(true);
        if (levelLoader != null) levelLoader.NextLevel();
        else Debug.Log("Level complete. Configure LevelLoader to advance.");
        yield return new WaitForSeconds(levelWaitTime);
        loadingImage.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("SavedLevel", currentLevelIndex);
    }
}
