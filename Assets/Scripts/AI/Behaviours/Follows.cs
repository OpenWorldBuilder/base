using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBuilder.AI
{
    public class Follows : AIBehaviour
    {
        public string followTag = "Colonist"; // Follow things with this tag.
        private Transform target;

        // Use this for initialization
        protected override void Start()
        {
            //Find the Player GameObject using it's tag and store a reference to its transform component.
            target = GameObject.FindGameObjectWithTag(followTag).transform;
        }

        // Update is called once per frame
        protected override void Update()
        {
            obj.MoveTowards(target.position);
        }
    }
}
