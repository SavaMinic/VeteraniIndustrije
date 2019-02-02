using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    #region Fields

    public Button playButton;
    public Button exitButton;

    #endregion

    #region Mono

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
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
        GameController.I.LoadLevel("Test");
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }
    
    #endregion
}
