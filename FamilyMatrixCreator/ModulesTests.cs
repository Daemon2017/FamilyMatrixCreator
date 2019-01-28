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
        private static int[][] currentCountMatrix = new int[][]
        {
            new int[]{ 2, 4, 8 },
            new int[]{ 1, 3, 7 },
            new int[]{ 0, 2, 6 },
            new int[]{ 0, 1, 5 },
            new int[]{ 0, 0, 4 }
        };
        private static int[][] maxCountMatrix = new int[][]
        {
            new int[]{ 0, 2 },
            new int[]{ 1, 4 },
            new int[]{ 2, 8 }
        };
        private static float[][] generatedOutputMatrix = new float[][]
        {
            new float[] { 1, 8, 3 },
            new float[] { 9, 1, 0 },
            new float[] { 2, 0, 1 }
        };
        private static float[][] generatedInputMatrix = new float[][]
        {
            new float[] { 6800, 3200, 1800 },
            new float[] { 0, 6800, 0 },
            new float[] { 0, 0, 6800 }
        };
        private static int[] allPossibleRelationships = new int[] { 0, 1, 2, 3, 4 };
        private static int[] currentPossibleRelationships = new int[] { 2, 4, 5 };
        private static List<int> persons = new List<int> { 0, 1, 2, 3, 4 };

        private static object[] MaxNumberOfThisRelationshipTypeIsNotExceeded_DataProvider =
        {
            new object[] { 1, currentCountMatrix, persons, 2, maxCountMatrix, true },
            new object[] { 1, currentCountMatrix, persons, 0, maxCountMatrix, false },
            new object[] { 2, currentCountMatrix, persons, 2, maxCountMatrix, true },
            new object[] { 2, currentCountMatrix, persons, 0, maxCountMatrix, false }
        };

        [TestCaseSource("MaxNumberOfThisRelationshipTypeIsNotExceeded_DataProvider")]
        private void MaxNumberOfThisRelationshipTypeIsNotExceeded_Test(int relationship, int[][] currentCountMatrix, List<int> persons, int person, int[][] maxCountMatrix, bool result)
        {
            Assert.That(result, Is.EqualTo(modules.MaxNumberOfThisRelationshipTypeIsNotExceeded(relationship, currentCountMatrix, persons, person, maxCountMatrix)));
        }

        private static object[] CollectStatistics_DataProvider =
        {
            new object[] { generatedOutputMatrix, new List<int> { 9, 8, 3, 2, 1 }, new int[] { 0, 0, 0, 0, 0 }, new int[] { 1, 1, 1, 1, 3 } },
            new object[] { generatedOutputMatrix, new List<int> { 9, 8, 3, 2, 1 }, new int[] { 1, 2, 3, 4, 5 }, new int[] { 2, 3, 4, 5, 8 } }
        };

        [TestCaseSource("CollectStatistics_DataProvider")]
        private void CollectStatistics_Test(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees, int[] quantityOfEachRelationship, int[] result)
        {
            Assert.That(result, Is.EqualTo(modules.CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees, quantityOfEachRelationship)));
        }

        private static object[] CreateComplianceMatrix_DataProvider =
        {
            new object[] { new List<int> { 1, 2, 3, 4, 5 },
                new List<int[]> { new int[2] { 0, 1 }, new int[2] { 1, 2 }, new int[2] { 2, 3 }, new int[2] { 3, 4 }, new int[2] { 4, 5 }, new int[2] { 5, 6 } } },
            new object[] { new List<int> { 5, 3, 6, 1, 2 },
                new List<int[]> { new int[2] { 0, 1 }, new int[2] { 5, 2 }, new int[2] { 3, 3 }, new int[2] { 6, 4 }, new int[2] { 1, 5 }, new int[2] { 2, 6 } } }
        };

        [TestCaseSource("CreateComplianceMatrix_DataProvider")]
        private void CreateComplianceMatrix_Test(List<int> existingRelationshipDegrees, List<int[]> result)
        {
            Assert.That(result, Is.EqualTo(modules.CreateComplianceMatrix(existingRelationshipDegrees)));
        }

        private static object[] TransformMatrix_DataProvider =
        {
            new object[] { generatedOutputMatrix, new List<int> { 9, 8, 3, 2, 1 },
                new float[][] { new float[] {6, 3, 4 }, new float[] {2, 6, 1 }, new float[] {5, 1, 6 } } }
        };

        [TestCaseSource("TransformMatrix_DataProvider")]
        private void TransformMatrix_Test(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees, float[][] result)
        {
            Assert.That(result, Is.EqualTo(modules.TransformMatrix(generatedOutputMatrix, existingRelationshipDegrees)));
        }

        [Test]
        private void IncreaseCurrentRelationshipCount_Test()
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
        private void RemoveImpossibleRelations_Test(int[] allPossibleRelationships, int[] currentPossibleRelationships, int[] result)
        {
            Assert.That(result, Is.EqualTo(modules.RemoveImpossibleRelations(allPossibleRelationships, currentPossibleRelationships)));
        }

        private static object[] FindAllExistingRelationshipDegrees_DataProvider =
        {
            new object[] { relationshipsMatrix, 0, new List<int> { 1, 6, 7, 2, 3 } },
            new object[] { relationshipsMatrix, 1, new List<int> { 7, 6, 1, 8, 9 } }
        };

        [TestCaseSource("FindAllExistingRelationshipDegrees_DataProvider")]
        private void FindAllExistingRelationshipDegrees_Test(int[,][] relationshipsMatrix, int numberOfProband, List<int> result)
        {
            Assert.That(result, Is.EqualTo(modules.FindAllExistingRelationshipDegrees(relationshipsMatrix, numberOfProband)));
        }

        [Test]
        private void OutputBuildLeftBottomPart_Test()
        {
            float[][] generatedOutputMatrix = new float[][]
            {
                new float[] { 1, 8, 3 },
                new float[] { 0, 1, 0 },
                new float[] { 0, 0, 1 }
            };
            int[,][] relationshipsMatrix = new int[3, 3][]
            {
                { new int[] { 1 }, new int[] { 8 }, new int[] { 3 } },
                { new int[] { 7 }, new int[] { 1 }, new int[] { 5 } },
                { new int[] { 2 }, new int[] { 4 }, new int[] { 1 } }
            };

            float[][] model = new float[][]
            {
                new float[] { 1, 8, 3 },
                new float[] { 7, 1, 0 },
                new float[] { 2, 0, 1 }
            };

            Assert.That(model,
                Is.EqualTo(modules.BuildLeftBottomPart(generatedOutputMatrix: generatedOutputMatrix, relationshipsMatrix: relationshipsMatrix, numberOfProband: 0)));
        }

        private static object[] InputBuildLeftBottomPart_DataProvider =
        {
            new object[] { generatedInputMatrix,
                new float[][] { new float[] { 6800, 3200, 1800 }, new float[] { 3200, 6800, 0 }, new float[] { 1800, 0, 6800 } } }
        };

        [TestCaseSource("InputBuildLeftBottomPart_DataProvider")]
        private void InputBuildLeftBottomPart_Test(float[][] generatedInputMatrix, float[][] result)
        {
            Assert.That(result, Is.EqualTo(modules.BuildLeftBottomPart(generatedInputMatrix)));
        }

        [Test]
        private void FillMainDiagonal_Test()
        {
            float[][] generatedOutputMatrix = new float[][]
            {
                new float[] { 0, 8, 3 },
                new float[] { 9, 0, 0 },
                new float[] { 2, 0, 0 }
            };

            float[][] model = new float[][]
            {
                new float[] { 1, 8, 3 },
                new float[] { 9, 1, 0 },
                new float[] { 2, 0, 1 }
            };

            Assert.That(model, Is.EqualTo(modules.FillMainDiagonal(generatedOutputMatrix: generatedOutputMatrix)));
        }
    }
}
