using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FamilyMatrixCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            InitializeComponent();
        }

        Modules modules = new Modules();
        Integrations integrations = new Integrations();
        FileSaverLoader fileSaverLoader = new FileSaverLoader();

        private int[,][] relationshipsMatrix;
        private float[] centimorgansMatrix;
        private int[][] maxCountMatrix;
        private int numberOfProband;

        private void Form1_Load(object sender, EventArgs e)
        {
            relationshipsMatrix = fileSaverLoader.LoadFromFile2DJagged("relationships.csv");
            numberOfProband = modules.FindNumberOfProband(relationshipsMatrix);

            centimorgansMatrix = fileSaverLoader.LoadFromFile1D("centimorgans.csv");

            maxCountMatrix = fileSaverLoader.LoadFromFile2D("maxCount.csv");
        }

        /*
         * Построение выходной матрицы (матрицы родственных отношений).
         */
        private float[][] GenerateOutputMatrix(int generatedMatrixSize, List<int> existingRelationshipDegrees)
        {
            float[][] generatedOutputMatrix = integrations.BuildRightTopPart(relationshipsMatrix,numberOfProband, generatedMatrixSize, existingRelationshipDegrees, maxCountMatrix, Convert.ToInt32(textBox4.Text), Convert.ToInt32(textBox5.Text));
            generatedOutputMatrix = modules.BuildLeftBottomPart(generatedOutputMatrix, relationshipsMatrix, numberOfProband);

            generatedOutputMatrix = modules.FillMainDiagonal(generatedOutputMatrix);

            return generatedOutputMatrix;
        }

        /*
         * Построение входной матрицы (матрицы сМ).
         */
        private float[][] GenerateInputMatrix(float[][] generatedOutputMatrix, int generatedMatrixSize)
        {
            float[][] generatedInputMatrix = new float[generatedMatrixSize][];

            generatedInputMatrix = integrations.BuildRightTopPart(generatedOutputMatrix, relationshipsMatrix, numberOfProband, generatedInputMatrix, centimorgansMatrix);
            generatedInputMatrix = modules.BuildLeftBottomPart(generatedInputMatrix);

            return generatedInputMatrix;
        }

        private void Generate(object sender, EventArgs e)
        {
            List<int> existingRelationshipDegrees = modules.FindAllExistingRelationshipDegrees(relationshipsMatrix, numberOfProband);

            List<int[]> complianceMatrix = modules.CreateComplianceMatrix(existingRelationshipDegrees);
            fileSaverLoader.SaveToFile("compliance.csv", complianceMatrix);

            int quantityOfMatrixes = Convert.ToInt32(textBox1.Text);
            textBox2.Text = "";

            Stopwatch myStopwatch = new Stopwatch();
            myStopwatch.Start();

            if (quantityOfMatrixes > 0)
            {
                int generatedMatrixSize = Convert.ToInt32(textBox3.Text);
                int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count()];

                Parallel.For(0, quantityOfMatrixes, matrixNumber =>
                {
                    float[][] generatedOutputMatrix = GenerateOutputMatrix(generatedMatrixSize, existingRelationshipDegrees);
                    float[][] generatedInputMatrix = GenerateInputMatrix(generatedOutputMatrix, generatedMatrixSize);

                    quantityOfEachRelationship = modules.CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees, quantityOfEachRelationship);

                    //generatedOutputMatrix = modules.TransformMatrix(generatedOutputMatrix, existingRelationshipDegrees);

                    /*
                     * Сохранение входной матрицы в файл.
                     */
                    Directory.CreateDirectory("input");
                    fileSaverLoader.SaveToFile(@"input\generated_input", generatedInputMatrix, matrixNumber);

                    /*
                     * Сохранение выходной матрицы в файл.
                     */
                    Directory.CreateDirectory("output");
                    fileSaverLoader.SaveToFile(@"output\generated_output", generatedOutputMatrix, matrixNumber);
                });

                myStopwatch.Stop();

                /*
                 * Вывод статистики по родству.
                 */
                int relationshipNumber = 0;
                int sumOfMeaningfulValues = 0;

                foreach (var quantity in quantityOfEachRelationship)
                {
                    textBox2.Text += "Родство " + existingRelationshipDegrees[relationshipNumber] + ": " + quantity + Environment.NewLine;
                    sumOfMeaningfulValues += quantity;

                    relationshipNumber++;
                }

                label5.Text = "Значащих значений: "
                    + 100 * ((float)(sumOfMeaningfulValues - (quantityOfMatrixes * generatedMatrixSize)) / (quantityOfMatrixes * Math.Pow(generatedMatrixSize, 2))) + "%";
                label6.Text = "Затрачено: " + (((float)myStopwatch.ElapsedMilliseconds) / 1000).ToString() + " сек";
            }
        }
    }
}
