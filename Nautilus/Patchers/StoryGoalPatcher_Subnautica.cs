using System;
using System.Collections.Generic;
using HarmonyLib;
using Story;
using Nautilus.Utility;

namespace Nautilus.Patchers;

#if SUBNAUTICA
internal static class StoryGoalPatcher
{
    private static readonly List<ItemGoal> _itemGoals = new List<ItemGoal>();
    private static readonly List<BiomeGoal> _biomeGoals = new List<BiomeGoal>();
    private static readonly List<LocationGoal> _locationGoals = new List<LocationGoal>();
    private static readonly List<CompoundGoal> _compoundGoals = new List<CompoundGoal>();
    private static readonly List<OnGoalUnlock> _onGoalUnlocks = new List<OnGoalUnlock>();
    private static readonly List<StoryGoalCustomEventData> _storyGoalCustomEvents = new List<StoryGoalCustomEventData>();

    internal record StoryGoalCustomEventData(string goalName, Action completionAction);

    internal static void RegisterItemGoal(ItemGoal goal)
    {
        if (ItemGoalTracker.main != null)
        {
            TrackItemGoal(ItemGoalTracker.main, goal);
        }
        _itemGoals.Add(goal);
    }

    internal static void Patch(Harmony harmony)
    {
        PatchUtils.PatchClass(harmony);
        SaveUtils.RegisterOnQuitEvent(() => _locationGoals.ForEach(x => x.timeRangeEntered = -1f));
    }

    [PatchUtils.Postfix]
    [HarmonyPatch(typeof(ItemGoalTracker), nameof(ItemGoalTracker.Start))]
    private static void ItemGoalTrackerStartPostfix(ItemGoalTracker __instance)
    {
        foreach (var itemGoal in _itemGoals)
        {
            TrackItemGoal(__instance, itemGoal);
        }
    }

    [PatchUtils.Postfix]
    [HarmonyPatch(typeof(BiomeGoalTracker), nameof(BiomeGoalTracker.Start))]
    private static void BiomeGoalTrackerStartPostfix(BiomeGoalTracker __instance)
    {
        foreach (var biomeGoal in _biomeGoals)
        {
            TrackBiomeGoal(__instance, biomeGoal);
        }
    }

    [PatchUtils.Postfix]
    [HarmonyPatch(typeof(LocationGoalTracker), nameof(LocationGoalTracker.Start))]
    private static void LocationGoalTrackerStartPostfix(LocationGoalTracker __instance)
    {
        foreach (var locationGoal in _locationGoals)
        {
            TrackLocationGoal(__instance, locationGoal);
        }
    }

    // must be a prefix because we want to add all these goals BEFORE NotifyGoalComplete is called at the end of the method
    [PatchUtils.Prefix]
    [HarmonyPatch(typeof(CompoundGoalTracker), nameof(CompoundGoalTracker.Initialize))]
    private static void CompoundGoalTrackerInitializePrefix(CompoundGoalTracker __instance, HashSet<string> completedGoals)
    {
        foreach (var compoundGoal in _compoundGoals)
        {
            TrackCompoundGoal(__instance, completedGoals, compoundGoal);
        }
    }

    private static void TrackItemGoal(ItemGoalTracker tracker, ItemGoal goal)
    {
        var techType = goal.techType;
        if (tracker.goals.TryGetValue(techType, out var list)) // see if a techtype is already tracking this goal (look for existing dictionary entries)
        {
            if (list == null) list = new List<ItemGoal>(); // ensure list is valid
            foreach (var existingGoal in list) // check existing registered goals
            {
                if (existingGoal.key == goal.key)
                    return; // you don't want to have 2 goals registered with the same key
            }
            list.Add(goal);
        }
        else
        {
            tracker.goals.Add(techType, new List<ItemGoal>() { goal }); // add new dictionary entry
        }
    }

    private static void TrackBiomeGoal(BiomeGoalTracker tracker, BiomeGoal goal)
    {
        foreach (var g in tracker.goals)
        {
            if (goal.key == g.key)
            {
                InternalLogger.Warn($"Attempting to register multiple goals with the key '{goal.key}'!");
                return;
            }
        }
        tracker.goals.Add(goal);
    }

    private static void TrackLocationGoal(LocationGoalTracker tracker, LocationGoal goal)
    {
        foreach (var g in tracker.goals)
        {
            if (goal.key == g.key)
            {
                InternalLogger.Warn($"Attempting to register multiple goals with the key '{goal.key}'!");
                return;
            }
        }
        tracker.goals.Add(goal);
    }

    private static void TrackCompoundGoal(CompoundGoalTracker tracker, HashSet<string> completedGoals, CompoundGoal goal)
    {
        foreach (var g in tracker.goals)
        {
            if (goal.key == g.key)
            {
                InternalLogger.Warn($"Attempting to register multiple goals with the key '{goal.key}'!");
                return;
            }
        }
        if (!completedGoals.Contains(goal.key))
        {
            tracker.goals.Add(goal);
        }
    }
}
#endif