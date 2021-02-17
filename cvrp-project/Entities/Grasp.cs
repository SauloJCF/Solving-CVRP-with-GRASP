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
                Console.WriteLine($"Iteration: {i}");
                Solution partialSolution = BuildGrasp();
                Console.WriteLine("Partial Distance: " + partialSolution.TotalDistance.ToString());

                Solution improvement = LocalSearch(partialSolution);
                Console.WriteLine("Improved Distance: " + improvement.TotalDistance.ToString());
                Console.WriteLine();

                if (improvement.TotalDistance < bestSolution.TotalDistance)
                {
                    bestSolution = improvement.Copy();
                }
                i++;
            }
            Console.WriteLine($"Best Solution:");
            Console.WriteLine(bestSolution.ToString());
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
                double cost = minDistance + alpha * (maxDistance - minDistance);

                // percorre a lista de candidatos e adiciona apenas aqueles que estão abaixo do custo
                for (int i = 0; i < listOfCandidates.Count; i++)
                {
                    int posNewPoint = listOfCandidates[i].Pos;
                    double demand = listOfCandidates[i].Demand;
                    double distance = Instance.GetDistance(posLastPoint, posNewPoint);

                    if (distance <= cost && (route.Cost + demand) <= Instance.MaxCapacity)
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
            int iterations = 0, maxIterations = 15;
            
            Random randomGenerator = new Random(DateTime.Now.Millisecond);

            Solution bestSolution = partialSolution.Copy();

            while(iterations < maxIterations)
            {

                for (int i = 0; i < partialSolution.Routes.Count - 1; i++)
                {
                    Solution newSolution = partialSolution.Copy();

                    Route r1 = partialSolution.Routes[i].Copy();
                    Route r2 = partialSolution.Routes[i + 1].Copy();

                    int pos1 = randomGenerator.Next(1, r1.Points.Count - 1);
                    int pos2 = randomGenerator.Next(1, r2.Points.Count - 1);
                    Route[] newRoutes = Exchange(pos1, pos2, r1, r2);

                    newSolution.Routes[i] = newRoutes[0].Copy();
                    newSolution.Routes[i + 1] = newRoutes[1].Copy();
                    newSolution.Recalculate();

                    if (newSolution.TotalDistance < bestSolution.TotalDistance)
                    {
                        Route bestRoute1 = IntraRouteExchange(newRoutes[0]);
                        Route bestRoute2 = IntraRouteExchange(newRoutes[1]);

                        newSolution.Routes[i] = bestRoute1;
                        newSolution.Routes[i + 1] = bestRoute2;
                        newSolution.Recalculate();
                        bestSolution = newSolution.Copy();
                        iterations = 0;
                    }
                    iterations++;
                }
            }
            return bestSolution;
        }

        private Route IntraRouteExchange(Route route)
        {
            Route bestRoute = route.Copy();

            for (int j = 1; j < route.Points.Count - 2; j++)
            {
                Route newRoute = Exchange(j, j + 1, route);
                if (newRoute.CurrDistance < bestRoute.CurrDistance)
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

        private Solution InterRoutesExchange(Solution partialSolution)
        {
            Random randomGenerator = new Random(DateTime.Now.Millisecond);
            Solution bestSolution = partialSolution.Copy();

            for (int i = 0; i < partialSolution.Routes.Count - 1; i++)
            {
                Solution newSolution = partialSolution.Copy();

                Route r1 = partialSolution.Routes[i].Copy();
                Route r2 = partialSolution.Routes[i + 1].Copy();

                int pos1 = randomGenerator.Next(1, r1.Points.Count - 1);
                int pos2 = randomGenerator.Next(1, r2.Points.Count - 1);
                Route[] newRoutes = Exchange(pos1, pos2, r1, r2);

                newSolution.Routes[i] = newRoutes[0].Copy();
                newSolution.Routes[i + 1] = newRoutes[1].Copy();
                newSolution.Recalculate();

                if (newSolution.TotalDistance < bestSolution.TotalDistance)
                {
                    bestSolution = newSolution.Copy();
                }
            }
            return bestSolution;
        }
    }
}
