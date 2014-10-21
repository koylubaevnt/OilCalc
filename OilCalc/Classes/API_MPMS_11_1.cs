using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OilCalc.ReferenceTables;
using System.Collections;

namespace OilCalc.Classes
{
    public class API_MPMS_11_1
    {
        public enum TableCalc { T24E, T23E, T54E, T53E, T60E, T59E };
        decimal tempKelvin = 273.15m;

        private ReferenceFluidParameterTable rfpt = new ReferenceFluidParameterTable();
        
        public string LogInfo { get; private set; }
        private StringBuilder sb = new StringBuilder();

        #region Конструкторы

        /// <summary>
        /// Конструктор класса "Temperature Correction for the Volume of NGL and LPG Tables 23E, 24E, 53E, 54E, 59E and 60E" со всеми параметрами
        /// </summary>
        /// <param name="table">Тип таблицы для расчета (24Е, 23Е, 54Е, 53Е, 60Е, 59Е)</param>
        public API_MPMS_11_1(TableCalc table)
        {
            this.Table = table;
            this.LogInfo = string.Empty;
        }

        /// <summary>
        /// Конструктор класса "Temperature Correction for the Volume of NGL and LPG Tables 23E, 24E, 53E, 54E, 59E and 60E" по умолчанию
        /// </summary>
        public API_MPMS_11_1() : this(TableCalc.T24E) { }

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

            if (Table == TableCalc.T23E)
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
            
            return result;
        }

        public decimal Calculate(TableCalc table, decimal dens, decimal temp)
        {
            this.Table = table;

            return Calculate(dens, temp);
        }

        #endregion

        #region Основные скрытые методы и функции

        private decimal CalculateT24E(bool needCheckBoundaries, decimal dens, decimal temp)
        {
            decimal temperatureKelvin, density, temperature, result = -1.0m;

            sb.AppendLine("Table 24");
            sb.AppendLine("\tInput Data.");
            sb.AppendLine("Relative density @60F: " + dens.ToString());
            sb.AppendLine("Observed Temperature, F: " + temp.ToString());

            if (needCheckBoundaries)
            {
                decimal[] limitTemperature = new decimal[2];
                decimal[] limitDensity = new decimal[2];
                limitTemperature[0] = 227.15m;
                limitTemperature[1] = 366.15m;
                limitDensity[0] = 0.3500m;
                limitDensity[1] = 0.6880m;

                density = RoundToNearest(dens, 0.0001m);
                temperature = RoundToNearest(temp, 0.1m);
                sb.AppendLine("\tComputed Data - last digit is rounded.");
                sb.AppendLine("  T24/1");
                sb.AppendLine("Relative density @60F:" + density.ToString());
                sb.AppendLine("Observed Temperature, F: " + temperature.ToString());
                
                temperatureKelvin = ConvertToKelvin(temperature);
                sb.AppendLine("  T24/2");
                sb.AppendLine("Temperature, Kelvin: " + temperatureKelvin.ToString());

                sb.AppendLine("  T24/3");
                bool isDensityInRange = CheckBoundaries(density, limitDensity);
                bool isTemperatureInRange = CheckBoundaries(temperatureKelvin, limitTemperature);
                if (!isDensityInRange && !isTemperatureInRange)
                {
                    sb.AppendLine("Density and temperature is not in range");
                    LogInfo = sb.ToString();
                    return result;
                }
                else if (!isDensityInRange)
                {
                    sb.AppendLine("Density is not in range");
                    LogInfo = sb.ToString();
                    return result;
                }
                else if (!isTemperatureInRange)
                {
                    sb.AppendLine("Temperature is not in range");
                    LogInfo = sb.ToString();
                    return result;
                }
                else
                    sb.AppendLine("Input data within range");
            }
            else
            {
                density = dens;
                temperatureKelvin = temp;
            }

            sb.AppendLine("  T24/4");
            ReferenceFluidParameter rfp1, rfp2;
            DetermineFluidReferences(density, out rfp1, out rfp2);
            sb.AppendLine("Reference Fluid 1: " + rfp1.Name);
            sb.AppendLine("Reference Fluid 2: " + rfp2.Name);
            
            sb.AppendLine("  T24/5");
            decimal sigma;
            sigma = (density - rfp1.RelativeDensity) / (rfp2.RelativeDensity - rfp1.RelativeDensity);
            sb.AppendLine("Delta: " + sigma.ToString());

            sb.AppendLine("  T24/6");
            decimal criticalTemperature;
            criticalTemperature = rfp1.CriticalTemperature + sigma * (rfp2.CriticalTemperature - rfp1.CriticalTemperature);
            sb.AppendLine("Critical temperature: " + criticalTemperature.ToString());

            sb.AppendLine("  T24/7");
            decimal reducedObservedTemperature;
            reducedObservedTemperature = temperatureKelvin / criticalTemperature;
            sb.AppendLine("Redused observed temperature: " + reducedObservedTemperature.ToString());
            if (reducedObservedTemperature > 1.0m)
            {
                sb.AppendLine("Redused temperature greather than 1.0");
                LogInfo = sb.ToString();
                return result;
            }

            sb.AppendLine("  T24/8");
            decimal reducedObservedTemperatureF;
            reducedObservedTemperatureF = 519.67m / (1.8m * criticalTemperature);
            sb.AppendLine("Redused temperature at 60F: " + reducedObservedTemperatureF.ToString());

            sb.AppendLine("  T24/9");
            decimal scalingFactor;
            scalingFactor = (rfp1.CriticalCompressiblityFactor * rfp1.CriticalDensity) / (rfp2.CriticalCompressiblityFactor * rfp2.CriticalDensity);
            sb.AppendLine("Scaling factor: " + scalingFactor.ToString());

            sb.AppendLine("  T24/10");
            decimal tau;
            decimal[] saturationDensity60 = new decimal[2];
            decimal[] saturationDensity = new decimal[2];
            tau = 1.0m - reducedObservedTemperatureF;
            SaturationDensity(rfp1, tau, out saturationDensity60[0]);
            SaturationDensity(rfp2, tau, out saturationDensity60[1]);

            sb.AppendLine("Tau for fluid at 60F: " + tau.ToString());
            sb.AppendLine("Sat den fluid 1 at 60F: " + saturationDensity60[0].ToString());
            sb.AppendLine("Sat den fluid 2 at 60F: " + saturationDensity60[1].ToString());

            sb.AppendLine("  T24/11");
            decimal interpolatingFactor;
            interpolatingFactor = saturationDensity60[0] /
                (1.0m + sigma * ((saturationDensity60[0] / (scalingFactor * saturationDensity60[1])) - 1.0m));
            sb.AppendLine("Interpolating factor: " + interpolatingFactor.ToString());

            sb.AppendLine("  T24/12");
            tau = 1.0m - reducedObservedTemperature;
            SaturationDensity(rfp1, tau, out saturationDensity[0]);
            SaturationDensity(rfp2, tau, out saturationDensity[1]);

            sb.AppendLine("Tau for fluid at obs temp: " + tau.ToString());
            sb.AppendLine("Sat den fluid 1 at obs temp: " + saturationDensity[0].ToString());
            sb.AppendLine("Sat den fluid 2 at obs temp: " + saturationDensity[1].ToString());

            sb.AppendLine("  T24/13");
            result = saturationDensity[0] /
                (interpolatingFactor * (1.0m + sigma * (saturationDensity[0] / (scalingFactor * saturationDensity[1]) - 1.0m)));
            sb.AppendLine("CTL: " + result.ToString());

            if (needCheckBoundaries)
            {
                sb.AppendLine("  T24/14");
                result = RoundToNearest(result, 0.00001m);
                sb.AppendLine("CTL rounded: " + result.ToString());
            }
            LogInfo = sb.ToString();
            return result;
        }
        
        private decimal CalculateT23E(bool needCheckBoundaries, decimal dens, decimal temp)
        {
            decimal temperatureKelvin, density, temperature, result = -1.0m;
            
            sb.AppendLine("Table 23");
            sb.AppendLine("\tInput Data.");
            sb.AppendLine("Relative density obs.: " + dens.ToString());
            sb.AppendLine("Observed Temperature, F: " + temp.ToString());
            
            density = RoundToNearest(dens, 0.0001m);
            temperature = RoundToNearest(temp, 0.1m);
            sb.AppendLine("\tComputed Data - last digit is rounded.");
            sb.AppendLine("  T23/1");
            sb.AppendLine("Relative density obs.:" + density.ToString());
            sb.AppendLine("Observed Temperature, F: " + temperature.ToString());

            if (needCheckBoundaries)
            {
                decimal[] limitDensity = new decimal[2];
                decimal[] limitTemperature = new decimal[2];
                limitTemperature[0] = 227.15m;
                limitTemperature[1] = 366.15m;
                limitDensity[0] = 0.2100m;
                limitDensity[1] = 0.7400m;

                temperatureKelvin = ConvertToKelvin(temperature);
                sb.AppendLine("  T23/2");
                sb.AppendLine("Temperature, Kelvin: " + temperatureKelvin.ToString());

                sb.AppendLine("  T23/3");
                bool isDensityInRange = CheckBoundaries(density, limitDensity);
                bool isTemperatureInRange = CheckBoundaries(temperatureKelvin, limitTemperature);
                if (!isDensityInRange && !isTemperatureInRange)
                {
                    sb.AppendLine("Density and temperature is not in range");
                    LogInfo = sb.ToString();
                    return result;
                }
                else if (!isDensityInRange)
                {
                    sb.AppendLine("Density is not in range");
                    LogInfo = sb.ToString();
                    return result;
                }
                else if (!isTemperatureInRange)
                {
                    sb.AppendLine("Temperature is not in range");
                    LogInfo = sb.ToString();
                    return result;
                }
                else
                    sb.AppendLine("Input data within range");
            }
            else
            {
                density = dens;
                temperatureKelvin = temp;
            }
            sb.AppendLine("  T23/4");
            decimal
                reducedObservedTemperature = 0.0m,
                reducedObservedTemperature60 = 0.0m,
                sat = 0.0m,
                sat60 = 0.0m,
                relativeObservedDensityRef = 0.0m,
                relativeObservedDensityRef1 = 0.0m,
                relativeObservedDensityRef2 = 0.0m;

            int i, indexFirstFluid = 0;
            ReferenceFluidParameter rfp_temp, rfp1, rfp2;
            
            foreach (ReferenceFluidParameter rfp in rfpt)
            {
                reducedObservedTemperature = temperatureKelvin / rfp.CriticalTemperature;
                if (reducedObservedTemperature <= 1.0m)
                {
                    indexFirstFluid = rfpt.ReferenceFluidParameterList.IndexOf(rfp);
                    reducedObservedTemperature60 = 519.67m / (1.8m * rfp.CriticalTemperature);
                    SaturationDensity(rfp, 1.0m - reducedObservedTemperature, out sat);
                    SaturationDensity(rfp, 1.0m - reducedObservedTemperature60, out sat60);
                    relativeObservedDensityRef = rfp.RelativeDensity * (sat / sat60);

                    if ((density <= relativeObservedDensityRef && relativeObservedDensityRef > 0.0m) || indexFirstFluid == 11)
                    {
                        if (indexFirstFluid == 0)
                        {
                            rfp_temp = (ReferenceFluidParameter)rfpt.ReferenceFluidParameterList[indexFirstFluid + 1];
                            relativeObservedDensityRef1 = relativeObservedDensityRef;
                            reducedObservedTemperature = temperatureKelvin / rfp_temp.CriticalTemperature;
                            reducedObservedTemperature60 = 519.67m / (1.8m * rfp_temp.CriticalTemperature);
                            SaturationDensity(rfp_temp, reducedObservedTemperature, out sat);
                            SaturationDensity(rfp_temp, reducedObservedTemperature60, out sat60);
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
            rfp1 = ((ReferenceFluidParameter)rfpt.ReferenceFluidParameterList[indexFirstFluid]);
            rfp2 = ((ReferenceFluidParameter)rfpt.ReferenceFluidParameterList[indexFirstFluid + 1]);
            sb.AppendLine("Reference Fluid 1: " + rfp1.Name);
            sb.AppendLine("Reference Fluid 2: " + rfp2.Name);

            if (density > relativeObservedDensityRef2)
            {
                sb.AppendLine("RDtf observed is greather than upper boundary RDtf, no solution");
                LogInfo = sb.ToString();
                return result;
            }

            decimal Trx1, Trx2, Tr601, Tr602;
            Trx1 = temperatureKelvin / rfp1.CriticalTemperature;
            Trx2 = temperatureKelvin / rfp2.CriticalTemperature;
            Tr601 = 519.67m / (1.8m * rfp1.CriticalTemperature);
            Tr602 = 519.67m / (1.8m * rfp2.CriticalTemperature);
            sb.AppendLine("Tr,x for Fluid 1: " + Trx1.ToString());
            sb.AppendLine("Tr,x for Fluid 2: " + Trx2.ToString());
            sb.AppendLine("Tr,60 for Fluid 1: " + Tr601.ToString());
            sb.AppendLine("Tr,60 for Fluid 2: " + Tr602.ToString());

            sb.AppendLine("  T23/6");
            decimal relativeObservedDensity60Hight, relativeObservedDensityHight,
                relativeObservedDensity60Low, relativeObservedDensityLow;
            relativeObservedDensity60Hight = rfp2.RelativeDensity;
            relativeObservedDensityHight = relativeObservedDensityRef2;
            relativeObservedDensity60Low = rfp1.RelativeDensity;
            relativeObservedDensityLow = relativeObservedDensityRef1;

            if (Trx1 > 1.0m)
            {
                relativeObservedDensity60Low = ((temperatureKelvin - rfp1.CriticalTemperature) / (rfp2.CriticalTemperature - rfp1.CriticalTemperature)) * (rfp2.RelativeDensity - rfp1.RelativeDensity) + rfp1.RelativeDensity;

            }
            if (relativeObservedDensity60Low < 0.3500m)
            {
                relativeObservedDensity60Low = 0.3500m;
            }
            decimal ret;
            ret = CalculateT24E(false, relativeObservedDensity60Low, temperatureKelvin);
            if (ret == -1.0m)
                relativeObservedDensityLow = -1.0m;
            else
                relativeObservedDensityLow = ret * relativeObservedDensity60Low;
            
            sb.AppendLine("Upper boundary 60: " + relativeObservedDensity60Hight);
            sb.AppendLine("Upper boundary: " + relativeObservedDensityHight);
            sb.AppendLine("Lower boundary 60: " + relativeObservedDensity60Low);
            sb.AppendLine("Lower boundary: " + relativeObservedDensityLow);
            if (density < relativeObservedDensityLow)
            {
                sb.AppendLine("RDtf observed is less than lower boundary RDtf, no solution");
                LogInfo = sb.ToString();
                return -1.0m;
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
                    relativeObservedDensity60Mid = (relativeObservedDensity60Hight - relativeObservedDensity60Low) / 2.0m;

                sb.AppendLine("RD60,mid: " + relativeObservedDensity60Mid.ToString());

                ret = CalculateT24E(false, relativeObservedDensity60Mid, temperatureKelvin);
                sb.AppendLine("CTL: " + ret.ToString());
                relativeObservedDensityMid = ret * relativeObservedDensity60Mid;
                
                sb.AppendLine("RDTf,mid " + relativeObservedDensityMid.ToString());

                sb.AppendLine("  T23/8");
                if ((density > relativeObservedDensityLow && density < relativeObservedDensityMid && Math.Abs(relativeObservedDensity60Low - relativeObservedDensity60Mid) < 0.00000001m) || (density < relativeObservedDensityHight && density > relativeObservedDensityMid && Math.Abs(relativeObservedDensity60Hight - relativeObservedDensity60Mid) < 0.00000001m))
                {
                    sb.AppendLine("Exit");
                    result = relativeObservedDensity60Mid;
                    break;
                }
                sb.AppendLine("Continue");
                sb.AppendLine("  T23/9");
                alpha = (relativeObservedDensity60Hight - relativeObservedDensity60Low);
                sb.AppendLine("Alpha: " + alpha.ToString());
                beta = (decimal)Math.Pow((double)relativeObservedDensityHight, 2d) - (decimal)Math.Pow((double)relativeObservedDensityLow, 2d);
                sb.AppendLine("Beta: " + beta.ToString());
                phi = (relativeObservedDensityHight - relativeObservedDensityLow) / (relativeObservedDensityMid - relativeObservedDensityLow);
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

                ret = CalculateT24E(false, relativeObservedDensity60Trial, temperatureKelvin);
                relativeObservedDensityTrial = ret * relativeObservedDensity60Trial;
                sb.AppendLine("CTL, Trial: " + ret.ToString());
                sb.AppendLine("RDTf, Trial: " + relativeObservedDensityTrial.ToString());

                sb.Append("  T23/10");
                if (Math.Abs(density - relativeObservedDensityTrial) < 0.00000001m)
                {
                    result = relativeObservedDensity60Trial;
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
                    result = -1.0m;
                    sb.AppendLine("No result");
                    LogInfo = sb.ToString();
                    return result;
                }
                i++;
            }

            if (needCheckBoundaries)
            {
                result = RoundToNearest(result, 0.0001m);
                sb.AppendLine("  T23/12");
                if (result < 0.3500m || result > 0.6880m)
                {
                    result = -1.0m;
                    sb.AppendLine("Out off boundaries");
                    LogInfo = sb.ToString();
                    return result;
                }
            }
            sb.AppendLine("RD60(RD60, Trial): " + result);
            LogInfo = sb.ToString();
            return result;
        }
        
        private decimal CalculateT53ET59E(decimal dens, decimal temp, decimal tempDelta)
        {
            decimal temperatureKelvin, density, temperature, result = -1.0m;

            sb.AppendLine("Input Data to Implementation Procedure T53");
            sb.AppendLine("\tInput Data.");
            sb.AppendLine("Density @ obs. temp. (kg/m3): " + dens.ToString());
            sb.AppendLine("Observed Temperature Tf (C): " + temp.ToString());

            decimal[] limitTemperature = new decimal[2];
            decimal[] limitDensity = new decimal[2];
            if (tempDelta == 15.0m)
            {
                limitTemperature[0] = 227.15m;
                limitTemperature[1] = 366.15m;
                limitDensity[0] = 0.2100m;
                limitDensity[1] = 0.7400m;
            }
            else
            {
                limitTemperature[0] = 227.15m;
                limitTemperature[1] = 366.15m;
                limitDensity[0] = 0.2100m;
                limitDensity[1] = 0.7400m;
            }
            density = RoundToNearest(dens, 0.1m);
            temperature = RoundToNearest(temp, 0.05m);
            sb.AppendLine("\tComputed Data - last digit is rounded.");
            sb.AppendLine("  T53/1");
            sb.AppendLine("Density, rounded to 0.1:" + density.ToString());
            sb.AppendLine("Temperature, rounded to 0.05: " + temperature.ToString());
            
            temperatureKelvin = ConvertToKelvinC(temperature);
            sb.AppendLine("  T53/2");
            sb.AppendLine("Temperature, Kelvin: " + temperatureKelvin.ToString());

            sb.AppendLine("  T53/3");
            density = density / 999.016m;
            sb.AppendLine("Density relative to 60 water....." + density);
            
            sb.AppendLine("  T53/4");
            bool isDensityInRange = CheckBoundaries(RoundToNearest(density, 0.0001m), limitDensity);
            bool isTemperatureInRange = CheckBoundaries(temperatureKelvin, limitTemperature);
            if (!isDensityInRange && !isTemperatureInRange)
            {
                sb.AppendLine("Density and temperature is not in range");
                LogInfo = sb.ToString();
                return result;
            }
            else if (!isDensityInRange)
            {
                sb.AppendLine("Density is not in range");
                LogInfo = sb.ToString();
                return result;
            }
            else if (!isTemperatureInRange)
            {
                sb.AppendLine("Temperature is not in range");
                LogInfo = sb.ToString();
                return result;
            }
            else
                sb.AppendLine("Input data within range");
            
            sb.AppendLine("  T53/5  Call Table 23 procedure to obtain relative density at 60F");
            
            decimal Y_60 = CalculateT23E(false, density, temperatureKelvin);
            sb.AppendLine("RD60 from Table 23: " + Y_60);
            if (Y_60 <= 0.0m)
            {
                sb.AppendLine("Value returned from Table 23 is not valid");
                LogInfo = sb.ToString();
                return result;
            }

            sb.AppendLine("  T53/6  Call Table 24 procedure to obtain CTL from 60F to 15C");
            decimal CTL = CalculateT24E(false, Y_60, tempKelvin + tempDelta);
            if (CTL <= 0.0m)
            {
                sb.AppendLine("Value returned from Table 24 is not valid");
                LogInfo = sb.ToString();
                return result;
            }

            decimal Y_15;
            Y_15 = CTL * Y_60;
            sb.AppendLine("CTL from Table 24: " + CTL);
            sb.AppendLine("Relative density at 15C: " + Y_15);

            sb.AppendLine("  T53/7");
            sb.AppendLine("Values returned from Table 23 and 24 valid, continue");

            sb.AppendLine("  T53/8");
            result = Y_15 * 999.016m;
            sb.AppendLine("Density at 15C (kg/m3): " + result);

            sb.AppendLine("  T53/9");
            result = RoundToNearest(result, 0.1m);
            sb.AppendLine("Density at 15C (rounded): " + result);
            LogInfo = sb.ToString();
            return result;
        }
        private decimal CalculateT54ET60E(decimal dens, decimal temp, decimal tempDelta)
        {
            decimal temperatureKelvin, density, temperature, result = -1.0m;

            sb.AppendLine("Table 54");
            sb.AppendLine("\tInput Data to Implementation Procedure T54.");
            sb.AppendLine("Density (kg/m3) @ 15 C (Denc15): " + dens.ToString());
            sb.AppendLine("Observed temperature Tf, C: " + temp.ToString());
            
            decimal[] limitTemperature = new decimal[2];
            decimal[] limitDensity = new decimal[2];

            if (tempDelta == 15.0m)
            {
                limitTemperature[0] = 227.15m;
                limitTemperature[1] = 366.15m;
                limitDensity[0] = 351.7m;
                limitDensity[1] = 687.8m;
            }
            else
            {
                limitTemperature[0] = 227.15m;
                limitTemperature[1] = 366.15m;
                limitDensity[0] = 331.7m;
                limitDensity[1] = 683.6m;
            }
            density = RoundToNearest(dens, 0.1m);
            temperature = RoundToNearest(temp, 0.05m);
            sb.AppendLine("\tComputed Data - last digit is rounded.");
            sb.AppendLine("  T54/1");
            sb.AppendLine("Denc15 rounded to 0.1:" + density.ToString());
            sb.AppendLine("Tf, C, rounded to 0.05: " + temperature.ToString());

            temperatureKelvin = ConvertToKelvinC(temperature);
            sb.AppendLine("  T54/2");
            sb.AppendLine("Temperature, Kelvin: " + temperatureKelvin.ToString());

            sb.AppendLine("  T54/3");
            bool isDensityInRange = CheckBoundaries(density, limitDensity);
            bool isTemperatureInRange = CheckBoundaries(temperatureKelvin, limitTemperature);
            if (!isDensityInRange && !isTemperatureInRange)
            {
                sb.AppendLine("Density and temperature is not in range");
                LogInfo = sb.ToString();
                return result;
            }
            else if (!isDensityInRange)
            {
                sb.AppendLine("Density is not in range");
                LogInfo = sb.ToString();
                return result;
            }
            else if (!isTemperatureInRange)
            {
                sb.AppendLine("Temperature is not in range");
                LogInfo = sb.ToString();
                return result;
            }
            else
                sb.AppendLine("Input data within range");
            
            sb.AppendLine("  T54/4");
            decimal Y_TB = density / 999.016m;
            sb.AppendLine("Denc15 relative to 60F water: " + Y_TB);

            sb.AppendLine("\tT54/5 Call Table 23 procedure to obtain relative density at 60F: ");
            decimal Y_60 = CalculateT23E(false, Y_TB, tempKelvin + tempDelta);
            sb.AppendLine("RD60 from Table 23: " + Y_60);

            sb.AppendLine("\tT54/6");
            if (RoundToNearest(Y_60, 0.0001m) < 0.34995m || RoundToNearest(Y_60, 0.0001m) > 0.68805m)
            {
                sb.AppendLine("RD60 is not in range, no solution");
                this.LogInfo = sb.ToString();
                return result;
            }
            else
                sb.AppendLine("RD60 is within range, continue");

            sb.AppendLine("\tT54/7 Call Table 24 Procedure with Tx and RD60");

            decimal c1, c2;

            c1 = CalculateT24E(false, Y_60, temperatureKelvin);
            if (c1 <= 0.0m)
            {
                sb.AppendLine("Value from Table 24 not valid, no solution");
                LogInfo = sb.ToString();
                return result;
            }

            sb.AppendLine("\tT54/8");
            c2 = CalculateT24E(false, Y_60, tempKelvin + tempDelta);
            if (c2 <= 0.0m)
            {
                sb.AppendLine("Value from Table 24 not valid, no solution");
                LogInfo = sb.ToString();
                return result;
            }

            decimal ctl;
            sb.AppendLine("\tT54/9");
            ctl = c1 / c2;
            if (ctl <= 0.0m)
            {
                sb.AppendLine("CTL is negative or ZERO");
                LogInfo = sb.ToString();
                return result;
            }
            else
                sb.AppendLine("CTL is positive, continue");

            sb.AppendLine("\tT54/11");

            result = RoundToNearest(ctl, 0.00001m);
            sb.AppendLine("CTL (rounded): " + result);

            LogInfo = sb.ToString();
            return result;
        }

        #endregion

        #region Вспомогательные функции

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

        public static decimal ConvertToKelvinC(decimal value)
        {
            return value + 273.15m;

        }
        
        public bool CheckBoundaries(decimal value, decimal[] limit)
        {
            if (value < limit[0] || value > limit[1])
                return false;
            else
                return true;
        }

        private void DetermineFluidReferences(decimal density, out ReferenceFluidParameter rfp1, out ReferenceFluidParameter rfp2)
        {
            rfp1 = (ReferenceFluidParameter) rfpt.ReferenceFluidParameterList[0];
            rfp2 = (ReferenceFluidParameter)rfpt.ReferenceFluidParameterList[0];
            foreach (ReferenceFluidParameter rfp in rfpt)
            {
                if (density <= rfp.RelativeDensity)
                {
                    rfp1 = rfp1;
                    rfp2 = rfp;
                    break;
                }
                rfp1 = rfp;
            }
        }

        private void SaturationDensity(ReferenceFluidParameter rfp, decimal tau, out decimal sat)
        {
            sat = rfp.CriticalDensity *
                (1 +
                    (((rfp.SaturationDensityFittingParameter[0] * (decimal)Math.Pow((double)tau, 0.35d) +
                        rfp.SaturationDensityFittingParameter[2] * (decimal)Math.Pow((double)tau, 2d) +
                        rfp.SaturationDensityFittingParameter[3] * (decimal)Math.Pow((double)tau, 3d)
                    ) / (
                        1 + rfp.SaturationDensityFittingParameter[1] * (decimal)Math.Pow((double)tau, 0.65d))))
                );
        }
        #endregion
    }
}
