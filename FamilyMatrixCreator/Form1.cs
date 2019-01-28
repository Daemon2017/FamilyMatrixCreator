using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        private static RNGCryptoServiceProvider _RNG = new RNGCryptoServiceProvider();
        private int[,][] relationshipsMatrix;
        private float[] centimorgansMatrix;
        private int[][] maxCountMatrix;
        private int numberOfProband;
        private int numberOfConstructedMatrices;

        private static int GetNextRnd(int min, int max)
        {
            byte[] rndBytes = new byte[4];
            _RNG.GetBytes(rndBytes);
            return (int)((BitConverter.ToInt32(rndBytes, 0) - (Decimal)int.MinValue) / (int.MaxValue - (Decimal)int.MinValue) * (max - min) + min);
        }

        /*
         * Построение выходной матрицы (матрицы родственных отношений).
         */
        private float[][] GenerateOutputMatrix(int generatedMatrixSize, List<int> existingRelationshipDegrees)
        {
            float[][] generatedOutputMatrix = new float[generatedMatrixSize][];
            int[][] currentCountMatrix = new int[generatedMatrixSize][];

            List<int> persons = modules.ShuffleSequence(1, generatedOutputMatrix.GetLength(0));
            persons.Insert(0, 0);

            /*
             * Построение правой (верхней) стороны.
             */
            for (int person = 0; person < persons.Count; person++)
            {
                generatedOutputMatrix[persons[person]] = new float[generatedOutputMatrix.GetLength(0)];
                currentCountMatrix[persons[person]] = new int[maxCountMatrix.Length];

                List<int> relatives = modules.ShuffleSequence(persons[person] + 1, generatedOutputMatrix.GetLength(0));

                for (int relative = 0; relative < relatives.Count; relative++)
                {
                    int[] allPossibleRelationships = { };

                    if (0 == persons[person])
                    {
                        allPossibleRelationships = modules.FindAllPossibleRelationshipsOfProband(relationshipsMatrix, numberOfProband);
                    }
                    else
                    {
                        allPossibleRelationships = integrations.RemoveImpossibleRelationships(generatedOutputMatrix, persons, person, relatives, relative, allPossibleRelationships, relationshipsMatrix, numberOfProband);
                    }

                    /*
                     * Устранение видов родства из списка допустимых видов родства, 
                     * добавление которых приведет к превышению допустимого числа родственников с таким видом родства.
                     */
                    foreach (int relationship in allPossibleRelationships)
                    {
                        if (false == modules.MaxNumberOfThisRelationshipTypeIsNotExceeded(relationship, currentCountMatrix, persons, person, maxCountMatrix))
                        {
                            allPossibleRelationships = allPossibleRelationships.Where(val => val != relationship).ToArray();
                        }
                    }

                    /*
                     * Создание родственника со случайным видом родства.
                     */
                    generatedOutputMatrix[persons[person]][relatives[relative]] = allPossibleRelationships[GetNextRnd(0, allPossibleRelationships.GetLength(0))];
                    currentCountMatrix = modules.IncreaseCurrentRelationshipCount(generatedOutputMatrix, currentCountMatrix, persons, person, relatives, relative, maxCountMatrix);
                }

                /*
                 * Проверка того, что выполняется требование по проценту значащих значений
                 */
                if (generatedOutputMatrix.GetLength(0) - 1 == person)
                {
                    numberOfConstructedMatrices++;

                    int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count()];
                    quantityOfEachRelationship = modules.CollectStatistics(generatedOutputMatrix, existingRelationshipDegrees, quantityOfEachRelationship);

                    int sumOfMeaningfulValues = 0;

                    foreach (var quantity in quantityOfEachRelationship)
                    {
                        sumOfMeaningfulValues += quantity;
                    }

                    double percentOfMeaningfulValues = (100 * ((2 * (float)sumOfMeaningfulValues) / Math.Pow(generatedMatrixSize, 2)));
                    if (percentOfMeaningfulValues < Convert.ToInt32(textBox4.Text) || percentOfMeaningfulValues > Convert.ToInt32(textBox5.Text))
                    {
                        generatedOutputMatrix = new float[generatedMatrixSize][];
                        currentCountMatrix = new int[generatedMatrixSize][];

                        persons = modules.ShuffleSequence(1, generatedOutputMatrix.GetLength(0));
                        persons.Insert(0, 0);

                        person = -1;
                    }
                }
            }

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

            /*
             * Построение правой (верхней) стороны.
             */
            for (int person = 0; person < generatedOutputMatrix.GetLength(0); person++)
            {
                generatedInputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];

                for (int relative = person; relative < generatedOutputMatrix.GetLength(0); relative++)
                {
                    for (int relationship = 0; relationship < relationshipsMatrix.GetLength(1); relationship++)
                    {
                        if (relationshipsMatrix[numberOfProband, relationship][0] == generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] = modules.TransformRelationshipTypeToCm(generatedInputMatrix, person, relative, relationship, centimorgansMatrix);
                        }

                        if (relationshipsMatrix[relationship, numberOfProband][0] == generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] = modules.TransformRelationshipTypeToCm(generatedInputMatrix, person, relative, relationship, centimorgansMatrix);
                        }
                    }
                }
            }

            generatedInputMatrix = modules.BuildLeftBottomPart(generatedInputMatrix);

            return generatedInputMatrix;
        }

        private void Generate(object sender, EventArgs e)
        {
            List<int> existingRelationshipDegrees = modules.FindAllExistingRelationshipDegrees(relationshipsMatrix, numberOfProband);

            List<int[]> complianceMatrix = modules.CreateComplianceMatrix(existingRelationshipDegrees);
            SaveToFile("compliance.csv", complianceMatrix);

            int quantityOfMatrixes = Convert.ToInt32(textBox1.Text);
            numberOfConstructedMatrices = 0;
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
                    SaveToFile(@"input\generated_input", generatedInputMatrix, matrixNumber);

                    /*
                     * Сохранение выходной матрицы в файл.
                     */
                    Directory.CreateDirectory("output");
                    SaveToFile(@"output\generated_output", generatedOutputMatrix, matrixNumber);
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
                label7.Text = "Построено (включая отклоненные): " + numberOfConstructedMatrices;
            }
        }
    }
}
