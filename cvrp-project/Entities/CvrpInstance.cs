using System;
using System.Collections.Generic;
using System.Text;

namespace cvrp_project.Entities
{
    public class CvrpInstance
    {
        public int Depot { get; set; }
        public int Dimension { get; set; }
        public double MaxCapacity { get; set; }
        public List<Point> Points { get; set; } = new List<Point>();
        public double[,] Distances { get; set; }

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

        public double GetDistance(int pos1, int pos2)
        {
            return Distances[pos1, pos2];
        }

        public override string ToString()
        {
            if (Points.Count == 0)
                throw new ApplicationException("Instance not valid.");

            StringBuilder sb = new StringBuilder("");
            sb.AppendLine($"Depot: {Depot}");
            sb.AppendLine($"Dimension: {Dimension}");
            sb.AppendLine($"Capacity: {MaxCapacity}");

            foreach (var p in Points)
            {
                sb.AppendLine(p.ToString());
            }

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    sb.Append($"{Distances[i, j].ToString("F2")} | ");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public void CalculateDistances()
        {
            if (Points.Count == 0)
                throw new ApplicationException("Instance not valid.");

            Distances = new double[Dimension, Dimension];

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    Distances[i, j] = Points[i].CalculateDistance(Points[j]);
                }
            }
        }
    }
}
