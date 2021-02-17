using System;
using System.Collections.Generic;
using System.Text;

namespace cvrp_project.Entities
{
    public class Route
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public double Cost { get; private set; }
        public double MaxCapacity { get; private set; }
        public double CurrDistance { get; set; }

        public Route()
        {

        }

        public Route(Point depot, double maxCapacity)
        {
            Points.Add(depot);
            MaxCapacity = maxCapacity;
            Cost = 0;
            CurrDistance = 0;
        }

        public Point LastPoint()
        {
            return Points[^1];
        }

        public void InsertPoint(Point newPoint, double distance)
        {
            if (Cost + newPoint.Demand > MaxCapacity)
                throw new ApplicationException("This point exceeds max capacity");
            Points.Add(newPoint);
            Cost += LastPoint().Demand;
            CurrDistance += distance;
        }

        public Route Copy()
        {
            Route copyRoute = new Route();
            copyRoute.Cost = Cost;
            copyRoute.CurrDistance = CurrDistance;
            copyRoute.MaxCapacity = MaxCapacity;
            copyRoute.Points.AddRange(Points);
            return copyRoute;
        }

        public void Recalculate(CvrpInstance instance)
        {
            CurrDistance = 0;
            Cost = Points[0].Demand;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                Cost += Points[i + 1].Demand;
                CurrDistance += instance.GetDistance(Points[i].Pos, Points[i + 1].Pos);
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
            sb.AppendLine($"Cost: {Cost.ToString()}");
            sb.AppendLine($"Distance: {CurrDistance.ToString()}");
            return sb.ToString();
        }
    }
}
