using System;
using System.Collections.Generic;
using System.Text;

namespace cvrp_project.Entities
{
    public class Route
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public double Cost { get; private set; }
        private readonly double MaxCapacity;
        public double CurrDistance { get; set; }

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
