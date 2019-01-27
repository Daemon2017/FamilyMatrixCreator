using NUnit.Framework;
using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    [TestFixture]
    class ServiceTests
    {
        [TestCase]
        public void CollectStatisticsTest()
        {
            Form1 form1 = new Form1();

            float[][] generatedOutputMatrix = new float[][] { new float[] {1, 8, 3 },
                                                              new float[] {9, 1, 0 },
                                                              new float[] {2, 0, 1 }};
            
            int[] model = { 1, 1, 1, 1, 3 };

            int[] result = form1.CollectStatistics(generatedOutputMatrix, new List<int> { 9, 8, 3, 2, 1 });

            Assert.That(model, Is.EqualTo(result));
        }

        [TestCase]
        public void CreateComplianceMatrixTest()
        {
            Form1 form1 = new Form1();

            List<int[]> model = new List<int[]> { new int[2] { 0, 1 },
                                                  new int[2] { 1, 2 },
                                                  new int[2] { 2, 3 },
                                                  new int[2] { 3, 4 },
                                                  new int[2] { 4, 5 },
                                                  new int[2] { 5, 6 }};

            List<int[]> result = form1.CreateComplianceMatrix(new List<int> { 1, 2, 3, 4, 5 });

            Assert.That(model, Is.EqualTo(result));
        }

        [TestCase]
        public void TransformMatrixTest()
        {
            Form1 form1 = new Form1();

            float[][] generatedOutputMatrix = new float[][] { new float[] {1, 8, 3 },
                                                              new float[] {9, 1, 0 },
                                                              new float[] {2, 0, 1 }};

            float[][] model = new float[][] { new float[] {6, 3, 4 },
                                              new float[] {2, 6, 1 },
                                              new float[] {5, 1, 6 }};

            float[][] result = form1.TransformMatrix(generatedOutputMatrix, new List<int> { 9, 8, 3, 2, 1 });

            Assert.That(model, Is.EqualTo(result));
        }

        [TestCase]
        public void RemoveImpossibleRelationsTest()
        {
            Form1 form1 = new Form1();

            int[] model = new int[] { 0, 2, 4 };

            int[] result = form1.RemoveImpossibleRelations(new int[] { 0, 1, 2, 3, 4 }, new int[] { 2, 4, 5 });

            Assert.That(model, Is.EqualTo(result));
        }
    }
}
