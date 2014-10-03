using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OilCalc.ReferenceTables;

namespace OilCalc
{
    public class CTest
    {
        /*
         * Table 24 - 60F Basis
         */

        decimal relativeDensity;
        decimal observedTemperature;
        public decimal TemperatureCorrectionFactor { get; private set; }
        public decimal TemperatureCorrectionFactorNoRounded { get; private set; }        
        public string LogInfo { get; private set; }
        
        //Справочная таблица
        ReferenceFluidParameter refFluidParametersTable = new ReferenceFluidParameter();
        //
        int indexFirstFluid = -1;
        //
        private decimal density, temperature;
            
        //Пока таблица у нас такая. МОжет есть решение лучше???
        

        //Считаю не очень удачным решением реализация лимитов. Нужно пересмотреть.
        public enum LimitType {DENSITY, TEMPERATURE};
        readonly ArrayList limitDensity = new ArrayList();
        readonly ArrayList limitTemperature = new ArrayList();
        
        //Конструктор по-умолчанию
        public CTest() : this(-1.0m, 0.0m) { }
        
        //Конструктор с передачей параметров для расчета
        public CTest(decimal density, decimal temperature)
        {
            this.relativeDensity = density;
            this.observedTemperature = temperature;
            this.TemperatureCorrectionFactor = -1.0m;
            this.TemperatureCorrectionFactorNoRounded = -1.0m;
            this.LogInfo = string.Empty;

            this.limitTemperature.Add(227.15m);
            this.limitTemperature.Add(366.15m);
            this.limitDensity.Add(0.3500m);
            this.limitDensity.Add(0.6880m);
        }

        /// <summary>
        /// Функция для расчета приведения
        /// </summary>
        /// <param name="density">Относительная плотность при 60F</param>
        /// <param name="temperature">Температура наблюдения</param>
        /// <param name="needCheckBoundaries">Проверядь диапазоны значений</param>
        /// <returns>Статус расчета: true - посчитано, false - не посчитано (ошибка)</returns>
        public bool Calculate(decimal density, decimal temperature, bool needCheckBoundaries)
        {
            this.relativeDensity = density;
            this.observedTemperature = temperature;

            //основной расчет
            return Calculate(needCheckBoundaries);
        }

        /// <summary>
        /// Функция для расчета приведения
        /// </summary>
        /// <param name="density">Относительная плотность при 60F</param>
        /// <param name="temperature">Температура наблюдения</param>
        /// <returns>Статус расчета: true - посчитано, false - не посчитано (ошибка)</returns>
        public bool Calculate(decimal density, decimal temperature)
        {
            this.relativeDensity        = density;
            this.observedTemperature    = temperature;

            //основной расчет
            return Calculate(true);
        }

        /// <summary>
        /// Функция для расчета приведения
        /// </summary>
        /// <param name="needCheckBoundaries">Проверядь диапазоны значений</param>
        /// <returns></returns>
        public bool Calculate(bool needCheckBoundaries)
        {
            this.LogInfo = string.Empty;
            StringBuilder sb = new StringBuilder();
            this.TemperatureCorrectionFactor = -1.0m;
            this.TemperatureCorrectionFactorNoRounded = -1.0m;

            if (this.relativeDensity == -1.0m)
            {
                LogInfo = "Не передана плотность для расчета!";
                return false;
            }

            decimal temperatureKelvin;

            sb.AppendLine("Table 24");
            sb.AppendLine("\tInput Data.");
            sb.AppendLine("Relative density @60F: " + this.relativeDensity.ToString());
            sb.AppendLine("Observed Temperature, F: " + this.observedTemperature.ToString());

            if (needCheckBoundaries)
            {
                density = RoundToNearest(this.relativeDensity, 0.0001m);
                temperature = RoundToNearest(this.observedTemperature, 0.1m);
                sb.AppendLine("\tComputed Data - last digit is rounded.");
                sb.AppendLine("  T24/1");
                sb.AppendLine("Relative density @60F:" + density.ToString());
                sb.AppendLine("Observed Temperature, F: " + temperature.ToString());
                //Calculate
                temperatureKelvin = ConvertToKelvin(temperature);
                sb.AppendLine("  T24/2");
                sb.AppendLine("Temperature, Kelvin: " + temperatureKelvin.ToString());

                sb.AppendLine("  T24/3");
                bool isDensityInRange = CheckBoundaries(density, LimitType.DENSITY);
                bool isTemperatureInRange = CheckBoundaries(temperatureKelvin, LimitType.TEMPERATURE);
                if (!isDensityInRange && !isTemperatureInRange)
                {
                    sb.AppendLine("Density and temperature is not in range");
                    LogInfo = sb.ToString();
                    return false;
                }
                else if (!isDensityInRange)
                {
                    sb.AppendLine("Density is not in range");
                    LogInfo = sb.ToString();
                    return false;
                }
                else if (!isTemperatureInRange)
                {
                    sb.AppendLine("Temperature is not in range");
                    LogInfo = sb.ToString();
                    return false;
                }
                else
                    sb.AppendLine("Input data within range");
            }
            else
            {
                density = this.relativeDensity;
                temperatureKelvin = this.observedTemperature;
            }

            sb.AppendLine("  T24/4");
            DetermineFluidReferences();
            //Думаю эта проверка не нужна
            if (indexFirstFluid == -1)
            {
                sb.AppendLine("Didn't find fluids");
                LogInfo = sb.ToString();
                return false;
            }
            else
            {
                sb.AppendLine("Reference Fluid 1: " + ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).Name);
                sb.AppendLine("Reference Fluid 2: " + ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]).Name);
            }

            sb.AppendLine("  T24/5");
            decimal sigma;
            sigma = (density - ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).RelativeDensity) /
                (((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]).RelativeDensity
                    - ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).RelativeDensity);
            sb.AppendLine("Delta: " + sigma.ToString());

            sb.AppendLine("  T24/6");
            decimal criticalTemperature;
            criticalTemperature = ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).CriticalTemperature + sigma *
                (((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]).CriticalTemperature
                    - ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).CriticalTemperature);
            sb.AppendLine("Critical temperature: " + criticalTemperature.ToString());

            sb.AppendLine("  T24/7");
            decimal reducedObservedTemperature;
            reducedObservedTemperature = temperatureKelvin / criticalTemperature;
            sb.AppendLine("Redused observed temperature: " + reducedObservedTemperature.ToString());
            if (reducedObservedTemperature > 1.0m)
            {
                sb.AppendLine("Redused temperature greather than 1.0");
                LogInfo = sb.ToString();
                return false;
            }

            sb.AppendLine("  T24/8");
            decimal reducedObservedTemperatureF;
            reducedObservedTemperatureF = 519.67m / (1.8m * criticalTemperature);
            sb.AppendLine("Redused temperature at 60F: " + reducedObservedTemperatureF.ToString());

            sb.AppendLine("  T24/9");
            decimal scalingFactor;
            scalingFactor =
                (((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).CriticalCompressiblityFactor *
                ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).CriticalDensity) /
                (((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]).CriticalCompressiblityFactor *
                ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]).CriticalDensity);
            sb.AppendLine("Scaling factor: " + scalingFactor.ToString());

            sb.AppendLine("  T24/10");
            decimal tau;
            decimal[] saturationDensity60, saturationDensity;
            tau = 1 - reducedObservedTemperatureF;
            SaturationDensity(tau, out saturationDensity60);
            sb.AppendLine("Tau for fluid at 60F: " + tau.ToString());
            sb.AppendLine("Sat den fluid 1 at 60F: " + saturationDensity60[0].ToString());
            sb.AppendLine("Sat den fluid 2 at 60F: " + saturationDensity60[1].ToString());

            sb.AppendLine("  T24/11");
            decimal interpolatingFactor;
            interpolatingFactor = saturationDensity60[0] /
                (1 + sigma * ((saturationDensity60[0] / (scalingFactor * saturationDensity60[1])) - 1));
            sb.AppendLine("Interpolating factor: " + interpolatingFactor.ToString());

            sb.AppendLine("  T24/12");
            tau = 1 - reducedObservedTemperature;
            SaturationDensity(tau, out saturationDensity);
            sb.AppendLine("Tau for fluid at obs temp: " + tau.ToString());
            sb.AppendLine("Sat den fluid 1 at obs temp: " + saturationDensity[0].ToString());
            sb.AppendLine("Sat den fluid 2 at obs temp: " + saturationDensity[1].ToString());

            sb.AppendLine("  T24/13");
            this.TemperatureCorrectionFactorNoRounded = saturationDensity[0] /
                (interpolatingFactor * (1 + sigma * (saturationDensity[0] / (scalingFactor * saturationDensity[1]) - 1)));
            sb.AppendLine("CTL: " + this.TemperatureCorrectionFactorNoRounded.ToString());

            sb.AppendLine("  T24/14");
            this.TemperatureCorrectionFactor = RoundToNearest(this.TemperatureCorrectionFactorNoRounded, 0.00001m);
            sb.AppendLine("CTL rounded: " + this.TemperatureCorrectionFactor.ToString());
            LogInfo = sb.ToString();
            return true;
        }

        /// <summary>
        /// Функция для расчета приведения без параметров (основная)
        /// </summary>
        /// <returns>Статус расчета: true - посчитано, false - не посчитано (ошибка)</returns>
        public bool Calculate()
        {
            return Calculate(true);
        }

        public static decimal RoundToNearest(decimal value, decimal precision)
        {
            if (precision == 0.0m)
                return 0.0m;

            decimal prec = Math.Abs(precision);
            decimal sign = Math.Sign(value);
            decimal val = Math.Abs(value);

            val = val / prec;
            decimal valFloor = Math.Floor(val);
            val -= valFloor;

            return (((val - 0.5m) >= 0)) ? sign * ((valFloor + 1) * prec) : sign * (valFloor * prec);
        }

        public static decimal ConvertToKelvin(decimal value)
        {
            return (value + 459.67m) / 1.8m;
        }

        public bool CheckBoundaries(decimal value, LimitType limitType)
        {
            
            bool status = true;
            if (limitType == LimitType.DENSITY
                    && (value < (decimal)limitDensity[0] || value > (decimal)limitDensity[1]))
                status = false;
            else if (limitType == LimitType.TEMPERATURE
                    && (value < (decimal)limitTemperature[0] || value > (decimal)limitTemperature[1]))
                status = false;

            return status;
        }

        private void DetermineFluidReferences()
        {
            indexFirstFluid = -1;
            foreach (ReferenceFluidParameter rfpt in refFluidParametersTable)
            {
                if (density <= rfpt.RelativeDensity){
                    indexFirstFluid = refFluidParametersTable.ReferenceFluidParameterTable.IndexOf(rfpt) - 1;
                    break;
			    }
            }
        }

        private void SaturationDensity(decimal tau, out decimal[] sat)
        {
            sat = new decimal[2];
            sat[0] = 
            ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).CriticalDensity *
                (1 +
                    (((((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).SaturationDensityFittingParameter[0] * (decimal)Math.Pow((double)tau, 0.35d) +
                        ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).SaturationDensityFittingParameter[2] * (decimal)Math.Pow((double)tau, 2d) +
                        ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).SaturationDensityFittingParameter[3] * (decimal)Math.Pow((double)tau, 3d)
                    ) / (
                        1 + ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).SaturationDensityFittingParameter[1] * (decimal)Math.Pow((double)tau, 0.65d))))
                );
            sat[1] = ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]).CriticalDensity *
                (1 +
                    (((((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]).SaturationDensityFittingParameter[0] * (decimal)Math.Pow((double)tau, 0.35d) +
                        ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]).SaturationDensityFittingParameter[2] * (decimal)Math.Pow((double)tau, 2d) +
                        ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]).SaturationDensityFittingParameter[3] * (decimal)Math.Pow((double)tau, 3d)
                    ) / (
                        1 + ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]).SaturationDensityFittingParameter[1] * (decimal)Math.Pow((double)tau, 0.65d))))
                );
            ;
        }
    }
}

