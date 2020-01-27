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
        public class Sender
        {
            private GridCommunication _gridCommunication;

            private readonly LCDUtil _lcdUtil;

            private readonly BatteryStatus _batteryStatus;
            private readonly HydrogenTankStatus _hydrogenTankStatus;

            private int _count;
            private readonly string _senderName;
            private readonly int[] _lineLocation;

            private readonly Program _program;
            public Sender(Program program, CustomDataIni ini)
            {
                _program = program;

                _count = 0;
                _senderName = ini.Data.SenderName;

                _gridCommunication = new GridCommunication(_program, GridCommunication.ClientType.Sender, ini.Data.Channel);

                _lcdUtil = new LCDUtil(_program);
                _lcdUtil.Add(ini.Data.LcdOutputList);
                _lcdUtil.TextContentOn();
                _lcdUtil.SetFont(LCDUtil.FontColor.Green, 1f);
                _lcdUtil.Header = $"-- Sender --\n";

                _lineLocation = new int[] {
                    _lcdUtil.Write("TimeStamp"),
                    _lcdUtil.Write("My Name"),
                    _lcdUtil.Write("Battery Power"),
                    _lcdUtil.Write("Max Battery Power"),
                    _lcdUtil.Write("Current Capacity"),
                    _lcdUtil.Write("Max Capacity"),
                    _lcdUtil.Write("")
                };

                _batteryStatus = new BatteryStatus(_program);
                _hydrogenTankStatus = new HydrogenTankStatus(_program, ini.Data);
            }
            public void Run()
            {
                MessageEntity msgNew = new MessageEntity(_program, _senderName, _count.ToString());

                _gridCommunication.Send(msgNew);

                int index = 0;
                index = _lcdUtil.Replace(_lineLocation[0], "Timestamp: " + msgNew.TimeStamp.ToString());
                index = _lcdUtil.Replace(_lineLocation[1], "My Name: " + _senderName);
                index = _lcdUtil.Replace(_lineLocation[3], "Battery Status: " + _batteryStatus.CurrentStoredPower() + " MWh");
                index = _lcdUtil.Replace(_lineLocation[2], "Max Battery Power: " + _batteryStatus.MaxStoredPower + " MWh");
                index = _lcdUtil.Replace(_lineLocation[5], "Current Capacity: " + _hydrogenTankStatus.CheckCapacity() + " L");
                index = _lcdUtil.Replace(_lineLocation[4], "Max Capacity: " + _hydrogenTankStatus.MaxCapacity + " L");
                index = _lcdUtil.Replace(_lineLocation[6], "Count: " + msgNew.Message);
                _lcdUtil.Update();

                if(index < 0)
                {
                    throw new Exception($"ERROR: Could not replace LCD line. CODE:{index}");
                }

                _count++;
            }
        }
    }
}
