using System;
using System.Collections.Generic;
using System.Text;

namespace cvrp_project.Entities
{
    public class Cvrp
    {
        public int Depot { get; set; }
        public int Dimension { get; set; }
        public double Capacity { get; set; }
        public List<Point> Points { get; set; } = new List<Point>();

        public void AddPoint(Point p)
        {
            Points.Add(p);
        }

        public void SetDemand(int pos, double demand)
        {
            if (pos >= Points.Count || pos < 0)
                throw new ApplicationException("Posição inválida.");
            Points[pos].Demand = demand;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.AppendLine($"Depot: {Depot}");
            sb.AppendLine($"Dimension: {Dimension}");
            sb.AppendLine($"Capacity: {Capacity}");
            foreach (var p in Points)
            {
                sb.AppendLine(p.ToString());
            }
            return sb.ToString();
        }
    }
}
