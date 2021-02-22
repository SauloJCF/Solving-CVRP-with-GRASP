using System;
using System.Collections.Generic;
using System.Text;

namespace cvrp_project.Entities
{
    public class Route
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public double TotalCapacity { get; private set; }
        public double MaxCapacity { get; private set; }
        public double TotalDistance { get; set; }

        public Route()
        {

        }

        public Route(Point depot, double maxCapacity)
        {
            Points.Add(depot);
            MaxCapacity = maxCapacity;
            TotalCapacity = 0;
            TotalDistance = 0;
        }

        public Point LastPoint()
        {
            return Points[^1];
        }

        public void InsertPoint(Point newPoint, double distance)
        {
            if (TotalCapacity + newPoint.Demand > MaxCapacity)
                throw new ApplicationException("This point exceeds max capacity");
            Points.Add(newPoint);
            TotalCapacity += LastPoint().Demand;
            TotalDistance += distance;
        }

        public Route Copy()
        {
            Route copyRoute = new Route();
            copyRoute.TotalCapacity = TotalCapacity;
            copyRoute.TotalDistance = TotalDistance;
            copyRoute.MaxCapacity = MaxCapacity;
            copyRoute.Points.AddRange(Points);
            return copyRoute;
        }

        public void Recalculate(CvrpInstance instance)
        {
            TotalDistance = 0;
            TotalCapacity = Points[0].Demand;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                TotalCapacity += Points[i + 1].Demand;
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
            sb.AppendLine($"Cost: {TotalCapacity.ToString()}");
            sb.AppendLine($"Distance: {TotalDistance.ToString()}");
            return sb.ToString();
        }
    }
}
