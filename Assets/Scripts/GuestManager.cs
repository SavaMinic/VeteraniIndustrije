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

    [SerializeField] WishQuip[] wishQuips = null;
    [SerializeField] GeneralQuips generalQuips = null;

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
    bool slavaHasEnded;

    public int guestsServedCount = 0;
    public List<GuestWish> allCompletedWishes = new List<GuestWish>();

    public static bool showDebugWindow;

    #endregion

    #region Mono

    private void Start()
    {
        InitializeSeatingPositions();
    }

    private void Update()
    {
        if (!Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.F12) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                showDebugWindow = !showDebugWindow;
        }

        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        // Slava has ended
        if (Candle.e.burnProgress >= 1 && !slavaHasEnded)
        {
            EndSlava();
            return;
        }

        if (slavaHasEnded)
        {
            Guest[] guests = FindObjectsOfType<Guest>();
            if (guests.Length <= 0)
                GameController.I.EndGame();

            return;
        }

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

    void EndSlava()
    {
        Debug.Log("End of slava, FAJRONT!");
        AllGuestsLeave();

        slavaHasEnded = true;

        FindObjectOfType<MenuNavigation>().ShowSlavaEndedTut();
    }

    void AllGuestsLeave()
    {
        Guest[] guests = FindObjectsOfType<Guest>();
        foreach (var guest in guests)
            guest.GoHomeImmediatelly();
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
        GenerateNewGuest(GetRandomUniqueGuest());
    }

    private void GenerateNewGuest(GameObject guestPrefab)
    {
        if (availableSittingPositionIndex.Count == 0)
        {
            Debug.Log("No sitting position");
            return;
        }

        Debug.Log("Generating new guest");
        var index = availableSittingPositionIndex[UnityEngine.Random.Range(0, availableSittingPositionIndex.Count)];
        availableSittingPositionIndex.Remove(index);

        var guestObject = Instantiate(guestPrefab);
        guestObject.name = guestPrefab.name;
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
    }

    GameObject GetRandomUniqueGuest()
    {
        Guest[] guestsInScene = FindObjectsOfType<Guest>();
        List<Guest> guestsInSceneList = new List<Guest>(guestsInScene);
        while (true)
        {
            int rand = UnityEngine.Random.Range(0, GuestPrefabs.Count);
            GameObject randGuest = GuestPrefabs[rand];

            bool exists = false;
            for (int i = 0; i < guestsInScene.Length; i++)
            {
                if (guestsInScene[i].name == randGuest.name)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
                return randGuest;
        }
    }

    GameObject GetRandomGuest()
    {
        return GuestPrefabs[UnityEngine.Random.Range(0, GuestPrefabs.Count)];
    }

    GameObject GetMarina()
    {
        foreach (var guest in GuestPrefabs)
        {
            if (guest.GetComponent<Guest>().isMarina)
                return guest;
        }

        return null;
    }

    public void ShowEntryMessage(Guest guest)
    {
        var entryMessage = generalQuips.entryMessages[UnityEngine.Random.Range(0, generalQuips.entryMessages.Length)];
        CanvasController.I.ShowNotification(guest, entryMessage);
    }

    #endregion

    private void OnGUI()
    {
        if (!Application.isEditor && !showDebugWindow) return;

        GUILayout.Window(0, new Rect(10, 10, 300, 500), Window, "Debug");
    }

    void Window(int id)
    {
        if (GUILayout.Button("Spawn Guest"))
        {
            GenerateNewGuest();
        }
        if (GUILayout.Button("Spawn Marina"))
        {
            GenerateNewGuest(GetMarina());
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

        if (GUILayout.Button("Set test level"))
            GameController.I.LoadLevel("TestLevel");

        if (GUILayout.Button("FAJRONT"))
            AllGuestsLeave();

        if (GUILayout.Button("End Slava"))
            EndSlava();

        if (GUILayout.Button("End game NOW"))
            GameController.I.EndGame();

        GUILayout.Label("Loaded level: " + GameController.I.Level.name);
        GUILayout.Label("Time left: " + Candle.e.TimeLeft.ToString("F1"));

        var guests = FindObjectsOfType<Guest>();
        foreach (var guest in guests)
        {
            GUILayout.Label($"Guest: {guest.name}");
            for (int w = 0; w < guest.AllWishes.Count; w++)
            {
                GuestWish wish = guest.AllWishes[w];
                string success = wish.IsSuccess.HasValue ? wish.IsSuccess.Value ? "Success" : "Failure" : "Active";
                GUILayout.Label($"      W{w}: {guest.AllWishes[w].Type}, {success}");
            }
        }

        GUILayout.Label($"All completed wishes");

        for (int w = 0; w < allCompletedWishes.Count; w++)
        {
            GuestWish wish = allCompletedWishes[w];
            string success = wish.IsSuccess.HasValue ? wish.IsSuccess.Value ? "Success" : "Failure" : "Active";
            GUILayout.Label($"      W{w}: {wish.Type}, {success}");
        }
    }

#if UNITY_EDITOR
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
