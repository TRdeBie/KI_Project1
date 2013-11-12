using System;
using System.Collections.Generic;

namespace Ants
{
    class Route
    {
        //Represents a route from one tile to another
        private Location start;
        private Location end;
        private int distance;

        public Route(Location s, Location e, int d) {
            start = s;
            end = e;
            distance = d;
        }

        public Location Start {
            get { return start; }
        }

        public Location End {
            get { return end; }
        }

        public int Distance {
            get { return distance; }
        }

        //Compare distance to distance of other route
        public int CompareTo(Route route) {
            return distance - route.distance;
        }

        //Create a hashcode for the route
        public int GetHashCode(IGameState state) {
            return start.GetHashCode() * (state.Width * state.Height) * (state.Width * state.Height) * end.GetHashCode();
        }

        //Check whether an object is the same route as this route (from point A to point B)
        public bool Equals(Object o)
        {
            bool result = false;
            if (ReferenceEquals(o, this))
            {
                Route route = (Route)o;
                result = start.Equals(route.Start) && end.Equals(route.End);
            }
            return result;
        }
    }
}
