using NUnit.Framework;
using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    [TestFixture]
    internal class IntegrationsTests
    {
        private static readonly object[] CalculatePercentOfMeaningfulValues_DataProvider =
        {
            new object[]
            {
                3,
                new List<int>
                {
                    0, 3, 8
                },
                new float[][] {new float[] {0, 8, 3}, new float[] {0, 0, 0}, new float[] {0, 0, 0}},
                100 * (double) 2 / 9
            },
            new object[]
            {
                3,
                new List<int>
                {
                    3, 8
                },
                new float[][] {new float[] {0, 8, 3}, new float[] {0, 0, 0}, new float[] {0, 0, 0}},
                100 * (double) 2 / 9
            },
            new object[]
            {
                3,
                new List<int>
                {
                    8
                },
                new float[][] {new float[] {0, 8, 3}, new float[] {0, 0, 0}, new float[] {0, 0, 0}},
                100 * (double) 1 / 9
            }
        };

        [TestCaseSource(nameof(CalculatePercentOfMeaningfulValues_DataProvider))]
        public void CalculatePercentOfMeaningfulValues_Test(int generatedMatrixSize,
            List<int> existingRelationshipDegrees, float[][] generatedOutputMatrix, double result)
        {
            Assert.That(
                Integrations.CalculatePercentOfMeaningfulValues(generatedMatrixSize, existingRelationshipDegrees,
                    generatedOutputMatrix),
                Is.EqualTo(result));
        }

        private static readonly object[] FindAllPossibleRelationships_DataProvider =
        {
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 11, 6, 6},
                    new float[] {0, 0, 8, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 10, 11, 5},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {12}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 10, 11, 11, 11},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 11, 11},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                1,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 11, 6, 5},
                    new float[] {0, 0, 0, 0, 5},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {2, 6}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 10, 11, 10},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {12}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 10, 11, 11},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {12}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 10, 10, 6},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 4, 7},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                1,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 9, 5, 9, 2},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 2, 3},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                2,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 10, 11, 2, 5},
                    new float[] {0, 0, 12, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {2}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 9, 5, 11},
                    new float[] {0, 0, 0, 2, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                1,
                new int[] {13}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 5, 9, 9},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 2},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                2,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 9, 6, 5, 2},
                    new float[] {0, 0, 8, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {3}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 11, 9, 5},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 9, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                1,
                new int[] {5}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 9, 5, 6, 11},
                    new float[] {0, 0, 3, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                2,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 9, 5, 2},
                    new float[] {0, 0, 5, 0, 0},
                    new float[] {0, 0, 0, 3, 8},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                1,
                new int[] {2}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 6, 11, 10},
                    new float[] {0, 0, 0, 7, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                1,
                new int[] {6}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 10, 10, 6, 11},
                    new float[] {0, 0, 4, 0, 0},
                    new float[] {0, 0, 0, 7, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                1,
                new int[] {7}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 6, 6, 10},
                    new float[] {0, 0, 4, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                1,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 6, 5, 9},
                    new float[] {0, 0, 6, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 9, 5, 11},
                    new float[] {0, 0, 5, 0, 0},
                    new float[] {0, 0, 0, 3, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                1,
                new int[] {2}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 10, 10, 10},
                    new float[] {0, 0, 6, 0, 0},
                    new float[] {0, 0, 0, 4, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                1,
                new int[] {6}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 10, 11, 6, 10},
                    new float[] {0, 0, 12, 0, 0},
                    new float[] {0, 0, 0, 6, 10},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 2, 1, 3, 4},
                2,
                new List<int> {2, 3, 4},
                1,
                new int[] {7}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 2, 5, 5},
                    new float[] {0, 0, 4, 2, 0},
                    null,
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                1,
                new List<int> {2, 3, 4},
                2,
                new int[] {2, 0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 9, 2, 9, 2},
                    new float[] {0, 0, 8, 0, 0},
                    null,
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                1,
                new List<int> {2, 3, 4},
                2,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 10, 2, 2},
                    new float[] {0, 0, 6, 4, 0},
                    null,
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                1,
                new List<int> {2, 3, 4},
                2,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 10, 6, 2, 2},
                    new float[] {0, 0, 7, 7, 0},
                    null,
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                1,
                new List<int> {2, 3, 4},
                2,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 6, 6},
                    new float[] {0, 0, 0, 4},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 6, 3},
                    new float[] {0, 0, 0, 12},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {12}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 6, 2},
                    new float[] {0, 0, 0, 4},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 3, 6},
                    new float[] {0, 0, 12, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {10}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 2, 6},
                    new float[] {0, 0, 4, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 3, 2},
                    new float[] {0, 0, 12, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {5}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 8, 3},
                    new float[] {0, 0, 18, 12},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {2, 6}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 2, 3},
                    new float[] {0, 0, 0, 12},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {8}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 2, 5},
                    new float[] {0, 0, 0, 2},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 6, 10, 6},
                    new float[] {0, 0, 2, 0, 6},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                1,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 6, 9, 6},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                1,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 6, 11, 10},
                    new float[] {0, 0, 0, 7, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 10, 6, 11, 9},
                    new float[] {0, 0, 0, 0, 2},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {3, 7}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 9, 9, 11, 6},
                    new float[] {0, 0, 0, 0, 8},
                    new float[] {0, 0, 0, 13, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 11, 9, 5},
                    new float[] {0, 0, 4, 0, 5},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                1,
                new int[] {5}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 10, 9, 6},
                    new float[] {0, 0, 0, 5, 4},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 2, 2, 10},
                    new float[] {0, 0, 6, 0, 0},
                    null,
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                1,
                new List<int> {2, 3, 4},
                1,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 10, 2, 9},
                    new float[] {0, 0, 4, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                1,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 11, 9, 5},
                    new float[] {0, 0, 11, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 5, 9, 6},
                    new float[] {0, 0, 0, 5, 0},
                    new float[] {0, 0, 0, 0, 3},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 10, 5, 9},
                    new float[] {0, 0, 0, 0, 5},
                    new float[] {0, 0, 0, 4, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                1,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 5, 6, 10},
                    new float[] {0, 0, 2, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 6, 11, 6},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 3, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                1,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 9, 6, 11, 2},
                    new float[] {0, 0, 8, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 11, 10, 6},
                    new float[] {0, 0, 4, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                2,
                new List<int> {3, 4},
                1,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 11, 17},
                    new float[] {0, 0, 3, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 6, 17},
                    new float[] {0, 0, 2, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 6, 11},
                    new float[] {0, 0, 2, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 17, 11, 17},
                    new float[] {0, 0, 2, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 17, 11, 11},
                    new float[] {0, 0, 2, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 2, 5},
                    new float[] {0, 1, 0, 2},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 2, 5, 9},
                    new float[] {0, 1, 0, 5},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 2, 2, 5},
                    new float[] {0, 1, 0, 2},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 2, 5, 2},
                    new float[] {0, 1, 2, 0},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 2, 2, 6},
                    new float[] {0, 1, 0, 4},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 5, 5, 9, 5},
                    new float[] {0, 0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4, 5},
                4,
                new List<int> {5},
                0,
                new int[] {3}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 6, 2},
                    new float[] {0, 0, 0, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 5, 5, 9},
                    new float[] {0, 0, 2, 2, 5},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {1, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {2}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 6, 11, 6},
                    new float[] {0, 0, 2, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {2, 6}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 2, 2, 5},
                    new float[] {0, 0, 3, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {2}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 5, 5, 2},
                    new float[] {0, 0, 2, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {3}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 6, 9, 5},
                    new float[] {0, 0, 3, 0, 0},
                    new float[] {0, 0, 0, 5, 2},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {3}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 6, 5},
                    new float[] {0, 0, 3, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {2, 0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 5, 6},
                    new float[] {0, 0, 0, 3},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {3, 0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 17, 17},
                    new float[] {0, 0, 0, 3},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 17, 23},
                    new float[] {0, 0, 3, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 5, 4},
                    new float[] {0, 0, 0, 8},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {8}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 5, 5, 5},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 9, 5, 5},
                    new float[] {0, 0, 5, 2, 2},
                    new float[] {0, 0, 0, 0, 3},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 9, 5, 5},
                    new float[] {0, 0, 5, 2, 2},
                    new float[] {0, 0, 0, 3, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 5, 5, 5, 5, 9},
                    new float[] {0, 0, 0, 0, 2, 2, 5},
                    new float[] {0, 0, 0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4, 5, 6},
                5,
                new List<int> {6},
                0,
                new int[] {2}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 5, 6, 5, 9},
                    new float[] {0, 0, 2, 4, 2, 5},
                    new float[] {0, 0, 0, 3, 0, 0},
                    new float[] {0, 0, 0, 0, 2, 5},
                    new float[] {0, 0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4, 5},
                4,
                new List<int> {5},
                0,
                new int[] {2}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 5, 6, 5, 9},
                    new float[] {0, 0, 2, 4, 2, 5},
                    new float[] {0, 0, 0, 3, 0, 2},
                    new float[] {0, 0, 0, 0, 2, 5},
                    new float[] {0, 0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4, 5},
                4,
                new List<int> {5},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 4, 5, 5, 9},
                    new float[] {0, 0, 5, 5, 9},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {2, 0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 6, 11, 6, 11},
                    new float[] {0, 0, 2, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 2, 4},
                    new float[] {0, 0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4, 5},
                4,
                new List<int> {5},
                0,
                new int[] {3}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 5, 6, 5},
                    new float[] {0, 0, 2, 4, 2},
                    new float[] {0, 0, 0, 3, 0},
                    new float[] {0, 0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3, 4},
                3,
                new List<int> {4},
                0,
                new int[] {2}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 5, 5},
                    new float[] {0, 0, 0, 0},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 5, 5},
                    new float[] {0, 0, 0, 0},
                    new float[] {0, 0, 0, 0},
                    new float[] {0, 0, 0, 0}
                },
                new int[][]
                {
                    new int[] {0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 3, 2},
                3,
                new List<int> {3},
                0,
                new int[] {0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 7, 4, 7},
                    new float[] {0, 0, 2, 11},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {7}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 5, 6, 6},
                    new float[] {0, 0, 3, 3},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 4, 6, 11},
                    new float[] {0, 0, 6, 11},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {3, 7, 0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 11, 6, 4},
                    new float[] {0, 0, 2, 11},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {7}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 7, 11, 17},
                    new float[] {0, 0, 16, 23},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {3, 7, 17, 0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 17, 4, 7},
                    new float[] {0, 0, 16, 23},
                    new float[] {0, 0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {3, 7}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 23, 16},
                    new float[] {0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2},
                1,
                new List<int> {2},
                0,
                new int[] {2, 6, 16, 0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 4, 7, 7},
                    new float[] {0, 1, 3, 3},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 5, 2},
                    new float[] {0, 1, 2, 4},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {3}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 49, 30},
                    new float[] {0, 1, 0},
                    new float[] {0, 0, 1}
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2},
                1,
                new List<int> {2},
                0,
                new int[] {5, 10, 22, 38, 0}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 2, 5},
                    new float[] {0, 1, 4, 2},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {2}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 2, 3},
                    new float[] {0, 1, 4, 12},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {8}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 8, 3},
                    new float[] {0, 1, 18, 12},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {2, 6}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 3, 2},
                    new float[] {0, 1, 12, 4},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {5}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 2, 6},
                    new float[] {0, 1, 0, 0},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 2, 6},
                    new float[] {0, 1, 4, 4},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 3, 6},
                    new float[] {0, 1, 12, 4},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {10}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 6, 2},
                    new float[] {0, 1, 0, 0},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 6, 2},
                    new float[] {0, 1, 4, 4},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 6, 3},
                    new float[] {0, 1, 4, 12},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {12}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 6, 6},
                    new float[] {0, 1, 0, 0},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {4}
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {1, 6, 6, 6},
                    new float[] {0, 1, 4, 4},
                    new float[] {0, 0, 1, 0},
                    new float[] {0, 0, 0, 1}
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },
                new List<int> {0, 1, 2, 3},
                2,
                new List<int> {3},
                0,
                new int[] {4}
            }
        };

        [TestCaseSource(nameof(FindAllPossibleRelationships_DataProvider))]
        public void FindAllPossibleRelationships_Test(float[][] generatedOutputMatrix, int[][] currentCountMatrix,
            List<int> persons, int person, List<int> relatives, int relative, int[] result)
        {
            int[,][] relationshipsMatrix =
                FileSaverLoader.LoadFromFile2DJagged(TestContext.CurrentContext.TestDirectory + "\\relationships.csv");
            int numberOfProband = 0;
            int[][] ancestorsMaxCountMatrix =
                FileSaverLoader.LoadFromFile2D(TestContext.CurrentContext.TestDirectory + "\\ancestorsMatrix.csv");
            int[][] descendantsMatrix =
                FileSaverLoader.LoadFromFile2D(TestContext.CurrentContext.TestDirectory + "\\descendantsMatrix.csv");

            string firstRow = "";

            foreach (float column in generatedOutputMatrix[0])
            {
                firstRow += column + ", ";
            }

            Assert.That(
                Integrations.FindAllPossibleRelationships(
                    generatedOutputMatrix,
                    persons, person,
                    relatives, relative,
                    relationshipsMatrix, numberOfProband,
                    ancestorsMaxCountMatrix, descendantsMatrix,
                    currentCountMatrix),
                Is.EquivalentTo(result),
                "First row: " + firstRow);
        }

        private static readonly object[] DetectAllPossibleRelationships_DataProvider =
        {
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1},
                0,
                new List<int> {1},
                0,
                new int[]
                {
                    2, 5, 6, 9, 10, 11, 14, 15, 16, 17, 20, 21, 22, 23, 24, 27, 28, 29, 30, 31, 32, 35, 36, 37, 38, 39,
                    40, 41, 44, 45, 46, 47, 48, 49, 50, 51
                }
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 6, 5},
                    new float[] {0, 0, 0},
                    null
                },
                new int[][]
                {
                    new int[] {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null
                },
                new List<int> {0, 1, 2},
                1,
                new List<int> {2},
                0,
                new int[]
                {
                    2, 0
                }
            },
            new object[]
            {
                new float[][]
                {
                    new float[] {0, 2, 2, 0},
                    null,
                    null,
                    null
                },
                new int[][]
                {
                    new int[] {2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    null,
                    null,
                    null
                },
                new List<int> {0, 1, 2, 3},
                0,
                new List<int> {1, 2, 3},
                2,
                new int[]
                {
                    5, 6, 9, 10, 11, 14, 15, 16, 17, 20, 21, 22, 23, 24, 27, 28, 29, 30, 31, 32, 35, 36, 37, 38, 39, 40,
                    41, 44, 45, 46, 47, 48, 49, 50, 51
                }
            }
        };

        [TestCaseSource(nameof(DetectAllPossibleRelationships_DataProvider))]
        public void DetectAllPossibleRelationships_Test(float[][] generatedOutputMatrix, int[][] currentCountMatrix,
            List<int> persons, int person, List<int> relatives, int relative, int[] result)
        {
            int[,][] relationshipsMatrix =
                FileSaverLoader.LoadFromFile2DJagged(TestContext.CurrentContext.TestDirectory + "\\relationships.csv");
            int numberOfProband = 0;
            int[][] ancestorsMaxCountMatrix =
                FileSaverLoader.LoadFromFile2D(TestContext.CurrentContext.TestDirectory + "\\ancestorsMatrix.csv");
            int[][] descendantsMatrix =
                FileSaverLoader.LoadFromFile2D(TestContext.CurrentContext.TestDirectory + "\\descendantsMatrix.csv");

            Assert.That(
                Integrations.DetectAllPossibleRelationships(
                    relationshipsMatrix, numberOfProband,
                    ancestorsMaxCountMatrix, descendantsMatrix,
                    generatedOutputMatrix, currentCountMatrix,
                    persons, person,
                    relatives, relative),
                Is.EquivalentTo(result));
        }
    }
}