using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyMatrixCreator
{
    public static class Integrations
    {
        /*
         * Подсчет процента значащих значений
         */
        public static double CalculatePercentOfMeaningfulValues(int generatedMatrixSize, List<int> existingRelationshipDegrees,
            float[][] generatedOutputMatrix)
        {
            int[] quantityOfEachRelationship = Modules.CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees);

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
            int[,][] relationshipsMatrix, List<int> existingRelationshipDegrees,
            int numberOfProband,
            float[][] generatedOutputMatrix, int[][] ancestorsCurrentCountMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative)
        {
            List<int> allPossibleRelationships;

            if (0 == persons[person])
            {
                allPossibleRelationships =
                    Modules.GetAllPossibleRelationshipsOfProband(relationshipsMatrix, existingRelationshipDegrees, numberOfProband);
            }
            else
            {
                allPossibleRelationships = FindAllPossibleRelationships(generatedOutputMatrix,
                    persons, person,
                    relatives, relative,
                    relationshipsMatrix, numberOfProband,
                    ancestorsCurrentCountMatrix);
            }

            /*
             * Устранение видов родства из списка допустимых видов родства, 
             * добавление которых приведет к превышению допустимого числа родственников с таким видом родства.
             */
            allPossibleRelationships =
                (from relationship in allPossibleRelationships
                 where !Form1.AncestorList.Where((ancestor) =>
                     relationship == ancestor &&
                     ancestorsCurrentCountMatrix[persons[person]][Form1.AncestorList.IndexOf(ancestor)] == Form1.ancestorsMaxCountDictionary[ancestor]).Any()
                 select relationship).ToList();

            return allPossibleRelationships;
        }

        /*
         * Поиск всех возможных видов родства.
         */
        public static List<int> FindAllPossibleRelationships(float[][] generatedOutputMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative,
            int[,][] relationshipsMatrix, int numberOfProband,
            int[][] ancestorsCurrentCountMatrix)
        {
            List<int> currentPossibleRelationships = new List<int>();

            for (int previousPerson = 0; previousPerson < person; previousPerson++)
            {
                int numberOfI = Modules.GetSerialNumberInListOfPossibleRelationships(
                    generatedOutputMatrix, persons,
                    persons, person,
                    relationshipsMatrix, numberOfProband, previousPerson);

                int numberOfJ = Modules.GetSerialNumberInListOfPossibleRelationships(
                    generatedOutputMatrix, persons,
                    relatives, relative,
                    relationshipsMatrix, numberOfProband, previousPerson);

                List<int> allPossibleRelationships;

                if (0 == persons[previousPerson])
                {
                    currentPossibleRelationships =
                        relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToList();
                    allPossibleRelationships =
                        (from j in Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                         select relationshipsMatrix[numberOfProband, j][0]).ToList();
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
                                relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToList();

                            bool numberOfAncestorsOfRelativeIsNotZero = Modules.IsNumberOfAncestorsNotZero(generatedOutputMatrix,
                            persons, person,
                            relatives, relative);

                            if (numberOfAncestorsOfRelativeIsNotZero)
                            {
                                allPossibleRelationships.AddRange(Form1.AncestorList);
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
                            Modules.IsCountOfRelativesOfThisTypeAlreadyMax(generatedOutputMatrix,
                                                                          persons, person,
                                                                          ancestorsCurrentCountMatrix);

                        bool relativesCountOfRelativesOfThisTypeAlreadyMax =
                            Modules.IsCountOfRelativesOfThisTypeAlreadyMax(generatedOutputMatrix,
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

                bool personAndRelativeAreRelatives = Modules.IsPersonAndRelativeAreRelatives(generatedOutputMatrix,
                    persons, person,
                    relatives, relative);

                bool personAndRelativeAreNotRelatives = Modules.IsPersonAndRelativeAreNotRelatives(generatedOutputMatrix,
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