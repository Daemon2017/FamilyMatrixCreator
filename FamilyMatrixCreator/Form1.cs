using System;
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
        int[][] ancestorsMatrix;
        int[][] descendantsMatrix;
        float[] centimorgansMatrix;
        float[] clustersMatrix;
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

        private void Generate(object sender, EventArgs e)
        {
            int quantityOfMatrixes = Convert.ToInt16(textBox1.Text);
            int[] quantityOfEachRelationship = new int[relationshipsMatrix.GetLength(1)];
            textBox2.Text = "";

            if (quantityOfMatrixes > 0)
            {
                Parallel.For(0, quantityOfMatrixes, matrixNumber =>
                {
                    /*
                     * Построение выходной матрицы (матрицы родственных отношений).
                     */
                    int generatedMatrixSize = 100;
                    float[][] generatedOutputMatrix = new float[generatedMatrixSize][];

                    for (int person = 0;
                        person < generatedOutputMatrix.GetLength(0);
                        person++)
                    {
                        generatedOutputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];

                        for (int relative = person;
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

                            /*
                             * Заполнение диагонали единицами.
                             */
                            if (person >= 0 && relative >= 0)
                            {
                                if (person == relative)
                                {
                                    generatedOutputMatrix[person][relative] = 1;
                                }
                            }
                        }
                    }

                    /*
                     * Построение входной матрицы (матрицы сМ).
                     */
                    float[][] generatedInputMatrix = new float[generatedMatrixSize][];

                    for (int person = 0;
                        person < generatedOutputMatrix.GetLength(0);
                        person++)
                    {
                        generatedInputMatrix[person] = new float[generatedOutputMatrix.GetLength(0)];

                        for (int relative = 0;
                            relative < generatedOutputMatrix.GetLength(0);
                            relative++)
                        {
                            for (int relationship = 0;
                                relationship < relationshipsMatrix.GetLength(1);
                                relationship++)
                            {
                                if (relationshipsMatrix[numberOfProband, relationship][0] == generatedOutputMatrix[person][relative])
                                {
                                    generatedInputMatrix[person][relative] = centimorgansMatrix[relationship];
                                }
                            }
                        }
                    }

                    /*
                     * Сбор статистики по родству осуществляем только сейчас, 
                     * т.к. некоторые значения могут меняться из-за relative--.
                     */
                    for (int person = 0;
                        person < generatedOutputMatrix.GetLength(0);
                        person++)
                    {
                        for (int relative = 0;
                            relative < generatedOutputMatrix.GetLength(0);
                            relative++)
                        {
                            for (int probandsRelatioship = 0;
                                probandsRelatioship < relationshipsMatrix.GetLength(1);
                                probandsRelatioship++)
                            {
                                if (generatedOutputMatrix[person][relative] == relationshipsMatrix[numberOfProband, probandsRelatioship][0])
                                {
                                    quantityOfEachRelationship[probandsRelatioship]++;
                                }
                            }
                        }
                    }

                    /*
                     * Преобразовываем матрицу так, 
                     * чтобы не было разрывов между номерами степеней родства.
                     */
                    for (int person = 0;
                        person < generatedOutputMatrix.GetLength(0);
                        person++)
                    {
                        for (int relative = 0;
                            relative < generatedOutputMatrix.GetLength(0);
                            relative++)
                        {
                            for (int relationship = 0;
                                relationship < relationshipsMatrix.GetLength(1);
                                relationship++)
                            {
                                if (relationshipsMatrix[numberOfProband, relationship][0] == generatedOutputMatrix[person][relative])
                                {
                                    /*
                                     * Делаем +1, чтобы нумерация значащих степеней родства шла с 1.
                                     */
                                    generatedOutputMatrix[person][relative] = relationship + 1;
                                    break;
                                }
                            }
                        }
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

                /*
                 * Вывод статистики по родству.
                 */
                int relationshipNumber = 0;

                foreach (var quantity in quantityOfEachRelationship)
                {
                    textBox2.Text += "Родство " + relationshipsMatrix[numberOfProband, relationshipNumber][0] + ": " + quantity + Environment.NewLine;

                    relationshipNumber++;
                }
            }
        }
    }
}
