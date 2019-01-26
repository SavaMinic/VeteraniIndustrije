using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    #region Simple singleton

    private static CanvasController instance;

    public static CanvasController I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CanvasController>();
            }
            return instance;
        }
    }
    
    #endregion
    
    #region Properties/Fields

    [SerializeField]
    private GameObject guestWishPanelPrefab;

    private List<GuestWishPanel> guestWishPanels = new List<GuestWishPanel>();
    
    public Canvas MainCanvas { get; private set; }

    #endregion
    
    #region Mono

    private void Start()
    {
        Application.targetFrameRate = 60;
        MainCanvas = GetComponent<Canvas>();
    }

    #endregion
    
    #region Public

    public void AddNewGuestWish(Guest guest)
    {
        var availableWishPanel = guestWishPanels.Find(p => !p.gameObject.activeSelf);
        if (availableWishPanel == null)
        {
            var wishPanelObject = Instantiate(guestWishPanelPrefab);
            wishPanelObject.transform.SetParent(transform);
            wishPanelObject.transform.localScale = Vector3.one;
            availableWishPanel = wishPanelObject.GetComponent<GuestWishPanel>();
            guestWishPanels.Add(availableWishPanel);
        }
        availableWishPanel.SetGuest(guest);
    }
    
    #endregion
}
