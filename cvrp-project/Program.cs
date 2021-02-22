﻿using System;
using cvrp_project.Entities;

namespace cvrp_project
{
    class Program
    {
        static void Main(string[] args)
        {
            string set = "A-n61-k9";
            string path = $"instances\\Vrp-Set-A\\A\\{set}.vrp";
            
            path = "myInstance.vrp";
            CvrpInstance instance = new CvrpInstance();
            instance.ReadInstance(path);

            Solution bestSolution = new Grasp().ExecuteGrasp(instance, 10000, 0.05);
            bestSolution.SaveSolution("solution.txt");
            bestSolution.GerarHTML(instance, "index.html");
            Console.ReadKey();
        }
    }
}
