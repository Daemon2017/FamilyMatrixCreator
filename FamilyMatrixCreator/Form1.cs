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

        int[,,] relationshipsMatrix;
        int[,] ancestorsMatrix;
        int[,] descendantsMatrix;

        private void Form1_Load(object sender, EventArgs e)
        {
            string input = File.ReadAllText(@"relationships.csv");
            int numberOfLines = input.Split('\n').Length - 1;
            int numberOfCells = 0;

            foreach (var row in input.Split('\n'))
            {
                foreach (var col in row.Trim().Split(','))
                {
                    int count = 0;

                    foreach (Match m in Regex.Matches(col, ";"))
                    {
                        count++;
                    }

                    if (count > numberOfCells)
                    {
                        numberOfCells = count;
                    }
                }
            }

            relationshipsMatrix = new int[numberOfLines, numberOfLines, numberOfCells + 1];

            int i = 0,
                j = 0,
                k = 0;

            foreach (var row in input.Split('\n'))
            {
                j = 0;

                foreach (var col in row.Trim().Split(','))
                {
                    k = 0;

                    foreach (var subcol in col.Trim().Split(';'))
                    {
                        if (subcol != "")
                        {
                            relationshipsMatrix[i, j, k] = int.Parse(subcol.Trim());
                        }

                        k++;
                    }

                    j++;
                }

                i++;
            }

            input = File.ReadAllText(@"ancestors.csv");
            numberOfLines = input.Split('\n').Length - 1;
            numberOfCells = 0;

            foreach (var row in input.Split('\n'))
            {
                int count = 0;

                foreach (Match m in Regex.Matches(row, ","))
                {
                    count++;
                }

                if (count > numberOfCells)
                {
                    numberOfCells = count;
                }
            }

            ancestorsMatrix = new int[numberOfLines, numberOfCells + 1];

            i = 0;
            j = 0;

            foreach (var row in input.Split('\n'))
            {
                j = 0;

                foreach (var col in row.Trim().Split(','))
                {
                    if (col != "")
                    {
                        ancestorsMatrix[i, j] = int.Parse(col.Trim());
                    }

                    j++;
                }

                i++;
            }

            input = File.ReadAllText(@"descendants.csv");
            numberOfLines = input.Split('\n').Length - 1;
            numberOfCells = 0;

            foreach (var row in input.Split('\n'))
            {
                int count = 0;

                foreach (Match m in Regex.Matches(row, ","))
                {
                    count++;
                }

                if (count > numberOfCells)
                {
                    numberOfCells = count;
                }
            }

            descendantsMatrix = new int[numberOfLines, numberOfCells + 1];

            i = 0;
            j = 0;

            foreach (var row in input.Split('\n'))
            {
                j = 0;

                foreach (var col in row.Trim().Split(','))
                {
                    if (col != "")
                    {
                        descendantsMatrix[i, j] = int.Parse(col.Trim());
                    }

                    j++;
                }

                i++;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
