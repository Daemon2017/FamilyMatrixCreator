using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FamilyMatrixCreator
{
    public class Form1
    {
        private static int[,][] _relationshipsMatrix;
        private static float[] _centimorgansMatrix;
        private static int[][] _ancestorsMaxCountMatrix;
        private static int[][] _descendantsMatrix;
        private static int _numberOfProband;
        private static int[][] quantityOfEachRelationship;

        public static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            Console.WriteLine("Подготовка необходимых файлов...");
            _relationshipsMatrix = FileSaverLoader.LoadFromFile2DJagged("relationships.csv");
            _numberOfProband = Modules.FindNumberOfProband(_relationshipsMatrix);
            _centimorgansMatrix = FileSaverLoader.LoadFromFile1D("centimorgans.csv");
            _ancestorsMaxCountMatrix = FileSaverLoader.LoadFromFile2D("ancestorsMatrix.csv");
            _descendantsMatrix = FileSaverLoader.LoadFromFile2D("descendantsMatrix.csv");
            Console.WriteLine("Необходимые файлы успешно подготовлены!");

            Console.WriteLine("Введите число пар матриц, которое необходимо построить:");
            int numberOfMatrices = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Введите требуемый размер стороны каждой матрицы:");
            int sizeOfMatrices = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Введите MIN % значащих значений каждой матрицы:");
            int minPercentOfValues = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Введите MAX % значащих значений каждой матрицы:");
            int maxPercentOfValues = Convert.ToInt32(Console.ReadLine());

            List<int> existingRelationshipDegrees =
                Modules.FindAllExistingRelationshipDegrees(_relationshipsMatrix, _numberOfProband);

            int quantityOfMatrixes = numberOfMatrices;

            Stopwatch myStopwatch = new Stopwatch();
            myStopwatch.Start();

            if (quantityOfMatrixes > 0)
            {
                Console.WriteLine("Все необходимые сведения получены, начинается работа...");
                int generatedMatrixSize = sizeOfMatrices;
                quantityOfEachRelationship = new int[quantityOfMatrixes][];

                Thread matricesCreator = new Thread(() => CreateMatrices(existingRelationshipDegrees, quantityOfMatrixes, generatedMatrixSize,
                    minPercentOfValues, maxPercentOfValues));
                matricesCreator.Start();
                matricesCreator.Join();

                myStopwatch.Stop();
                Console.WriteLine("Построение матриц завершено! Затрачено: " + (float)myStopwatch.ElapsedMilliseconds / 1000 + " сек");

                GetStatisticReport(existingRelationshipDegrees, quantityOfMatrixes, generatedMatrixSize);
            }

            Console.ReadLine();
        }

        private static void GetStatisticReport(List<int> existingRelationshipDegrees, int quantityOfMatrixes, int generatedMatrixSize)
        {
            /*
             * Подсчет общей статистики по родству.
             */
            int[] sumQuantityOfEachRelationship = new int[existingRelationshipDegrees.Count];

            for (int i = 0; i < quantityOfMatrixes; i++)
            {
                for (int j = 0; j < existingRelationshipDegrees.Count; j++)
                {
                    sumQuantityOfEachRelationship[j] += quantityOfEachRelationship[i][j];
                }
            }

            /*
             * Запись общей статистики по родству.
             */
            int relationshipNumber = 0;
            float sumOfMeaningfulValues = 0;
            string statistic = "";

            foreach (var relationship in sumQuantityOfEachRelationship)
            {
                statistic += "Родство " + existingRelationshipDegrees[relationshipNumber] + ": " + relationship +
                                 Environment.NewLine;
                sumOfMeaningfulValues += relationship;

                relationshipNumber++;
            }

            FileSaverLoader.SaveToFile(@"statistic.csv", statistic);

            foreach (var row in quantityOfEachRelationship)
            {
                sumOfMeaningfulValues -= row[0];
            }

            Console.WriteLine("Значащих значений: "
                          + 100 * ((sumOfMeaningfulValues - quantityOfMatrixes * generatedMatrixSize) /
                                   (quantityOfMatrixes * Math.Pow(generatedMatrixSize, 2))) + "%");
        }

        private static void CreateMatrices(List<int> existingRelationshipDegrees, int quantityOfMatrixes, int generatedMatrixSize,
            int minPercentOfValues, int maxPercentOfValues)
        {
            Parallel.For(0, quantityOfMatrixes, matrixNumber =>
            {
                Console.WriteLine("Начинается построение матрицы #{0}...", matrixNumber);
                float[][] generatedOutputMatrix =
                    GenerateOutputMatrix(generatedMatrixSize, existingRelationshipDegrees,
                    minPercentOfValues, maxPercentOfValues);
                float[][] generatedInputMatrix =
                    GenerateInputMatrix(generatedOutputMatrix, generatedMatrixSize);
                Console.WriteLine("Завершено построение матрицы #{0}!", matrixNumber);

                quantityOfEachRelationship[matrixNumber] = Modules.CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees);

                /*
                 * Сохранение входной матрицы в файл.
                 */
                Directory.CreateDirectory("input");
                FileSaverLoader.SaveToFile(@"input\generated_input", generatedInputMatrix, matrixNumber);

                /*
                 * Сохранение выходной матрицы в файл.
                 */
                Directory.CreateDirectory("output");
                FileSaverLoader.SaveToFile(@"output\generated_output", generatedOutputMatrix, matrixNumber);
            });
        }

        /*
         * Построение выходной матрицы (матрицы родственных отношений).
         */
        private static float[][] GenerateOutputMatrix(int generatedMatrixSize, List<int> existingRelationshipDegrees,
            int minPercentOfValues, int maxPercentOfValues)
        {
            float[][] generatedOutputMatrix = OutputBuildRightTopPart(_relationshipsMatrix,
                _numberOfProband, generatedMatrixSize, existingRelationshipDegrees,
                _ancestorsMaxCountMatrix, _descendantsMatrix,
                minPercentOfValues, maxPercentOfValues);
            generatedOutputMatrix =
                Modules.OutputBuildLeftBottomPart(generatedOutputMatrix, _relationshipsMatrix, _numberOfProband);

            generatedOutputMatrix = Modules.FillMainDiagonal(generatedOutputMatrix);

            return generatedOutputMatrix;
        }

        /*
         * Построение правой (верхней) стороны.
         */
        public static float[][] OutputBuildRightTopPart(int[,][] relationshipsMatrix, int numberOfProband,
            int generatedMatrixSize, List<int> existingRelationshipDegrees,
            int[][] ancestorsMaxCountMatrix, int[][] descendantsMatrix,
            int minPercent, int maxPercent)
        {
            float[][] generatedOutputMatrix = new float[generatedMatrixSize][];
            int[][] currentCountMatrix = new int[generatedMatrixSize][];

            List<int> persons = (from x in Enumerable.Range(1, generatedOutputMatrix.GetLength(0) - 1)
                                 orderby new ContinuousUniform().Sample()
                                 select x).ToList();
            persons.Insert(0, 0);

            for (int person = 0; person < persons.Count; person++)
            {
                generatedOutputMatrix[persons[person]] = new float[generatedOutputMatrix.GetLength(0)];
                currentCountMatrix[persons[person]] = new int[ancestorsMaxCountMatrix.Length];

                List<int> relatives = (from x in Enumerable.Range(persons[person] + 1,
                        generatedOutputMatrix.GetLength(0) - (persons[person] + 1))
                                       orderby new ContinuousUniform().Sample()
                                       select x).ToList();

                for (int relative = 0; relative < relatives.Count; relative++)
                {
                    List<int> allPossibleRelationships = Integrations.DetectAllPossibleRelationships(
                        relationshipsMatrix, numberOfProband,
                        ancestorsMaxCountMatrix, descendantsMatrix,
                        generatedOutputMatrix, currentCountMatrix,
                        persons, person,
                        relatives, relative);

                    /*
                     * Создание родственника со случайным видом родства.
                     */
                    try
                    {
                        generatedOutputMatrix[persons[person]][relatives[relative]] =
                            allPossibleRelationships[Modules.GetNextRnd(0, allPossibleRelationships.Count)];
                        currentCountMatrix = Modules.IncreaseCurrentRelationshipCount(generatedOutputMatrix,
                            currentCountMatrix, persons, person, relatives, relative, ancestorsMaxCountMatrix);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        generatedOutputMatrix = new float[generatedMatrixSize][];
                        currentCountMatrix = new int[generatedMatrixSize][];

                        persons = (from x in Enumerable.Range(1, generatedOutputMatrix.GetLength(0) - 1)
                                   orderby new ContinuousUniform().Sample()
                                   select x).ToList();
                        persons.Insert(0, 0);

                        person = -1;

                        break;
                    }
                }

                /*
                 * Проверка того, что выполняется требование по проценту значащих значений
                 */
                if (generatedOutputMatrix.GetLength(0) - 1 == person)
                {
                    double percentOfMeaningfulValues = 2 * Integrations.CalculatePercentOfMeaningfulValues(
                                                           generatedMatrixSize,
                                                           existingRelationshipDegrees, generatedOutputMatrix);

                    if (percentOfMeaningfulValues < minPercent || percentOfMeaningfulValues > maxPercent)
                    {
                        generatedOutputMatrix = new float[generatedMatrixSize][];
                        currentCountMatrix = new int[generatedMatrixSize][];

                        persons = (from x in Enumerable.Range(1, generatedOutputMatrix.GetLength(0) - 1)
                                   orderby new ContinuousUniform().Sample()
                                   select x).ToList();
                        persons.Insert(0, 0);

                        person = -1;
                    }
                }
            }

            return generatedOutputMatrix;
        }

        /*
         * Построение входной матрицы (матрицы сМ).
         */
        private static float[][] GenerateInputMatrix(float[][] generatedOutputMatrix, int generatedMatrixSize)
        {
            float[][] generatedInputMatrix = new float[generatedMatrixSize][];

            generatedInputMatrix = InputBuildRightTopPart(generatedOutputMatrix, _relationshipsMatrix,
                _numberOfProband, generatedInputMatrix, _centimorgansMatrix);
            generatedInputMatrix = Modules.InputBuildLeftBottomPart(generatedInputMatrix);

            return generatedInputMatrix;
        }

        /*
         * Построение правой (верхней) стороны  (сМ).
         */
        public static float[][] InputBuildRightTopPart(float[][] generatedOutputMatrix, int[,][] relationshipsMatrix,
            int numberOfProband, float[][] generatedInputMatrix, float[] centimorgansMatrix)
        {
            for (int person = 0; person < generatedOutputMatrix.GetLength(0); person++)
            {
                generatedInputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];

                for (int relative = person; relative < generatedOutputMatrix.GetLength(0); relative++)
                {
                    for (int relationship = 0; relationship < relationshipsMatrix.GetLength(1); relationship++)
                    {
                        if (relationshipsMatrix[numberOfProband, relationship][0] ==
                            generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] =
                                Modules.TransformRelationshipTypeToCm(generatedInputMatrix, person, relative,
                                    relationship, centimorgansMatrix);
                        }

                        if (relationshipsMatrix[relationship, numberOfProband][0] ==
                            generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] =
                                Modules.TransformRelationshipTypeToCm(generatedInputMatrix, person, relative,
                                    relationship, centimorgansMatrix);
                        }
                    }
                }
            }

            return generatedInputMatrix;
        }
    }
}
