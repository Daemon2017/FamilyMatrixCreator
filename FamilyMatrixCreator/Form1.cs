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

        private readonly Modules _modules = new Modules();
        private readonly Integrations _integrations = new Integrations();
        private readonly FileSaverLoader _fileSaverLoader = new FileSaverLoader();

        private int[,][] _relationshipsMatrix;
        private float[] _centimorgansMatrix;
        private int[][] _maxCountMatrix;
        private int _numberOfProband;

        private void Form1_Load(object sender, EventArgs e)
        {
            _relationshipsMatrix = _fileSaverLoader.LoadFromFile2DJagged("relationships.csv");
            _numberOfProband = _modules.FindNumberOfProband(_relationshipsMatrix);
            _centimorgansMatrix = _fileSaverLoader.LoadFromFile1D("centimorgans.csv");
            _maxCountMatrix = _fileSaverLoader.LoadFromFile2D("maxCount.csv");
        }

        /*
         * Построение выходной матрицы (матрицы родственных отношений).
         */
        private float[][] GenerateOutputMatrix(int generatedMatrixSize, List<int> existingRelationshipDegrees)
        {
            float[][] generatedOutputMatrix = _integrations.OutputBuildRightTopPart(_relationshipsMatrix,_numberOfProband, generatedMatrixSize, existingRelationshipDegrees, _maxCountMatrix, Convert.ToInt32(textBox4.Text), Convert.ToInt32(textBox5.Text));
            generatedOutputMatrix = _modules.OutputBuildLeftBottomPart(generatedOutputMatrix, _relationshipsMatrix, _numberOfProband);

            generatedOutputMatrix = _modules.FillMainDiagonal(generatedOutputMatrix);

            return generatedOutputMatrix;
        }

        /*
         * Построение входной матрицы (матрицы сМ).
         */
        private float[][] GenerateInputMatrix(float[][] generatedOutputMatrix, int generatedMatrixSize)
        {
            float[][] generatedInputMatrix = new float[generatedMatrixSize][];

            generatedInputMatrix = _integrations.InputBuildRightTopPart(generatedOutputMatrix, _relationshipsMatrix, _numberOfProband, generatedInputMatrix, _centimorgansMatrix);
            generatedInputMatrix = _modules.InputBuildLeftBottomPart(generatedInputMatrix);

            return generatedInputMatrix;
        }
       
        private void Generate(object sender, EventArgs e)
        {
            List<int> existingRelationshipDegrees = _modules.FindAllExistingRelationshipDegrees(_relationshipsMatrix, _numberOfProband);

            List<int[]> complianceMatrix = Enumerable.Range(0, existingRelationshipDegrees.Count)
                .Select(relationship => new int[] { existingRelationshipDegrees[relationship], relationship }).ToList();
            _fileSaverLoader.SaveToFile("compliance.csv", complianceMatrix);
            
            int quantityOfMatrixes = Convert.ToInt32(textBox1.Text);
            textBox2.Text = "";

            Stopwatch myStopwatch = new Stopwatch();
            myStopwatch.Start();

            if (quantityOfMatrixes > 0)
            {
                int generatedMatrixSize = Convert.ToInt32(textBox3.Text);
                int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count];

                Parallel.For(0, quantityOfMatrixes, matrixNumber =>
                {
                    float[][] generatedOutputMatrix = GenerateOutputMatrix(generatedMatrixSize, existingRelationshipDegrees);
                    float[][] generatedInputMatrix = GenerateInputMatrix(generatedOutputMatrix, generatedMatrixSize);

                    quantityOfEachRelationship = _modules.CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees, quantityOfEachRelationship);

                    //generatedOutputMatrix = modules.TransformMatrix(generatedOutputMatrix, existingRelationshipDegrees);

                    /*
                     * Сохранение входной матрицы в файл.
                     */
                    Directory.CreateDirectory("input");
                    _fileSaverLoader.SaveToFile(@"input\generated_input", generatedInputMatrix, matrixNumber);

                    /*
                     * Сохранение выходной матрицы в файл.
                     */
                    Directory.CreateDirectory("output");
                    _fileSaverLoader.SaveToFile(@"output\generated_output", generatedOutputMatrix, matrixNumber);
                });

                myStopwatch.Stop();

                /*
                 * Вывод статистики по родству.
                 */
                int relationshipNumber = 0;
                float sumOfMeaningfulValues = 0;

                foreach (var quantity in quantityOfEachRelationship)
                {
                    textBox2.Text += "Родство " + existingRelationshipDegrees[relationshipNumber] + ": " + quantity + Environment.NewLine;
                    sumOfMeaningfulValues += quantity;

                    relationshipNumber++;
                }

                sumOfMeaningfulValues -= quantityOfEachRelationship[0];

                label5.Text = "Значащих значений: "
                    + 100 * ((sumOfMeaningfulValues - quantityOfMatrixes * generatedMatrixSize) / (quantityOfMatrixes * Math.Pow(generatedMatrixSize, 2))) + "%";
                label6.Text = "Затрачено: " + (float)myStopwatch.ElapsedMilliseconds / 1000 + " сек";
            }
        }
    }
}
