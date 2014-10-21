using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace OilCalc.ReferenceTables
{
    public class ReferenceFluidParameterTable : IEnumerable
    {
        public ArrayList ReferenceFluidParameterList { get; private set; }

        public ReferenceFluidParameterTable()
        {
            ReferenceFluidParameterList = new ArrayList(12);
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("EE (68/32)", 0.325022m, 298.11m, 0.27998m, 6.250m, new decimal[] { 2.54616855327m, -0.058244177754m, 0.803398090807m, -0.745720314137m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("Ethane", 0.355994m, 305.33m, 0.28220m, 6.870m, new decimal[] { 1.89113042610m, -0.370305782347m, -0.544867288720m, 0.337876634952m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("EP (65/35)", 0.429277m, 333.67m, 0.28060m, 5.615m, new decimal[] { 2.20970078464m, -0.294253708172m, -0.405754420098m, 0.319443433421m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("EP (35/65)", 0.470381m, 352.46m, 0.27930m, 5.110m, new decimal[] { 2.25341981320m, -0.266542138024m, -0.372756711655m, 0.384734185665m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("Propane", 0.507025m, 369.78m, 0.27626m, 5.000m, new decimal[] { 1.96568366933m, -0.327662435541m, -0.417979702538m, 0.303271602831m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("i-Butane", 0.562827m, 407.85m, 0.28326m, 3.860m, new decimal[] { 2.04748034410m, -0.289734363425m, -0.330345036434m, 0.291757103132m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("n-Butane", 0.584127m, 425.16m, 0.27536m, 3.920m, new decimal[] { 2.03734743118m, -0.299059145695m, -0.418883095671m, 0.380367738748m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("i-Pentane", 0.624285m, 460.44m, 0.27026m, 3.247m, new decimal[] { 2.06541640707m, -0.238366208840m, -0.161440492247m, 0.258681568613m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("n-Pentane", 0.631054m, 469.65m, 0.27235m, 3.200m, new decimal[] { 2.11263474494m, -0.261269413560m, -0.291923445075m, 0.308344290017m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("i-Hexane", 0.657167m, 498.05m, 0.26706m, 2.727m, new decimal[] { 2.02382197871m, -0.423550090067m, -1.152810982570m, 0.950139001678m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("n-Hexane", 0.664064m, 507.35m, 0.26762m, 2.704m, new decimal[] { 2.17134547773m, -0.232997313405m, -0.267019794036m, 0.378629524102m }));
            ReferenceFluidParameterList.Add(new ReferenceFluidParameter("n-Heptane", 0.688039m, 540.15m, 0.26312m, 2.315m, new decimal[] { 2.19773533433m, -0.275056764147m, -0.447144095029m, 0.493770995799m }));
        }

        public IEnumerator GetEnumerator()
        {
            return (ReferenceFluidParameterList as IEnumerable).GetEnumerator();
        }
    }
}
