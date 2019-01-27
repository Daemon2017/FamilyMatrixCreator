using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FamilyMatrixCreator;

namespace FamilyMatrixCreatorTest
{
    [TestClass]
    public class ServiceTest
    {
        private readonly Form1 form1;

        public ServiceTest()
        {
            form1 = new Form1();
        }

        [TestMethod]
        public void CreateComplianceMatrixTest()
        {
            List<int[]> model = new List<int[]> { new int[2] { 0, 1 },
                                                  new int[2] { 1, 2 },
                                                  new int[2] { 2, 3 },
                                                  new int[2] { 3, 4 },
                                                  new int[2] { 4, 5 },
                                                  new int[2] { 5, 6 }};

            List<int[]> result = form1.CreateComplianceMatrix(new List<int> { 1, 2, 3, 4, 5 });

            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < result[i].Length; j++)
                {
                    Assert.AreEqual(model[i].GetValue(j), result[i].GetValue(j));
                }
            }
        }
    }
}
