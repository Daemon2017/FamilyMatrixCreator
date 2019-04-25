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
                    Modules.FindAllPossibleRelationshipsOfProband(relationshipsMatrix, numberOfProband);
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
                int numberOfI = -1;
                numberOfI = Modules.FindSerialNumberInListOfPossibleRelationships(
                    generatedOutputMatrix, persons,
                    persons, person,
                    relationshipsMatrix, numberOfProband, previousPerson, numberOfI);

                int numberOfJ = -1;
                numberOfJ = Modules.FindSerialNumberInListOfPossibleRelationships(
                    generatedOutputMatrix, persons,
                    relatives, relative,
                    relationshipsMatrix, numberOfProband, previousPerson, numberOfJ);

                if (0 == persons[previousPerson])
                {
                    allPossibleRelationships =
                        relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToList();
                    List<int> currentPossibleRelationships =
                        (from j in Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                         select relationshipsMatrix[numberOfProband, j][0]).ToList();
                    currentPossibleRelationships.Add(0);

                    /*
                     * Исключение возможных видов родства, которые невозможно сгенерировать.
                     */
                    allPossibleRelationships =
                        allPossibleRelationships.Intersect(currentPossibleRelationships).ToList();
                }
                else
                {
                    if (!(person - 1 == previousPerson &&
                          persons.Count - 1 == person))
                    {
                        List<int> currentPossibleRelationships;

                        /*
                         * Составление списка предковых степеней родства.
                         */
                        List<int> ancestorsRelationships =
                            (from j in Enumerable.Range(0, ancestorsMaxCountMatrix.GetLength(0))
                             select ancestorsMaxCountMatrix[j][0]).ToList();

                        if (!(0 == generatedOutputMatrix[persons[previousPerson]][persons[person]] &&
                          0 == generatedOutputMatrix[persons[previousPerson]][relatives[relative]]))
                        {
                            currentPossibleRelationships = new List<int>();

                            /*
                             * Составление списка потомковых степеней родства.
                             */
                            List<int> descendantsRelationships =
                                (from j in Enumerable.Range(0, descendantsMatrix.GetLength(0))
                                 select descendantsMatrix[j][0]).ToList();

                            if (-1 != numberOfI && -1 != numberOfJ)
                            {
                                currentPossibleRelationships =
                                    relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToList();
                            }
                            else
                            {
                                currentPossibleRelationships =
                                    allPossibleRelationships.Intersect(ancestorsRelationships).ToList();
                                currentPossibleRelationships.AddRange(allPossibleRelationships
                                    .Intersect(descendantsRelationships).ToList());
                            }

                            if (-1 == numberOfI || -1 == numberOfJ)
                            {
                                currentPossibleRelationships.Add(0);
                            }

                            /*
                             * Определение количества родственников-предков текущего родственника.
                             */
                            int numberOfAncestorsOfRelative =
                                (from prevPerson in Enumerable.Range(1, person - 1)
                                 from ancestralRelationship in Enumerable.Range(0, ancestorsRelationships.Count)
                                 where ancestorsRelationships[ancestralRelationship] ==
                                       generatedOutputMatrix[persons[prevPerson]][relatives[relative]] &&
                                       0 != generatedOutputMatrix[persons[prevPerson]][persons[person]]
                                 select ancestralRelationship).Count();

                            if (0 != numberOfAncestorsOfRelative)
                            {
                                currentPossibleRelationships.AddRange(ancestorsRelationships);
                            }

                            bool relationWithAncestorMustExist =
                                Modules.IsRelationWithAncestorMustExist(generatedOutputMatrix,
                                                                        persons, person,
                                                                        relatives, relative,
                                                                        ancestorsMaxCountMatrix, ancestorsCurrentCountMatrix,
                                                                        ancestorsRelationships);

                            if (relationWithAncestorMustExist)
                            {
                                currentPossibleRelationships =
                                    currentPossibleRelationships.Except(new List<int> { 0 }).ToList();
                            }
                            else
                            {
                                currentPossibleRelationships = currentPossibleRelationships.Except(
                                    from p in Enumerable.Range(1, person - 1)
                                    from relationship in Enumerable.Range(0, ancestorsRelationships.Count)
                                    where generatedOutputMatrix[persons[p]][persons[person]] ==
                                          ancestorsRelationships[relationship]
                                    where ancestorsMaxCountMatrix[relationship][1] ==
                                          ancestorsCurrentCountMatrix[persons[p]][relationship]
                                    select ancestorsRelationships[relationship]).ToList();
                            }
                        }
                        else
                        {
                            currentPossibleRelationships = allPossibleRelationships;

                            bool personHaveMaxCountOfRelativesOfThisType =
                                Modules.IsItHaveMaxCountOfRelativesOfThisType(generatedOutputMatrix,
                                                                              persons, person,
                                                                              ancestorsMaxCountMatrix, ancestorsCurrentCountMatrix,
                                                                              ancestorsRelationships);

                            bool relativeHaveMaxCountOfRelativesOfThisType =
                                Modules.IsItHaveMaxCountOfRelativesOfThisType(generatedOutputMatrix,
                                                                              relatives, relative,
                                                                              ancestorsMaxCountMatrix, ancestorsCurrentCountMatrix,
                                                                              ancestorsRelationships);

                            if (personHaveMaxCountOfRelativesOfThisType || relativeHaveMaxCountOfRelativesOfThisType)
                            {
                                if ((int)generatedOutputMatrix[0][relatives[relative]] != (int)generatedOutputMatrix[0][persons[person]])
                                {
                                    currentPossibleRelationships =
                                        currentPossibleRelationships.Except(new List<int> { 0 }).ToList();
                                }
                            }
                        }

                        /*
                         * Исключение возможных видов родства, которые невозможно сгенерировать.
                         */
                        allPossibleRelationships =
                            allPossibleRelationships.Intersect(currentPossibleRelationships).ToList();
                    }
                }
            }

            return allPossibleRelationships;
        }
    }
}