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

    public GameObject guestWishPanelPrefab;

    public RectTransform notificationsPanel;

    public GameObject notificationRendererPrefab;

    private List<GuestWishPanel> guestWishPanels = new List<GuestWishPanel>();
    
    public Canvas MainCanvas { get; private set; }
    
    private List<int> availableNotificationIndexes;

    #endregion
    
    #region Mono

    private void Start()
    {
        Application.targetFrameRate = 60;
        MainCanvas = GetComponent<Canvas>();
        availableNotificationIndexes = new List<int> {0, 1, 2, 3};
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

    public void ShowNotification(Guest guest, string text, int numberOfStars = -1)
    {
        var notificationObject = Instantiate(notificationRendererPrefab);
        notificationObject.transform.SetParent(notificationsPanel.transform);
        notificationObject.transform.localScale = Vector3.one;

        var notification = notificationObject.GetComponent<NotificationRenderer>();

        var index = availableNotificationIndexes[0];
        availableNotificationIndexes.RemoveAt(0);
        notification.Show(index, guest, text, numberOfStars);
    }

    public void NotificationEnded(int index)
    {
        availableNotificationIndexes.Add(index);
        availableNotificationIndexes.Sort();
    }
    
    #endregion
}
