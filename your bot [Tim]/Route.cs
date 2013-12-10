using System;
using System.Collections.Generic;

namespace Ants
{
    class Route
    {
        //This class stores a route from one point to another
        private Location start, end;
        private int distance;

        public Route(Location s, Location e, int d)
        {
            start = s;
            end = e;
            distance = d;
        }

        public Location GetStart
        {
            get { return start; }
        }

        public Location GetEnd
        {
            get { return end; }
        }

        public int GetDistance
        {
            get { return distance; }
        }

        public int CompareTo(Route route)
        {
            return distance - route.distance;
        }

        public int HashCode(IGameState state)
        {
            // =start.hashcode * mapsize * mapsize + end.hashcode
            return start.GetHashCode() * state.Width * state.Height * state.Width * state.Height + end.GetHashCode();
        }

        public bool Equals(Object o)
        {
            //Returns whether two routes are in fact from the same start to end
            bool result = false;
            if (o.GetType() == this.GetType())
            {
                Route route = (Route)o;
                result = start.Equals(route.start) && end.Equals(route.end);
            }
            return result;
        }
    }
}
