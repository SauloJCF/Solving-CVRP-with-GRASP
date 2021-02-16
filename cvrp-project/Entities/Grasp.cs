using System;
using System.Collections.Generic;

namespace cvrp_project.Entities
{
    public class Grasp
    {
        private readonly CvrpInstance Instance;
        private const double alpha = 0.05;

        public Grasp(CvrpInstance instance)
        {
            Instance = instance;
        }

        public void ExecuteGrasp(int maxIterations)
        {
            int i = 0;
            Solution bestSolution = new Solution();
            bestSolution.TotalDistance = double.MaxValue;

            while (i < maxIterations)
            {
                Solution partialSolution = BuildGrasp();
                Solution improvement = LocalSearch(partialSolution);
                
                if (improvement.TotalDistance < bestSolution.TotalDistance)
                {
                    bestSolution = improvement.Copy();
                }


                Console.WriteLine(partialSolution.TotalCost.ToString());
                Console.WriteLine(partialSolution.TotalDistance.ToString());
                Console.WriteLine();
                i++;
            }
            Console.WriteLine($"Best Solution: {bestSolution.TotalDistance}");
        }

        public Solution BuildGrasp()
        {
            List<Point> listOfCandidates = new List<Point>();
            listOfCandidates.AddRange(Instance.Points);
            Solution partialSolution = new Solution();
            Point depot = listOfCandidates[Instance.Depot - 1];
            listOfCandidates.Remove(depot);
            Route route = new Route(depot, Instance.MaxCapacity);

            while (listOfCandidates.Count > 0)
            {
                List<Point> restritedListOfCandidates = new List<Point>();
                Point lastPoint = route.LastPoint();
                int posLastPoint = lastPoint.Id - 1;
                double minDistance = double.MaxValue, maxDistance = double.MinValue;

                for (int i = 0; i < listOfCandidates.Count; i++)
                {
                    int posNewPoint = listOfCandidates[i].Id - 1;
                    double distance = Instance.GetDistance(posLastPoint, posNewPoint);

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
                    double distance = Instance.GetDistance(posLastPoint, posNewPoint);

                    if (distance <= cost && (route.Cost + demand) <= Instance.MaxCapacity)
                    {
                        restritedListOfCandidates.Add(listOfCandidates[i]);
                    }
                }
                if (restritedListOfCandidates.Count != 0)
                {
                    Random randomGenerator = new Random();
                    int pos = randomGenerator.Next(0, restritedListOfCandidates.Count);
                    route.InsertPoint(restritedListOfCandidates[pos], Instance.GetDistance(lastPoint.Id - 1, restritedListOfCandidates[pos].Id - 1));
                    listOfCandidates.Remove(restritedListOfCandidates[pos]);
                }
                if (restritedListOfCandidates.Count == 0 || listOfCandidates.Count == 0)
                {
                    route.InsertPoint(depot, Instance.GetDistance(lastPoint.Id - 1, depot.Id - 1));
                    partialSolution.AddRoute(route);
                    route = new Route(depot, Instance.MaxCapacity);
                }

            }
            return partialSolution;
        }

        public Point SelectRandomCandidate(List<Point> bestCandidates)
        {
            Random random = new Random();
            int pos = random.Next(0, bestCandidates.Count);
            Point selectedPoint = bestCandidates[pos];
            return selectedPoint;
        }

        public Solution LocalSearch(Solution partialSolution)
        {
            Solution newSolution = partialSolution.Copy();
            int iterations = 0;
            int maxIterations = 15;

            while (iterations < maxIterations)
            {
                Random randomGenerator = new Random();
                int routePos = randomGenerator.Next(0, partialSolution.Routes.Count);
                Route selectedRoute = partialSolution.Routes[routePos];

                int point1Pos = randomGenerator.Next(0, selectedRoute.Points.Count);
                int point2Pos;

                do
                {
                    point2Pos = randomGenerator.Next(0, selectedRoute.Points.Count);
                } while (point1Pos == point2Pos);

                Route newRoute = IntraRouteExchange(point1Pos, point2Pos, selectedRoute);

                if (newRoute.CurrDistance < selectedRoute.CurrDistance)
                {
                    partialSolution.Routes[routePos] = newRoute;
                    partialSolution.Recalculate();
                    iterations = 0;
                }
                iterations++;
            }
            return newSolution;
        }

        private Route IntraRouteExchange(int i, int j, Route r)
        {
            Route copyRoute = r.Copy();
            Point p1 = r.Points[i].Copy();
            Point p2 = r.Points[j].Copy();
            copyRoute.Points[i] = p2;
            copyRoute.Points[j] = p1;
            copyRoute.Recalculate(Instance);
            return copyRoute;
        }
    }
}
