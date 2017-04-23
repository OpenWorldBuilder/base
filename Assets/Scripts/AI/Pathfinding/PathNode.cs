using UnityEngine;
using System.Collections;
using ReGoap.Planner;
using System;
using System.Collections.Generic;

namespace WorldBuilder.AI.PathFinding
{
    public class PathNode
    {
        List<PathNode> children;
        internal Vector3 position;
        internal float extraCost;

        internal PathNode(Vector3 position)
        {
            this.position = position;
        }

        internal void SetChildren(List<PathNode> children)
        {
            this.children = children;
        }

        internal void OnUpdate(GameObject obj)
        {
            if (obj == null)
            {
                extraCost = 0f;
                return;
            }

            // Is this a wall?
            String layer = obj.GetComponent<SpriteRenderer>().sortingLayerName;
            if (layer != "Floor" && layer != "GroundItems")
            {
                extraCost = 9999999.0f;
            }
        }

        private float GetNodeCost(Vector3 destination)
        {
            float xDelta = Math.Abs(position.x - destination.x);
            float yDelta = Math.Abs(position.y - destination.y);
            if (xDelta > float.Epsilon && yDelta > float.Epsilon)
            {
                return extraCost + ((xDelta + yDelta) / 2.0f);
            }

            return extraCost + xDelta + yDelta;
        }

        internal float GetPathCost(Vector3 destination, List<PathNode> visited, float bestCost = 0.0f)
        {
            visited.Add(this);

            // Calculate our cost.
            float cost = GetNodeCost(destination);
            if (bestCost > float.Epsilon && bestCost < cost)
            {
                return 9999999.0f; // Cancel out early, A* style.
            }

            // Calculate child cost.
            float minCost = bestCost;
            foreach (PathNode node in children)
            {
                if (visited.Contains(node))
                {
                    continue;
                }

                // We're clear!
                float nodeCost = node.GetPathCost(destination, visited, minCost);
                if (minCost <= float.Epsilon || minCost > nodeCost)
                {
                    minCost = nodeCost;
                }
            }

            return cost + minCost;
        }

        internal Vector3? GetBestNext(Vector3 destination)
        {
            Vector3? best = null;
            float minCost = 0.0f;
            foreach (PathNode node in children)
            {
                float cost = node.GetPathCost(destination, new List<PathNode>(), minCost);
                if (minCost <= float.Epsilon || minCost > cost)
                {
                    best = node.position;
                    minCost = cost;
                }
            }

            return best;
        }
    }
}
