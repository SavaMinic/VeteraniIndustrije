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

    public List<Vector3> GuestSittingPositions = new List<Vector3>();
    
    public List<GameObject> GuestPrefabs = new List<GameObject>();

    public int InitialGuestCount;
    public float DelayForGeneratingGuests;
    
    private float timeToGenerateNewGuests;

    private List<int> availableSittingPositionIndex = new List<int>();
    
    #endregion

    #region Mono

    private void Start()
    {
        timeToGenerateNewGuests = DelayForGeneratingGuests;
        // fill in available sitting index
        for (int i = 0; i < GuestSittingPositions.Count; i++)
        {
            availableSittingPositionIndex.Add(i);
        }
        
        // generate initial guests
        for (int i = 0; i < InitialGuestCount; i++)
        {
            GenerateNewGuest();
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

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

    public void SittingPlaceAvailable(int sitPositionIndex)
    {
        availableSittingPositionIndex.Add(sitPositionIndex);
    }

    #endregion

    #region Private

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
        guest.SitHere(index, GuestSittingPositions[index]);
        
        CanvasController.I.AddNewGuestWish(guest);
    }

    #endregion
    
    #if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        var color = new Color(1f, 1f, 0f, 0.5f);
        for (int i = 0; i < GuestSittingPositions.Count; i++)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(GuestSittingPositions[i], 1f);
        }
    }

#endif
}
