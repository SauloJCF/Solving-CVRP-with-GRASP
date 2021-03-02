using System.Collections.Generic;
using System.IO;
using System.Text;

namespace cvrp_project.Entities
{
    public class Solution
    {
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public double TotalDistance { get; set; }
        public double TotalCost { get; set; }

        public Solution()
        {
            TotalDistance = 0;
            TotalCost = 0;
        }

        public void AddRoute(Vehicle newRoute)
        {
            Vehicles.Add(newRoute);
            TotalDistance += newRoute.TotalDistance;
            TotalCost += newRoute.TotalLoad;
        }

        public Vehicle LastRoute()
        {
            return Vehicles[^1];
        }

        public Solution Copy()
        {
            Solution s = new Solution();
            s.Vehicles = new List<Vehicle>();
            s.TotalCost = this.TotalCost;
            s.TotalDistance = this.TotalDistance;
            s.Vehicles.AddRange(this.Vehicles);
            return s;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            double totalDistance = 0, totalCost = 0;
            foreach (var r in Vehicles)
            {
                sb.AppendLine($"Vehicle {Vehicles.IndexOf(r)}");
                sb.AppendLine(r.ToString());
                totalDistance += r.TotalDistance;
                totalCost += r.TotalLoad;
            }
            sb.AppendLine($"Total distance: {totalDistance}");
            sb.AppendLine($"Total Cost: {totalCost}");
            return sb.ToString();
        }

        public void Recalculate()
        {
            TotalDistance = 0;
            TotalCost = 0;
            foreach (var r in Vehicles)
            {
                TotalDistance += r.TotalDistance;
                TotalCost += r.TotalLoad;
            }
        }

        public void SaveSolution(string filePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Vehicles.Count.ToString());

            foreach (var route in Vehicles)
            {
                sb.AppendLine($"Route #{Vehicles.IndexOf(route)} - Distance: {route.TotalDistance} Capacity: {route.TotalLoad}");
                foreach (var point in route.Route)
                {
                    sb.Append(point.Id + " ");
                }
                sb.AppendLine();
            }
            sb.AppendLine($"Solution Distance: {TotalDistance.ToString()}");
            File.WriteAllText(filePath, sb.ToString());
        }

        public void GerarHTML(CvrpInstance instance, string fileName)
        {
            StringBuilder vertices = new StringBuilder();
            StringBuilder arestas = new StringBuilder();
            StringBuilder modelo = new StringBuilder();
            StringBuilder associations = new StringBuilder();
            int[] routes = new int[instance.Dimension];

            modelo.Append(File.ReadAllText("modelo.html"));

            int cont = 0;

            foreach (var point in instance.Points)
            {
                if (cont != 0)
                {
                    vertices.Append(",");
                }
                cont++;
                vertices.Append("{\"l\":" + point.Id + ", \"x\": " + point.X + ", \"y\": " + point.Y + "}");
            }

            cont = 0;
            foreach (var route in Vehicles)
            {
                for (int i = 0; i < route.Route.Count - 1; i++)
                {
                    if (cont != 0)
                    {
                        arestas.Append(",");
                    }
                    routes[route.Route[i].Pos] = Vehicles.IndexOf(route);

                    arestas.Append("{\"o\": " + route.Route[i].Pos + ", \"d\": " + route.Route[i + 1].Pos + "}");

                    cont++;
                }
            }

            for (int i = 0; i < routes.Length; i++)
            {
                if (i != 0)
                    associations.Append(",");
                associations.Append(routes[i]);
            }

            modelo = modelo.Replace("{v}", vertices.ToString()).Replace("{a}", arestas.ToString()).Replace("{r}", associations.ToString());
            File.WriteAllText(fileName, modelo.ToString());
            //Console.WriteLine(modelo.ToString());
        }
    }
}
