using System.Collections.Generic;
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
            TotalDistance += newRoute.CurrDistance;
            TotalCost += newRoute.Cost;
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
                totalDistance += r.CurrDistance;
                totalCost += r.Cost;
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
                TotalDistance += r.CurrDistance;
                TotalCost += r.Cost;
            }
        }
    }
}
