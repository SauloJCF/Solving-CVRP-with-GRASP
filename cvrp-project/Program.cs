﻿using cvrp_project.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cvrp_project
{
    class Program
    {
        private static CvrpInstance GetInstance(string filePath)
        {
            string line = "";
            string[] splitted;
            StreamReader file = new StreamReader(filePath);
            CvrpInstance cvrp = new CvrpInstance();

            // Leitura do cabeçalho
            for (int i = 0; i < 4; i++)// Pula três linhas até chegar na quarta, onde está a dimensão
                line = file.ReadLine();

            splitted = line.Split(" ");
            cvrp.Dimension = int.Parse(splitted[splitted.Length - 1]);

            for (int i = 0; i < 2; i++)// Pula uma e pega a capacidade
                line = file.ReadLine();

            splitted = line.Split(" ");
            cvrp.MaxCapacity = double.Parse(splitted[splitted.Length - 1]);

            file.ReadLine();// Pula o título    

            for (int i = 0; i < cvrp.Dimension; i++)// Leitura das coordenadas
            {
                line = file.ReadLine();
                splitted = line.Split(" ");
                Point p = new Point();
                p.Id = int.Parse(splitted[1]);// índice
                p.X = double.Parse(splitted[2]);// cx
                p.Y = double.Parse(splitted[3]);// cy
                cvrp.AddPoint(p);
            }

            file.ReadLine();// Pula o título    

            for (int i = 0; i < cvrp.Dimension; i++)// Leitura das demandas
            {
                line = file.ReadLine();
                splitted = line.Split(" ");
                double demand = double.Parse(splitted[1]);// demanda
                cvrp.SetDemand(i, demand);
            }

            file.ReadLine();// Pula o título    

            cvrp.Depot = int.Parse(file.ReadLine().Replace(" ", ""));

            file.Close();

            return cvrp;
        }

        static void Main(string[] args)
        {

            string test = @"instances\Vrp-Set-A\A\A-n32-k5.vrp";
            // test = "myInstance.vrp";
            CvrpInstance instance = GetInstance(test);
            instance.CalculateDistances();
            new Grasp(instance).ExecuteGrasp(1000);

            // Console.WriteLine(instance.ToString());
            Console.ReadKey();
        }
    }
}
