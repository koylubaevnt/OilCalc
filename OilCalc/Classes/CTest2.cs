using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OilCalc.ReferenceTables;
using System.Collections;

namespace OilCalc
{
    public class CTest2
    {
        /*
            * Table 23 - 60F Basis
            */

        decimal relativeDensityObserved;
        decimal observedTemperature;
        
        public decimal RelativeDensity { get; private set; }
        public decimal RelativeDensityNoRounded { get; private set; }
        /// <summary>
        /// Не используется здесь!!!! оставил для возможности объединения классов через интерфейс
        /// </summary>
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
        public enum LimitType { DENSITY, TEMPERATURE };
        readonly ArrayList limitDensity = new ArrayList();
        readonly ArrayList limitTemperature = new ArrayList();

        //Конструктор по-умолчанию
        public CTest2() : this(-1.0m, 0.0m) { }

        //Конструктор с передачей параметров для расчета
        public CTest2(decimal density, decimal temperature)
        {
            this.relativeDensityObserved = density;
            this.observedTemperature = temperature;
            this.RelativeDensity = -1.0m;
            this.RelativeDensityNoRounded = -1.0m;
            this.TemperatureCorrectionFactor = -1.0m;
            this.TemperatureCorrectionFactorNoRounded = -1.0m;
            this.LogInfo = string.Empty;

            this.limitTemperature.Add(227.15m);
            this.limitTemperature.Add(366.15m);
            this.limitDensity.Add(0.2100m);
            this.limitDensity.Add(0.7400m);
        }

        /// <summary>
        /// Функция для расчета приведения
        /// </summary>
        /// <param name="density">Относительная плотность при 60F</param>
        /// <param name="temperature">Температура наблюдения</param>
        /// <returns>Статус расчета: true - посчитано, false - не посчитано (ошибка)</returns>
        public bool Calculate(decimal density, decimal temperature)
        {
            this.relativeDensityObserved = density;
            this.observedTemperature = temperature;

            //основной расчет
            return Calculate();
        }

        /// <summary>
        /// Функция для расчета приведения без параметров (основная)
        /// </summary>
        /// <returns>Статус расчета: true - посчитано, false - не посчитано (ошибка)</returns>
        public bool Calculate()
        {
            this.LogInfo = string.Empty;
            StringBuilder sb = new StringBuilder();
            this.RelativeDensity = -1.0m;
            this.RelativeDensityNoRounded = -1.0m;
            this.TemperatureCorrectionFactor = -1.0m;
            this.TemperatureCorrectionFactorNoRounded = -1.0m;
            CTest cTest = new CTest();

            if (this.relativeDensityObserved == -1.0m)
            {
                LogInfo = "Не передана плотность для расчета!";
                return false;
            }

            decimal temperatureKelvin;

            sb.AppendLine("Table 23");
            sb.AppendLine("\tInput Data.");
            sb.AppendLine("Relative density obs.: " + this.relativeDensityObserved.ToString());
            sb.AppendLine("Observed Temperature, F: " + this.observedTemperature.ToString());
            //Calculate
            density = RoundToNearest(this.relativeDensityObserved, 0.0001m);
            temperature = RoundToNearest(this.observedTemperature, 0.1m);
            sb.AppendLine("\tComputed Data - last digit is rounded.");
            sb.AppendLine("  T23/1");
            sb.AppendLine("Relative density obs.:" + density.ToString());
            sb.AppendLine("Observed Temperature, F: " + temperature.ToString());
            //Calculate
            temperatureKelvin = ConvertToKelvin(temperature);
            sb.AppendLine("  T23/2");
            sb.AppendLine("Temperature, Kelvin: " + temperatureKelvin.ToString());

            sb.AppendLine("  T23/3");
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

            sb.AppendLine("  T23/4");
            decimal 
                reducedObservedTemperature = 0, 
                reducedObservedTemperature60 = 0, 
                sat = 0, 
                sat60 = 0,
                relativeObservedDensityRef = 0, 
                relativeObservedDensityRef1 = 0, relativeObservedDensityRef2 = 0;

            int i;
            ReferenceFluidParameter rfp;
            List<ReferenceFluidParameter> rfpResult = new List<ReferenceFluidParameter>();

            foreach(ReferenceFluidParameter rfpt in refFluidParametersTable)
            {
                reducedObservedTemperature = temperatureKelvin / rfpt.CriticalTemperature;
                if (reducedObservedTemperature <= 1.0m)
                {
                    reducedObservedTemperature60 = 519.67m / (1.8m * rfpt.CriticalTemperature);
                    SaturationDensity(reducedObservedTemperature, out sat);
                    SaturationDensity(reducedObservedTemperature60, out sat60);
                    relativeObservedDensityRef = rfpt.RelativeDensity * (sat / sat60);
                    i = refFluidParametersTable.ReferenceFluidParameterTable.IndexOf(rfpt);
                        
                    if ((density <= relativeObservedDensityRef && relativeObservedDensityRef > 0.0m) || i == 11)
                    {
                        if (i == 0)
                        {
                            rfp = (ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[i + 1];
                            relativeObservedDensityRef1 = relativeObservedDensityRef;
                            reducedObservedTemperature = temperatureKelvin / rfp.CriticalTemperature;
                            reducedObservedTemperature60 = 519.67m / (1.8m * rfp.CriticalTemperature);
                            SaturationDensity(reducedObservedTemperature, out sat);
                            SaturationDensity(reducedObservedTemperature60, out sat60);
                            relativeObservedDensityRef2 = rfp.RelativeDensity * (sat / sat60);
                            indexFirstFluid = 0;
                        }
                        else 
                        {
                            relativeObservedDensityRef2 = relativeObservedDensityRef;
                            indexFirstFluid = i - 1;
                        }
                        break;
                    }
                    relativeObservedDensityRef1 = relativeObservedDensityRef;
                }
            }
            sb.AppendLine("RDtf for Fluid 1: " + relativeObservedDensityRef1.ToString());
            sb.AppendLine("RDtf for Fluid 2: " + relativeObservedDensityRef2.ToString());

            sb.AppendLine("  T23/5");
            rfpResult.Add((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]);
            rfpResult.Add((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1]);
            sb.AppendLine("Reference Fluid 1: " + rfpResult[0].Name);
            sb.AppendLine("Reference Fluid 2: " + rfpResult[1].Name);

            if (density >  relativeObservedDensityRef2) 
            {
                sb.AppendLine("RDtf observed is greather than upper boundary RDtf, no solution");
                return false;
            }
            
            decimal Trx1, Trx2, Tr601, Tr602;
            Trx1 = temperatureKelvin / rfpResult[0].CriticalTemperature;
            Trx2 = temperatureKelvin / rfpResult[1].CriticalTemperature;
            Tr601 = 519.67m / (1.8m * rfpResult[0].CriticalTemperature);
            Tr602 = 519.67m / (1.8m * rfpResult[1].CriticalTemperature);
            sb.AppendLine("Tr,x for Fluid 1: " + Trx1.ToString());
            sb.AppendLine("Tr,x for Fluid 2: " + Trx2.ToString());
            sb.AppendLine("Tr,60 for Fluid 1: " + Tr601.ToString());
            sb.AppendLine("Tr,60 for Fluid 2: " + Tr602.ToString());
            
            sb.AppendLine("  T23/6");
            decimal relativeObservedDensity60Hight, relativeObservedDensityHight,
                relativeObservedDensity60Low, relativeObservedDensityLow,;
            relativeObservedDensity60Hight = rfpResult[1].RelativeDensity;
            relativeObservedDensityHight = relativeObservedDensityRef2;
            relativeObservedDensity60Low = rfpResult[0].RelativeDensity;
            relativeObservedDensityLow = relativeObservedDensityRef1;

            if (Trx1 > 1)
            {
                relativeObservedDensity60Low = ( (temperatureKelvin - rfpResult[0].CriticalTemperature) / (rfpResult[1].CriticalTemperature - rfpResult[0].CriticalTemperature) ) * (rfpResult[1].RelativeDensity - rfpResult[0].RelativeDensity ) + rfpResult[0].RelativeDensity;

            }
            if (relativeObservedDensity60Low < 0.3500m)
            {
                relativeObservedDensity60Low = 0.3500m;
            }
            cTest.Calculate(relativeObservedDensity60Low, temperatureKelvin, false);
            if (cTest.TemperatureCorrectionFactorNoRounded == -1.0m)
                relativeObservedDensityLow = -1;
            else
                relativeObservedDensityLow = cTest.TemperatureCorrectionFactorNoRounded * relativeObservedDensity60Low;
            sb.AppendLine("Upper boundary 60: " + relativeObservedDensity60Hight);
            sb.AppendLine("Upper boundary: " + relativeObservedDensityHight);
            sb.AppendLine("Lower boundary 60: " + relativeObservedDensity60Low);
            sb.AppendLine("Lower boundary: " + relativeObservedDensityLow);

            sb.AppendLine("  T23/7");
            decimal sigma, relativeObservedDensity60Mid, relativeObservedDensityMid,
                alpha, beta, phi, A, B, C, relativeObservedDensity60Trial, relativeObservedDensityTrial;
            i = 1;
            while (true)
            {
                sb.AppendLine("     Pass " + i);
                if (relativeObservedDensityLow != -1.0m)
                {
                    sigma = (density - relativeObservedDensityLow) / (relativeObservedDensityHight - relativeObservedDensityLow);
                    if (sigma < 0.001m)
                        sigma = 0.001m;
                    else if (sigma > 0.999m)
                        sigma = 0.999m;
                    relativeObservedDensity60Mid = relativeObservedDensity60Low + sigma * (relativeObservedDensity60Hight - relativeObservedDensity60Low);
                    sb.AppendLine("Delta: " + sigma.ToString());
                }
                else//понять когда сюда заходим вообще!!!!
                    relativeObservedDensity60Mid = (relativeObservedDensity60Hight - relativeObservedDensity60Low) / 2;
                
                sb.AppendLine("RD60,mid: " + relativeObservedDensity60Mid.ToString());
                cTest.Calculate(relativeObservedDensity60Mid, temperatureKelvin, false);
                sb.AppendLine("CTL: " + cTest.TemperatureCorrectionFactorNoRounded.ToString());
                relativeObservedDensityMid = cTest.TemperatureCorrectionFactorNoRounded * relativeObservedDensity60Mid;
                sb.AppendLine("RDTf,mid " + relativeObservedDensityMid.ToString());
                
                sb.AppendLine("  T23/8");
                if ((density > relativeObservedDensityLow && density < relativeObservedDensityMid && Math.Abs(relativeObservedDensity60Low - relativeObservedDensity60Mid) < 0.00000001m) || (density < relativeObservedDensityHight && density > relativeObservedDensityMid && Math.Abs(relativeObservedDensity60Hight - relativeObservedDensity60Mid) < 0.00000001m))
                {
                    sb.AppendLine("Exit");
                    this.RelativeDensity = relativeObservedDensity60Mid;
                    break;
                }
                sb.AppendLine("Continue");
                sb.AppendLine("  T23/9");
                alpha = (relativeObservedDensity60Hight - relativeObservedDensity60Low);
                sb.AppendLine("Alpha: " + alpha.ToString());
                beta = (decimal) Math.Pow((double)relativeObservedDensityHight, 2d) - (decimal) Math.Pow((double)relativeObservedDensityLow, 2d);
                sb.AppendLine("Beta: " + beta.ToString());
                phi =  (relativeObservedDensityHight - relativeObservedDensityLow) / (relativeObservedDensityMid - relativeObservedDensityLow);
                sb.AppendLine("Phi: " + phi.ToString());
                A = (alpha - phi * (relativeObservedDensity60Mid - relativeObservedDensity60Low)) / (beta - phi * ((decimal)Math.Pow((double)relativeObservedDensityMid, 2d) - (decimal)Math.Pow((double)relativeObservedDensityLow, 2d)));
                sb.AppendLine("A: " + A.ToString());
                B = (alpha - A * beta) / (relativeObservedDensityHight - relativeObservedDensityLow);
                sb.AppendLine("B: " + B.ToString());
                C = relativeObservedDensity60Low - B * relativeObservedDensityLow - A * (decimal)Math.Pow((double)relativeObservedDensityLow, 2d);
                sb.AppendLine("C: " + C.ToString());
                relativeObservedDensity60Trial = A * (decimal)Math.Pow((double)density, 2d) + B * density + C;
                if (relativeObservedDensity60Trial < relativeObservedDensity60Low)
                    relativeObservedDensity60Trial = relativeObservedDensity60Low + ((relativeObservedDensity60Mid - relativeObservedDensity60Low) * (density - relativeObservedDensityLow)) / (relativeObservedDensityMid - relativeObservedDensityLow);
                else if (relativeObservedDensity60Trial > relativeObservedDensity60Hight)
                    relativeObservedDensity60Trial = relativeObservedDensity60Mid + ((relativeObservedDensity60Hight - relativeObservedDensity60Mid) * (density - relativeObservedDensityMid)) / (relativeObservedDensityHight - relativeObservedDensityMid);
                
                sb.AppendLine("RD60,trial: " + relativeObservedDensity60Trial.ToString());
                
                cTest.Calculate(relativeObservedDensity60Trial, temperatureKelvin, false);
                relativeObservedDensityTrial = cTest.TemperatureCorrectionFactorNoRounded * relativeObservedDensity60Trial;
                sb.AppendLine("CTL, Trial: " + cTest.TemperatureCorrectionFactorNoRounded.ToString());
                sb.AppendLine("RDTf, Trial: " + relativeObservedDensityTrial.ToString());

                sb.Append("  T23/10");
                if (Math.Abs(density - relativeObservedDensityTrial) < 0.00000001m)
                {
                    this.RelativeDensity = relativeObservedDensity60Trial;
                    sb.AppendLine("Converged");
                    sb.Append("  T23/11");
                    sb.AppendLine(" not need, convergence already achieved");
                    break;
                }
                else
                    sb.AppendLine("");

                sb.AppendLine("  T23/11");
                if (relativeObservedDensityTrial > density)
                {
                relativeObservedDensityHight = relativeObservedDensityTrial;
                relativeObservedDensity60Hight = relativeObservedDensity60Trial;
                if (relativeObservedDensityMid < density)
                {
                    relativeObservedDensityLow = relativeObservedDensityMid;
                    relativeObservedDensity60Low = relativeObservedDensity60Mid;
                }
                }
                else if (relativeObservedDensityTrial < density)
                {
                relativeObservedDensityLow = relativeObservedDensityTrial;
                relativeObservedDensity60Low = relativeObservedDensity60Trial;
                if (relativeObservedDensityMid > density)
                {
                    relativeObservedDensityHight = relativeObservedDensityMid;
                    relativeObservedDensity60Hight = relativeObservedDensity60Mid;
                }
                }

                if (i >= 10)
                {
                    this.RelativeDensity = -1.0m;
                    sb.AppendLine("No result");
                    LogInfo = sb.ToString();
                    return false;
                }
                i++;   
            }
            
            sb.AppendLine("  T23/12");
            this.RelativeDensityNoRounded = this.RelativeDensity;
            this.RelativeDensity = RoundToNearest(this.RelativeDensity, 0.0001m);
            if (this.RelativeDensity < 0.3500m || this.RelativeDensity > 0.6880m)
            {
                this.RelativeDensityNoRounded = -1.0m;
                this.RelativeDensity = -1.0m;
                sb.AppendLine("Out off boundaries");
                return false;
            }
            sb.AppendLine("RD60(RD60, Trial): " + this.RelativeDensityNoRounded);
            sb.AppendLine("RD60(RD60, Trial rounded): " + this.RelativeDensity);

            LogInfo = sb.ToString();
            return true;
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
                if (density <= rfpt.RelativeDensity)
                {
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

        private void SaturationDensity(decimal tau, out decimal sat)
        {
            sat  =
            ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).CriticalDensity *
                (1 +
                    (((((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).SaturationDensityFittingParameter[0] * (decimal)Math.Pow((double)tau, 0.35d) +
                        ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).SaturationDensityFittingParameter[2] * (decimal)Math.Pow((double)tau, 2d) +
                        ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).SaturationDensityFittingParameter[3] * (decimal)Math.Pow((double)tau, 3d)
                    ) / (
                        1 + ((ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid]).SaturationDensityFittingParameter[1] * (decimal)Math.Pow((double)tau, 0.65d))))
                );
        }
    }
}