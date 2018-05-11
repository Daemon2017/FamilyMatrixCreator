using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FamilyMatrixCreator
{
    public partial class Form1 : Form
    {
        private int[,][] LoadFromFile2DJagged(string inputFileName)
        {
            int person = 0,
                relative = 0,
                relationship = 0;
            string input = File.ReadAllText(inputFileName);
            int numberOfLines = input.Split('\n').Length - 1;
            int quantityOfCells = 0;
            int[,][] matrix = new int[numberOfLines, numberOfLines][];

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

                        matrix[person, relative] = new int[quantityOfCells + 1];
                        quantityOfCells = 0;

                        foreach (var cell in column.Trim().Split(';'))
                        {
                            if (cell != "")
                            {
                                matrix[person, relative][relationship] = int.Parse(cell.Trim());
                            }

                            relationship++;
                        }

                        relative++;
                    }
                }

                person++;
            }

            return matrix;
        }

        private int[][] LoadFromFile1DJagged(string inputFileName)
        {
            int person = 0,
                relative = 0;
            string input = File.ReadAllText(inputFileName);
            int numberOfLines = input.Split('\n').Length - 1;
            int quantityOfCells = 0;
            int[][] matrix = new int[numberOfLines][];

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

                    matrix[person] = new int[quantityOfCells + 1];
                    quantityOfCells = 0;

                    foreach (var column in row.Trim().Split(','))
                    {
                        if (column != "")
                        {
                            matrix[person][relative] = int.Parse(column.Trim());
                        }

                        relative++;
                    }
                }

                person++;
            }

            return matrix;
        }

        private float[] LoadFromFile1D(string inputFileName)
        {
            int person = 0;
            string input = File.ReadAllText(inputFileName);
            int numberOfLines = input.Split('\n').Length - 1;
            float[] matrix = new float[numberOfLines];

            foreach (var row in input.Split('\n'))
            {
                if (!(row.Equals("")) && !(row.Equals("\r")))
                {
                    matrix[person] = float.Parse(row);
                }

                person++;
            }

            return matrix;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
             * Загрузка матрицы возможных степеней родства.
             */
            relationshipsMatrix = LoadFromFile2DJagged("relationships.csv");

            /*
             * Загрузка матрицы предковых степеней родства.
             */
            ancestorsMatrix = LoadFromFile1DJagged("ancestors.csv");

            /*
             * Загрузка матрицы потомковых степеней родства.
             */
            descendantsMatrix = LoadFromFile1DJagged("descendants.csv");

            /*
            * Загрузка матрицы значений сантиморган.
            */
            centimorgansMatrix = LoadFromFile1D("centimorgans.csv");

            /*
             * Загрузка матрицы принадлежности к кластерам.
             */
            clustersMatrix = LoadFromFile1D("clusters.csv");
        }

    }
}
