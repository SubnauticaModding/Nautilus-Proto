# Using the Story Goal system

The progression of Subnautica is primarily based around the Story Goal system. This system is composed of the `StoryGoalManager` class and several sub-systems which together handle hundreds of different goals, each with their own unique triggers and effects on completion.

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

The `StoryGoal` object is the basis of all StoryGoals.

Every Story Goal has a `key`, `delay`, and `GoalType`. It can also have associated data for what happens on completion, which must be defined in another class. An example of this is the OnGoalUnlockTracker (see [StoryGoalHandler.RegisterOnGoalUnlockData](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.StoryGoalHandler.html#Nautilus_Handlers_StoryGoalHandler_RegisterOnGoalUnlockData_System_String_Story_UnlockBlueprintData___Story_UnlockSignalData___Story_UnlockItemData___GameAchievements_Id___)).

Many sub-classes and trackers exist to automate the unlocking process, as shown in the next section.

> [!NOTE]
> A StoryGoal object can be instantiated directly, without accessing the `StoryGoalHandler` or any goal tracking classes. However, a goal created in this way must be triggered manually.

## Trackers
These internal game classes manage the automatic unlocking of specific goals. You do not need to access them directly.

| Tracker type        | Description                                                                                                                              |
| ------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| ItemGoalTracker     | Completes a goal (or multiple) when an object with the given TechType is picked up, equipped, or crafted through the Mobile Vehicle Bay. |
| BiomeGoalTracker    | Completes a goal when the player stays in a given biome for a specified period of time.                                                  |
| LocationGoalTracker | Completes a goal when the player stays within range of a certain position for a specified period of time.                                |
| CompoundGoalTracker | Completes a goal when all required "precondition" goals have been completed.                                                             |

## GoalType
Every Story Goal has an assigned Goal Type which determines the action that is executed on completion (if any):

| GoalType | Purpose |
| --- | --- |
| GoalType.Story | Generic GoalType with no default effects. Primarily used for tracking story progress and triggering custom events. |
| GoalType.Encyclopedia | Adds a PDA databank entry on completion with the corresponding key. Also see [PDAHandler.AddEncyclopediaEntry(...)](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.PDAHandler.html#Nautilus_Handlers_PDAHandler_AddEncyclopediaEntry_PDAEncyclopedia_EntryData_). |
| GoalType.PDA | Adds a PDA Log message with the corresponding key. Also see [PDAHandler.AddLogEntry(...)](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.PDAHandler.html#Nautilus_Handlers_PDAHandler_AddLogEntry). |
| GoalType.Radio | Adds a pending radio message with the corresponding key. Also see [PDAHandler.AddLogEntry(...)](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.PDAHandler.html#Nautilus_Handlers_PDAHandler_AddLogEntry). |

## StoryGoalHandler

This is the main class for interacting with the game's Story Goal system. It allows you to add goals to specific trackers and gives you full control over their actions on completion.

> [!WARNING]
> As of now, the `StoryGoalHandler` class is only designed to work for the first Subnautica game. Remember, you can [always contribute](https://github.com/SubnauticaModding/Nautilus/blob/master/Nautilus/Handlers/StoryGoalHandler_Subnautica.cs).

A more comprehensive overview of the class can be viewed [here](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.StoryGoalHandler.html).

## Creating goals

Example code for registering a Story Goal is shown below. This goal will be triggered after staying in the Kelp Forest for 30 seconds or more. On completion it plays a voice line, kills the player, and unlocks the Seamoth blueprint.

```csharp
using Story;

// ...

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

Story goals can be completed in various ways, some more useful than others:

| Method | Notes |
| --- | --- |
| Automatically, through the tracker system (accessible through the `StoryGoalHandler`). | This is the easiest way to add story goals and is recommended for typical use cases. |
| `StoryGoal.Trigger()` (instance) | This is the recommended method for triggering goals without a tracker. When called, schedules the goal for completion (based on the delay), then executes all associated actions. |
| `StoryGoalManager.main.OnGoalComplete(string key)` | Returns false if the goal has already been completed. Otherwise, returns true and adds the goal instantly. Has no delay and does not apply the actions defined by the goal's GoalType. |
| `StoryGoal.Execute(string key, GoalType goalType)` (static) | Instantly completes a goal by calling OnGoalComplete, without applying the delay. Properly applies the actions defined by the goal's GoalType. |

> [!NOTE]
> When not using a tracker, the most proper way to complete a StoryGoal is through calling the `StoryGoal.Trigger()` method on a given instance. This is the only way to ensure the delay is applied properly and all actions are executed.

## Action on completion

Within the `StoryGoalHandler` class, action on completion can be defined in a couple ways:

| Method | Notes |
| --- | --- |
| [StoryGoalHandler.RegisterCustomEvent(string key, Action customEventCallback)](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.StoryGoalHandler.html#Nautilus_Handlers_StoryGoalHandler_RegisterCustomEvent_System_String_System_Action_) | Allows code of any length to be run when the goal is completed. |
| [StoryGoalHandler.RegisterOnGoalUnlockData(...)](https://subnauticamodding.github.io/Nautilus/api/Nautilus.Handlers.StoryGoalHandler.html#Nautilus_Handlers_StoryGoalHandler_RegisterOnGoalUnlockData_System_String_Story_UnlockBlueprintData___Story_UnlockSignalData___Story_UnlockItemData___GameAchievements_Id___) | Allows the user to define any blueprints, items or achievements that are gained on completion. |

## Saving progress

Every story goal can only be completed once, so no custom saving logic is required.

The `StoryGoalManager.main.OnGoalComplete(string key)` method can be used for one-time events (WITHOUT story goals!) because it will only return true once for any given string, which persists between game sessions.

The `StoryGoalManager.main.IsGoalComplete(string key)` method can be used to check if a goal with the given key has already been completed.

## Summary

The Story Goal system is a powerful tool for creating story and exploration-driven progression in Subnautica mods. By using the methods within the `StoryGoalHandler` class, you can easily add goals and customize their effects upon completion.