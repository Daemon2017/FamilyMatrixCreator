using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace FamilyMatrixCreator
{
    public static class Modules
    {
        private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        /*
         * Преобразование видов родства в сантиморганы.
         * TODO: возможно, что массив centimorgansMatrix стоит преобразовать в словарь
         */
        public static float TransformRelationshipTypeToCm(float[][] generatedInputMatrix, int person, int relative,
            int relationship, float[] centimorgansMatrix)
        {
            if (!(centimorgansMatrix[relationship] <= 3950))
            {
                return generatedInputMatrix[person][relative] = centimorgansMatrix[relationship];
            }

            Normal normalDist = new Normal(centimorgansMatrix[relationship],
                centimorgansMatrix[relationship] * (-0.2819 * Math.Log(centimorgansMatrix[relationship]) + 2.335) / 3);
            float normalyDistributedValue = (float)normalDist.Sample();

            if (normalyDistributedValue < 0)
            {
                normalyDistributedValue = 0;
            }

            return generatedInputMatrix[person][relative] = normalyDistributedValue;
        }

        /*
        * Сбор статистики по родству.
        */
        public static int[] CollectStatistics(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees)
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
         * Увеличение числа родственников данного вида у указанного лица.
         * TODO: возможно, что массив ancestorsMaxCountMatrix стоит преобразовать в словарь
         */
        public static int[][] IncreaseCurrentRelationshipCount(float[][] generatedOutputMatrix, int[][] ancestorsCurrentCountMatrix,
            List<int> persons, int person, List<int> relatives, int relative, int[][] ancestorsMaxCountMatrix)
        {
            for (int i = 0; i < ancestorsMaxCountMatrix.Length; i++)
            {
                if (ancestorsMaxCountMatrix[i][0] == generatedOutputMatrix[persons[person]][relatives[relative]])
                {
                    ancestorsCurrentCountMatrix[persons[person]][i]++;

                    return ancestorsCurrentCountMatrix;
                }
            }

            return ancestorsCurrentCountMatrix;
        }

        /*
         * Построение списка возможных степеней родства пробанда.
         */
        public static List<int> GetAllPossibleRelationshipsOfProband(int[,][] relationshipsMatrix, int numberOfProband)
        {
            List<int> allPossibleRelationshipsOfProband =
                (from i in Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                 where (from possibleRelative in Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                        where (from possibleRelationship in Enumerable.Range(0, relationshipsMatrix[i, possibleRelative].Length)
                               where relationshipsMatrix.GetLength(1) ==
                                     (from possibleProbandsRelationship in Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                                      where relationshipsMatrix[numberOfProband, possibleProbandsRelationship][0] ==
                                         relationshipsMatrix[i, possibleRelative][possibleRelationship] ||
                                         0 == relationshipsMatrix[i, possibleRelative][possibleRelationship]
                                      select possibleProbandsRelationship).Count()
                               select possibleRelationship).Any()
                        select possibleRelative).Any()
                 select relationshipsMatrix[numberOfProband, i][0]).ToList();

            return allPossibleRelationshipsOfProband.Distinct().Where(val => val != 0).ToList();
        }

        /*
         * Нахождение всех существующих степеней родства.
         */
        public static List<int> GetAllExistingRelationshipDegrees(int[,][] relationshipsMatrix, int numberOfProband)
        {
            List<int> existingRelationshipDegrees = new List<int> { 0 };

            existingRelationshipDegrees.AddRange((from i in Enumerable.Range(0, relationshipsMatrix.GetLength(0))
                                                  select relationshipsMatrix[numberOfProband, i][0]).Union
                                                   (from i in Enumerable.Range(0, relationshipsMatrix.GetLength(0))
                                                    select relationshipsMatrix[i, numberOfProband][0]).ToList());

            return existingRelationshipDegrees.Distinct().ToList();
        }

        /*
         * Построение левой (нижней) стороны.
         */
        public static float[][] BuildLeftBottomPartOfOutput(float[][] generatedOutputMatrix, int[,][] relationshipsMatrix,
            int numberOfProband)
        {
            for (int genPerson = 1; genPerson < generatedOutputMatrix.GetLength(0); genPerson++)
            {
                for (int genRelative = 0; genRelative < genPerson; genRelative++)
                {
                    try
                    {
                        generatedOutputMatrix[genPerson][genRelative] =
                            (from genRelationship in Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                             where relationshipsMatrix[numberOfProband, genRelationship][0] ==
                                   generatedOutputMatrix[genRelative][genPerson]
                             select relationshipsMatrix[genRelationship, numberOfProband][0]).Single();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
            }

            return generatedOutputMatrix;
        }

        /*
         * Построение левой (нижней) стороны (сМ).
         */
        public static float[][] BuildLeftBottomPartOfInput(float[][] generatedInputMatrix)
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
         * Заполнение главной диагонали.
         */
        public static float[][] FillMainDiagonal(float[][] generatedOutputMatrix)
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
        public static int GetNextRnd(int min, int max)
        {
            byte[] rndBytes = new byte[4];
            Rng.GetBytes(rndBytes);

            return (int)((BitConverter.ToInt32(rndBytes, 0) - (decimal)int.MinValue) /
                          (int.MaxValue - (decimal)int.MinValue) * (max - min) + min);
        }

        public static bool IsRelationWithAncestorMustExist(float[][] generatedOutputMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative,
            int[][] ancestorsMaxCountMatrix, int[][] ancestorsCurrentCountMatrix,
            List<int> ancestralRelationships)
        {
            bool relationsWithAncestorMustExist = false;

            try
            {
                relationsWithAncestorMustExist =
                    (from i in Enumerable.Range(1, persons[person] - 1)
                     where ancestralRelationships.Contains((int)generatedOutputMatrix[persons[i]][persons[person]])
                     where ancestorsMaxCountMatrix[ancestralRelationships.IndexOf((int)generatedOutputMatrix[persons[i]][persons[person]])][1] ==
                           ancestorsCurrentCountMatrix[i][ancestralRelationships.IndexOf((int)generatedOutputMatrix[persons[i]][persons[person]])]
                     where (from j in Enumerable.Range(0, relatives[relative] - 1)
                            where generatedOutputMatrix[persons[i]][persons[j]] ==
                                  ancestorsMaxCountMatrix[ancestralRelationships.IndexOf((int)generatedOutputMatrix[persons[i]][persons[person]])][1]
                            where 0 == generatedOutputMatrix[persons[j]][relatives[relative]]
                            select j).Count() + 1 ==
                           ancestorsMaxCountMatrix[ancestralRelationships.IndexOf((int)generatedOutputMatrix[persons[i]][persons[person]])][1]
                     select i).Any();
            }
            catch (NullReferenceException)
            {

            }

            return relationsWithAncestorMustExist;
        }

        public static bool IsCountOfRelativesOfThisTypeAlreadyMax(float[][] generatedOutputMatrix,
            List<int> relatives, int relative,
            int[][] ancestorsMaxCountMatrix, int[][] ancestorsCurrentCountMatrix,
            List<int> ancestorsRelationships)
        {
            bool relativeHaveMaxCountOfRelativesOfThisType = false;

            if (ancestorsRelationships.Contains((int)generatedOutputMatrix[0][relatives[relative]]))
            {
                try
                {
                    relativeHaveMaxCountOfRelativesOfThisType =
                        (from i in Enumerable.Range(0, ancestorsMaxCountMatrix.GetLength(0))
                         where ancestorsMaxCountMatrix[i][0] == (int)generatedOutputMatrix[0][relatives[relative]]
                         where ancestorsMaxCountMatrix[i][1] == ancestorsCurrentCountMatrix[0][i]
                         select i).Any();
                }
                catch (NullReferenceException)
                {

                }
            }

            return relativeHaveMaxCountOfRelativesOfThisType;
        }

        /*
         * Среди возможных видов родства пробанда ищутся порядковые номера тех, что содержат выбранные виды родства.
         */
        public static int GetSerialNumberInListOfPossibleRelationships(float[][] generatedOutputMatrix, List<int> persons,
            List<int> relatives, int relative,
            int[,][] relationshipsMatrix, int numberOfProband, int previousPerson, int numberOfJ)
        {
            try
            {
                numberOfJ =
                    (from number in Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                     where relationshipsMatrix[numberOfProband, number][0] ==
                           generatedOutputMatrix[persons[previousPerson]][relatives[relative]]
                     select number).Single();
            }
            catch (InvalidOperationException)
            {

            }

            return numberOfJ;
        }

        /*
         * Определение количества родственников-предков текущего родственника.
         */
        public static bool IsNumberOfAncestorsNotZero(float[][] generatedOutputMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative,
            List<int> ancestorsRelationships)
        {
            return (from prevPerson in Enumerable.Range(1, person - 1)
                    where ancestorsRelationships.Contains((int)generatedOutputMatrix[persons[prevPerson]][relatives[relative]]) &&
                          0 != generatedOutputMatrix[persons[prevPerson]][persons[person]]
                    select prevPerson).Any();
        }

        public static bool IsPersonAndRelativeAreNotRelatives(float[][] generatedOutputMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative,
            List<int> ancestorsRelationships)
        {
            bool PersonAndRelativeAreNotRelatives = false;

            if ((int)generatedOutputMatrix[0][relatives[relative]] !=
                (int)generatedOutputMatrix[0][persons[person]])
            {
                if (ancestorsRelationships.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                    ancestorsRelationships.Contains((int)generatedOutputMatrix[0][persons[person]]))
                {
                    PersonAndRelativeAreNotRelatives = (from i in Enumerable.Range(0, persons[person])
                                                        where (0 == (int)generatedOutputMatrix[i][relatives[relative]] && 0 != (int)generatedOutputMatrix[i][persons[person]]) ||
                                                        (0 != (int)generatedOutputMatrix[i][relatives[relative]] && 0 == (int)generatedOutputMatrix[i][persons[person]])
                                                        select i).Any();
                }
            }

            return PersonAndRelativeAreNotRelatives;
        }
    }
}