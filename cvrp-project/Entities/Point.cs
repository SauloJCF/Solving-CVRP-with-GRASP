﻿using System;

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
            return new Point
            {
                Id = this.Id,
                X = this.X,
                Y = this.Y,
                Demand = this.Demand,
            };
        }
    }
}
