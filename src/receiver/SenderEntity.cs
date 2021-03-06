﻿using Sandbox.Game.EntityComponents;
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
        public class SenderEntity
        {
            public long ID { get; private set; }
            public DateTime LastUpdate { get; set; }
            public Status CurrentStatus { get; set; }
            public LCDDrawEntity LCD { get; set; }
            public int[] LineNumber { get; private set; }

            public enum Status : byte { Connected, Disconnected }

            public SenderEntity(long id, DateTime lastUpdate, LCDDrawEntity lcd, Status status, int[] lineNumber)
            {
                ID = id;
                LastUpdate = lastUpdate;
                LCD = lcd;
                CurrentStatus = status;
                LineNumber = lineNumber;
            }
        }
    }
}
