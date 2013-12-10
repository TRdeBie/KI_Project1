using System;
using System.Collections.Generic;

namespace Ants
{
	class MyBot : Bot
    {
        //double weight;
        private List<Location> orders = new List<Location>();
        private Map map = new Map();
        private List<Location> unseenTiles; //Keeps track of what tiles have been seen
        private List<Location> enemyHills = new List<Location>();

		// DoTurn is run once per turn
		public override void DoTurn (IGameState state)
        {
            //For assigning jobs to and keeping track of ants
            List<Route> foodRoutes = new List<Route>();
            List<Location> foodLocations = state.FoodTiles;
            //Keep track of food and which ants are delegated to foodfinding
            List<Location> foodTargets = new List<Location>();
            List<Location> foodAnts = new List<Location>();

            //Add all locations to unseen tiles set, run once
            if (unseenTiles == null)
            {
                unseenTiles = new List<Location>();
                for (int row = 0; row < state.Width; row++)
                {
                    for (int col = 0; col < state.Height; col++)
                    { unseenTiles.Add(new Location(row, col)); }
                }
            }
            //Remove any tiles that can be seen, run each turn
            int count = unseenTiles.Count;
            for (int i = 0; i < count; i++)
            {
                //Check if each tile in unseentiles is visible or not
                if (state.GetIsVisible(unseenTiles[i]))
                {
                    //Add the tile info to the map
                    map.AddLocation(unseenTiles[i], state);
                    //If visible, remove. Then put i back 1 and lower count by 1
                    unseenTiles.RemoveAt(i);
                    i--;
                    count--;
                }
            }

            //To prevent stepping on your own hills
            foreach (AntHill myHill in state.MyHills)
            {
                orders.Add(myHill); //Now the hills always count as occupied
            }
            if (state.TimeRemaining > 10)
            {
                //Create a route from each ant to each food
                foreach (Location food in foodLocations)
                {
                    foreach (Ant ant in state.MyAnts)
                    {
                        int distance = state.GetDistance(ant, food);
                        Route route = new Route(ant, food, distance);
                        foodRoutes.Add(route);
                    }
                }

                //Sort routes on distance (Note: This took waaay too long to figure out)
                foodRoutes.Sort(delegate(Route r1, Route r2)
                {
                    return r1.CompareTo(r2);
                });
                foreach (Route route in foodRoutes)
                {
                    //Check if food has ant sent to it already
                    //Check if ant has been sent already
                    //If not, sent ant to food if move is possible
                    if (!foodTargets.Contains(route.GetEnd)
                        && !foodAnts.Contains(route.GetStart)
                        && DoMoveLocation(route.GetStart, route.GetEnd, state))
                    {
                        foodTargets.Add(route.GetEnd);
                        foodAnts.Add(route.GetStart);
                    }
                    // check if we have time left to calculate more orders
                    if (state.TimeRemaining < 10) break;
                }
            }

            //Add new hills to the list of enemy hills
            foreach (Location enemyHill in state.EnemyHills)
                if (!enemyHills.Contains(enemyHill)) enemyHills.Add(enemyHill);
            //Attack enemy hills
            if (state.TimeRemaining > 10)
            {
                List<Route> hillRoutes = new List<Route>();
                foreach (Location hillLoc in enemyHills)
                {
                    foreach (Location antLoc in state.MyAnts)
                    {
                        //Check for all the ants whether 
                        if (!orders.Contains(antLoc))
                        {
                            int distance = state.GetDistance(antLoc, hillLoc);
                            Route route = new Route(antLoc, hillLoc, distance);
                            hillRoutes.Add(route);
                        }
                    }
                }
                //Sort the routes to enemy hill
                if (state.TimeRemaining > 10)
                {
                    hillRoutes.Sort(delegate(Route r1, Route r2)
                    {
                        return r1.CompareTo(r2);
                    });
                    foreach (Route route in hillRoutes)
                    {
                        //Perform the routes from shortest to longest
                        DoMoveLocation(route.GetStart, route.GetEnd, state);
                        // check if we have time left to calculate more orders
                        if (state.TimeRemaining < 10) break;
                    }
                }
            }
            //Explore unseen areas
            if (state.TimeRemaining > 10)
            {
                foreach (Ant ant in state.MyAnts)
                {
                    if (!orders.Contains(ant))
                    {
                        List<Route> unseenRoutes = new List<Route>();
                        //Find the routes to all unseen tiles in the map
                        foreach (Location unseenLoc in unseenTiles)
                        {
                            int distance = state.GetDistance(ant, unseenLoc);
                            Route route = new Route(ant, unseenLoc, distance);
                            unseenRoutes.Add(route);
                        }
                        //Sort the routes from short to long
                        unseenRoutes.Sort(delegate(Route r1, Route r2)
                        {
                            return r1.CompareTo(r2);
                        });
                        //From closest to furthest, check the routes to unseen tiles
                        foreach (Route route in unseenRoutes)
                            if (DoMoveLocation(route.GetStart, route.GetEnd, state)) break;

                        // check if we have time left to calculate more orders
                        if (state.TimeRemaining < 10) break;
                    }
                }
            }
            if (state.TimeRemaining > 10)
            {
                //Create a list of ants in format Location instead of Ant
                List<Location> antlist = new List<Location>();
                foreach (Location ant in state.MyAnts)
                    antlist.Add(ant);
                //Unblock the hills
                foreach (Location myHill in state.MyHills)
                {
                    if (antlist.Contains(myHill) && !orders.Contains(myHill))
                        foreach (Direction direction in Ants.Aim.Keys)
                            if (DoMoveDirection(myHill, direction, state))
                                break;
                }
            }
            orders.Clear();
		}

        //Handles collision checking and movement tracking for a start and end point
        private bool DoMoveLocation(Location ant, Location destination, IGameState state)
        {
            //Collect the best directions to go to from state.getdirections
            ICollection<Direction> directions = state.GetDirections(ant, destination);
            foreach (Direction direction in directions)
            {
                //Check whether those directions are appropriate
                if (DoMoveDirection(ant, direction, state))
                    return true;
            }
            return false;
        }

        //Handles collision checking and movement tracking
        private bool DoMoveDirection(Location ant, Direction direction, IGameState state)
        {
            // GetDestination will wrap around the map properly
            // and give us a new location
            Location newLoc = state.GetDestination(ant, direction);

            // GetIsPassable returns true if the location is land
            if (state.GetIsPassable(newLoc) && !(orders.Contains(newLoc) && state.GetIsUnoccupied(newLoc)))
            {
                IssueOrder(ant, direction);
                orders.Add(newLoc);
            }
            return false;
        }
		
		public static void Main (string[] args)
        {
            new Ants().PlayGame(new MyBot());
		}
	}
}