using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZGraphTools;

namespace ImageFilters
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ImageOperations x = new ImageOperations();
        byte[,] ImageMatrix;
        string OpenedFilePath;
        int trim;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);

            }
        }

        private void btnZGraph_Click(object sender, EventArgs e)
        {
            int MwinSize = Convert.ToInt32(textBox2.Text);
            
                double[] x_axis = new double[MwinSize / 2];
                double[] y_Kth = new double[MwinSize / 2];
                double[] y_CS = new double[MwinSize / 2];
                double[] y_MedianQS = new double[MwinSize / 2];
                double[] y_MedCS = new double[MwinSize / 2];

                int counter = 0;
                double timeBefore;
                double timeAfter;
            if (MwinSize % 2 != 0)
            {
                for (int i = 3; i <= MwinSize; i += 2)
                {
                    x_axis[counter] = i;
                    counter++;
                }
                // counting sort

                if (OpenedFilePath != null)
                {
                    for (int i = 0; i < x_axis.Length; i++)
                    {
                        int t = (int)x_axis[i];

                        byte[,] temp1 = new byte[ImageMatrix.GetLength(0), ImageMatrix.GetLength(1)];
                        byte[,] temp2 = new byte[ImageMatrix.GetLength(0), ImageMatrix.GetLength(1)];
                        byte[,] temp3 = new byte[ImageMatrix.GetLength(0), ImageMatrix.GetLength(1)];
                        byte[,] temp4 = new byte[ImageMatrix.GetLength(0), ImageMatrix.GetLength(1)];


                        for (int k = 0; k < ImageMatrix.GetLength(0); k++)
                        {
                            for (int l = 0; l < ImageMatrix.GetLength(1); l++)
                            {
                                temp1[k, l] = ImageMatrix[k, l];
                                temp2[k, l] = ImageMatrix[k, l];
                                temp3[k, l] = ImageMatrix[k, l];
                                temp4[k, l] = ImageMatrix[k, l];
                            }

                        }


                        timeBefore = System.Environment.TickCount;
                        x.AlphaFilter(t, t * t, temp1, 1);
                        timeAfter = System.Environment.TickCount;
                        y_CS[i] = timeAfter - timeBefore;

                        timeBefore = System.Environment.TickCount;
                        x.AlphaFilter(t, t * t, temp2, 2);
                        timeAfter = System.Environment.TickCount;
                        y_Kth[i] = timeAfter - timeBefore;

                        timeBefore = System.Environment.TickCount;
                        x.Adeptive_median(t, t * t, temp3, t, 0);
                        timeAfter = System.Environment.TickCount;
                        y_MedCS[i] = timeAfter - timeBefore;

                        timeBefore = System.Environment.TickCount;
                        x.Adeptive_median(t, t * t, temp4, t, 1);
                        timeAfter = System.Environment.TickCount;
                        y_MedianQS[i] = timeAfter - timeBefore;
                    }

                    ZGraphForm ZGF = new ZGraphForm("Alpha trim Graph", "Win size", "time");
                    ZGF.add_curve("counting sort", x_axis, y_CS, Color.Yellow);
                    ZGF.add_curve("kth", x_axis, y_Kth, Color.Black);
                    ZGF.Show();

                    ZGraphForm ZGF2 = new ZGraphForm("Adeptive Median Graph", "Win size", "Time");
                    ZGF2.add_curve("counting sort", x_axis, y_MedCS, Color.Green);
                    ZGF2.add_curve("quick sort", x_axis, y_MedianQS, Color.Black);
                    ZGF2.Show();

                }
            }

            else
            {
                label5.Text = "Ws must be odd";            }




            //Create a graph and add two curves to it
            //ZGF.add_curve("f(N) = N Log(N)", x_values, y_values_NLogN, Color.Blue);
        }

        private void button1_Click(object sender, EventArgs e)
        {
             trim = Convert.ToInt32(textBox1.Text);
            int Windowsize = Convert.ToInt32(textBox3.Text);

            if (trim % 2 == 0 || Windowsize % 2 ==0) {

                string mess = "trim value and win size must be odd number";
                label4.Text = mess;
            }
            else
            {
                label4.Text = " ";
                Windowsize = trim * trim;
                
                if(OpenedFilePath != null)
                {
                    if (comboBox1.SelectedIndex == 0)
                    {
                        byte[,] newImage = new byte[ImageMatrix.GetLength(0), ImageMatrix.GetLength(1)];
                         x.AlphaFilter(trim, Windowsize, ImageMatrix, 1);
                        ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
                    }
                    else if (comboBox1.SelectedIndex == 1)
                    {
                        byte[,] newImage = new byte[ImageMatrix.GetLength(0), ImageMatrix.GetLength(1)];
                       
                        x.AlphaFilter(trim, Windowsize, ImageMatrix, 2);
                        ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
                    }
                    else if (comboBox1.SelectedIndex == 2)
                    {
                        //byte[,] newImage = new byte[ImageMatrix.GetLength(0), ImageMatrix.GetLength(1)];
                        x.Adeptive_median(trim, Windowsize, ImageMatrix, trim, 0);
                        ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
                    }
                    else if (comboBox1.SelectedIndex == 3)
                    {
                        //byte[,] newImage = new byte[ImageMatrix.GetLength(0), ImageMatrix.GetLength(1)];
                        x.Adeptive_median(trim, Windowsize, ImageMatrix, trim, 1);
                        ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
                    }
                }
            }
            

            

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}