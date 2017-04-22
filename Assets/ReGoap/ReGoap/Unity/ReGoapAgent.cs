﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ReGoapAgent<T, W> : MonoBehaviour, IReGoapAgent<T, W>, IReGoapAgentHelper
{
    public string Name;
    public float CalculationDelay = 0.5f;
    public bool BlackListGoalOnFailure;

    protected float lastCalculationTime;

    protected List<IReGoapGoal<T, W>> goals;
    protected List<IReGoapAction<T, W>> actions;
    protected IReGoapMemory<T, W> memory;
    protected IReGoapGoal<T, W> currentGoal;

    protected ReGoapActionState<T, W> currentActionState;

    protected Dictionary<IReGoapGoal<T, W>, float> goalBlacklist;
    protected List<IReGoapGoal<T, W>> possibleGoals;
    protected bool possibleGoalsDirty;
    protected List<ReGoapActionState<T, W>> startingPlan;
    protected Dictionary<T, W> planValues;
    protected bool interruptOnNextTransistion;

    protected ReGoapPlanWork<T, W>? CurrentReGoapPlanWorker;
    public bool IsPlanning
    {
        get { return CurrentReGoapPlanWorker != null && CurrentReGoapPlanWorker.Value.NewGoal == null; }
    }

    public bool WorkInFixedUpdate;
    public bool ValidateActiveAction;

    #region UnityFunctions
    protected virtual void Awake()
    {
        lastCalculationTime = -100;
        goalBlacklist = new Dictionary<IReGoapGoal<T, W>, float>();

        RefreshGoalsSet();
        RefreshActionsSet();
        RefreshMemory();
    }

    protected virtual void Start()
    {
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {
        if (currentActionState != null)
        {
            currentActionState.Action.Exit(null);
            currentActionState = null;
            currentGoal = null;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!WorkInFixedUpdate) return;
        Tick();
    }

    protected virtual void Update()
    {
        if (WorkInFixedUpdate) return;
        Tick();
    }

    private void Tick()
    {
        possibleGoalsDirty = true;

        if (currentActionState == null)
        {
            if (!IsPlanning)
                CalculateNewGoal();
            return;
        }
        // check if current action preconditions are still valid, else invalid action and restart planning
        if (ValidateActiveAction)
        {
            var state = memory.GetWorldState();
            if (currentActionState.Action.GetPreconditions(state).MissingDifference(state, 1) > 0)
                TryWarnActionFailure(currentActionState.Action);
        }
    }
    #endregion

    protected virtual void UpdatePossibleGoals()
    {
        possibleGoalsDirty = false;
        if (goalBlacklist.Count > 0)
        {
            possibleGoals = new List<IReGoapGoal<T, W>>(goals.Count);
            foreach (var goal in goals)
                if (!goalBlacklist.ContainsKey(goal))
                {
                    possibleGoals.Add(goal);
                }
                else if (goalBlacklist[goal] < Time.time)
                {
                    goalBlacklist.Remove(goal);
                    possibleGoals.Add(goal);
                }
        }
        else
        {
            possibleGoals = goals;
        }
    }

    protected virtual void TryWarnActionFailure(IReGoapAction<T, W> action)
    {
        if (action.IsInterruptable())
            WarnActionFailure(action);
        else
            action.AskForInterruption();
    }

    protected virtual bool CalculateNewGoal(bool forceStart = false)
    {
        if (IsPlanning)
            return false;
        if (!forceStart && (Time.time - lastCalculationTime <= CalculationDelay))
            return false;
        lastCalculationTime = Time.time;

        interruptOnNextTransistion = false;
        UpdatePossibleGoals();
        //var watch = System.Diagnostics.Stopwatch.StartNew();
        CurrentReGoapPlanWorker = ReGoapPlannerManager<T, W>.Instance.Plan(this, BlackListGoalOnFailure ? currentGoal : null,
            currentGoal != null ? currentGoal.GetPlan() : null, OnDonePlanning);

        return true;
    }

    protected virtual void OnDonePlanning(IReGoapGoal<T, W> newGoal)
    {
        CurrentReGoapPlanWorker = null;
        if (newGoal == null) { 
            if (currentGoal == null)
            {
                ReGoapLogger.LogWarning("GoapAgent " + this + " could not find a plan.");
            }
            return;
        }

        if (currentActionState != null)
            currentActionState.Action.Exit(null);
        currentActionState = null;
        currentGoal = newGoal;
        var plan = currentGoal.GetPlan();
        startingPlan = plan.ToList();
        ClearPlanValues();
        foreach (var actionState in startingPlan)
        {
            actionState.Action.PostPlanCalculations(this);
        }
        currentGoal.Run(WarnGoalEnd);
        PushAction();
    }

    public static string PlanToString(IEnumerable<IReGoapAction<T, W>> plan)
    {
        var result = "GoapPlan(";
        var reGoapActions = plan as IReGoapAction<T, W>[] ?? plan.ToArray();
        for (var index = 0; index < reGoapActions.Length; index++)
        {
            var action = reGoapActions[index];
            result += string.Format("'{0}'{1}", action, index + 1 < reGoapActions.Length ? ", " : "");
        }
        result += ")";
        return result;
    }

    public virtual void WarnActionEnd(IReGoapAction<T, W> thisAction)
    {
        if (thisAction != currentActionState.Action)
            return;
        PushAction();
    }

    protected virtual void PushAction()
    {
        if (interruptOnNextTransistion)
        {
            CalculateNewGoal();
            return;
        }
        var plan = currentGoal.GetPlan();
        if (plan.Count == 0)
        {
            if (currentActionState != null)
            {
                currentActionState.Action.Exit(currentActionState.Action);
                currentActionState = null;
            }
            CalculateNewGoal();
        }
        else
        {
            var previous = currentActionState;
            currentActionState = plan.Dequeue();
            IReGoapAction<T, W> next = null;
            if (plan.Count > 0)
                next = plan.Peek().Action;
            if (previous != null)
                previous.Action.Exit(currentActionState.Action);
            currentActionState.Action.Run(previous != null ? previous.Action : null, next, currentActionState.Settings, currentGoal.GetGoalState(), WarnActionEnd, WarnActionFailure);
        }
    }

    public virtual void WarnActionFailure(IReGoapAction<T, W> thisAction)
    {
        if (currentActionState != null && thisAction != currentActionState.Action)
        {
            ReGoapLogger.LogWarning(string.Format("[GoapAgent] Action {0} warned for failure but is not current action.", thisAction));
            return;
        }
        if (BlackListGoalOnFailure)
            goalBlacklist[currentGoal] = Time.time + currentGoal.GetErrorDelay();
        CalculateNewGoal(true);
    }

    public virtual void WarnGoalEnd(IReGoapGoal<T, W> goal)
    {
        if (goal != currentGoal)
        {
            ReGoapLogger.LogWarning(string.Format("[GoapAgent] Goal {0} warned for end but is not current goal.", goal));
            return;
        }
        CalculateNewGoal();
    }

    public virtual void WarnPossibleGoal(IReGoapGoal<T, W> goal)
    {
        if ((currentGoal != null) && (goal.GetPriority() <= currentGoal.GetPriority()))
            return;
        if (currentActionState != null && !currentActionState.Action.IsInterruptable())
        {
            interruptOnNextTransistion = true;
            currentActionState.Action.AskForInterruption();
        }
        else
            CalculateNewGoal();
    }

    public virtual bool IsActive()
    {
        return enabled;
    }

    public virtual List<ReGoapActionState<T, W>> GetStartingPlan()
    {
        return startingPlan;
    }

    protected virtual void ClearPlanValues()
    {
        if (planValues == null)
            planValues = new Dictionary<T, W>();
        else
        {
            planValues.Clear();
        }
    }

    public virtual W GetPlanValue(T key)
    {
        return planValues[key];
    }

    public virtual bool HasPlanValue(T key)
    {
        return planValues.ContainsKey(key);
    }

    public virtual void SetPlanValue(T key, W value)
    {
        planValues[key] = value;
    }

    public virtual void RefreshMemory()
    {
        memory = GetComponent<IReGoapMemory<T, W>>();
    }

    public virtual void RefreshGoalsSet()
    {
        goals = new List<IReGoapGoal<T, W>>(GetComponents<IReGoapGoal<T, W>>());
        possibleGoalsDirty = true;
    }

    public virtual void RefreshActionsSet()
    {
        actions = new List<IReGoapAction<T, W>>(GetComponents<IReGoapAction<T, W>>());
    }

    public virtual List<IReGoapGoal<T, W>> GetGoalsSet()
    {
        if (possibleGoalsDirty)
            UpdatePossibleGoals();
        return possibleGoals;
    }

    public virtual List<IReGoapAction<T, W>> GetActionsSet()
    {
        return actions;
    }

    public virtual IReGoapMemory<T, W> GetMemory()
    {
        return memory;
    }

    public virtual IReGoapGoal<T, W> GetCurrentGoal()
    {
        return currentGoal;
    }

    public virtual ReGoapState<T, W> InstantiateNewState()
    {
        return ReGoapState<T, W>.Instantiate();
    }

    public override string ToString()
    {
        return string.Format("GoapAgent('{0}')", Name);
    }

    // this only works if the ReGoapAgent has been inherited. For "special cases" you have to override this
    public virtual Type[] GetGenericArguments()
    {
        return GetType().BaseType.GetGenericArguments();
    }
}