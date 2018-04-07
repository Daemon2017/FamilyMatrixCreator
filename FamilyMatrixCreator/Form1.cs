using System;
using System.IO;
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
        int[] relationshipsHelper;
        int[][] ancestorsMatrix;
        int[][] descendantsMatrix;
        int numberOfProband;
        int[][] generatedMatrix;

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
             * Загрузка матрицы возможных степеней родства
             * Определение номера строки, содержащей возможные степени родства пробанда
             */
            int i = 0,
                j = 0,
                k = 0;
            string input = File.ReadAllText(@"relationships.csv");
            int numberOfLines = input.Split('\n').Length - 1;
            int quantityOfCells = 0;
            relationshipsMatrix = new int[numberOfLines, numberOfLines][];

            foreach (var row in input.Split('\n'))
            {
                j = 0;
                int counter = 0;

                if (!(row.Equals("")) && !(row.Equals("\r")))
                {
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

            /*
             * Загрузка указателя к матрице возможных степеней родства
             */
            i = 0;
            input = File.ReadAllText(@"relationshipsHelper.csv");
            numberOfLines = input.Split('\n').Length - 1;
            quantityOfCells = 0;
            relationshipsHelper = new int[numberOfLines];

            foreach (var row in input.Split('\n'))
            {
                if (row != "")
                {
                    relationshipsHelper[i] = int.Parse(row.Trim());
                }

                i++;
            }

            /*
             * Загрузка матрицы предковых степеней родства
             */
            i = 0;
            j = 0;
            input = File.ReadAllText(@"ancestors.csv");
            numberOfLines = input.Split('\n').Length - 1;
            quantityOfCells = 0;
            ancestorsMatrix = new int[numberOfLines][];

            foreach (var row in input.Split('\n'))
            {
                j = 0;
                int counter = 0;

                if (!(row.Equals("")) && !(row.Equals("\r")))
                {
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

            /*
             * Загрузка матрицы потомковых степеней родства
             */
            i = 0;
            j = 0;
            input = File.ReadAllText(@"descendants.csv");
            numberOfLines = input.Split('\n').Length - 1;
            quantityOfCells = 0;
            descendantsMatrix = new int[numberOfLines][];

            foreach (var row in input.Split('\n'))
            {
                j = 0;
                int counter = 0;

                if (!(row.Equals("")) && !(row.Equals("\r")))
                {
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
            generatedMatrix = new int[100][];
            Random rnd = new Random();

            for (int i = 0; i < 100; i++)
            {
                generatedMatrix[i] = new int[100];

                for (int j = i; j < 100; j++)
                {
                    int rndValue = rnd.Next(relationshipsMatrix.GetLength(1));
                    generatedMatrix[i][j] = relationshipsMatrix[numberOfProband, rndValue][0];
                }
            }
        }
    }
}
