using NUnit.Framework;
using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    [TestFixture]
    internal class IntegrationsTests
    {
        [SetUp]
        public void DerivedSetUp()
        {
            Form1.GetStaticData();
        }

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
                Form1.GetPercentOfMeaningfulValues(generatedMatrixSize, existingRelationshipDegrees,
                    generatedOutputMatrix),
                Is.EqualTo(result));
        }        
    }
}