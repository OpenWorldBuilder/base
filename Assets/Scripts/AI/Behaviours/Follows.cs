using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBuilder.AI
{
    public class Follows : Behaviour
    {
        public string followTag = "Colonist"; // Follow things with this tag.
        private AIBase obj;
        private Transform target;

        // Use this for initialization
        void Start()
        {
            //Find the Player GameObject using it's tag and store a reference to its transform component.
            target = GameObject.FindGameObjectWithTag(followTag).transform;
        }

        // Update is called once per frame
        void Update()
        {
            //Declare variables for X and Y axis move directions, these range from -1 to 1.
            //These values allow us to choose between the cardinal directions: up, down, left and right.
            int xDir = 0;
            int yDir = 0;

            //If the difference in positions is not approximately zero (Epsilon) do the following:
            if (Mathf.Abs(target.position.y - transform.position.y) > float.Epsilon)
            {
                //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
                yDir = target.position.y > transform.position.y ? 1 : -1;
            }

            //If the difference in positions is not approximately zero (Epsilon) do the following:
            if (Mathf.Abs(target.position.x - transform.position.x) > float.Epsilon)
            {
                //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
                xDir = target.position.x > transform.position.x ? 1 : -1;
            }

            //Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
            obj.AttemptMove(xDir, yDir);
        }
    }
}
