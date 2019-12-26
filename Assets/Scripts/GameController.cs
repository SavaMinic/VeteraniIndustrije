﻿using System.Collections;
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

    private const string HighScoreKey = "HighScore_";

    LevelAsset level;
    public LevelAsset Level
    {
        get
        {
            if (!level) level = Resources.Load<LevelAsset>("Levels/Endless");
            return level;
        }

        set => level = value;
    }

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

    public int GetHighScore(string levelName)
    {
        return PlayerPrefs.GetInt(HighScoreKey + levelName, 0);
    }

    public void SetHighScore(string levelName, int score)
    {
        PlayerPrefs.SetInt(HighScoreKey + levelName, score);
    }

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
        var results = CalculateResults();

        FindObjectOfType<MenuNavigation>().ShowEndGame(results);
        //CanvasController.I.ShowEndPanel();
    }

    public class Results
    {
        public float totalSlavaTime;
        public int totalSuccessWishes;
        public int totalWishes;
        public int totalFailedWishes;
        public float successRatio;
        public int totalGuestsServed;

        public int totalScore;
    }



    Results CalculateResults()
    {
        Results results = new Results();

        results.totalGuestsServed = GuestManager.I.guestsServedCount;

        foreach (var wish in GuestManager.I.allCompletedWishes)
        {
            if (!wish.IsFinished)
                continue;

            if (wish.IsSuccess.Value)
                results.totalSuccessWishes++;
            else
                results.totalFailedWishes++;
        }

        results.totalWishes = GuestManager.I.allCompletedWishes.Count;
        results.successRatio = ((float)results.totalSuccessWishes / results.totalWishes) * 100f;

        results.totalScore =
            (int)(results.totalGuestsServed * results.totalSlavaTime * results.successRatio);

        return results;
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

        GuestManager.I.LimitGuestNumber = Level.LimitGuestNumber;

        var availableWishes = Level.AvailableWishes;
        Debug.Assert(availableWishes != null, "Available wishes are null");
        var objectsThatCanBeDisabled = LevelSettings.I.InteractableObjectsNames;
        for (int i = 0; i < objectsThatCanBeDisabled.Count; i++)
        {
            var objectThatCanBeDisabled = objectsThatCanBeDisabled[i];
            if (!availableWishes.Contains(objectThatCanBeDisabled.WishType))
            {
                var objectToDisable = GameObject.Find(objectThatCanBeDisabled.ItemName);
                if (objectToDisable != null)
                {
                    var interactable = objectToDisable.GetComponent<Interactable>();
                    if (interactable != null && interactable.inSlot != null)
                    {
                        interactable.inSlot.itemInSlot = null;
                        interactable.inSlot = null;
                    }
                    objectToDisable.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError("can't find " + objectThatCanBeDisabled.ItemName);
                }
            }
        }

        if (!Level.IsRaining)
        {
            var metla = FindObjectOfType<Metla>();
            metla.inSlot.itemInSlot = null;
            metla.inSlot = null;
            metla.gameObject.SetActive(false);
        }

        IsPaused = false;
    }


    #endregion
}
