using System;
using System.Collections.Generic;

namespace Ants
{
    class Map
    {
        //Keeps track of what the map looks like
        MapTile[,] map;
        int w, h;
        //Key: u = unknown, d = dirt, w = water/wall

        public Map(int width, int height)
        {
            w = width;
            h = height;
            map = new MapTile[width, height];
            //Whole map is unknown at creation
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y] = new MapTile();
                }
            }
        }

        public void AddLocation(Location location, IGameState state)
        {
            //Sets whether a tile is dirt or water
            char tile = 'u';
            if (state.GetIsPassable(location)) tile = 'd';
            else tile = 'w';
            map[location.Col, location.Row].type = tile;
        }

        public Location FindRoute(Location start, Location end)
        {
            //Find a route from start to end, using A* and what we know of the map
            List<Node> closedSet = new List<Node>(); //Set with nodes already evaluated
            List<Node> openSet = new List<Node>(); //Set with nodes that have not yet been evaluated
            //Add the starting node to the openSet
            openSet.Add(new Node(start.Col, start.Row, 0 + DistanceToLocation(start.Col, start.Row, end), 0));

            int counter = 0; //To prevent it from taking too long
            while (openSet.Count > 0 && counter < 50)
            {
                //Sort the openSet, arranging the nodes from high to low and putting the lowest on top of the list
                openSet.Sort(delegate(Node n1, Node n2)
                {
                    return -(int)n1.CompareTo(n2);
                });
                Node current = openSet[openSet.Count - 1]; //Find the last node in the list
                if (current.IsLocation(end))
                {
                    return ReconstructPath(closedSet, end); //Check whether it's the goal
                }
                openSet.RemoveAt(openSet.Count - 1); //Remove the last node from the open list
                closedSet.Add(current); //Add the current node to closed set

                List<Node> neighbours = ConnectedNodes(current, end); //Find the neighbours of the node, aka the traversible nodes
                foreach (Node neighbour in neighbours)
                {
                    double fScore = DistanceToLocation(neighbour.X, neighbour.Y, end); //calculate the f score
                    if (!openSet.Contains(neighbour) || neighbour.TentativeFScore < fScore)
                    {
                        //Ensure that the neighbour knows where it came from
                        neighbour.CameFrom(current);
                        //Add the neighbour to the open set
                        openSet.Add(neighbour);
                    }
                }
                counter++;
            }

            return null;
        }

        public Location ReconstructPath(List<Node> cameFrom, Location end)
        {
            //Should return a direction for the ant to go to
            return end;
        }

        public List<Node> ConnectedNodes(Node current, Location targetLoc)
        {
            //Returns the nodes around a certain node, depending on whether those surrounding nodes are dirt, water or unknown
            //Input requires location of node, with the distance that has been traversed so far
            int x = current.X;
            int y = current.Y;
            //Check left, right, up and down
            List<Node> connected = new List<Node>();
            //Check left
            x--;
            if (x < 0) x = w - 1; //For wrapping around the map
            if (map[x, y].IsPassable)
                connected.Add(new Node(x, y, DistanceToLocation(x, y, targetLoc) + current.DistanceTraversed + 1, current.DistanceTraversed + 1)); //new node has x, y, tentative f score, tentative g score
            //Check right
            x = current.X + 1;
            if (x == w) x = 0; //For wrapping around the map
            if (map[x, y].IsPassable)
                connected.Add(new Node(x, y, DistanceToLocation(x, y, targetLoc) + current.DistanceTraversed + 1, current.DistanceTraversed + 1));
            //Check up
            x = current.X;
            y--;
            if (y < 0) y = h - 1; //For wrapping around the map
            if (map[x, y].IsPassable)
                connected.Add(new Node(x, y, DistanceToLocation(x, y, targetLoc) + current.DistanceTraversed + 1, current.DistanceTraversed + 1));
            //Check down
            y = current.Y + 1;
            if (y == h) y = h - 1; //For wrapping around the map
            if (map[x, y].IsPassable)
                connected.Add(new Node(x, y, DistanceToLocation(x, y, targetLoc) + current.DistanceTraversed + 1, current.DistanceTraversed + 1));
            return connected;
        }

        public double DistanceToLocation(int x, int y, Location locOther)
        {
            //Calculates the distance to a certain location, does not need to be exact distance, only an indication to see how far away it is relative to other tiles
            return Math.Pow((x - locOther.Col), 2) + Math.Pow((y - locOther.Row), 2); //=X^2 + Y^2, no need for square root
        }
    }

    class Node
    {
        //Simple dataclass, holds location information without the extra stuff Location has
        //Also holds information about distance from start to this and from this location to other location
        int x;
        int y;
        int tentative_g_score; //The tentative g score is the g score from the previous node + distance to this node (1)
        double tentative_f_score; //The tentative f score is the tentative g score + estimate of heuristic cost
        Node cameFrom;

        public Node(int col, int row, double distTo, int distCovered)
        {
            x = col;
            y = row;
            tentative_f_score = distTo;
            tentative_g_score = distCovered;
        }

        public double CompareTo(Node n)
        {
            //Compare the distanceToTarget with another node
            return tentative_f_score - n.tentative_f_score;
        }

        public bool IsLocation(Location loc)
        {
            //Returns whether the node is on a certain location
            return (x == loc.Col && y == loc.Row);
        }

        public void CameFrom(Node node)
        {
            cameFrom = node;
        }

        public int X
        { get { return x; } }
        public int Y
        { get { return y; } }
        public int DistanceTraversed
        { get { return tentative_g_score; } }
        public double TentativeFScore
        { get { return tentative_f_score; } }
    }

    class MapTile
    {
        //MapTiles have a few properties to make certain calculations a bit easier
        public char type = 'u'; //type indicates the type of the tile, standard is u for unknown. Other possible types are d for dirt and w for water

        //Maptiles do not need to know their location, that's handled by the Map class
        public MapTile()
        { }

        public bool IsPassable
        {
            //Return false if water, otherwise return true
            get { if (type == 'w') return false; return true; }
        }
    }
}
