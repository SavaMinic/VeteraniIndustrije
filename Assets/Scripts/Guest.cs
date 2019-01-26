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

    private float timeForNewWish;

    #region Mono

    private void Start()
    {
        // TODO: do the walking first
        RequestingWish();
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        switch (CurrentState)
        {
            case GuestState.WalkingIn:
            case GuestState.GoingOut:
                // TODO: walking
                break;
            case GuestState.Delay:
                // currently delayed after the wish
                timeForNewWish -= Time.deltaTime;
                if (timeForNewWish <= 0f)
                {
                    Debug.Log("end delay, new wish");
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
                    Debug.Log("Timeout active wish, start delay");
                    Delay(DelayAfterWish);
                }
                // if the current wish is not active, activate it
                else if (!currentWish.IsActive)
                {
                    currentWish.ActivateWish();
                }
                break;
        }
    }

    #endregion

    #region Public

    public void FinishActiveWish()
    {
        var activeWish = CurrentWish;
        if (activeWish == null || CurrentState != GuestState.WaitingForService)
            return;
        
        Debug.Log("Finished active wish, start delay");
        activeWish.FinishWish();
        Delay(DelayAfterWish);
    }

    #endregion

    #region Private

    private void Delay(float delayTime)
    {
        CurrentState = GuestState.Delay;
        timeForNewWish = DelayAfterWish;
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
        Debug.Log("Going home");
        CurrentState = GuestState.GoingOut;
    }

    #endregion
}
