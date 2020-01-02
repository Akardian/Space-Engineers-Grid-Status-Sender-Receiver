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
            private Dictionary<string,LCDUtil> _lcdUtil;
            private List<SenderEntity> _senderList;

            private GridCommunication _gridCommunication;
            private int MaxSenderOnLCD;

            private Program _program;

            public Receiver(Program program, CustomDataIni ini)
            {
                _program = program;
                _lcdUtil = new Dictionary<string, LCDUtil>();

                _gridCommunication = new GridCommunication(program, GridCommunication.ClientType.Reciever, ini.Data.Channel);
                _senderList = new List<SenderEntity>();

                MaxSenderOnLCD = ini.Data.MaxSenderOnLCD;

                foreach (KeyValuePair<string, string> lcd in ini.Data.LcdOutputList)
                {
                    _program.Echo($"Out LCD: {lcd.Key}, {lcd.Value}");
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
                        KeyValuePair<string, LCDUtil> lcd = new KeyValuePair<string, LCDUtil>("-1", null);
                        for (int i = 0 ; lcd.Value == null && _lcdUtil.Count > i; i++)
                        {
                            KeyValuePair<string, LCDUtil> selectedLcd = _lcdUtil.ElementAt(i);
                            _program.Echo($"Sender Count: {selectedLcd.Value.SenderCount} Max: {MaxSenderOnLCD}");
                            if (selectedLcd.Value.SenderCount < MaxSenderOnLCD)
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
                            lcd.Key,
                            new int[] {
                                lcd.Value.Write("TimeStamp"),
                                lcd.Value.Write("SenderID"),
                                lcd.Value.Write("Message:"),
                                lcd.Value.Write("")
                            }));
                            _program.Echo($"Sender LCD Lines: " +
                                $" {_senderList.Last().LineNumber[0]}" +
                                $" {_senderList.Last().LineNumber[1]}" +
                                $" {_senderList.Last().LineNumber[3]}");
                            _program.Echo($"Sender ID: {_senderList.Last().ID}");

                            lcd.Value.Update();
                        }
                            
                    } else
                    {
                        foreach (KeyValuePair<string, LCDUtil> lcd in _lcdUtil)
                        {
                            if (lcd.Value != null && lcd.Key.Equals(senderEntity.LCD))
                            {
                                lcd.Value.Replace(senderEntity.LineNumber[0], msg.TimeStamp.ToString());
                                lcd.Value.Replace(senderEntity.LineNumber[1], msg.SenderID.ToString());
                                lcd.Value.Replace(senderEntity.LineNumber[3], msg.Message);
                                lcd.Value.Update();
                            }
                        }
                    }

                    msg = _gridCommunication.Receive();
                }
            }
        }
    }
}
