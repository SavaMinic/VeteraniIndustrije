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

    private List<string> entryMessages = new List<string>
    {
        "Srecna slava, domacine!",
        "Pomaze bog, komsija!",
        "Domacice, sve najbolje!",
        "Nek je srecna slava!",
        //"Dajte da popijemo poneku!",
        //"Komsinice, stavi kafu!",
    };

    private List<List<string>> exitMessages = new List<List<string>>
    {
        new List<string> { "?!?!?", "***** ** *****", "uff zurim kuci" },
        new List<string> { "Mlaka sarma", "Vruce pivo?" },
        new List<string> { "Bice bolje dogodine" },
        new List<string> { "Nit' smrdi nit' mirise", "Zbogom" },
        new List<string> { "Vise vina", "I dogodine" },
        new List<string> { "Svaka cast, domacine!", "Sve najbolje!" },
    };

    private List<string> noZitoNoPartyMessages = new List<string>
    {
        "Gde je zito?", "Nema zita za slavu?", "Nemam vremena za ovo"
    };

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
        var messages = exitMessages[numberOfStars];
        var exitMessage = messages[UnityEngine.Random.Range(0, messages.Count)];
        CanvasController.I.ShowNotification(guest, exitMessage, numberOfStars, guest.NumberOfWishes);

        CanvasController.I.AddStars(numberOfStars);
    }

    public void NoZitoNoParty(Guest guest)
    {
        var message = noZitoNoPartyMessages[UnityEngine.Random.Range(0, noZitoNoPartyMessages.Count)];
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

        // generate initial guests
        for (int i = 0; i < InitialGuestCount; i++)
        {
            GenerateNewGuest();
        }

        slavaHasStarted = true;
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

        var guest = guestObject.GetComponent<Guest>();

        var guestAgent = Instantiate(guestAgentPrefab).GetComponent<GuestAI>();
        guestAgent.transform.position = entrancePosition.position;
        guestAgent.exitDestination = exitPosition;
        guestAgent.zitoDestination = zitoPosition;
        guest.AI = guestAgent;

        guest.AssignSeat(index, guestSeats[index]);

        CanvasController.I.AddNewGuestWish(guest);

        var entryMessage = entryMessages[UnityEngine.Random.Range(0, entryMessages.Count)];
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
