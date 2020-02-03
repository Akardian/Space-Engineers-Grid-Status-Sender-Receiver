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
        public class LCDUtil
        {
            public string Header { get; set; }
            public string ID { get; private set; }
            public int SenderCount { get; set; }

            private readonly List<IMyTextPanel> _lcdList;
            private readonly List<string> _lcdLines;
            private readonly List<IMyTerminalBlock> _searchLCD;

            private readonly Program _program;
            public enum FontColor : byte { Green, Red, Blue }

            public LCDUtil (Program program, string header)
            {
                _program = program;
                _lcdList = new List<IMyTextPanel>();
                _lcdLines = new List<string>();
                _searchLCD = new List<IMyTerminalBlock>();

                Header = header;
                SenderCount = 0;
            }
            public LCDUtil(Program program) : this(program, "") { }

            public void Set(string lcdName)
            {
                _lcdList.Clear();
                Add(lcdName);
            }

            public void Add(List<KeyValuePair<string, string>> lcdNameList)
            {
                foreach(KeyValuePair<string, string> lcdName in lcdNameList)
                {
                    _program.Echo($"LCDUtil: Add output LCD:{lcdName.Value}");
                    Add(lcdName.Value);
                }
            }

            public void Add(string lcdName) 
            {
                _searchLCD.Clear();
                _program.GridTerminalSystem.SearchBlocksOfName(lcdName, _searchLCD);
                foreach (IMyTerminalBlock lcd in _searchLCD)
                {
                    if (lcd != null && lcd.IsSameConstructAs(_program.Me))
                    {
                        
                        _lcdList.Add(_program.GridTerminalSystem.GetBlockWithId(lcd.EntityId) as IMyTextPanel);
                    }
                    else
                    {
                        _program.Echo($"LCDUtil: LCD Not found: {lcdName}");
                    }
                }
            }

            public void TextContentOn()
            {
                foreach (IMyTextPanel lcd in _lcdList)
                {
                    lcd.ContentType = ContentType.TEXT_AND_IMAGE;
                }
            }

            public void SetFont(FontColor color, float fontSize)
            {
                switch(color)
                {
                    case FontColor.Blue:
                        SetFont(new Color(0, 0, 0, 255), new Color(0, 0, 255, 255), fontSize);
                        break;
                    case FontColor.Green:
                        SetFont(new Color(0, 0, 0, 255), new Color(0, 255, 0, 255), fontSize);
                        break;
                    case FontColor.Red:
                        SetFont(new Color(0, 0, 0, 255), new Color(255, 0, 0, 255), fontSize);
                        break;
                }
            }   

            public void SetFont(Color backroundColor, Color fontColor, float fontSize)
            {
                foreach (IMyTextPanel lcd in _lcdList)
                {
                    lcd.BackgroundColor = backroundColor;
                    lcd.FontColor = fontColor;
                    lcd.FontSize = fontSize;
                }
            }

            public int[] ReserveLines(int count)
            {
                int[] lineIndex = new int[count];
               for(int i = 0; i < count; i++)
               {
                    _lcdLines.Add("");
                    lineIndex[i] = _lcdLines.Count() - 1;
               }

                return lineIndex;
            }

            public void Remove(string line)
            {
                _lcdLines.Remove(line);
            }

            public void RemoveAt(int index)
            {
                _lcdLines.RemoveAt(index);
            }

            public int Write(string oldLine, string newLine)
            {
                int index = _lcdLines.IndexOf(oldLine);
                index = Write(index, newLine);

                return index;
            }

            public int Write(int index, string newLine)
            {
                if(_lcdLines.Count >= 0 && _lcdLines.Count > index)
                {
                    _lcdLines[index] = newLine;
                }
                else
                {
                    _lcdLines.Add(newLine);
                    index = _lcdLines.Count() - 1;
                }
                return index;
            }

            public void Update()
            {
                string msg = Header + "\n";

                foreach(string line in _lcdLines)
                {
                    msg += line + "\n";
                }

                Echo(msg, false);
            }
             
            public void Echo(string msg)
            {
                Echo(msg, true);
            }

            public void Echo(string msg, bool append)
            {
                foreach(IMyTextPanel lcd in _lcdList)
                {
                    lcd?.WriteText($"{msg}\n", append);
                }
            }
        }
    }
}
