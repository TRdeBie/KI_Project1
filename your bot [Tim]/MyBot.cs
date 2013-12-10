using System;
using System.Collections.Generic;

namespace Ants
{
	class MyBot : Bot
    {
        //double weight;
        List<Location> orders = new List<Location>();

		// DoTurn is run once per turn
		public override void DoTurn (IGameState state)
        {
            //For assigning jobs to and keeping track of ants
            List<Route> foodRoutes = new List<Route>();
            List<Location> foodLocations = state.FoodTiles;
            List<Ant> ants = state.MyAnts;
            
            //Create a route from each ant to each food
            foreach (Location food in foodLocations)
            {
                foreach (Ant ant in ants)
                {
                    int distance = state.GetDistance(ant, food);
                    Route route = new Route(ant, food, distance);
                    foodRoutes.Add(route);
                }
            }

            //Sort routes on distance
            //Keep track of food and ants
            List<Location> foodTargets = new List<Location>();
            List<Location> foodAnts = new List<Location>();
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
            }

			// loop through all the ants without routes yet
			foreach (Ant ant in ants)
            {
				// check if we have time left to calculate more orders
				if (state.TimeRemaining < 10) break;
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