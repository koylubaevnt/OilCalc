using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace OilCalc.Classes
{
    class StandartLimits // : IEnumerable
    {
        public StandartLimits()
        { }
        /*public enum StandartLimitType { DENSITY, TEMPERATURE };
        public List<StandartLimit> StandartLimitsTable { get; private set; }

        public StandartLimits()
        {
            StandartLimitsTable = new List<StandartLimit>();
            StandartLimitsTable.Add(new StandartLimit(API_MPMS_11_1.TableCalc.T24E, StandartLimitType.DENSITY, 0.0m, 0.0m));
            StandartLimitsTable.Add(new StandartLimit(API_MPMS_11_1.TableCalc.T23E, StandartLimitType.DENSITY, 0.0m, 0.0m));
            StandartLimitsTable.Add(new StandartLimit(API_MPMS_11_1.TableCalc.T54E, StandartLimitType.DENSITY, 0.0m, 0.0m));
            StandartLimitsTable.Add(new StandartLimit(API_MPMS_11_1.TableCalc.T53E, StandartLimitType.DENSITY, 0.0m, 0.0m));
            StandartLimitsTable.Add(new StandartLimit(API_MPMS_11_1.TableCalc.T60E, StandartLimitType.DENSITY, 0.0m, 0.0m));
            StandartLimitsTable.Add(new StandartLimit(API_MPMS_11_1.TableCalc.T59E, StandartLimitType.DENSITY, 0.0m, 0.0m));
            
        }
        
        public IEnumerator GetEnumerator()
        {
            return (StandartLimitsTable as IEnumerable).GetEnumerator();
        }

        class StandartLimit
        {
            API_MPMS_11_1.TableCalc table;
            StandartLimitType limitType;

            decimal upperBoundary;
            decimal lowerBoundary;

            public StandartLimit(API_MPMS_11_1.TableCalc table, StandartLimitType limitType, decimal upperBoundary, decimal lowerBoundary)
            {
                this.table = table;
                this.limitType = limitType;
                this.upperBoundary = upperBoundary;
                this.lowerBoundary = lowerBoundary;
            }

        }
     */   
    }
}
