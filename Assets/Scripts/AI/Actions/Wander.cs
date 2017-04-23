using ReGoap.Core;
using ReGoap.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBuilder.AI.Actions
{
    public class Wander : ReGoapAction<string, object>
    {

        public override ReGoapState<string, object> GetEffects(ReGoapState<string, object> goalState, IReGoapAction<string, object> next = null)
        {
            effects.Clear();
            effects.Set("explored", true);
            return effects;
        }

        public override ReGoapState<string, object> GetPreconditions(ReGoapState<string, object> goalState, IReGoapAction<string, object> next = null)
        {
            preconditions.Clear();
            preconditions.Set("isAtPosition", GameManager.instance.boardScript.RandomPosition());
            return preconditions;
        }

        public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, IReGoapActionSettings<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
        {
            base.Run(previous, next, settings, goalState, done, fail);
            done(this);
        }

        public override string ToString()
        {
            return string.Format("GoapAction('{0}')", Name);
        }
    }
}