using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FamilyMatrixCreator
{
    public partial class Form1 : Form
    {
        /*
         * Проверка того, что на данное мгновение не превышено максимальное допустимое число
         * родственников с таким видом родства.
         */
        private bool MaxNumberOfThisRelationshipTypeIsNotExceeded(float[][] generatedOutputMatrix, int[][] currentCountMatrix, int person, List<int> relatives, int relative)
        {
            bool allowToAddRelative = true;

            for (int i = 0; i < maxCountMatrix.Length; i++)
            {
                if (generatedOutputMatrix[person][relatives[relative]] == maxCountMatrix[i][0])
                {
                    if (currentCountMatrix[person][i] == maxCountMatrix[i][0])
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
        * Сбор статистики по родству
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
    }
}
