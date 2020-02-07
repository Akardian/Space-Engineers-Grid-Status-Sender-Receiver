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
            private readonly Dictionary<string,LCDUtil> _lcdUtil;
            private readonly List<SenderEntity> _senderList;

            private readonly GridCommunication _gridCommunication;
            private readonly int _maxSenderOnLCD;
            private readonly int _maxSender;
            private readonly int _timeoutTime;

            private int _checkConnection;
            private bool _maxSenderReached;

            private readonly Program _program;

            private const int ReservedLCDLines = 5;
            private const int LINE_TIMESTAMP = 1;
            private const int LINE_SENDER_NAME = 2;
            private const int LINE_CONNECTION = 3;
            private const int LINE_STATUS = 4;

            public Receiver(Program program, CustomDataIni ini)
            {
                _program = program;
                _lcdUtil = new Dictionary<string, LCDUtil>();

                _gridCommunication = new GridCommunication(program, GridCommunication.ClientType.Reciever, ini.Data.Channel);
                _senderList = new List<SenderEntity>();

                _maxSenderOnLCD = ini.Data.MaxSenderOnLCD;
                _timeoutTime = ini.Data.TimeOutTime;
                _program.Echo($"TimeoutTime: {_timeoutTime}");

                foreach (KeyValuePair<string, string> lcd in ini.Data.LcdOutputList)
                {
                    if (!_lcdUtil.ContainsKey(lcd.Value))
                    {
                        LCDUtil newLCD = new LCDUtil(_program, "");
                        newLCD.Add(lcd.Value);
                        newLCD.TextContentOn();
                        newLCD.SetDefaultFont(LCDUtil.FontColor.Green, 1f);
                        newLCD.Header = $"-- Receiver --";
                        newLCD.Update();

                        _lcdUtil.Add(lcd.Value, newLCD);
                    }
                }

                _maxSender = _maxSenderOnLCD * _lcdUtil.Count;
                _maxSenderReached = false;
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
                        if(_senderList.Count < _maxSender)
                        {
                            AddSender(msg);
                        } else if (!_maxSenderReached)
                        {
                            _program.Echo($"ERROR: Max Sender space reached {_senderList.Count}");
                            _maxSenderReached = true;
                            return;
                        }
                        
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
                    sender.LCD.Update();
                }
                else
                {
                    sender.CurrentStatus = SenderEntity.Status.Connected;
                }

                _checkConnection++;
            }

            private void AddSender(MessageEntity msg)
            {
                KeyValuePair<string, LCDUtil> lcd = new KeyValuePair<string, LCDUtil>("-1", null);
                for (int i = 0; lcd.Value == null && _lcdUtil.Count > i; i++)
                {
                    KeyValuePair<string, LCDUtil> selectedLcd = _lcdUtil.ElementAt(i);
                    _program.Echo($"Sender Count: {selectedLcd.Value.SenderCount} Max: {_maxSenderOnLCD}");
                    if (selectedLcd.Value.SenderCount < _maxSenderOnLCD)
                    {
                        lcd = selectedLcd;
                        _program.Echo($"Select LCD: {lcd.Key}");
                        selectedLcd.Value.SenderCount++;
                    }
                }

                if (lcd.Value != null)
                {
                    _senderList.Add(new SenderEntity(
                    msg.SenderID,
                    msg.TimeStamp,
                    lcd.Value,
                    SenderEntity.Status.Connected,
                    lcd.Value.ReserveLines(ReservedLCDLines)));

                    lcd.Value.Update();
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
                sender.LCD.Update();
            }
        }
    }
}
