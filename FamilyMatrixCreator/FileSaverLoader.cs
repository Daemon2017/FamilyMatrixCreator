using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FamilyMatrixCreator
{
    public partial class Form1 : Form
    {
        private void SaveToFile(string outputFileName, float[][] generatedMatrix, int matrixNumber)
        {
            using (StreamWriter outfile = new StreamWriter(outputFileName + matrixNumber + ".csv"))
            {
                foreach (float[] raw in generatedMatrix)
                {
                    string content = "";

                    foreach (float column in raw)
                    {
                        string temp = column.ToString();

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

        private void SaveToFile(string outputFileName, List<int[]> complianceMatrix)
        {
            using (StreamWriter outfile = new StreamWriter(outputFileName))
            {
                foreach (var relationship in complianceMatrix)
                {
                    string content = "";

                    content += relationship[0].ToString() + ", " + relationship[1].ToString();

                    outfile.WriteLine(content);
                }
            }
        }

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

        private int[][] LoadFromFile2D(string inputFileName)
        {
            int relationship = 0;
            string input = File.ReadAllText(inputFileName);
            int numberOfLines = input.Split('\n').Length - 1;
            int[][] matrix = new int[numberOfLines][];

            foreach (var row in input.Split('\n'))
            {
                int count = 0;

                if (!(row.Equals("")) && !(row.Equals("\r")))
                {
                    matrix[relationship] = new int[2];

                    foreach (var column in row.Trim().Split(','))
                    {
                        matrix[relationship][count] = int.Parse(column.Trim());

                        count++;
                    }
                }

                relationship++;
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
            * Загрузка матрицы значений сантиморган.
            */
            centimorgansMatrix = LoadFromFile1D("centimorgans.csv");

            /*
            * Загрузка матрицы максимального числа предков заданного вида.
            */
            maxCountMatrix = LoadFromFile2D("maxCount.csv");
        }
    }
}
