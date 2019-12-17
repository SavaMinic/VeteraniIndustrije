using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Downloader")]
public class QuipsDownloadData : ScriptableObject
{
    public string address;

    List<string[]> all;

    [ContextMenu("Download")]
    public void DownloadQuips()
    {
        string csv = DownloadCSV.FromSheets(address, "tsv");

#if DEBUG_TEXT
        Debug.Log(csv);
#endif

        string[] lines = csv.Split('\n');

        all = new List<string[]>();

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            all.Add(lines[i].Split('\t'));
        }

#if DEBUG_TEXT
        for (int i = 0; i < all.Count; i++)
        {
            string o = "|";
            for (int j = 0; j < all[i].Length; j++)
            {
                o += all[i][j] + "|";
            }

            Debug.Log(o);
        }
#endif

#if DEBUG_TAG
        var strs = FindByTag("NO_ZITO_NO_PARTY");

        for (int i = 0; i < strs.Length; i++)
        {
            Debug.Log(strs[i]);
        }
#endif

        var consumables = Resources.LoadAll("Consumables");

        foreach (var consumable in consumables)
        {
            SetConsumable((Consumable)consumable);
        }

        var wishQuips = Resources.LoadAll("WishQuips");

        foreach (var wishQuip in wishQuips)
        {
            SetWishQuip((WishQuip)wishQuip);
        }
    }

    void SetConsumable(Consumable consumable)
    {
        string tagName = consumable.name.ToUpper();
        consumable.quips.request = FindByTag(tagName + "_REQUEST");
        consumable.quips.perfect = FindByTag(tagName + "_PERFECT");
        consumable.quips.wrongTemperature = FindByTag(tagName + "_WRONG_TEMPERATURE");
        consumable.quips.wrongConsumable = FindByTag(tagName + "_WRONG_CONSUMABLE");
        consumable.quips.tooLate = FindByTag(tagName + "_TOO_LATE");
        UnityEditor.EditorUtility.SetDirty(consumable);

        Debug.Log($"Set {consumable.name} consumable quips");
    }

    void SetWishQuip(WishQuip wishQuip)
    {
        string tagName = wishQuip.name.ToUpper();
        wishQuip.request = FindByTag(tagName + "_REQUEST");
        wishQuip.success = FindByTag(tagName + "_SUCCESS");
        wishQuip.failure = FindByTag(tagName + "_FAILURE");
        wishQuip.tooLate = FindByTag(tagName + "_TOO_LATE");
        UnityEditor.EditorUtility.SetDirty(wishQuip);

        Debug.Log($"Set {wishQuip.name} wish quips");
    }

    string[] FindByTag(string tag)
    {
        bool read = false;

        List<string> found = new List<string>();

        tag = "#" + tag;

        for (int i = 0; i < all.Count; i++)
        {
            if (all[i][0] == tag)
            {
                read = true;
                continue;
            }

            if (read)
            {
                if (string.IsNullOrWhiteSpace(all[i][0]) || all[i][0].StartsWith("#"))
                {
                    break;
                }
                else
                {
                    found.Add(all[i][0]);
                }
            }
        }

        if (!read)
            Debug.LogError($"Tag {tag} not found");

        return found.ToArray();
    }
}
