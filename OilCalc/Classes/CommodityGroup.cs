using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OilCalc.Classes
{
    class CommodityGroup
    {
        enum ComodityName { CrudeOil, FuelOil, JetFuels, TransitionZone, Gasolines, LubricantingOil }
        enum ComodityGroupName { CrudeOil, RefinedProducts, LubricantingOil }
        Entity e;
        GroupEntity g;

        CommodityGroup()
        {
            //ComodityName.CrudeOil
            e = new Entity(ComodityGroupName.CrudeOil, new decimal[2] {610.6m, 1163.5m}, new decimal[3] {341.0957m, 0.0m, 0.0m});
            //ComodityName.FuelOil
            e = new Entity(ComodityGroupName.RefinedProducts, new decimal[2] { 838.3127m, 1163.5m }, new decimal[3] { 103.8720m, 0.2701m, 0.0m });
            //ComodityName.JetFuels
            e = new Entity(ComodityGroupName.RefinedProducts, new decimal[2] { 787.5195m, 838.3127m }, new decimal[3] { 330.3010m, 0.0m, 0.0m });
            //ComodityName.TransitionZone
            e = new Entity(ComodityGroupName.RefinedProducts, new decimal[2] { 770.3520m, 787.5195m }, new decimal[3] { 1489.0670m, 0.0m, -0.00186840m });
            //ComodityName.Gasolines
            e = new Entity(ComodityGroupName.RefinedProducts, new decimal[2] { 610.6m, 770.3520m }, new decimal[3] { 192.4571m, 0.2438m, 0.0m });
            //ComodityName.LubricantingOil
            e = new Entity(ComodityGroupName.LubricantingOil, new decimal[2] { 800.9m, 1163.5m }, new decimal[3] { 0.0m, 0.34878m, 0.0m });

            //ComodityGroupName.CrudeOil
            g = new GroupEntity(new decimal[2] { 610.6m, 1163.5m });
            //ComodityGroupName.RefinedProducts
            g = new GroupEntity(new decimal[2] { 610.6m, 1163.5m });
            //ComodityGroupName.LubricantingOil
            g = new GroupEntity(new decimal[2] { 610.6m, 1163.5m });
            
        }


        class Entity
        {
            //ComodityName Name { set; get; }
            ComodityGroupName GroupName { set; get; }
            decimal[] DensityRange { set; get; }
            decimal[] K { set; get; }

            public Entity(/*ComodityName name, */ComodityGroupName groupName, decimal[] densityRange, decimal[] k)
            {
                //Name = name;
                GroupName = groupName;
                if (densityRange.Length == 2)
                    DensityRange = densityRange;
                else
                {
                    DensityRange = new decimal[2];
                    DensityRange[0] = 0.0m;
                    DensityRange[1] = 0.0m;
                }
                if (k.Length == 3)
                    K = k;
                else
                {
                    K = new decimal[3];
                    K[0] = 0.0m;
                    K[1] = 0.0m;
                    K[2] = 0.0m;
                }
                
            }

        }

        class GroupEntity
        {
            decimal[] DensityRange { set; get; }

            public GroupEntity(decimal[] densityRange)
            {
                if (densityRange.Length == 2)
                    DensityRange = densityRange;
                else
                {
                    DensityRange = new decimal[2];
                    DensityRange[0] = 0.0m;
                    DensityRange[1] = 0.0m;
                }
            }
        }
    }
}
