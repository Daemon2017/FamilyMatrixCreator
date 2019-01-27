using NUnit.Framework;
using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    [TestFixture]
    class ModulesTests
    {
        [TestCase]
        public void CollectStatisticsTest()
        {
            Modules modules = new Modules();

            float[][] generatedOutputMatrix = new float[][] { new float[] {1, 8, 3 },
                                                              new float[] {9, 1, 0 },
                                                              new float[] {2, 0, 1 }};
            
            Assert.That(new int[] { 1, 1, 1, 1, 3 }, 
                Is.EqualTo(modules.CollectStatistics(generatedOutputMatrix, new List<int> { 9, 8, 3, 2, 1 })));
        }

        [TestCase]
        public void CreateComplianceMatrixTest()
        {
            Modules modules = new Modules();

            List<int[]> model = new List<int[]> { new int[2] { 0, 1 },
                                                  new int[2] { 1, 2 },
                                                  new int[2] { 2, 3 },
                                                  new int[2] { 3, 4 },
                                                  new int[2] { 4, 5 },
                                                  new int[2] { 5, 6 }};
            
            Assert.That(model, 
                Is.EqualTo(modules.CreateComplianceMatrix(new List<int> { 1, 2, 3, 4, 5 })));
        }

        [TestCase]
        public void TransformMatrixTest()
        {
            Modules modules = new Modules();

            float[][] generatedOutputMatrix = new float[][] { new float[] {1, 8, 3 },
                                                              new float[] {9, 1, 0 },
                                                              new float[] {2, 0, 1 }};

            float[][] model = new float[][] { new float[] {6, 3, 4 },
                                              new float[] {2, 6, 1 },
                                              new float[] {5, 1, 6 }};
            
            Assert.That(model, 
                Is.EqualTo(modules.TransformMatrix(generatedOutputMatrix, new List<int> { 9, 8, 3, 2, 1 })));
        }

        [TestCase]
        public void RemoveImpossibleRelationsTest()
        {
            Modules modules = new Modules();

            Assert.That(new int[] { 0, 2, 4 }, 
                Is.EqualTo(modules.RemoveImpossibleRelations(new int[] { 0, 1, 2, 3, 4 }, new int[] { 2, 4, 5 })));
        }

        [TestCase]
        public void FillMainDiagonalTest()
        {
            Modules modules = new Modules();

            float[][] generatedOutputMatrix = new float[][] { new float[] {0, 8, 3 },
                                                              new float[] {9, 0, 0 },
                                                              new float[] {2, 0, 0 }};

            float[][] model = new float[][] { new float[] {1, 8, 3 },
                                              new float[] {9, 1, 0 },
                                              new float[] {2, 0, 1 }};
        
            Assert.That(model,
                Is.EqualTo(modules.FillMainDiagonal(generatedOutputMatrix)));
        }
    }
}
