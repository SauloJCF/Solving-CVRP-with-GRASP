using System.Collections.Generic;
using System.Text;

namespace cvrp_project.Entities
{
    public class Solution
    {
        public List<Route> Routes { get; set; } = new List<Route>();

        public void CreateRoute(Point depot, double maxCapacity)
        {
            Routes.Add(new Route(depot, maxCapacity));
        }

        public Route LastRoute()
        {
            return Routes[^1];
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            double totalDistance = 0, totalCost = 0;
            foreach (var r in Routes)
            {
                sb.AppendLine($"Route {Routes.IndexOf(r)}");
                sb.AppendLine(r.ToString());
                totalDistance += r.CurrDistance;
                totalCost += r.Cost;
            }
            sb.AppendLine($"Total distance: {totalDistance}");
            sb.AppendLine($"Total Cost: {totalCost}");
            return sb.ToString();
        }
    }
}
