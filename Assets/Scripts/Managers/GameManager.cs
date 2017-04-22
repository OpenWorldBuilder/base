using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace WorldBuilder
{
    using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        internal BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
        internal CameraManager cameraMan;						//Store a reference to our CameraManager which will set up the level.
        internal EnemyManager enemyManager;
        internal ColonyManager colonyManager;
        private int worldClock, worldClockDay, worldClockHour, worldClockMinute;
        private float worldClockDelta;
        private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
        public int secondsPerMinute = 1;


        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                //if not, set instance to this
                instance = this;
            }
            else if (instance != this)
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            }

			// Sets this to not be destroyed when reloading scene.
			DontDestroyOnLoad(gameObject);

            //Get a component reference to the attached BoardManager script
            boardScript = GetComponent<BoardManager>();

            //Get a component reference to the attached EnemyManager script
            enemyManager = GetComponent<EnemyManager>();

            //Get a component reference to the attached ColonyManager script
            colonyManager = GetComponent<ColonyManager>();

            //Get a component reference to the attached CameraManager script
            cameraMan = Camera.main.GetComponent<CameraManager>();

            //Call the InitGame function to initialize the first level 
            InitGame();
		}

        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance.InitGame();
        }
		
		//Initializes the game for each level.
		void InitGame()
		{
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;
			
			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene();

            //Center the camera.
            Vector2 vec = boardScript.GetCenter();
            cameraMan.SetPosition(vec);

            // Set the world clock.
            worldClock = worldClockDay = worldClockHour = worldClockMinute = 0;
            worldClockDelta = (float)secondsPerMinute;

            //Set doingSetup to false allowing player to move again.
            doingSetup = false;

            // Remove the loading screen.
            GameObject.Find("LevelImage").SetActive(false);
        }
		
		//Update is called every frame.
		void Update()
		{
			if (doingSetup)
            {
                return;
            }

            // Update the world clock.
            worldClockDelta -= Time.deltaTime;
            if (worldClockDelta <= 0)
            {
                UpdateWorldClock();
            }
        }

        // Update the world clock on tick because there's no telling how many times we might call
        // the get methods elsewhere.
        private void UpdateWorldClock()
        {
            worldClock++;
            worldClockMinute++;

            if (worldClockMinute > 60)
            {
                worldClockMinute = 0;
                worldClockHour++;
            }

            if (worldClockHour > 24)
            {
                worldClockHour = 0;
                worldClockDay++;
            }
        }

        public int GetDay()
        {
            return worldClockDay;
        }

        public int GetHour()
        {
            return worldClockHour;
        }

        public int GetMinute()
        {
            return worldClockMinute;
        }

        //GameOver is called when the player reaches 0 food points
        public void GameOver()
		{
			//Disable this GameManager.
			enabled = false;
		}
	}
}

