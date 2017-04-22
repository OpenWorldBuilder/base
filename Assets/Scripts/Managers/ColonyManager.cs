using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBuilder
{
    public class ColonyManager : MonoBehaviour
    {
        public int numberOfColonists = 5;
        public GameObject[] colonistTypes;

        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < numberOfColonists; i++)
            {
                Vector3 pos = GameManager.instance.boardScript.RandomPosition();

                // Spawn a random colonist here.
                int randomIndex = Random.Range(0, colonistTypes.Length);
                GameObject obj = Instantiate(colonistTypes[randomIndex], pos, Quaternion.identity);
                obj.tag = "Colonist";
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}