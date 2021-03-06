﻿using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FamilyMatrixCreator
{
    [TestFixture]
    public class ModulesTests
    {
        [SetUp]
        public void DerivedSetUp()
        {
            Form1.GetStaticData();
        }

        private static readonly object[] CollectStatistics_DataProvider =
        {
            new object[]
            {
                new float[][] {new float[] {1, 8, 3}, new float[] {9, 1, 0}, new float[] {2, 0, 1}},
                new List<int> {9, 8, 3, 2, 1},
                new int[] {1, 1, 1, 1, 3}
            },
            new object[]
            {
                new float[][] {new float[] {1, 8, 3}, new float[] {0, 1, 0}, new float[] {0, 0, 1}},
                new List<int> {9, 8, 3, 2, 1},
                new int[] {0, 1, 1, 0, 3}
            },
            new object[]
            {
                new float[][] {new float[] {0, 8, 3}, new float[] {9, 0, 0}, new float[] {2, 0, 0}},
                new List<int> {9, 8, 3, 2, 1},
                new int[] {1, 1, 1, 1, 0}
            }
        };

        [TestCaseSource(nameof(CollectStatistics_DataProvider))]
        public void CollectStatistics_Test(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees, int[] result)
        {
            Assert.That(
                Form1.GetRelationshipStatistics(generatedOutputMatrix, existingRelationshipDegrees),
                Is.EqualTo(result));
        }
    
        private static readonly object[] FindAllExistingRelationshipDegrees_DataProvider =
        {
            new object[]
            {
                new List<int>
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26,
                    27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51,
                    52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73,
                    74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89
                }
            }
        };

        [TestCaseSource(nameof(FindAllExistingRelationshipDegrees_DataProvider))]
        public void FindAllExistingRelationshipDegrees_Test(List<int> result)
        {            
            Assert.That(Form1.GetAllExistingRelationshipDegrees(),
                Is.EquivalentTo(result));
        }

        private static readonly object[] OutputBuildLeftBottomPart_DataProvider =
        {
            new object[]
            {
                new float[][] {new float[] {1, 8, 3}, new float[] {0, 1, 2}, new float[] {0, 0, 1}},
                new float[][] {new float[] {1, 8, 3}, new float[] {5, 1, 2}, new float[] {2, 3, 1}}
            },
            new object[]
            {
                new float[][] {new float[] {0, 8, 3}, new float[] {0, 0, 2}, new float[] {0, 0, 0}},
                new float[][] {new float[] {0, 8, 3}, new float[] {5, 0, 2}, new float[] {2, 3, 0}}
            },
            new object[]
            {
                new float[][] {new float[] {1, 8, 3}, new float[] {5, 1, 2}, new float[] {2, 3, 1}},
                new float[][] {new float[] {1, 8, 3}, new float[] {5, 1, 2}, new float[] {2, 3, 1}}
            }
        };

        [TestCaseSource(nameof(OutputBuildLeftBottomPart_DataProvider))]
        public void OutputBuildLeftBottomPart_Test(float[][] generatedOutputMatrix, float[][] result)
        {
            Assert.That(Form1.GetLeftBottomPartOfOutputMatrix(generatedOutputMatrix),
                Is.EqualTo(result));
        }

        private static readonly object[] InputBuildLeftBottomPart_DataProvider =
        {
            new object[]
            {
                new float[][] {new float[] {6800, 3200, 1800}, new float[] {0, 6800, 0}, new float[] {0, 0, 6800}},
                new float[][] {new float[] {6800, 3200, 1800}, new float[] {3200, 6800, 0}, new float[] {1800, 0, 6800}}
            }
        };

        [TestCaseSource(nameof(InputBuildLeftBottomPart_DataProvider))]
        public void InputBuildLeftBottomPart_Test(float[][] generatedInputMatrix, float[][] result)
        {
            Assert.That(Form1.GetLeftBottomPartOfInputMatrix(generatedInputMatrix), Is.EqualTo(result));
        }

        private static readonly object[] FillMainDiagonal_DataProvider =
        {
            new object[]
            {
                new float[][] {new float[] {1, 8, 3}, new float[] {9, 1, 0}, new float[] {2, 0, 1}},
                new float[][] {new float[] {1, 8, 3}, new float[] {9, 1, 0}, new float[] {2, 0, 1}}
            },
            new object[]
            {
                new float[][] {new float[] {1, 8, 3}, new float[] {0, 1, 0}, new float[] {0, 0, 1}},
                new float[][] {new float[] {1, 8, 3}, new float[] {0, 1, 0}, new float[] {0, 0, 1}}
            },
            new object[]
            {
                new float[][] {new float[] {0, 8, 3}, new float[] {9, 0, 0}, new float[] {2, 0, 0}},
                new float[][] {new float[] {1, 8, 3}, new float[] {9, 1, 0}, new float[] {2, 0, 1}}
            }
        };

        [TestCaseSource(nameof(FillMainDiagonal_DataProvider))]
        public void FillMainDiagonal_Test(float[][] generatedOutputMatrix, float[][] result)
        {
            Assert.That(Form1.FillMainDiagonalOfOutputMatrix(generatedOutputMatrix), Is.EqualTo(result));
        }

        private static readonly object[] GetNextRnd_DataProvider =
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

        [TestCaseSource(nameof(GetNextRnd_DataProvider))]
        public void GetNextRnd_Test(int min, int max)
        {
            Assert.That(Form1.GetNextRandomValue(min, max), Is.GreaterThan(min - 1).And.LessThan(max + 1));
        }
    }
}