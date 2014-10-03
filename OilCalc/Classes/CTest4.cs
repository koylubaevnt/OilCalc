using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OilCalc.ReferenceTables;
using System.Collections;

namespace OilCalc
{
    public class CTest4
    {
        /*
            * Table 54
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
        public CTest4() : this(-1.0m, 0.0m) { }

        //Конструктор с передачей параметров для расчета
        public CTest4(decimal density, decimal temperature)
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
                    indexFirstFluid = refFluidParametersTable.ReferenceFluidParameterTable.IndexOf(rfpt); 
                    reducedObservedTemperature60 = 519.67m / (1.8m * rfpt.CriticalTemperature);
                    SaturationDensity(1.0m - reducedObservedTemperature, out sat);
                    SaturationDensity(1.0m - reducedObservedTemperature60, out sat60);
                    relativeObservedDensityRef = rfpt.RelativeDensity * (sat / sat60);

                    if ((density <= relativeObservedDensityRef && relativeObservedDensityRef > 0.0m) || indexFirstFluid == 11)
                    {
                        if (indexFirstFluid == 0)
                        {
                            rfp = (ReferenceFluidParameter)refFluidParametersTable.ReferenceFluidParameterTable[indexFirstFluid + 1];
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
                            indexFirstFluid = indexFirstFluid - 1;
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
                LogInfo = sb.ToString();
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
                relativeObservedDensity60Low, relativeObservedDensityLow;
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
            if (density < relativeObservedDensityLow)
            {
                sb.AppendLine("RDtf observed is less than lower boundary RDtf, no solution");
                LogInfo = sb.ToString();
                return false;
            }

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
                LogInfo = sb.ToString();
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
 /*
    * Таблица T54E*
    * Вход:
    * p_denc15 - плотность при наблюдаемой температуре (кг\м3)
    * p_temp - наблюдаемая температура (Градусы)
    * Выход:
    * CTL - фактор температурной корректировки
    
    FUNCTION FNC_TABLE_54E(p_denc15 IN NUMBER, p_temp IN NUMBER, p_type NUMBER DEFAULT 0) RETURN NUMBER IS
        CTL NUMBER;
        Y_15 NUMBER;
        TF NUMBER;
        T_x NUMBER;
        Y_TB NUMBER;
        Y_60 NUMBER;
        CTL1 NUMBER;
        CTL2 NUMBER;
    BEGIN
        CTL := -1;
        --LOG(p_type,'Input Data to Implementation Procedure T54');
        --LOG(p_type,'Density (kg/m3) @ 15 C (Denc15)........ ' || to_char(p_denc15,'fm9999990d999999999999'));
        --LOG(p_type,'Observed temperature Tf, C .......... ' || to_char(p_temp,'fm9999990d999999999999'));
        --54/1
        --Округлить относительную плотность к ближайшему значению 0,1 и наблюдаемую температуру - 0,05 C
        Y_15 := ROUND(p_denc15, 1);
        TF := ROUND(p_temp /0.05, 0) * 0.05;--тут неверно - надо округлить до 0.05!!!
        --LOG(p_type,'  Computed Data - last digit is rounded');
        --LOG(p_type,'  T54/1');
        --LOG(p_type,'  InputData - rounded');
        --LOG(p_type,'Denc15 rounded to 0.1 ........ ' || to_char(Y_15,'fm9999990d999999999999'));
        --LOG(p_type,'Tf, C, rounded to 0.05 .................................... ' || to_char(TF,'fm9999990d999999999999'));

        --54/2
        --Перевести наблюдаемую температуру к Кельвинам
        T_x := TF + 273.15;
        --LOG(p_type,'  T54/2');
        --LOG(p_type,'Tx, Kelvin ................................ ' || to_char(ROUND(T_x,12),'fm9999990d999999999999'));

        --54/3
        --проверить интервалы, если не удовлетворяет значениям, тогда вернем -1
        --LOG(p_type,'  T54/3');
        IF T_x < 227.15 OR T_x > 366.15 OR T_x IS NULL OR Y_15 < 351.7 OR Y_15 > 687.8 OR Y_15 IS NULL THEN
            --LOG(p_type,'Tx or Denc15 is not in segment: Tx=' || to_char(ROUND(T_x,12),'fm9999990d999999999999') || ' Denc15=' || to_char(Y_15,'fm9999990d999999999999'));
            RETURN CTL;
        --ELSE
            --LOG(p_type,'Input data within range, continue');
        END IF;

        --54/4
        --LOG(p_type,'  T54/4');
        Y_TB := Y_15 / 999.016;
        --LOG(p_type,'Denc15 relative to 60F water ................................ ' || to_char(ROUND(Y_TB,12),'fm9999990d999999999999'));

        --54/5
        --LOG(p_type,'  T54/5 Call Table 23 procedure to obtain relative density at 60F');
        Y_60 := FNC_TABLE_23E(Y_TB, 288.15, 1);
        --LOG(p_type,'RD60 from Table 23  ................................ ' || to_char(ROUND(Y_60,12),'fm9999990d999999999999'));

        --54/6
        --LOG(p_type,'  T54/6');
        IF ROUND(Y_60,4) < 0.34995 OR ROUND(Y_60,4) > 0.68805 THEN
            --LOG(p_type,'RD60 ');
            RETURN CTL;
        --ELSE
            --LOG(p_type,'RD60 is within range, continue');
        END IF;

        --54/7
        --LOG(p_type,'  T54/7 Call Table 24 Procedure with Tx and RD60');
        CTL1 := FNC_TABLE_24E(Y_60, T_x, 1);
        --LOG(p_type,'Reference Fluid 1 ....................... ' || fluids(1).fluid_name);
        --LOG(p_type,'Reference Fluid 2 ....................... ' || fluids(2).fluid_name);
        --LOG(p_type,'CTL1, Tx to 60F  ................................ ' || to_char(ROUND(CTL1,12),'fm9999990d999999999999'));
        IF CTL1 <= 0 THEN
            --LOG(p_type,'Value from Table 24 not valid, no solution');
            RETURN -1;
        END IF;
        --LOG(p_type,'  T54/8');
        CTL2 := FNC_TABLE_24E(Y_60, 288.15, 1);
        --LOG(p_type,'Reference Fluid 1 ....................... ' || fluids(1).fluid_name);
        --LOG(p_type,'Reference Fluid 2 ....................... ' || fluids(2).fluid_name);
        --LOG(p_type,'CTL2, 15C to 60F  ................................ ' || to_char(ROUND(CTL2,12),'fm9999990d999999999999'));
        IF CTL2 <= 0 THEN
            --LOG(p_type,'Value from Table 24 not valid, no solution');
            RETURN -1;
        END IF;

        --LOG(p_type,'  T54/9 CTL = CTL1/CTL2');
        CTL := CTL1 / CTL2;
        --LOG(p_type,'CTL, Tx to 15C  ................................ ' || to_char(ROUND(CTL,12),'fm9999990d999999999999'));

        --LOG(p_type,'  T54/10');
        IF CTL <= 0 THEN
            --LOG(p_type,'CTL is negative or ZERO');
            RETURN -1;
        --ELSE
            --LOG(p_type,'CTL is positive, continue');
        END IF;
        --LOG(p_type,'  T54/11');
        IF p_type = 0 THEN
            CTL := ROUND(CTL,5);
        END IF;
        --LOG(p_type,'CTL (rounded) ...............' ||  to_char(CTL,'fm9999990d999999999999'));

        v_glb_coef := ROUND(CTL2 / CTL1,5);
        v_glb_coef_ := CTL;
        RETURN CTL;
    END;

    
    * Таблица T53E*
    * Вход:
    * p_denc - плотность при наблюдаемой температуре (кг\м3)
    * p_temp - наблюдаемая температура (Градусы)
    * Выход:
    * p_denc15 - плотность при 15
    
    FUNCTION FNC_TABLE_53E(p_denc IN NUMBER, p_temp IN NUMBER, p_type NUMBER DEFAULT 0) RETURN NUMBER IS
        p_denc15 NUMBER; -- плотность при 15 градусах
        Y_TF NUMBER;        -- округленная плотность до 0.1 кг\м3
        TF NUMBER;           -- окуругленная температура до 0.05 градусов
        T_x NUMBER;          -- Температура в Кельвинах
        Y_x NUMBER;          -- плотность воды при 60 фаренгейтах
        Y_60 NUMBER;        -- относительная плотность при 60 фаренгейтах
        CTL NUMBER;          -- фактор  коррекции температуры
        Y_15 NUMBER;        -- относительная плотность при 15 градусах
    BEGIN
        p_denc15 := -1;
        --LOG(p_type,'Input Data to Implementation Procedure T53');
        --LOG(p_type,'Density @ obs. temp. (kg/m3) .....' || to_char(p_denc,'fm9999990d999999999999'));
        --LOG(p_type,'Observed Temperature Tf (C) .....' || to_char(p_temp,'fm9999990d999999999999'));
        --53/1
        --LOG(p_type,'  T53/1');
        Y_TF := ROUND(p_denc, 1);
        TF := ROUND(p_temp * 2, 1) / 2;--тут неверно - надо округлить до 0.5!!!
        --LOG(p_type,'Density, rounded to 0.1 .....' || to_char(Y_TF,'fm9999990d999999999999'));
        --LOG(p_type,'Temperature, rounded to 0.05 .....' || to_char(TF,'fm9999990d999999999999'));
        --53/2
        --LOG(p_type,'  T53/2');
        T_x := TF + 273.15;
        --LOG(p_type,'Tx, kelvin ......................' || to_char(T_x,'fm9999990d999999999999'));
        --53/3
        --LOG(p_type,'  T53/3');
        Y_x := Y_TF / 999.016;
        --LOG(p_type,'Density relative to 60 water.....' || to_char(Y_x,'fm9999990d999999999999'));
        --53/4
        --LOG(p_type,'  T53/4');
        IF T_x < 227.15 OR T_x > 366.15 OR Y_x < 0.2100 OR Y_x > 0.7400 OR Y_x IS NULL OR T_x IS NULL THEN
            --LOG(p_type,'Tx or relative density is not in segment: Tf=' || to_char(ROUND(T_x,12),'fm9999990d999999999999') || ' RDtf=' || to_char(Y_x,'fm9999990d999999999999'));
            RETURN p_denc15;
        --ELSE
            --LOG(p_type,'Tx or relative density are within range, continue');
        END IF;

        --53/5
        --LOG(p_type,'  T53/5  Call Table 23 procedure to obtain relative density at 60F');
        Y_60 := FNC_TABLE_23E(Y_x, T_x, 1);
        --LOG(p_type,'RD60 from Table 23.....' || to_char(Y_60,'fm9999990d999999999999'));
        IF Y_60 <= 0 THEN
            --LOG(p_type,'Value returned from Table 23 is not valid');
            RETURN p_denc15;
        END IF;

        --53/6
        --LOG(p_type,'  T53/6  Call Table 24 procedure to obtain CTL from 60F to 15C');
        CTL := FNC_TABLE_24E(Y_60, 288.15, 1);
        IF CTL <= 0 THEN
            --LOG(p_type,'Value returned from Table 24 is not valid');
            RETURN p_denc15;
        END IF;
        Y_15 := CTL * Y_60;
        --LOG(p_type,'CTL from Table 24 .....' || to_char(CTL,'fm9999990d999999999999'));
        --LOG(p_type,'Relative density at 15C .....' || to_char(Y_15,'fm9999990d999999999999'));

        --53/7
        --LOG(p_type,'  T53/7');
        --LOG(p_type,'Values returned from Table 23 and 24 valid, continue');

        --53/8
        --LOG(p_type,'  T53/8');
        p_denc15 := Y_15*999.016;
        --LOG(p_type,'Density at 15C (kg/m3) .....' || to_char(p_denc15,'fm9999990d999999999999'));

        --53/9
        --LOG(p_type,'  T53/9');
        IF p_type = 0 THEN
            p_denc15 := ROUND(p_denc15,1);
        END IF;
        --LOG(p_type,'Density at 15C (rounded) .....' || to_char(p_denc15,'fm9999990d999999999999'));

        v_glb_coef  := ROUND(p_denc15 / p_denc, 12);
        v_glb_coef_ := ROUND(p_denc / p_denc15, 12);

        RETURN p_denc15;
    END;
*/