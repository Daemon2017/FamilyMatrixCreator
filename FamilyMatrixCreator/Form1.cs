﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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
        int[] clustersMatrix;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
             * Загрузка матрицы возможных степеней родства.
             */
            int person = 0,
                relative = 0,
                relationship = 0;
            string input = File.ReadAllText(@"relationships.csv");
            int numberOfLines = input.Split('\n').Length - 1;
            int quantityOfCells = 0;
            relationshipsMatrix = new int[numberOfLines, numberOfLines][];

            foreach (var row in input.Split('\n'))
            {
                relative = 0;
                int counter = 0;

                if (!(row.Equals("")) && !(row.Equals("\r")))
                {
                    /*
                     * Определение номера строки, содержащей возможные степени родства пробанда.
                     */
                    foreach (Match m in Regex.Matches(row, ";"))
                    {
                        counter++;
                    }

                    if (0 == counter)
                    {
                        numberOfProband = person;
                    }

                    foreach (var column in row.Trim().Split(','))
                    {
                        relationship = 0;
                        counter = 0;

                        /*
                         * Определение числа возможных степеней родства. 
                         */
                        foreach (Match m in Regex.Matches(column, ";"))
                        {
                            counter++;
                        }

                        if (counter > quantityOfCells)
                        {
                            quantityOfCells = counter;
                        }

                        relationshipsMatrix[person, relative] = new int[quantityOfCells + 1];
                        quantityOfCells = 0;

                        foreach (var cell in column.Trim().Split(';'))
                        {
                            if (cell != "")
                            {
                                relationshipsMatrix[person, relative][relationship] = int.Parse(cell.Trim());
                            }

                            relationship++;
                        }

                        relative++;
                    }
                }

                person++;
            }

            /*
             * Загрузка матрицы предковых степеней родства.
             */
            person = 0;
            relative = 0;
            input = File.ReadAllText(@"ancestors.csv");
            numberOfLines = input.Split('\n').Length - 1;
            quantityOfCells = 0;
            ancestorsMatrix = new int[numberOfLines][];

            foreach (var row in input.Split('\n'))
            {
                relative = 0;
                int counter = 0;

                if (!(row.Equals("")) && !(row.Equals("\r")))
                {
                    /*
                     * Определение количества степеней родства, 
                     * приходящихся предковыми текущей степени родства.
                     */
                    foreach (Match m in Regex.Matches(row, ","))
                    {
                        counter++;
                    }

                    if (counter > quantityOfCells)
                    {
                        quantityOfCells = counter;
                    }

                    ancestorsMatrix[person] = new int[quantityOfCells + 1];
                    quantityOfCells = 0;

                    foreach (var column in row.Trim().Split(','))
                    {
                        if (column != "")
                        {
                            ancestorsMatrix[person][relative] = int.Parse(column.Trim());
                        }

                        relative++;
                    }
                }

                person++;
            }

            /*
             * Загрузка матрицы потомковых степеней родства.
             */
            person = 0;
            relative = 0;
            input = File.ReadAllText(@"descendants.csv");
            numberOfLines = input.Split('\n').Length - 1;
            quantityOfCells = 0;
            descendantsMatrix = new int[numberOfLines][];

            foreach (var row in input.Split('\n'))
            {
                relative = 0;
                int counter = 0;

                if (!(row.Equals("")) && !(row.Equals("\r")))
                {
                    /*
                     * Определение количества степеней родства, 
                     * приходящихся потомковыми текущей степени родства.
                     */
                    foreach (Match m in Regex.Matches(row, ","))
                    {
                        counter++;
                    }

                    if (counter > quantityOfCells)
                    {
                        quantityOfCells = counter;
                    }

                    descendantsMatrix[person] = new int[quantityOfCells + 1];
                    quantityOfCells = 0;

                    foreach (var column in row.Trim().Split(','))
                    {
                        if (column != "")
                        {
                            descendantsMatrix[person][relative] = int.Parse(column.Trim());
                        }

                        relative++;
                    }
                }

                person++;
            }

            /*
            * Загрузка матрицы значений сантиморган.
            */
            person = 0;
            input = File.ReadAllText(@"centimorgans.csv");
            numberOfLines = input.Split('\n').Length - 1;
            centimorgansMatrix = new float[numberOfLines];

            foreach (var row in input.Split('\n'))
            {
                if (!(row.Equals("")) && !(row.Equals("\r")))
                {
                    centimorgansMatrix[person] = float.Parse(row);
                }

                person++;
            }

            /*
             * Загрузка матрицы принадлежности к кластерам.
             */
            person = 0;
            input = File.ReadAllText(@"clusters.csv");
            numberOfLines = input.Split('\n').Length - 1;
            clustersMatrix = new int[numberOfLines];

            foreach (var row in input.Split('\n'))
            {
                if (!(row.Equals("")) && !(row.Equals("\r")))
                {
                    clustersMatrix[person] = int.Parse(row);
                }

                person++;
            }
        }

        private void Generate(object sender, EventArgs e)
        {
            int quantityOfMatrixes = Convert.ToInt16(textBox1.Text);
            int[] quantityOfEachRelationship = new int[relationshipsMatrix.GetLength(1)];
            textBox2.Text = "";

            if (quantityOfMatrixes > 0)
            {
                for (int matrix = 0; matrix < quantityOfMatrixes; matrix++)
                {
                    int generatedMatrixSize = 100;
                    int[][] generatedOutputMatrix = new int[generatedMatrixSize][];

                    /*
                     * Построение матрицы родственных связей.
                     */
                    for (int person = 0;
                        person < generatedOutputMatrix.GetLength(0);
                        person++)
                    {
                        generatedOutputMatrix[person] = new int[generatedOutputMatrix.GetLength(0)];

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
                                 * Устранение нулевых степеней родства везде, где это возможно.
                                 */
                                if (true == checkBox1.Checked)
                                {
                                    bool relationZeroExists = false;
                                    bool relationOneExists = false;

                                    /*
                                     * Степень родства вида "Пробанд" (1) впоследствии будет устраняться,
                                     * поэтому важно проверить, присутствует ли оно в списке допустимых степеней родства.
                                     */
                                    foreach (var relationship in allPossibleRelationships)
                                    {
                                        if (0 == relationship)
                                        {
                                            relationZeroExists = true;
                                        }
                                        if (1 == relationship)
                                        {
                                            relationOneExists = true;
                                        }
                                    }

                                    /*
                                     * Если присутствует - подвергаем чистке только те списки,
                                     * где помимо "0" и "1" может быть, как минимум, еще одна степень родства.
                                     */
                                    if (true == relationZeroExists)
                                    {
                                        if (true == relationOneExists)
                                        {
                                            if (allPossibleRelationships.GetLength(0) > 2)
                                            {
                                                allPossibleRelationships = allPossibleRelationships.Where(val => val != 0).ToArray();
                                            }
                                        }
                                        else
                                        {
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

                            if (person >= 0 && relative >= 0)
                            {
                                if (person == relative)
                                {
                                    /*
                                     * Заполнение диагонали единицами.
                                     */
                                    generatedOutputMatrix[person][relative] = 1;
                                }
                                else
                                {
                                    /*
                                     * Недопущение появления единиц где-либо, 
                                     * кроме диагонали.
                                     */
                                    if (1 == generatedOutputMatrix[person][relative])
                                    {
                                        relative--;
                                    }
                                }
                            }
                        }
                    }

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

                    if (true == checkBox2.Checked)
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
                                    relationship < relationshipsMatrix.GetLength(1);
                                    relationship++)
                                {
                                    if (relationshipsMatrix[numberOfProband, relationship][0] == generatedOutputMatrix[person][relative])
                                    {
                                        generatedOutputMatrix[person][relative] = clustersMatrix[relationship];
                                    }
                                }
                            }
                        }
                    }

                    Directory.CreateDirectory("input");

                    /*
                     * Сохранение входной матрицы в файл.
                     */
                    using (StreamWriter outfile = new StreamWriter(@"input\generated_input" + matrix + ".csv"))
                    {
                        for (int person = 0;
                            person < generatedInputMatrix.GetLength(0);
                            person++)
                        {
                            string content = "";

                            for (int relative = 0;
                                relative < generatedInputMatrix[person].GetLength(0);
                                relative++)
                            {
                                string temp = generatedInputMatrix[person][relative].ToString();

                                if (temp != null)
                                {
                                    content += temp + ",";
                                }
                            }

                            if (content != "")
                            {
                                content = content.Remove(content.Length - 1);
                            }

                            outfile.WriteLine(content);
                        }
                    }

                    Directory.CreateDirectory("output");

                    /*
                     * Сохранение выходной матрицы в файл.
                     */
                    using (StreamWriter outfile = new StreamWriter(@"output\generated_output" + matrix + ".csv"))
                    {
                        for (int person = 0;
                            person < generatedOutputMatrix.GetLength(0);
                            person++)
                        {
                            string content = "";

                            for (int relative = 0;
                                relative < generatedOutputMatrix[person].GetLength(0);
                                relative++)
                            {
                                string temp = generatedOutputMatrix[person][relative].ToString();

                                if (temp != null)
                                {
                                    content += temp + ",";
                                }
                            }

                            if (content != "")
                            {
                                content = content.Remove(content.Length - 1);
                            }

                            outfile.WriteLine(content);
                        }
                    }

                    /*
                     * Сбор статистики по родству.
                     */
                    if (true == checkBox2.Checked)
                    {

                    }
                    else
                    {
                        for (int person = 0; person < generatedOutputMatrix.GetLength(0); person++)
                        {
                            for (int relative = 0; relative < generatedOutputMatrix.GetLength(0); relative++)
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
                    }
                }

                /*
                 * Вывод статистики по родству.
                 */
                if (true == checkBox2.Checked)
                {

                }
                else
                {
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
}
