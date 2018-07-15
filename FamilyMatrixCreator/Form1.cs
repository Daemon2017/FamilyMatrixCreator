using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
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

        private static RNGCryptoServiceProvider _RNG = new RNGCryptoServiceProvider();
        int[,][] relationshipsMatrix;
        float[] centimorgansMatrix;
        int numberOfProband;

        private static int GetNextRnd(int min, int max)
        {
            byte[] rndBytes = new byte[4];
            _RNG.GetBytes(rndBytes);
            int rand = BitConverter.ToInt32(rndBytes, 0);
            const Decimal OldRange = int.MaxValue - (Decimal)int.MinValue;
            Decimal NewRange = max - min;
            Decimal NewValue = (rand - (Decimal)int.MinValue) / OldRange * NewRange + min;
            return (int)NewValue;
        }

        /*
         * Построение выходной матрицы (матрицы родственных отношений).
         */
        private float[][] GenerateOutputMatrix(int generatedMatrixSize, List<int> existingRelationshipDegrees)
        {
            float[][] generatedOutputMatrix = new float[generatedMatrixSize][];

            /*
             * Построение правой (верхней) стороны.
             */
            for (int person = 0;
                person < generatedOutputMatrix.GetLength(0);
                person++)
            {
                generatedOutputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];

                for (int relative = person + 1;
                    relative < generatedOutputMatrix.GetLength(0);
                    relative++)
                {
                    if (0 == person)
                    {
                        /*
                         * Создание случайных степеней родства для пробанда.
                         */
                        int randomRelative = GetNextRnd(0, relationshipsMatrix.GetLength(1));
                        generatedOutputMatrix[person][relative] = relationshipsMatrix[numberOfProband, randomRelative][0];

                        int numberOfPerson = 0;

                        /*
                         * Среди возможных степеней родства пробанда ищется порядковый номер того,
                         * что содержит выбранную степень родства.
                         */
                        for (int number = 0;
                            number < relationshipsMatrix.GetLength(1);
                            number++)
                        {
                            if (relationshipsMatrix[numberOfProband, number][0] == generatedOutputMatrix[person][relative])
                            {
                                numberOfPerson = number;
                                break;
                            }
                        }

                        /*
                         * Исключение тех степеней родства, 
                         * которые не имеют ни одной общей степени родства с теми, 
                         * что доступны пробанду.
                         */
                        int quantityOfPossibleRelatives = 0;

                        for (int possibleRelative = 0;
                            possibleRelative < relationshipsMatrix.GetLength(1);
                            possibleRelative++)
                        {
                            int quantityOfPossibleRelationships = 0;

                            for (int possibleRelationship = 0;
                                possibleRelationship < relationshipsMatrix[numberOfPerson, possibleRelative].Length;
                                possibleRelationship++)
                            {
                                int quantityOfPossibleProbandsRelationships = 0;

                                for (int possibleProbandsRelationship = 0;
                                    possibleProbandsRelationship < relationshipsMatrix.GetLength(1);
                                    possibleProbandsRelationship++)
                                {
                                    if (relationshipsMatrix[numberOfProband, possibleProbandsRelationship][0] == relationshipsMatrix[numberOfPerson, possibleRelative][possibleRelationship]
                                        || 0 == relationshipsMatrix[numberOfPerson, possibleRelative][possibleRelationship])
                                    {
                                        quantityOfPossibleProbandsRelationships++;
                                    }
                                }

                                if (quantityOfPossibleProbandsRelationships == relationshipsMatrix.GetLength(1))
                                {
                                    quantityOfPossibleRelationships++;
                                }
                            }

                            if (quantityOfPossibleRelationships > 0)
                            {
                                quantityOfPossibleRelatives++;
                            }
                        }

                        if (quantityOfPossibleRelatives == 0)
                        {
                            relative--;
                        }
                    }
                    else if (person > 0)
                    {
                        int[] allPossibleRelationships = { 0 };

                        /*
                         * Исключение невозможных степеней родства.
                         */
                        for (int k = 0;
                            k < person;
                            k++)
                        {
                            int numberOfI = 0,
                                numberOfJ = 0;

                            /*
                             * Среди возможных степеней родства пробанда ищутся порядковые номера тех,
                             * что содержат выбранные степени родства.
                             */
                            for (int number = 0;
                                number < relationshipsMatrix.GetLength(1);
                                number++)
                            {
                                if (relationshipsMatrix[numberOfProband, number][0] == generatedOutputMatrix[k][person])
                                {
                                    numberOfI = number;
                                }

                                if (relationshipsMatrix[numberOfProband, number][0] == generatedOutputMatrix[k][relative])
                                {
                                    numberOfJ = number;
                                }
                            }

                            if (0 == k)
                            {
                                allPossibleRelationships = relationshipsMatrix[numberOfI, numberOfJ];

                                allPossibleRelationships = allPossibleRelationships.Where(val => val != 1).ToArray();

                                /*
                                 * Исключение возможных степеней родства, 
                                 * которые невозможно сгенерировать.
                                 */
                                for (int m = 0;
                                    m < allPossibleRelationships.GetLength(0);
                                    m++)
                                {
                                    bool isRelationshipAllowed = false;

                                    for (int n = 0;
                                        n < relationshipsMatrix.GetLength(1);
                                        n++)
                                    {
                                        if (allPossibleRelationships[m] == relationshipsMatrix[numberOfProband, n][0])
                                        {
                                            isRelationshipAllowed = true;
                                            break;
                                        }
                                    }

                                    if (false == isRelationshipAllowed && 0 != allPossibleRelationships[m])
                                    {
                                        allPossibleRelationships = allPossibleRelationships.Where(val => val != allPossibleRelationships[m]).ToArray();
                                        m--;
                                    }
                                }
                            }
                            else
                            {
                                int[] currentPossibleRelationships = relationshipsMatrix[numberOfI, numberOfJ];

                                /*
                                 * Исключение возможных степеней родства, 
                                 * которые могут вызвать конфликт с уже существующими родственниками.
                                 */
                                for (int m = 0;
                                    m < allPossibleRelationships.GetLength(0);
                                    m++)
                                {
                                    bool isRelationshipAllowed = false;

                                    for (int n = 0;
                                        n < currentPossibleRelationships.GetLength(0);
                                        n++)
                                    {
                                        if (allPossibleRelationships[m] == currentPossibleRelationships[n])
                                        {
                                            isRelationshipAllowed = true;
                                            break;
                                        }
                                    }

                                    if (false == isRelationshipAllowed && 0 != allPossibleRelationships[m])
                                    {
                                        allPossibleRelationships = allPossibleRelationships.Where(val => val != allPossibleRelationships[m]).ToArray();
                                        m--;
                                    }
                                }
                            }
                        }

                        /*
                         * Избегание степени родства "0" везде, 
                         * где это возможно.
                         */
                        if (true == checkBox1.Checked)
                        {
                            /*
                             * Проверяем списки на наличие степени родства "0".
                             */
                            foreach (var relationship in allPossibleRelationships)
                            {
                                if (0 == relationship)
                                {
                                    /*
                                     * Подвергаем избавлению от "0" только те списки,
                                     * где помимо "0" может быть, как минимум, еще одна степень родства.
                                     */
                                    if (allPossibleRelationships.GetLength(0) > 1)
                                    {
                                        allPossibleRelationships = allPossibleRelationships.Where(val => val != 0).ToArray();
                                    }
                                }
                            }
                        }

                        int randomRelative = GetNextRnd(0, allPossibleRelationships.GetLength(0));
                        generatedOutputMatrix[person][relative] = allPossibleRelationships[randomRelative];
                    }
                }

                if (generatedOutputMatrix.GetLength(0) - 1 == person)
                {
                    int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count()];
                    quantityOfEachRelationship = CollectStatistics(generatedOutputMatrix, quantityOfEachRelationship, existingRelationshipDegrees);

                    int sumOfMeaningfulValues = 0;

                    foreach (var quantity in quantityOfEachRelationship)
                    {
                        sumOfMeaningfulValues += quantity;
                    }

                    if (100 * ((generatedMatrixSize + 2 * (float)sumOfMeaningfulValues) / (generatedMatrixSize * generatedMatrixSize))
                        < Convert.ToInt32(textBox4.Text))
                    {
                        generatedOutputMatrix = new float[generatedMatrixSize][];
                        person = -1;
                    }
                }
            }

            /*
            * Построение левой (нижней) стороны.
            */
            for (int genPerson = 1;
                genPerson < generatedOutputMatrix.GetLength(0);
                genPerson++)
            {
                for (int genRelative = 0;
                    genRelative < genPerson;
                    genRelative++)
                {
                    for (int genRelationship = 0;
                        genRelationship < relationshipsMatrix.GetLength(1);
                        genRelationship++)
                    {
                        if (relationshipsMatrix[numberOfProband, genRelationship][0] == generatedOutputMatrix[genRelative][genPerson])
                        {
                            generatedOutputMatrix[genPerson][genRelative] = relationshipsMatrix[genRelationship, numberOfProband][0];
                        }
                    }
                }
            }

            for (int i = 0;
                i < generatedOutputMatrix.GetLength(0);
                i++)
            {
                generatedOutputMatrix[i][i] = 1;
            }

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
            for (int person = 0;
                person < generatedOutputMatrix.GetLength(0);
                person++)
            {
                generatedInputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];

                for (int relative = person;
                    relative < generatedOutputMatrix.GetLength(0);
                    relative++)
                {
                    for (int relationship = 0;
                        relationship < relationshipsMatrix.GetLength(1);
                        relationship++)
                    {
                        if (relationshipsMatrix[numberOfProband, relationship][0] == generatedOutputMatrix[person][relative])
                        {
                            if (centimorgansMatrix[relationship] <= 3950)
                            {
                                double mean = centimorgansMatrix[relationship];
                                double stdDev = (centimorgansMatrix[relationship] * (-0.2819 * Math.Log(centimorgansMatrix[relationship]) + 2.335)) / 3;

                                Normal normalDist = new Normal(mean, stdDev);
                                float normalyDistributedValue = (float)normalDist.Sample();

                                if (normalyDistributedValue < 0)
                                {
                                    normalyDistributedValue = 0;
                                }

                                generatedInputMatrix[person][relative] = normalyDistributedValue;
                            }
                            else
                            {
                                generatedInputMatrix[person][relative] = centimorgansMatrix[relationship];
                            }
                        }

                        if (relationshipsMatrix[relationship, numberOfProband][0] == generatedOutputMatrix[person][relative])
                        {
                            if (centimorgansMatrix[relationship] <= 3950)
                            {
                                double mean = centimorgansMatrix[relationship];
                                double stdDev = (centimorgansMatrix[relationship] * (-0.2819 * Math.Log(centimorgansMatrix[relationship]) + 2.335)) / 3;

                                Normal normalDist = new Normal(mean, stdDev);
                                float normalyDistributedValue = (float)normalDist.Sample();

                                if (normalyDistributedValue < 0)
                                {
                                    normalyDistributedValue = 0;
                                }

                                generatedInputMatrix[person][relative] = normalyDistributedValue;
                            }
                            else
                            {
                                generatedInputMatrix[person][relative] = centimorgansMatrix[relationship];
                            }
                        }
                    }
                }
            }

            /*
            * Построение левой (нижней) стороны.
            */
            for (int genPerson = 1;
                genPerson < generatedOutputMatrix.GetLength(0);
                genPerson++)
            {
                for (int genRelative = 0;
                    genRelative < genPerson;
                    genRelative++)
                {
                    generatedInputMatrix[genPerson][genRelative] = generatedInputMatrix[genRelative][genPerson];
                }
            }

            return generatedInputMatrix;
        }

        /*
        * Сбор статистики по родству осуществляем только сейчас, 
        * т.к. некоторые значения могут меняться из-за relative--.
        */
        private int[] CollectStatistics(float[][] generatedOutputMatrix, int[] quantityOfEachRelationship, List<int> existingRelationshipDegrees)
        {
            for (int person = 0;
                person < generatedOutputMatrix.GetLength(0);
                person++)
            {
                for (int relative = 0;
                    relative < generatedOutputMatrix.GetLength(0);
                    relative++)
                {
                    for (int probandsRelatioship = 0;
                        probandsRelatioship < existingRelationshipDegrees.Count();
                        probandsRelatioship++)
                    {
                        if (generatedOutputMatrix[person][relative] == existingRelationshipDegrees[probandsRelatioship])
                        {
                            quantityOfEachRelationship[probandsRelatioship]++;
                        }
                    }
                }
            }

            return quantityOfEachRelationship;
        }

        private void CreateComplianceMatrix(List<int> existingRelationshipDegrees)
        {
            List<int[]> complianceMatrix = new List<int[]>
            {
                new int[2] {0, 1}
            };

            for (int relationship = 0;
                relationship < existingRelationshipDegrees.Count();
                relationship++)
            {
                int[] compliance = { existingRelationshipDegrees[relationship], relationship + 2 };
                complianceMatrix.Add(compliance);
            }

            using (StreamWriter outfile = new StreamWriter("compliance.csv"))
            {
                foreach (var relationship in complianceMatrix)
                {
                    string content = "";

                    content += relationship[0].ToString() + ", " + relationship[1].ToString();

                    outfile.WriteLine(content);
                }
            }
        }

        /*
         * Преобразовываем матрицу так, 
         * чтобы не было разрывов между номерами степеней родства.
         */
        private float[][] TransformMatrix(float[][] generatedOutputMatrix, List<int> existingRelationshipDegrees)
        {
            for (int person = 0;
                person < generatedOutputMatrix.GetLength(0);
                person++)
            {
                for (int relative = 0;
                    relative < generatedOutputMatrix.GetLength(0);
                    relative++)
                {
                    for (int relationship = 0;
                        relationship < existingRelationshipDegrees.Count();
                        relationship++)
                    {
                        if (existingRelationshipDegrees[relationship] == generatedOutputMatrix[person][relative])
                        {
                            /*
                             * Делаем +2, чтобы нумерация значащих степеней родства шла с 2.
                             */
                            generatedOutputMatrix[person][relative] = relationship + 2;
                            break;
                        }
                        else if (0 == generatedOutputMatrix[person][relative])
                        {
                            generatedOutputMatrix[person][relative] = 1;
                            break;
                        }
                    }
                }
            }

            return generatedOutputMatrix;
        }

        private void Generate(object sender, EventArgs e)
        {
            List<int> existingRelationshipDegrees = new List<int>();
            for (int i = 0;
                i < relationshipsMatrix.GetLength(0);
                i++)
            {
                if (!existingRelationshipDegrees.Contains(relationshipsMatrix[numberOfProband, i][0]))
                {
                    existingRelationshipDegrees.Add(relationshipsMatrix[numberOfProband, i][0]);
                }

                if (!existingRelationshipDegrees.Contains(relationshipsMatrix[i, numberOfProband][0]))
                {
                    existingRelationshipDegrees.Add(relationshipsMatrix[i, numberOfProband][0]);
                }
            }

            CreateComplianceMatrix(existingRelationshipDegrees);

            int quantityOfMatrixes = Convert.ToInt32(textBox1.Text);
            int generatedMatrixSize = Convert.ToInt32(textBox3.Text);
            int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count()];
            textBox2.Text = "";

            if (quantityOfMatrixes > 0)
            {
                Parallel.For(0, quantityOfMatrixes, matrixNumber =>
                {
                    float[][] generatedOutputMatrix = GenerateOutputMatrix(generatedMatrixSize, existingRelationshipDegrees);

                    float[][] generatedInputMatrix = GenerateInputMatrix(generatedOutputMatrix, generatedMatrixSize);

                    if (true == checkBox2.Checked)
                    {
                        quantityOfEachRelationship = CollectStatistics(generatedOutputMatrix, quantityOfEachRelationship, existingRelationshipDegrees);
                    }

                    if (true == checkBox3.Checked)
                    {
                        generatedOutputMatrix = TransformMatrix(generatedOutputMatrix, existingRelationshipDegrees);
                    }

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

                if (true == checkBox2.Checked)
                {
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

                    toolStripStatusLabel1.Text = "Значащих значений: " +
                        100 * ((float)sumOfMeaningfulValues / (quantityOfMatrixes * generatedMatrixSize * generatedMatrixSize)) + "%";
                }
            }
        }
    }
}
