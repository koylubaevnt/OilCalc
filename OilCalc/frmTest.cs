using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OilCalc.Classes;

namespace OilCalc
{
    public partial class frmTest : Form
    {

        //public enum Method { TABLE_24, TABLE_23 , TABLE_54, TABLE_53 }
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

            tbResult.Text = API_MPMS_11_1.RoundToNearest(value, dimension).ToString();

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
                sb.AppendLine(startValue.ToString() + "\t" + API_MPMS_11_1.RoundToNearest(startValue, round).ToString());
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

            cbMethod.Items.Add(API_MPMS_11_1.TableCalc.T24E);
            cbMethod.Items.Add(API_MPMS_11_1.TableCalc.T23E);
            cbMethod.Items.Add(API_MPMS_11_1.TableCalc.T54E);
            cbMethod.Items.Add(API_MPMS_11_1.TableCalc.T53E);
            cbMethod.Items.Add(API_MPMS_11_1.TableCalc.T60E);
            cbMethod.Items.Add(API_MPMS_11_1.TableCalc.T59E);
            cbMethod.SelectedItem = API_MPMS_11_1.TableCalc.T24E;

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
            initData(out length);

            for (int i = 0; i < length; i++)
            {
                sb.Append((i + 1).ToString());
                calculate(density[i], temperature[i], result[i]);
            }
            tbResultCalc.Text = sb.ToString();
        }

        private void calculate(decimal density, decimal temperature, decimal resultVal)
        {
            API_MPMS_11_1.TableCalc selectedMethod = (API_MPMS_11_1.TableCalc)cbMethod.SelectedItem;
            API_MPMS_11_1 api;
            decimal result = -1.0m;

            api = new API_MPMS_11_1(selectedMethod);
            result = api.Calculate(density, temperature);
            if (resultVal == 0.0m)
            {
                sb.AppendLine("Расчет произведен. Результат: " + result);
                sb.AppendLine(api.LogInfo);
            }
            else
            {
                if (result == resultVal)
                {
                    sb.AppendLine(" - Совпало");
                }
                else
                {
                    sb.AppendLine(" - Ошибка " + result.ToString() + " != " + resultVal.ToString());
                }
            }
        }

        private void initData(out int length)
        {
            length = 0;
            if ((API_MPMS_11_1.TableCalc)cbMethod.SelectedItem == API_MPMS_11_1.TableCalc.T24E)
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
            else if ((API_MPMS_11_1.TableCalc)cbMethod.SelectedItem == API_MPMS_11_1.TableCalc.T23E)
            {
                length = 19;
                density = new decimal[length];
                temperature = new decimal[length];
                result = new decimal[length];

                //Поменять на праивльные значения для теста
                density[0] = 0.67432m;
                temperature[0] = -23.33m;
                result[0] = 0.6311m;

                density[1] = 0.24573m;
                temperature[1] = 190.0m;
                result[1] = 0.4889m;

                density[2] = 0.5000m;
                temperature[2] = 190.04m;
                result[2] = 0.5917m;

                density[3] = 0.2224m;
                temperature[3] = 87.3m;
                result[3] = 0.3500m;

                density[4] = 0.3401m;
                temperature[4] = 64.6m;
                result[4] = -1.0m;

                density[5] = 0.72858m;
                temperature[5] = -27.53m;
                result[5] = -1.0m;

                density[6] = 0.45572m;
                temperature[6] = -24.67m;
                result[6] = -1.0m;

                density[7] = 0.25776m;
                temperature[7] = 179.28m;
                result[7] = 0.4774m;

                density[8] = 0.39548m;
                temperature[8] = 59.78m;
                result[8] = 0.3952m;

                density[9] = 0.21056m;
                temperature[9] = 87.46m;
                result[9] = 0.3502m;

                density[10] = 0.45003m;
                temperature[10] = 199.4m;
                result[10] = 0.5655m;

                density[11] = 0.60133m;
                temperature[11] = 177.17m;
                result[11] = 0.6627m;

                density[12] = 0.73592m;
                temperature[12] = -44.13m;
                result[12] = 0.6880m;

                density[13] = 0.21m;
                temperature[13] = 189.4m;
                result[13] = 0.4876m;

                density[14] = 0.20993m;
                temperature[14] = 187.94m;
                result[14] = -1m;

                density[15] = 0.74005m;
                temperature[15] = -28.48m;
                result[15] = -1m;

                density[16] = 0.74m;
                temperature[16] = -50m;
                result[16] = -1.0m;

                density[17] = 0.5m;
                temperature[17] = -50.8000m;
                result[17] = 0.3892m;

                density[18] = 0.5m;
                temperature[18] = -50.9m;
                result[18] = -1.0m;
            }
            else if ((API_MPMS_11_1.TableCalc)cbMethod.SelectedItem == API_MPMS_11_1.TableCalc.T53E)
            {
                length = 15;
                density = new decimal[length];
                temperature = new decimal[length];
                result = new decimal[length];

                density[0] = 532.57m;
                temperature[0] = -44.120m;
                result[0] = 441.2m;

                density[1] = 673.66m;
                temperature[1] = -23.330m;
                result[1] = 638.4m;

                density[2] = 245.49m;
                temperature[2] = 87.770m;
                result[2] = 489.2m;

                density[3] = 499.55m;
                temperature[3] = 87.820m;
                result[3] = 591.8m;

                density[4] = 395.09m;
                temperature[4] = 15.430m;
                result[4] = 396.2m;

                density[5] = 449.59m;
                temperature[5] = 93.020m;
                result[5] = 565.6m;

                density[6] = 600.74m;
                temperature[6] = 80.650m;
                result[6] = 662.5m;

                density[7] = 736.80m;
                temperature[7] = -44.230m;
                result[7] = 687.8m;

                density[8] = 224.56m;
                temperature[8] = 30.680m;
                result[8] = 351.7m;

                density[9] = 339.63m;
                temperature[9] = 18.130m;
                result[9] = -1.0m;

                density[10] = 727.86m;
                temperature[10] = -33.070m;
                result[10] = -1m;

                density[11] = 209.74m;
                temperature[11] = 11.530m;
                result[11] = -1m;

                density[12] = 739.32m;
                temperature[12] = 11.530m;
                result[12] = -1m;

                density[13] = 645.62m;
                temperature[13] = -46.030m;
                result[13] = -1m;

                density[14] = 645.62m;
                temperature[14] = 93.070m;
                result[14] = -1m;
            }
            else if ((API_MPMS_11_1.TableCalc)cbMethod.SelectedItem == API_MPMS_11_1.TableCalc.T54E)
            {
                length = 18;
                density = new decimal[length];
                temperature = new decimal[length];
                result = new decimal[length];

                density[0] = 352.59m;
                temperature[0] = -45.020m;
                result[0] = 1.36646m;

                density[1] = 399.55m;
                temperature[1] = -3.920m;
                result[1] = 1.09812m;

                density[2] = 451.09m;
                temperature[2] = 30.774m;
                result[2] = 0.93027m;

                density[3] = 489.92m;
                temperature[3] = 84.975m;
                result[3] = 0.60750m;

                density[4] = 539.49m;
                temperature[4] = 68.360m;
                result[4] = 0.84917m;

                density[5] = 569.42m;
                temperature[5] = -16.090m;
                result[5] = 1.06129m;

                density[6] = 599.37m;
                temperature[6] = 43.360m;
                result[6] = 0.94734m;

                density[7] = 624.42m;
                temperature[7] = 76.650m;
                result[7] = 0.89266m;

                density[8] = 639.41m;
                temperature[8] = -24.460m;
                result[8] = 1.05656m;

                density[9] = 659.38m;
                temperature[9] = 80.580m;
                result[9] = 0.90521m;

                density[10] = 669.38m;
                temperature[10] = 82.790m;
                result[10] = 0.90653m;

                density[11] = 399.83m;
                temperature[11] = 90.570m;
                result[11] = -1m;

                density[12] = 449.56m;
                temperature[12] = -46.030m;
                result[12] = -1m;

                density[13] = 349.59m;
                temperature[13] = 4.440m;
                result[13] = -1m;

                density[14] = 449.56m;
                temperature[14] = 93.030m;
                result[14] = -1m;

                density[15] = 687.85m;
                temperature[15] = -17.78m;
                result[15] = -1m;

                density[16] = 687.84m;
                temperature[16] = 93.020m;
                result[16] = 0.89986m;

                density[17] = 351.67m;
                temperature[17] = -46.020m;
                result[17] = 1.37337m;
            }
            else if ((API_MPMS_11_1.TableCalc)cbMethod.SelectedItem == API_MPMS_11_1.TableCalc.T60E)
            {
                length = 18;
                density = new decimal[length];
                temperature = new decimal[length];
                result = new decimal[length];

                density[0] = 332.69m;
                temperature[0] = -5.020m;
                result[0] = 1.22648m;

                density[1] = 399.55m;
                temperature[1] = -3.90m;
                result[1] = 1.12232m;

                density[2] = 451.09m;
                temperature[2] = 30.774m;
                result[2] = 0.95341m;

                density[3] = 489.92m;
                temperature[3] = 84.975m;
                result[3] = 0.66831m;

                density[4] = 539.49m;
                temperature[4] = 68.360m;
                result[4] = 0.86567m;

                density[5] = 569.42m;
                temperature[5] = -16.090m;
                result[5] = 1.07038m;
                
                density[6] = 599.37m;
                temperature[6] = 43.360m;
                result[6] = 0.95706m;

                density[7] = 624.42m;
                temperature[7] = 76.650m;
                result[7] = 0.90318m;

                density[8] = 639.41m;
                temperature[8] = -24.460m;
                result[8] = 1.06325m;

                density[9] = 659.38m;
                temperature[9] = 80.580m;
                result[9] = 0.91345m;

                density[10] = 669.38m;
                temperature[10] = 82.790m;
                result[10] = 0.91427m;

                density[11] = 399.83m;
                temperature[11] = 90.570m;
                result[11] = -1m;

                density[12] = 449.56m;
                temperature[12] = -46.030m;
                result[12] = -1m;

                density[13] = 349.59m;
                temperature[13] = 64.440m;
                result[13] = -1m;

                density[14] = 449.56m;
                temperature[14] = 93.030m;
                result[14] = -1m;

                density[15] = 683.65m;
                temperature[15] = -17.78m;
                result[15] = -1m;

                density[16] = 683.64m;
                temperature[16] = 93.020m;
                result[16] = 0.90540m;

                density[17] = 331.67m;
                temperature[17] = -46.020m;
                result[17] = -1m;
            }
            else if ((API_MPMS_11_1.TableCalc)cbMethod.SelectedItem == API_MPMS_11_1.TableCalc.T59E)
            {
                length = 16;
                density = new decimal[length];
                temperature = new decimal[length];
                result = new decimal[length];

                density[0] = 210.00m;
                temperature[0] = -44.50m;
                result[0] = -1m;

                density[1] = 532.57m;
                temperature[1] = -44.120m;
                result[1] = 431.3m;

                density[2] = 673.66m;
                temperature[2] = -23.330m;
                result[2] = 633.7m;

                density[3] = 245.49m;
                temperature[3] = 87.770m;
                result[3] = 481.2m;

                density[4] = 499.55m;
                temperature[4] = 87.820m;
                result[4] = 586.3m;

                density[5] = 395.09m;
                temperature[5] = 15.430m;
                result[5] = 383.6m;

                density[6] = 449.59m;
                temperature[6] = 93.020m;
                result[6] = 559.6m;

                density[7] = 600.74m;
                temperature[7] = 80.650m;
                result[7] = 658.1m;

                density[8] = 736.80m;
                temperature[8] = -44.230m;
                result[8] = 683.6m;

                density[9] = 224.56m;
                temperature[9] = 30.680m;
                result[9] = 331.7m;

                density[10] = 339.63m;
                temperature[10] = 18.130m;
                result[10] = -1m;

                density[11] = 727.86m;
                temperature[11] = -33.070m;
                result[11] = -1m;

                density[12] = 209.74m;
                temperature[12] = 11.530m;
                result[12] = -1m;

                density[13] = 739.32m;
                temperature[13] = 11.530m;
                result[13] = -1m;

                density[14] = 654.62m;
                temperature[14] = -46.030m;
                result[14] = -1m;

                density[15] = 654.62m;
                temperature[15] = 93.070m;
                result[15] = -1m;
            }
            else
                return;
        }
    }
}
