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
            public string ConifgSectionName { get; set; }
            public string LCDSectiongName { get; set; }
            public string Channel { get; set; }
            public string ChannelName { get; set; }
            public int MaxSenderOnLCD { get; set; }
            public string MaxSenderOnLCDName { get; set; }
            public bool Sender { get; set; }
            public string SenderName { get; set; }
            public string DebugLCD { get; set; }
            public string DebugLCDName { get; set; }
            public int TimeOutTime { get; set; }
            public string TimeOutTimeName { get; set; }

            private const string _defaultChannel = "channel-0";
            private const bool _defaultSender = false;
            private const string _defaultDebugLCD = "debug-0";
            private const int _defaultMaxLCDEntries = 3;
            private const string _defaultLCD = "LCD-0";
            private const int _defaultTimeout = 60;

            public List<KeyValuePair<string, string>> LcdOutputList { get; private set; }
            public List<MyIniKey> KeyList { get; private set; }

            private readonly Program _program;

            public CustomDataEntity(Program program, string conifgSectionName, string lcdSectionName)
            {
                _program = program;

                ConifgSectionName = conifgSectionName;
                LCDSectiongName = lcdSectionName;

                ChannelName = "Channel";
                SenderName = "Sender";
                DebugLCDName = "Debug LCD";
                MaxSenderOnLCDName = "Max sender on LCD";
                TimeOutTimeName = "Timeout in Seconds";

                LcdOutputList = new List<KeyValuePair<string, string>>();
                KeyList = new List<MyIniKey>();
            }

            public CustomDataEntity(Program program) : this(program, "Config", "LCDs") { }

            public void LoadData(MyIni ini)
            {
                Channel = ini.Get(ConifgSectionName, ChannelName).ToString(_defaultChannel);
                Sender = ini.Get(ConifgSectionName, SenderName).ToBoolean(_defaultSender);
                DebugLCD = ini.Get(ConifgSectionName, DebugLCDName).ToString(_defaultDebugLCD);
                MaxSenderOnLCD = ini.Get(ConifgSectionName, MaxSenderOnLCDName).ToInt32(_defaultMaxLCDEntries);
                TimeOutTime = ini.Get(ConifgSectionName, TimeOutTimeName).ToInt32(_defaultTimeout);

                LcdOutputList.Clear();
                KeyList.Clear();
                ini.GetKeys(LCDSectiongName, KeyList);

                _program.Echo($"Key list has entry: {KeyList.Any()}");
                if (KeyList.Any())
                {
                    foreach (MyIniKey key in KeyList)
                    {
                        _program.Echo($"Section: {key.Section} Name: {key.Name}");
                        LcdOutputList.Add(new KeyValuePair<string, string>(
                            key.Name,
                            ini.Get(LCDSectiongName, key.Name).ToString("DefaultValue")));
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
                ini.Set(ConifgSectionName, ChannelName, Channel);
                ini.Set(ConifgSectionName, SenderName, Sender);
                ini.Set(ConifgSectionName, DebugLCDName, DebugLCD);
                ini.Set(ConifgSectionName, MaxSenderOnLCDName, MaxSenderOnLCD);
                ini.Set(ConifgSectionName, TimeOutTimeName, TimeOutTime);

                foreach (KeyValuePair<string, string> lcdOutput in LcdOutputList)
                {
                    ini.Set(LCDSectiongName, lcdOutput.Key, lcdOutput.Value);
                }
            }

            public void SetDefault(MyIni ini)
            {
                ini.Set(ConifgSectionName, ChannelName, _defaultChannel);
                ini.Set(ConifgSectionName, SenderName, _defaultSender);
                ini.Set(ConifgSectionName, DebugLCDName, _defaultDebugLCD);
                ini.Set(ConifgSectionName, MaxSenderOnLCDName, _defaultMaxLCDEntries);
                ini.Set(LCDSectiongName, _defaultLCD, _defaultLCD);

            }
        }
    }
}


