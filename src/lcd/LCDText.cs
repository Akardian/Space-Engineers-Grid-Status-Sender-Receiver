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
        public class LCDText : LCDUtil
        {
            public string Header { get; set; }
            public int MaxLines { get; private set; }

            public LCDText(Program program) : this(program, "") { }
            public LCDText(Program program, string header) : this(program, header, 17) { }
            public LCDText(Program program, string header, int maxLines) : base(program)
            {
                Header = header;
                MaxLines = maxLines;
            }

            public void Update()
            {
                foreach (LCDEntity entity in LcdEntitys)
                {
                    entity.Update(Header);
                }
   
            }

            public MyTuple<LCDEntity, int[]> ReserveLCD(int lineCount)
            {
                LCDEntity selectedLCD = null;
                foreach (LCDEntity entity in LcdEntitys)
                {
                    if(MaxLines - entity.LcdLines.Count >= lineCount)
                    {
                        selectedLCD = entity;
                        break;
                    }
                }

                return new MyTuple<LCDEntity, int[]>(selectedLCD, selectedLCD.ReserveLines(lineCount));
            }

            public void Write(string oldLine, string newLine)
            {
                foreach (LCDEntity entity in LcdEntitys)
                {
                    entity.Write(oldLine, newLine);
                }
            }

            public void Write(int index, string newLine)
            {
                foreach (LCDEntity entity in LcdEntitys)
                {
                    entity.Write(index, newLine);
                }
            }

            public void Echo(string msg)
            {
                Echo(msg, true);
            }

            public void Echo(string msg, bool append)
            {
                foreach(LCDEntity entity in LcdEntitys)
                {
                    entity.Echo(msg, append);
                }
            }
        }
    }
} 
