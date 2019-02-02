using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    #region Simple singleton

    private static GameController instance;

    public static GameController I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameController>();
            }
            if (instance == null)
            {
                var go = new GameObject("GameController");
                instance = go.AddComponent<GameController>();
            }
            return instance;
        }
    }

    #endregion

    #region Properties

    public LevelSettings.LevelData Level{ get; private set; }
    
    public bool IsPaused { get; private set; }

    #endregion

    #region Init

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #endregion
    
    #region Public

    public void LoadLevel(string levelName)
    {
        if (LevelSettings.I.LevelsData.Exists(d => d.Name == levelName))
        {
            Level = LevelSettings.I.LevelsData.Find(d => d.Name == levelName);
        }
        else
        {
            Debug.LogError("No level " + levelName);
            Level = LevelSettings.I.LevelsData[0];
        }
        
        // load the game scene
        SceneManager.LoadScene("SampleScene");
    }

    public void EndGame()
    {
        IsPaused = true;
        CanvasController.I.ShowEndPanel();
    }
    
    #endregion
    
    #region Events

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            InitializeLevel();
        }
    }
    
    #endregion

    #region Private

    private void InitializeLevel()
    {
        var candle = FindObjectOfType<Candle>();
        candle.burnTime = Level.CandleDuration;

        IsPaused = false;
    }
    

    #endregion
}
