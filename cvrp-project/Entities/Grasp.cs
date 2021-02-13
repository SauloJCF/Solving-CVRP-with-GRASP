using System;
using System.Collections.Generic;
using System.Text;

namespace cvrp_project.Entities
{
    public class Grasp
    {
        private const double alpha = 0.4;

        public void BuildGrasp(CvrpInstance instance)
        {
            List<Point> listOfCandidates = new List<Point>();
            listOfCandidates.AddRange(instance.Points);
            Solution partialSolution = new Solution();
            Point depot = listOfCandidates[instance.Depot - 1];
            listOfCandidates.Remove(depot);
            Route route = new Route(depot, instance.MaxCapacity);

            while (listOfCandidates.Count > 0)
            {
                List<Point> listOfRestrictCandidates = new List<Point>();
                Point lastPoint = route.LastPoint();
                int posLastPoint = lastPoint.Id - 1;
                double minDistance = double.MaxValue, maxDistance = double.MinValue;

                for (int i = 0; i < listOfCandidates.Count; i++)
                {
                    int posNewPoint = listOfCandidates[i].Id - 1;
                    double distance = instance.GetDistance(posLastPoint, posNewPoint);

                    if (minDistance > distance)
                        minDistance = distance;

                    if (maxDistance < distance)
                        maxDistance = distance;
                }

                double cost = minDistance + alpha * (maxDistance - minDistance);

                for (int i = 0; i < listOfCandidates.Count; i++)
                {
                    int posNewPoint = listOfCandidates[i].Id - 1;
                    double demand = listOfCandidates[i].Demand;
                    double distance = instance.GetDistance(posLastPoint, posNewPoint);

                    if (distance <= cost && (route.Cost + demand) <= instance.MaxCapacity)
                    {
                        listOfRestrictCandidates.Add(listOfCandidates[i]);
                    }
                }
                if (listOfRestrictCandidates.Count != 0)
                {
                    Random randomGenerator = new Random();
                    int pos = randomGenerator.Next(0, listOfRestrictCandidates.Count);
                    route.InsertPoint(listOfRestrictCandidates[pos], instance.GetDistance(lastPoint.Id - 1, listOfRestrictCandidates[pos].Id - 1));
                    listOfCandidates.Remove(listOfRestrictCandidates[pos]);
                }
                if(listOfRestrictCandidates.Count == 0 || listOfCandidates.Count == 0)
                {
                    route.InsertPoint(depot, instance.GetDistance(lastPoint.Id - 1, depot.Id - 1));
                    partialSolution.Routes.Add(route);
                    route = new Route(depot, instance.MaxCapacity);
                }
                
            }

            Console.WriteLine(partialSolution.ToString());
        }

        public Point SelectRandomCandidate(List<Point> bestCandidates)
        {
            Random random = new Random();
            int pos = random.Next(0, bestCandidates.Count);
            Point selectedPoint = bestCandidates[pos];
            return selectedPoint;
        }

        public bool IsBetterCandidate(Point previousPoint, Point currentPoint, Point newPoint)
        {
            return previousPoint.CalculateDistance(newPoint) < previousPoint.CalculateDistance(currentPoint);
        }

        public void UpdateCandidates(int oldPointPosition, Point newPoint, List<Point> bestCandidates)
        {
            bestCandidates[oldPointPosition] = newPoint;
        }

        public void RefineGrasp()
        {

        }
    }
}
