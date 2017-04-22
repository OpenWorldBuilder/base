using UnityEngine;
using System.Collections;

namespace ZombieLand
{
    public class Loader : MonoBehaviour 
	{
		public GameObject gameManager;			//GameManager prefab to instantiate.

		void Awake ()
		{
            //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
            if (GameManager.instance == null)
            {
                //Instantiate gameManager prefab
                Instantiate(gameManager);
            }
		}
	}
}