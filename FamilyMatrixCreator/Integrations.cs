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

        public static List<int> DetectAllPossibleRelationships(int[,][] relationshipsMatrix, int numberOfProband,
            int[][] ancestorsMaxCountMatrix, int[][] descendantsMatrix,
            float[][] generatedOutputMatrix, int[][] ancestorsCurrentCountMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative)
        {
            List<int> allPossibleRelationships;

            if (0 == persons[person])
            {
                allPossibleRelationships =
                    Modules.GetAllPossibleRelationshipsOfProband(relationshipsMatrix, numberOfProband);
            }
            else
            {
                allPossibleRelationships = FindAllPossibleRelationships(generatedOutputMatrix,
                    persons, person,
                    relatives, relative,
                    relationshipsMatrix, numberOfProband,
                    ancestorsMaxCountMatrix, descendantsMatrix,
                    ancestorsCurrentCountMatrix);
            }

            /*
             * Устранение видов родства из списка допустимых видов родства, 
             * добавление которых приведет к превышению допустимого числа родственников с таким видом родства.
             */
            allPossibleRelationships =
                (from relationship in allPossibleRelationships
                 where !ancestorsMaxCountMatrix.Where((raw, column) =>
                     relationship == raw[0] && ancestorsCurrentCountMatrix[persons[person]][column] == raw[1]).Any()
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
            int[][] ancestorsMaxCountMatrix, int[][] descendantsMatrix,
            int[][] ancestorsCurrentCountMatrix)
        {
            List<int> allPossibleRelationships = new List<int>();

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

                /*
                 * Составление списка предковых степеней родства.
                 */
                List<int> ancestorsRelationships =
                    (from j in Enumerable.Range(0, ancestorsMaxCountMatrix.GetLength(0))
                     select ancestorsMaxCountMatrix[j][0]).ToList();

                /*
                 * Составление списка потомковых степеней родства.
                 */
                List<int> descendantsRelationships =
                    (from j in Enumerable.Range(0, descendantsMatrix.GetLength(0))
                     select descendantsMatrix[j][0]).ToList();

                List<int> currentPossibleRelationships;

                if (0 == persons[previousPerson])
                {
                    allPossibleRelationships =
                        relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToList();
                    currentPossibleRelationships =
                        (from j in Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                         select relationshipsMatrix[numberOfProband, j][0]).ToList();
                    currentPossibleRelationships.Add(0);
                }
                else
                {
                    if (!(0 == generatedOutputMatrix[persons[previousPerson]][persons[person]] &&
                      0 == generatedOutputMatrix[persons[previousPerson]][relatives[relative]]))
                    {
                        if (-1 != numberOfI && -1 != numberOfJ)
                        {
                            currentPossibleRelationships =
                                relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToList();

                            bool numberOfAncestorsOfRelativeIsNotZero = Modules.IsNumberOfAncestorsNotZero(generatedOutputMatrix,
                            persons, person,
                            relatives, relative,
                            ancestorsRelationships);

                            if (numberOfAncestorsOfRelativeIsNotZero)
                            {
                                currentPossibleRelationships.AddRange(ancestorsRelationships);
                            }
                        }
                        else
                        {
                            currentPossibleRelationships =
                                allPossibleRelationships.Intersect(ancestorsRelationships).ToList();
                            currentPossibleRelationships.AddRange(allPossibleRelationships
                                .Intersect(descendantsRelationships).ToList());
                            currentPossibleRelationships.Add(0);
                        }

                        bool personAndRelativeAreRelatives = Modules.IsPersonAndRelativeAreRelatives(generatedOutputMatrix,
                            persons, person,
                            relatives, relative,
                            descendantsRelationships);

                        if (personAndRelativeAreRelatives)
                        {
                            currentPossibleRelationships = allPossibleRelationships.Where(val => val != 0).ToList();
                        }

                        bool PersonAndRelativeAreNotRelatives = Modules.IsPersonAndRelativeAreNotRelatives(generatedOutputMatrix,
                            persons, person,
                            relatives, relative,
                            ancestorsRelationships, descendantsRelationships);

                        if (PersonAndRelativeAreNotRelatives && !personAndRelativeAreRelatives)
                        {
                            currentPossibleRelationships = currentPossibleRelationships.Where(val => val == 0).ToList();
                        }
                    }
                    else
                    {
                        currentPossibleRelationships = allPossibleRelationships;

                        bool personsCountOfRelativesOfThisTypeAlreadyMax =
                            Modules.IsCountOfRelativesOfThisTypeAlreadyMax(generatedOutputMatrix,
                                                                          persons, person,
                                                                          ancestorsMaxCountMatrix, ancestorsCurrentCountMatrix,
                                                                          ancestorsRelationships);

                        bool relativesCountOfRelativesOfThisTypeAlreadyMax =
                            Modules.IsCountOfRelativesOfThisTypeAlreadyMax(generatedOutputMatrix,
                                                                          relatives, relative,
                                                                          ancestorsMaxCountMatrix, ancestorsCurrentCountMatrix,
                                                                          ancestorsRelationships);

                        if (personsCountOfRelativesOfThisTypeAlreadyMax || relativesCountOfRelativesOfThisTypeAlreadyMax)
                        {
                            if ((int)generatedOutputMatrix[0][relatives[relative]] != (int)generatedOutputMatrix[0][persons[person]])
                            {
                                currentPossibleRelationships =
                                    currentPossibleRelationships.Except(new List<int> { 0 }).ToList();
                            }
                        }
                    }
                }

                bool relationshipWithProbandsAncestorMustNotExist = Modules.IsRelationshipWithProbandsAncestorMustNotExist(generatedOutputMatrix,
                    persons, person,
                    relatives, relative,
                    ancestorsRelationships, descendantsRelationships);

                if (relationshipWithProbandsAncestorMustNotExist)
                {
                    currentPossibleRelationships = currentPossibleRelationships.Where(val => val == 0).ToList();
                }

                /*
                 * Исключение возможных видов родства, которые невозможно сгенерировать.
                 */
                allPossibleRelationships =
                    allPossibleRelationships.Intersect(currentPossibleRelationships).ToList();
            }

            return allPossibleRelationships;
        }
    }
}