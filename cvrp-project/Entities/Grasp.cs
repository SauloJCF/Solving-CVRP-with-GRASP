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
            Vehicle route = new Vehicle(depot, Instance.MaxCapacity);

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

                    if (distance <= cost && (route.TotalLoad + demand) <= Instance.MaxCapacity)
                    {
                        restrictedListOfCandidates.Add(listOfCandidates[i]);
                    }
                }

                // seleciona um candidato aleatório da lista e adiciona à rota
                if (restrictedListOfCandidates.Count != 0)
                {
                    //Console.WriteLine(restrictedListOfCandidates.Count);
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
                    route = new Vehicle(depot, Instance.MaxCapacity);
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
                Solution newSolution = bestSolution.Copy();
                int routePos1 = randomGenerator.Next(0, partialSolution.Vehicles.Count);
                int routePos2 = routePos1;

                while (routePos2 == routePos1)
                {
                    routePos2 = randomGenerator.Next(0, partialSolution.Vehicles.Count);
                }
                Vehicle r1 = partialSolution.Vehicles[routePos1].Copy();
                Vehicle r2 = partialSolution.Vehicles[routePos2].Copy();

                int pointPos1 = randomGenerator.Next(1, r1.Route.Count - 1);
                int pointPos2 = randomGenerator.Next(1, r2.Route.Count - 1);
                Vehicle[] newRoutes = Exchange(pointPos1, pointPos2, r1, r2);


                if (r1.TotalDistance + r2.TotalDistance > newRoutes[0].TotalDistance + newRoutes[1].TotalDistance && newRoutes[0].TotalLoad <= newRoutes[0].MaxCapacity && newRoutes[1].TotalLoad <= newRoutes[1].MaxCapacity)
                {
                    Vehicle bestRoute1 = IntraRouteExchange(newRoutes[0]);
                    Vehicle bestRoute2 = IntraRouteExchange(newRoutes[1]);
                    newSolution.Vehicles[routePos1] = bestRoute1.Copy();
                    newSolution.Vehicles[routePos2] = bestRoute2.Copy();
                    newSolution.Recalculate();
                    bestSolution = newSolution.Copy();
                    iterations = 0;
                    //Console.WriteLine("Melhoria");
                }

                iterations++;
            }
            for (int i = 0; i < bestSolution.Vehicles.Count; i++)
            {
                bestSolution.Vehicles[i] = IntraRouteExchange(bestSolution.Vehicles[i].Copy());
            }
            bestSolution.Recalculate();
            return bestSolution;
        }

        private Vehicle IntraRouteExchange(Vehicle route)
        {
            Vehicle bestRoute = route.Copy();

            for (int j = 1; j < route.Route.Count - 2; j++)
            {
                Vehicle newRoute = Exchange(j, j + 1, route);
                if (newRoute.TotalDistance < bestRoute.TotalDistance)
                {
                    bestRoute = newRoute.Copy();
                }
            }
            return bestRoute.Copy();
        }

        private Vehicle Exchange(int i, int j, Vehicle r)
        {
            Vehicle copyRoute = r.Copy();
            Point p1 = r.Route[i].Copy();
            Point p2 = r.Route[j].Copy();
            copyRoute.Route[i] = p2;
            copyRoute.Route[j] = p1;
            copyRoute.Recalculate(Instance);
            return copyRoute;
        }

        private Vehicle[] Exchange(int i, int j, Vehicle r1, Vehicle r2)
        {
            Vehicle copyR1 = r1.Copy();
            Vehicle copyR2 = r2.Copy();

            Point aux = copyR1.Route[i].Copy();
            copyR1.Route[i] = copyR2.Route[j].Copy();
            copyR2.Route[j] = aux;

            copyR1.Recalculate(Instance);
            copyR2.Recalculate(Instance);

            Vehicle[] routes = new Vehicle[2] { copyR1, copyR2 };
            return routes;
        }
    }
}
