using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyMatrixCreator
{
    public class Integrations
    {
        Modules modules = new Modules();

        /*
         * Исключение невозможных видов родства.
         */
        public int[] FindAppPossibleRelationships(float[][] generatedOutputMatrix, List<int> persons, int person, List<int> relatives, int relative, int[] allPossibleRelationships, int[,][] relationshipsMatrix, int numberOfProband)
        {
            for (int previousPerson = 0; previousPerson < person; previousPerson++)
            {
                int numberOfI = 0,
                    numberOfJ = 0;

                /*
                 * Среди возможных видов родства пробанда ищутся порядковые номера тех, что содержат выбранные виды родства.
                 */
                for (int number = 0; number < relationshipsMatrix.GetLength(1); number++)
                {
                    if (relationshipsMatrix[numberOfProband, number][0] == generatedOutputMatrix[persons[previousPerson]][persons[person]])
                    {
                        numberOfI = number;
                    }

                    if (relationshipsMatrix[numberOfProband, number][0] == generatedOutputMatrix[persons[previousPerson]][relatives[relative]])
                    {
                        numberOfJ = number;
                    }
                }

                if (0 == persons[previousPerson])
                {
                    allPossibleRelationships = relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToArray();
                    int[] currentPossibleRelationships = Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                                                                   .Select(j => relationshipsMatrix[numberOfProband, j][0]).ToArray();

                    allPossibleRelationships = modules.RemoveImpossibleRelations(allPossibleRelationships, currentPossibleRelationships);
                }
                else
                {
                    int[] currentPossibleRelationships = relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToArray();

                    allPossibleRelationships = modules.RemoveImpossibleRelations(allPossibleRelationships, currentPossibleRelationships);
                }
            }

            return allPossibleRelationships;
        }

        /*
         * Подсчет процента значащих значений
         */
        public double CalculatePercentOfMeaningfulValues(int generatedMatrixSize, List<int> existingRelationshipDegrees, float[][] generatedOutputMatrix)
        {
            int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count()];
            quantityOfEachRelationship = modules.CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees, quantityOfEachRelationship);

            int sumOfMeaningfulValues = 0;

            foreach (var quantity in quantityOfEachRelationship)
            {
                sumOfMeaningfulValues += quantity;
            }
            
            return (100 * ((2 * (float)sumOfMeaningfulValues) / Math.Pow(generatedMatrixSize, 2)));
        }

        /*
         * Построение правой (верхней) стороны.
         */
        public float[][] BuildRightTopPart(float[][] generatedOutputMatrix, float[][] generatedInputMatrix, int[,][] relationshipsMatrix, float[] centimorgansMatrix, int numberOfProband)
        {
            for (int person = 0; person < generatedOutputMatrix.GetLength(0); person++)
            {
                generatedInputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];

                for (int relative = person; relative < generatedOutputMatrix.GetLength(0); relative++)
                {
                    for (int relationship = 0; relationship < relationshipsMatrix.GetLength(1); relationship++)
                    {
                        if (relationshipsMatrix[numberOfProband, relationship][0] == generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] = modules.TransformRelationshipTypeToCm(generatedInputMatrix, person, relative, relationship, centimorgansMatrix);
                        }

                        if (relationshipsMatrix[relationship, numberOfProband][0] == generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] = modules.TransformRelationshipTypeToCm(generatedInputMatrix, person, relative, relationship, centimorgansMatrix);
                        }
                    }
                }
            }

            return generatedInputMatrix;
        }
    }
}
