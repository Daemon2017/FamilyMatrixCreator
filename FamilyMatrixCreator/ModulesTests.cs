using NUnit.Framework;
using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    [TestFixture]
    public class ModulesTests
    {
        Modules modules = new Modules();

        private static int[,][] relationshipsMatrix = new int[3, 3][]
        {
            { new int[] { 1 }, new int[] { 6 }, new int[] { 2 } },
            { new int[] { 7 }, new int[] { 1 }, new int[] { 8 } },
            { new int[] { 3 }, new int[] { 9 }, new int[] { 1 } }
        };
        private static int[,][] relationshipsMatrix_2 = new int[3, 3][]
        {
            { new int[] { 1 }, new int[] { 8 }, new int[] { 3 } },
            { new int[] { 7 }, new int[] { 1 }, new int[] { 5 } },
            { new int[] { 2 }, new int[] { 4 }, new int[] { 1 } }
        };
        private static int[][] currentCountMatrix = new int[][]
        {
            new int[] { 2, 4, 8 },
            new int[] { 1, 3, 7 },
            new int[] { 0, 2, 6 },
            new int[] { 0, 1, 5 },
            new int[] { 0, 0, 4 }
        };
        private static int[][] maxCountMatrix = new int[][]
        {
            new int[] { 0, 2 },
            new int[] { 1, 4 },
            new int[] { 2, 8 }
        };
        private static float[][] generatedOutputMatrix = new float[][]
        {
            new float[] { 1, 8, 3 },
            new float[] { 9, 1, 0 },
            new float[] { 2, 0, 1 }
        };
        private static float[][] generatedOutputMatrix_RightTopAndMainDiagonal = new float[][]
        {
            new float[] { 1, 8, 3 },
            new float[] { 0, 1, 0 },
            new float[] { 0, 0, 1 }
        };
        private static float[][] generatedOutputMatrix_ZeroDiagonal = new float[][]
        {
            new float[] { 0, 8, 3 },
            new float[] { 9, 0, 0 },
            new float[] { 2, 0, 0 }
        };
        private static float[][] generatedInputMatrix = new float[][]
        {
            new float[] { 6800, 3200, 1800 },
            new float[] { 0, 6800, 0 },
            new float[] { 0, 0, 6800 }
        };
        private static int[] allPossibleRelationships = new int[] { 0, 1, 2, 3, 4 };
        private static int[] currentPossibleRelationships = new int[] { 2, 4, 5 };
        private static List<int> existingRelationshipDegrees = new List<int> { 9, 8, 3, 2, 1 };
        private static List<int> persons = new List<int> { 0, 1, 2, 3, 4 };

        private static object[] MaxNumberOfThisRelationshipTypeIsNotExceeded_DataProvider =
        {
            new object[] { 1, currentCountMatrix, persons, 2, maxCountMatrix, true },
            new object[] { 1, currentCountMatrix, persons, 0, maxCountMatrix, false },
            new object[] { 2, currentCountMatrix, persons, 2, maxCountMatrix, true },
            new object[] { 2, currentCountMatrix, persons, 0, maxCountMatrix, false }
        };

        [TestCaseSource("MaxNumberOfThisRelationshipTypeIsNotExceeded_DataProvider")]
        public void MaxNumberOfThisRelationshipTypeIsNotExceeded_Test(int relationship, int[][] currentCountMatrix, List<int> persons, int person, int[][] maxCountMatrix, bool result)
        {
            Assert.That(result, Is.EqualTo(modules.MaxNumberOfThisRelationshipTypeIsNotExceeded(relationship, currentCountMatrix, persons, person, maxCountMatrix)));
        }

        private static object[] CollectStatistics_DataProvider =
        {
            new object[] { generatedOutputMatrix, existingRelationshipDegrees, new int[] { 0, 0, 0, 0, 0 }, new int[] { 1, 1, 1, 1, 3 } },
            new object[] { generatedOutputMatrix, existingRelationshipDegrees, new int[] { 1, 2, 3, 4, 5 }, new int[] { 2, 3, 4, 5, 8 } },
            new object[] { generatedOutputMatrix_RightTopAndMainDiagonal, existingRelationshipDegrees, new int[] { 0, 0, 0, 0, 0 }, new int[] { 0, 1, 1, 0, 3 } },
            new object[] { generatedOutputMatrix_RightTopAndMainDiagonal, existingRelationshipDegrees, new int[] { 1, 2, 3, 4, 5 }, new int[] { 1, 3, 4, 4, 8 } },
            new object[] { generatedOutputMatrix_ZeroDiagonal, existingRelationshipDegrees, new int[] { 0, 0, 0, 0, 0 }, new int[] { 1, 1, 1, 1, 0 } },
            new object[] { generatedOutputMatrix_ZeroDiagonal, existingRelationshipDegrees, new int[] { 1, 2, 3, 4, 5 }, new int[] { 2, 3, 4, 5, 5 } }
        };

        [TestCaseSource("CollectStatistics_DataProvider")]
        public void CollectStatistics_Test(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees, int[] quantityOfEachRelationship, int[] result)
        {
            int[] a = modules.CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees, quantityOfEachRelationship);
            Assert.That(result, Is.EqualTo(a));
        }

        private static object[] CreateComplianceMatrix_DataProvider =
        {
            new object[] { new List<int> { 1, 2, 3, 4, 5 },
                new List<int[]> { new int[2] { 0, 1 }, new int[2] { 1, 2 }, new int[2] { 2, 3 }, new int[2] { 3, 4 }, new int[2] { 4, 5 }, new int[2] { 5, 6 } } },
            new object[] { new List<int> { 5, 3, 6, 1, 2 },
                new List<int[]> { new int[2] { 0, 1 }, new int[2] { 5, 2 }, new int[2] { 3, 3 }, new int[2] { 6, 4 }, new int[2] { 1, 5 }, new int[2] { 2, 6 } } }
        };

        [TestCaseSource("CreateComplianceMatrix_DataProvider")]
        public void CreateComplianceMatrix_Test(List<int> existingRelationshipDegrees, List<int[]> result)
        {
            Assert.That(result, Is.EqualTo(modules.CreateComplianceMatrix(existingRelationshipDegrees)));
        }

        private static object[] TransformMatrix_DataProvider =
        {
            new object[] { generatedOutputMatrix, existingRelationshipDegrees, new float[][] { new float[] {6, 3, 4 }, new float[] {2, 6, 1 }, new float[] {5, 1, 6 } } },
            new object[] { generatedOutputMatrix_RightTopAndMainDiagonal, existingRelationshipDegrees, new float[][] { new float[] {6, 3, 4 }, new float[] {7, 6, 1 }, new float[] {5, 1, 6 } } },
            new object[] { generatedOutputMatrix_ZeroDiagonal, existingRelationshipDegrees, new float[][] { new float[] {6, 3, 4 }, new float[] {2, 6, 1 }, new float[] {5, 1, 6 } } }
        };

        [TestCaseSource("TransformMatrix_DataProvider")]
        public void TransformMatrix_Test(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees, float[][] result)
        {
            Assert.That(result, Is.EqualTo(modules.TransformMatrix(generatedOutputMatrix, existingRelationshipDegrees)));
        }

        [Test]
        public void IncreaseCurrentRelationshipCount_Test()
        {
            int[][] currentCountMatrix = new int[][]
            {
                new int[]{ 0, 1, 0, 1, 1, 0 },
                new int[]{ 1, 1, 0, 0, 0, 1 },
                new int[]{ 1, 1, 1, 0, 0, 0 }
            };
            int[][] maxCountMatrix = new int[][]
            {
                new int[]{ 0, 2 },
                new int[]{ 1, 4 },
                new int[]{ 2, 4 },
                new int[]{ 3, 4 },
                new int[]{ 8, 4 },
                new int[]{ 9, 8 }
            };

            int[][] model = new int[][]
            {
                new int[]{ 0, 1, 0, 2, 1, 0 },
                new int[]{ 1, 1, 0, 0, 0, 1 },
                new int[]{ 1, 1, 1, 0, 0, 0 }
            };

            Assert.That(model,
                Is.EqualTo(modules.IncreaseCurrentRelationshipCount(generatedOutputMatrix: generatedOutputMatrix,
                                                                    currentCountMatrix: currentCountMatrix,
                                                                    persons: new List<int> { 0, 1, 2 },
                                                                    person: 0,
                                                                    relatives: new List<int> { 1, 2 },
                                                                    relative: 1,
                                                                    maxCountMatrix: maxCountMatrix)));
        }

        private static object[] RemoveImpossibleRelations_DataProvider =
        {
            new object[] { allPossibleRelationships, currentPossibleRelationships, new int[] { 0, 2, 4 } }
        };

        [TestCaseSource("RemoveImpossibleRelations_DataProvider")]
        public void RemoveImpossibleRelations_Test(int[] allPossibleRelationships, int[] currentPossibleRelationships, int[] result)
        {
            Assert.That(result, Is.EqualTo(modules.RemoveImpossibleRelations(allPossibleRelationships, currentPossibleRelationships)));
        }

        private static object[] FindAllExistingRelationshipDegrees_DataProvider =
        {
            new object[] { relationshipsMatrix, 0, new List<int> { 1, 6, 7, 2, 3 } },
            new object[] { relationshipsMatrix, 1, new List<int> { 7, 6, 1, 8, 9 } }
        };

        [TestCaseSource("FindAllExistingRelationshipDegrees_DataProvider")]
        public void FindAllExistingRelationshipDegrees_Test(int[,][] relationshipsMatrix, int numberOfProband, List<int> result)
        {
            Assert.That(result, Is.EqualTo(modules.FindAllExistingRelationshipDegrees(relationshipsMatrix, numberOfProband)));
        }

        private static object[] OutputBuildLeftBottomPart_DataProvider =
        {
            new object[] { generatedOutputMatrix, relationshipsMatrix, 0, new float[][] { new float[] { 1, 8, 3 }, new float[] { 9, 1, 0 }, new float[] { 2, 0, 1 } } },
            //new object[] { generatedOutputMatrix, relationshipsMatrix_2, 0, new float[][] { new float[] { 1, 8, 3 }, new float[] { 7, 1, 0 }, new float[] { 2, 0, 1 } } },
            new object[] { generatedOutputMatrix_RightTopAndMainDiagonal, relationshipsMatrix, 0, new float[][] { new float[] { 1, 8, 3 }, new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 } } },
            new object[] { generatedOutputMatrix_RightTopAndMainDiagonal, relationshipsMatrix_2, 0, new float[][] { new float[] { 1, 8, 3 }, new float[] { 7, 1, 0 }, new float[] { 2, 0, 1 } } },
            //new object[] { generatedOutputMatrix_ZeroDiagonal, relationshipsMatrix, 0, new float[][] { new float[] { 0, 8, 3 }, new float[] { 9, 0, 0 }, new float[] { 2, 0, 0 } } },
            //new object[] { generatedOutputMatrix_ZeroDiagonal, relationshipsMatrix_2, 0, new float[][] { new float[] { 0, 8, 3 }, new float[] { 7, 0, 0 }, new float[] { 2, 0, 0 } } }
        };

        [TestCaseSource("OutputBuildLeftBottomPart_DataProvider")]
        public void OutputBuildLeftBottomPart_Test(float[][] generatedOutputMatrix, int[,][] relationshipsMatrix, int numberOfProband, float[][] result)
        {
            Assert.That(result, Is.EqualTo(modules.BuildLeftBottomPart(generatedOutputMatrix, relationshipsMatrix, numberOfProband)));
        }

        private static object[] InputBuildLeftBottomPart_DataProvider =
        {
            new object[] { generatedInputMatrix, new float[][] { new float[] { 6800, 3200, 1800 }, new float[] { 3200, 6800, 0 }, new float[] { 1800, 0, 6800 } } }
        };

        [TestCaseSource("InputBuildLeftBottomPart_DataProvider")]
        public void InputBuildLeftBottomPart_Test(float[][] generatedInputMatrix, float[][] result)
        {
            Assert.That(result, Is.EqualTo(modules.BuildLeftBottomPart(generatedInputMatrix)));
        }

        private static object[] FillMainDiagonal_DataProvider =
        {
            new object[] { generatedOutputMatrix, new float[][] { new float[] { 1, 8, 3 }, new float[] { 9, 1, 0 }, new float[] { 2, 0, 1 } } },
            new object[] { generatedOutputMatrix_RightTopAndMainDiagonal, new float[][] { new float[] { 1, 8, 3 }, new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 } } },
            new object[] { generatedOutputMatrix_ZeroDiagonal, new float[][] { new float[] { 1, 8, 3 }, new float[] { 9, 1, 0 }, new float[] { 2, 0, 1 } } }
        };

        [TestCaseSource("FillMainDiagonal_DataProvider")]
        public void FillMainDiagonal_Test(float[][] generatedOutputMatrix, float[][] result)
        {
            Assert.That(result, Is.EqualTo(modules.FillMainDiagonal(generatedOutputMatrix)));
        }

        private static object[] GetNextRnd_DataProvider =
        {
            new object[] { 5, 10 },
            new object[] { 6, 8 },
            new object[] { 7, 7 }
        };

        [TestCaseSource("GetNextRnd_DataProvider")]
        public void GetNextRnd_Test(int min, int max)
        {
            Assert.That(modules.GetNextRnd(min, max), Is.GreaterThan(min - 1).And.LessThan(max + 1));
        }
    }
}
