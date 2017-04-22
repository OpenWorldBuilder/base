using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace WorldBuilder
{
	public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.
			
			
			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}

        public int columns = 256; 										//Number of columns in our game board.
		public int rows = 256;											//Number of rows in our game board.
 
		public Count veinCount = new Count (5, 20);                     //Lower and upper limit for our random number of resource tiles per level.
		public Count veinSize = new Count (5, 35);                      //Lower and upper limit for our random number of resource tiles per level.
        public int veinMaxWidth = 15;
        public GameObject[] resourceTiles;                              //Array of resource tiles prefabs.

        public GameObject[] floorTiles;									//Array of floor prefabs.
		
		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <List<Vector3>> gridPositions = new List <List<Vector3>> ();	//A list of possible locations to place tiles.
		
		
		//Clears our list gridPositions and prepares it to generate a new board.
		void InitialiseList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();
			
			//Loop through x axis (columns).
			for(int x = 0; x < columns; x++)
			{
                List<Vector3> row = new List<Vector3>();

                //Within each column, loop through y axis (rows).
                for (int y = 0; y < rows; y++)
				{
                    //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                    row.Add (new Vector3(x, y, 0f));
				}

                gridPositions.Add(row);
            }
		}
		
		
		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{
			//Instantiate Board and set boardHolder to its transform.
			boardHolder = new GameObject ("Board").transform;
			
			//Loop along x axis.
			for(int x = 0; x < columns; x++)
			{
				//Loop along y axis.
				for(int y = 0; y < rows; y++)
				{
					//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
					GameObject toInstantiate = floorTiles[0];
					
					//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
					GameObject instance =
						Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
					//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
					instance.transform.SetParent (boardHolder);
				}
			}
		}
		
		
		//RandomPosition returns a random position from our list gridPositions.
		public Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);

            //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
            List<Vector3> randomPositions = gridPositions[randomIndex];

            // Extract from this row.
            randomIndex = Random.Range(0, randomPositions.Count);
            Vector3 randomPosition = randomPositions[randomIndex];

            //Remove the entry at randomIndex from the list so that it can't be re-used.
            randomPositions.RemoveAt (randomIndex);
			
			//Return the randomly selected Vector3 position.
			return randomPosition;
		}

        internal Vector2 GetCenter()
        {
            return new Vector2(columns / 2.0f, rows / 2.0f);
        }


        //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
        void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				
				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

                //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                GameObject obj = Instantiate(tileChoice, randomPosition, Quaternion.identity);
                obj.tag = "Unbuildable";
            }
        }

        //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
        void LayoutVeinAtRandom(GameObject resource, int minimum, int maximum)
        {
            // Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Choose a random number of objects to instantiate within the minimum and maximum limits
            int objectCount = Random.Range(minimum, maximum + 1);
            int maxWidth = Math.Min(objectCount / 3, veinMaxWidth); // Minimum of 3 layers.
            if (maxWidth < 3)
            {
                maxWidth = objectCount;
            }

            //Instantiate objects until the randomly chosen limit objectCount is reached
            int prevWidth = 0;
            float y = randomPosition.y;
            while (objectCount > 0 && y < rows)
            {
                // Okay We have a vein size, but veins are many levels deep.
                // Let's go through a number levels.
                int minWidth = Math.Max(1, (int)Math.Ceiling(prevWidth / 2.0f));
                int width = Math.Min(objectCount, Random.Range(minWidth, maxWidth));
                if (randomPosition.x + width > columns)
                {
                    width = columns - (int)randomPosition.x;
                }

                // Generate the row.
                int startX = (int)randomPosition.x + Random.Range(0, (int)(width / 2));
                for (float x = startX; x < randomPosition.x + width; x++ )
                {
                    Vector3 mypos = new Vector3(x, y, randomPosition.z);
                    int index = gridPositions[(int)x].IndexOf(mypos);
                    // Check we can lay here using our primative method.
                    if (index == -1)
                    {
                        objectCount--; // In case we get stuck.
                        continue;
                    }
                    else
                    {
                        gridPositions[(int)x].RemoveAt(index);
                    }

                    //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                    GameObject obj = Instantiate(resource, mypos, Quaternion.identity);
                    obj.tag = "Unbuildable";

                    objectCount--;
                }

                y++;
            }
        }


        //SetupScene initializes our level and calls the previous functions to lay out the game board
        public void SetupScene ()
		{
			//Creates the outer walls and floor.
			BoardSetup ();
			
			//Reset our list of gridpositions.
			InitialiseList ();
			
			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (resourceTiles, veinCount.minimum, veinCount.maximum);

            // Layout resource veins.
            int count = Random.Range(veinCount.minimum, veinCount.maximum);
            for (int i = 0; i < count; i++)
            {
                //Choose a random tile from tileArray and assign it to tileChoice
                GameObject resource = resourceTiles[Random.Range(0, resourceTiles.Length)];

                LayoutVeinAtRandom (resource, veinSize.minimum, veinSize.maximum);
            }
        }
    }
}
