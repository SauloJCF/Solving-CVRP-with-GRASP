using System;
using System.Collections.Generic;
using System.Text;

namespace cvrp_project.Entities
{
    public class Route
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public double TotalCost { get; private set; }
        public double MaxCapacity { get; private set; }
        public double TotalDistance { get; set; }

        public Route()
        {

        }

        public Route(Point depot, double maxCapacity)
        {
            Points.Add(depot);
            MaxCapacity = maxCapacity;
            TotalCost = 0;
            TotalDistance = 0;
        }

        public Point LastPoint()
        {
            return Points[^1];
        }

        public void InsertPoint(Point newPoint, double distance)
        {
            if (TotalCost + newPoint.Demand > MaxCapacity)
                throw new ApplicationException("This point exceeds max capacity");
            Points.Add(newPoint);
            TotalCost += LastPoint().Demand;
            TotalDistance += distance;
        }

        public Route Copy()
        {
            Route copyRoute = new Route();
            copyRoute.TotalCost = TotalCost;
            copyRoute.TotalDistance = TotalDistance;
            copyRoute.MaxCapacity = MaxCapacity;
            copyRoute.Points.AddRange(Points);
            return copyRoute;
        }

        public void Recalculate(CvrpInstance instance)
        {
            TotalDistance = 0;
            TotalCost = Points[0].Demand;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                TotalCost += Points[i + 1].Demand;
                TotalDistance += instance.GetDistance(Points[i].Pos, Points[i + 1].Pos);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in Points)
            {
                sb.Append($"{p.Id} ");
            }
            sb.AppendLine();
            sb.AppendLine($"Cost: {TotalCost.ToString()}");
            sb.AppendLine($"Distance: {TotalDistance.ToString()}");
            return sb.ToString();
        }
    }
}
