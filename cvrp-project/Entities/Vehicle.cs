using System;
using System.Collections.Generic;
using System.Text;

namespace cvrp_project.Entities
{
    public class Vehicle
    {
        public List<Point> Route { get; set; } = new List<Point>();
        public double TotalLoad { get; private set; }
        public double MaxCapacity { get; private set; }
        public double TotalDistance { get; set; }

        public Vehicle()
        {

        }

        public Vehicle(Point depot, double maxCapacity)
        {
            Route.Add(depot);
            MaxCapacity = maxCapacity;
            TotalLoad = 0;
            TotalDistance = 0;
        }

        public Point LastPoint()
        {
            return Route[^1];
        }

        public void InsertPoint(Point newPoint, double distance)
        {
            if (TotalLoad + newPoint.Demand > MaxCapacity)
                throw new ApplicationException("This point exceeds max capacity");
            Route.Add(newPoint);
            TotalLoad += LastPoint().Demand;
            TotalDistance += distance;
        }

        public Vehicle Copy()
        {
            Vehicle copyRoute = new Vehicle();
            copyRoute.TotalLoad = TotalLoad;
            copyRoute.TotalDistance = TotalDistance;
            copyRoute.MaxCapacity = MaxCapacity;
            copyRoute.Route.AddRange(Route);
            return copyRoute;
        }

        public void Recalculate(CvrpInstance instance)
        {
            TotalDistance = 0;
            TotalLoad = Route[0].Demand;
            for (int i = 0; i < Route.Count - 1; i++)
            {
                TotalLoad += Route[i + 1].Demand;
                TotalDistance += instance.GetDistance(Route[i].Pos, Route[i + 1].Pos);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in Route)
            {
                sb.Append($"{p.Id} ");
            }
            sb.AppendLine();
            sb.AppendLine($"Cost: {TotalLoad.ToString()}");
            sb.AppendLine($"Distance: {TotalDistance.ToString()}");
            return sb.ToString();
        }
    }
}
