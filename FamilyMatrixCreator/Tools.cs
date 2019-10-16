using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FamilyMatrixCreator
{
    public static partial class Form1
    {
        private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();
        private static Random random = new Random();

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
            DescendantsList = FileSaverLoader.LoadFromFile1dInt("descendantsMatrix.csv").ToList();
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

            Dictionary<int, int> coordXDictionary = new Dictionary<int, int> { { 0, -1 } };
            for (int i = 0; i < coordXMatrix.Length; i++)
            {
                coordXDictionary.Add(ExistingRelationshipDegrees[i], coordXMatrix[i]);
            }

            Dictionary<int, int> coordYDictionary = new Dictionary<int, int> { { 0, -1 } };
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
                bool isSiblindantOfProband = DescendantsList.Contains(degree);
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

            RelationshipDegreesDictionary = new Dictionary<int, RelationshipDegree>();
            foreach (RelationshipDegree relationship in relationships)
            {
                RelationshipDegreesDictionary.Add(relationship.RelationshipDegreeNumber, relationship);
            }
        }

        private static void CreateMatrices(List<int> existingRelationshipDegrees, int quantityOfMatrixes, int generatedMatrixSize, double noRelationPercent)
        {
            Parallel.For(0, quantityOfMatrixes, matrixNumber =>
            {
                Console.WriteLine("Начинается построение матрицы #{0}...", matrixNumber);
                float[][] generatedOutputMatrix =
                    GetOutputMatrix(generatedMatrixSize, existingRelationshipDegrees, noRelationPercent);
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
        private static float[][] GetOutputMatrix(int generatedMatrixSize, List<int> existingRelationshipDegrees, double probabilityOfNotCreatingNewRelative)
        {
            float[][] generatedOutputMatrix = GetRightTopPartOfOutputMatrix(
                generatedMatrixSize, existingRelationshipDegrees, probabilityOfNotCreatingNewRelative);
            generatedOutputMatrix =
                GetLeftBottomPartOfOutputMatrix(generatedOutputMatrix);

            generatedOutputMatrix = FillMainDiagonalOfOutputMatrix(generatedOutputMatrix);

            return generatedOutputMatrix;
        }

        /*
         * Построение правой (верхней) стороны выходной матрицы.
         */
        public static float[][] GetRightTopPartOfOutputMatrix(int generatedMatrixSize, List<int> existingRelationshipDegrees, double noRelationPercent)
        {
            float[][] relativesMatrix;

            try
            {
                List<Relative> relativesList = GetTree(generatedMatrixSize, existingRelationshipDegrees, noRelationPercent);
                relativesMatrix = GetMatrix(generatedMatrixSize, relativesList);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("{0}\n Осуществляется новая попытка построения правой верхней части матрицы.", e);
                relativesMatrix = GetRightTopPartOfOutputMatrix(generatedMatrixSize, existingRelationshipDegrees, noRelationPercent);
            }

            return relativesMatrix;
        }

        public static float[][] GetMatrix(int generatedMatrixSize, List<Relative> relativesList)
        {
            List<int> relativesNumbersRange = Enumerable.Range(0, relativesList.Count).ToList();

            List<int> randomNumbers = new List<int> { 0 };
            for (int i = 1; i < generatedMatrixSize; i++)
            {
                int randomValue = GetNextRandomValue(1, relativesNumbersRange.Count);
                randomNumbers.Add(randomValue);
                relativesNumbersRange.RemoveAt(randomValue);
            }

            float[][] relativesMatrix = new float[generatedMatrixSize][];
            relativesMatrix[0] = new float[generatedMatrixSize];
            for (int i = 0; i < generatedMatrixSize; i++)
            {
                relativesMatrix[0][i] = relativesList[randomNumbers[i]].RelationshipDegree.RelationshipDegreeNumber;
            }

            for (int i = 1; i < generatedMatrixSize; i++)
            {
                relativesMatrix[i] = new float[generatedMatrixSize];
                Relative zeroRelative = relativesList[randomNumbers[i]];

                for (int j = i + 1; j < generatedMatrixSize; j++)
                {
                    Relative firstRelative = relativesList[randomNumbers[j]];
                    int yMrca = GetYOfMRCA(zeroRelative.RelationshipDegree.X, zeroRelative.RelationshipDegree.Y,
                        firstRelative.RelationshipDegree.X, firstRelative.RelationshipDegree.Y);
                    int y0Result = yMrca - zeroRelative.RelationshipDegree.Y;
                    int y1Result = yMrca - firstRelative.RelationshipDegree.Y;

                    List<RelationshipDegree> possibleRelationshipDegreesList = GetPossibleRelationshipsList(yMrca,
                        y0Result, y1Result,
                        zeroRelative.RelationshipDegree, firstRelative.RelationshipDegree,
                        RelationshipDegreesDictionary.Values.ToList());
                    possibleRelationshipDegreesList = possibleRelationshipDegreesList.Where(x => x.RelationshipDegreeNumber != 1).ToList();
                    RelationshipDegree trueRelationshipDegree = GetTrueRelationshipDegree(zeroRelative, firstRelative, possibleRelationshipDegreesList);

                    relativesMatrix[i][j] = trueRelationshipDegree.RelationshipDegreeNumber;
                }
            }

            return relativesMatrix;
        }

        private static RelationshipDegree GetTrueRelationshipDegree(Relative zeroRelative, Relative firstRelative, List<RelationshipDegree> possibleRelationshipDegreesList)
        {
            RelationshipDegree trueRelationshipDegree = null;

            foreach (RelationshipDegree possibleRelationshipDegree in possibleRelationshipDegreesList)
            {
                if (possibleRelationshipDegree.RelationshipDegreeNumber == 0)
                {
                    trueRelationshipDegree = possibleRelationshipDegree;
                }
                else
                {
                    if (AncestorList.Contains(possibleRelationshipDegree.RelationshipDegreeNumber))
                    {
                        int yDistanceToMRCA = possibleRelationshipDegree.Y;
                        List<Relative> parentsList = GetParentsListInSelectedGeneration(zeroRelative, yDistanceToMRCA);

                        if (parentsList.Contains(firstRelative))
                        {
                            trueRelationshipDegree = possibleRelationshipDegree;
                            break;
                        }
                    }
                    else if (DescendantsList.Contains(possibleRelationshipDegree.RelationshipDegreeNumber))
                    {
                        int yDistanceToMRCA = possibleRelationshipDegree.Y;
                        List<Relative> childsList = GetChildsListInSelectedGeneration(zeroRelative, yDistanceToMRCA);

                        if (childsList.Contains(firstRelative))
                        {
                            trueRelationshipDegree = possibleRelationshipDegree;
                            break;
                        }
                    }
                    else
                    {
                        int yDistanceToMRCAFromZeroPerson = possibleRelationshipDegree.X;
                        List<Relative> parentsListOfZeroPerson = GetParentsListInSelectedGeneration(zeroRelative, yDistanceToMRCAFromZeroPerson);

                        int yDistanceToMRCAFromFirstPerson = possibleRelationshipDegree.X - possibleRelationshipDegree.Y;
                        List<Relative> parentsListOfFirstPerson = GetParentsListInSelectedGeneration(firstRelative, yDistanceToMRCAFromFirstPerson);

                        if (parentsListOfZeroPerson.Intersect(parentsListOfFirstPerson).ToList().Count > 0)
                        {
                            trueRelationshipDegree = possibleRelationshipDegree;
                            break;
                        }
                    }
                }
            }

            return trueRelationshipDegree;
        }

        private static List<Relative> GetChildsListInSelectedGeneration(Relative zeroRelative, int yDistanceToMRCA)
        {
            List<Relative> childsList = new List<Relative> { zeroRelative };

            for (int g = 0; g > yDistanceToMRCA; g--)
            {
                List<Relative> newChildsList = new List<Relative> { };
                foreach (Relative child in childsList)
                {
                    newChildsList.AddRange(child.ChildsList);
                }

                childsList = newChildsList;
            }

            return childsList;
        }

        private static List<Relative> GetParentsListInSelectedGeneration(Relative zeroRelative, int yDistanceToMRCA)
        {
            List<Relative> parentsList = new List<Relative> { zeroRelative };

            for (int g = 0; g < yDistanceToMRCA; g++)
            {
                List<Relative> newParentsList = new List<Relative> { };
                foreach (Relative parent in parentsList)
                {
                    newParentsList.AddRange(parent.ParentsList);
                }

                parentsList = newParentsList;
            }

            return parentsList;
        }

        /*
         * Построение левой (нижней) стороны выходной матрицы.
         */
        public static float[][] GetLeftBottomPartOfOutputMatrix(float[][] generatedOutputMatrix)
        {
            for (int genPerson = 1; genPerson < generatedOutputMatrix.GetLength(0); genPerson++)
            {
                for (int genRelative = 0; genRelative < genPerson; genRelative++)
                {
                    try
                    {
                        generatedOutputMatrix[genPerson][genRelative] =
                            (from genRelationship in Enumerable.Range(0, RelationshipsMatrix.GetLength(1))
                             where RelationshipsMatrix[NumberOfProband, genRelationship][0] ==
                                   generatedOutputMatrix[genRelative][genPerson]
                             select RelationshipsMatrix[genRelationship, NumberOfProband][0]).Single();
                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
            }

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

        /*
         * Построение левой (нижней) стороны входной матрицы.
         */
        public static float[][] GetLeftBottomPartOfInputMatrix(float[][] generatedInputMatrix)
        {
            for (int genPerson = 1; genPerson < generatedInputMatrix.GetLength(0); genPerson++)
            {
                for (int genRelative = 0; genRelative < genPerson; genRelative++)
                {
                    generatedInputMatrix[genPerson][genRelative] = generatedInputMatrix[genRelative][genPerson];
                }
            }

            return generatedInputMatrix;
        }

        /*
         * Подсчет процента значащих значений
         */
        public static double GetPercentOfMeaningfulValues(int generatedMatrixSize, List<int> existingRelationshipDegrees,
            float[][] generatedOutputMatrix)
        {
            int[] quantityOfEachRelationship = GetRelationshipStatistics(generatedOutputMatrix, existingRelationshipDegrees);

            float sumOfMeaningfulValues =
                quantityOfEachRelationship.Aggregate<int, float>(0, (current, quantity) => current + quantity);

            int i = 0;
            foreach (var degree in existingRelationshipDegrees)
            {
                if (0 == degree)
                {
                    sumOfMeaningfulValues -= quantityOfEachRelationship[i];
                    break;
                }

                i++;
            }

            return 100 * (sumOfMeaningfulValues / Math.Pow(generatedMatrixSize, 2));
        }

        /*
         * Поиск всех возможных видов родства.
         */
        public static List<int> GetListOfAllPossibleRelationships(
            List<int> existingRelationshipDegrees,
            float[][] generatedOutputMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative)
        {
            List<int> allPossibleRelationships;
            int[][] ancestorsCurrentCountMatrix = GetAncestorsCurrentCountMatrix(generatedOutputMatrix);

            if (0 == persons[person])
            {
                allPossibleRelationships =
                    GetListOfAllPossibleRelationshipsOfProband(existingRelationshipDegrees);
            }
            else
            {
                allPossibleRelationships = GetListOfAllPossibleRelationshipsOfRelative(generatedOutputMatrix,
                    persons, person,
                    relatives, relative,
                    ancestorsCurrentCountMatrix);
            }

            /*
             * Устранение видов родства из списка допустимых видов родства, 
             * добавление которых приведет к превышению допустимого числа родственников с таким видом родства.
             */
            allPossibleRelationships =
                (from relationship in allPossibleRelationships
                 where !AncestorList.Where((ancestor) =>
                     relationship == ancestor &&
                     ancestorsCurrentCountMatrix[persons[person]][AncestorList.IndexOf(ancestor)] == RelationshipDegreesDictionary[ancestor].RelationshipMaxCount).Any()
                 select relationship).ToList();

            return allPossibleRelationships;
        }

        /*
         * Поиск всех возможных видов родства указанного родственника.
         */
        public static List<int> GetListOfAllPossibleRelationshipsOfRelative(float[][] generatedOutputMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative,
            int[][] ancestorsCurrentCountMatrix)
        {
            List<int> currentPossibleRelationships = new List<int>();

            for (int previousPerson = 0; previousPerson < person; previousPerson++)
            {
                int numberOfI = GetSerialNumberInListOfPossibleRelationships(
                    generatedOutputMatrix, persons,
                    persons, person,
                    previousPerson);

                int numberOfJ = GetSerialNumberInListOfPossibleRelationships(
                    generatedOutputMatrix, persons,
                    relatives, relative,
                    previousPerson);

                List<int> allPossibleRelationships;

                if (0 == persons[previousPerson])
                {
                    currentPossibleRelationships =
                        RelationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToList();
                    allPossibleRelationships =
                        (from j in Enumerable.Range(0, RelationshipsMatrix.GetLength(1))
                         select RelationshipsMatrix[NumberOfProband, j][0]).ToList();
                    allPossibleRelationships.Add(0);
                }
                else
                {
                    if (!(0 == generatedOutputMatrix[persons[previousPerson]][persons[person]] &&
                      0 == generatedOutputMatrix[persons[previousPerson]][relatives[relative]]))
                    {
                        if (-1 != numberOfI && -1 != numberOfJ)
                        {
                            allPossibleRelationships =
                                RelationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToList();

                            bool numberOfAncestorsOfRelativeIsNotZero = IsNumberOfAncestorsNotZero(generatedOutputMatrix,
                            persons, person,
                            relatives, relative);

                            if (numberOfAncestorsOfRelativeIsNotZero)
                            {
                                allPossibleRelationships.AddRange(AncestorList);
                            }
                        }
                        else
                        {
                            allPossibleRelationships = currentPossibleRelationships;
                        }
                    }
                    else
                    {
                        allPossibleRelationships = currentPossibleRelationships;

                        bool personsCountOfRelativesOfThisTypeAlreadyMax =
                            IsCountOfRelativesOfThisTypeAlreadyMax(generatedOutputMatrix,
                                                                          persons, person,
                                                                          ancestorsCurrentCountMatrix);

                        bool relativesCountOfRelativesOfThisTypeAlreadyMax =
                            IsCountOfRelativesOfThisTypeAlreadyMax(generatedOutputMatrix,
                                                                          relatives, relative,
                                                                          ancestorsCurrentCountMatrix);

                        if ((personsCountOfRelativesOfThisTypeAlreadyMax || relativesCountOfRelativesOfThisTypeAlreadyMax) &&
                            ((int)generatedOutputMatrix[0][relatives[relative]] != (int)generatedOutputMatrix[0][persons[person]]))
                        {
                            allPossibleRelationships =
                                allPossibleRelationships.Where(val => val != 0).ToList();
                        }
                    }
                }

                /*
                 * Исключение возможных видов родства, которые невозможно сгенерировать.
                 */
                currentPossibleRelationships =
                    currentPossibleRelationships.Intersect(allPossibleRelationships).ToList();
            }

            return currentPossibleRelationships;
        }

        private static List<Relative> GetTree(int generatedMatrixSize, List<int> existingRelationshipDegrees, double probabilityOfNotCreatingNewRelative)
        {
            List<Relative> relativesList = new List<Relative>
            {
                { new Relative(0, RelationshipDegreesDictionary[1], new List<Relative>(), new List<Relative>()) }
            };
            int relativeNumber = 1;

            List<int> allPossibleRelationships = GetListOfAllPossibleRelationshipsOfProband(existingRelationshipDegrees);

            for (int i = 1; i < generatedMatrixSize; i++)
            {
                RelationshipDegree selectedRandomRelationship = RelationshipDegreesDictionary[allPossibleRelationships[GetNextRandomValue(0, allPossibleRelationships.Count)]];

                int distanceX = selectedRandomRelationship.X - relativesList[0].RelationshipDegree.X;
                int distanceY = selectedRandomRelationship.Y - relativesList[0].RelationshipDegree.Y;

                if (distanceX == 0)
                {
                    if (distanceY > 0)
                    {
                        for (int stepY = 0; stepY < distanceY; stepY++)
                        {
                            relativesList = AddParentalRelationship(relativesList, relativeNumber,
                                0, stepY + 1,
                                stepY == 0, stepY == distanceY - 1,
                                probabilityOfNotCreatingNewRelative);

                            relativeNumber++;
                        }
                    }
                    else
                    {
                        for (int stepY = 0; stepY > distanceY; stepY--)
                        {
                            relativesList = AddDescendantRelationship(relativesList, relativeNumber,
                                0, stepY - 1,
                                stepY == 0, stepY == distanceY + 1,
                                true, probabilityOfNotCreatingNewRelative);

                            relativeNumber++;
                        }
                    }
                }
                else
                {
                    for (int stepY = 0; stepY < distanceX; stepY++)
                    {
                        relativesList = AddParentalRelationship(relativesList, relativeNumber,
                            0, stepY + 1,
                            stepY == 0, 0 != 0,
                            probabilityOfNotCreatingNewRelative);

                        relativeNumber++;
                    }

                    for (int stepY = distanceX; stepY > distanceY; stepY--)
                    {
                        relativesList = AddDescendantRelationship(relativesList, relativeNumber,
                            distanceX, stepY - 1,
                            1 == 0, stepY == distanceY + 1,
                            false, probabilityOfNotCreatingNewRelative);

                        relativeNumber++;
                    }
                }
            }

            return relativesList.Distinct().ToList();
        }

        /*
         * Получить число сМ, эквивалентное указанной степени родства.
         */
        public static float GetCmEquivalentOfRelationshipType(float[][] generatedInputMatrix, int person, int relative,
            int relationship)
        {
            if (RelationshipDegreesDictionary[relationship].CommonCm > 3950)
            {
                return generatedInputMatrix[person][relative] = RelationshipDegreesDictionary[relationship].CommonCm;
            }

            if (0 != relationship)
            {
                float mean = RelationshipDegreesDictionary[relationship].CommonCm;
                double std = mean * ((-0.2819 * Math.Log(mean)) + 2.335) / 3;
                Normal normalDist = new Normal(mean, std);

                float normalyDistributedValue = (float)normalDist.Sample();

                if (normalyDistributedValue < 0)
                {
                    normalyDistributedValue = 0;
                }

                return normalyDistributedValue;
            }
            else
            {
                return 0;
            }
        }

        /*
         * Сбор статистики по родству.
         */
        public static int[] GetRelationshipStatistics(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees)
        {
            int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count];

            for (int probandsRelatioship = 0;
                probandsRelatioship < existingRelationshipDegrees.Count;
                probandsRelatioship++)
            {
                quantityOfEachRelationship[probandsRelatioship] += (from raw in generatedOutputMatrix
                                                                    from column in raw
                                                                    where column == existingRelationshipDegrees[probandsRelatioship]
                                                                    select column).Count();
            }

            return quantityOfEachRelationship;
        }

        /*
         * Построение списка возможных степеней родства пробанда.
         */
        public static List<int> GetListOfAllPossibleRelationshipsOfProband(List<int> existingRelationshipDegrees)
        {
            existingRelationshipDegrees = existingRelationshipDegrees.Where(var => var != 1).ToList();

            List<int> allPossibleRelationshipsOfProband = new List<int>();

            for (int i = 1; i < RelationshipsMatrix.GetLength(0); i++)
            {
                int numberOfPossibleRelationships = 0;

                for (int j = 1; j < RelationshipsMatrix.GetLength(1); j++)
                {
                    for (int k = 0; k < RelationshipsMatrix[i, j].Length; k++)
                    {
                        if (existingRelationshipDegrees.Contains(RelationshipsMatrix[i, j][k]))
                        {
                            numberOfPossibleRelationships++;
                            break;
                        }
                    }
                }

                if (numberOfPossibleRelationships > 0.5 * existingRelationshipDegrees.Count)
                {
                    allPossibleRelationshipsOfProband.Add(RelationshipsMatrix[NumberOfProband, i][0]);
                }
            }

            return allPossibleRelationshipsOfProband;
        }

        public static int[][] GetAncestorsCurrentCountMatrix(float[][] generatedOutputMatrix)
        {
            int[][] ancestorsCurrentCountMatrix = new int[generatedOutputMatrix.Length][];

            for (int row = 0; row < generatedOutputMatrix.Length; row++)
            {
                ancestorsCurrentCountMatrix[row] = new int[AncestorList.Count];

                if (null != generatedOutputMatrix[row])
                {
                    for (int column = row + 1; column < generatedOutputMatrix[0].Length; column++)
                    {
                        for (int i = 0; i < AncestorList.Count; i++)
                        {
                            if (generatedOutputMatrix[row][column] == AncestorList[i])
                            {
                                ancestorsCurrentCountMatrix[row][i]++;

                                break;
                            }
                        }
                    }
                }
            }

            return ancestorsCurrentCountMatrix;
        }

        /*
         * Нахождение всех существующих степеней родства.
         */
        public static List<int> GetAllExistingRelationshipDegrees()
        {
            List<int> existingRelationshipDegrees = new List<int>();

            existingRelationshipDegrees.AddRange((from i in Enumerable.Range(0, RelationshipsMatrix.GetLength(0))
                                                  select RelationshipsMatrix[NumberOfProband, i][0]).ToList());

            return existingRelationshipDegrees.Distinct().ToList();
        }

        /*
         * Заполнение главной диагонали выходной матрицы.
         */
        public static float[][] FillMainDiagonalOfOutputMatrix(float[][] generatedOutputMatrix)
        {
            for (int i = 0; i < generatedOutputMatrix.GetLength(0); i++)
            {
                generatedOutputMatrix[i][i] = 1;
            }

            return generatedOutputMatrix;
        }

        /*
         * Создание случайного значения.
         */
        public static int GetNextRandomValue(int min, int max)
        {
            byte[] rndBytes = new byte[4];
            Rng.GetBytes(rndBytes);

            return (int)((BitConverter.ToInt32(rndBytes, 0) - (decimal)int.MinValue) /
                          (int.MaxValue - (decimal)int.MinValue) * (max - min) + min);
        }

        /*
         * Проверка того, что число данных степеней родства у данного лица уже максимально.
         */
        public static bool IsCountOfRelativesOfThisTypeAlreadyMax(float[][] generatedOutputMatrix,
            List<int> relatives, int relative,
            int[][] ancestorsCurrentCountMatrix)
        {
            bool countOfRelativesOfThisTypeAlreadyMax = false;

            if (AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]))
            {
                countOfRelativesOfThisTypeAlreadyMax =
                    (from i in Enumerable.Range(0, AncestorList.Count)
                     where AncestorList[i] == (int)generatedOutputMatrix[0][relatives[relative]]
                     where RelationshipDegreesDictionary[AncestorList[i]].RelationshipMaxCount == ancestorsCurrentCountMatrix[0][i]
                     select i).Any();
            }

            return countOfRelativesOfThisTypeAlreadyMax;
        }

        /*
         * Среди возможных видов родства пробанда ищутся порядковые номера тех, что содержат выбранные виды родства.
         */
        public static int GetSerialNumberInListOfPossibleRelationships(float[][] generatedOutputMatrix, List<int> persons,
            List<int> relatives, int relative,
            int previousPerson)
        {
            int numberOfJ = -1;

            try
            {
                numberOfJ =
                    (from number in Enumerable.Range(0, RelationshipsMatrix.GetLength(1))
                     where RelationshipsMatrix[NumberOfProband, number][0] ==
                           generatedOutputMatrix[persons[previousPerson]][relatives[relative]]
                     select number).Single();
            }
            catch (InvalidOperationException)
            {

            }

            return numberOfJ;
        }

        /*
         * Проверка того, что число предков исследуемого лица не равно нулю.
         */
        public static bool IsNumberOfAncestorsNotZero(float[][] generatedOutputMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative)
        {
            bool numberOfAncestorsNotZero = false;

            numberOfAncestorsNotZero = (from prevPerson in Enumerable.Range(1, person - 1)
                                        where AncestorList.Contains((int)generatedOutputMatrix[persons[prevPerson]][relatives[relative]]) &&
                                              0 != generatedOutputMatrix[persons[prevPerson]][persons[person]]
                                        select prevPerson).Any();

            return numberOfAncestorsNotZero;
        }

        public static List<Relative> AddParentalRelationship(List<Relative> relativesList, int relativeNumber,
            int coordX, int coordY,
            bool isFirstIteration, bool isLastIteration,
            double probabilityOfNotCreatingNewRelative)
        {
            if (isFirstIteration)
            {
                if (relativesList[0].ParentsList.Count != 0 && random.NextDouble() <= probabilityOfNotCreatingNewRelative)
                {
                    relativesList.Add(relativesList[0].ParentsList[GetNextRandomValue(0, relativesList[0].ParentsList.Count)]);
                }
                else
                {
                    relativesList.Add(new Relative(
                        relativeNumber,
                        RelationshipDegreesDictionary[RelationshipDegreesDictionary.First(x => x.Value.X == coordX && x.Value.Y == coordY).Key],
                        new List<Relative>(),
                        new List<Relative>()));

                    relativesList[0].ParentsList.Add(relativesList[relativesList.Count - 1]);
                    relativesList[relativesList.Count - 1].ChildsList.Add(relativesList[0]);
                }
            }
            else
            {
                if (relativesList[relativesList.Count - 1].ParentsList.Count != 0 && random.NextDouble() <= probabilityOfNotCreatingNewRelative && !isLastIteration)
                {
                    relativesList.Add(relativesList[relativesList.Count - 1].ParentsList[GetNextRandomValue(0, relativesList[relativesList.Count - 1].ParentsList.Count)]);
                }
                else
                {
                    relativesList.Add(new Relative(
                        relativeNumber,
                        RelationshipDegreesDictionary[RelationshipDegreesDictionary.First(x => x.Value.X == coordX && x.Value.Y == coordY).Key],
                        new List<Relative>(),
                        new List<Relative>()));

                    relativesList[relativesList.Count - 2].ParentsList.Add(relativesList[relativesList.Count - 1]);
                    relativesList[relativesList.Count - 1].ChildsList.Add(relativesList[relativesList.Count - 2]);
                }
            }

            return relativesList;
        }

        private static List<Relative> AddDescendantRelationship(List<Relative> relativesList, int relativeNumber,
            int coordX, int coordY,
            bool isFirstIteration, bool isLastIteration,
            bool descendantOfProband, double probabilityOfNotCreatingNewRelative)
        {
            if (isFirstIteration)
            {
                if (relativesList[0].ChildsList.Count != 0 && random.NextDouble() <= probabilityOfNotCreatingNewRelative)
                {
                    relativesList.Add(relativesList[0].ChildsList[GetNextRandomValue(0, relativesList[0].ChildsList.Count)]);
                }
                else
                {
                    relativesList.Add(new Relative(
                        relativeNumber,
                        RelationshipDegreesDictionary[RelationshipDegreesDictionary.First(x => x.Value.X == coordX && x.Value.Y == coordY).Key],
                        new List<Relative>(),
                        new List<Relative>()));

                    relativesList[relativesList.Count - 1].ParentsList.Add(relativesList[0]);
                    relativesList[0].ChildsList.Add(relativesList[relativesList.Count - 1]);
                }
            }
            else
            {
                List<Relative> cleanChildsList = relativesList[relativesList.Count - 1].ChildsList.Where(child => child.RelationshipDegree.X != 0).ToList();

                if (((descendantOfProband && relativesList[relativesList.Count - 1].ChildsList.Count != 0) || (!descendantOfProband && cleanChildsList.Count != 0))
                     && random.NextDouble() <= probabilityOfNotCreatingNewRelative && !isLastIteration)
                {
                    if (descendantOfProband)
                    {
                        relativesList.Add(relativesList[relativesList.Count - 1].ChildsList[GetNextRandomValue(0, relativesList[relativesList.Count - 1].ChildsList.Count)]);
                    }
                    else
                    {
                        relativesList.Add(cleanChildsList[GetNextRandomValue(0, cleanChildsList.Count)]);
                    }
                }
                else
                {
                    relativesList.Add(new Relative(
                        relativeNumber,
                        RelationshipDegreesDictionary[RelationshipDegreesDictionary.First(x => x.Value.X == coordX && x.Value.Y == coordY).Key],
                        new List<Relative>(),
                        new List<Relative>()));

                    relativesList[relativesList.Count - 1].ParentsList.Add(relativesList[relativesList.Count - 2]);
                    relativesList[relativesList.Count - 2].ChildsList.Add(relativesList[relativesList.Count - 1]);
                }
            }

            return relativesList;
        }
    }
}