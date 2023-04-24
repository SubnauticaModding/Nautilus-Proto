using System;
using Story;
using UnityEngine;
using static HandReticle;

namespace Nautilus.Handlers;

#if SUBNAUTICA
/// <summary>
/// <para>A handler class for interacting with all of the major goal systems in Subnautica, which are essential in the game progression. Utilizes the following systems:</para>
/// <list type="bullet">
///    <item>
///        <term><see cref="ItemGoalTracker"/></term>
///        <description>Completes an <see cref="ItemGoal"/> (or multiple) when an object with the given TechType is picked up, equipped, or crafted through the Mobile Vehicle Bay.</description>
///    </item>
///    <item>
///        <term><see cref="BiomeGoalTracker"/></term>
///        <description>Completes a <see cref="BiomeGoal"/> when the player stays in a given biome for a specified period of time.</description>
///    </item>
///    <item>
///        <term><see cref="LocationGoalTracker"/></term>
///        <description>Completes a <see cref="LocationGoal"/> when the player stays within range of a certain position for a specified period of time.</description>
///    </item>
///    <item>
///        <term><see cref="CompoundGoalTracker"/></term>
///        <description>Completes a <see cref="CompoundGoal"/> when all required "precondition" goals have been completed.</description>
///    </item>
///    <item>
///        <term><see cref="OnGoalUnlockTracker"/></term>
///        <description>Handles the completion of a goal with the data defined in an <see cref="OnGoalUnlock"/> object. Defines the unlocked blueprints, signals, items, and achievements that should be unlocked.</description>
///    </item>
///    <item>
///        <term><see cref="StoryGoalCustomEventHandler"/></term>
///        <description>Handles arbitrary code that is executed after completing a goal, for use in more specific story events.</description>
///    </item>
/// </list>
/// </summary>
public static class StoryGoalHandler
{
    /// <summary>
    /// <para>Registers a goal that is completed when an object with the given TechType is picked up, equipped, or crafted through the Mobile Vehicle Bay.</para>
    /// <para>Important: This method can be called <b>as many times as needed</b> to add different goals to the same TechType.</para>
    /// </summary>
    /// <param name="key">The unique identifier, required for all types of StoryGoals.</param>
    /// <param name="goalType">If assigned a value other than 'Story', this will determine the automatic response to being triggered. Can add a PDA log, Radio message or Databank entry.</param>
    /// <param name="delay">StoryGoal listeners will not be notified until this many seconds after the goal is completed.</param>
    /// <param name="techType">The TechType that causes this goal to trigger, when picked up, equipped or crafted through the Mobile Vehicle Bay.</param>
    /// <returns></returns>
    public static ItemGoal RegisterItemGoal(string key, StoryGoalType goalType, float delay, TechType techType)
    {
        var goal = new ItemGoal() { key = key, goalType = goalType.GetActualValue(), delay = delay, techType = techType };
    }

    /// <summary>
    /// Registers a goal that is completed when the player stays in a given biome for a specified period of time.
    /// </summary>
    /// <param name="key">The unique identifier, required for all types of StoryGoals.</param>
    /// <param name="goalType">If assigned a value other than 'Story', this will determine the automatic response to being triggered. Can add a PDA log, Radio message or Databank entry.</param>
    /// <param name="delay">StoryGoal listeners will not be notified until this many seconds after the goal is completed.</param>
    /// <param name="biomeName">The biome that must be entered to trigger this goal.</param>
    /// <param name="minStayDuration">The minimum amount of time the player must stay in the given biome.</param>
    /// <returns></returns>
    public static BiomeGoal RegisterBiomeGoal(string key, StoryGoalType goalType, float delay, string biomeName, float minStayDuration)
    {
        var goal = new BiomeGoal() { key = key, goalType = goalType.GetActualValue(), delay = delay, biome = biomeName, minStayDuration = minStayDuration };
    }

    /// <summary>
    /// Registers a goal that is completed when the player stays within range of a certain position for a specified period of time.
    /// </summary>
    /// <param name="key">The unique identifier, required for all types of StoryGoals.</param>
    /// <param name="goalType">If assigned a value other than 'Story', this will determine the automatic response to being triggered. Can add a PDA log, Radio message or Databank entry.</param>
    /// <param name="delay">StoryGoal listeners will not be notified until this many seconds after the goal is completed.</param>
    /// <param name="position"></param>
    /// <param name="range"></param>
    /// <param name="minStayDuration"></param>
    /// <returns></returns>
    public static LocationGoal RegisterLocationGoal(string key, StoryGoalType goalType, float delay, Vector3 position, float range, float minStayDuration)
    {
        var goal = new LocationGoal() { key = key,}
    }

    public static CompoundGoal RegisterCompoundGoal(string key, StoryGoalType goalType, float delay, params string[] requiredGoals)
    {
        
    }

    public static OnGoalUnlock RegisterOnGoalUnlockData(string goal, UnlockBlueprintData[] blueprints = null, UnlockSignalData[] signals = null, UnlockItemData[] items = null, GameAchievements.Id[] achievements = null)
    {

    }

    public static void RegisterStoryGoalCustomEvent(string goalName, Action actionOnComplete)
    {

    }

    private static Story.GoalType GetActualValue(this StoryGoalType type)
    {
        return (Story.GoalType)type;
    }
}
/// <summary>
/// Determines the primary effect of a StoryGoal's completion. This is a documented wrapper for the original <see cref="Story.GoalType"/> enum, for use in the <see cref="StoryGoalHandler"/>.
/// </summary>
public enum StoryGoalType
{
    /// <summary>
    /// Adds a PDA Log message on completion using the StoryGoal's key. Also see <see cref="PDAHandler.AddLogEntry"/> to create a PDA log entry.
    /// </summary>
    PDA,
    /// <summary>
    /// On completion, StoryGoals of this type will add a pending radio message using the StoryGoal's key. Also see <see cref="PDAHandler.AddLogEntry"/> to create a Radio signal.
    /// </summary>
    Radio,
    /// <summary>
    /// Adds a Databank entry on completion using the StoryGoal's key. Also see <see cref="PDAHandler.AddCustomScannerEntry(PDAScanner.EntryData)"/> or its overloads to create an encyclopedia entry.
    /// </summary>
    Encyclopedia,
    /// <summary>
    /// Generic GoalType with no default effects. Primarily used for tracking story progress, or advanced systems such as <see cref="StoryGoalCustomEventHandler"/>, <see cref="OnGoalUnlockTracker"/> and <see cref="CompoundGoalTracker"/>.
    /// </summary>
    Story
}
#endif