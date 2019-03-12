using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyMatrixCreator
{
    public class Integrations
    {
        private readonly Modules _modules = new Modules();

        /*
         * Подсчет процента значащих значений
         */
        public double CalculatePercentOfMeaningfulValues(int generatedMatrixSize, List<int> existingRelationshipDegrees,
            float[][] generatedOutputMatrix)
        {
            int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count];
            quantityOfEachRelationship = _modules.CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees,
                quantityOfEachRelationship);

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

        public List<int> DetectAllPossibleRelationships(int[,][] relationshipsMatrix, int numberOfProband,
            int[][] maxCountMatrix, float[][] generatedOutputMatrix, int[][] currentCountMatrix, List<int> persons,
            int person, List<int> relatives, int relative)
        {
            List<int> allPossibleRelationships;

            if (0 == persons[person])
            {
                allPossibleRelationships =
                    _modules.FindAllPossibleRelationshipsOfProband(relationshipsMatrix, numberOfProband);
            }
            else
            {
                allPossibleRelationships = FindAllPossibleRelationships(generatedOutputMatrix, persons, person,
                    relatives, relative, relationshipsMatrix, numberOfProband, maxCountMatrix, currentCountMatrix);
            }

            /*
             * Устранение видов родства из списка допустимых видов родства, 
             * добавление которых приведет к превышению допустимого числа родственников с таким видом родства.
             */
            allPossibleRelationships = (from relationship in allPossibleRelationships
                where !maxCountMatrix.Where((raw, column) =>
                    relationship == raw[0] && currentCountMatrix[persons[person]][column] == raw[1]).Any()
                select relationship).ToList();

            return allPossibleRelationships;
        }

        /*
         * Поиск всех возможных видов родства.
         */
        public List<int> FindAllPossibleRelationships(float[][] generatedOutputMatrix, List<int> persons, int person,
            List<int> relatives, int relative, int[,][] relationshipsMatrix, int numberOfProband,
            int[][] maxCountMatrix, int[][] currentCountMatrix)
        {
            List<int> allPossibleRelationships = new List<int>();

            /*
             * Составление списка предковых степеней родства.
             */
            List<int> ancestralRelationships = (from j in Enumerable.Range(0, maxCountMatrix.GetLength(0))
                select maxCountMatrix[j][0]).ToList();

            /*
             * Определение количества родственников-предков текущего родственника.
             */
            int numberOfAncestralRelativesOfRelative = (from previousPerson in Enumerable.Range(1, person - 1)
                from ancestralRelationship in Enumerable.Range(0, ancestralRelationships.Count)
                where ancestralRelationships[ancestralRelationship] ==
                      generatedOutputMatrix[persons[previousPerson]][relatives[relative]]
                      && 0 != generatedOutputMatrix[persons[previousPerson]][persons[person]]
                select ancestralRelationship).Count();

            /*
             * Определение количества неродственных родственников текущего лица и родственника.
             */
            int numberOfNotRelativesOfPerson = (from previousPerson in Enumerable.Range(1, person - 1)
                where 0 == generatedOutputMatrix[persons[previousPerson]][persons[person]]
                select previousPerson).Count();
            int numberOfNotRelativesOfRelative = (from previousPerson in Enumerable.Range(1, person - 1)
                where 0 == generatedOutputMatrix[persons[previousPerson]][relatives[relative]]
                select previousPerson).Count();

            for (int previousPerson = 0; previousPerson < person; previousPerson++)
            {
                /*
                 * Среди возможных видов родства пробанда ищутся порядковые номера тех, что содержат выбранные виды родства.
                 */
                int numberOfI = 0;
                bool numberOfIExists = true;
                try
                {
                    numberOfI = (from number in Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                        where relationshipsMatrix[numberOfProband, number][0] ==
                              generatedOutputMatrix[persons[previousPerson]][persons[person]]
                        select number).Single();
                }
                catch (InvalidOperationException)
                {
                    numberOfIExists = false;
                }

                int numberOfJ = 0;
                bool numberOfJExists = true;
                try
                {
                    numberOfJ = (from number in Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                        where relationshipsMatrix[numberOfProband, number][0] ==
                              generatedOutputMatrix[persons[previousPerson]][relatives[relative]]
                        select number).Single();
                }
                catch (InvalidOperationException)
                {
                    numberOfJExists = false;
                }

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
                    if (!(person - 1 == numberOfNotRelativesOfPerson &&
                          person - 1 == numberOfNotRelativesOfRelative) &&
                        !(person - 1 == previousPerson &&
                          persons.Count - 1 == person) &&
                        !(0 == generatedOutputMatrix[persons[previousPerson]][persons[person]] &&
                          0 == generatedOutputMatrix[persons[previousPerson]][relatives[relative]]))
                    {
                        List<int> currentPossibleRelationships = new List<int>();

                        if (numberOfIExists && numberOfJExists)
                        {
                            currentPossibleRelationships =
                                relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToList();
                        }

                        if (0 == numberOfI || 0 == numberOfJ)
                        {
                            currentPossibleRelationships.Add(0);
                        }

                        if (0 != numberOfAncestralRelativesOfRelative)
                        {
                            currentPossibleRelationships.AddRange(ancestralRelationships);
                        }

                        for (int p = 1; p < person; p++)
                        {
                            for (int relationship = 0; relationship < ancestralRelationships.Count(); relationship++)
                            {
                                if (generatedOutputMatrix[persons[p]][persons[person]] ==
                                    ancestralRelationships[relationship])
                                {
                                    if (maxCountMatrix[relationship][1] ==
                                        currentCountMatrix[persons[p]][relationship])
                                    {
                                        currentPossibleRelationships.RemoveAll(values =>
                                            values == ancestralRelationships[relationship]);
                                    }
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