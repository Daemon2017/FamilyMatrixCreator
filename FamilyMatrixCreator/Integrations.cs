﻿using MathNet.Numerics.Distributions;
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

        /*
         * Построение правой (верхней) стороны  (сМ).
         */
        public float[][] InputBuildRightTopPart(float[][] generatedOutputMatrix, int[,][] relationshipsMatrix,
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
                                _modules.TransformRelationshipTypeToCm(generatedInputMatrix, person, relative,
                                    relationship, centimorgansMatrix);
                        }

                        if (relationshipsMatrix[relationship, numberOfProband][0] ==
                            generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] =
                                _modules.TransformRelationshipTypeToCm(generatedInputMatrix, person, relative,
                                    relationship, centimorgansMatrix);
                        }
                    }
                }
            }

            return generatedInputMatrix;
        }

        /*
         * Построение правой (верхней) стороны.
         */
        public float[][] OutputBuildRightTopPart(int[,][] relationshipsMatrix, int numberOfProband,
            int generatedMatrixSize, List<int> existingRelationshipDegrees, int[][] maxCountMatrix, int minPercent,
            int maxPercent)
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
                currentCountMatrix[persons[person]] = new int[maxCountMatrix.Length];

                List<int> relatives = (from x in Enumerable.Range(persons[person] + 1,
                        generatedOutputMatrix.GetLength(0) - (persons[person] + 1))
                    orderby new ContinuousUniform().Sample()
                    select x).ToList();

                for (int relative = 0; relative < relatives.Count; relative++)
                {
                    List<int> allPossibleRelationships = DetectAllPossibleRelationships(relationshipsMatrix,
                        numberOfProband, maxCountMatrix, generatedOutputMatrix, currentCountMatrix, persons, person,
                        relatives, relative);

                    /*
                     * Создание родственника со случайным видом родства.
                     */
                    try
                    {
                        generatedOutputMatrix[persons[person]][relatives[relative]] =
                            allPossibleRelationships[_modules.GetNextRnd(0, allPossibleRelationships.Count)];
                        currentCountMatrix = _modules.IncreaseCurrentRelationshipCount(generatedOutputMatrix,
                            currentCountMatrix, persons, person, relatives, relative, maxCountMatrix);
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
                    double percentOfMeaningfulValues = 2 * CalculatePercentOfMeaningfulValues(generatedMatrixSize,
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
                    if (person - 1 == numberOfNotRelativesOfPerson && person - 1 == numberOfNotRelativesOfRelative)
                    {
                    }
                    else if (person - 1 == previousPerson && persons.Count - 1 == person)
                    {
                    }
                    else if (0 == generatedOutputMatrix[persons[previousPerson]][persons[person]]
                             && 0 == generatedOutputMatrix[persons[previousPerson]][relatives[relative]])
                    {
                    }
                    else
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