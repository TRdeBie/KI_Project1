using System;
using System.Collections.Generic;

namespace Ants
{
	class MyBot : Bot
    {
        //double weight;
        List<Location> orders = new List<Location>();
        List<Route> foodRoutes = new List<Route>();

        // DoTurn is run once per turn
        public override void DoTurn(IGameState state)
        {
            //Clear all orders before starting the loop
            orders.Clear();
            foodRoutes.Clear();
            List<Location> visibleFood = new List<Location>(state.FoodTiles);
            List<Location> myAnts = new List<Location>(state.MyAnts);

            //Find the shortest routes from ants to food
            foreach (Location foodLoc in visibleFood)
            {
                //For each foodlocation, go through all the ants and 
                //compare their location to the current closest ant
                Location closestAnt = myAnts[0];
                int closestDistance = state.Width * state.Height * 10;
                foreach (Location antLoc in myAnts) 
                {
                    //Find the distance from ant to food
                    int distance = state.GetDistance(antLoc, foodLoc);
                    if (distance < closestDistance) 
                    {
                        //If the distance is closer than the current closest, make this ant the closest one
                        closestDistance = distance;
                        closestAnt = antLoc;
                    }
                }
                //After checking all ants, create a route from the closest ant to the food
                Route route = new Route(closestAnt, foodLoc, closestDistance);
                foodRoutes.Add(route);
            }

            // loop through all my ants and try to give them orders
            foreach (Ant ant in state.MyAnts)
            {
                //Check to see whether the ant is part of a foodroute
                bool hasOrders = false;
                foreach (Route route in foodRoutes)
                {
                    //If the ant is part of a route to food
                    if (route.Start == ant)
                    {
                        //Get the closest directions to target location
                        ICollection<Direction> directions = state.GetDirections(ant,route.End);
                        foreach (Direction direction in directions)
                        {
                            //Test the directions one by one
                            Location newLoc = state.GetDestination(ant, direction);
                            if (state.GetIsPassable(newLoc))
                            {
                                //If a move in that direction is possible, perform it
                                IssueOrder(ant, direction);
                                orders.Add(newLoc);
                                hasOrders = true;
                                break;
                            }
                        }
                    }
                }
                // try all the directions as usual if not given orders above
                if (!hasOrders)
                    foreach (Direction direction in Ants.Aim.Keys)
                    {
                        // GetDestination will wrap around the map properly
                        // and give us a new location
                        Location newLoc = state.GetDestination(ant, direction);

                        // GetIsPassable returns true if the location is land
                        if (state.GetIsPassable(newLoc) && !(orders.Contains(newLoc)))
                        {
                            IssueOrder(ant, direction);
                            orders.Add(newLoc);
                            break;
                        }
                    }

                // check if we have time left to calculate more orders
                if (state.TimeRemaining < 10) break;
            }
        }
        /*
		// DoTurn is run once per turn
		public override void DoTurnOld (IGameState state)
        {
			// loop through all my ants and try to give them orders
			foreach (Ant ant in state.MyAnts)
            {
				// try all the directions
				foreach (Direction direction in Ants.Aim.Keys)
                {
					// GetDestination will wrap around the map properly
					// and give us a new location
					Location newLoc = state.GetDestination(ant, direction);

					// GetIsPassable returns true if the location is land
					if (state.GetIsPassable(newLoc) && !(orders.Contains(newLoc)))
                    {
						IssueOrder(ant, direction);
                        orders.Add(newLoc);
						break;
					}
				}
				
				// check if we have time left to calculate more orders
				if (state.TimeRemaining < 10) break;
			}
            orders.Clear();
		}
		*/
		
		public static void Main (string[] args)
        {
            new Ants().PlayGame(new MyBot());
		}
	}
}