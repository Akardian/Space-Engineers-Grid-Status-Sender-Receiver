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
        public class Receiver
        {
            private readonly LCDText _lcd;
            private readonly List<SenderEntity> _senderList;

            private readonly GridCommunication _gridCommunication;
            private readonly int _timeoutTime;

            private int _checkConnection;

            private readonly Program _program;

            private const int ReservedLCDLines = 5;
            private const int LINE_TIMESTAMP = 1;
            private const int LINE_SENDER_NAME = 2;
            private const int LINE_CONNECTION = 3;
            private const int LINE_STATUS = 4;

            public Receiver(Program program, CustomDataIni ini)
            {
                _program = program;
                int maxLines = ini.Data.MaxSenderOnLCD * ReservedLCDLines;

                _lcd = new LCDText(program, $"-- Receiver --", maxLines);
                _lcd.AddLCD(ini.Data.LcdOutputList);
                _lcd.TextContentOn();
                _lcd.SetDefaultFont(LCDText.FontColor.Green);
                _lcd.Update();

                _gridCommunication = new GridCommunication(program, GridCommunication.ClientType.Reciever, ini.Data.Channel);
                _senderList = new List<SenderEntity>();

                _timeoutTime = ini.Data.TimeOutTime;
                _program.Echo($"TimeoutTime: {_timeoutTime}, MaxLinesOnLCD: {maxLines}");
            }

            public void Run()
            {
                MessageEntity msg = _gridCommunication.Receive();
                while (msg != null)
                {
                    SenderEntity senderEntity = (
                        from sender in _senderList.AsEnumerable()
                        where sender.ID == msg.SenderID
                        select sender).SingleOrDefault();

                    if (senderEntity == null)
                    {
                        AddSender(msg);                        
                    } else
                    {
                        UpdateSender(msg, senderEntity);
                    }

                    msg = _gridCommunication.Receive();
                }

                if (_senderList.Any())
                {
                    CheckConnection();
                }
            }

            private void CheckConnection()
            {
                if(_senderList.Count <= _checkConnection)
                {
                    _checkConnection = 0;
                }
                
                SenderEntity sender = _senderList[_checkConnection];
                Double time = Math.Abs((DateTime.Now - sender.LastUpdate).TotalSeconds);
                if (time > _timeoutTime)
                {
                    sender.CurrentStatus = SenderEntity.Status.Disconnected;
                    sender.LCD.Write(sender.LineNumber[LINE_CONNECTION], $"Status: {sender.CurrentStatus}");
                    sender.LCD.Update(_lcd.Header);
                }
                else
                {
                    sender.CurrentStatus = SenderEntity.Status.Connected;
                }

                _checkConnection++;
            }

            private void AddSender(MessageEntity msg)
            {
                _program.Echo($"Add Sender: {msg.SenderID} ");
                MyTuple<LCDEntity, int[]> lcdTuple = _lcd.ReserveLCD(ReservedLCDLines);
                
                if (lcdTuple.Item1 != null)
                {
                    _senderList.Add(new SenderEntity(
                    msg.SenderID,
                    msg.TimeStamp,
                    lcdTuple.Item1,
                    SenderEntity.Status.Connected,
                    lcdTuple.Item2));

                    lcdTuple.Item1.Update(_lcd.Header);
                } else
                {
                    _program.Echo($"ERROR: Could not reserve LCD");
                }
            }

            private void UpdateSender(MessageEntity msg, SenderEntity sender)
            {
                sender.LastUpdate = msg.TimeStamp;
                sender.CurrentStatus = SenderEntity.Status.Connected;

                sender.LCD.Write(sender.LineNumber[LINE_TIMESTAMP], $"TimeStamp: {msg.TimeStamp.ToString()}");
                sender.LCD.Write(sender.LineNumber[LINE_SENDER_NAME], $"Sender Name: {msg.SenderName}");
                sender.LCD.Write(sender.LineNumber[LINE_CONNECTION], $"Status: {sender.CurrentStatus}");
                sender.LCD.Write(sender.LineNumber[LINE_STATUS], $"B: {msg.CurrentBatteryPower} H: {msg.CurrentHydrogen}");
                sender.LCD.Update(_lcd.Header);
            }
        }
    }
}
