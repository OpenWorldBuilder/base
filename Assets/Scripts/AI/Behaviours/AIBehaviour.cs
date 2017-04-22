using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBuilder.AI
{
    public class AIBehaviour : MonoBehaviour
    {
        protected AIBase obj;

        // Use this for initialization
        protected virtual void Start()
        {
            obj = gameObject.GetComponent<AIBase>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
        //OnCantMove will be overriden by functions in the inheriting classes.
        internal virtual void OnCantMove(GameObject obj)
        {
            // Do nothing by default.
        }
    }
}
