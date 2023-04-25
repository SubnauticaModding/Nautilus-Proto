using System;
using Story;
using UnityEngine;

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
    /// <para><b>Important:</b> This method can be called <b>as many times as needed</b> to add different goals to the same TechType.</para>
    /// </summary>
    /// <param name="key">The unique identifier, required for all types of StoryGoals.</param>
    /// <param name="goalType">If assigned a value other than 'Story', this will determine the automatic response to being triggered. Can add a PDA log, Radio message or Databank entry.</param>
    /// <param name="delay">StoryGoal listeners will not be notified until this many seconds after the goal is completed.</param>
    /// <param name="techType">The TechType that causes this goal to trigger, when picked up, equipped or crafted through the Mobile Vehicle Bay.</param>
    /// <returns>The registered <see cref="ItemGoal"/>.</returns>
    public static ItemGoal RegisterItemGoal(string key, Story.GoalType goalType, float delay, TechType techType)
    {
        var goal = new ItemGoal() { key = key, goalType = goalType, delay = delay, techType = techType };
    }

    /// <summary>
    /// Registers a goal that is completed when the player stays in a given biome for a specified period of time.
    /// </summary>
    /// <param name="key">The unique identifier, required for all types of StoryGoals.</param>
    /// <param name="goalType">If assigned a value other than 'Story', this will determine the automatic response to being triggered. Can add a PDA log, Radio message or Databank entry.</param>
    /// <param name="delay">StoryGoal listeners will not be notified until this many seconds after the goal is completed.</param>
    /// <param name="biomeName">The biome that must be entered to trigger this goal.</param>
    /// <param name="minStayDuration">The minimum amount of time the player must stay in the given biome.</param>
    /// <returns>The registered <see cref="BiomeGoal"/>.</returns>
    public static BiomeGoal RegisterBiomeGoal(string key, Story.GoalType goalType, float delay, string biomeName, float minStayDuration)
    {
        var goal = new BiomeGoal() { key = key, goalType = goalType, delay = delay, biome = biomeName, minStayDuration = minStayDuration };
    }

    /// <summary>
    /// Registers a goal that is completed when the player stays within range of a certain position for a specified period of time.
    /// </summary>
    /// <param name="key">The unique identifier, required for all types of StoryGoals.</param>
    /// <param name="goalType">If assigned a value other than 'Story', this will determine the automatic response to being triggered. Can add a PDA log, Radio message or Databank entry.</param>
    /// <param name="delay">StoryGoal listeners will not be notified until this many seconds after the goal is completed.</param>
    /// <param name="position">The center of the sphere that must be occupied.</param>
    /// <param name="range">The radius of the sphere that must be occupied.</param>
    /// <param name="minStayDuration">The minimum amount of time the player must stay for this goal to be completed.</param>
    /// <returns>The registered <see cref="LocationGoal"/>.</returns>
    public static LocationGoal RegisterLocationGoal(string key, Story.GoalType goalType, float delay, Vector3 position, float range, float minStayDuration)
    {
        var goal = new LocationGoal() { key = key, goalType = goalType, delay = delay, position = position, range = range, minStayDuration = minStayDuration };
    }

    /// <summary>
    /// Registers a goal that is completed when all required "precondition" goals have been completed
    /// </summary>
    /// <param name="key">The unique identifier, required for all types of StoryGoals.</param>
    /// <param name="goalType">If assigned a value other than 'Story', this will determine the automatic response to being triggered. Can add a PDA log, Radio message or Databank entry.</param>
    /// <param name="delay">StoryGoal listeners will not be notified until this many seconds after the goal is completed.</param>
    /// <param name="requiredGoals">The list of all goals that must be completed before this goal is marked as complete.</param>
    /// <returns>The registered <see cref="CompoundGoal"/>.</returns>
    public static CompoundGoal RegisterCompoundGoal(string key, Story.GoalType goalType, float delay, params string[] requiredGoals)
    {
        var goal = new CompoundGoal() { key = key, goalType = goalType, delay = delay, preconditions = requiredGoals };
    }

    /// <summary>
    /// <para>Registers a new <see cref="OnGoalUnlock"/> object for an existing goal. Handles complex actions that occur with the goal's completion.</para>
    /// <para><b>Important:</b> Since these are stored in a dictionary, only <b>one</b> <see cref="OnGoalUnlock"/> object can be added for each specific goal key. Therefore, be careful when adding unlock data to base-game features.</para>
    /// </summary>
    /// <param name="goal">The goal that is associated with this action.</param>
    /// <param name="blueprints"></param>
    /// <param name="signals"></param>
    /// <param name="items"></param>
    /// <param name="achievements"></param>
    /// <returns>The registered <see cref="OnGoalUnlock"/> object.</returns>
    public static OnGoalUnlock RegisterOnGoalUnlockData(string goal, UnlockBlueprintData[] blueprints = null, UnlockSignalData[] signals = null, UnlockItemData[] items = null, GameAchievements.Id[] achievements = null)
    {
        var obj = new OnGoalUnlock()
        {
            goal = goal,
            blueprints = blueprints ?? new UnlockBlueprintData[0],
            signals = signals ?? new UnlockSignalData[0],
            items = items ?? new UnlockItemData[0],
            achievements = achievements ?? new GameAchievements.Id[0]
        };
    }

    public static void RegisterStoryGoalCustomEvent(string goalName, Action actionOnComplete)
    {

    }
}
#endif