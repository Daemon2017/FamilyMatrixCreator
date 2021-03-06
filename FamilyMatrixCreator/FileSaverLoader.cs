﻿using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FamilyMatrixCreator
{
    public static class FileSaverLoader
    {
        public static void SaveToFile(string outputFileName, float[][] generatedMatrix, int matrixNumber)
        {
            using (StreamWriter outfile = new StreamWriter(outputFileName + matrixNumber + ".csv"))
            {
                foreach (float[] raw in generatedMatrix)
                {
                    string content = raw.Select(column => column.ToString())
                        .Aggregate("", (current, temp) => current + temp + ",");

                    if (content != "")
                    {
                        content = content.Remove(content.Length - 1);
                    }

                    outfile.WriteLine(content);
                }
            }
        }

        /*
         * Загрузка матрицы возможных степеней родства.
         */
        public static int[,][] LoadFromFile2dJagged(string inputFileName)
        {
            string input = File.ReadAllText(inputFileName);
            int numberOfLines = input.Split('\n').Length - 1;
            int quantityOfCells = 0;
            int[,][] matrix = new int[numberOfLines, numberOfLines][];

            {
                int person = 0;

                foreach (var row in input.Split('\n'))
                {
                    int relative = 0;

                    if (!row.Equals("") && !row.Equals("\r"))
                    {
                        foreach (var column in row.Trim().Split(','))
                        {
                            int relationship = 0;
                            int counter = 0;

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
            }

            for (int person = 0; person < matrix.GetLength(0); person++)
            {
                for (int relative = 0; relative < matrix.GetLength(0); relative++)
                {
                    matrix[person, relative] = matrix[person, relative].Distinct().ToArray();
                }
            }

            return matrix;
        }

        /*
        * Загрузка двумерной матрицы из файла.
        */
        public static int[][] LoadFromFile2dInt(string inputFileName)
        {
            int relationship = 0;
            string input = File.ReadAllText(inputFileName);
            int numberOfLines = input.Split('\n').Length - 1;
            int[][] matrix = new int[numberOfLines][];

            foreach (var row in input.Split('\n'))
            {
                int count = 0;

                if (!row.Equals("") && !row.Equals("\r"))
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

        /*
        * Загрузка одномерной матрицы из файла.
        */
        public static float[] LoadFromFile1dFloat(string inputFileName)
        {
            int person = 0;
            string input = File.ReadAllText(inputFileName);
            int numberOfLines = input.Split('\n').Length - 1;
            float[] matrix = new float[numberOfLines];

            foreach (var row in input.Split('\n'))
            {
                if (!row.Equals("") && !row.Equals("\r"))
                {
                    matrix[person] = float.Parse(row);
                }

                person++;
            }

            return matrix;
        }

        public static int[] LoadFromFile1dInt(string inputFileName)
        {
            int person = 0;
            string input = File.ReadAllText(inputFileName);
            int numberOfLines = input.Split('\n').Length - 1;
            int[] matrix = new int[numberOfLines];

            foreach (var row in input.Split('\n'))
            {
                if (!row.Equals("") && !row.Equals("\r"))
                {
                    matrix[person] = int.Parse(row);
                }

                person++;
            }

            return matrix;
        }
    }
}