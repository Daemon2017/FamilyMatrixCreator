using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FamilyMatrixCreator
{
    public partial class Form1 : Form
    {
        /*
         * Проверка того, что на данное мгновение у данного лица не превышено MAX допустимое число родственников с таким видом родства.
         */
        private bool MaxNumberOfThisRelationshipTypeIsNotExceeded(int relationship, int[][] currentCountMatrix, List<int> persons, int person)
        {
            bool allowToAddRelative = true;

            for (int i = 0; i < maxCountMatrix.Length; i++)
            {
                if (relationship == maxCountMatrix[i][0])
                {
                    if (currentCountMatrix[persons[person]][i] == maxCountMatrix[i][0])
                    {
                        allowToAddRelative = false;
                    }
                }
            }

            return allowToAddRelative;
        }

        /*
         * Преобразование видов родства в сантиморганы.
         */
        private float TransformRelationshipTypeToCm(float[][] generatedInputMatrix, int person, int relative, int relationship)
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
        private int[] CollectStatistics(float[][] generatedOutputMatrix, int[] quantityOfEachRelationship, List<int> existingRelationshipDegrees)
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
         * Создание матрицы соответствий и ее сохранение в файл.
         */
        private void CreateComplianceMatrix(List<int> existingRelationshipDegrees)
        {
            List<int[]> complianceMatrix = new List<int[]>
            {
                new int[2] {0, 1}
            };

            for (int relationship = 0; relationship < existingRelationshipDegrees.Count(); relationship++)
            {
                int[] compliance = { existingRelationshipDegrees[relationship], relationship + 2 };
                complianceMatrix.Add(compliance);
            }

            SaveToFile("compliance.csv", complianceMatrix);
        }

        /*
         * Устранение разрывов между номерами видов родства.
         */
        private float[][] TransformMatrix(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees)
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
         * Перемешивание порядка значений в списке.
         */
        private static List<int> ShuffleSequence(int startValue, int endValue)
        {
            List<int> relatives = new List<int> { };
            for (int relative = startValue; relative < endValue; relative++)
            {
                relatives.Add(relative);
            }
            return relatives.OrderBy(x => new ContinuousUniform().Sample()).ToList();
        }

        /*
         * Увеличение числа родственников данного вида у указанного лица.
         */
        private void IncreaseCurrentCount(float[][] generatedOutputMatrix, int[][] currentCountMatrix, List<int> persons, int person, List<int> relatives, int relative)
        {
            for (int i = 0; i < maxCountMatrix.Length; i++)
            {
                if (maxCountMatrix[i][0] == generatedOutputMatrix[persons[person]][relatives[relative]])
                {
                    currentCountMatrix[0][i]++;
                }
            }
        }

        /*
         * Построение списка возможных степеней родства пробанда.
         */
        private int[] FindAllPossibleRelationshipsOfProband()
        {
            int[] allPossibleRelationshipsOfProband = new int[relationshipsMatrix.GetLength(1)];

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
                    allPossibleRelationshipsOfProband[i] = relationshipsMatrix[numberOfProband, i][0];
                }
            }

            return allPossibleRelationshipsOfProband.Where(val => val != 0).ToArray();
        }
    }
}
