using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OilCalc
{
    public partial class frmTest : Form
    {

        public enum Method { TABLE_24, TABLE_23 , TABLE_54, TABLE_53 }
        StringBuilder sb = new StringBuilder();
     
        decimal[] density;
        decimal[] temperature;
        decimal[] result;

        public frmTest()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            tbResult.Text = string.Empty;

            if(string.IsNullOrEmpty(tbValue.Text)
                || string.IsNullOrEmpty(tbDimension.Text))
            {
                MessageBox.Show("Не заполнены поля Значение или Округление!","Ошибка");
                return;
            }

            decimal value, dimension;
            try
            {
                value = decimal.Parse(tbValue.Text.Replace('.',','));
                dimension = decimal.Parse(tbDimension.Text.Replace('.', ','));
            }
            catch (Exception ex)
            {
                return;
            }

            tbResult.Text = CTest.RoundToNearest(value, dimension).ToString();

        }

        private void btCycle_Click(object sender, EventArgs e)
        {
            tbInfo.Text = string.Empty;
            if (string.IsNullOrEmpty(tbStartValue.Text)
                || string.IsNullOrEmpty(tbEndValue.Text)
                || string.IsNullOrEmpty(tbStep.Text)
                || string.IsNullOrEmpty(tbRound.Text))
            {
                MessageBox.Show("Не заполнены поля Значение или Округление!", "Ошибка");
                return;
            }

            decimal startValue, endValue, step, round;
            try
            {
                startValue = decimal.Parse(tbStartValue.Text.Replace('.', ','));
                endValue = decimal.Parse(tbEndValue.Text.Replace('.', ','));
                step = decimal.Parse(tbStep.Text.Replace('.', ','));
                round = decimal.Parse(tbRound.Text.Replace('.', ','));
            }
            catch (Exception ex)
            {
                return;
            }

            sb.Length = 0;

            while (startValue <= endValue) 
            {
                sb.AppendLine(startValue.ToString() + "\t" + CTest.RoundToNearest(startValue, round).ToString());
                startValue += step;
            }

            tbInfo.Text = sb.ToString();
        }

        private void frmTest_Load(object sender, EventArgs e)
        {
            tbValue.Text = "-0,05";
            tbDimension.Text = "0,1";


            tbStartValue.Text = "0,127";
            tbEndValue.Text = "0,128";
            tbStep.Text = "0,00001";
            tbRound.Text = "0,001";

            cbMethod.Items.Add(Method.TABLE_24);
            cbMethod.Items.Add(Method.TABLE_23);
            cbMethod.Items.Add(Method.TABLE_54);
            cbMethod.Items.Add(Method.TABLE_53);
            cbMethod.SelectedItem = Method.TABLE_24;

            tbDensity.Text = "0.451530";
            tbTemperature.Text = "87.4200";
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            tbResultCalc.Clear();
            sb.Length = 0;
            decimal density, temperature;
            try
            {
                density = decimal.Parse(tbDensity.Text.Replace('.', ','));
                temperature = decimal.Parse(tbTemperature.Text.Replace('.', ','));
            }
            catch (Exception ex)
            {
                return;
            }
            
            calculate(density, temperature, 0m);
            tbResultCalc.Text = sb.ToString();
        }

        private void btnCalcCheck_Click(object sender, EventArgs e)
        {
            tbResultCalc.Clear();
            sb.Length = 0;
            int length = 0;
            if ((Method)cbMethod.SelectedItem == Method.TABLE_24)
            {
                length = 18;
                density = new decimal[length];
                temperature = new decimal[length];
                result = new decimal[length];

                density[0] = 0.350130m;
                temperature[0] = -48.0200m;
                result[0] = 1.37417m;

                density[1] = 0.399950m;
                temperature[1] = 24.9500m;
                result[1] = 1.10076m;
            
                density[2] = 0.451530m;
                temperature[2] = 87.4200m;
                result[2] = 0.93275m;

                density[3] = 0.490400m;
                temperature[3] = 184.9700m;
                result[3] = 0.61595m;

                density[4] = 0.540020m;
                temperature[4] = 155.0400m;
                result[4] = 0.85107m;

                density[5] = 0.569980m;
                temperature[5] = 3.0330m;
                result[5] = 1.06231m;

                density[6] = 0.599970m;
                temperature[6] = 110.0400m;
                result[6] = 0.94847m;

                density[7] = 0.625020m;
                temperature[7] = 169.9700m;
                result[7] = 0.89382m;

                density[8] = 0.640040m;
                temperature[8] = -12.0200m;
                result[8] = 1.05730m;

                density[9] = 0.660033m;
                temperature[9] = 177.0450m;
                result[9] = 0.90619m;

                density[10] = 0.670042m;
                temperature[10] = 181.0300m;
                result[10] = 0.90740m;

                density[11] = 0.350180m;
                temperature[11] = 195.0250m;
                result[11] = -1m;

                density[12] = 0.500000m;
                temperature[12] = -50.8500m;
                result[12] = -1m;

                density[13] = 0.349940m;
                temperature[13] = 40m;
                result[13] = -1m;

                density[14] = 0.450000m;
                temperature[14] = 199.46m;
                result[14] = -1m;

                density[15] = 0.688070m;
                temperature[15] = 0.0000m;
                result[15] = -1m;

                density[16] = 0.688000m;
                temperature[16] = 199.4400m;
                result[16] = 0.90047m;

                density[17] = 0.350000m;
                temperature[17] = -50.8000m;
                result[17] = 1.38138m;
            }
            else if ((Method)cbMethod.SelectedItem == Method.TABLE_23)
            {
                length = 19;
                //Поменять на праивльные значения для теста
                density[0] = 0.350130m;
                temperature[0] = -48.0200m;
                result[0] = 1.37417m;

                density[1] = 0.399950m;
                temperature[1] = 24.9500m;
                result[1] = 1.10076m;
            
                density[2] = 0.451530m;
                temperature[2] = 87.4200m;
                result[2] = 0.93275m;

                density[3] = 0.490400m;
                temperature[3] = 184.9700m;
                result[3] = 0.61595m;

                density[4] = 0.540020m;
                temperature[4] = 155.0400m;
                result[4] = 0.85107m;

                density[5] = 0.569980m;
                temperature[5] = 3.0330m;
                result[5] = 1.06231m;

                density[6] = 0.599970m;
                temperature[6] = 110.0400m;
                result[6] = 0.94847m;

                density[7] = 0.625020m;
                temperature[7] = 169.9700m;
                result[7] = 0.89382m;

                density[8] = 0.640040m;
                temperature[8] = -12.0200m;
                result[8] = 1.05730m;

                density[9] = 0.660033m;
                temperature[9] = 177.0450m;
                result[9] = 0.90619m;

                density[10] = 0.670042m;
                temperature[10] = 181.0300m;
                result[10] = 0.90740m;

                density[11] = 0.350180m;
                temperature[11] = 195.0250m;
                result[11] = -1m;

                density[12] = 0.500000m;
                temperature[12] = -50.8500m;
                result[12] = -1m;

                density[13] = 0.349940m;
                temperature[13] = 40m;
                result[13] = -1m;

                density[14] = 0.450000m;
                temperature[14] = 199.46m;
                result[14] = -1m;

                density[15] = 0.688070m;
                temperature[15] = 0.0000m;
                result[15] = -1m;

                density[16] = 0.688000m;
                temperature[16] = 199.4400m;
                result[16] = 0.90047m;

                density[17] = 0.350000m;
                temperature[17] = -50.8000m;
                result[17] = 1.38138m;
            }
            else
                return;

            for (int i = 0; i < length; i++)
            {
                sb.Append((i + 1).ToString());
                calculate(density[i], temperature[i], result[i]);
            }
            tbResultCalc.Text = sb.ToString();
        }

        private void calculate(decimal density, decimal temperature, decimal resultVal)
        {
            Method selectedMethod = (Method)cbMethod.SelectedItem;
            bool result = false;
            switch (selectedMethod)
            {
                case Method.TABLE_24:
                    CTest cTest = new CTest(density, temperature);
                    result = cTest.Calculate();
                    if (resultVal == 0.0m)
                    {
                        if (result)
                        {
                            sb.AppendLine("Расчет произведен. Результат: " + cTest.TemperatureCorrectionFactor);
                        }
                        else
                        {
                            sb.AppendLine("Расчет не произведен.");
                        }
                        sb.AppendLine(cTest.LogInfo);
                    }
                    else
                    {
                        if (cTest.TemperatureCorrectionFactor == resultVal)
                        {
                            sb.AppendLine(" - Совпало");
                        }
                        else
                        {
                            sb.AppendLine(" - Ошибка " + cTest.TemperatureCorrectionFactor.ToString() + " != " + resultVal.ToString());
                        }
                    }
                    break;

                case Method.TABLE_23:
                    CTest2 cTest2 = new CTest2(density, temperature);
                    result = cTest2.Calculate();
                    if (resultVal == 0.0m)
                    {
                        if (result)
                        {
                            sb.AppendLine("Расчет произведен. Результат: " + cTest2.RelativeDensity);
                        }
                        else
                        {
                            sb.AppendLine("Расчет не произведен.");
                        }
                        sb.AppendLine(cTest2.LogInfo);
                    }
                    else
                    {
                        if (cTest2.RelativeDensity == resultVal)
                        {
                            sb.AppendLine(" - Совпало");
                        }
                        else
                        {
                            sb.AppendLine(" - Ошибка " + cTest2.RelativeDensity.ToString() + " != " + resultVal.ToString());
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void initData()
        {
            int length = 18;
            density = new decimal[length];
            temperature = new decimal[length];
            result = new decimal[length];

            density[0] = 0.350130m;
            temperature[0] = -48.0200m;
            result[0] = 1.37417m;

            density[1] = 0.399950m;
            temperature[1] = 24.9500m;
            result[1] = 1.10076m;
            
            density[2] = 0.451530m;
            temperature[2] = 87.4200m;
            result[2] = 0.93275m;

            density[3] = 0.490400m;
            temperature[3] = 184.9700m;
            result[3] = 0.61595m;

            density[4] = 0.540020m;
            temperature[4] = 155.0400m;
            result[4] = 0.85107m;

            density[5] = 0.569980m;
            temperature[5] = 3.0330m;
            result[5] = 1.06231m;

            density[6] = 0.599970m;
            temperature[6] = 110.0400m;
            result[6] = 0.94847m;

            density[7] = 0.625020m;
            temperature[7] = 169.9700m;
            result[7] = 0.89382m;

            density[8] = 0.640040m;
            temperature[8] = -12.0200m;
            result[8] = 1.05730m;

            density[9] = 0.660033m;
            temperature[9] = 177.0450m;
            result[9] = 0.90619m;

            density[10] = 0.670042m;
            temperature[10] = 181.0300m;
            result[10] = 0.90740m;

            density[11] = 0.350180m;
            temperature[11] = 195.0250m;
            result[11] = -1m;

            density[12] = 0.500000m;
            temperature[12] = -50.8500m;
            result[12] = -1m;

            density[13] = 0.349940m;
            temperature[13] = 40m;
            result[13] = -1m;

            density[14] = 0.450000m;
            temperature[14] = 199.46m;
            result[14] = -1m;

            density[15] = 0.688070m;
            temperature[15] = 0.0000m;
            result[15] = -1m;

            density[16] = 0.688000m;
            temperature[16] = 199.4400m;
            result[16] = 0.90047m;

            density[17] = 0.350000m;
            temperature[17] = -50.8000m;
            result[17] = 1.38138m;
        }
    }
}
