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
        public class CustomDataEntity
        {
            public string Channel { get; set; }
            public string ChannelName { get; set; }
            public int MaxSenderOnLCD { get; set; }
            public string MaxSenderOnLCDName { get; set; }
            public ClientTypes ClientType { get; set; }
            public string ClientTypeName { get; set; }
            public string SenderName { get; set; }
            public string SenderNameName { get; set; }
            public string DebugLCD { get; set; }
            public string DebugLCDName { get; set; }
            public int TimeOutTime { get; set; }
            public string TimeOutTimeName { get; set; }

            private const string _defaultChannel = "channel-0";
            private const ClientTypes _defaultClientType = ClientTypes.NotSet;
            private const string _defaultDebugLCD = "debug-0";

            private const string _defaultSenderName = "Unkown";

            private const int _defaultMaxLCDEntries = 3;
            private const int _defaultTimeout = 60;

            private const string _defaultLCD = "LCD-0";

            public readonly string _conifgSectionName;
            public readonly string _senderConifgSectionName;
            public readonly string _recieverConifgSectionName;
            public readonly string _lcdSectiongName;

            public List<KeyValuePair<string, string>> LcdOutputList { get; private set; }
            public List<MyIniKey> KeyList { get; private set; }

            private readonly Program _program;

            public enum ClientTypes : byte { Sender = 0, Reciever = 1, NotSet = 2}

            public CustomDataEntity(Program program)
            {
                _program = program;

                _conifgSectionName = "General";
                _senderConifgSectionName = "Sender";
                _recieverConifgSectionName = "Receiver";
                _lcdSectiongName = "LCDs";

                ChannelName = "Client Type";
                ClientTypeName = "Sender";
                DebugLCDName = "Debug LCD";

                SenderNameName = "MyName";

                MaxSenderOnLCDName = "Max sender on LCD";
                TimeOutTimeName = "Timeout in Seconds";

                LcdOutputList = new List<KeyValuePair<string, string>>();
                KeyList = new List<MyIniKey>();
            }

            public void LoadData(MyIni ini)
            {
                Channel = ini.Get(_conifgSectionName, ChannelName).ToString(_defaultChannel);
                ClientType = (ClientTypes)System.Enum.Parse(typeof(ClientTypes), ini.Get(_conifgSectionName, ClientTypeName).ToString(_defaultClientType.ToString()));
                DebugLCD = ini.Get(_conifgSectionName, DebugLCDName).ToString(_defaultDebugLCD);

                SenderName = ini.Get(_senderConifgSectionName, SenderNameName).ToString(_defaultSenderName);

                MaxSenderOnLCD = ini.Get(_recieverConifgSectionName, MaxSenderOnLCDName).ToInt32(_defaultMaxLCDEntries);
                TimeOutTime = ini.Get(_recieverConifgSectionName, TimeOutTimeName).ToInt32(_defaultTimeout);

                LcdOutputList.Clear();
                KeyList.Clear();
                ini.GetKeys(_lcdSectiongName, KeyList);

                _program.Echo($"Key list has entry: {KeyList.Any()}");
                if (KeyList.Any())
                {
                    foreach (MyIniKey key in KeyList)
                    {
                        _program.Echo($"Section: {key.Section} Name: {key.Name}");
                        LcdOutputList.Add(new KeyValuePair<string, string>(
                            key.Name,
                            ini.Get(_lcdSectiongName, key.Name).ToString("DefaultValue")));
                    }
                } else
                {
                    _program.Echo("No Data to Load");
                    LcdOutputList.Add(new KeyValuePair<string, string>(
                        "LCD-0","Default-LCD"));
                }

                
            }

            public void SaveData(MyIni ini)
            {
                ini.Set(_conifgSectionName, ChannelName, Channel);
                ini.Set(_conifgSectionName, ClientTypeName, ClientType.ToString());
                ini.Set(_conifgSectionName, DebugLCDName, DebugLCD);

                ini.Set(_senderConifgSectionName, SenderNameName, SenderName);

                ini.Set(_recieverConifgSectionName, MaxSenderOnLCDName, MaxSenderOnLCD);
                ini.Set(_recieverConifgSectionName, TimeOutTimeName, TimeOutTime);

                foreach (KeyValuePair<string, string> lcdOutput in LcdOutputList)
                {
                    ini.Set(_lcdSectiongName, lcdOutput.Key, lcdOutput.Value);
                }
            }

            public void SetDefault(MyIni ini)
            {
                ini.Set(_conifgSectionName, ChannelName, _defaultChannel);
                ini.Set(_conifgSectionName, ClientTypeName, _defaultClientType.ToString());
                ini.Set(_conifgSectionName, DebugLCDName, _defaultDebugLCD);

                ini.Set(_senderConifgSectionName, SenderNameName, _defaultSenderName);

                ini.Set(_recieverConifgSectionName, MaxSenderOnLCDName, _defaultMaxLCDEntries);
                ini.Set(_recieverConifgSectionName, TimeOutTimeName, _defaultTimeout);

                ini.Set(_lcdSectiongName, _defaultLCD, _defaultLCD);

            }
        }
    }
}


