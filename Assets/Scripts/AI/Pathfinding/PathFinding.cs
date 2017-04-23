using UnityEngine;
using System.Collections;
using ReGoap.Planner;
using System.Collections.Generic;

namespace WorldBuilder.AI.PathFinding
{
    public class PathFinding : MonoBehaviour
    {
        internal BoardManager boardScript;
        private Dictionary<Vector3, PathNode> boardstate;

        //Awake is always called before any Start functions
        void Awake()
        {
            boardScript = GetComponent<BoardManager>();
            boardstate = new Dictionary<Vector3, PathNode>(boardScript.columns * boardScript.rows);

            //Loop along x axis.
            for (int x = 0; x < boardScript.columns; x++)
            {
                //Loop along y axis.
                for (int y = 0; y < boardScript.rows; y++)
                {
                    Vector3 vec = new Vector3(x, y, 0f);
                    boardstate.Add(vec, new PathNode(vec));
                }
            }

            // Set all children.
            foreach (PathNode node in boardstate.Values)
            {
                node.SetChildren(GetChildren(node));
            }
        }

        // Given a PathNode, get all nodes around it.
        private List<PathNode> GetChildren(PathNode node)
        {
            List<PathNode> children = new List<PathNode>();
            Vector3 vec = node.position;

            //Loop along x axis.
            for (int x = -1; x <= 1; x++)
            {
                //Loop along y axis.
                for (int y = -1; y <= 1; y++)
                {
                    // Try to grab a child.
                    PathNode child;
                    if (boardstate.TryGetValue(new Vector3(vec.x + x, vec.y + y, 0f), out child))
                    {
                        if (child.position != node.position)
                        {
                            children.Add(child);
                        }
                    }
                }
            }

            return children;
        }

        // Get a path to a location.
        public Vector3? NextNode(Vector3 from, Vector3 to)
        {
            from = new Vector3(Mathf.Round(from.x), Mathf.Round(from.y), 0.0f);
            to = new Vector3(Mathf.Round(to.x), Mathf.Round(to.y), 0.0f);

            PathNode fromNode;
            if (!boardstate.TryGetValue(from, out fromNode))
            {
                Debug.Log("Invalid from position in pathfinder NextNode!");
                return null;
            }

            PathNode toNode;
            if (!boardstate.TryGetValue(to, out toNode) || toNode.extraCost >= 999999999.0f)
            {
                Debug.Log("Invalid to position in pathfinder NextNode!");
                return null;
            }

            return fromNode.GetBestNext(to);
        }

        // Called when the boardmanager becomes aware of a new item being placed.
        public void OnBoardUpdate(Vector3 pos, GameObject obj)
        {
            pos.z = 0.0f;

            PathNode value;
            if (!boardstate.TryGetValue(pos, out value))
            {
                Debug.Log("Invalid position in pathfinder boardstate!");
                return;
            }

            value.OnUpdate(obj);
        }
    }
}