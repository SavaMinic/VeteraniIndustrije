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
        WalkingIn,
        WaitingForService,
        Consuming,
        Delay,
        GoingOut,
    }

    public GuestType Type;
    public int NumberOfWishes;
    public float WaitingTimePerWish;
    public float DelayAfterWish;
    public float InitialDelay = 5f;

    public GuestState CurrentState;
    public List<GuestWish> AllWishes = new List<GuestWish>();

    public GuestWish CurrentWish => AllWishes.Find(w => !w.IsFinished);
    public GuestWish LastFinishedWish => AllWishes.FindLast(w => w.IsFinished);

    public Consumer consumer;

    private float timeForNewWish;
    private Vector3 sittingPosition;
    private int sittingIndex;

    private SpriteRenderer spriteRenderer;

    private float timeForTvWish = -1f;
    private float timeForWindowWish = -1f;

    #region Mono

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.transform.localScale = Vector3.one;
    }

    private void Start()
    {

        // TODO: do the walking first
        Delay(Random.Range(0f, 2f) * InitialDelay);
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        switch (CurrentState)
        {
            case GuestState.WalkingIn:
                // TODO: walking
                break;
            case GuestState.GoingOut:
                // TODO: walking
                Debug.Log(sittingIndex + " BYE!");
                Destroy(gameObject);
                return;
            case GuestState.Consuming:

                return;
            case GuestState.Delay:
                // currently delayed after the wish
                timeForNewWish -= Time.deltaTime;
                if (timeForNewWish <= 0f)
                {
                    Debug.Log(sittingIndex + " end delay, new wish");
                    RequestingWish();
                }
                break;
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
                                FinishActiveWish();
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
                    var isWindowOpen = Prozor.IsOpen;
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
                                FinishActiveWish();
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
                else if (currentWish.IsDrinkWish)
                {
                    var dc = consumer.drinkContainer;

                    const float minimumAmount = 0.5f;

                    bool correctDrink = false;

                    if (dc.amount > minimumAmount)
                    {
                        if (currentWish.Type == GuestWish.GuestWishType.Water
                            && dc.drinkType == Drink.Water)
                            correctDrink = true;

                        if (currentWish.Type == GuestWish.GuestWishType.Coffee
                            && dc.drinkType == Drink.Coffee)
                            correctDrink = true;

                        if (currentWish.Type == GuestWish.GuestWishType.Rakija
                            && dc.drinkType == Drink.Rakija)
                            correctDrink = true;

                        if (currentWish.Type == GuestWish.GuestWishType.ZutiSok
                            && dc.drinkType == Drink.ZutiSok)
                            correctDrink = true;

                        if (currentWish.Type == GuestWish.GuestWishType.CrniSok
                            && dc.drinkType == Drink.CrniSok)
                            correctDrink = true;

                        if (correctDrink)
                        {
                            FinishActiveWish();
                        }
                    }
                }
                break;
        }
    }

    #endregion

    #region Public

    public void SitHere(int sitIndex, Vector3 sitPosition, bool isFlipped)
    {
        // keep the y position
        sitPosition.y = transform.position.y;
        
        // save data
        sittingPosition = sitPosition;
        sittingIndex = sitIndex;
        
        spriteRenderer.flipX = isFlipped;
        
        // TODO: set target position for guest, and he should move to it
        transform.position = sittingPosition;
    }

    public void FinishActiveWish()
    {
        var activeWish = CurrentWish;
        if (activeWish == null || CurrentState != GuestState.WaitingForService)
            return;
        
        Debug.Log(sittingIndex + " Finished active wish, start delay");
        activeWish.FinishWish();
        Delay(DelayAfterWish);
    }

    #endregion

    #region Private

    private void Delay(float delayTime)
    {
        CurrentState = GuestState.Delay;
        timeForNewWish = delayTime;
    }

    private void GenerateNewWish()
    {
        GuestWish.GuestWishType wishType = GuestWish.GuestWishType.Random;
        var wish = new GuestWish(wishType, WaitingTimePerWish);
        AllWishes.Add(wish);
        wish.ActivateWish();
    }

    private void RequestingWish()
    {
        CurrentState = GuestState.WaitingForService;
    }

    private void GoHome()
    {
        Debug.Log(sittingIndex + " Going home");
        CurrentState = GuestState.GoingOut;
        GuestManager.I.SittingPlaceAvailable(sittingIndex);
    }

    #endregion
}
