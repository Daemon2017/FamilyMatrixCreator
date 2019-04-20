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
        public static int[][] IncreaseCurrentRelationshipCount(float[][] generatedOutputMatrix, int[][] currentCountMatrix,
            List<int> persons, int person, List<int> relatives, int relative, int[][] ancestorsMaxCountMatrix)
        {
            for (int i = 0; i < ancestorsMaxCountMatrix.Length; i++)
            {
                if (ancestorsMaxCountMatrix[i][0] == generatedOutputMatrix[persons[person]][relatives[relative]])
                {
                    currentCountMatrix[persons[person]][i]++;

                    return currentCountMatrix;
                }
            }

            return currentCountMatrix;
        }

        /*
         * Построение списка возможных степеней родства пробанда.
         */
        public static List<int> FindAllPossibleRelationshipsOfProband(int[,][] relationshipsMatrix, int numberOfProband)
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
        public static List<int> FindAllExistingRelationshipDegrees(int[,][] relationshipsMatrix, int numberOfProband)
        {
            List<int> existingRelationshipDegrees = new List<int> { 0 };

            for (int i = 0; i < relationshipsMatrix.GetLength(0); i++)
            {
                existingRelationshipDegrees.Add(relationshipsMatrix[numberOfProband, i][0]);
                existingRelationshipDegrees.Add(relationshipsMatrix[i, numberOfProband][0]);
            }

            return existingRelationshipDegrees.Distinct().ToList();
        }

        /*
         * Построение левой (нижней) стороны.
         */
        public static float[][] OutputBuildLeftBottomPart(float[][] generatedOutputMatrix, int[,][] relationshipsMatrix,
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
        public static float[][] InputBuildLeftBottomPart(float[][] generatedInputMatrix)
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
            int[][] ancestorsMaxCountMatrix, int[][] currentCountMatrix, List<int> ancestralRelationships)
        {
            bool relationsWithAncestorMustExist = false;

            try
            {
                relationsWithAncestorMustExist =
                    (from i in Enumerable.Range(1, persons[person] - 1)
                     where ancestralRelationships.Contains((int)generatedOutputMatrix[persons[i]][persons[person]])
                     where ancestorsMaxCountMatrix[ancestralRelationships.IndexOf((int)generatedOutputMatrix[persons[i]][persons[person]])][1] ==
                           currentCountMatrix[i][ancestralRelationships.IndexOf((int)generatedOutputMatrix[persons[i]][persons[person]])]
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

    }
}