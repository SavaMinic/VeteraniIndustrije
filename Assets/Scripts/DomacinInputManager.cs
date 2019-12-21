using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class DomacinInputManager : MonoBehaviour
{
    

    public static DomacinInputManager e;
    void Awake()
    {
        e = this;
        DontDestroyOnLoad(gameObject);

        var playerActionmap = inputActionsAsset.actionMaps[0]; // player 1
        p1 = new Actions();
        p1.moveLeftAction = playerActionmap.FindAction("Move Left");
        p1.moveRightAction = playerActionmap.FindAction("Move Right");
        p1.moveUpAction = playerActionmap.FindAction("Move Up");
        p1.moveDownAction = playerActionmap.FindAction("Move Down");
        p1.interactAction = playerActionmap.FindAction("Interact");

        playerActionmap = inputActionsAsset.actionMaps[1]; // player 2
        p2 = new Actions();
        p2.moveLeftAction = playerActionmap.FindAction("Move Left");
        p2.moveRightAction = playerActionmap.FindAction("Move Right");
        p2.moveUpAction = playerActionmap.FindAction("Move Up");
        p2.moveDownAction = playerActionmap.FindAction("Move Down");
        p2.interactAction = playerActionmap.FindAction("Interact");
    }

    public InputActionAsset inputActionsAsset;

    public class Actions
    {
        public InputAction moveLeftAction;
        public InputAction moveRightAction;
        public InputAction moveUpAction;
        public InputAction moveDownAction;
        public InputAction interactAction;
    }

    public Actions p1;
    public Actions p2;
}
