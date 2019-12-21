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

    // private

    enum State { MainMenu, InputMenu, HowTo, None }
    State state;

    int cx, cy;

    RectTransform lastSelected;

    Vector2 inputOptionsStartPos;
    Vector2 tepihInPos;
    Vector2 tepihOutPos;

    private void Awake()
    {
        rebinder.OnBindingComplete += BindingComplete;
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
                case 2: break; // open credits
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
    }

    void StartGame()
    {
        DisableMainMenu();
        Camera.main.transform.DOMoveY(Camera.main.transform.position.y - 30, 1)
            .SetEase(Ease.InCubic);
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }

    public void Cancel()
    {
        if (state == State.InputMenu)
        {
            DisableOptions();
            ShowMainMenu();
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
            .SetEase(Ease.OutExpo, 1)
            .SetDelay(0.3f);
    }

    void DisableOptions()
    {
        MainMenuSelectItem(1);
        state = State.MainMenu;

        inputOptions.DOAnchorPos(inputOptionsStartPos, 0.5f)
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

        t.localScale = Vector3.one * 1.5f;
        lastSelected = t;
    }

    private void Start()
    {
        SetKeyTexts();
        inputOptionsStartPos = inputOptions.anchoredPosition;
        tepihInPos = mainMenu.anchoredPosition;
        tepihOutPos = tepihOutTarget.anchoredPosition;

        MainMenuSelectItem(0);
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
        var binding = rebinder.inputActionsAsset.actionMaps[player].actions[index].bindings[0];
        return InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    void MainMenuSelectItem(int y)
    {
        if (lastSelected)
            lastSelected.localScale = Vector3.one;

        if (y < 0) return;

        mainMenuTransforms[y].localScale = Vector3.one * 1.3f;
        lastSelected = mainMenuTransforms[y];
    }

    private void Update()
    {
    }
}
