using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyMatrixCreator
{
    public class Integrations
    {
        Modules modules = new Modules();

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

            return (100 * (((float)sumOfMeaningfulValues) / Math.Pow(generatedMatrixSize, 2)));
        }

        /*
         * Построение правой (верхней) стороны  (сМ).
         */
        public float[][] InputBuildRightTopPart(float[][] generatedOutputMatrix, int[,][] relationshipsMatrix, int numberOfProband, float[][] generatedInputMatrix, float[] centimorgansMatrix)
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

        /*
         * Построение правой (верхней) стороны.
         */
        public float[][] OutputBuildRightTopPart(int[,][] relationshipsMatrix, int numberOfProband, int generatedMatrixSize, List<int> existingRelationshipDegrees, int[][] maxCountMatrix, int minPercent, int maxPercent)
        {
            float[][] generatedOutputMatrix = new float[generatedMatrixSize][];
            int[][] currentCountMatrix = new int[generatedMatrixSize][];

            List<int> persons = modules.ShuffleSequence(1, generatedOutputMatrix.GetLength(0));
            persons.Insert(0, 0);

            for (int person = 0; person < persons.Count; person++)
            {
                generatedOutputMatrix[persons[person]] = new float[generatedOutputMatrix.GetLength(0)];
                currentCountMatrix[persons[person]] = new int[maxCountMatrix.Length];

                List<int> relatives = modules.ShuffleSequence(persons[person] + 1, generatedOutputMatrix.GetLength(0));

                for (int relative = 0; relative < relatives.Count; relative++)
                {
                    int[] allPossibleRelationships = DetectAllPossibleRelationships(relationshipsMatrix, numberOfProband, maxCountMatrix, generatedOutputMatrix, currentCountMatrix, persons, person, relatives, relative);

                    /*
                     * Создание родственника со случайным видом родства.
                     */
                    generatedOutputMatrix[persons[person]][relatives[relative]] = allPossibleRelationships[modules.GetNextRnd(0, allPossibleRelationships.GetLength(0))];
                    currentCountMatrix = modules.IncreaseCurrentRelationshipCount(generatedOutputMatrix, currentCountMatrix, persons, person, relatives, relative, maxCountMatrix);
                }

                /*
                 * Проверка того, что выполняется требование по проценту значащих значений
                 */
                if (generatedOutputMatrix.GetLength(0) - 1 == person)
                {
                    double percentOfMeaningfulValues = 2 * CalculatePercentOfMeaningfulValues(generatedMatrixSize, existingRelationshipDegrees, generatedOutputMatrix);

                    if (percentOfMeaningfulValues < minPercent || percentOfMeaningfulValues > maxPercent)
                    {
                        generatedOutputMatrix = new float[generatedMatrixSize][];
                        currentCountMatrix = new int[generatedMatrixSize][];

                        persons = modules.ShuffleSequence(1, generatedOutputMatrix.GetLength(0));
                        persons.Insert(0, 0);

                        person = -1;
                    }
                }
            }

            return generatedOutputMatrix;
        }

        public int[] DetectAllPossibleRelationships(int[,][] relationshipsMatrix, int numberOfProband, int[][] maxCountMatrix, float[][] generatedOutputMatrix, int[][] currentCountMatrix, List<int> persons, int person, List<int> relatives, int relative)
        {
            int[] allPossibleRelationships;

            if (0 == persons[person])
            {
                allPossibleRelationships = modules.FindAllPossibleRelationshipsOfProband(relationshipsMatrix, numberOfProband);
            }
            else
            {
                allPossibleRelationships = FindAllPossibleRelationships(generatedOutputMatrix, persons, person, relatives, relative, relationshipsMatrix, numberOfProband, maxCountMatrix);
            }

            /*
             * Устранение видов родства из списка допустимых видов родства, 
             * добавление которых приведет к превышению допустимого числа родственников с таким видом родства.
             */
            foreach (int relationship in allPossibleRelationships)
            {
                if (false == modules.MaxNumberOfThisRelationshipTypeIsNotExceeded(relationship, currentCountMatrix, persons, person, maxCountMatrix))
                {
                    allPossibleRelationships = allPossibleRelationships.Where(val => val != relationship).ToArray();
                }
            }

            return allPossibleRelationships;
        }

        /*
         * Поиск всех возможных видов родства.
         */
        public int[] FindAllPossibleRelationships(float[][] generatedOutputMatrix, List<int> persons, int person, List<int> relatives, int relative, int[,][] relationshipsMatrix, int numberOfProband, int[][] maxCountMatrix)
        {
            int[] allPossibleRelationships = { };

            List<int> ancestralRelationships = new List<int> { };
            for (int relationship = 0; relationship < maxCountMatrix.GetLength(0); relationship++)
            {
                ancestralRelationships.Add(maxCountMatrix[relationship][0]);
            }

            int numberOfAncestralRelativesOfRelative = 0;
            for (int previousPerson = 1; previousPerson < person; previousPerson++)
            {
                for (int ancestralRelationship = 0; ancestralRelationship < ancestralRelationships.Count; ancestralRelationship++)
                {
                    if (ancestralRelationships[ancestralRelationship] == generatedOutputMatrix[persons[previousPerson]][relatives[relative]]
                        && 0 != generatedOutputMatrix[persons[previousPerson]][persons[person]])
                    {
                        numberOfAncestralRelativesOfRelative++;
                        break;
                    }
                }
            }

            int numberOfNotRelativesOfPerson = 0,
                numberOfNotRelativesOfRelative = 0;
            for (int previousPerson = 1; previousPerson < person; previousPerson++)
            {
                if (0 == generatedOutputMatrix[persons[previousPerson]][persons[person]])
                {
                    numberOfNotRelativesOfPerson++;
                }
                if (0 == generatedOutputMatrix[persons[previousPerson]][relatives[relative]])
                {
                    numberOfNotRelativesOfRelative++;
                }
            }

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
                    List<int> currentPossibleRelationships = Enumerable.Range(0, relationshipsMatrix.GetLength(1)).Select(j => relationshipsMatrix[numberOfProband, j][0]).ToList();
                    currentPossibleRelationships.Add(0);

                    /*
                     * Исключение возможных видов родства, которые невозможно сгенерировать.
                     */
                    allPossibleRelationships = allPossibleRelationships.Intersect(currentPossibleRelationships).ToArray();
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
                        List<int> currentPossibleRelationships = relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToList();
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
                        allPossibleRelationships = allPossibleRelationships.Intersect(currentPossibleRelationships).ToArray();
                    }
                }
            }

            return allPossibleRelationships;
        }
    }
}
