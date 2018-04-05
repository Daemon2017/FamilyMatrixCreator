using System;
using System.IO;
using System.Windows.Forms;

namespace FamilyMatrixCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int[,,] relationshipsMatrix = new int[42, 42, 10];
        int[,] ancestorsMatrix = new int[42, 42];
        int[,] descendantsMatrix = new int[42, 42];

        private void Form1_Load(object sender, EventArgs e)
        {
            string input = File.ReadAllText(@"relationships.csv");

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
