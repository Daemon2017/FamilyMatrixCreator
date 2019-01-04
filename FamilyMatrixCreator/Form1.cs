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
        private int[,][] relationshipsMatrix;
        private float[] centimorgansMatrix;
        private int[][] maxCountMatrix;
        private int numberOfProband;

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

            /*
             * Построение правой (верхней) стороны.
             */
            for (int person = 0; person < generatedOutputMatrix.GetLength(0); person++)
            {
                generatedOutputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];
                currentCountMatrix[person] = new int[maxCountMatrix.Length];

                List<int> relatives = ShuffleSequence(generatedOutputMatrix, person);

                for (int relative = 0; relative < relatives.Count; relative++)
                {
                    if (0 == person)
                    {
                        /*
                         * Создание родственника пробанда со случайным видом родства.
                         */
                        int numberOfPerson = GetNextRnd(0, relationshipsMatrix.GetLength(1));
                        generatedOutputMatrix[person][relatives[relative]] = relationshipsMatrix[numberOfProband, numberOfPerson][0];

                        if (true == MaxNumberOfThisRelationshipTypeIsNotExceeded(generatedOutputMatrix, currentCountMatrix, person, relatives, relative))
                        {
                            /*
                             * Исключение тех видов родства, которые не имеют ни одного общего вида родства с теми, что доступны пробанду.
                             */
                            int quantityOfPossibleRelatives = 0;

                            for (int possibleRelative = 0; possibleRelative < relationshipsMatrix.GetLength(1); possibleRelative++)
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

                            if (0 == quantityOfPossibleRelatives)
                            {
                                relative--;
                            }
                            else
                            {
                                IncreaseCurrentCount(generatedOutputMatrix, currentCountMatrix, person, relatives, relative);
                            }
                        }
                        else
                        {
                            relative--;
                        }
                    }
                    else if (person > 0)
                    {
                        int[] allPossibleRelationships = { 0 };

                        /*
                         * Исключение невозможных видов родства.
                         */
                        for (int previousPerson = 0; previousPerson < person; previousPerson++)
                        {
                            int numberOfI = 0,
                                numberOfJ = 0;

                            /*
                             * Среди возможных видов родства пробанда ищутся порядковые номера тех, что содержат выбранные виды родства.
                             */
                            for (int number = 0; number < relationshipsMatrix.GetLength(1); number++)
                            {
                                if (relationshipsMatrix[numberOfProband, number][0] == generatedOutputMatrix[previousPerson][person])
                                {
                                    numberOfI = number;
                                }

                                if (relationshipsMatrix[numberOfProband, number][0] == generatedOutputMatrix[previousPerson][relatives[relative]])
                                {
                                    numberOfJ = number;
                                }
                            }

                            if (0 == previousPerson)
                            {
                                allPossibleRelationships = relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToArray();

                                /*
                                 * Исключение возможных видов родства, которые невозможно сгенерировать.
                                 */
                                for (int m = 0; m < allPossibleRelationships.GetLength(0); m++)
                                {
                                    bool isRelationshipAllowed = false;

                                    for (int n = 0; n < relationshipsMatrix.GetLength(1); n++)
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
                                 * Исключение возможных видов родства, которые могут вызвать конфликт с уже существующими родственниками.
                                 */
                                for (int m = 0; m < allPossibleRelationships.GetLength(0); m++)
                                {
                                    bool isRelationshipAllowed = false;

                                    for (int n = 0; n < currentPossibleRelationships.GetLength(0); n++)
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
                         * Устранение вида родства "0" из списка допустимых видов родства, если помимо "0" в нем присутствует, как минимум, еще один вид родства.
                         */
                        foreach (var relationship in allPossibleRelationships)
                        {
                            if (0 == relationship)
                            {
                                if (allPossibleRelationships.GetLength(0) > 1)
                                {
                                    allPossibleRelationships = allPossibleRelationships.Where(val => val != 0).ToArray();
                                }
                            }
                        }

                        /*
                         * Устранение видов родства из списка допустимых видов родства, 
                         * добавление которых приведет к превышению допустимого числа родственников с таким видом родства.
                         */
                        foreach (int relationship in allPossibleRelationships)
                        {
                            if (false == MaxNumberOfThisRelationshipTypeIsNotExceeded(relationship, currentCountMatrix, person))
                            {
                                allPossibleRelationships = allPossibleRelationships.Where(val => val != relationship).ToArray();
                            }
                        }

                        /*
                        * Создание родственника со случайным видом родства.
                        */
                        generatedOutputMatrix[person][relatives[relative]] = allPossibleRelationships[GetNextRnd(0, allPossibleRelationships.GetLength(0))];

                        IncreaseCurrentCount(generatedOutputMatrix, currentCountMatrix, person, relatives, relative);
                    }
                }

                /*
                 * Проверка того, что выполняется требование по проценту значащих значений
                 */
                if (generatedOutputMatrix.GetLength(0) - 1 == person)
                {
                    int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count()];
                    quantityOfEachRelationship = CollectStatistics(generatedOutputMatrix, quantityOfEachRelationship, existingRelationshipDegrees);

                    int sumOfMeaningfulValues = 0;

                    foreach (var quantity in quantityOfEachRelationship)
                    {
                        sumOfMeaningfulValues += quantity;
                    }

                    if ((100 * ((generatedMatrixSize + 2 * (float)sumOfMeaningfulValues) / (generatedMatrixSize * generatedMatrixSize))) < Convert.ToInt32(textBox4.Text)
                        || (100 * ((generatedMatrixSize + 2 * (float)sumOfMeaningfulValues) / (generatedMatrixSize * generatedMatrixSize))) > Convert.ToInt32(textBox5.Text))
                    {
                        currentCountMatrix = new int[generatedMatrixSize][];
                        generatedOutputMatrix = new float[generatedMatrixSize][];
                        person = -1;
                    }
                }
            }

            /*
            * Построение левой (нижней) стороны.
            */
            for (int genPerson = 1; genPerson < generatedOutputMatrix.GetLength(0); genPerson++)
            {
                for (int genRelative = 0; genRelative < genPerson; genRelative++)
                {
                    for (int genRelationship = 0; genRelationship < relationshipsMatrix.GetLength(1); genRelationship++)
                    {
                        if (relationshipsMatrix[numberOfProband, genRelationship][0] == generatedOutputMatrix[genRelative][genPerson])
                        {
                            generatedOutputMatrix[genPerson][genRelative] = relationshipsMatrix[genRelationship, numberOfProband][0];
                        }
                    }
                }
            }

            for (int i = 0; i < generatedOutputMatrix.GetLength(0); i++)
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
            for (int person = 0; person < generatedOutputMatrix.GetLength(0); person++)
            {
                generatedInputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];

                for (int relative = person; relative < generatedOutputMatrix.GetLength(0); relative++)
                {
                    for (int relationship = 0; relationship < relationshipsMatrix.GetLength(1); relationship++)
                    {
                        if (relationshipsMatrix[numberOfProband, relationship][0] == generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] = TransformRelationshipTypeToCm(generatedInputMatrix, person, relative, relationship);
                        }

                        if (relationshipsMatrix[relationship, numberOfProband][0] == generatedOutputMatrix[person][relative])
                        {
                            generatedInputMatrix[person][relative] = TransformRelationshipTypeToCm(generatedInputMatrix, person, relative, relationship);
                        }
                    }
                }
            }

            /*
            * Построение левой (нижней) стороны.
            */
            for (int genPerson = 1; genPerson < generatedOutputMatrix.GetLength(0); genPerson++)
            {
                for (int genRelative = 0; genRelative < genPerson; genRelative++)
                {
                    generatedInputMatrix[genPerson][genRelative] = generatedInputMatrix[genRelative][genPerson];
                }
            }

            return generatedInputMatrix;
        }

        private void Generate(object sender, EventArgs e)
        {
            List<int> existingRelationshipDegrees = new List<int>();
            for (int i = 0; i < relationshipsMatrix.GetLength(0); i++)
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
            textBox2.Text = "";

            if (quantityOfMatrixes > 0)
            {
                int generatedMatrixSize = Convert.ToInt32(textBox3.Text);
                int[] quantityOfEachRelationship = new int[existingRelationshipDegrees.Count()];

                Parallel.For(0, quantityOfMatrixes, matrixNumber =>
                {
                    float[][] generatedOutputMatrix = GenerateOutputMatrix(generatedMatrixSize, existingRelationshipDegrees);
                    float[][] generatedInputMatrix = GenerateInputMatrix(generatedOutputMatrix, generatedMatrixSize);

                    quantityOfEachRelationship = CollectStatistics(generatedOutputMatrix, quantityOfEachRelationship, existingRelationshipDegrees);

                    generatedOutputMatrix = TransformMatrix(generatedOutputMatrix, existingRelationshipDegrees);

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

                toolStripStatusLabel1.Text = "Значащих значений: "
                    + 100 * ((float)sumOfMeaningfulValues / (quantityOfMatrixes * generatedMatrixSize * generatedMatrixSize)) + "%";
            }
        }
    }
}
