# Using the Story Goal system

The progression of Subnautica is primarily based around the Story Goal system. This system is composed of several sub-systems that together handle hundreds of different goals, each with their own unique triggers and effects on completion.

Nautilus provides a new handler for accessing this system, which was not available in SMLHelper. This guide covers the basics of how to use them, and how they may help you implement certain features into your mod.

### Vanilla use-cases
- Alien data terminals
- Data consoles in Alterra Wrecks
- Most story events
- The PDA databank tab
- The PDA log tab (any sort of voice lines that play during progression!)
  - The radio, which is an extension of the log system.

### Possible actions on completion
- Adding items to the player's inventory
- Adding PDA databank entries
- Adding pending radio messages.
- Playing voice lines through the PDA log system
- Triggering custom events (this in particular means that a goal can do ANYTHING on completion!)
- Unlocking achievements
- Unlocking blueprints
- Unlocking signal locations

## The StoryGoal class

The `StoryGoal` object is the basis of all StoryGoals. Many sub-classes and trackers exist to automate the unlocing process:

| Tracker type        | StoryGoal Type | Description                                                                                                                              |
| ------------------- | -------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| ItemGoalTracker     | ItemGoal       | Completes a goal (or multiple) when an object with the given TechType is picked up, equipped, or crafted through the Mobile Vehicle Bay. |
| BiomeGoalTracker    | BiomeGoal      | Completes a goal when the player stays in a given biome for a specified period of time.                                                  |
| LocationGoalTracker | LocationGoal   | Completes a goal when the player stays within range of a certain position for a specified period of time.                                |
| CompoundGoalTracker | CompoundGoal   | Completes a goal when all required "precondition" goals have been completed.                                                             |

However, beyond the tracking and scheduling there are only two essential properties of Story Goal events: their `key` and `goalType`. Due to this fact, goals can be arbitrarily executed with methods such as `StoryGoal.Execute(string, GoalType)` and `StoryGoalManager.main.IsGoalComplete(string key)` (without a reference to the goal object), and all actions on completion will still be performed.

Every Story Goal has its own Goal Type which determines the automatic action on completion:

| GoalType | Purpose |
| --- | --- |
| GoalType.Story | Generic GoalType with no default effects. Primarily used for tracking story progress and triggering custom events. |
| GoalType.Encyclopedia | Adds a PDA databank entry on completion with the corresponding key. Also see [PDAHandler.AddEncyclopediaEntry(...)](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.PDAHandler.html#Nautilus_Handlers_PDAHandler_AddEncyclopediaEntry_PDAEncyclopedia_EntryData_). |
| GoalType.PDA | Adds a PDA Log message with the corresponding key. Also see [PDAHandler.AddLogEntry(...)](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.PDAHandler.html#Nautilus_Handlers_PDAHandler_AddLogEntry). |
| GoalType.Radio | Adds a pending radio message with the corresponding key. Also see [PDAHandler.AddLogEntry(...)](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.PDAHandler.html#Nautilus_Handlers_PDAHandler_AddLogEntry). |

## StoryGoalHandler

> [!WARNING]
> As of now, the StoryGoalHandler system is only designed to work for the first Subnautica game. Remember, you can [always contribute](https://github.com/SubnauticaModding/Nautilus/blob/master/Nautilus/Handlers/StoryGoalHandler_Subnautica.cs).

This is the main class for interacting with the game's Story Goal system. It allows you to add goals to specific trackers and gives you full control over their actions on completion.

A more comprehensive overview of the class can be viewed [here](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.StoryGoalHandler.html).

## Creating goals

Example code for registering a Story Goal is shown below. This goal will be triggered after staying in the Kelp Forest for 30 seconds or more. On completion it plays a voice line, kills the player, and unlocks the Seamoth blueprint.

```csharp
// Register the goal to the BiomeGoalTracker. A GoalType of PDA means that this goal will trigger a PDA line and add it to the log on completion:
StoryGoalHandler.RegisterBiomeGoal("KelpForestEnjoyer", GoalType.PDA, biomeName: "kelpForest", minStayDuration: 30f, delay: 3f);
// Register the PDA voice line. Note how the key matches the key of the story goal:
PDAHandler.AddLogEntry("KelpForestEnjoyer", "KelpForestEnjoyer", sound);
// Set the English translation for PDA message's subtitles:
LanguageHandler.SetLanguageLine("KelpForestEnjoyer", "Congratulations for staying in the Kelp Forest for 30 seconds!", "English");

// Add a custom event that kills the player when this goal is completed:
StoryGoalHandler.RegisterCustomEvent("KelpForestEnjoyer", () =>
{
    Player.main.liveMixin.TakeDamage(10000f);
});

// Unlock the seamoth on completion of this goal:
StoryGoalHandler.RegisterOnGoalUnlockData("KelpForestEnjoyer", blueprints: new Story.UnlockBlueprintData[]
{
    new Story.UnlockBlueprintData() {techType = TechType.Seamoth, unlockType = Story.UnlockBlueprintData.UnlockType.Available},
});
```

## Completing goals

Story goals can be completed in various ways:

| Method | Notes |
| Automatically, through the tracker system | This is the easiest way to add story goals and is recommended for typical use cases. |
| `StoryGoal.Trigger()`| This is the recommended method for triggering goals without a tracker. When called, notifies listeners of the goal's completion AFTER the goal's delay, and executes all completion actions. Requires a reference to the goal object! |
| `StoryGoalManager.main.OnGoalComplete(string key)` | Returns false if the goal has already been completed. Otherwise, adds the goal instantly (delay is NOT applied). |
| `StoryGoal.Execute(string key, GoalType goalType)` (static) | Instantly completes a goal by calling OnGoalComplete, but also 

## Saving progress

Every story goal can only be completed once, so no custom saving logic is required.

The `OnGoalComplete(string key)` method can be called at any point, and does not require a Story Goal to exist. This method will only return true once for any given string, which persists between game sessions.

The `StoryGoalManager.main.IsGoalComplete(string key)` method can be used to check if a goal with the given key has already been completed.

