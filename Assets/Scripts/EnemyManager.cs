using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieLand
{
    public class EnemyManager : MonoBehaviour
    {
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

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}