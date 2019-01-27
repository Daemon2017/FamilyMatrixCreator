using NUnit.Framework;
using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    [TestFixture]
    class ModulesTests
    {
        [TestCase]
        public void MaxNumberOfThisRelationshipTypeIsNotExceededPositiveTest()
        {
            Modules modules = new Modules();

            int[][] currentCountMatrix = new int[][]
            {
                new int[]{ 2, 4, 8 },
                new int[]{ 1, 3, 7 },
                new int[]{ 0, 2, 6 },
                new int[]{ 0, 1, 5 },
                new int[]{ 0, 0, 4 }
            };
            int[][] maxCountMatrix = new int[][]
            {
                new int[]{ 0, 2 },
                new int[]{ 1, 4 },
                new int[]{ 2, 8 }
            };

            Assert.True(modules.MaxNumberOfThisRelationshipTypeIsNotExceeded(relationship: 1,
                                                                             currentCountMatrix: currentCountMatrix,
                                                                             persons: new List<int> { 0, 1, 2, 3, 4 },
                                                                             person: 2,
                                                                             maxCountMatrix: maxCountMatrix));
        }

        [TestCase]
        public void MaxNumberOfThisRelationshipTypeIsNotExceededNegativeTest()
        {
            Modules modules = new Modules();

            int[][] currentCountMatrix = new int[][]
            {
                new int[]{ 2, 4, 8 },
                new int[]{ 1, 3, 7 },
                new int[]{ 0, 2, 6 },
                new int[]{ 0, 1, 5 },
                new int[]{ 0, 0, 4 }
            };
            int[][] maxCountMatrix = new int[][]
            {
                new int[]{ 0, 2 },
                new int[]{ 1, 4 },
                new int[]{ 2, 8 }
            };

            Assert.False(modules.MaxNumberOfThisRelationshipTypeIsNotExceeded(relationship: 1,
                                                                              currentCountMatrix: currentCountMatrix,
                                                                              persons: new List<int> { 0, 1, 2, 3, 4 },
                                                                              person: 0,
                                                                              maxCountMatrix: maxCountMatrix));
        }

        [TestCase]
        public void CollectStatisticsTest()
        {
            Modules modules = new Modules();

            float[][] generatedOutputMatrix = new float[][]
            {
                new float[] { 1, 8, 3 },
                new float[] { 9, 1, 0 },
                new float[] { 2, 0, 1 }
            };

            Assert.That(new int[] { 2, 3, 4, 5, 8 },
                Is.EqualTo(modules.CollectStatistics(generatedOutputMatrix: generatedOutputMatrix,
                                                     existingRelationshipDegrees: new List<int> { 9, 8, 3, 2, 1 },
                                                     quantityOfEachRelationship: new int[] { 1, 2, 3, 4, 5 })));
        }

        [TestCase]
        public void CreateComplianceMatrixTest()
        {
            Modules modules = new Modules();

            List<int[]> model = new List<int[]>
            {
                new int[2] { 0, 1 },
                new int[2] { 1, 2 },
                new int[2] { 2, 3 },
                new int[2] { 3, 4 },
                new int[2] { 4, 5 },
                new int[2] { 5, 6 }
            };

            Assert.That(model,
                Is.EqualTo(modules.CreateComplianceMatrix(existingRelationshipDegrees: new List<int> { 1, 2, 3, 4, 5 })));
        }

        [TestCase]
        public void TransformMatrixTest()
        {
            Modules modules = new Modules();

            float[][] generatedOutputMatrix = new float[][]
            {
                new float[] {1, 8, 3 },
                new float[] {9, 1, 0 },
                new float[] {2, 0, 1 }
            };

            float[][] model = new float[][]
            {
                new float[] {6, 3, 4 },
                new float[] {2, 6, 1 },
                new float[] {5, 1, 6 }
            };

            Assert.That(model,
                Is.EqualTo(modules.TransformMatrix(generatedOutputMatrix: generatedOutputMatrix, existingRelationshipDegrees: new List<int> { 9, 8, 3, 2, 1 })));
        }

        [TestCase]
        public void IncreaseCurrentRelationshipCountTest()
        {
            Modules modules = new Modules();

            float[][] generatedOutputMatrix = new float[][]
            {
                new float[] { 1, 8, 3 },
                new float[] { 9, 1, 0 },
                new float[] { 2, 0, 1 }
            };
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

        [TestCase]
        public void RemoveImpossibleRelationsTest()
        {
            Modules modules = new Modules();

            Assert.That(new int[] { 0, 2, 4 },
                Is.EqualTo(modules.RemoveImpossibleRelations(allPossibleRelationships: new int[] { 0, 1, 2, 3, 4 },
                                                             currentPossibleRelationships: new int[] { 2, 4, 5 })));
        }

        [TestCase]
        public void FindAllExistingRelationshipDegreesTest()
        {
            Modules modules = new Modules();

            int[,][] relationshipsMatrix = new int[3, 3][]
            {
                { new int[] { 1 }, new int[] { 6 }, new int[] { 2 } },
                { new int[] { 7 }, new int[] { 1 }, new int[] { 8 } },
                { new int[] { 3 }, new int[] { 9 }, new int[] { 1 } }
            };

            Assert.That(new List<int> { 1, 6, 7, 2, 3 },
                Is.EqualTo(modules.FindAllExistingRelationshipDegrees(relationshipsMatrix: relationshipsMatrix, numberOfProband: 0)));
        }

        [TestCase]
        public void OutputBuildLeftBottomPartTest()
        {
            Modules modules = new Modules();

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

        [TestCase]
        public void InputBuildLeftBottomPartTest()
        {
            Modules modules = new Modules();

            float[][] generatedInputMatrix = new float[][]
            {
                new float[] { 6800, 3200, 1800 },
                new float[] { 0, 6800, 0 },
                new float[] { 0, 0, 6800 }
            };

            float[][] model = new float[][]
            {
                new float[] { 6800, 3200, 1800 },
                new float[] { 3200, 6800, 0 },
                new float[] { 1800, 0, 6800 }
            };

            Assert.That(model,
                Is.EqualTo(modules.BuildLeftBottomPart(generatedInputMatrix: generatedInputMatrix)));
        }

        [TestCase]
        public void FillMainDiagonalTest()
        {
            Modules modules = new Modules();

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

            Assert.That(model,
                Is.EqualTo(modules.FillMainDiagonal(generatedOutputMatrix: generatedOutputMatrix)));
        }
    }
}
