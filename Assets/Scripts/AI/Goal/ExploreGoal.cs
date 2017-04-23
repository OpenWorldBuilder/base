using ReGoap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBuilder.AI.Goal
{
    public class ExploreGoal : ReGoapGoal<string, object>
    {
        protected override void Awake()
        {
            base.Awake();
            goal.Set("explored", true);
        }

        public override string ToString()
        {
            return string.Format("GoapGoal('ExploreGoal: {0}')", Name);
        }
    }
}