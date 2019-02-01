using NUnit.Framework;
using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    [TestFixture]
    class IntegrationsTests
    {
        Integrations integrations = new Integrations();

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
