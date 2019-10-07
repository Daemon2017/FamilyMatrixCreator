﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace FamilyMatrixCreator
{
    public static partial class Form1
    {
        private static Dictionary<int, RelationshipDegree> RelationshipDictionary;
        private static List<int> ExistingRelationshipDegrees;
        private static int[,][] RelationshipsMatrix;
        private static List<int> SiblindantsList;
        private static List<int> AncestorList;
        private static int NumberOfProband;

        public static void Main(string[] args)
        {
            GetStaticData();

            Console.WriteLine("Введите число пар матриц, которое необходимо построить (0;+inf):");
            int numberOfMatrices = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Введите требуемый размер стороны каждой матрицы (0;+inf):");
            int sizeOfMatrices = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Введите MIN % значащих значений каждой матрицы (0;+inf):");
            int minPercentOfValues = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Введите MAX % значащих значений каждой матрицы (0;+inf):");
            int maxPercentOfValues = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Введите % случаев, когда необходимо предпочитать отсутствие родства (0;1):");
            double noRelationPercent = Convert.ToDouble(Console.ReadLine());

            int quantityOfMatrixes = numberOfMatrices;

            Stopwatch myStopwatch = new Stopwatch();
            myStopwatch.Start();

            if (quantityOfMatrixes > 0)
            {
                Console.WriteLine("Все необходимые сведения получены, начинается работа...");
                int generatedMatrixSize = sizeOfMatrices;

                Thread matricesCreator = new Thread(() => CreateMatrices(ExistingRelationshipDegrees, quantityOfMatrixes, generatedMatrixSize,
                    minPercentOfValues, maxPercentOfValues, noRelationPercent));
                matricesCreator.Start();
                matricesCreator.Join();

                myStopwatch.Stop();
                Console.WriteLine("Построение матриц завершено! Затрачено: " + (float)myStopwatch.ElapsedMilliseconds / 1000 + " сек");
            }

            Console.ReadLine();
        }
    }
}
