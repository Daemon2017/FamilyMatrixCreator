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
                     ancestorsCurrentCountMatrix[persons[person]][AncestorList.IndexOf(ancestor)] == RelationshipDictionary[ancestor].RelationshipMaxCount).Any()
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

        private static List<Relative> GetTree(int generatedMatrixSize, List<int> existingRelationshipDegrees)
        {
            List<Relative> relativesList = new List<Relative>
            {
                { new Relative(0, RelationshipDictionary[1], new List<Relative>(), new List<Relative>()) }
            };
            int relativeNumber = 1;

            List<int> allPossibleRelationships = GetListOfAllPossibleRelationshipsOfProband(existingRelationshipDegrees);

            for (int i = 1; i < generatedMatrixSize; i++)
            {
                RelationshipDegree selectedRandomRelationship = RelationshipDictionary[allPossibleRelationships[GetNextRandomValue(0, allPossibleRelationships.Count)]];

                int distanceX = selectedRandomRelationship.CoordX - relativesList[0].RelationshipDegree.CoordX;
                int distanceY = selectedRandomRelationship.CoordY - relativesList[0].RelationshipDegree.CoordY;

                if (distanceX == 0)
                {
                    if (distanceY > 0)
                    {
                        for (int stepY = 0; stepY < distanceY; stepY++)
                        {
                            relativesList = AddParentalRelationship(relativesList, relativeNumber, 0, stepY + 1, stepY == 0, stepY == distanceY - 1);

                            relativeNumber++;
                        }
                    }
                    else
                    {
                        for (int stepY = 0; stepY > distanceY; stepY--)
                        {
                            relativesList = AddDescendantRelationship(relativesList, relativeNumber, 0, stepY - 1, stepY == 0, stepY == distanceY + 1, true);

                            relativeNumber++;
                        }
                    }
                }
                else
                {
                    for (int stepY = 0; stepY < distanceX; stepY++)
                    {
                        relativesList = AddParentalRelationship(relativesList, relativeNumber, 0, stepY + 1, stepY == 0, 1 != 0);

                        relativeNumber++;
                    }

                    for (int stepY = distanceX; stepY > distanceY; stepY--)
                    {
                        relativesList = AddDescendantRelationship(relativesList, relativeNumber, distanceX, stepY - 1, 1 == 0, stepY == distanceY + 1, false);

                        relativeNumber++;
                    }
                }
            }

            return relativesList.Distinct().ToList();
        }
    }
}