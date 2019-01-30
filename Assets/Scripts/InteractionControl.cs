using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionControl : MonoBehaviour
{
    #region Simple singleton

    private static InteractionControl instance;

    public static InteractionControl I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InteractionControl>();
            }
            return instance;
        }
    }

    #endregion

    [System.Serializable]
    public struct InteractionData
    {
        public GameObject interactionObject1;
        public GameObject interactionObject2;
    }

    public List<InteractionData> InteractionObjectData;

    public bool CanInteract(GameObject obj1, GameObject obj2)
    {
        if (obj1.GetComponent<Consumer>() || obj2.GetComponent<Consumer>())
            return true;
        return InteractionObjectData.Exists(d => 
            (d.interactionObject1 == obj1 && d.interactionObject2 == obj2)
            ||
            (d.interactionObject1 == obj2 && d.interactionObject2 == obj1)
        );
    }
}
