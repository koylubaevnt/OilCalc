using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OilCalc.Classes
{
    class API_MPMS_11_2
    {
        public enum TableCalc { T5A, T5B, T6A, T6B, T6C, T23A, T23B, T24A, T24B, T24C, T53A, T53B, T54A, T54B, T54C };
        decimal tempKelvin = 273.15m;
        
        const decimal SIGMA = 0.01374979547m;

        public string LogInfo { get; private set; }
        private StringBuilder sb = new StringBuilder();

        #region Конструкторы
        /// <summary>
        /// Конструктор класса "Temperature Correction for the Volume of NGL and LPG Tables 23E, 24E, 53E, 54E, 59E and 60E" со всеми параметрами
        /// </summary>
        /// <param name="table">Тип таблицы для расчета (24Е, 23Е, 54Е, 53Е, 60Е, 59Е)</param>
        public API_MPMS_11_2(TableCalc table)
        {
            this.Table = table;
            this.LogInfo = string.Empty;

            this.a[0] = -0.148759m;
            this.a[1] = -0.267408m;
            this.a[2] = 1.080760m;
            this.a[3] = 1.269056m;
            this.a[4] = -4.089591m;
            this.a[5] = -1.871251m;
            this.a[6] = 7.438081m;
            this.a[7] = -3.536296m;
        }

        /// <summary>
        /// Конструктор класса "Temperature Correction for the Volume of NGL and LPG Tables 23E, 24E, 53E, 54E, 59E and 60E" по умолчанию
        /// </summary>
        public API_MPMS_11_2() : this(TableCalc.T5A) { }

        #endregion

        #region Методы: set, get

        //Установка и получение типа таблицы для расчета (24Е, 23Е, 54Е, 53Е, 60Е, 59Е)
        public TableCalc Table { set; get; }

        #endregion

        #region Основные публичные методы и функции

        public decimal Calculate(decimal dens, decimal temp)
        {
            decimal result = -1.0m;

            this.sb.Length = 0;
            this.LogInfo = string.Empty;

        /*    if (Table == TableCalc.T23E)
                result = CalculateT23E(true, dens, temp);
            else if (Table == TableCalc.T24E)
                result = CalculateT24E(true, dens, temp);
            else if (Table == TableCalc.T53E)
                result = CalculateT53ET59E(dens, temp, 15.0m);
            else if (Table == TableCalc.T54E)
                result = CalculateT54ET60E(dens, temp, 15.0m);
            else if (Table == TableCalc.T60E)
                result = CalculateT54ET60E(dens, temp, 20.0m);
            else if (Table == TableCalc.T59E)
                result = CalculateT53ET59E(dens, temp, 20.0m);
            */
            return result;
        }

        public decimal Calculate(TableCalc table, decimal dens, decimal temp)
        {
            this.Table = table;

            return Calculate(dens, temp);
        }

        #endregion


        #region Вспомогательны скрытые методы и функции

        private decimal CalculateT(CommodityGroup cg, decimal thermalFactor, decimal dens, decimal temp, decimal pres, decimal vol)
        {
            decimal result = -1.0m;

            decimal density, temperature, pressure, thermalFactorCalc, deltaTemperature;
            
            if (temp < -58.0m || temp > 302.0m)
                return result;
            if (pres > 1500)
                return result;
            if (pres < 0)
                pressure = 0;
            else
                pressure = pres;

            if (thermalFactor == -9999.9999m)
            {
                //проверяем плотности зная тип жидкости!!!
                
            }
            else
            {
                if (thermalFactor < 0.000230m || thermalFactor > 0.000930m)
                    return result;
            }

            temperature = ConvertTemperatureFromITS90toIPTS68(temp, Measure.fahrengeit);

            if (thermalFactor != -9999.9999m)
            {
                density = dens * (decimal)Math.Exp(0.5d * (double)thermalFactor * (double)SIGMA * (1.0d + 0.4d * (double)thermalFactor * (double)SIGMA));
                thermalFactorCalc = thermalFactor;
            }
            else
            {
                double A, B;
                decimal[] K = new decimal[3];//тут будет данные из Commodityroup!!!
                A = (double) (SIGMA / 2 * ((K[0] / dens + K[1]) * 1 / dens + K[2]));
                B = (double) ((2 * K[0] + K[1] * dens) / (K[0] + (K[1] + K[2] * dens) * dens));
                density = dens * (1.0m + (decimal)((Math.Exp(A * (1 + 0.8 * A)) - 1) / (1 + A * (1 + 1.6 * A) * B)));
                thermalFactorCalc = (K[0] / density + K[1]) * 1 / density + K[2];
            }
            
            deltaTemperature = temperature;

            decimal CTL, Fp, CPL;

            CTL = (decimal) Math.Exp((double) (-1.0m * thermalFactorCalc * deltaTemperature * (1.0m + 0.8m * thermalFactorCalc * (deltaTemperature + SIGMA))));

            Fp = (decimal) Math.Exp((double) (-1.9947m + 0.00013427m * temperature + (793920m + 2326.0m * temperature) / (decimal) (Math.Pow((double)density, 2))));

            CPL = 1.0m / (1.0m - 0.0001m * Fp * pressure);

            result = Round(CTL * CPL, Unit.CTPL, Measure.API);

            return result;
        }

        #endregion
        
        #region Вспомогательны скрытые методы и функции

        enum Unit { Temperature, Pressure, Density, ThermalFactor, API, CTL, ScaledCompressibilityFactor, CPL, CTPL }
        enum Measure { celsi, fahrengeit, psig, bar, kPa, relativeDensity, API, relativeDensity60, kgm3 }

        decimal[] a = new decimal[8];

        //Method to Convert Units of Temperture, Pressure, ThermalExpansion Factor, and Density-Related Values
        private decimal ConvertUnits(decimal value, Unit unit, Measure measure, Measure measureOut)
        {
            switch (unit)
            {
                case Unit.Temperature:
                    if (measure.Equals(Measure.celsi))
                        return 1.8m * value + 32;
                    else
                        return value;
                case Unit.Pressure:
                    if (measure.Equals(Measure.kPa))
                        return value / 6.894757m;
                    else if (measure.Equals(Measure.bar))
                        return value / 0.06894757m;
                    else
                        return value;
                case Unit.Density:
                    if (measure.Equals(Measure.relativeDensity))
                        return value * 999.016m;
                    else if (measure.Equals(Measure.API))
                        return 141.5m / (value + 131.5m) * 999.016m;
                    else if (measure.Equals(Measure.relativeDensity60))
                        return value / 999.016m;
                    else
                        return value;
                case Unit.ThermalFactor:
                    if (measure.Equals(Measure.fahrengeit) && measureOut.Equals(Measure.celsi))
                        return 1.8m * value;
                    else if (measure.Equals(Measure.celsi) && measureOut.Equals(Measure.fahrengeit))
                        return value / 1.8m;
                    else
                        return value;
                case Unit.API:
                    return 141.5m / value / 999.016m - 131.5m;
                default:
                    return value;
            }
        }

        private decimal ConvertTemperatureFromITS90toIPTS68(decimal value, Measure measure)
        {
            decimal temperature, tau, delta;
            if (measure.Equals(Measure.fahrengeit))
                temperature = (value - 32.0m) / 1.8m;
            else
                temperature = value;
            tau = temperature / 630;
            delta = (a[0] + (a[1] + (a[2] + (a[3] + (a[4] + (a[5] + (a[6] + a[7] * tau) * tau) * tau) * tau) * tau);
            temperature = temperature - delta;
            
            if (measure.Equals(Measure.fahrengeit))
                temperature = 1.8m * value + 32.0m;
            
            return temperature;
        }

        private decimal Round(decimal value, Unit unit, Measure measure)
        {
            decimal roundIncrement = 0.0m;
            switch (unit)
            {
                case Unit.Density:
                    switch(measure)
                    {
                        case Measure.API: roundIncrement = 0.1m; break;
                        case Measure.relativeDensity: roundIncrement = 0.0001m; break;
                        case Measure.kgm3: roundIncrement = 0.1m; break;
                        default:
                            return value;
                    }
                    break;
                case Unit.Temperature:
                    switch (measure)
                    {
                        case Measure.fahrengeit: roundIncrement = 0.1m;  break;
                        case Measure.celsi: roundIncrement = 0.05m; break;
                        default:
                            return value;
                    }
                    break;
                case Unit.Pressure:
                    switch (measure)
                    {
                        case Measure.psig: roundIncrement = 1.0m;  break;
                        case Measure.kPa: roundIncrement = 5.0m;  break;
                        case Measure.bar: roundIncrement = 0.05m; break;
                        default:
                            return value;
                    }
                    break;
                case Unit.ThermalFactor:
                    switch (measure)
                    {
                        case Measure.fahrengeit: roundIncrement = 0.0000001m;  break;
                        case Measure.celsi: roundIncrement = 0.0000002m;  break;
                        default:
                            return value;
                    }
                    break;
                case Unit.CTL:
                    roundIncrement = 0.00001m;
                    break;
                case Unit.ScaledCompressibilityFactor:
                    switch (measure)
                    {
                        case Measure.psig: roundIncrement = 0.001m; break;
                        case Measure.kPa: roundIncrement = 0.0001m; break;
                        case Measure.bar: roundIncrement = 0.01m; break;
                        default:
                            return value;
                    }
                    break;
                case Unit.CPL:
                    roundIncrement = 0.00001m;
                    break;
                case Unit.CTPL:
                    roundIncrement = 0.00001m;
                    break;
                default:
                    return value;
            }

            decimal normalizedValue = Math.Abs(value) / roundIncrement;

            //Условие ДОЛЖНО БЫТЬ другим:
            // после запятой это должно быть 5 для любого числа!!! 10.5, 5.5, -3.5 и т.д.
            if (!normalizedValue.Equals(0.5m))
            {
                normalizedValue = Math.Truncate(normalizedValue + 0.5m); 
            }
            else
            {
                normalizedValue = Math.Truncate(normalizedValue) + 1.0m * 0.0m;
                //Написать условие определения четности (по-идее то просто проверять остаток от деления на 2). Но может еесть штатная фукция???
            }

            return normalizedValue * roundIncrement * Math.Sign(value);
        }
        #endregion
    }
}
