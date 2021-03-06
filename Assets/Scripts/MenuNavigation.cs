﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.SceneManagement;

using UnityEngine.Audio;

public class MenuNavigation : MonoBehaviour
{
    // Text assignments

    public AudioClip[] menuMoveClips;
    public AudioClip[] pageTurnClips;

    public Rebinder rebinder;

    public RectTransform tepihOutTarget;
    public RectTransform mainMenu;
    public RectTransform[] mainMenuTransforms;

    public RectTransform inputOptions;
    public RectTransform[] inputOptionsPlayer1Transforms;
    public RectTransform[] inputOptionsPlayer2Transforms;
    public TMP_Text[] player1RebindTexts;
    public TMP_Text[] player2RebindTexts;
    public RectTransform resolutionOption;
    public RectTransform windowedOption;

    public RectTransform ingameMenu;
    public TMP_Text[] ingameMenuTexts;

    public TMP_Text helpText;

    public RectTransform howToPlay;

    public RectTransform endGame;
    public TMP_Text endGameScoreText;

    public RectTransform lineSelector;

    public Color selectedTextColor;
    public Color unselectedTextColor;

    public RectTransform credits;

    public AudioMixer mixer;

    const int MAIN_HOW_TO_PLAY_INDEX = 1;
    const int MAIN_INPUT_OPTIONS_INDEX = 2;
    const int MAIN_CREDITS_INDEX = 3;

    const int INGAME_RESTART_INDEX = 0;
    const int INGAME_HOW_TO_PLAY_INDEX = 1;
    const int INGAME_INPUT_OPTIONS_INDEX = 2;
    const int INGAME_CREDITS_INDEX = 3;

    const string HELP_TEXT_INPUT = "Accept to rebind, cancel to go back";
    const string HELP_TEXT_INPUT_BINDING = "Press a key to bind, esc to cancel";
    const string HELP_TEXT_RESOLUTION = "< > to pick, accept to apply, cancel to go back";

    // private

    enum State { MainMenu, InputMenu, IngameMenu, Credits, HowToPlay, Ingame, EndGame, None }
    State state;

    bool gameStarted;

    int cx, cy;

    RectTransform lastSelected;

    Vector2 inputOptionsStartPos;
    Vector2 tepihInPos;
    Vector2 tepihOutPos;

    int currentResolutionIndex;
    Resolution[] resolutions;
    int isWindowed = 0;

    private void Awake()
    {
        rebinder.OnBindingComplete += BindingComplete;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        rebinder.OnBindingComplete -= BindingComplete;
    }

    public void Move(int x, int y)
    {
        if (state == State.None)
        {
            return;
        }
        else if (state == State.MainMenu)
        {
            cy = wrap(cy - y, 4);

            MainMenuSelectItem(cy);

            PlaySound();
        }
        else if (state == State.InputMenu)
        {
            cy = wrap(cy - y, 6);
            cx = wrap(cx + x, 1);
            //Debug.Log($"{cx}, {cy}");
            InputOptionsSelectItem(cx, cy);

            if (cy == 5)
            {


                currentResolutionIndex =
                    wrap(currentResolutionIndex + x, resolutions.Length - 1);
                resolutionOption.GetComponent<TMP_Text>().text = $"Resolution {resolutions[currentResolutionIndex].ToString()}";
            }
            else if (cy == 6)
            {
                isWindowed = wrap(isWindowed + x, 1);
                windowedOption.GetComponent<TMP_Text>().text = isWindowed == 1 ? "Windowed" : "Fullscreen";
                //Screen.fullScreen = isWindowed == 0 ? true : false;
            }

            PlaySound();
        }
        else if (state == State.IngameMenu)
        {
            cy = wrap(cy - y, 4);

            IngameMenuSelectItem(cy);

            PlaySound();
        }
        else if (state == State.HowToPlay)
        {
            cy = wrap(cy - y, 1);
            MoveHowToPlay(cy == 1);
        }
    }

    Vector2 howToUpperTarget = new Vector2(0, -30);
    Vector2 howToLowerTarget = new Vector2(0, 210);

    void MoveHowToPlay(bool down)
    {
        if (!down)
            howToPlay.DOAnchorPos(howToUpperTarget, 0.7f).SetUpdate(true);
        else
            howToPlay.DOAnchorPos(howToLowerTarget, 0.7f).SetUpdate(true);
    }

    void PlaySound()
    {
        menuMoveClips.Play2D(true, 1);
    }

    int wrap(int i, int max)
    {
        if (i < 0) return max;
        else if (i > max) return 0;
        else return i;
    }

    public void Accept()
    {
        if (state == State.None)
        {
            return;
        }
        else if (state == State.MainMenu)
        {
            switch (cy)
            {
                case 0: StartGame(); break; // start game
                case 1: DisableMainMenu(); ShowHowToPlay(); break;
                case 2: DisableMainMenu(); ShowOptions(); break; // open options
                case 3: DisableMainMenu(); ShowCredits(); break; // open credits
                case 4: Application.Quit(); break; // quit
                default: break;
            }
        }
        else if (state == State.InputMenu)
        {
            if (cy < 5)
            {
                rebinder.StartRebindDirection(cx, cy);
                MenuInput.e.EnableInput(false);

                var text = cx == 0 ? player1RebindTexts[cy] : player2RebindTexts[cy];
                text.text = "<PRESS>";
                StartCoroutine(MoveLineSelectorDelayed(text, 1.5f));

                resolutions = Screen.resolutions;

                helpText.text = HELP_TEXT_INPUT_BINDING;
            }
            else if (cy == 5 || cy == 6)
            {
                SetResolution(resolutions[currentResolutionIndex], isWindowed == 0 ? true : false);
            }
        }
        else if (state == State.IngameMenu)
        {
            switch (cy)
            {
                case 0: Unpause(); DisableIngameMenu(); StartGame(); break; // restart game
                case 1: DisableIngameMenu(); ShowHowToPlay(); break;  // how to play
                case 2: DisableIngameMenu(); ShowOptions(); break; // input options
                case 3: DisableIngameMenu(); ShowCredits(); break; // open credits
                case 4: Application.Quit(); break; // quit
                default: break;
            }
        }
        else if (state == State.EndGame)
        {
            Unpause(); DisableEndGame(); StartGame();
        }
    }

    void SetResolution(Resolution res, bool fullscreen)
    {
        Screen.SetResolution(
            res.width, res.height, fullscreen);

        Debug.Log($"Setting resolution to {res.ToString()}, windowed: {fullscreen}");
    }

    void StartGame()
    {
        DisableMainMenu();
        Camera.main.transform.DOMoveY(Camera.main.transform.position.y - 30, 1)
            .SetEase(Ease.InCubic);
        StartCoroutine(LoadLevel());
        gameStarted = true;
        state = State.Ingame;
    }

    IEnumerator LoadLevel()
    {
        musicTarget = 0;

        yield return new WaitForSeconds(1);
        GameController.I.LoadLevel("Dugacak");
        //SceneManager.LoadScene(1);

        yield return new WaitForSeconds(1);
        ShowCandleTut();
    }

    Tween musicTween;

    /*
    void MusicTo(float to)
    {
        musicTween?.Kill();
        musicTween = DOTween.To(() => musicVolume, x => musicVolume = x, to, 2);
    }*/

    public void Cancel()
    {
        switch (state)
        {
            case State.MainMenu: break;
            case State.InputMenu:
                if (rebinder.bindingInProgress)
                    break;

                DisableOptions();
                BackToMenu(MAIN_INPUT_OPTIONS_INDEX, INGAME_INPUT_OPTIONS_INDEX);
                break;
            case State.Credits:
                DisableCredits();
                BackToMenu(MAIN_CREDITS_INDEX, INGAME_CREDITS_INDEX);
                break;
            case State.HowToPlay:
                DisableHowToPlay();

                BackToMenu(MAIN_HOW_TO_PLAY_INDEX, INGAME_HOW_TO_PLAY_INDEX);
                break;
            //ShowIngameMenu(); IngameMenuSelectItem(INGAME_HOW_TO_PLAY_INDEX); break;
            case State.Ingame: ShowIngameMenu(); Pause(); break;
            case State.IngameMenu: DisableIngameMenu(); Unpause(); break;
            case State.EndGame: Unpause(); DisableEndGame(); StartGame(); break;
            case State.None: break;
            default: break;
        }
    }

    void BackToMenu(int mainY, int ingameY)
    {
        if (!gameStarted)
        {
            ShowMainMenu();
            MainMenuSelectItem(mainY);
            state = State.MainMenu;
        }
        else
        {
            ShowIngameMenu();
            IngameMenuSelectItem(ingameY);
            state = State.IngameMenu;
        }
    }

    void BindingComplete()
    {
        SetKeyTexts();
        //Debug.Log("Refresh texts!");

        var textT = cx == 0 ?
            inputOptionsPlayer1Transforms[cy] :
            inputOptionsPlayer2Transforms[cy];

        var text = textT.GetComponent<TMP_Text>();
        MoveLineSelectorTo(text, 1.5f);
        SelectText(text);

        helpText.text = HELP_TEXT_INPUT;

        StartCoroutine(DelayEnableInput());
    }

    IEnumerator DelayEnableInput()
    {
        yield return null;
        MenuInput.e.EnableInput(true);
    }

    void ShowOptions()
    {
        if (resolutions == null)
        {
            resolutions = Screen.resolutions;

            for (int i = 0; i < resolutions.Length; i++)
            {
                Resolution cur = Screen.currentResolution;
                if (resolutions[i].width == cur.width &&
                    resolutions[i].height == cur.height &&
                    resolutions[i].refreshRate == cur.refreshRate)
                {
                    currentResolutionIndex = i;
                    break;
                }
            }
        }

        resolutionOption.GetComponent<TMP_Text>().text = $"Resolution {resolutions[currentResolutionIndex].ToString()}";

        cx = 0;
        cy = 0;
        MainMenuSelectItem(-1);
        state = State.InputMenu;
        InputOptionsSelectItem(0, 0);

        inputOptions.DOAnchorPos(Vector2.zero, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutExpo, 1)
            .SetDelay(0.3f);

        pageTurnClips.Play2D(true, 1);

        helpText.text = HELP_TEXT_INPUT;
    }

    void DisableOptions()
    {
        MainMenuSelectItem(1);
        state = State.MainMenu;

        inputOptions.DOAnchorPos(inputOptionsStartPos, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InCubic);

        cx = 0;
        cy = 1;

        pageTurnClips.Play2D(true, 1);
    }

    void ShowMainMenu()
    {
        mainMenu.DOAnchorPos(tepihInPos, 0.5f)
            .SetEase(Ease.OutExpo, 1)
            .SetDelay(0.3f);
    }

    void DisableMainMenu()
    {
        mainMenu.DOAnchorPos(tepihOutPos, 0.5f)
             .SetEase(Ease.InCubic);
    }

    void ShowCredits()
    {
        Debug.Log("Show credits");
        state = State.Credits;
        credits.gameObject.SetActive(true);
        credits.DOAnchorPos(Vector2.zero, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutExpo, 1)
            .SetDelay(0.3f);
    }

    void DisableCredits()
    {
        credits.DOAnchorPos(inputOptionsStartPos, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InCubic);

        //MainMenuSelectItem(2);
        //state = State.MainMenu;

        pageTurnClips.Play2D(true, 1);
    }

    void ShowIngameMenu()
    {
        state = State.IngameMenu;
        ingameMenu.gameObject.SetActive(true);
        ingameMenu.DOAnchorPos(Vector2.zero, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutExpo, 1)
            .SetDelay(0.3f);

        IngameMenuSelectItem(0);

        pageTurnClips.Play2D(true, 1);
    }

    void DisableIngameMenu()
    {
        state = State.Ingame;
        ingameMenu.DOAnchorPos(inputOptionsStartPos, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InCubic);

        pageTurnClips.Play2D(true, 1);
    }

    void ShowHowToPlay()
    {
        cy = 0;

        state = State.HowToPlay;
        howToPlay.gameObject.SetActive(true);
        howToPlay.DOAnchorPos(howToUpperTarget, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutExpo, 1)
            .SetDelay(0.3f);

        pageTurnClips.Play2D(true, 1);
    }

    void DisableHowToPlay()
    {
        howToPlay.DOAnchorPos(inputOptionsStartPos, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InCubic);
    }

    public void ShowEndGame(GameController.Results results)
    {
        DisableTut();

        state = State.EndGame;
        endGame.gameObject.SetActive(true);
        endGame.DOAnchorPos(Vector2.zero, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutExpo, 1)
            .SetDelay(0.3f);

        string ratio = float.IsNaN(results.successRatio) ? "Uncomputable" : results.successRatio.ToString("F1");

        endGameScoreText.text = $@"Total guests served: {results.totalGuestsServed},
Success wishes: {results.totalSuccessWishes},
Failed wishes: {results.totalFailedWishes},
Success ratio: {ratio}%,
Promaja time: {results.promajaTime},
Dirt complaints time: {results.prljavoTime},

Total Score: {results.totalScore}";

        //Score: {results.totalScore}";

        musicTarget = 1;
    }

    void DisableEndGame()
    {
        endGame.DOAnchorPos(inputOptionsStartPos, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InCubic);
    }

    void InputOptionsSelectItem(int x, int y)
    {
        if (lastSelected)
            lastSelected.localScale = Vector3.one;

        RectTransform t = null;

        if (y < 5)
        {
            if (x == 0)
            {
                t = inputOptionsPlayer1Transforms[y];
            }
            else if (x == 1)
            {
                t = inputOptionsPlayer2Transforms[y];
            }

            helpText.text = HELP_TEXT_INPUT;
        }
        else if (y == 5)
        {
            t = resolutionOption;
            helpText.text = HELP_TEXT_RESOLUTION;
        }
        else if (y == 6)
        {
            t = windowedOption;
            helpText.text = HELP_TEXT_RESOLUTION;
        }

        var text = t.GetComponent<TMP_Text>();
        SelectText(text);
        MoveLineSelectorTo(text, 1.5f);

        //t.localScale = Vector3.one * 1.5f;
        lastSelected = t;
    }

    private IEnumerator Start()
    {
        inputOptionsStartPos = inputOptions.anchoredPosition;
        tepihInPos = mainMenu.anchoredPosition;
        tepihOutPos = tepihOutTarget.anchoredPosition;

        yield return null;

        SetKeyTexts();

        yield return null;
        yield return null;
        yield return null;

        MainMenuSelectItem(0);
        //MoveLineSelectorTo(mainMenuTransforms[0].GetComponent<TMP_Text>(), 1.5f);
    }

    void SetKeyTexts()
    {
        player1RebindTexts[0].text = GetBindingString(0, 0);
        player1RebindTexts[1].text = GetBindingString(0, 1);
        player1RebindTexts[2].text = GetBindingString(0, 2);
        player1RebindTexts[3].text = GetBindingString(0, 3);
        player1RebindTexts[4].text = GetBindingString(0, 4);

        player2RebindTexts[0].text = GetBindingString(1, 0);
        player2RebindTexts[1].text = GetBindingString(1, 1);
        player2RebindTexts[2].text = GetBindingString(1, 2);
        player2RebindTexts[3].text = GetBindingString(1, 3);
        player2RebindTexts[4].text = GetBindingString(1, 4);
    }

    string GetBindingString(int player, int index)
    {
        var binding = DomacinInputManager.e.inputActionsAsset.actionMaps[player].actions[index].bindings[0];
        return InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    void MainMenuSelectItem(int y)
    {
        if (lastSelected)
            lastSelected.localScale = Vector3.one;

        if (y < 0) return;

        cy = y;

        var textT = mainMenuTransforms[y];
        var text = textT.GetComponent<TMP_Text>();

        SelectText(text);
        MoveLineSelectorTo(text);

        //mainMenuTransforms[y].localScale = Vector3.one * 1.3f;
        lastSelected = mainMenuTransforms[y];
    }

    void IngameMenuSelectItem(int y)
    {
        cy = y;

        var text = ingameMenuTexts[y];
        SelectText(text);
        MoveLineSelectorTo(text, 1.5f);

        lastSelected = ingameMenuTexts[y].rectTransform;
    }

    IEnumerator MoveLineSelectorDelayed(TMP_Text text, float scale = 1)
    {
        yield return null;
        MoveLineSelectorTo(text, scale);
    }

    void MoveLineSelectorTo(TMP_Text text, float scale = 1)
    {
        var textT = text.rectTransform;

        lineSelector.SetParent(textT.parent);
        float textWidth = text.textBounds.extents.x;
        Vector2 corner = text.textBounds.min;
        corner = textT.TransformPoint(corner);
        //lineSelector.position = corner;

        var sd = lineSelector.sizeDelta;
        var sd0 = sd;
        sd0.x = 0;
        sd.x = textWidth * 11 * scale;

        const float speed = 0.1f;

        lineSelector.DOSizeDelta(sd0, speed)
            .SetUpdate(true)
            .OnComplete(() => { lineSelector.position = corner; });

        lineSelector.DOSizeDelta(sd, speed)
            .SetUpdate(true)
            .SetDelay(speed);
        //lineSelector.sizeDelta = sd;
    }

    void SelectText(TMP_Text text)
    {
        if (lastSelected)
            lastSelected.GetComponent<TMP_Text>().color = unselectedTextColor;

        text.color = selectedTextColor;
    }

    void Pause()
    {
        Time.timeScale = 0;
        musicTarget = 1;
    }

    void Unpause()
    {
        Time.timeScale = 1;
        musicTarget = 0;
    }

    public RectTransform candleTut;
    bool showingCandleTut;
    public RectTransform slavaEndedTut;

    RectTransform currentTut;

    void ShowCandleTut()
    {
        ShowTut(candleTut);

        showingCandleTut = true;
    }

    public void ShowSlavaEndedTut()
    {
        ShowTut(slavaEndedTut);
    }

    void ShowTut(RectTransform elem)
    {
        if (currentTut)
            DisableTut();

        elem.DOAnchorPos(new Vector2(0, -20f), 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutExpo, 1)
            .SetDelay(0.4f);

        currentTut = elem;
    }

    void DisableTut()
    {
        currentTut.DOAnchorPos(new Vector2(0, -1000), 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InExpo, 1)
            .SetDelay(0.3f);
    }

    float musicVolume = 1;
    float musicTarget = 1;
    float musicVelo;

    //float musicLowpass = 1;
    //float musicLowpassTarget = 1;
    //float musicLowpassVelo;

    private void Update()
    {
        musicVolume = Mathf.SmoothDamp(musicVolume, musicTarget, ref musicVelo, 1, 1000, 1.0f / 60f);
        float lowPass = Mathf.Lerp(200, 22000, musicVolume);

        //Debug.Log($"{musicVolume}, {musicTarget}");
        mixer.SetFloat("Music Volume", NAudio.GetLogVolume(musicVolume));
        mixer.SetFloat("Music Lowpass", lowPass);
        mixer.SetFloat("Diegetic Volume", NAudio.GetLogVolume(1 - musicVolume));

        if (showingCandleTut && Candle.e.isBurning)
        {
            showingCandleTut = false;
            DisableTut();
        }
    }
}
