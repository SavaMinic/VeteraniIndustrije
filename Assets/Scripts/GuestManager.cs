using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GuestManager : MonoBehaviour
{
    #region Simple singleton

    private static GuestManager instance;

    public static GuestManager I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GuestManager>();
            }
            return instance;
        }
    }

    #endregion

    #region Properties

    [SerializeField] WishQuip[] wishQuips;
    [SerializeField] GeneralQuips generalQuips;

    public Transform entrancePosition;
    public Transform exitPosition;
    public Transform zitoPosition;
    public Seat[] guestSeats;

    public List<GameObject> GuestPrefabs = new List<GameObject>();
    public GameObject guestAgentPrefab;

    public int InitialGuestCount;
    public float DelayForGeneratingGuestsSlow = 10f;
    public float DelayForGeneratingGuestsFast = 25f;

    // if != 0, then it will limit sitting position to this
    public int LimitGuestNumber;

    public int GuestCount => guestSeats.Length - availableSittingPositionIndex.Count;
    public float GuestCountRatio => (float)GuestCount / availableSittingPositionIndex.Count;

    public float DelayForGeneratingGuests =>
        availableSittingPositionIndex.Count > 3
        ? DelayForGeneratingGuestsFast
        : DelayForGeneratingGuestsSlow;

    private float timeToGenerateNewGuests;

    private bool autoSpawnGuests = true;

    private List<int> availableSittingPositionIndex = new List<int>();

    private bool slavaHasStarted;

    #endregion

    #region Mono

    private void Start()
    {
        InitializeSeatingPositions();
    }

    private void Update()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        if (!Candle.e.isBurning || !autoSpawnGuests)
            return;

        if (Candle.e.isBurning && !slavaHasStarted)
            StartSlava();

        if (timeToGenerateNewGuests > 0f)
        {
            timeToGenerateNewGuests -= Time.deltaTime;
            if (timeToGenerateNewGuests <= 0f)
            {
                timeToGenerateNewGuests = DelayForGeneratingGuests;
                GenerateNewGuest();
            }
        }
    }

    #endregion

    #region Public

    public void SittingPlaceAvailable(Guest guest, int sitPositionIndex)
    {
        availableSittingPositionIndex.Add(sitPositionIndex);

        var numberOfStars = Mathf.Clamp(Mathf.RoundToInt(guest.NumberOfStars), 0, 5);
        var messages =
            numberOfStars == 0 ? generalQuips.exitMessages1 :
            numberOfStars == 1 ? generalQuips.exitMessages2 :
            numberOfStars == 2 ? generalQuips.exitMessages3 :
            numberOfStars == 3 ? generalQuips.exitMessages4 :
            numberOfStars == 4 ? generalQuips.exitMessages5 :
            numberOfStars == 5 ? generalQuips.exitMessages6 : null;

        var exitMessage = messages[UnityEngine.Random.Range(0, messages.Length)];
        CanvasController.I.ShowNotification(guest, exitMessage, numberOfStars, guest.NumberOfWishes);

        CanvasController.I.AddStars(numberOfStars);
    }

    public void NoZitoNoParty(Guest guest)
    {
        var message = generalQuips.noZitoNoParty[UnityEngine.Random.Range(0, generalQuips.noZitoNoParty.Length)];
        CanvasController.I.ShowNotification(guest, message, 0, 1);
        CanvasController.I.AddStars(-1);
    }

    public WishQuip GetWishQuip(GuestWish.GuestWishType wishType)
    {
        for (int i = 0; i < wishQuips.Length; i++)
        {
            if (wishQuips[i].wishType == wishType)
                return wishQuips[i];
        }

        return null;
    }

    #endregion

    #region Private

    private void InitializeSeatingPositions()
    {
        // fill in available sitting index
        for (int i = 0; i < guestSeats.Length; i++)
        {
            availableSittingPositionIndex.Add(i);
        }

        if (LimitGuestNumber > 0)
        {
            var limit = Mathf.Min(LimitGuestNumber, availableSittingPositionIndex.Count);
            availableSittingPositionIndex.Shuffle();
            availableSittingPositionIndex.RemoveRange(0, availableSittingPositionIndex.Count - limit);
        }
    }

    private void StartSlava()
    {
        timeToGenerateNewGuests = DelayForGeneratingGuests;

        StartCoroutine(WaitASecond());

        slavaHasStarted = true;
    }

    IEnumerator WaitASecond()
    {
        yield return new WaitForSeconds(1);

        // generate initial guests
        for (int i = 0; i < InitialGuestCount; i++)
        {
            GenerateNewGuest();
        }
    }

    private void GenerateNewGuest()
    {
        if (availableSittingPositionIndex.Count == 0)
        {
            Debug.Log("No sitting position");
            return;
        }

        Debug.Log("Generating new guest");
        var index = availableSittingPositionIndex[UnityEngine.Random.Range(0, availableSittingPositionIndex.Count)];
        availableSittingPositionIndex.Remove(index);

        var guestPrefab = GuestPrefabs[UnityEngine.Random.Range(0, GuestPrefabs.Count)];
        var guestObject = Instantiate(guestPrefab);
        guestObject.transform.SetParent(transform);
        guestObject.transform.position = entrancePosition.position;

        var guest = guestObject.GetComponent<Guest>();

        var guestAgent = Instantiate(guestAgentPrefab).GetComponent<GuestAI>();
        guestAgent.transform.position = entrancePosition.position;
        guestAgent.exitDestination = exitPosition;
        guestAgent.zitoDestination = zitoPosition;
        guest.AI = guestAgent;

        guest.AssignSeat(index, guestSeats[index]);

        CanvasController.I.AddNewGuestWish(guest);

        var entryMessage = generalQuips.entryMessages[UnityEngine.Random.Range(0, generalQuips.entryMessages.Length)];
        CanvasController.I.ShowNotification(guest, entryMessage);
    }

    #endregion

#if UNITY_EDITOR

    private void OnGUI()
    {
        GUILayout.Window(0, new Rect(10, 500, 200, 200), Window, "Debug");
    }

    void Window(int id)
    {
        if (GUILayout.Button("Spawn Guest"))
        {
            GenerateNewGuest();
        }

        autoSpawnGuests = GUILayout.Toggle(autoSpawnGuests, "Auto Spawn Guests");
        GUI.enabled = false;
        slavaHasStarted = GUILayout.Toggle(slavaHasStarted, "Slava started");
        GUI.enabled = true;
        if (GUILayout.Button("Spawn Fleka"))
        {
            var player = GameObject.Find("Player 1");
            Database.e.CreateFleka(player.transform.position);
        }
        if (GUILayout.Button("Extinguish Candle"))
        {
            Candle.e.Extinguish();
        }
    }

    private void OnDrawGizmos()
    {
        var color = new Color(1f, 1f, 0f, 0.5f);
        if (guestSeats.Length > 0)
            for (int i = 0; i < guestSeats.Length; i++)
            {
                Gizmos.color = color;
                Gizmos.DrawSphere(guestSeats[i].transform.position, 1f);
                UnityEditor.Handles.Label(guestSeats[i].transform.position, (i + 1).ToString());
            }
    }

#endif
}
