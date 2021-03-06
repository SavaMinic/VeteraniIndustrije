﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings : ScriptableObject
{
    
    #region Simple singleton

    private static LevelSettings instance;

    public static LevelSettings I
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<LevelSettings>(typeof(LevelSettings).Name);
            }
            return instance;
        }
    }

    #endregion
    
    #region Definition

    [System.Serializable]
    public struct InteractableObjectNames
    {
        public string ItemName;
        public GuestWish.GuestWishType WishType;
    }
    
    #endregion

    public List<LevelAsset> LevelsData;

    public List<InteractableObjectNames> InteractableObjectsNames;
}
