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
            //General
            public string Channel { get; set; }
            public ClientTypes ClientType { get; set; }
            public string DebugLCD { get; set; }

            //Sender
            public string SenderName { get; set; }
            public float SmallTankCapacity { get; set; }
            public float LargeTankCapacity { get; set; }

            //Reciever
            public int MaxSenderOnLCD { get; set; }
            public int TimeOutTime { get; set; }

            //Lists
            public List<KeyValuePair<string, string>> LcdOutputList { get; private set; }
            public List<MyIniKey> KeyList { get; private set; }

            // Config Var Names
            private const string _channelName = "Channel Name";
            private const string _clientTypeName = "Client Type";
            private const string _debugLCDName = "Debug LCD";

            private const string _senderNameName = "Sender Name";
            private const string _smallTankCapacityName = "Small Tank Max Capacity";
            private const string _largeTankCapacityName = "Large Tank Max Capacity";

            private const string _maxSenderOnLCDName = "Max sender on LCD";
            private const string _timeOutTimeName = "Timeout in Seconds";

            //defaul values
            private const string _defaultChannel = "channel-0";
            private const ClientTypes _defaultClientType = ClientTypes.NotSet;
            private const string _defaultDebugLCD = "debug-0";

            private const string _defaultSenderName = "Unkown";
            private const float _defaultSmallTankCapacity = 160000;
            private const float _defaultLargeTankCapacity = 5000000;

            private const int _defaultMaxLCDEntries = 3;
            private const int _defaultTimeout = 60;

            private const string _defaultLCD = "LCD-0";

            // COnfig Section Names
            public const string _conifgSectionName = "General";
            public const string _senderConifgSectionName = "Sender";
            public const string _recieverConifgSectionName = "Receiver";
            public const string _lcdSectiongName = "LCDs";

            private readonly Program _program;

            public enum ClientTypes : byte { Sender = 0, Reciever = 1, NotSet = 2}

            public CustomDataEntity(Program program)
            {
                _program = program;

                LcdOutputList = new List<KeyValuePair<string, string>>();
                KeyList = new List<MyIniKey>();
            }

            public void LoadData(MyIni ini)
            {
                Channel = ini.Get(_conifgSectionName, _channelName).ToString(_defaultChannel);
                ClientType = (ClientTypes)System.Enum.Parse(typeof(ClientTypes), ini.Get(_conifgSectionName, _clientTypeName).ToString(_defaultClientType.ToString()));
                DebugLCD = ini.Get(_conifgSectionName, _debugLCDName).ToString(_defaultDebugLCD);

                SenderName = ini.Get(_senderConifgSectionName, _senderNameName).ToString(_defaultSenderName);
                SmallTankCapacity = ini.Get(_senderConifgSectionName, _smallTankCapacityName).ToSingle(_defaultSmallTankCapacity);
                LargeTankCapacity = ini.Get(_senderConifgSectionName, _largeTankCapacityName).ToSingle(_defaultLargeTankCapacity);

                MaxSenderOnLCD = ini.Get(_recieverConifgSectionName, _maxSenderOnLCDName).ToInt32(_defaultMaxLCDEntries);
                TimeOutTime = ini.Get(_recieverConifgSectionName, _timeOutTimeName).ToInt32(_defaultTimeout);

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
                ini.Set(_conifgSectionName, _channelName, Channel);
                ini.Set(_conifgSectionName, _clientTypeName, ClientType.ToString());
                ini.Set(_conifgSectionName, _debugLCDName, DebugLCD);

                ini.Set(_senderConifgSectionName, _senderNameName, SenderName);
                ini.Set(_senderConifgSectionName, _smallTankCapacityName, SmallTankCapacity);
                ini.Set(_senderConifgSectionName, _largeTankCapacityName, LargeTankCapacity);

                ini.Set(_recieverConifgSectionName, _maxSenderOnLCDName, MaxSenderOnLCD);
                ini.Set(_recieverConifgSectionName, _timeOutTimeName, TimeOutTime);

                foreach (KeyValuePair<string, string> lcdOutput in LcdOutputList)
                {
                    ini.Set(_lcdSectiongName, lcdOutput.Key, lcdOutput.Value);
                }
            }

            public void SetDefault(MyIni ini)
            {
                ini.Set(_conifgSectionName, _channelName, _defaultChannel);
                ini.Set(_conifgSectionName, _clientTypeName, _defaultClientType.ToString());
                ini.Set(_conifgSectionName, _debugLCDName, _defaultDebugLCD);

                ini.Set(_senderConifgSectionName, _senderNameName, _defaultSenderName);
                ini.Set(_senderConifgSectionName, _smallTankCapacityName, _defaultSmallTankCapacity);
                ini.Set(_senderConifgSectionName, _largeTankCapacityName, _defaultLargeTankCapacity);

                ini.Set(_recieverConifgSectionName, _maxSenderOnLCDName, _defaultMaxLCDEntries);
                ini.Set(_recieverConifgSectionName, _timeOutTimeName, _defaultTimeout);

                ini.Set(_lcdSectiongName, _defaultLCD, _defaultLCD);

            }
        }
    }
}


