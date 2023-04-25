using System;
using System.Collections.Generic;
using Nautilus.Patchers;
using Story;
using UnityEngine;

namespace Nautilus.MonoBehaviours;

internal class CustomStoryGoalManager : MonoBehaviour, IStoryGoalListener
{
    public static CustomStoryGoalManager Instance { get; private set; }
    
    private ItemGoalTracker _itemGoalTracker;
    private BiomeGoalTracker _biomeGoalTracker;
    private LocationGoalTracker _locationGoalTracker;
    private CompoundGoalTracker _compoundGoalTracker;
    private OnGoalUnlockTracker _onGoalUnlockTracker;

    private Queue<ItemGoal> _itemGoals = new();
    private Queue<BiomeGoal> _biomeGoals = new();
    private Queue<LocationGoal> _locationGoals = new();
    private Queue<CompoundGoal> _compoundGoals = new();
    private Queue<OnGoalUnlock> _onGoalUnlocks = new();
    
    public void AddImmediately<T>(T storyGoal) where T : StoryGoal
    {
        // goals that are already completed have no effect
        if (StoryGoalManager.main.IsGoalComplete(storyGoal.key))
            return;
        
        switch (storyGoal)
        {
            case ItemGoal itemGoal:
                _itemGoals.Enqueue(itemGoal);
                break;
            case BiomeGoal biomeGoal:
                _biomeGoals.Enqueue(biomeGoal);
                break;
            case LocationGoal locationGoal:
                _locationGoals.Enqueue(locationGoal);
                break;
            case CompoundGoal compoundGoal:
                _compoundGoals.Enqueue(compoundGoal);
                break;
        }
    }

    public void AddImmediately(OnGoalUnlock onGoalUnlock)
    {
        // make sure to unlock new blueprints and achievements for completed goals
        if (StoryGoalManager.main.IsGoalComplete(onGoalUnlock.goal))
        {
            onGoalUnlock.TriggerBlueprints();
            onGoalUnlock.TriggerAchievements();
            return;
        }
        
        _onGoalUnlocks.Enqueue(onGoalUnlock);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _itemGoalTracker = GetComponent<ItemGoalTracker>();
        _biomeGoalTracker = GetComponent<BiomeGoalTracker>();
        _locationGoalTracker = GetComponent<LocationGoalTracker>();
        _compoundGoalTracker = GetComponent<CompoundGoalTracker>();
        _onGoalUnlockTracker = GetComponent<OnGoalUnlockTracker>();
        
        if (StoryGoalManager.main)
        {
            StoryGoalManager.main.AddListener(this);
        }
    }

    private void OnUpdate()
    {
        TrackItemGoals();

        TrackBiomeGoals();

        TrackLocationGoals();

        TrackCompoundGoals();

        TrackOnGoalUnlocks();
    }

    void IStoryGoalListener.NotifyGoalComplete(string key)
    {
        StoryGoalPatcher.StoryGoalCustomEvents?.Invoke(key);
    }
    
    private void OnEnable()
    {
        ManagedUpdate.Subscribe(ManagedUpdate.Queue.Update, OnUpdate);
    }

    private void OnDisable()
    {
        ManagedUpdate.Unsubscribe(ManagedUpdate.Queue.Update, OnUpdate);
    }
    
    private void TrackItemGoals()
    {
        if (_itemGoals.Count <= 0)
            return;
        
        var itemGoal = _itemGoals.Dequeue();
        _itemGoalTracker.goals.GetOrAddNew(itemGoal.techType).Add(itemGoal);
    }
    
    private void TrackBiomeGoals()
    {
        if (_biomeGoals.Count <= 0)
            return;
        
        var biomeGoal = _biomeGoals.Dequeue();
        _biomeGoalTracker.goals.Add(biomeGoal);
    }
    
    private void TrackLocationGoals()
    {
        if (_locationGoals.Count <= 0)
            return;
        
        var locationGoal = _locationGoals.Dequeue();
        _locationGoalTracker.goals.Add(locationGoal);
    }
    
    private void TrackCompoundGoals()
    {
        if (_compoundGoals.Count <= 0)
            return;
        
        var compoundGoal = _compoundGoals.Dequeue();
        _compoundGoalTracker.goals.Add(compoundGoal);
    }
    
    private void TrackOnGoalUnlocks()
    {
        if (_onGoalUnlocks.Count <= 0)
            return;
        
        var onGoalUnlock = _onGoalUnlocks.Dequeue();
        _onGoalUnlockTracker.goalUnlocks[onGoalUnlock.goal] = onGoalUnlock;
    }
}