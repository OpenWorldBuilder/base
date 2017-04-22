using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldBuilder.AI;

namespace WorldBuilder
{
    /**
     * This (will be) an enemy manager for some wave-like spawning.
     */
    public class EnemyManager : MonoBehaviour
    {
        public float waveDelay = 120;							     //Delay between each Wave spawn.
        private List<GameObject> enemies;                            //List of all Enemy units, used to issue them move commands.

        // Awake is always called before any Start functions
        void Awake()
        {
            //Assign enemies to a new List of Enemy objects.
            enemies = new List<GameObject>();
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
            StartCoroutine(SpawnEnemies());
        }

        //Call this to add the passed in Enemy to the List of Enemy objects.
        public void AddEnemyToList(GameObject obj)
        {
            //Add Enemy to List enemies.
            enemies.Add(obj);
        }

        //Coroutine to move enemies in sequence.
        IEnumerator SpawnEnemies()
        {
            //Wait for waveDelay seconds, defaults to 120s.
            yield return new WaitForSeconds(waveDelay);

            // TODO?
        }
    }
}