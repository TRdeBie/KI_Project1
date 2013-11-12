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
		
		
		public static void Main (string[] args)
        {
            new Ants().PlayGame(new MyBot());
		}
	}
}