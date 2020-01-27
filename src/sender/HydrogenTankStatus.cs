using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class HydrogenTankStatus
        {
            
            public double MaxCapacity { get; private set; }

            private readonly List<IMyGasTank> _gasTanks;

            private readonly float _smallTankCapacity;
            private readonly float _largeTankCapacity;

            private readonly Program _program;

            public HydrogenTankStatus(Program program, CustomDataEntity ini)
            {
                _program = program;
                _gasTanks = new List<IMyGasTank>();

                _smallTankCapacity = ini.SmallTankCapacity;
                _largeTankCapacity = ini.LargeTankCapacity;
            }

            public double CheckCapacity()
            {
                MaxCapacity = 0.0d;
                double currentCapacity = 0.0d;

                _program.GridTerminalSystem.GetBlocksOfType<IMyGasTank>(_gasTanks);
                foreach (IMyGasTank tank in _gasTanks)
                {
                    if (tank.Capacity == _smallTankCapacity || tank.Capacity == _largeTankCapacity )
                    {
                        MaxCapacity += tank.Capacity;
                        currentCapacity += (tank.FilledRatio * tank.Capacity);
                    }                         
                }
                return currentCapacity;
            }
        }
    }
}
