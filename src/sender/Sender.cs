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
            private const int ReservedLCDLines = 8;

            private readonly GridCommunication _gridCommunication;

            private readonly LCDText _lcdUtil;

            private readonly BatteryStatus _batteryStatus;
            private readonly HydrogenTankStatus _hydrogenTankStatus;

            private readonly string _senderName;
            private readonly int[] _lineLocation;

            private readonly Program _program;
            public Sender(Program program, CustomDataIni ini)
            {
                _program = program;

                _senderName = ini.Data.SenderName;

                _gridCommunication = new GridCommunication(_program, GridCommunication.ClientType.Sender, ini.Data.Channel);

                _lcdUtil = new LCDText(_program, $"-- Sender --\n");
                _lcdUtil.AddLCD(ini.Data.LcdOutputList);
                _lcdUtil.SetLCDContent(ContentType.TEXT_AND_IMAGE);
                _lcdUtil.SetDefaultFont(LCDText.FontColor.Green);
                _lineLocation = _lcdUtil.ReserveLCD(ReservedLCDLines).Item2;

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

                _lcdUtil.Write(_lineLocation[0], $"Timestamp: {msgNew.TimeStamp.ToString()}");
                _lcdUtil.Write(_lineLocation[1], $"My Name: {msgNew.SenderName}");
                _lcdUtil.Write(_lineLocation[3], $"Max Battery Power: {msgNew.MaxBatteryPower} MWh");
                _lcdUtil.Write(_lineLocation[4], $"Battery Status: {msgNew.CurrentBatteryPower} MWh");
                _lcdUtil.Write(_lineLocation[6], $"Max Capacity: {msgNew.MaxHydrogen} L");
                _lcdUtil.Write(_lineLocation[7], $"Current Capacity: {msgNew.CurrentHydrogen} L");
                _lcdUtil.Update();
            }
        }
    }
}
