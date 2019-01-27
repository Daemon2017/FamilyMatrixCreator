using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FamilyMatrixCreator
{
    public class Modules
    {
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
        public int[] CollectStatistics(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees)
        {
            int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count()];

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
         * Создание матрицы соответствий.
         */
        public List<int[]> CreateComplianceMatrix(List<int> existingRelationshipDegrees)
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

            return complianceMatrix;
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
         * Перемешивание порядка значений в списке.
         */
        public List<int> ShuffleSequence(int startValue, int endValue)
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
        public int[][] IncreaseCurrentRelationshipCount(float[][] generatedOutputMatrix, int[][] currentCountMatrix, List<int> persons, int person, List<int> relatives, int relative, int[][] maxCountMatrix)
        {
            int[][] newCurrentCountMatrix = currentCountMatrix;

            for (int i = 0; i < maxCountMatrix.Length; i++)
            {
                if (maxCountMatrix[i][0] == generatedOutputMatrix[persons[person]][relatives[relative]])
                {
                    newCurrentCountMatrix[0][i]++;
                }
            }

            return newCurrentCountMatrix;
        }

        /*
         * Построение списка возможных степеней родства пробанда.
         */
        public int[] FindAllPossibleRelationshipsOfProband(int[,][] relationshipsMatrix, int numberOfProband)
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

        /*
         * Исключение возможных видов родства, которые невозможно сгенерировать.
         */
        public int[] RemoveImpossibleRelations(int[] allPossibleRelationships, int[] currentPossibleRelationships)
        {
            List<int> allCurrentPossibleRelations = new List<int> { 0 };

            for (int m = 0; m < allPossibleRelationships.GetLength(0); m++)
            {
                for (int n = 0; n < currentPossibleRelationships.GetLength(0); n++)
                {
                    if (allPossibleRelationships[m] == currentPossibleRelationships[n])
                    {
                        allCurrentPossibleRelations.Add(allPossibleRelationships[m]);
                    }
                }
            }

            return allCurrentPossibleRelations.ToArray();
        }

        /*
         * Нахождение всех существующих степеней родства.
         */
        public List<int> FindAllExistingRelationshipDegrees(int[,][] relationshipsMatrix, int numberOfProband)
        {
            List<int> existingRelationshipDegrees = new List<int>();

            for (int i = 0; i < relationshipsMatrix.GetLength(0); i++)
            {
                if (!existingRelationshipDegrees.Contains(relationshipsMatrix[numberOfProband, i][0]))
                {
                    existingRelationshipDegrees.Add(relationshipsMatrix[numberOfProband, i][0]);
                }

                if (!existingRelationshipDegrees.Contains(relationshipsMatrix[i, numberOfProband][0]))
                {
                    existingRelationshipDegrees.Add(relationshipsMatrix[i, numberOfProband][0]);
                }
            }

            return existingRelationshipDegrees;
        }

        /*
         * Построение левой (нижней) стороны.
         */
        public float[][] BuildLeftBottomPart(float[][] generatedOutputMatrix, int[,][] relationshipsMatrix, int numberOfProband)
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
         * Построение левой (нижней) стороны.
         */
        public float[][] BuildLeftBottomPart(float[][] generatedInputMatrix)
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
    }
}
