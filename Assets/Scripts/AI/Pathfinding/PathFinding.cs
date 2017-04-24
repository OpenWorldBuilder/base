using UnityEngine;
using System.Collections;
using ReGoap.Planner;
using System.Collections.Generic;

namespace WorldBuilder.AI.PathFinding
{
    public class PathFinding : MonoBehaviour
    {
        internal BoardManager boardScript;
        Dictionary<Vector3, Dictionary<Vector3, int>> boardstate;
        Dictionary<Vector3, Dictionary<Vector3, List<Vector3>>> cache;

        //Awake is always called before any Start functions
        void Awake()
        {
            boardScript = GetComponent<BoardManager>();
            boardstate = new Dictionary<Vector3, Dictionary<Vector3, int>>(boardScript.columns * boardScript.rows);
            cache = new Dictionary<Vector3, Dictionary<Vector3, List<Vector3>>>();

            //Loop along x axis.
            for (int x = 0; x < boardScript.columns; x++)
            {
                //Loop along y axis.
                for (int y = 0; y < boardScript.rows; y++)
                {
                    Vector3 vec = new Vector3(x, y, 0f);

                    Dictionary<Vector3, int> surrounding = new Dictionary<Vector3, int>();
                    boardstate.Add(vec, BuildChildren(vec));
                }
            }
        }

        // Given a PathNode, get all nodes around it.
        private Dictionary<Vector3, int> BuildChildren(Vector3 vec)
        {
            Dictionary<Vector3, int> children = new Dictionary<Vector3, int>();

            //Loop along x axis.
            for (float x = vec.x - 1; x <= vec.x + 1; x++)
            {
                //Loop along y axis.
                for (float y = vec.y - 1; y <= vec.y + 1; y++)
                {
                    if (x < 0 || y < 0 || x > boardScript.columns -1 || y > boardScript.rows -1 || (vec.x == x && vec.y == y))
                    {
                        continue;
                    }

                    children.Add(new Vector3(x, y, 0f), 1);
                }
            }

            return children;
        }

        // Get a path to a location.
        public List<Vector3> GetPath(Vector3 from, Vector3 to)
        {
            from = new Vector3(Mathf.Round(from.x), Mathf.Round(from.y), 0.0f);
            to = new Vector3(Mathf.Round(to.x), Mathf.Round(to.y), 0.0f);

            if (cache.ContainsKey(from) && cache[from].ContainsKey(to))
            {
                return cache[from][to];
            }

            var previous = new Dictionary<Vector3, Vector3>();
            var distances = new Dictionary<Vector3, int>();
            var nodes = new List<Vector3>();

            List<Vector3> path = null;

            foreach (var vertex in boardstate)
            {
                if (vertex.Key == from)
                {
                    distances[vertex.Key] = 0;
                }
                else
                {
                    distances[vertex.Key] = int.MaxValue;
                }

                nodes.Add(vertex.Key);
            }

            while (nodes.Count > 0)
            {
                nodes.Sort((x, y) => distances[x] - distances[y]); // TODO - add heuristic algorithm based on raycast?

                Vector3 smallest = nodes[0];
                if (!nodes.Remove(smallest))
                {
                    return null;
                }

                if (smallest == to)
                {
                    path = new List<Vector3>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in boardstate[smallest])
                {
                    int alt = distances[smallest] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            if (!cache.ContainsKey(from))
            {
                cache[from] = new Dictionary<Vector3, List<Vector3>>();
            }

            cache[from][to] = path;

            return path;
        }

        // Called when the boardmanager becomes aware of a new item being placed.
        public void OnBoardUpdate(Vector3 pos, GameObject obj)
        {
            pos.z = 0.0f;

            int cost = 1;
 
            // Is this a wall?
            string layer = obj.GetComponent<SpriteRenderer>().sortingLayerName;
            if (layer != "Floor" && layer != "GroundItems")
            {
                cost = int.MaxValue;
            }

            Dictionary<Vector3, int> value;
            if (!boardstate.TryGetValue(pos, out value))
            {
                Debug.Log("Invalid position in pathfinder boardstate!");
                return;
            }

            foreach (Vector3 child in value.Keys)
            {
                Dictionary<Vector3, int> links;
                if (boardstate.TryGetValue(child, out links))
                {
                    // Update cost to pos.
                    links[pos] = cost;
                }
            }

            cache = new Dictionary<Vector3, Dictionary<Vector3, List<Vector3>>>();
        }
    }
}