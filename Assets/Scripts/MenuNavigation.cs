using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class MenuNavigation : MonoBehaviour
{
    // Text assignments

    public Rebinder rebinder;

    public RectTransform startGame;
    public RectTransform inputOptions;
    public RectTransform credits;
    public RectTransform quit;

    public RectTransform[] mainMenuTransforms;

    // private

    enum State { MainMenu, InputMenu, HowTo, None }
    State state;

    int cx, cy;

    RectTransform lastSelected;

    public void Move(int x, int y)
    {
        if (state == State.None)
        {
            return;
        }
        if (state == State.MainMenu)
        {
            cy -= y;
            if (cy > 3) cy = 0;
            if (cy < 0) cy = 3;

            SelectItem(cy);
        }
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
                case 0: Application.LoadLevel(0); break; // start game
                case 1: break; // open options
                case 2: break; // open credits
                case 3: Application.Quit(); break; // quit
                default: break;
            }
        }
    }

    public void Cancel() { }

    void SelectItem(int y)
    {
        if (lastSelected)
            lastSelected.localScale = Vector3.one;

        mainMenuTransforms[y].localScale = Vector3.one * 1.5f;
        lastSelected = mainMenuTransforms[y];
    }

    private void Update()
    {
    }
}
