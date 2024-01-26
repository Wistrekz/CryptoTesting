using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void File_check_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();    //Открытие и настройка поля для выбора файла
            openFileDialog1.Title = "Select File";
            openFileDialog1.InitialDirectory = @"C:\";//--"C:\\";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            { 
                textBox1.Text = openFileDialog1.FileName; 
            }
            else
            {
                textBox1.Text = "";
            }
        }

        private void Byte_Generate_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text)) //Проверка на существование файла
            {
                FileStream Workfile = new FileStream(textBox1.Text, FileMode.Open);   //
                long LenBytes = Workfile.Length;                                      //
                double a = Convert.ToDouble(Workfile.Length) / 1024 / 1024;           // Открытие файла и подсчёт байтов для вывода         
                int b = 0;                                                            //
                if (Math.Round(a - 0.5) == Math.Round(a))                             //
                {
                    b = (int)Math.Round(a);
                }
                else
                {
                    b = (int)Math.Round(a) - 1;
                }
                if(a % Math.Round(a) == 1)
                {
                    progressBar1.Maximum = b;
                }
                else
                {
                    progressBar1.Maximum = b + 1;
                }

                progressBar1.Minimum = 0;                        //Настройка ProgressBar
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.Value = 0;
                progressBar1.Step = 1;
                if(a < (12 * 1024) && a > (2 * 1024))      //Если больше 12 гб или меньше 2гб то не проходит
                {
                    byte[] BY = new byte[1024*1024];
                    BinaryReader reader = new BinaryReader(Workfile, Encoding.GetEncoding(1251));     //Счёт байтов с поддержкой 
                    
                    for (int i = 0; i < b; i++)     //Циклом проходятся по всем гигабайтам
                    {
                        if (LenBytes >= BY.Length)
                        {
                            reader.Read(BY, 0, BY.Length);//Запись в массив гигабайт байтов
                        }
                        else
                        {
                            break;
                        }
                        string[] a1 = HexDump(BY);        //Вывод в указанном в задании виде
                        for (int q = 0; q < a1.Length; q++)
                        {
                            richTextBox1.Text += a1[q];
                        }
                        
                        LenBytes -= BY.Length;           //Считает сколько осталось байтов
                        Array.Clear(BY, 0, BY.Length);
                        progressBar1.PerformStep();  //Продвигается ProgressBar
                    }
                    if(LenBytes > 0) //Оставшийся байты досчитываются
                    {
                        byte[] BY1 = new byte[LenBytes];
                        reader.Read(BY1, 0, (int)LenBytes);
                        string[] R = HexDump(BY);
                        for (int i = 0; i < R.Length; i++)
                        {
                            richTextBox1.Text += R[i];
                        }
                        progressBar1.PerformStep();
                    }
                    reader.Close();
                    Workfile.Close(); //Закрываются потоки файла
                    label1.Visible = true;
                }
                else
                {
                    Workfile.Close();
                    if (a < (2 * 1024))  //Если меньше 2гб то быстрая запись файла
                    {
                        byte[] buffer = File.ReadAllBytes(textBox1.Text);
                        string[] A = HexDump(buffer);
                        for (int i = 0; i < A.Length; i++)
                        {
                            richTextBox1.Text += A[i];
                        }
                    }
                    else
                    {
                        MessageBox.Show("Больше 12 гигов нельзя"); //Искуственное ограничение по размеру файла
                    }
                }
            }
            else
            {
                MessageBox.Show("Такого файла не существует");  //Если пути нет такого
            }
        }

        public static string[] HexDump(byte[] bytes, int bytesPerLine = 16)      
        {
            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 символов для адреса
                + 3;                  // 3 пробела

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 цифры для шестнадцатеричного значения и 1 пробел
                + (bytesPerLine - 1) / 8 // - 1 дополнительный пробел через каждые 8 символов, начиная с 9-го
                + 2;                  // 2 пропуска

            //int lineLength = firstCharColumn
            //    + bytesPerLine           // - символы для отображения значения ascii
            //    + Environment.NewLine.Length; // Возврат каретки и подача строки (обычно должно быть 2)

            char[] line = (new String(' ', /*lineLength*/(firstCharColumn + bytesPerLine + Environment.NewLine.Length) - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytes.Length + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder((expectedLines * (firstCharColumn + bytesPerLine + Environment.NewLine.Length)) / 8);
            int lineCount = 0;
            string[] Y = new string[8];
            for (int i = 0; i < bytes.Length; i += bytesPerLine)
            {
                
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytes.Length)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                if(result.Length < (expectedLines * (firstCharColumn + bytesPerLine + Environment.NewLine.Length)) / 8)
                {
                    result.Append(line);
                }
                else
                {
                    switch (lineCount)
                    {
                        case 0:
                            Y[0] = result.ToString();
                            break;
                        case 1:
                            Y[1] = result.ToString();
                            break;
                        case 2:
                            Y[2] = result.ToString();
                            break;
                        case 3:
                            Y[3] = result.ToString();
                            break;
                        case 4:
                            Y[4] = result.ToString();
                            break;
                        case 5:
                            Y[5] = result.ToString();
                            break;
                        case 6:
                            Y[6] = result.ToString();
                            break;
                        case 7:
                            Y[7] = result.ToString();
                            break;
                    }
                    result.Remove(0, result.Length);
                    lineCount++;

                    result.Append(line);
                }
            }
            return Y;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
