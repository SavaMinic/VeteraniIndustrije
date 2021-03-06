﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Guest : MonoBehaviour
{
    public enum GuestState
    {
        WaitingAtTheDoor,
        Entered,
        WaitingForZito,
        WalkingIn,
        WaitingForService,
        Delay,
        GoingOut,
        Exit
    }

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

    public SpriteRenderer spriteRenderer { get; private set; }

    private float timeForTvWish = -1f;
    private float timeForWindowWish = -1f;

    public int NumberOfStars => AllWishes.Count(w => w.IsSuccess.HasValue && w.IsSuccess.Value);

    public bool isMarina;

    private bool isFadeOut;

    public GuestAI AI { private get; set; }
    Seat seat;

    bool followAI = true;

    private GuestWish entryWish;

    const float ZITO_RANGE = 3;

    float lastPromajaReaction;
    float timeBetweenPromajaReactions = 10;

    float lastPrljavoReaction;
    float timeBetweenPrljavoReactions = 10;

    #region Mono

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
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

        if (!isMarina)
        {
            if (Promaja.IsActive)
            {
                if (Time.time - lastPromajaReaction > timeBetweenPromajaReactions)
                {
                    lastPromajaReaction = Time.time;
                    timeBetweenPromajaReactions = Random.Range(5.0f, 10.0f);
                    if (Random.value > 0.5f) // not all guests should react
                        GuestManager.I.ShowPromajaQuip(this);
                }

                //lastPromajaReaction 1
            }

            if (Fleka.Prljavo)
            {
                if (Time.time - lastPrljavoReaction > timeBetweenPrljavoReactions)
                {
                    lastPrljavoReaction = Time.time;
                    timeBetweenPrljavoReactions = Random.Range(5.0f, 10.0f);
                    if (Random.value > 0.5f)
                        GuestManager.I.ShowPrljavoQuip(this);
                }
            }
        }

        switch (CurrentState)
        {
            case GuestState.WaitingAtTheDoor:
                RingTillDoorIsOpen();
                DoMoveFlipping();

                if (Database.e.entranceDoor.IsOpen)
                {
                    if (!isMarina)
                        GuestManager.I.ShowEntryMessage(this);
                    CurrentState = GuestState.Entered;
                }
                break;
            case GuestState.Entered:
                DoMoveFlipping();

                if (isMarina)
                {
                    AI.GoToSeat();
                    CurrentState = GuestState.WalkingIn;
                }
                // Wait for the domacin to open the door
                else if (IsCloseTo(AI.zitoDestination.position, ZITO_RANGE))
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
                    Debug.Log(sittingIndex + " Timeout active wish, start delay");

                    bool wasWaitingForZito = CurrentState == GuestState.WaitingForZito;

                    // Timeout
                    if (currentWish.IsSuccess.Value == false)
                    {
                        Debug.LogWarning($"Wish {currentWish.Type} has timed out!");

                        string quipText = "";
                        if (currentWish.IsWindowWish || currentWish.IsTvWish)
                        {
                            Debug.Log("Was window tv wish");

                            WishQuip quip = GuestManager.I.GetWishQuip(currentWish.Type);
                            Debug.Assert(quip != null, "Quip is null");
                            quipText = quip ? quip.GetToolate() : "";
                        }
                        else if (currentWish.IsFoodWish || currentWish.IsDrinkWish)
                        {
                            Debug.Log("Was drink food wish");

                            //var container = currentWish.IsDrinkWish ? consumer.drinkContainer : consumer.foodContainer;
                            Consumable.Quips q = null;
                            foreach (var consumable in Database.e.consumables)
                            {
                                if (consumable.wishType == currentWish.Type)
                                {
                                    q = consumable.quips;
                                    break;
                                }
                            }

                            //Consumable.Quips q = container.type.quips;
                            Debug.Assert(q != null, "Quip is null");
                            quipText = q.tooLate[Random.Range(0, q.tooLate.Length)];
                        }

                        Debug.Assert(currentWish != null, "Current wish is null before");
                        Debug.Assert(!string.IsNullOrEmpty(quipText), "No quip text");
                        FinishActiveWish(success: false, message: quipText, currentWish);
                        Delay(DelayAfterWish);
                    }

                    if (wasWaitingForZito)
                    {
                        //if (entryWish.IsSuccess.HasValue) // && entryWish.IsSuccess.Value
                        //{
                        Debug.Log($"Waiting for zito completed with {entryWish.IsSuccess.Value}");

                        // go in
                        //AllWishes.RemoveAt(0);
                        /*
                        if (!entryWish.IsSuccess.Value)
                        {
                            //entryWish.FinishWish(false);
                            WishQuip quip = GuestManager.I.GetWishQuip(currentWish.Type);
                            string quipText = quip ? quip.GetSuccess() : "";

                            FinishActiveWish(message: quipText);
                        }*/

                        AI.GoToSeat();
                        followAI = true;
                        consumer.EnablePlayerInteraction(false);
                        CurrentState = GuestState.WalkingIn;

                        if (hasDirtyShoes)
                        {
                            StartCoroutine(MakeFleka(Random.Range(1.2f, 3f)));
                        }
                        //}
                        //else
                        //{
                        // just rage-quit
                        //GoHome(false);
                        //}
                    }

                    //else
                    //Delay(DelayAfterWish);
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
                    var isWindowOpen = Prozor.IsProzorOpen || Database.e.balkonDoor.IsOpen;
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

                //Debug.Log($"Happening! {CurrentState}, {entryWish.IsFinished}");



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

    public void FinishActiveWish(bool success = true, string message = "", GuestWish wish = null)
    {
        var activeWish = wish ?? CurrentWish;

        //if (activeWish == null
        //  || !(CurrentState == GuestState.WaitingForService || CurrentState == GuestState.WaitingForZito))
        //return;

        Debug.Log(sittingIndex + " Finished active wish, start delay");
        Debug.Assert(activeWish != null, "Active wish is null");
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

    public void GoHomeImmediatelly()
    {
        GoHome();
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

        AI.GoToExit();
        followAI = true;
    }

    private IEnumerator DelayDestroy(float delay)
    {
        GuestManager.I.guestsServedCount++;

        // remove unfinished wishes
        for (int i = AllWishes.Count - 1; i >= 0; i--)
        {
            if (!AllWishes[i].IsFinished)
                AllWishes.RemoveAt(i);
        }

        GuestManager.I.allCompletedWishes.AddRange(AllWishes);

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
