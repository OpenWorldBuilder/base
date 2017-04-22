using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBuilder
{
    public class EnemyManager : MonoBehaviour
    {
        public float turnDelay = 0.01f;							//Delay between each Player turn.
        private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.

        // Awake is always called before any Start functions
        void Awake()
        {
            //Assign enemies to a new List of Enemy objects.
            enemies = new List<Enemy>();
        }

        // Use this for initialization
        void Start()
        {
            //Clear any Enemy objects in our List to prepare for next level.
            enemies.Clear();
        }

        // Update is called once per frame
        void Update()
        {
            //Start moving enemies.
            StartCoroutine(MoveEnemies());
        }

        //Call this to add the passed in Enemy to the List of Enemy objects.
        public void AddEnemyToList(Enemy script)
        {
            //Add Enemy to List enemies.
            enemies.Add(script);
        }

        //Coroutine to move enemies in sequence.
        IEnumerator MoveEnemies()
        {
            //Wait for turnDelay seconds, defaults to .1 (100 ms).
            yield return new WaitForSeconds(turnDelay);

            //If there are no enemies spawned (IE in first level):
            if (enemies.Count == 0)
            {
                //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
                yield return new WaitForSeconds(turnDelay);
            }

            //Loop through List of Enemy objects.
            for (int i = 0; i < enemies.Count; i++)
            {
                //Call the MoveEnemy function of Enemy at index i in the enemies List.
                enemies[i].MoveEnemy();

                //Wait for Enemy's moveTime before moving next Enemy, 
                yield return new WaitForSeconds(enemies[i].moveTime);
            }
        }
    }
}