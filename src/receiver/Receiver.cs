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
            private readonly int _timeoutTime;

            private int _checkConnection;

            private readonly Program _program;

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
                    _program.Echo($"Output LCD: {lcd.Key}, {lcd.Value}");
                    if (!_lcdUtil.ContainsKey(lcd.Value))
                    {
                        LCDUtil newLCD = new LCDUtil(_program, "");
                        newLCD.Add(lcd.Value);
                        newLCD.TextContentOn();
                        newLCD.SetFont(LCDUtil.FontColor.Green, 1f);
                        newLCD.Header = $"-- Receiver --\n";
                        newLCD.Update();

                        _lcdUtil.Add(lcd.Value, newLCD);
                    }
                }
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
                    sender.CurrentStatus = SenderEntity.Status.LostConnection;
                    sender.LCD.Replace(sender.LineNumber[2], "Status: Lost Connection");
                }
                else
                {
                    sender.CurrentStatus = SenderEntity.Status.Connected;
                    sender.LCD.Replace(sender.LineNumber[2], "Status: Connected");
                }
                sender.LCD.Update();
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
                    new int[] {
                                lcd.Value.Write("TimeStamp: " + msg.TimeStamp.ToString()),
                                lcd.Value.Write("Sender Name:" + msg.SenderName),
                                lcd.Value.Write("Status: Connecting ..."),
                                lcd.Value.Write("Message:"),
                                lcd.Value.Write(msg.Message)
                    }));
                    lcd.Value.Update();

                    _program.Echo($"Sender LCD Lines: " +
                        $" {_senderList.Last().LineNumber[0]}" +
                        $" {_senderList.Last().LineNumber[1]}" +
                        $" {_senderList.Last().LineNumber[2]}" +
                        $" {_senderList.Last().LineNumber[3]}");
                    _program.Echo($"Sender ID: {_senderList.Last().ID}");
                }
            }

            private void UpdateSender(MessageEntity msg, SenderEntity sender)
            {
                sender.LastUpdate = msg.TimeStamp;
                CheckConnection();

                sender.LCD.Replace(sender.LineNumber[0], "TimeStamp: " + msg.TimeStamp.ToString());
                sender.LCD.Replace(sender.LineNumber[1], "Sender Name: " + msg.SenderName);
                sender.LCD.Replace(sender.LineNumber[4], msg.Message);
                sender.LCD.Update();
            }
        }
    }
}
