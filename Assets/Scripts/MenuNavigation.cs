using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    // Text assignments

    public Rebinder rebinder;

    public RectTransform tepihOutTarget;
    public RectTransform mainMenu;
    public RectTransform[] mainMenuTransforms;

    public RectTransform inputOptions;
    public RectTransform[] inputOptionsPlayer1Transforms;
    public RectTransform[] inputOptionsPlayer2Transforms;

    public TMP_Text[] player1RebindTexts;
    public TMP_Text[] player2RebindTexts;

    public RectTransform ingameMenu;
    public TMP_Text[] ingameMenuTexts;

    public RectTransform howToPlay;

    public RectTransform lineSelector;

    public Color selectedTextColor;
    public Color unselectedTextColor;

    public RectTransform credits;

    const int INGAME_RESTART_INDEX = 0;
    const int INGAME_HOW_TO_PLAY_INDEX = 1;
    const int INGAME_INPUT_OPTIONS_INDEX = 2;
    const int INGAME_CREDITS_INDEX = 3;

    // private

    enum State { MainMenu, InputMenu, IngameMenu, Credits, HowToPlay, Ingame, None }
    State state;

    bool gameStarted;

    int cx, cy;

    RectTransform lastSelected;

    Vector2 inputOptionsStartPos;
    Vector2 tepihInPos;
    Vector2 tepihOutPos;

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
        if (state == State.MainMenu)
        {
            cy = wrap(cy - y, 3);

            MainMenuSelectItem(cy);
        }
        else if (state == State.InputMenu)
        {
            cy = wrap(cy - y, 4);
            cx = wrap(cx + x, 1);
            Debug.Log($"{cx}, {cy}");
            InputOptionsSelectItem(cx, cy);
        }
        if (state == State.IngameMenu)
        {
            cy = wrap(cy - y, 4);

            IngameMenuSelectItem(cy);
        }
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
                case 1: DisableMainMenu(); ShowOptions(); break; // open options
                case 2: DisableMainMenu(); ShowCredits(); break; // open credits
                case 3: Application.Quit(); break; // quit
                default: break;
            }
        }
        else if (state == State.InputMenu)
        {
            if (cy < 5)
            {
                rebinder.StartRebindDirection(cx, cy);
                MenuInput.e.EnableInput(false);
            }
        }
        else if (state == State.IngameMenu)
        {
            switch (cy)
            {
                case 0: StartGame(); break; // restart game
                case 1: DisableIngameMenu(); ShowHowToPlay(); break;  // how to play
                case 2: DisableIngameMenu(); ShowOptions(); break; // input options
                case 3: DisableIngameMenu(); ShowCredits(); break; // open credits
                case 4: Application.Quit(); break; // quit
                default: break;
            }
        }
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
        yield return new WaitForSeconds(1);
        GameController.I.LoadLevel("Endless");
        //SceneManager.LoadScene(1);
    }

    public void Cancel()
    {
        switch (state)
        {
            case State.MainMenu: break;
            case State.InputMenu:
                DisableOptions();
                if (!gameStarted)
                    ShowMainMenu();
                else
                {
                    ShowIngameMenu();
                    IngameMenuSelectItem(INGAME_INPUT_OPTIONS_INDEX);
                }
                break;
            case State.Credits:
                DisableCredits();
                if (!gameStarted)
                    ShowMainMenu();
                else
                {
                    ShowIngameMenu();
                    IngameMenuSelectItem(INGAME_CREDITS_INDEX);
                }
                break;
            case State.HowToPlay: DisableHowToPlay(); ShowIngameMenu(); IngameMenuSelectItem(INGAME_HOW_TO_PLAY_INDEX); break;
            case State.Ingame: ShowIngameMenu(); Pause(); break;
            case State.IngameMenu: DisableIngameMenu(); Unpause(); break;
            case State.None: break;
            default: break;
        }
    }

    void BindingComplete()
    {
        SetKeyTexts();
        Debug.Log("Refresh texts!");
        MenuInput.e.EnableInput(true);
    }

    void ShowOptions()
    {
        cx = 0;
        cy = 0;
        MainMenuSelectItem(-1);
        state = State.InputMenu;
        InputOptionsSelectItem(0, 0);

        inputOptions.DOAnchorPos(Vector2.zero, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutExpo, 1)
            .SetDelay(0.3f);
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

        MainMenuSelectItem(2);
        state = State.MainMenu;
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
    }

    void DisableIngameMenu()
    {
        state = State.Ingame;
        ingameMenu.DOAnchorPos(inputOptionsStartPos, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InCubic);
    }

    void ShowHowToPlay()
    {
        state = State.HowToPlay;
        howToPlay.gameObject.SetActive(true);
        howToPlay.DOAnchorPos(Vector2.zero, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutExpo, 1)
            .SetDelay(0.3f);
    }

    void DisableHowToPlay()
    {
        howToPlay.DOAnchorPos(inputOptionsStartPos, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InCubic);
    }

    void InputOptionsSelectItem(int x, int y)
    {
        if (lastSelected)
            lastSelected.localScale = Vector3.one;

        RectTransform t = null;

        if (x == 0)
        {
            t = inputOptionsPlayer1Transforms[y];
        }
        else if (x == 1)
        {
            t = inputOptionsPlayer2Transforms[y];
        }

        var text = t.GetComponent<TMP_Text>();
        SelectText(text);
        MoveLineSelectorTo(text, 1.5f);

        //t.localScale = Vector3.one * 1.5f;
        lastSelected = t;
    }

    private IEnumerator Start()
    {
        SetKeyTexts();
        inputOptionsStartPos = inputOptions.anchoredPosition;
        tepihInPos = mainMenu.anchoredPosition;
        tepihOutPos = tepihOutTarget.anchoredPosition;

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
    }

    void Unpause()
    {
        Time.timeScale = 1;
    }

    private void Update()
    {
    }
}
