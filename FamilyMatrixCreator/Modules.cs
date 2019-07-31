using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace FamilyMatrixCreator
{
    public static partial class Form1
    {
        private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        /*
         * Преобразование видов родства в сантиморганы.
         */
        public static float TransformRelationshipTypeToCm(float[][] generatedInputMatrix, int person, int relative,
            int relationship)
        {
            if (RelationshipDictionary[relationship].CommonCm > 3950)
            {
                return generatedInputMatrix[person][relative] = RelationshipDictionary[relationship].CommonCm;
            }

            if (0 != relationship)
            {
                float mean = RelationshipDictionary[relationship].CommonCm;
                double std = mean * ((-0.2819 * Math.Log(mean)) + 2.335) / 3;
                Normal normalDist = new Normal(mean, std);

                float normalyDistributedValue = (float)normalDist.Sample();

                if (normalyDistributedValue < 0)
                {
                    normalyDistributedValue = 0;
                }

                return normalyDistributedValue;
            }
            else
            {
                return 0;
            }
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
         */
        public static int[][] IncreaseCurrentRelationshipCount(float[][] generatedOutputMatrix, int[][] ancestorsCurrentCountMatrix,
            List<int> persons, int person, List<int> relatives, int relative, List<int> ancestorsMatrix)
        {
            for (int i = 0; i < ancestorsMatrix.Count; i++)
            {
                if (ancestorsMatrix[i] == generatedOutputMatrix[persons[person]][relatives[relative]])
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
        public static List<int> GetAllPossibleRelationshipsOfProband(List<int> existingRelationshipDegrees)
        {
            existingRelationshipDegrees = existingRelationshipDegrees.Where(var => var != 1).ToList();

            List<int> allPossibleRelationshipsOfProband = new List<int>();

            for (int i = 1; i < RelationshipsMatrix.GetLength(0); i++)
            {
                int numberOfPossibleRelationships = 0;

                for (int j = 1; j < RelationshipsMatrix.GetLength(1); j++)
                {
                    for (int k = 0; k < RelationshipsMatrix[i, j].Length; k++)
                    {
                        if (existingRelationshipDegrees.Contains(RelationshipsMatrix[i, j][k]))
                        {
                            numberOfPossibleRelationships++;
                            break;
                        }
                    }
                }

                if (numberOfPossibleRelationships > 0.5 * existingRelationshipDegrees.Count)
                {
                    allPossibleRelationshipsOfProband.Add(RelationshipsMatrix[NumberOfProband, i][0]);
                }
            }

            return allPossibleRelationshipsOfProband;
        }

        /*
         * Нахождение всех существующих степеней родства.
         */
        public static List<int> GetAllExistingRelationshipDegrees()
        {
            List<int> existingRelationshipDegrees = new List<int>();

            existingRelationshipDegrees.AddRange((from i in Enumerable.Range(0, RelationshipsMatrix.GetLength(0))
                                                  select RelationshipsMatrix[NumberOfProband, i][0]).ToList());

            return existingRelationshipDegrees.Distinct().ToList();
        }

        /*
         * Построение левой (нижней) стороны.
         */
        public static float[][] BuildLeftBottomPartOfOutput(float[][] generatedOutputMatrix)
        {
            for (int genPerson = 1; genPerson < generatedOutputMatrix.GetLength(0); genPerson++)
            {
                for (int genRelative = 0; genRelative < genPerson; genRelative++)
                {
                    try
                    {
                        generatedOutputMatrix[genPerson][genRelative] =
                            (from genRelationship in Enumerable.Range(0, RelationshipsMatrix.GetLength(1))
                             where RelationshipsMatrix[NumberOfProband, genRelationship][0] ==
                                   generatedOutputMatrix[genRelative][genPerson]
                             select RelationshipsMatrix[genRelationship, NumberOfProband][0]).Single();
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

        /*
         * Проверка того, что число данных степеней родства у данного лица уже максимально.
         */
        public static bool IsCountOfRelativesOfThisTypeAlreadyMax(float[][] generatedOutputMatrix,
            List<int> relatives, int relative,
            int[][] ancestorsCurrentCountMatrix)
        {
            bool countOfRelativesOfThisTypeAlreadyMax = false;

            if (AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]))
            {
                countOfRelativesOfThisTypeAlreadyMax =
                    (from i in Enumerable.Range(0, AncestorList.Count)
                     where AncestorList[i] == (int)generatedOutputMatrix[0][relatives[relative]]
                     where RelationshipDictionary[AncestorList[i]].RelationshipMaxCount == ancestorsCurrentCountMatrix[0][i]
                     select i).Any();
            }

            return countOfRelativesOfThisTypeAlreadyMax;
        }

        /*
         * Среди возможных видов родства пробанда ищутся порядковые номера тех, что содержат выбранные виды родства.
         */
        public static int GetSerialNumberInListOfPossibleRelationships(float[][] generatedOutputMatrix, List<int> persons,
            List<int> relatives, int relative,
            int previousPerson)
        {
            int numberOfJ = -1;

            try
            {
                numberOfJ =
                    (from number in Enumerable.Range(0, RelationshipsMatrix.GetLength(1))
                     where RelationshipsMatrix[NumberOfProband, number][0] ==
                           generatedOutputMatrix[persons[previousPerson]][relatives[relative]]
                     select number).Single();
            }
            catch (InvalidOperationException)
            {

            }

            return numberOfJ;
        }

        /*
         * Проверка того, что число предков исследуемого лица не равно нулю.
         */
        public static bool IsNumberOfAncestorsNotZero(float[][] generatedOutputMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative)
        {
            bool numberOfAncestorsNotZero = false;

            numberOfAncestorsNotZero = (from prevPerson in Enumerable.Range(1, person - 1)
                                        where AncestorList.Contains((int)generatedOutputMatrix[persons[prevPerson]][relatives[relative]]) &&
                                              0 != generatedOutputMatrix[persons[prevPerson]][persons[person]]
                                        select prevPerson).Any();

            return numberOfAncestorsNotZero;
        }

        public static bool IsPersonAndRelativeAreRelatives(float[][] generatedOutputMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative)
        {
            bool personAndRelativeAreRelatives = false;

            if (AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                AncestorList.Contains((int)generatedOutputMatrix[0][persons[person]]) &&
                generatedOutputMatrix[0][relatives[relative]] != generatedOutputMatrix[0][persons[person]])
            {
                personAndRelativeAreRelatives = (from i in Enumerable.Range(1, person)
                                                 where
                                                 (persons[i] < persons[person] &&
                                                 !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                 //!(siblindantsRelationships.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                 //generatedOutputMatrix[0][persons[i]] == generatedOutputMatrix[0][persons[person]]) &&
                                                 ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 == (int)generatedOutputMatrix[persons[i]][persons[person]]) ||
                                                 (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 != (int)generatedOutputMatrix[persons[i]][persons[person]]))) ||
                                                 (persons[i] > persons[person] &&
                                                 !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                 //!(siblindantsRelationships.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                 //generatedOutputMatrix[0][persons[i]] == generatedOutputMatrix[0][persons[person]]) &&
                                                 ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 == (int)generatedOutputMatrix[persons[person]][persons[i]]) ||
                                                 (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 != (int)generatedOutputMatrix[persons[person]][persons[i]])))
                                                 select i).Any();
            }
            else if (!AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                     !SiblindantsList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                     !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[person]]) &&
                     !AncestorList.Contains((int)generatedOutputMatrix[0][persons[person]]))
            {
                personAndRelativeAreRelatives = (from i in Enumerable.Range(1, person)
                                                 where
                                                 (persons[i] < persons[person] &&
                                                 !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                 ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 == (int)generatedOutputMatrix[persons[i]][persons[person]]) ||
                                                 (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 != (int)generatedOutputMatrix[persons[i]][persons[person]]))) ||
                                                 (persons[i] > persons[person] &&
                                                 !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                 ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 == (int)generatedOutputMatrix[persons[person]][persons[i]]) ||
                                                 (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 != (int)generatedOutputMatrix[persons[person]][persons[i]])))
                                                 select i).Any();
            }
            else if (AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                    !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[person]]) &&
                    !AncestorList.Contains((int)generatedOutputMatrix[0][persons[person]]))
            {
                personAndRelativeAreRelatives = (from i in Enumerable.Range(1, person)
                                                 where
                                                 (persons[i] < persons[person] &&
                                                 !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                 ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 == (int)generatedOutputMatrix[persons[i]][persons[person]]) ||
                                                 (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 != (int)generatedOutputMatrix[persons[i]][persons[person]]))) ||
                                                 (persons[i] > persons[person] &&
                                                 !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                 ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 == (int)generatedOutputMatrix[persons[person]][persons[i]]) ||
                                                 (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 != (int)generatedOutputMatrix[persons[person]][persons[i]])))
                                                 select i).Any();
            }
            else if (!AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                     !SiblindantsList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                     AncestorList.Contains((int)generatedOutputMatrix[0][persons[person]]))
            {
                personAndRelativeAreRelatives = (from i in Enumerable.Range(1, person)
                                                 where
                                                 (persons[i] < persons[person] &&
                                                 !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                 ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 == (int)generatedOutputMatrix[persons[i]][persons[person]]) ||
                                                 (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 != (int)generatedOutputMatrix[persons[i]][persons[person]]))) ||
                                                 (persons[i] > persons[person] &&
                                                 !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                 ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 == (int)generatedOutputMatrix[persons[person]][persons[i]]) ||
                                                 (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                 0 != (int)generatedOutputMatrix[persons[person]][persons[i]])))
                                                 select i).Any();
            }

            return personAndRelativeAreRelatives;
        }

        /*
         * Проверка того, что Персона и Родственник не могут быть родственниками.
         */
        public static bool IsPersonAndRelativeAreNotRelatives(float[][] generatedOutputMatrix,
            List<int> persons, int person,
            List<int> relatives, int relative)
        {
            bool personAndRelativeAreNotRelatives = false;

            if (AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                AncestorList.Contains((int)generatedOutputMatrix[0][persons[person]]))
            {
                personAndRelativeAreNotRelatives = (from i in Enumerable.Range(1, person)
                                                    where
                                                    (persons[i] < persons[person] &&
                                                    !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    !(AncestorList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][persons[i]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][persons[person]]].CoordY &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][relatives[relative]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][persons[person]]].CoordY) &&
                                                    ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 != (int)generatedOutputMatrix[persons[i]][persons[person]]) ||
                                                    (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 == (int)generatedOutputMatrix[persons[i]][persons[person]]))) ||
                                                    (persons[i] > persons[person] &&
                                                    !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    !(AncestorList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][persons[i]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][persons[person]]].CoordY &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][relatives[relative]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][persons[person]]].CoordY) &&
                                                    ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 != (int)generatedOutputMatrix[persons[person]][persons[i]]) ||
                                                    (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 == (int)generatedOutputMatrix[persons[person]][persons[i]])))
                                                    select i).Any();
            }
            else if (!AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                     !SiblindantsList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                     !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[person]]) &&
                     !AncestorList.Contains((int)generatedOutputMatrix[0][persons[person]]))
            {
                personAndRelativeAreNotRelatives = (from i in Enumerable.Range(1, person)
                                                    where
                                                    (persons[i] < persons[person] &&
                                                    !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 != (int)generatedOutputMatrix[persons[i]][persons[person]]) ||
                                                    (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 == (int)generatedOutputMatrix[persons[i]][persons[person]]))) ||
                                                    (persons[i] > persons[person] &&
                                                    !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 != (int)generatedOutputMatrix[persons[person]][persons[i]]) ||
                                                    (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 == (int)generatedOutputMatrix[persons[person]][persons[i]])))
                                                    select i).Any();
            }
            else if (AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                     !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[person]]) &&
                     !AncestorList.Contains((int)generatedOutputMatrix[0][persons[person]]))
            {
                personAndRelativeAreNotRelatives = (from i in Enumerable.Range(1, person)
                                                    where
                                                    (persons[i] < persons[person] &&
                                                    !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    !(AncestorList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][persons[i]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][persons[person]]].CoordY &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][relatives[relative]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][persons[person]]].CoordY) &&
                                                    ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 != (int)generatedOutputMatrix[persons[i]][persons[person]]) ||
                                                    (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 == (int)generatedOutputMatrix[persons[i]][persons[person]]))) ||
                                                    (persons[i] > persons[person] &&
                                                    !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    !(AncestorList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][persons[i]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][persons[person]]].CoordY &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][relatives[relative]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][persons[person]]].CoordY) &&
                                                    ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 != (int)generatedOutputMatrix[persons[person]][persons[i]]) ||
                                                    (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 == (int)generatedOutputMatrix[persons[person]][persons[i]])))
                                                    select i).Any();
            }
            else if (!AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                     !SiblindantsList.Contains((int)generatedOutputMatrix[0][relatives[relative]]) &&
                     AncestorList.Contains((int)generatedOutputMatrix[0][persons[person]]))
            {
                personAndRelativeAreNotRelatives = (from i in Enumerable.Range(1, person)
                                                    where
                                                    (persons[i] < persons[person] &&
                                                    !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    !(AncestorList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][persons[i]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][relatives[relative]]].CoordY &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][persons[person]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][relatives[relative]]].CoordY) &&
                                                    ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 != (int)generatedOutputMatrix[persons[i]][persons[person]]) ||
                                                    (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 == (int)generatedOutputMatrix[persons[i]][persons[person]]))) ||
                                                    (persons[i] > persons[person] &&
                                                    !SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    !(AncestorList.Contains((int)generatedOutputMatrix[0][persons[i]]) &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][persons[i]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][relatives[relative]]].CoordY &&
                                                    RelationshipDictionary[(int)generatedOutputMatrix[0][persons[person]]].CoordY > RelationshipDictionary[(int)generatedOutputMatrix[0][relatives[relative]]].CoordY) &&
                                                    ((0 == (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 != (int)generatedOutputMatrix[persons[person]][persons[i]]) ||
                                                    (0 != (int)generatedOutputMatrix[persons[i]][relatives[relative]] &&
                                                    0 == (int)generatedOutputMatrix[persons[person]][persons[i]])))
                                                    select i).Any();
            }

            if (!SiblindantsList.Contains((int)generatedOutputMatrix[0][persons[person]]) &&
                AncestorList.Contains((int)generatedOutputMatrix[0][relatives[relative]]))
            {
                for (int i = persons[person] + 1; i < relatives[relative]; i++)
                {
                    if (((int)generatedOutputMatrix[0][relatives[relative]] == (int)generatedOutputMatrix[0][i]) &&
                        (!AncestorList.Contains((int)generatedOutputMatrix[persons[person]][i])))
                    {
                        personAndRelativeAreNotRelatives = true;
                    }
                }
            }

            return personAndRelativeAreNotRelatives;
        }
    }
}