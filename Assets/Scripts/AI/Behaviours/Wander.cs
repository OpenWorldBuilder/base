using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBuilder.AI
{
    public class Wander : AIBehaviour
    {
        private Vector3 targetPos;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            targetPos = GameManager.instance.boardScript.RandomPosition();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            // Every now and then, choose a new place to wander to.
            if (Random.Range(0, 1000) == 0)
            {
                targetPos = GameManager.instance.boardScript.RandomPosition();
            }

            obj.MoveTowards(targetPos);
        }

        //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
        //OnCantMove will be overriden by functions in the inheriting classes.
        internal override void OnCantMove(GameObject obj)
        {
            targetPos = GameManager.instance.boardScript.RandomPosition();
        }
    }
}
