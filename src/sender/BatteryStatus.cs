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
        public class BatteryStatus
        {
            
            public float MaxStoredPower { get; private set; }

            private readonly List<IMyBatteryBlock> _batteryBlocks;
            private readonly Program _program;
            public BatteryStatus(Program program)
            {
                _program = program;

                _batteryBlocks = new List<IMyBatteryBlock>();
            }

            public float CurrentStoredPower()
            {
                MaxStoredPower = 0;
                float currentStoredPower = 0.0f;

                _batteryBlocks.Clear();
                _program.GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(_batteryBlocks);
                foreach (IMyBatteryBlock battery in _batteryBlocks)
                {
                    MaxStoredPower += battery.MaxStoredPower;
                    currentStoredPower += battery.CurrentStoredPower;
                }
                return currentStoredPower;    
            }
        }
    }
}
