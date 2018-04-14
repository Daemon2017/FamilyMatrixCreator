using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FamilyMatrixCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int[,][] relationshipsMatrix;
        int[][] ancestorsMatrix;
        int[][] descendantsMatrix;
        int numberOfProband;

        private void Form1_Load(object sender, EventArgs e)
        {
            int i = 0,
                j = 0,
                k = 0;
            string input = File.ReadAllText(@"relationships.csv");
            int numberOfLines = input.Split('\n').Length - 1;
            int quantityOfCells = 0;
            relationshipsMatrix = new int[numberOfLines, numberOfLines][];

            /*
             * Загрузка матрицы возможных степеней родства.
             */
            foreach (var row in input.Split('\n'))
            {
                j = 0;
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
                        numberOfProband = i;
                    }

                    foreach (var column in row.Trim().Split(','))
                    {
                        k = 0;
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

                        relationshipsMatrix[i, j] = new int[quantityOfCells + 1];
                        quantityOfCells = 0;

                        foreach (var cell in column.Trim().Split(';'))
                        {
                            if (cell != "")
                            {
                                relationshipsMatrix[i, j][k] = int.Parse(cell.Trim());
                            }

                            k++;
                        }

                        j++;
                    }
                }

                i++;
            }

            i = 0;
            j = 0;
            input = File.ReadAllText(@"ancestors.csv");
            numberOfLines = input.Split('\n').Length - 1;
            quantityOfCells = 0;
            ancestorsMatrix = new int[numberOfLines][];

            /*
             * Загрузка матрицы предковых степеней родства.
             */
            foreach (var row in input.Split('\n'))
            {
                j = 0;
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

                    ancestorsMatrix[i] = new int[quantityOfCells + 1];
                    quantityOfCells = 0;

                    foreach (var column in row.Trim().Split(','))
                    {
                        if (column != "")
                        {
                            ancestorsMatrix[i][j] = int.Parse(column.Trim());
                        }

                        j++;
                    }
                }

                i++;
            }

            i = 0;
            j = 0;
            input = File.ReadAllText(@"descendants.csv");
            numberOfLines = input.Split('\n').Length - 1;
            quantityOfCells = 0;
            descendantsMatrix = new int[numberOfLines][];

            /*
             * Загрузка матрицы потомковых степеней родства.
             */
            foreach (var row in input.Split('\n'))
            {
                j = 0;
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

                    descendantsMatrix[i] = new int[quantityOfCells + 1];
                    quantityOfCells = 0;

                    foreach (var column in row.Trim().Split(','))
                    {
                        if (column != "")
                        {
                            descendantsMatrix[i][j] = int.Parse(column.Trim());
                        }

                        j++;
                    }
                }

                i++;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int[][] generatedMatrix = new int[10][];
            Random rnd = new Random();

            /*
             * Построение матрицы родственных связей.
             */
            for (int i = 0; i < 10; i++)
            {
                generatedMatrix[i] = new int[10];

                for (int j = i; j < 10; j++)
                {
                    if (0 == i)
                    {
                        /*
                         * Создание случайных степеней родства для пробанда.
                         */
                        int rndValueY = rnd.Next(relationshipsMatrix.GetLength(1));
                        generatedMatrix[i][j] = relationshipsMatrix[numberOfProband, rndValueY][0];
                    }
                    else
                    {
                        int[] allPossibleRelationships = { 0 };

                        /*
                         * Исключение невозможных степеней родства.
                         */
                        for (int k = 0; k < i; k++)
                        {
                            int numberOfI = 0,
                                numberOfJ = 0;

                            /*
                             * Среди возможных степеней родства пробанда ищутся порядковые номера тех,
                             * что содержат выбранные степени родства.
                             */
                            for (int l = 0; l < relationshipsMatrix.GetLength(1); l++)
                            {
                                if (relationshipsMatrix[numberOfProband, l][0] == generatedMatrix[k][i])
                                {
                                    numberOfI = l;
                                }

                                if (relationshipsMatrix[numberOfProband, l][0] == generatedMatrix[k][j])
                                {
                                    numberOfJ = l;
                                }
                            }

                            if (0 == k)
                            {
                                allPossibleRelationships = relationshipsMatrix[numberOfI, numberOfJ];

                                /*
                                 * Исключение возможных степеней родства, 
                                 * которые невозможно сгенерировать.
                                 */
                                for (int m = 0; m < allPossibleRelationships.GetLength(0); m++)
                                {
                                    bool isRelationshipAllowed = false;

                                    for (int n = 0; n < relationshipsMatrix.GetLength(1); n++)
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
                                for (int m = 0; m < allPossibleRelationships.GetLength(0); m++)
                                {
                                    bool isRelationshipAllowed = false;

                                    for (int n = 0; n < currentPossibleRelationships.GetLength(0); n++)
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

                        int rndValueY = rnd.Next(allPossibleRelationships.GetLength(0));
                        generatedMatrix[i][j] = allPossibleRelationships[rndValueY];
                    }

                    if (i == j)
                    {
                        generatedMatrix[i][j] = 1;
                    }
                    else
                    {
                        if (1 == generatedMatrix[i][j])
                        {
                            j--;
                        }
                    }
                }
            }

            using (StreamWriter outfile = new StreamWriter(@"generated.csv"))
            {
                for (int x = 0; x < generatedMatrix.GetLength(0); x++)
                {
                    string content = "";

                    for (int y = 0; y < generatedMatrix[x].GetLength(0); y++)
                    {
                        string temp = generatedMatrix[x][y].ToString();

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
        }
    }
}
