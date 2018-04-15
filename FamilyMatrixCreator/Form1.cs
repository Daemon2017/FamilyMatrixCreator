﻿using System;
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
            int person = 0,
                relative = 0,
                relationship = 0;
            string input = File.ReadAllText(@"relationships.csv");
            int numberOfLines = input.Split('\n').Length - 1;
            int quantityOfCells = 0;
            relationshipsMatrix = new int[numberOfLines, numberOfLines][];

            /*
             * Загрузка матрицы возможных степеней родства.
             */
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

            person = 0;
            relative = 0;
            input = File.ReadAllText(@"ancestors.csv");
            numberOfLines = input.Split('\n').Length - 1;
            quantityOfCells = 0;
            ancestorsMatrix = new int[numberOfLines][];

            /*
             * Загрузка матрицы предковых степеней родства.
             */
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

            person = 0;
            relative = 0;
            input = File.ReadAllText(@"descendants.csv");
            numberOfLines = input.Split('\n').Length - 1;
            quantityOfCells = 0;
            descendantsMatrix = new int[numberOfLines][];

            /*
             * Загрузка матрицы потомковых степеней родства.
             */
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
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int generatedMatrixSize = 100;
            int[][] generatedMatrix = new int[generatedMatrixSize][];
            Random rnd = new Random();

            /*
             * Построение матрицы родственных связей.
             */
            for (int person = 0;
                person < generatedMatrixSize;
                person++)
            {
                generatedMatrix[person] = new int[generatedMatrixSize];

                for (int relative = person;
                    relative < generatedMatrixSize;
                    relative++)
                {
                    if (0 == person)
                    {
                        /*
                         * Создание случайных степеней родства для пробанда.
                         */
                        int randomRelative = rnd.Next(relationshipsMatrix.GetLength(1));
                        generatedMatrix[person][relative] = relationshipsMatrix[numberOfProband, randomRelative][0];

                        int numberOfPerson = 0;

                        /*
                         * Среди возможных степеней родства пробанда ищется порядковый номер того,
                         * что содержит выбранную степень родства.
                         */
                        for (int number = 0;
                            number < relationshipsMatrix.GetLength(1);
                            number++)
                        {
                            if (relationshipsMatrix[numberOfProband, number][0] == generatedMatrix[person][relative])
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
                                if (relationshipsMatrix[numberOfProband, number][0] == generatedMatrix[k][person])
                                {
                                    numberOfI = number;
                                }

                                if (relationshipsMatrix[numberOfProband, number][0] == generatedMatrix[k][relative])
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

                        int randomRelative = rnd.Next(allPossibleRelationships.GetLength(0));
                        generatedMatrix[person][relative] = allPossibleRelationships[randomRelative];
                    }

                    if (person >= 0 && relative >= 0)
                    {
                        if (person == relative)
                        {
                            /*
                             * Заполнение диагонали единицами.
                             */
                            generatedMatrix[person][relative] = 1;
                        }
                        else
                        {
                            /*
                             * Недопущение появления единиц где-либо, 
                             * кроме диагонали.
                             */
                            if (1 == generatedMatrix[person][relative])
                            {
                                relative--;
                            }
                        }
                    }
                }
            }

            /*
             * Сохранение матрицы в файл.
             */
            using (StreamWriter outfile = new StreamWriter(@"generated.csv"))
            {
                for (int person = 0;
                    person < generatedMatrix.GetLength(0);
                    person++)
                {
                    string content = "";

                    for (int relative = 0;
                        relative < generatedMatrix[person].GetLength(0);
                        relative++)
                    {
                        string temp = generatedMatrix[person][relative].ToString();

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
