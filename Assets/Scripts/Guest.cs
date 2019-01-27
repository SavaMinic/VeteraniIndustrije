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
        Delay,
        GoingOut,
    }

    public GuestType Type;
    public int NumberOfWishes;
    public float WaitingTimePerWish;
    public float DelayAfterWish;

    public GuestState CurrentState;
    public List<GuestWish> AllWishes = new List<GuestWish>();

    public GuestWish CurrentWish => AllWishes.Find(w => !w.IsFinished);
    public GuestWish LastFinishedWish => AllWishes.FindLast(w => w.IsFinished);

    private float timeForNewWish;
    private Vector3 sittingPosition;
    private int sittingIndex;

    private SpriteRenderer spriteRenderer;

    private float timeForTvWish = -1f;

    #region Mono

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.transform.localScale = Vector3.one;
    }

    private void Start()
    {
        
        // TODO: do the walking first
        var initialDelay = Random.Range(0f, 2f) * DelayAfterWish;
        Delay(initialDelay);
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
