using BepInEx.Logging;
using Nautilus.Patchers;
using Nautilus.Utility;
using UnityEngine;

namespace Nautilus.Handlers;

/// <summary>
/// A handler class for various scanner related data.
/// </summary>
public static class PDAHandler 
{

    /// <summary>
    /// Edits how many fragments must be scanned before unlocking the techtype's blueprint.
    /// </summary>
    /// <param name="techType">Can be either techtype of the fragment or the crafted item.</param>
    /// <param name="fragmentCount">The number of fragments to scan.</param>
    public static void EditFragmentsToScan(TechType techType, int fragmentCount)
    {
        if (fragmentCount <= 0)
        {
            fragmentCount = 1;
        }

        PDAPatcher.FragmentCount[techType] = fragmentCount;
    }

    /// <summary>
    /// Edits the time it takes to finish scanning a fragment.
    /// </summary>
    /// <param name="techType">Can be either techtype of the fragment or the crafted item.</param>
    /// <param name="scanTime">The relative time spent on scanning. Default value is 1.</param>
    public static void EditFragmentScanTime(TechType techType, float scanTime)
    {
        if (scanTime <= 0f)
        {
            scanTime = 1f;
        }

        PDAPatcher.FragmentScanTime[techType] = scanTime;
    }

    /// <summary>
    /// Adds in a custom <see cref="PDAScanner.EntryData"/>. ***Cannot be used to Change the values of a techtype that has data already!***
    /// </summary>
    /// <param name="entryData">The <see cref="PDAScanner.EntryData"/> of the entry. Must be populated when passed in.</param>
    public static void AddCustomScannerEntry(PDAScanner.EntryData entryData)
    {
        if (PDAPatcher.CustomEntryData.ContainsKey(entryData.key))
        {
            InternalLogger.Log($"{entryData.key} already has custom PDAScanner.EntryData. Replacing with latest.", LogLevel.Debug);
        }

        PDAPatcher.CustomEntryData[entryData.key] = entryData;
    }

    /// <summary>
    /// Adds in a custom <see cref="PDAScanner.EntryData"/>.
    /// </summary>
    /// <param name="key">The scanned object's <see cref="TechType"/>. In case of fragments, the fragment <see cref="TechType"/> is the key.</param>
    /// <param name="blueprint">The <paramref name="blueprint"/> when unlocked when scanned. In case of fragments, this is the actual <see cref="TechType"/> that unlocks when all fragments are scanned.</param>
    /// <param name="isFragment">Whether the <paramref name="key"/> is a fragment or not.</param>
    /// <param name="totalFragmentsRequired">The total amount of objects of <paramref name="key"/> that need to be scanned to unlock the <paramref name="blueprint"/> and <paramref name="encyclopediaKey"/>.</param>
    /// <param name="scanTime">The amount of time it takes to finish one scan. In seconds.</param>
    /// <param name="destroyAfterScan">Whether the object should be destroyed after the scan is finished.</param>
    /// <param name="encyclopediaKey">The key to the encyclopedia entry.</param>
    public static void AddCustomScannerEntry(TechType key, TechType blueprint, bool isFragment, string encyclopediaKey, int totalFragmentsRequired = 2, float scanTime = 2f, bool destroyAfterScan = true)
    {
        AddCustomScannerEntry(new PDAScanner.EntryData()
        {
            key = key,
            blueprint = blueprint,
            isFragment = isFragment,
            totalFragments = totalFragmentsRequired,
            scanTime = scanTime,
            destroyAfterScan = destroyAfterScan
        });
    }

    /// <summary>
    /// Adds a custom log entry.
    /// </summary>
    /// <param name="key">The key to refer to this entry.</param>
    /// <param name="languageKey">The subtitles language key for this entry.</param>
    /// <param name="icon">The icon that will be used in the Log tab for this entry. if <c>null</c> It will use the default log entry icon.</param>
    /// <param name="sound">The sound that will be played once this entry is triggered or played in the Log tab.<br/>
    /// If <c>null</c> the Play button in the Log tab will disappear and a sound wont play when this entry is triggered.</param>
    public static void AddLogEntry(string key, string languageKey, Sprite icon, FMODAsset sound)
    {
        PDALog.EntryData entry = new()
        {
            key = languageKey,
            icon = icon,
            sound = sound
        };
        PDALogPatcher.CustomEntryData[key] = entry;
    }

    /// <summary>
    /// Adds custom entry.
    /// </summary>
    /// <param name="entry">The <see cref="PDAEncyclopedia.EntryData"/> entry.</param>
    public static void AddEncyclopediaEntry(PDAEncyclopedia.EntryData entry)
    {
        if(PDAEncyclopediaPatcher.CustomEntryData.ContainsKey(entry.key))
        {
            InternalLogger.Log($"{entry.key} already has custom PDAEncyclopedia.EntryData. Replacing with latest.", LogLevel.Debug);
        }

        PDAEncyclopediaPatcher.CustomEntryData[entry.key] = entry;
    }

    /// <summary>
    /// Registers a single encylopedia entry into the game.
    /// </summary>
    /// <param name="key">Key (internal ID) of this PDA entry, primarily used for the language system.</param>
    /// <param name="path"><para>Path to this entry in the databank.</para>
    /// <para>To find examples of this string, open "...Subnautica\Subnautica_Data\StreamingAssets\SNUnmanagedData\LanguageFiles\English.json" and search for "EncyPath".</para>
    /// </param>
    /// <param name="title">Displayed title of the PDA entry in English. If set to null, you must implement your own translations. Language key is 'Ency_{<paramref name="key"/>}'.</param>
    /// <param name="desc">Displayed description of the PDA entry in English. If set to null, you must implement your own translations. Language key is 'EncyDesc_{<paramref name="key"/>}'.</param>
    /// <param name="image">Databank entry image. Can be null.</param>
    /// <param name="popupImage">Small popup image in the notification. Can be null.</param>
    public static void AddEncyclopediaEntry(string key, string path, string title, string desc, Texture2D image, Sprite popupImage)
    {
        if (string.IsNullOrEmpty(path))
        {
            InternalLogger.Error($"Attempting to add encyclopedia entry with null path for ClassId '{key}'!");
            return;
        }

        string[] encyNodes;
        if (string.IsNullOrEmpty(path))
            encyNodes = new string[0];
        else
            encyNodes = path.Split('/');


        var encyEntryData = new PDAEncyclopedia.EntryData()
        {
            key = key,
            nodes = encyNodes,
            path = path,
            image = image,
            popup = popupImage
        };

        if (!string.IsNullOrEmpty(title)) LanguageHandler.SetLanguageLine("Ency_" + key, title);
        if (!string.IsNullOrEmpty(desc)) LanguageHandler.SetLanguageLine("EncyDesc_" + key, desc);

        AddEncyclopediaEntry(encyEntryData);
    }
}