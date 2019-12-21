using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInput : MonoBehaviour
{
    public static MenuInput e;

    public MenuNavigation menuNavigation;

    MenuInputActions input;

    private void Awake()
    {
        e = this;

        input = new MenuInputActions();
        input.Enable();
    }

    public void EnableInput(bool b)
    {
        if (b)
            input.Enable();
        else
            input.Disable();
    }

    Vector2Int lastMove;

    private void Update()
    {
        if (input.Menu.Accept.triggered)
            Accept();

        if (input.Menu.Cancel.triggered)
            Cancel();

        if (input.Menu.Move.triggered)
        {
            var v = Vector2Int.RoundToInt(input.Menu.Move.ReadValue<Vector2>());
            if (lastMove != v)
            {
                if (v != Vector2Int.zero)
                    Move(v.x, v.y);

                lastMove = v;
            }
        }

        if (input.Menu.Move.ReadValue<Vector2>() == Vector2.zero)
            lastMove = Vector2Int.zero;
    }

    void Move(int x, int y)
    {
        //Debug.Log("" + x + " " + y);
        menuNavigation.Move(x, y);
    }

    void Accept()
    {
        menuNavigation.Accept();
    }

    void Cancel()
    {
        menuNavigation.Cancel();
    }
}
