using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    #region Fields

    public Button playButton;
    public Button exitButton;

    public GameObject levelButtonPrefab;
    public Transform levelPanel;

    #endregion

    #region Mono

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        
        // generate levels buttons
        var levels = LevelSettings.I.LevelsData;
        for (int i = 0; i < levels.Count; i++)
        {
            var button = Instantiate(levelButtonPrefab);
            button.transform.SetParent(levelPanel);
            button.transform.localScale = Vector3.one;

            var levelButton = button.GetComponent<LevelButton>();
            levelButton.SetLevelName(levels[i].Name, i == 0);
        }
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClick);
        exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    #endregion
    
    #region Events

    private void OnPlayButtonClick()
    {
        GameController.I.LoadLevel(LevelButton.SelectedButton.LevelName);
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }
    
    #endregion
}
