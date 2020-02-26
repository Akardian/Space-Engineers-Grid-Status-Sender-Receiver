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
            public new List<LCDTextEntity> LcdEntitys { get; private set; }
            public string Header { get; set; }
            public int MaxLines { get; private set; }

            public LCDText(Program program) : this(program, "") { }
            public LCDText(Program program, string header) : this(program, header, 17) { }
            public LCDText(Program program, string header, int maxLines) : base(program)
            {
                Header = header;
                MaxLines = maxLines;

                LcdEntitys = new List<LCDTextEntity>();
            }

            public override void AddSurfaceToEntity(List<IMyTextSurface> lcdList)
            {
                LCDTextEntity newEntity = new LCDTextEntity(_program);
                foreach (IMyTextSurface lcd in lcdList)
                {
                    newEntity.Add(lcd);
                }
                LcdEntitys.Add(newEntity);
            }

            public void Update()
            {
                foreach (LCDTextEntity entity in LcdEntitys)
                {
                    entity.Update(Header);
                }
            }

            public MyTuple<LCDTextEntity, int[]> ReserveLCD(int lineCount)
            {
                LCDTextEntity selectedLCD = null;
                foreach (LCDTextEntity entity in LcdEntitys)
                {
                    if(MaxLines - entity.LcdLines.Count >= lineCount)
                    {
                        selectedLCD = entity;
                        break;
                    }
                }

                return new MyTuple<LCDTextEntity, int[]>(selectedLCD, selectedLCD.ReserveSpace(lineCount));
            }

            public void Write(string oldLine, string newLine)
            {
                foreach (LCDTextEntity entity in LcdEntitys)
                {
                    entity.Write(oldLine, newLine);
                }
            }

            public void Write(int index, string newLine)
            {
                foreach (LCDTextEntity entity in LcdEntitys)
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
                foreach(LCDTextEntity entity in LcdEntitys)
                {
                    entity.Echo(msg, append);
                }
            }
        }
    }
} 
