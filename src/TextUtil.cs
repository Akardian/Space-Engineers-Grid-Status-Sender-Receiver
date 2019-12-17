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
        public class TextUtil
        {
            private List<IMyTextPanel> _lcdList;
            private string _header;
            public string Header
            {
                get { return _header; }
                set { _header = value; WriteHeader(); }
            }

            private readonly Program _program;
            public enum FontColor : byte { Green, Red, Blue }

            public TextUtil (Program program, string header)
            {
                _program = program;
                _lcdList = new List<IMyTextPanel>();
                _header = header;
                WriteHeader();
            }
            public TextUtil(Program program) : this(program, "") { }

            public void SetLCD(string lcdName)
            {
                _lcdList.Clear();
                AddLCD(lcdName);
            }

            public void AddLCD(string lcdName) 
            {
                IMyTextPanel lcd = _program.GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextPanel;
                if(lcd != null)
                {
                    _program.Echo(lcd.CustomName);
                    _lcdList.Add(lcd);
                } else
                {
                    _program.Echo("No LCD found");
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

            private void WriteHeader()
            {
                if(_header.Length > 0)
                {
                    _program.Echo(_header);
                    foreach (IMyTextPanel lcd in _lcdList)
                    {
                        lcd?.WriteText($"{_header}\n\n", false);
                    }
                }                
            }
             
            public void Echo(string msg)
            {
                Echo(msg, true);
            }
            public void Echo(string msg, bool append)
            {
                if(append == false)
                {
                    WriteHeader();
                }

                foreach(IMyTextPanel lcd in _lcdList)
                {
                    lcd?.WriteText($"{msg}\n", append);
                }
            }
        }
    }
}
