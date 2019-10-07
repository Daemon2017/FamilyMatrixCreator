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
    public static partial class Form1
    {
        private static Dictionary<int, RelationshipDegree> RelationshipDictionary;
        private static List<int> ExistingRelationshipDegrees;
        private static int[,][] RelationshipsMatrix;
        private static List<int> SiblindantsList;
        private static List<int> AncestorList;
        private static int NumberOfProband;

        private static Random random = new Random();

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

        public static void GetStaticData()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            Console.WriteLine("Подготовка необходимых файлов...");
            RelationshipsMatrix = FileSaverLoader.LoadFromFile2dJagged("relationships.csv");
            NumberOfProband = 0;
            float[] centimorgansMatrix = FileSaverLoader.LoadFromFile1dFloat("centimorgans.csv");
            int[] coordXMatrix = Array.ConvertAll(FileSaverLoader.LoadFromFile1dFloat("xs.csv"), x => (int)x);
            int[] coordYMatrix = Array.ConvertAll(FileSaverLoader.LoadFromFile1dFloat("ys.csv"), y => (int)y);
            int[][] _ancestorsMaxCountMatrix = FileSaverLoader.LoadFromFile2dInt("ancestorsMatrix.csv");
            int[] siblindantsMatrix = FileSaverLoader.LoadFromFile1dInt("siblindantsMatrix.csv");
            SiblindantsList = siblindantsMatrix.ToList();
            Console.WriteLine("Необходимые файлы успешно подготовлены!");

            AncestorList = Enumerable
                .Range(0, _ancestorsMaxCountMatrix.GetLength(0))
                .Select(x => _ancestorsMaxCountMatrix[x][0])
                .ToList();

            Dictionary<int, int> AncestorsMaxCountDictionary = new Dictionary<int, int>();
            for (int i = 0; i < _ancestorsMaxCountMatrix.GetLength(0); i++)
            {
                AncestorsMaxCountDictionary.Add(_ancestorsMaxCountMatrix[i][0], _ancestorsMaxCountMatrix[i][1]);
            }

            ExistingRelationshipDegrees =
                GetAllExistingRelationshipDegrees();

            Dictionary<int, float> _centimorgansDictionary = new Dictionary<int, float> { { 0, 0 } };
            for (int i = 0; i < centimorgansMatrix.Length; i++)
            {
                _centimorgansDictionary.Add(ExistingRelationshipDegrees[i], centimorgansMatrix[i]);
            }

            Dictionary<int, int> coordXDictionary = new Dictionary<int, int> { { 0, 0 } };
            for (int i = 0; i < coordXMatrix.Length; i++)
            {
                coordXDictionary.Add(ExistingRelationshipDegrees[i], coordXMatrix[i]);
            }

            Dictionary<int, int> coordYDictionary = new Dictionary<int, int> { { 0, 0 } };
            for (int i = 0; i < coordYMatrix.Length; i++)
            {
                coordYDictionary.Add(ExistingRelationshipDegrees[i], coordYMatrix[i]);
            }

            ExistingRelationshipDegrees.Insert(0, 0);

            List<RelationshipDegree> relationships = new List<RelationshipDegree>();
            foreach (int degree in ExistingRelationshipDegrees)
            {
                int relationshipNumber = degree;
                float commonCm = _centimorgansDictionary[degree];
                int coordX = coordXDictionary[degree];
                int coordY = coordYDictionary[degree];
                bool isAncestorOfProband = AncestorList.Contains(degree);
                bool isSiblindantOfProband = SiblindantsList.Contains(degree);
                int relationshipMaxCount;
                if (AncestorsMaxCountDictionary.ContainsKey(degree))
                {
                    relationshipMaxCount = AncestorsMaxCountDictionary[degree];
                }
                else
                {
                    relationshipMaxCount = int.MaxValue;
                }

                RelationshipDegree rel = new RelationshipDegree(
                    relationshipNumber,
                    commonCm,
                    coordX,
                    coordY,
                    isAncestorOfProband,
                    isSiblindantOfProband,
                    relationshipMaxCount);
                relationships.Add(rel);
            }

            RelationshipDictionary = new Dictionary<int, RelationshipDegree>();
            foreach (RelationshipDegree relationship in relationships)
            {
                RelationshipDictionary.Add(relationship.RelationshipNumber, relationship);
            }
        }

        private static void CreateMatrices(List<int> existingRelationshipDegrees, int quantityOfMatrixes, int generatedMatrixSize,
            int minPercentOfValues, int maxPercentOfValues, double noRelationPercent)
        {
            Parallel.For(0, quantityOfMatrixes, matrixNumber =>
            {
                Console.WriteLine("Начинается построение матрицы #{0}...", matrixNumber);
                float[][] generatedOutputMatrix =
                    GetOutputMatrix(generatedMatrixSize, existingRelationshipDegrees,
                    minPercentOfValues, maxPercentOfValues, noRelationPercent);
                float[][] generatedInputMatrix =
                    GetInputMatrix(generatedOutputMatrix, generatedMatrixSize);
                Console.WriteLine("Завершено построение матрицы #{0}!", matrixNumber);

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
        private static float[][] GetOutputMatrix(int generatedMatrixSize, List<int> existingRelationshipDegrees,
            int minPercentOfValues, int maxPercentOfValues, double noRelationPercent)
        {
            float[][] generatedOutputMatrix = GetRightTopPartOfOutputMatrix(
                generatedMatrixSize, existingRelationshipDegrees,
                minPercentOfValues, maxPercentOfValues, noRelationPercent);
            generatedOutputMatrix =
                GetLeftBottomPartOfOutputMatrix(generatedOutputMatrix);

            generatedOutputMatrix = FillMainDiagonalOfOutputMatrix(generatedOutputMatrix);

            return generatedOutputMatrix;
        }

        /*
         * Построение правой (верхней) стороны выходной матрицы.
         */
        public static float[][] GetRightTopPartOfOutputMatrix(int generatedMatrixSize, List<int> existingRelationshipDegrees,
            int minPercent, int maxPercent, double noRelationPercent)
        {
            List<Relative> relativesList = GetTree(generatedMatrixSize, existingRelationshipDegrees);
            List<int> relativesNumbersRange = Enumerable.Range(0, relativesList.Count).ToList();

            List<int> randomNumbers = new List<int> { 0 };
            for (int i = 1; i < generatedMatrixSize; i++)
            {
                int randomValue = GetNextRandomValue(1, relativesNumbersRange.Count);
                randomNumbers.Add(randomValue);
                relativesNumbersRange.RemoveAt(randomValue);
            }

            float[][] generatedOutputMatrix = new float[generatedMatrixSize][];

            //List<int> persons = (from x in Enumerable.Range(1, generatedOutputMatrix.GetLength(0) - 1)
            //                     orderby new ContinuousUniform().Sample()
            //                     select x).ToList();
            //persons.Insert(0, 0);

            //for (int person = 0; person < persons.Count; person++)
            //{
            //    generatedOutputMatrix[persons[person]] = new float[generatedOutputMatrix.GetLength(0)];

            //    List<int> relatives = (from x in Enumerable.Range(persons[person] + 1,
            //                           generatedOutputMatrix.GetLength(0) - (persons[person] + 1))
            //                           orderby new ContinuousUniform().Sample()
            //                           select x).ToList();

            //    for (int relative = 0; relative < relatives.Count; relative++)
            //    {
            //        List<int> allPossibleRelationships = GetListOfAllPossibleRelationships(
            //            existingRelationshipDegrees,
            //            generatedOutputMatrix,
            //            persons, person,
            //            relatives, relative);
            //    }

            //    /*
            //     * Проверка того, что выполняется требование по проценту значащих значений
            //     */
            //    if (generatedOutputMatrix.GetLength(0) - 1 == person)
            //    {
            //        double percentOfMeaningfulValues = 2 * GetPercentOfMeaningfulValues(
            //                                               generatedMatrixSize,
            //                                               existingRelationshipDegrees, generatedOutputMatrix);

            //        if (percentOfMeaningfulValues < minPercent || percentOfMeaningfulValues > maxPercent)
            //        {
            //            if (percentOfMeaningfulValues < minPercent)
            //            {
            //                Console.WriteLine("[ОШИБКА] Процент значащих значений у полученной матрицы ниже заданного! " + percentOfMeaningfulValues);
            //            }
            //            else if (percentOfMeaningfulValues > maxPercent)
            //            {
            //                Console.WriteLine("[ОШИБКА] Процент значащих значений у полученной матрицы выше заданного! " + percentOfMeaningfulValues);
            //            }

            //            generatedOutputMatrix = new float[generatedMatrixSize][];

            //            persons = (from x in Enumerable.Range(1, generatedOutputMatrix.GetLength(0) - 1)
            //                       orderby new ContinuousUniform().Sample()
            //                       select x).ToList();
            //            persons.Insert(0, 0);

            //            person = -1;
            //        }
            //    }
            //}

            return generatedOutputMatrix;
        }

        /*
         * Построение входной матрицы (матрицы сМ).
         */
        private static float[][] GetInputMatrix(float[][] generatedOutputMatrix, int generatedMatrixSize)
        {
            float[][] generatedInputMatrix = new float[generatedMatrixSize][];

            generatedInputMatrix = GetRightTopPartOfInputMatrix(generatedOutputMatrix,
                generatedInputMatrix);
            generatedInputMatrix = GetLeftBottomPartOfInputMatrix(generatedInputMatrix);

            return generatedInputMatrix;
        }

        /*
         * Построение правой (верхней) стороны входной матрицы.
         */
        public static float[][] GetRightTopPartOfInputMatrix(float[][] generatedOutputMatrix,
            float[][] generatedInputMatrix)
        {
            for (int person = 0; person < generatedOutputMatrix.GetLength(0); person++)
            {
                generatedInputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];

                for (int relative = person; relative < generatedOutputMatrix.GetLength(0); relative++)
                {
                    for (int relationship = 0; relationship < RelationshipsMatrix.GetLength(1); relationship++)
                    {
                        if (RelationshipsMatrix[NumberOfProband, relationship][0] ==
                            generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] =
                                GetCmEquivalentOfRelationshipType(generatedInputMatrix, person, relative,
                                    relationship);
                        }

                        if (RelationshipsMatrix[relationship, NumberOfProband][0] ==
                            generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] =
                                GetCmEquivalentOfRelationshipType(generatedInputMatrix, person, relative,
                                    relationship);
                        }
                    }
                }
            }

            return generatedInputMatrix;
        }
    }
}
