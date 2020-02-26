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
        public class LCDDraw : LCDUtil
        {
            public new List<LCDDrawEntity> LcdEntitys { get; private set; }
            public int MaxSpace { get; private set; }

            public LCDDraw(Program program, int maxSpace) : base(program) 
            {
                LcdEntitys = new List<LCDDrawEntity>();

                MaxSpace = maxSpace;
            }
            public override void AddSurfaceToEntity(List<IMyTextSurface> lcdList)
            {
                LCDDrawEntity newEntity = new LCDDrawEntity(_program);
                foreach (IMyTextSurface lcd in lcdList)
                {
                    newEntity.Add(lcd);
                }
                LcdEntitys.Add(newEntity);
            }

            public void Script() { Script(""); }
            public void Script(string script)
            {
                foreach (LCDDrawEntity lcd in LcdEntitys)
                    foreach (SurfaceEntity surface in lcd.Surfaces)
                    {
                        surface.Surface.Script = script;
                    }
            }

            public void ScriptBackgroundColor(Color color)
            {
                foreach (LCDDrawEntity lcd in LcdEntitys)
                    foreach (SurfaceEntity surface in lcd.Surfaces)
                    {
                        surface.Surface.ScriptBackgroundColor = color;
                    }
            }

            public void ScriptForegroundColor(Color color)
            {
                foreach (LCDDrawEntity lcd in LcdEntitys)
                    foreach (SurfaceEntity surface in lcd.Surfaces)
                    {
                        surface.Surface.ScriptForegroundColor = color;
                    }
            }

            public void ScriptColor(Color backgroundColor, Color foregroundColor)
            {
                foreach (LCDDrawEntity lcd in LcdEntitys)
                    foreach (SurfaceEntity surface in lcd.Surfaces)
                    {
                        surface.Surface.ScriptBackgroundColor = backgroundColor;
                        surface.Surface.ScriptForegroundColor = foregroundColor;
                    }
            }

            public MyTuple<LCDDrawEntity, int[]> ReserveLCD(float senderID)
            {
                int[] count = new int[] { -1 };
                MyTuple<LCDDrawEntity, int[]> selectedLCD = new MyTuple<LCDDrawEntity, int[]>(null, count);
                foreach (LCDDrawEntity entity in LcdEntitys)
                {
                    if (MaxSpace > entity.Count)
                    {
                        count[0] = entity.ReserveSpace(senderID);

                        selectedLCD = new MyTuple<LCDDrawEntity, int[]>(entity, count);
                        break;
                    }
                }
                return selectedLCD;
            }

            public void UpdateDraw()
            {
                foreach (LCDDrawEntity lcd in LcdEntitys)
                {
                    lcd.Draw();
                }
            }
        }
    }
} 
