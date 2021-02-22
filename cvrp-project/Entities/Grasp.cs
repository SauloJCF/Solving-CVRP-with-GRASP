using System;
using System.Collections.Generic;

namespace cvrp_project.Entities
{
    public class Grasp
    {
        private CvrpInstance Instance;
        private double Alpha;

        public Grasp()
        {
        }

        public Solution ExecuteGrasp(CvrpInstance instance, int maxIterations, double alpha)
        {
            Alpha = alpha;
            Instance = instance;
            double sum = 0;
            int i = 0;
            Solution bestSolution = new Solution
            {
                TotalDistance = double.MaxValue
            };

            while (i < maxIterations)
            {
                Solution partialSolution = BuildGrasp();
                // Console.WriteLine("Partial Distance: " + partialSolution.TotalDistance.ToString());

                Solution improvement = LocalSearch(partialSolution);
                // Console.WriteLine("Improved Distance: " + improvement.TotalDistance.ToString());
                // Console.WriteLine();
                sum += improvement.TotalDistance;
                if (improvement.TotalDistance < bestSolution.TotalDistance)
                {
                    bestSolution = improvement.Copy();
                }
                i++;
            }
            Console.WriteLine($"Média: {sum/maxIterations}");

            Console.WriteLine($"Best Solution:");
            Console.WriteLine(bestSolution.ToString());
            return bestSolution;
        }

        public Solution BuildGrasp()
        {
            List<Point> listOfCandidates = new List<Point>();
            listOfCandidates.AddRange(Instance.Points);
            Solution partialSolution = new Solution();

            // Remove o depósito da lista de candidatos
            Point depot = listOfCandidates[Instance.Depot - 1];
            listOfCandidates.Remove(depot);

            // Inicia uma rota, com o ponto inicial sendo o depósito
            Route route = new Route(depot, Instance.MaxCapacity);

            while (listOfCandidates.Count > 0)
            {
                List<Point> restrictedListOfCandidates = new List<Point>();
                Point lastPoint = route.LastPoint();// Retorna o último ponto da rota
                int posLastPoint = lastPoint.Pos;
                double minDistance = double.MaxValue, maxDistance = double.MinValue;

                // Percorre toda a lista de candidatos, para encontrar o mínimo e o máximo
                for (int i = 0; i < listOfCandidates.Count; i++)
                {
                    int posNewPoint = listOfCandidates[i].Pos;
                    double distance = Instance.GetDistance(posLastPoint, posNewPoint);// acessa a matriz de distâncias e retorna a distância

                    if (minDistance > distance)
                        minDistance = distance;

                    if (maxDistance < distance)
                        maxDistance = distance;
                }

                // calcula o custo de acordo com os valores encotrados
                double cost = minDistance + Alpha * (maxDistance - minDistance);

                // percorre a lista de candidatos e adiciona apenas aqueles que estão abaixo do custo
                for (int i = 0; i < listOfCandidates.Count; i++)
                {
                    int posNewPoint = listOfCandidates[i].Pos;
                    double demand = listOfCandidates[i].Demand;
                    double distance = Instance.GetDistance(posLastPoint, posNewPoint);

                    if (distance <= cost && (route.TotalCapacity + demand) <= Instance.MaxCapacity)
                    {
                        restrictedListOfCandidates.Add(listOfCandidates[i]);
                    }
                }

                // seleciona um candidato aleatório da lista e adiciona à rota
                if (restrictedListOfCandidates.Count != 0)
                {
                    Random randomGenerator = new Random();
                    int pos = randomGenerator.Next(0, restrictedListOfCandidates.Count);
                    route.InsertPoint(restrictedListOfCandidates[pos], Instance.GetDistance(lastPoint.Pos, restrictedListOfCandidates[pos].Pos));
                    listOfCandidates.Remove(restrictedListOfCandidates[pos]);
                }
                // se nenhum candidato for adicionado à lista restrita ou se não houverem mais candidatos, o depósito é adicionado ao final da rota e uma nova é criada
                if (restrictedListOfCandidates.Count == 0 || listOfCandidates.Count == 0)
                {
                    route.InsertPoint(depot, Instance.GetDistance(lastPoint.Pos, depot.Pos));
                    partialSolution.AddRoute(route);
                    route = new Route(depot, Instance.MaxCapacity);
                }

            }
            return partialSolution;
        }

        public Point SelectRandomCandidate(List<Point> bestCandidates)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int pos = random.Next(0, bestCandidates.Count);
            Point selectedPoint = bestCandidates[pos];
            return selectedPoint;
        }

        public Solution LocalSearch(Solution partialSolution)
        {
            int maxIterations = 35, iterations = 0;
            Random randomGenerator = new Random(DateTime.Now.Millisecond);
            Solution bestSolution = partialSolution.Copy();

            while (iterations < maxIterations)
            {
                Solution newSolution = partialSolution.Copy();
                int routePos1 = randomGenerator.Next(0, partialSolution.Routes.Count);
                int routePos2 = routePos1;

                while (routePos2 == routePos1)
                {
                    routePos2 = randomGenerator.Next(0, partialSolution.Routes.Count);
                }
                Route r1 = partialSolution.Routes[routePos1].Copy();
                Route r2 = partialSolution.Routes[routePos2].Copy();

                int pointPos1 = randomGenerator.Next(1, r1.Points.Count - 1);
                int pointPos2 = randomGenerator.Next(1, r2.Points.Count - 1);
                Route[] newRoutes = Exchange(pointPos1, pointPos2, r1, r2);


                if (r1.TotalDistance + r2.TotalDistance > newRoutes[0].TotalDistance + newRoutes[1].TotalDistance && newRoutes[0].TotalCapacity <= newRoutes[0].MaxCapacity && newRoutes[1].TotalCapacity <= newRoutes[1].MaxCapacity)
                {
                    Route bestRoute1 = IntraRouteExchange(newRoutes[0]);
                    Route bestRoute2 = IntraRouteExchange(newRoutes[1]);
                    newSolution.Routes[routePos1] = bestRoute1.Copy();
                    newSolution.Routes[routePos2] = bestRoute2.Copy();
                    newSolution.Recalculate();
                    bestSolution = newSolution.Copy();
                    iterations = 0;
                    //Console.WriteLine("Melhoria");
                }

                iterations++;
            }
            return bestSolution;
        }

        private Route IntraRouteExchange(Route route)
        {
            Route bestRoute = route.Copy();

            for (int j = 1; j < route.Points.Count - 2; j++)
            {
                Route newRoute = Exchange(j, j + 1, route);
                if (newRoute.TotalDistance < bestRoute.TotalDistance)
                {
                    bestRoute = newRoute.Copy();
                }
            }
            return bestRoute.Copy();
        }

        private Route Exchange(int i, int j, Route r)
        {
            Route copyRoute = r.Copy();
            Point p1 = r.Points[i].Copy();
            Point p2 = r.Points[j].Copy();
            copyRoute.Points[i] = p2;
            copyRoute.Points[j] = p1;
            copyRoute.Recalculate(Instance);
            return copyRoute;
        }

        private Route[] Exchange(int i, int j, Route r1, Route r2)
        {
            Route copyR1 = r1.Copy();
            Route copyR2 = r2.Copy();

            Point aux = copyR1.Points[i].Copy();
            copyR1.Points[i] = copyR2.Points[j].Copy();
            copyR2.Points[j] = aux;

            copyR1.Recalculate(Instance);
            copyR2.Recalculate(Instance);

            Route[] routes = new Route[2] { copyR1, copyR2 };
            return routes;
        }
    }
}
