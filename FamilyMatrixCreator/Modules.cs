using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace FamilyMatrixCreator
{
    public class Modules
    {
        private static RNGCryptoServiceProvider _RNG = new RNGCryptoServiceProvider();

        /*
         * Проверка того, что на данное мгновение у данного лица не превышено MAX допустимое число родственников с таким видом родства.
         */
        public bool MaxNumberOfThisRelationshipTypeIsNotExceeded(int relationship, int[][] currentCountMatrix, List<int> persons, int person, int[][] maxCountMatrix)
        {
            bool allowToAddRelative = true;

            for (int i = 0; i < maxCountMatrix.Length; i++)
            {
                if (relationship == maxCountMatrix[i][0])
                {
                    if (currentCountMatrix[persons[person]][i] == maxCountMatrix[i][1])
                    {
                        allowToAddRelative = false;

                        return allowToAddRelative;
                    }
                }
            }

            return allowToAddRelative;
        }

        /*
         * Преобразование видов родства в сантиморганы.
         */
        public float TransformRelationshipTypeToCm(float[][] generatedInputMatrix, int person, int relative, int relationship, float[] centimorgansMatrix)
        {
            if (centimorgansMatrix[relationship] <= 3950)
            {
                double mean = centimorgansMatrix[relationship];
                double stdDev = (centimorgansMatrix[relationship] * (-0.2819 * Math.Log(centimorgansMatrix[relationship]) + 2.335)) / 3;

                Normal normalDist = new Normal(mean, stdDev);
                float normalyDistributedValue = (float)normalDist.Sample();

                if (normalyDistributedValue < 0)
                {
                    normalyDistributedValue = 0;
                }

                return (generatedInputMatrix[person][relative] = normalyDistributedValue);
            }
            else
            {
                return (generatedInputMatrix[person][relative] = centimorgansMatrix[relationship]);
            }
        }

        /*
        * Сбор статистики по родству.
        */
        public int[] CollectStatistics(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees, int[] quantityOfEachRelationship)
        {
            foreach (float[] raw in generatedOutputMatrix)
            {
                foreach (float column in raw)
                {
                    for (int probandsRelatioship = 0; probandsRelatioship < existingRelationshipDegrees.Count(); probandsRelatioship++)
                    {
                        if (column == existingRelationshipDegrees[probandsRelatioship])
                        {
                            quantityOfEachRelationship[probandsRelatioship]++;
                        }
                    }
                }
            }

            return quantityOfEachRelationship;
        }

        /*
         * Устранение разрывов между номерами видов родства.
         */
        public float[][] TransformMatrix(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees)
        {
            for (int person = 0; person < generatedOutputMatrix.GetLength(0); person++)
            {
                for (int relative = 0; relative < generatedOutputMatrix.GetLength(0); relative++)
                {
                    for (int relationship = 0; relationship < existingRelationshipDegrees.Count(); relationship++)
                    {
                        if (existingRelationshipDegrees[relationship] == generatedOutputMatrix[person][relative])
                        {
                            /*
                             * Нумерация значащих видов родства должна начинаться с 2.
                             */
                            generatedOutputMatrix[person][relative] = relationship + 2;
                            break;
                        }
                        else if (0 == generatedOutputMatrix[person][relative])
                        {
                            generatedOutputMatrix[person][relative] = 1;
                            break;
                        }
                    }
                }
            }

            return generatedOutputMatrix;
        }

        /*
         * Увеличение числа родственников данного вида у указанного лица.
         */
        public int[][] IncreaseCurrentRelationshipCount(float[][] generatedOutputMatrix, int[][] currentCountMatrix, List<int> persons, int person, List<int> relatives, int relative, int[][] maxCountMatrix)
        {
            for (int i = 0; i < maxCountMatrix.Length; i++)
            {
                if (maxCountMatrix[i][0] == generatedOutputMatrix[persons[person]][relatives[relative]])
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
        public List<int> FindAllPossibleRelationshipsOfProband(int[,][] relationshipsMatrix, int numberOfProband)
        {
            List<int> allPossibleRelationshipsOfProband = new List<int> { };

            for (int i = 0; i < relationshipsMatrix.GetLength(1); i++)
            {
                int quantityOfPossibleRelatives = 0;

                for (int possibleRelative = 0; possibleRelative < relationshipsMatrix.GetLength(1); possibleRelative++)
                {
                    int quantityOfPossibleRelationships = 0;

                    for (int possibleRelationship = 0;
                        possibleRelationship < relationshipsMatrix[i, possibleRelative].Length;
                        possibleRelationship++)
                    {
                        int quantityOfPossibleProbandsRelationships = 0;

                        for (int possibleProbandsRelationship = 0;
                            possibleProbandsRelationship < relationshipsMatrix.GetLength(1);
                            possibleProbandsRelationship++)
                        {
                            if (relationshipsMatrix[numberOfProband, possibleProbandsRelationship][0] == relationshipsMatrix[i, possibleRelative][possibleRelationship]
                                || 0 == relationshipsMatrix[i, possibleRelative][possibleRelationship])
                            {
                                quantityOfPossibleProbandsRelationships++;
                            }
                        }

                        if (quantityOfPossibleProbandsRelationships == relationshipsMatrix.GetLength(1))
                        {
                            quantityOfPossibleRelationships++;
                        }
                    }

                    if (quantityOfPossibleRelationships > 0)
                    {
                        quantityOfPossibleRelatives++;
                    }
                }

                if (0 < quantityOfPossibleRelatives)
                {
                    allPossibleRelationshipsOfProband.Add(relationshipsMatrix[numberOfProband, i][0]);
                }
            }

            return allPossibleRelationshipsOfProband.Distinct()
                                                    .Where(val => val != 0)
                                                    .ToList();
        }

        /*
         * Нахождение всех существующих степеней родства.
         */
        public List<int> FindAllExistingRelationshipDegrees(int[,][] relationshipsMatrix, int numberOfProband)
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
        public float[][] OutputBuildLeftBottomPart(float[][] generatedOutputMatrix, int[,][] relationshipsMatrix, int numberOfProband)
        {
            for (int genPerson = 1; genPerson < generatedOutputMatrix.GetLength(0); genPerson++)
            {
                for (int genRelative = 0; genRelative < genPerson; genRelative++)
                {
                    for (int genRelationship = 0; genRelationship < relationshipsMatrix.GetLength(1); genRelationship++)
                    {
                        if (relationshipsMatrix[numberOfProband, genRelationship][0] == generatedOutputMatrix[genRelative][genPerson])
                        {
                            generatedOutputMatrix[genPerson][genRelative] = relationshipsMatrix[genRelationship, numberOfProband][0];
                        }
                    }
                }
            }

            return generatedOutputMatrix;
        }

        /*
         * Построение левой (нижней) стороны (сМ).
         */
        public float[][] InputBuildLeftBottomPart(float[][] generatedInputMatrix)
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
        public float[][] FillMainDiagonal(float[][] generatedOutputMatrix)
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
        public int GetNextRnd(int min, int max)
        {
            byte[] rndBytes = new byte[4];
            _RNG.GetBytes(rndBytes);

            return (int)((BitConverter.ToInt32(rndBytes, 0) - (Decimal)int.MinValue) / (int.MaxValue - (Decimal)int.MinValue) * (max - min) + min);
        }

        /*
         * Определение номера строки, содержащей возможные степени родства пробанда.
         */
        public int FindNumberOfProband(int[,][] relationshipsMatrix)
        {
            int onlyOneRelationshipCount = 0;

            for (int x = 0; x < relationshipsMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < relationshipsMatrix.GetLength(0); y++)
                {
                    if (relationshipsMatrix[x, y].GetLength(0) == 1)
                    {
                        onlyOneRelationshipCount++;

                        if (relationshipsMatrix.GetLength(0) == onlyOneRelationshipCount)
                        {
                            return x;
                        }
                    }
                }

                onlyOneRelationshipCount = 0;
            }

            MessageBox.Show("Не удалось найти номер строки с возможными степенями родства пробанда.");
            Application.Exit();

            return 0;
        }
    }
}
