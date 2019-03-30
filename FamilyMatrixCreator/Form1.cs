using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.Distributions;

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
        private int[][] _ancestorsMaxCountMatrix;
        private int[][] _descendantsMatrix;
        private int _numberOfProband;

        private void Form1_Load(object sender, EventArgs e)
        {
            _relationshipsMatrix = _fileSaverLoader.LoadFromFile2DJagged("relationships.csv");
            _numberOfProband = _modules.FindNumberOfProband(_relationshipsMatrix);
            _centimorgansMatrix = _fileSaverLoader.LoadFromFile1D("centimorgans.csv");
            _ancestorsMaxCountMatrix = _fileSaverLoader.LoadFromFile2D("ancestorsMatrix.csv");
            _descendantsMatrix = _fileSaverLoader.LoadFromFile2D("descendantsMatrix.csv");
        }

        private void Generate(object sender, EventArgs e)
        {
            List<int> existingRelationshipDegrees =
                _modules.FindAllExistingRelationshipDegrees(_relationshipsMatrix, _numberOfProband);

            List<int[]> complianceMatrix = Enumerable.Range(0, existingRelationshipDegrees.Count)
                .Select(relationship => new int[] {existingRelationshipDegrees[relationship], relationship}).ToList();
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
                    float[][] generatedOutputMatrix =
                        GenerateOutputMatrix(generatedMatrixSize, existingRelationshipDegrees);
                    float[][] generatedInputMatrix =
                        GenerateInputMatrix(generatedOutputMatrix, generatedMatrixSize);

                    quantityOfEachRelationship = _modules.CollectStatistics(generatedOutputMatrix,
                        existingRelationshipDegrees, quantityOfEachRelationship);

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
                    textBox2.Text += "Родство " + existingRelationshipDegrees[relationshipNumber] + ": " + quantity +
                                     Environment.NewLine;
                    sumOfMeaningfulValues += quantity;

                    relationshipNumber++;
                }

                _fileSaverLoader.SaveToFile(@"statistic.csv", textBox2.Text);

                sumOfMeaningfulValues -= quantityOfEachRelationship[0];

                label5.Text = "Значащих значений: "
                              + 100 * ((sumOfMeaningfulValues - quantityOfMatrixes * generatedMatrixSize) /
                                       (quantityOfMatrixes * Math.Pow(generatedMatrixSize, 2))) + "%";
                label6.Text = "Затрачено: " + (float) myStopwatch.ElapsedMilliseconds / 1000 + " сек";
            }
        }

        /*
         * Построение выходной матрицы (матрицы родственных отношений).
         */
        private float[][] GenerateOutputMatrix(int generatedMatrixSize, List<int> existingRelationshipDegrees)
        {
            float[][] generatedOutputMatrix = OutputBuildRightTopPart(_relationshipsMatrix,
                _numberOfProband, generatedMatrixSize, existingRelationshipDegrees,
                _ancestorsMaxCountMatrix, _descendantsMatrix,
                Convert.ToInt32(textBox4.Text), Convert.ToInt32(textBox5.Text));
            generatedOutputMatrix =
                _modules.OutputBuildLeftBottomPart(generatedOutputMatrix, _relationshipsMatrix, _numberOfProband);

            generatedOutputMatrix = _modules.FillMainDiagonal(generatedOutputMatrix);

            return generatedOutputMatrix;
        }

        /*
         * Построение правой (верхней) стороны.
         */
        public float[][] OutputBuildRightTopPart(int[,][] relationshipsMatrix, int numberOfProband,
            int generatedMatrixSize, List<int> existingRelationshipDegrees,
            int[][] ancestorsMaxCountMatrix, int[][] descendantsMatrix,
            int minPercent, int maxPercent)
        {
            float[][] generatedOutputMatrix = new float[generatedMatrixSize][];
            int[][] currentCountMatrix = new int[generatedMatrixSize][];

            List<int> persons = (from x in Enumerable.Range(1, generatedOutputMatrix.GetLength(0) - 1)
                orderby new ContinuousUniform().Sample()
                select x).ToList();
            persons.Insert(0, 0);

            for (int person = 0; person < persons.Count; person++)
            {
                generatedOutputMatrix[persons[person]] = new float[generatedOutputMatrix.GetLength(0)];
                currentCountMatrix[persons[person]] = new int[ancestorsMaxCountMatrix.Length];

                List<int> relatives = (from x in Enumerable.Range(persons[person] + 1,
                        generatedOutputMatrix.GetLength(0) - (persons[person] + 1))
                    orderby new ContinuousUniform().Sample()
                    select x).ToList();

                for (int relative = 0; relative < relatives.Count; relative++)
                {
                    List<int> allPossibleRelationships = _integrations.DetectAllPossibleRelationships(
                        relationshipsMatrix, numberOfProband,
                        ancestorsMaxCountMatrix, descendantsMatrix,
                        generatedOutputMatrix, currentCountMatrix,
                        persons, person,
                        relatives, relative);

                    /*
                     * Создание родственника со случайным видом родства.
                     */
                    try
                    {
                        generatedOutputMatrix[persons[person]][relatives[relative]] =
                            allPossibleRelationships[_modules.GetNextRnd(0, allPossibleRelationships.Count)];
                        currentCountMatrix = _modules.IncreaseCurrentRelationshipCount(generatedOutputMatrix,
                            currentCountMatrix, persons, person, relatives, relative, ancestorsMaxCountMatrix);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        generatedOutputMatrix = new float[generatedMatrixSize][];
                        currentCountMatrix = new int[generatedMatrixSize][];

                        persons = (from x in Enumerable.Range(1, generatedOutputMatrix.GetLength(0) - 1)
                            orderby new ContinuousUniform().Sample()
                            select x).ToList();
                        persons.Insert(0, 0);

                        person = -1;

                        break;
                    }
                }

                /*
                 * Проверка того, что выполняется требование по проценту значащих значений
                 */
                if (generatedOutputMatrix.GetLength(0) - 1 == person)
                {
                    double percentOfMeaningfulValues = 2 * _integrations.CalculatePercentOfMeaningfulValues(
                                                           generatedMatrixSize,
                                                           existingRelationshipDegrees, generatedOutputMatrix);

                    if (percentOfMeaningfulValues < minPercent || percentOfMeaningfulValues > maxPercent)
                    {
                        generatedOutputMatrix = new float[generatedMatrixSize][];
                        currentCountMatrix = new int[generatedMatrixSize][];

                        persons = (from x in Enumerable.Range(1, generatedOutputMatrix.GetLength(0) - 1)
                            orderby new ContinuousUniform().Sample()
                            select x).ToList();
                        persons.Insert(0, 0);

                        person = -1;
                    }
                }
            }

            return generatedOutputMatrix;
        }

        /*
         * Построение входной матрицы (матрицы сМ).
         */
        private float[][] GenerateInputMatrix(float[][] generatedOutputMatrix, int generatedMatrixSize)
        {
            float[][] generatedInputMatrix = new float[generatedMatrixSize][];

            generatedInputMatrix = InputBuildRightTopPart(generatedOutputMatrix, _relationshipsMatrix,
                _numberOfProband, generatedInputMatrix, _centimorgansMatrix);
            generatedInputMatrix = _modules.InputBuildLeftBottomPart(generatedInputMatrix);

            return generatedInputMatrix;
        }

        /*
         * Построение правой (верхней) стороны  (сМ).
         */
        public float[][] InputBuildRightTopPart(float[][] generatedOutputMatrix, int[,][] relationshipsMatrix,
            int numberOfProband, float[][] generatedInputMatrix, float[] centimorgansMatrix)
        {
            for (int person = 0; person < generatedOutputMatrix.GetLength(0); person++)
            {
                generatedInputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];

                for (int relative = person; relative < generatedOutputMatrix.GetLength(0); relative++)
                {
                    for (int relationship = 0; relationship < relationshipsMatrix.GetLength(1); relationship++)
                    {
                        if (relationshipsMatrix[numberOfProband, relationship][0] ==
                            generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] =
                                _modules.TransformRelationshipTypeToCm(generatedInputMatrix, person, relative,
                                    relationship, centimorgansMatrix);
                        }

                        if (relationshipsMatrix[relationship, numberOfProband][0] ==
                            generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] =
                                _modules.TransformRelationshipTypeToCm(generatedInputMatrix, person, relative,
                                    relationship, centimorgansMatrix);
                        }
                    }
                }
            }

            return generatedInputMatrix;
        }
    }
}