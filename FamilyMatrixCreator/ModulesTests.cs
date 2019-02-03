using NUnit.Framework;
using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    [TestFixture]
    public class ModulesTests
    {
        Modules modules = new Modules();
        FileSaverLoader fileSaverLoader = new FileSaverLoader();

        private static object[] MaxNumberOfThisRelationshipTypeIsNotExceeded_DataProvider =
        {
            new object[]
            {
                1,
                new int[][] { new int[]{ 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2 },
                2,
                true
            },
            new object[]
            {
                5,
                new int[][] { new int[]{ 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2 },
                1,
                false
            },
            new object[]
            {
                2,
                new int[][] { new int[]{ 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2 },
                2,
                true
            },
            new object[]
            {
                2,
                new int[][] { new int[]{ 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2 },
                0,
                false
            }
        };

        [TestCaseSource("MaxNumberOfThisRelationshipTypeIsNotExceeded_DataProvider")]
        public void MaxNumberOfThisRelationshipTypeIsNotExceeded_Test(int relationship, int[][] currentCountMatrix, List<int> persons, int person, bool result)
        {
            int[][] maxCountMatrix = fileSaverLoader.LoadFromFile2D(TestContext.CurrentContext.TestDirectory + "\\maxCount.csv");

            Assert.That(result, Is.EqualTo(modules.MaxNumberOfThisRelationshipTypeIsNotExceeded(relationship, currentCountMatrix, persons, person, maxCountMatrix)));
        }

        private static object[] CollectStatistics_DataProvider =
        {
            new object[]
            {
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 9, 1, 0 }, new float[] { 2, 0, 1 } },
                new List<int> { 9, 8, 3, 2, 1 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 1, 1, 1, 1, 3 }
            },
            new object[]
            {
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 9, 1, 0 }, new float[] { 2, 0, 1 } },
                new List<int> { 9, 8, 3, 2, 1 },
                new int[] { 1, 2, 3, 4, 5 },
                new int[] { 2, 3, 4, 5, 8 }
            },
            new object[]
            {
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 } },
                new List<int> { 9, 8, 3, 2, 1 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 1, 1, 0, 3 }
            },
            new object[]
            {
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 } },
                new List<int> { 9, 8, 3, 2, 1 },
                new int[] { 1, 2, 3, 4, 5 },
                new int[] { 1, 3, 4, 4, 8 }
            },
            new object[]
            {
                new float[][] { new float[] { 0, 8, 3 }, new float[] { 9, 0, 0 }, new float[] { 2, 0, 0 } },
                new List<int> { 9, 8, 3, 2, 1 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 1, 1, 1, 1, 0 }
            },
            new object[]
            {
                new float[][] { new float[] { 0, 8, 3 }, new float[] { 9, 0, 0 }, new float[] { 2, 0, 0 } },
                new List<int> { 9, 8, 3, 2, 1 },
                new int[] { 1, 2, 3, 4, 5 },
                new int[] { 2, 3, 4, 5, 5 }
            }
        };

        [TestCaseSource("CollectStatistics_DataProvider")]
        public void CollectStatistics_Test(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees, int[] quantityOfEachRelationship, int[] result)
        {
            Assert.That(result, Is.EqualTo(modules.CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees, quantityOfEachRelationship)));
        }

        private static object[] CreateComplianceMatrix_DataProvider =
        {
            new object[]
            {
                new List<int> { 1, 2, 3, 4, 5 },
                new List<int[]> { new int[2] { 0, 1 }, new int[2] { 1, 2 }, new int[2] { 2, 3 }, new int[2] { 3, 4 }, new int[2] { 4, 5 }, new int[2] { 5, 6 } }
            },
            new object[]
            {
                new List<int> { 5, 3, 6, 1, 2 },
                new List<int[]> { new int[2] { 0, 1 }, new int[2] { 5, 2 }, new int[2] { 3, 3 }, new int[2] { 6, 4 }, new int[2] { 1, 5 }, new int[2] { 2, 6 } }
            }
        };

        [TestCaseSource("CreateComplianceMatrix_DataProvider")]
        public void CreateComplianceMatrix_Test(List<int> existingRelationshipDegrees, List<int[]> result)
        {
            Assert.That(result, Is.EqualTo(modules.CreateComplianceMatrix(existingRelationshipDegrees)));
        }

        private static object[] TransformMatrix_DataProvider =
        {
            new object[]
            {
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 9, 1, 0 }, new float[] { 2, 0, 1 } },
                new List<int> { 9, 8, 3, 2, 1 },
                new float[][] { new float[] {6, 3, 4 }, new float[] {2, 6, 1 }, new float[] {5, 1, 6 } }
            },
            new object[]
            {
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 } },
                new List<int> { 9, 8, 3, 2, 1 },
                new float[][] { new float[] {6, 3, 4 }, new float[] {1, 6, 1 }, new float[] {1, 1, 6 } }
            },
            new object[]
            {
                new float[][] { new float[] { 0, 8, 3 }, new float[] { 9, 0, 0 }, new float[] { 2, 0, 0 } },
                new List<int> { 9, 8, 3, 2, 1 },
                new float[][] { new float[] {1, 3, 4 }, new float[] {2, 1, 1 }, new float[] {5, 1, 1 } }
            }
        };

        [TestCaseSource("TransformMatrix_DataProvider")]
        public void TransformMatrix_Test(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees, float[][] result)
        {
            float[][] a = modules.TransformMatrix(generatedOutputMatrix, existingRelationshipDegrees);
            Assert.That(result, Is.EqualTo(a));
        }

        private static object[] IncreaseCurrentRelationshipCount_DataProvider =
        {
            new object[]
            {
                new float[][] { new float[] { 1, 2, 5 }, new float[] { 3, 1, 9 }, new float[] { 5, 10, 1 } },
                new int[][] { new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2 },
                0,
                new List<int> { 1, 2 },
                0,
                new int[][] { new int[]{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            },
            new object[]
            {
                new float[][] { new float[] { 1, 2, 5 }, new float[] { 3, 1, 9 }, new float[] { 5, 10, 1 } },
                new int[][] { new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2 },
                0,
                new List<int> { 1, 2 },
                1,
                new int[][] { new int[]{ 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            },
            new object[]
            {
                new float[][] { new float[] { 1, 2, 5 }, new float[] { 3, 1, 9 }, new float[] { 5, 10, 1 } },
                new int[][] { new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2 },
                1,
                new List<int> { 1, 2 },
                0,
                new int[][] { new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            },
            new object[]
            {
                new float[][] { new float[] { 1, 2, 5 }, new float[] { 3, 1, 9 }, new float[] { 5, 10, 1 } },
                new int[][] { new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                new List<int> { 0, 1, 2 },
                1,
                new List<int> { 1, 2 },
                1,
                new int[][] { new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            }
        };
        
        [TestCaseSource("IncreaseCurrentRelationshipCount_DataProvider")]
        public void IncreaseCurrentRelationshipCount_Test(float[][] generatedOutputMatrix, int[][] currentCountMatrix, List<int> persons, int person, List<int> relatives, int relative, int[][] result)
        {
            int[][] maxCountMatrix = fileSaverLoader.LoadFromFile2D(TestContext.CurrentContext.TestDirectory + "\\maxCount.csv");

            Assert.That(result, Is.EqualTo(modules.IncreaseCurrentRelationshipCount(generatedOutputMatrix, currentCountMatrix, persons, person, relatives, relative, maxCountMatrix)));
        }

        private static object[] RemoveImpossibleRelations_DataProvider =
        {
            new object[]
            {
                new int[] { 0, 1, 2, 3, 4 },
                new int[] { 2, 4, 5 },
                new int[] { 0, 2, 4 }
            },
            new object[]
            {
                new int[] { 0, 1, 2, 3, 4 },
                new int[] { 0 },
                new int[] { 0 }
            },
            new object[]
            {
                new int[] { 0 },
                new int[] { 2, 4, 5 },
                new int[] { 0 }
            }
        };

        [TestCaseSource("RemoveImpossibleRelations_DataProvider")]
        public void RemoveImpossibleRelations_Test(int[] allPossibleRelationships, int[] currentPossibleRelationships, int[] result)
        {
            Assert.That(result, Is.EqualTo(modules.RemoveImpossibleRelations(allPossibleRelationships, currentPossibleRelationships)));
        }

        private static object[] FindAllExistingRelationshipDegrees_DataProvider =
        {
            new object[]
            {
                3,
                new List<int> { 13, 9, 8, 5, 3, 2, 1, 18, 15, 12, 10, 7, 6, 4, 32, 29, 24, 22, 17, 16, 11, 50, 47, 40, 38, 31, 30, 23, 72, 69, 60, 58, 49, 48, 39, 98, 95, 84, 82, 71, 70, 59, 128, 125, 112, 110, 97, 96, 83 }
            }
        };

        [TestCaseSource("FindAllExistingRelationshipDegrees_DataProvider")]
        public void FindAllExistingRelationshipDegrees_Test(int numberOfProband, List<int> result)
        {
            int[,][] relationshipsMatrix = fileSaverLoader.LoadFromFile2DJagged(TestContext.CurrentContext.TestDirectory + "\\relationships.csv");

            Assert.That(result, Is.EqualTo(modules.FindAllExistingRelationshipDegrees(relationshipsMatrix, numberOfProband)));
        }

        private static object[] OutputBuildLeftBottomPart_DataProvider =
        {
            new object[]
            {
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 0, 1, 2 }, new float[] { 0, 0, 1 } },
                3,
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 5, 1, 2 }, new float[] { 2, 3, 1 } }
            },
            new object[]
            {
                new float[][] { new float[] { 0, 8, 3 }, new float[] { 0, 0, 2 }, new float[] { 0, 0, 0 } },
                3,
                new float[][] { new float[] { 0, 8, 3 }, new float[] { 5, 0, 2 }, new float[] { 2, 3, 0 } }
            },
            new object[]
            {
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 5, 1, 2 }, new float[] { 2, 3, 1 } },
                3,
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 5, 1, 2 }, new float[] { 2, 3, 1 } }
            }
        };

        [TestCaseSource("OutputBuildLeftBottomPart_DataProvider")]
        public void OutputBuildLeftBottomPart_Test(float[][] generatedOutputMatrix, int numberOfProband, float[][] result)
        {
            int[,][] relationshipsMatrix = fileSaverLoader.LoadFromFile2DJagged(TestContext.CurrentContext.TestDirectory + "\\relationships.csv");

            Assert.That(result, Is.EqualTo(modules.BuildLeftBottomPart(generatedOutputMatrix, relationshipsMatrix, numberOfProband)));
        }

        private static object[] InputBuildLeftBottomPart_DataProvider =
        {
            new object[]
            {
                new float[][] { new float[] { 6800, 3200, 1800 }, new float[] { 0, 6800, 0 }, new float[] { 0, 0, 6800 } },
                new float[][] { new float[] { 6800, 3200, 1800 }, new float[] { 3200, 6800, 0 }, new float[] { 1800, 0, 6800 } }
            }
        };

        [TestCaseSource("InputBuildLeftBottomPart_DataProvider")]
        public void InputBuildLeftBottomPart_Test(float[][] generatedInputMatrix, float[][] result)
        {
            Assert.That(result, Is.EqualTo(modules.BuildLeftBottomPart(generatedInputMatrix)));
        }

        private static object[] FillMainDiagonal_DataProvider =
        {
            new object[]
            {
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 9, 1, 0 }, new float[] { 2, 0, 1 } },
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 9, 1, 0 }, new float[] { 2, 0, 1 } }
            },
            new object[]
            {
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 } },
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 } }
            },
            new object[]
            {
                new float[][] { new float[] { 0, 8, 3 }, new float[] { 9, 0, 0 }, new float[] { 2, 0, 0 } },
                new float[][] { new float[] { 1, 8, 3 }, new float[] { 9, 1, 0 }, new float[] { 2, 0, 1 } }
            }
        };

        [TestCaseSource("FillMainDiagonal_DataProvider")]
        public void FillMainDiagonal_Test(float[][] generatedOutputMatrix, float[][] result)
        {
            Assert.That(result, Is.EqualTo(modules.FillMainDiagonal(generatedOutputMatrix)));
        }

        private static object[] GetNextRnd_DataProvider =
        {
            new object[]
            {
                5,
                10
            },
            new object[]
            {
                6,
                8
            },
            new object[]
            {
                7,
                7
            }
        };

        [TestCaseSource("GetNextRnd_DataProvider")]
        public void GetNextRnd_Test(int min, int max)
        {
            Assert.That(modules.GetNextRnd(min, max), Is.GreaterThan(min - 1).And.LessThan(max + 1));
        }
    }
}
