using NUnit.Framework;
using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    [TestFixture]
    class IntegrationsTests
    {
        Integrations integrations = new Integrations();
        Modules modules = new Modules();
        FileSaverLoader fileSaverLoader = new FileSaverLoader();

        private static object[] CalculatePercentOfMeaningfulValues_DataProvider =
        {
            new object[]
            {
                3,
                new List<int> { 8, 3 },
                new float[][] { new float[] { 0, 8, 3 }, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 } },
                100 * (double)2 / 9
            },
            new object[]
            {
                3,
                new List<int> { 8 },
                new float[][] { new float[] { 0, 8, 3 }, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 } },
                100 * (double)1 / 9
            }
        };

        [TestCaseSource("CalculatePercentOfMeaningfulValues_DataProvider")]
        public void CalculatePercentOfMeaningfulValues_Test(int generatedMatrixSize, List<int> existingRelationshipDegrees, float[][] generatedOutputMatrix, double result)
        {
            Assert.That(result, Is.EqualTo(integrations.CalculatePercentOfMeaningfulValues(generatedMatrixSize, existingRelationshipDegrees, generatedOutputMatrix)));
        }

        private static object[] FindAllPossibleRelationships_DataProvider =
        {
            new object[]
            {
                new float[][] {
                    new float[] { 0, 6, 5, 6, 5, 9 },
                    new float[] { 0, 0, 2, 4, 2, 5 },
                    new float[] { 0, 0, 0, 3, 0, 0 },
                    new float[] { 0, 0, 0, 0, 2, 5 },
                    new float[] { 0, 0, 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    null },
                new List<int> { 0, 1, 2, 3, 4, 5 },
                4,
                new List<int> { 5 },
                0,
                new int[] { 2 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 6, 5, 6, 5, 9 },
                    new float[] { 0, 0, 2, 4, 2, 5 },
                    new float[] { 0, 0, 0, 3, 0, 2 },
                    new float[] { 0, 0, 0, 0, 2, 5 },
                    new float[] { 0, 0, 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    null },
                new List<int> { 0, 1, 2, 3, 4, 5 },
                4,
                new List<int> { 5 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 4, 5, 5, 9 },
                    new float[] { 0, 0, 5, 5, 9 },
                    new float[] { 0, 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3, 4 },
                3,
                new List<int> { 4 },
                0,
                new int[] { 2, 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 11, 6, 11, 6, 11 },
                    new float[] { 0, 0, 2, 0, 0, 0 },
                    new float[] { 0, 0, 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 0, 2, 4 },
                    new float[] { 0, 0, 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3, 4, 5 },
                4,
                new List<int> { 5 },
                0,
                new int[] { 3 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 11, 6, 11, 6, 11 },
                    new float[] { 0, 0, 2, 0, 0, 4 },
                    new float[] { 0, 0, 0, 0, 0, 3 },
                    new float[] { 0, 0, 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3, 4, 5 },
                4,
                new List<int> { 5 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 6, 5, 6, 5 },
                    new float[] { 0, 0, 2, 4, 2 },
                    new float[] { 0, 0, 0, 3, 0 },
                    new float[] { 0, 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3, 4 },
                3,
                new List<int> { 4 },
                0,
                new int[] { 2 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 11, 6, 11, 6 },
                    new float[] { 0, 0, 2, 0, 0 },
                    new float[] { 0, 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3, 4 },
                3,
                new List<int> { 4 },
                0,
                new int[] { 2, 6, 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 5, 5, 5 },
                    new float[] { 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 5, 5, 5 },
                    new float[] { 0, 0, 0, 0 },
                    null,
                    new float[] { 0, 0, 0, 0 } },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 3, 2 },
                3,
                new List<int> { 3 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 7, 4, 7 },
                    new float[] { 0, 0, 2, 11 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 7 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 7, 4, 7 },
                    new float[] { 0, 0, 2, 11 },
                    null,
                    new float[] { 0, 0, 0, 0 } },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 3, 2 },
                3,
                new List<int> { 3 },
                0,
                new int[] { 7 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 5, 5, 6 },
                    new float[] { 0, 0, 0, 3 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 5, 5, 6 },
                    new float[] { 0, 0, 0, 3 },
                    null,
                    new float[] { 0, 0, 0, 0 } },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 3, 2 },
                3,
                new List<int> { 3 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 5, 6, 5 },
                    new float[] { 0, 0, 3, 0 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 5, 6, 6 },
                    new float[] { 0, 0, 3, 3 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 4 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 4, 6, 11 },
                    new float[] { 0, 0, 6, 11 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 3, 7, 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 11, 6, 4 },
                    new float[] { 0, 0, 2, 11 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 7 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 11, 23, 17 },
                    new float[] { 0, 0, 0, 3 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 11, 17, 23 },
                    new float[] { 0, 0, 3, 0 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 7, 11, 17 },
                    new float[] { 0, 0, 16, 23 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 3, 7, 17, 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 17, 4, 7 },
                    new float[] { 0, 0, 16, 23 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 3, 7 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 17, 11, 17 },
                    new float[] { 0, 0, 2, 0 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 11, 17, 17 },
                    new float[] { 0, 0, 0, 3 },
                    new float[] { 0, 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 0, 23, 16 },
                    new float[] { 0, 0, 0 },
                    null },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2 },
                1,
                new List<int> { 2 },
                0,
                new int[] { 2, 6, 16, 0 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 1, 4, 7, 7 },
                    new float[] { 0, 1, 3, 3 },
                    new float[] { 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 1 } },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 4 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 1, 6, 5, 2 },
                    new float[] { 0, 1, 2, 4 },
                    new float[] { 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 1 } },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 3 }
            },
            new object[]
            {
                new float[][] {
                    new float[] { 1, 49, 30 },
                    new float[] { 0, 1, 0 },
                    new float[] { 0, 0, 1 } },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2 },
                1,
                new List<int> { 2 },
                0,
                new int[] { 5, 10, 22, 38, 0 }
            },  
            new object[]
            {
                new float[][] {
                    new float[] { 1, 2, 2, 5 },
                    new float[] { 0, 1, 0, 2 },
                    new float[] { 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 1 } },
                new int[][] {
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                new int[] { 0 }
            }
        };

        [TestCaseSource("FindAllPossibleRelationships_DataProvider")]
        public void FindAllPossibleRelationships_Test(float[][] generatedOutputMatrix, int[][] currentCountMatrix, List<int> persons, int person, List<int> relatives, int relative, int[] result)
        {
            int[,][] relationshipsMatrix = fileSaverLoader.LoadFromFile2DJagged(TestContext.CurrentContext.TestDirectory + "\\relationships.csv");
            int numberOfProband = modules.FindNumberOfProband(relationshipsMatrix);
            int[][] maxCountMatrix = fileSaverLoader.LoadFromFile2D(TestContext.CurrentContext.TestDirectory + "\\maxCount.csv");

            Assert.That(result, Is.EqualTo(integrations.FindAllPossibleRelationships(generatedOutputMatrix, persons, person, relatives, relative, relationshipsMatrix, numberOfProband, maxCountMatrix, currentCountMatrix)));
        }
    }
}
