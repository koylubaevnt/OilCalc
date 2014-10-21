using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace OilCalc.ReferenceTables
{
    public class ReferenceFluidParameter : IEnumerable
    {
        
        public string Name { get; private set; }
        public decimal RelativeDensity { get; private set; }
        public decimal CriticalTemperature { get; private set; }
        public decimal CriticalCompressiblityFactor { get; private set; }
        public decimal CriticalDensity { get; private set; }
        public decimal[] SaturationDensityFittingParameter { get; private set; }

        public ReferenceFluidParameter(string Name, decimal RelativeDensity,
            decimal CriticalTemperature, decimal CriticalCompressiblityFactor,
            decimal CriticalDensity, decimal[] SaturationDensityFittingParameter)
        {
            this.Name = Name;
            this.RelativeDensity = RelativeDensity;
            this.CriticalTemperature = CriticalTemperature;
            this.CriticalCompressiblityFactor = CriticalCompressiblityFactor;
            this.CriticalDensity = CriticalDensity;
            this.SaturationDensityFittingParameter = SaturationDensityFittingParameter;
        }

        public ReferenceFluidParameter()
        { }
        public ArrayList ReferenceFluidParameterTable { get; private set; }
        public IEnumerator GetEnumerator()
        {
            return (ReferenceFluidParameterTable as IEnumerable).GetEnumerator();
        }
    }
}
