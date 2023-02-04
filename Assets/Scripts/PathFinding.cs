using System.Linq;
using System.Collections.Generic;
using UnityEngine;


/**
 * @brief This class implements the A*-algorithm for pathfinding and navigation on grids.
 */
public class PathFinding 
{
    /**
     * @brief This class represents a node for the pathfinding algorithm.
     */
    public class PathNode
    {
        /**
         * @brief The coordinates of the node.
         */
        public Vector2Int position;

        /**
         * @brief The cost to get to this node from the starting point.
         */
        public float gValue;

        /**
         * @brief The heuristic cost to get to the target node from this node.
         */
        public float hValue;

        /**
         * @brief The parent node of this node on the optimal path.
         */
        public PathNode parent;

        /**
         * @brief Constructs a default pathnode from the specified coordinates.
         * @param position [in] The coordinates of the node on the grid.
         */
        public PathNode(Vector2Int position)
        {
            this.position = position;
            this.gValue = 0;
            this.hValue = 0;
            this.parent = null;
        }
    } 

    /**
     * @brief Calculates the heursitic cost between two grid positions.
     * @param nodePosition [in] The coordinates of the starting position.
     * @param endPosition  [in] The coordinates of the end position.
     * @retval The heuristic cost to get from the start position to the end position.
     */
    private static float HeuristicDistance(Vector2Int nodePosition, Vector2Int endPosition)
    {
        return Mathf.Sqrt(Mathf.Pow(nodePosition.x - endPosition.x, 2) + 
                          Mathf.Pow(nodePosition.y - endPosition.y, 2));
    }

    /**
     * @brief Constructs the list of walkable neighbour nodes of the specified node in all cardinal and diagonal directions.
     * @param grid        [in] The grid the specified node is on.
     * @param nodes       [in] The list of path nodes of the grid.
     * @param currentNode [in] The node to get the walkable neighbours of.
     * @retval The list of walkable neighbour nodes of the specified path node.
     */
    private static List<PathNode> FindWalkableNeighbours(List<PathNode> nodes, PathNode currentNode)
    {
        // Initializing the list of walkable neighbour coordinates
        List<PathNode> neighbours = new List<PathNode>();

        // Iterating over the possible neighbouring coordinates
        foreach(int x in Enumerable.Range(-1, 3))
        {
            foreach(int y in Enumerable.Range(-1, 3))
            {
                // Finding the pathnode at this position
                PathNode checkedNode = nodes.FirstOrDefault(node => node.position == (currentNode.position + new Vector2Int(x, y)));

                // Checking whether we found pathnode at this position
                if(checkedNode != null)
                {
                    // Adding the current coordinates to the list of walkable neighbours
                    neighbours.Add(checkedNode);
                }
            }
        }

        // Returning the walkable neighbours of the node
        return neighbours;
    }

    /**
     * @brief Finds a path from the specified starting point to the end point on the grid.
     * @param nodes [in] The list of nodes to search for a path on.
     * @param from  [in] The starting coordinates of the pathfinding.
     * @param to    [in] The target position of the pathfinding.
     * @retval The list of path nodes for on the path from the starting point to the end point. 
     */
    public static List<PathNode> FindPath(List<PathNode> nodes, Vector2Int from, Vector2Int to)
    {
        // Initializing the node sets
        List<PathNode> openNodes = new List<PathNode>();
        List<PathNode> closedNodes = new List<PathNode>();

        // Adding the starting position to the open nodes
        PathNode startNode = nodes.Find(x => x.position == from);
        if(startNode != null)
        {
            openNodes.Add(startNode);
        }

        // Initializing the the current node
        PathNode currentNode;

        // Counter for the number of iterations performed
        int iteration = 0;

        // While we have nodes that we did not check
        while(openNodes.Count != 0)
        {
            // If the search takes too long, return an empty path
            if(iteration++ > 1000) return new List<PathNode>();

            // Finding the cheapest node from the open set
            float minCost = openNodes.Min(x => (x.gValue + HeuristicDistance(x.position, to)));
            currentNode = openNodes.First(x => (x.gValue + HeuristicDistance(x.position, to)) == minCost);

            // Moving the current node to the closed set
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            // Checking whether we have finished
            if(currentNode.position == to)
            {
                break;
            }

            // Checking and adding traversable neighbour nodes
            List<PathNode> neighbours = FindWalkableNeighbours(nodes, currentNode);

            // Processing each neighbour
            foreach(PathNode neighbour in neighbours)
            {
                // Checking whether the neighbour value is null
                if(neighbour == null) continue;

                // If we already checked the neighbour, skip it
                if(closedNodes.Contains(neighbour)) continue;

                // Calculating the cost of the neighbour cell
                float neighbourCost = neighbour.gValue + neighbour.hValue;

                // Checking whether we found a better path to the neighbour
                if(neighbourCost < neighbour.gValue || !openNodes.Contains(neighbour))
                {
                    // Updating the neighbour path values
                    neighbour.gValue = neighbourCost;
                    neighbour.hValue = HeuristicDistance(neighbour.position, to);
                    neighbour.parent = currentNode;

                    // Checking whether we need to add the neighbour to the open set
                    if(!openNodes.Contains(neighbour))
                    {
                        openNodes.Add(neighbour);
                    }
                }
            }
        }

        // Initializing the list of path nodes from the starting point to the end point
        List<PathNode> path = new List<PathNode>();
        currentNode = nodes.Find(x => x.position == to);

        // Retracing the path from the target node to the start node
        while(currentNode != null && currentNode.parent != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        // Reversing the list to start from the starting point and end with the ending point
        path.Reverse();

        // Returning the list of path nodes from the starting point to the end point
        return path;
    }
}

