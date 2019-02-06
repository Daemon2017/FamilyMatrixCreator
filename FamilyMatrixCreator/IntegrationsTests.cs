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

        private static object[] FindAllPossibleRelationships_DataProvider =
        {
            new object[]
            {
                new float[][] { new float[] { 1, 4, 7, 7 }, new float[] { 0, 1, 3, 3 }, new float[] { 0, 0, 1, 0 }, new float[] { 0, 0, 0, 1 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                3,
                new int[] { 4 }
            },
            new object[]
            {
                new float[][] { new float[] { 1, 6, 5, 2 }, new float[] { 0, 1, 2, 4 }, new float[] { 0, 0, 1, 0 }, new float[] { 0, 0, 0, 1 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                3,
                new int[] { 3 }
            },
            new object[]
            {
                new float[][] { new float[] { 1, 49, 30 }, new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 } },
                new List<int> { 0, 1, 2 },
                1,
                new List<int> { 2 },
                0,
                3,
                new int[] { 5, 10, 22, 38, 0 }
            },
            new object[]
            {
                new float[][] { new float[] { 1, 2, 2, 5 }, new float[] { 0, 1, 0, 2 }, new float[] { 0, 0, 1, 0 }, new float[] { 0, 0, 0, 1 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                3,
                new int[] { 0 }
            },
            new object[]
            {
                new float[][] { new float[] { 1, 7, 6, 2 }, new float[] { 0, 1, 2, 0 }, new float[] { 0, 0, 1, 0 }, new float[] { 0, 0, 0, 1 } },
                new List<int> { 0, 1, 2, 3 },
                2,
                new List<int> { 3 },
                0,
                3,
                new int[] { 0 }
            }
        };

        [TestCaseSource("FindAllPossibleRelationships_DataProvider")]
        public void FindAllPossibleRelationships_Test(float[][] generatedOutputMatrix, List<int> persons, int person, List<int> relatives, int relative, int numberOfProband, int[] result)
        {
            int[,][] relationshipsMatrix = fileSaverLoader.LoadFromFile2DJagged(TestContext.CurrentContext.TestDirectory + "\\relationships.csv");

            Assert.That(result, Is.EqualTo(integrations.FindAllPossibleRelationships(generatedOutputMatrix, persons, person, relatives, relative, relationshipsMatrix, numberOfProband)));
        }

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
    }
}
