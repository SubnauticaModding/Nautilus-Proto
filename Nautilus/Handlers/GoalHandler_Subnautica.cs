using System;
using Story;

namespace Nautilus.Handlers;

#if SUBNAUTICA
/// <summary>
/// <para>A handler class for interacting with all of the major goal systems in Subnautica, which are essential in the game progression. Utilizes the following systems:</para>
/// <list type="bullet">
///    <item>
///        <term><see cref="ItemGoalTracker"/></term>
///        <description>Completes an <see cref="ItemGoal"/> (or multiple) when an object with the given <see cref="TechType"/> is picked up, equipped, or crafted through the Mobile Vehicle Bay.</description>
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
    /// <para>This goal is completed when an object with the given <see cref="TechType"/> is picked up, equipped, or crafted through the Mobile Vehicle Bay.</para>
    /// <para>This method CAN be called multiple times to add different goals to the same TechType.</para>
    /// </summary>
    /// <param name="key">The unique ID, required for all types of StoryGoals.</param>
    /// <param name="goalType">
    /// <para>Determines any basic events that may follow this StoryGoal event:</para>
    /// <list type="bullet">
    ///    <item>
    ///        <term><see cref="Story.GoalType.Story"/></term>
    ///        <description>Generic GoalType with no default effects. Primarily used for tracking story progress.</description>
    ///    </item>
    ///    <item>
    ///        <term><see cref="Story.GoalType.Encyclopedia"/></term>
    ///        <description>Adds a PDA entry on completion with the corresponding key. See <see cref="PDAHandler.AddCustomScannerEntry(PDAScanner.EntryData)"/>.</description>
    ///    </item>
    ///    <item>
    ///        <term><see cref="Story.GoalType.PDA"/></term>
    ///        <description>Adds a PDA Log message on completion with the corresponding key. See <see cref="PDAHandler.AddLogEntry"/>.</description>
    ///    </item>
    ///    <item>
    ///        <term><see cref="Story.GoalType.Radio"/></term>
    ///        <description>On completion, StoryGoals of this type will add a pending radio message with the corresponding key. See <see cref="PDAHandler.AddLogEntry"/></description>
    ///    </item>
    /// </list>
    /// </param>
    /// <param name="delay">StoryGoal listeners will not be notified until this many seconds after the goal is completed.</param>
    /// <param name="techType">The TechType that causes this goal to trigger, when picked up, equipped or crafted through the Mobile Vehicle Bay.</param>
    /// <returns></returns>
    public static ItemGoal RegisterItemGoal(string key, GoalType goalType, float delay, TechType techType)
    {
        
    }

    public static BiomeGoal RegisterBiomeGoal(string key, GoalType goalType, float delay, string biomeName, float minStayDuration)
    {

    }
    
    public static LocationGoal RegisterLocationGoal(string key, GoalType goalType, float delay, Vector3 position, float range, float minStayDuration)
    {

    }

    public static CompoundGoal RegisterCompoundGoal(string key, GoalType goalType, float delay, params string[] requiredGoals)
    {
        
    }

    public static OnGoalUnlock RegisterOnGoalUnlockData(string goal, UnlockBlueprintData[] blueprints = null, UnlockSignalData[] signals = null, UnlockItemData[] items = null, GameAchievements.Id[] achievements = null)
    {

    }

    public static void RegisterStoryGoalCustomEvent(string goalName, Action actionOnComplete)
    {

    }
}
#endif