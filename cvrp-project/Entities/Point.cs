using System;

namespace cvrp_project.Entities
{
    public class Point
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Demand { get; set; }
        public int Pos
        {
            get
            {
                return Id - 1;
            }
        }

        public double CalculateDistance(Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - X, 2) + Math.Pow(p2.Y - Y, 2));
        }

        public override string ToString()
        {
            return $"Id: {Id}; X: {X}; Y: {Y}; Demand: {Demand};";
        }

        public Point Copy()
        {
            Point copyPoint = new Point();
            copyPoint.Id = Id;
            copyPoint.X = X;
            copyPoint.Y = Y;
            copyPoint.Demand = Demand;
            return copyPoint;
        }
    }
}
