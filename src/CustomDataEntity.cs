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

            public bool Sender { get; set; }
            public string SenderName { get; set; }

            public string DebugLCD { get; set; }
            public string DebugLCDName { get; set; }

            public List<KeyValuePair<string, string>> LcdOutputList { get; set; }

            private Program _program;

            public CustomDataEntity(Program program, string conifgSectionName, string lcdSectionName, string channel, bool sender, string debugLCD)
            {
                _program = program;

                ConifgSectionName = conifgSectionName;
                LCDSectiongName = lcdSectionName;

                Channel = channel;
                ChannelName = "Channel";

                Sender = sender;
                SenderName = "Sender";

                DebugLCD = debugLCD;
                DebugLCDName = "Debug LCD";

                LcdOutputList = new List<KeyValuePair<string, string>>();
            }

            public CustomDataEntity(Program program) : this(program, "Config", "LCDs", "channel-0", false, "Debug-0") { }

            public void LoadData(MyIni ini)
            {
                Channel = ini.Get(ConifgSectionName, ChannelName).ToString("channel-0");
                Sender = ini.Get(ConifgSectionName, SenderName).ToBoolean(false);
                DebugLCD = ini.Get(ConifgSectionName, DebugLCDName).ToString("Debug-0");

                LcdOutputList.Clear();
                List<MyIniKey> keyList = new List<MyIniKey>();
                ini.GetKeys(LCDSectiongName, keyList);
               
                _program.Echo($"Key List: {keyList.Any()}");

                if (keyList.Any())
                {   
                    foreach (MyIniKey key in keyList)
                    {
                        try
                        {
                            LcdOutputList.Add(new KeyValuePair<string, string>(
                                key.Name,
                                ini.Get(LCDSectiongName, key.Name).ToString("DefaultValue")));
                        } catch (Exception e)
                        {
                            _program.Echo($"ERROR: Could not load LCD Name: {e}");
                        }
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

                foreach (KeyValuePair<string, string> lcdOutput in LcdOutputList)
                {
                    ini.Set(LCDSectiongName, lcdOutput.Key, lcdOutput.Value);
                }
            }
        }
    }
}


