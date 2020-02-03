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
                _lineLocation = _lcdUtil.ReserveLines(6);

                _batteryStatus = new BatteryStatus(_program);
                _hydrogenTankStatus = new HydrogenTankStatus(_program, ini.Data);
            }
            public void Run()
            {
                MessageEntity msgNew = new MessageEntity(
                    _program,
                    _senderName,
                    _batteryStatus.CurrentStoredPower(),
                    _batteryStatus.MaxStoredPower,
                    _hydrogenTankStatus.CheckCapacity(),
                    _hydrogenTankStatus.MaxCapacity);

                _gridCommunication.Send(msgNew);

                _lcdUtil.Write(_lineLocation[0], "Timestamp: " + msgNew.TimeStamp.ToString());
                _lcdUtil.Write(_lineLocation[1], "My Name: " + _senderName);
                _lcdUtil.Write(_lineLocation[3], "Battery Status: " + _batteryStatus.CurrentStoredPower() + " MWh");
                _lcdUtil.Write(_lineLocation[2], "Max Battery Power: " + _batteryStatus.MaxStoredPower + " MWh");
                _lcdUtil.Write(_lineLocation[5], "Current Capacity: " + _hydrogenTankStatus.CheckCapacity() + " L");
                _lcdUtil.Write(_lineLocation[4], "Max Capacity: " + _hydrogenTankStatus.MaxCapacity + " L");
                _lcdUtil.Update();

                _count++;
            }
        }
    }
}
