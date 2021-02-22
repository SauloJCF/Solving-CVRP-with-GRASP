using System.Collections.Generic;
using System.IO;
using System.Text;

namespace cvrp_project.Entities
{
    public class Solution
    {
        public List<Route> Routes { get; set; } = new List<Route>();
        public double TotalDistance { get; set; }
        public double TotalCost { get; set; }

        public Solution()
        {
            TotalDistance = 0;
            TotalCost = 0;
        }

        public void AddRoute(Route newRoute)
        {
            Routes.Add(newRoute);
            TotalDistance += newRoute.TotalDistance;
            TotalCost += newRoute.TotalCapacity;
        }

        public Route LastRoute()
        {
            return Routes[^1];
        }

        public Solution Copy()
        {
            Solution s = new Solution();
            s.Routes = new List<Route>();
            s.TotalCost = this.TotalCost;
            s.TotalDistance = this.TotalDistance;
            s.Routes.AddRange(this.Routes);
            return s;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            double totalDistance = 0, totalCost = 0;
            foreach (var r in Routes)
            {
                sb.AppendLine($"Route {Routes.IndexOf(r)}");
                sb.AppendLine(r.ToString());
                totalDistance += r.TotalDistance;
                totalCost += r.TotalCapacity;
            }
            sb.AppendLine($"Total distance: {totalDistance}");
            sb.AppendLine($"Total Cost: {totalCost}");
            return sb.ToString();
        }

        public void Recalculate()
        {
            TotalDistance = 0;
            TotalCost = 0;
            foreach (var r in Routes)
            {
                TotalDistance += r.TotalDistance;
                TotalCost += r.TotalCapacity;
            }
        }

        public void SaveSolution(string filePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Routes.Count.ToString());

            foreach (var route in Routes)
            {
                sb.AppendLine($"Route #{Routes.IndexOf(route)} - Distance: {route.TotalDistance} Capacity: {route.TotalCapacity}");
                foreach (var point in route.Points)
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
            foreach (var route in Routes)
            {
                for (int i = 0; i < route.Points.Count - 1; i++)
                {
                    if (cont != 0)
                    {
                        arestas.Append(",");
                    }
                    routes[route.Points[i].Pos] = Routes.IndexOf(route);

                    arestas.Append("{\"o\": " + route.Points[i].Pos + ", \"d\": " + route.Points[i + 1].Pos + "}");

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
