using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Guest : MonoBehaviour
{
    public enum GuestType
    {
        Default,
        // just testing
        DrunkGuy,
        Priest,
        CrazyAunt
    }

    public enum GuestState
    {
        WaitingAtTheDoor,
        WaitingForZito,
        WalkingIn,
        WaitingForService,
        Delay,
        GoingOut,
        Exit
    }

    public GuestType Type;
    public int NumberOfWishes;
    public float WaitingTimePerWish;
    public float DelayAfterWish;
    public float InitialDelay = 5f;

    private bool hasDirtyShoes;
    public float dirtyShoesChance;

    public GuestState CurrentState;
    public List<GuestWish> AllWishes = new List<GuestWish>();

    public GuestWish CurrentWish => CurrentState == GuestState.WaitingForZito ? entryWish : AllWishes.Find(w => !w.IsFinished);
    public GuestWish LastFinishedWish => AllWishes.FindLast(w => w.IsFinished);

    public Consumer consumer;

    const float TIME_BETWEEN_RINGS = 6;

    private float timeForNewWish;
    private Vector3 sittingPosition;
    private int sittingIndex;

    private SpriteRenderer spriteRenderer;

    private float timeForTvWish = -1f;
    private float timeForWindowWish = -1f;

    public int NumberOfStars => AllWishes.Count(w => w.IsSuccess.HasValue && w.IsSuccess.Value);

    private bool isFadeOut;

    public GuestAI AI { private get; set; }
    Seat seat;

    bool followAI = true;

    private GuestWish entryWish;

    const float ZITO_RANGE = 3;

    #region Mono

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.transform.localScale = Vector3.one;

        hasDirtyShoes = GameController.I.Level.IsRaining && Random.value <= dirtyShoesChance;
        if (hasDirtyShoes)
            Debug.LogWarning("DIRTY SHOES");
    }

    private void Start()
    {
        WalkIn();
    }



    private void Update()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        // Follow guest

        if (followAI)
        {
            transform.position = AI.transform.position + Vector3.up;
        }

        switch (CurrentState)
        {
            case GuestState.WaitingAtTheDoor:
                RingTillDoorIsOpen();
                DoMoveFlipping();

                // Wait for the domacin to open the door
                if (IsCloseTo(AI.zitoDestination.position, ZITO_RANGE))
                {
                    CurrentState = GuestState.WaitingForZito;
                    AI.Stop();
                    consumer.EnablePlayerInteraction(true);

                    if (entryWish == null)
                    {
                        entryWish = new GuestWish(GuestWish.GuestWishType.Zito, 30f);
                        AllWishes.Insert(0, entryWish);
                        entryWish.ActivateWish();
                    }
                }
                break;
            case GuestState.WalkingIn:
                DoMoveFlipping();

                if (IsCloseTo(seat.transform.position))
                {
                    Sit();
                    Delay(Random.Range(0f, 2f) * InitialDelay);
                }
                break;
            case GuestState.GoingOut:

                if (Vector3.Distance(transform.position, AI.exitDestination.position) < 1.5f)
                {
                    CurrentState = GuestState.Exit;
                }
                return;
            case GuestState.Exit:
                if (!isFadeOut)
                {
                    isFadeOut = true;
                    Debug.Log(sittingIndex + " BYE!");
                    StartCoroutine(DelayDestroy(1.2f));
                }
                break;
            case GuestState.Delay:
                // currently delayed after the wish
                timeForNewWish -= Time.deltaTime;
                if (timeForNewWish <= 0f)
                {
                    Debug.Log(sittingIndex + " end delay, new wish");
                    RequestingWish();
                }
                break;
            case GuestState.WaitingForZito:
            case GuestState.WaitingForService:
                var currentWish = CurrentWish;

                // update all wishes
                for (int i = 0; i < AllWishes.Count; i++)
                {
                    AllWishes[i].UpdateWish(Time.deltaTime);
                }

                // check if there is no current wish
                if (currentWish == null)
                {
                    // if wishes count is at max, leave
                    if (AllWishes.Count == NumberOfWishes)
                    {
                        GoHome();
                    }
                    else
                    {
                        GenerateNewWish();
                    }
                }
                else if (currentWish.IsFinished)
                {
                    // if current wish is finished, delay the next one
                    Debug.Log(sittingIndex + " Timeout active wish, start delay");
                    Delay(DelayAfterWish);
                }
                // if the current wish is not active, activate it
                else if (!currentWish.IsActive)
                {
                    currentWish.ActivateWish();

                }
                else if (currentWish.IsTvWish)
                {
                    var selectedChannel = TV.SelectedChannel;
                    // correct channel is activated
                    if (selectedChannel == currentWish.Type)
                    {
                        // we have time set
                        if (timeForTvWish >= 0)
                        {
                            timeForTvWish -= Time.deltaTime;
                            if (timeForTvWish <= 0f)
                            {
                                WishQuip quip = GuestManager.I.GetWishQuip(currentWish.Type);
                                string quipText = quip ? quip.GetSuccess() : "";

                                FinishActiveWish(message: quipText);
                            }
                        }
                        else
                        {
                            // set the timer
                            timeForTvWish = 3f;
                        }
                    }
                    else
                    {
                        timeForTvWish = -1f;
                    }
                }
                else if (currentWish.IsWindowWish)
                {
                    var isWindowOpen = Prozor.IsProzorOpen;
                    // windows is as we like it
                    if ((currentWish.Type == GuestWish.GuestWishType.OpenWindow && isWindowOpen)
                        || (currentWish.Type == GuestWish.GuestWishType.CloseWindow && !isWindowOpen))
                    {
                        // we have time set
                        if (timeForWindowWish >= 0)
                        {
                            timeForWindowWish -= Time.deltaTime;
                            if (timeForWindowWish <= 0f)
                            {
                                WishQuip quip = GuestManager.I.GetWishQuip(currentWish.Type);
                                string quipText = quip ? quip.GetSuccess() : "";

                                FinishActiveWish(message: quipText);
                            }
                        }
                        else
                        {
                            // set the timer
                            timeForWindowWish = 3f;
                        }
                    }
                    else
                    {
                        timeForWindowWish = -1f;
                    }
                }
                else if (currentWish.IsDrinkWish || currentWish.IsFoodWish)
                {
                    var container = currentWish.IsDrinkWish ? consumer.drinkContainer : consumer.foodContainer;

                    const float minimumAmount = 0.5f;

                    bool correctConsumable = currentWish.Type == container.type.wishType;
                    bool correctTemperature = container.IsAtGoodTemperature();


                    if (container.amount > minimumAmount)
                    {
                        Consumable.Quips q = container.type.quips;

                        if (correctConsumable && correctTemperature)
                            FinishActiveWish(true, q.perfect[Random.Range(0, q.perfect.Length)]);
                        else if (correctConsumable)
                            FinishActiveWish(false, q.wrongTemperature[Random.Range(0, q.wrongTemperature.Length)]);
                        else
                            FinishActiveWish(false, q.wrongConsumable[Random.Range(0, q.wrongConsumable.Length)]);
                    }
                }

                if (CurrentState == GuestState.WaitingForZito && entryWish.IsFinished)
                {
                    if (entryWish.IsSuccess.HasValue && entryWish.IsSuccess.Value)
                    {
                        // go in
                        AllWishes.RemoveAt(0);
                        AI.GoToSeat();
                        followAI = true;
                        consumer.EnablePlayerInteraction(false);
                        CurrentState = GuestState.WalkingIn;

                        if (hasDirtyShoes)
                        {
                            StartCoroutine(MakeFleka(Random.Range(1.2f, 3f)));
                        }
                    }
                    else
                    {
                        // just rage-quit
                        GoHome(false);
                    }
                }

                break;
        }
    }

    #endregion

    #region Public

    public void AssignSeat(int sitIndex, Seat seat)
    {
        // keep the y position
        //Vector3 v = seat.transform.position;
        //v.y = transform.position.y;
        //seat.transform.position = v;

        // save data
        sittingPosition = seat.transform.position;
        sittingIndex = sitIndex;

        this.seat = seat;

        AI.seatDestination = seat.transform;
        AI.GoToSeat();
    }

    void Sit()
    {
        AI.Stop();
        followAI = false;
        consumer.EnablePlayerInteraction(true);

        spriteRenderer.flipX = seat.isFlipped;
        transform.position = seat.transform.position;
        Debug.Log("Sat down");
    }

    public void FinishActiveWish(bool success = true, string message = "")
    {
        var activeWish = CurrentWish;
        if (activeWish == null
            || !(CurrentState == GuestState.WaitingForService || CurrentState == GuestState.WaitingForZito))
            return;

        Debug.Log(sittingIndex + " Finished active wish, start delay");
        activeWish.FinishWish(success);

        if (CurrentState == GuestState.WaitingForService)
        {
            Delay(DelayAfterWish);
        }

        if (!string.IsNullOrEmpty(message))
            CanvasController.I.ShowNotification(this, message);
    }

    #endregion

    #region Private

    float lastRingTime;

    void WalkIn()
    {
        consumer.EnablePlayerInteraction(false);
        CurrentState = GuestState.WaitingAtTheDoor;

        lastRingTime = -Mathf.Infinity;

        RingTillDoorIsOpen();
    }

    void RingTillDoorIsOpen()
    {
        if (!Database.e.entranceDoor.IsOpen && Time.time - lastRingTime > TIME_BETWEEN_RINGS)
        {
            lastRingTime = Time.time;
            Bell.e.Ring();
        }
    }

    private void Delay(float delayTime)
    {
        CurrentState = GuestState.Delay;
        timeForNewWish = delayTime;
    }

    void DoMoveFlipping()
    {
        spriteRenderer.flipX = AI.VelocityX() >= -0.05f;
    }

    private void GenerateNewWish()
    {
        GuestWish.GuestWishType wishType = GuestWish.GuestWishType.Random;
        var wish = new GuestWish(wishType, WaitingTimePerWish);
        AllWishes.Add(wish);
        wish.ActivateWish();

        // Display a request quip

        string quipText = "";
        if (wish.IsDrinkWish || wish.IsFoodWish)
        {
            Consumable.Quips quip = null;
            foreach (var consumable in Database.e.consumables)
            {
                if (consumable.wishType == wish.Type)
                {
                    quip = consumable.quips;
                    break;
                }
            }

            if (quip != null)
                quipText = quip.request[Random.Range(0, quip.request.Length)];
        }
        else
        {
            WishQuip quip = GuestManager.I.GetWishQuip(wish.Type);
            if (quip != null)
            {
                quipText = quip.GetRequest();
            }
        }

        if (quipText != "") CanvasController.I.ShowNotification(this, quipText);
    }

    private void RequestingWish()
    {
        CurrentState = GuestState.WaitingForService;
    }

    private void GoHome(bool wasSitting = true)
    {
        consumer.EnablePlayerInteraction(false);

        Debug.Log(sittingIndex + " Going home");
        CurrentState = GuestState.GoingOut;
        if (wasSitting)
        {
            GuestManager.I.SittingPlaceAvailable(this, sittingIndex);
        }
        else
        {
            GuestManager.I.NoZitoNoParty(this);
        }

        if (!followAI)
        {
            AI.GoToExit();
            followAI = true;
        }
    }

    private IEnumerator DelayDestroy(float delay)
    {
        var startColor = Color.white;
        var endColor = new Color(1f, 1f, 1f, 0f);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / delay)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        Destroy(gameObject);
        Destroy(AI.gameObject);
    }

    private IEnumerator MakeFleka(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        Database.e.CreateFleka(transform.position);
    }

    bool IsCloseTo(Vector3 point, float range = 0.1f)
    {
        Vector2 v1 = new Vector2(transform.position.x, transform.position.z);
        Vector2 v2 = new Vector2(point.x, point.z);

        return Vector2.Distance(v1, v2) < range;
    }

    #endregion
}
