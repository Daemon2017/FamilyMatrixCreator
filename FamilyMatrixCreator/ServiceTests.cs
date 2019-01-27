using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyMatrixCreator
{
    [TestFixture]
    class ServiceTests
    {
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
    }
}
