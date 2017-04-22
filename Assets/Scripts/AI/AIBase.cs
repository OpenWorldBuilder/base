using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBuilder.AI
{
    public class AIBase : MovingObject
    {
        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
        //OnCantMove will be overriden by functions in the inheriting classes.
        protected override void OnCantMove(GameObject collided)
        {
            AIBehaviour[] behaviours = GetComponents<AIBehaviour>();
            foreach (AIBehaviour behaviour in behaviours)
            {
                behaviour.OnCantMove(collided);
            }
        }
    }
}