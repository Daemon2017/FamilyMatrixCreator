using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyMatrixCreator
{
    public static partial class Form1
    {
        /*
         * Подсчет процента значащих значений
         */
        public static double CalculatePercentOfMeaningfulValues(int generatedMatrixSize, List<int> existingRelationshipDegrees,
            float[][] generatedOutputMatrix)
        {
            int[] quantityOfEachRelationship = CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees);

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

        public static List<int> DetectAllPossibleRelationships(
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
                    GetAllPossibleRelationshipsOfProband(existingRelationshipDegrees);
            }
            else
            {
                allPossibleRelationships = FindAllPossibleRelationships(generatedOutputMatrix,
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
                     ancestorsCurrentCountMatrix[persons[person]][AncestorList.IndexOf(ancestor)] == RelationshipDictionary[ancestor].RelationshipMaxCount).Any()
                 select relationship).ToList();

            return allPossibleRelationships;
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
         * Поиск всех возможных видов родства.
         */
        public static List<int> FindAllPossibleRelationships(float[][] generatedOutputMatrix,
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

                bool personAndRelativeAreRelatives = IsPersonAndRelativeAreRelatives(generatedOutputMatrix,
                    persons, person,
                    relatives, relative);

                bool personAndRelativeAreNotRelatives = IsPersonAndRelativeAreNotRelatives(generatedOutputMatrix,
                    persons, person,
                    relatives, relative);

                if (personAndRelativeAreRelatives && !personAndRelativeAreNotRelatives)
                {
                    currentPossibleRelationships = currentPossibleRelationships.Where(val => val != 0).ToList();
                }
                else if (!personAndRelativeAreRelatives && personAndRelativeAreNotRelatives)
                {
                    currentPossibleRelationships = currentPossibleRelationships.Where(val => val == 0).ToList();
                }

                /*
                 * Исключение возможных видов родства, которые невозможно сгенерировать.
                 */
                currentPossibleRelationships =
                    currentPossibleRelationships.Intersect(allPossibleRelationships).ToList();
            }

            return currentPossibleRelationships;
        }
    }
}