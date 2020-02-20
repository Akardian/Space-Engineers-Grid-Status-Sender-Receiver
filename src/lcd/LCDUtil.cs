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
        public abstract class LCDUtil
        {
            public List<LCDEntity> LcdEntitys { get; private set; }

            protected readonly Program _program;
            private readonly List<IMyTerminalBlock> _searchLCD;
            private readonly List<IMyTextSurface> _searchSurface;

            public enum FontColor : byte { Green, Red, Blue }

            public LCDUtil (Program program)
            {
                LcdEntitys = new List<LCDEntity>();

                _program = program;
                _searchLCD = new List<IMyTerminalBlock>();
                _searchSurface = new List<IMyTextSurface>();
            }
            
            public void ClearLCD()
            {
                LcdEntitys.Clear();
            }

            public void AddLCD(List<KeyValuePair<string, string>> lcdNameList)
            {
                foreach(KeyValuePair<string, string> lcdName in lcdNameList)
                {
                    AddLCD(lcdName.Value);
                }
            }

            public abstract void AddSurfaceToEntity(List<IMyTextSurface> lcdList);

            public void AddLCD(string lcdName) 
            {
                _searchSurface.Clear();
                _searchLCD.Clear();
                _program.GridTerminalSystem.SearchBlocksOfName(lcdName, _searchLCD);
                foreach (IMyTerminalBlock lcd in _searchLCD)
                {
                    if (lcd != null && lcd.IsSameConstructAs(_program.Me) && lcd is IMyTextSurface)
                    {
                        _program.Echo($"New LCD: {lcdName}");

                        _searchSurface.Add((IMyTextSurface)lcd);
                    }
                    else if (lcd == null || !lcd.IsSameConstructAs(_program.Me))
                    {
                        _program.Echo($"ERROR: LCD [{lcdName}] not found");
                    }
                    else
                    {
                        _program.Echo($"ERROR: Block [{lcdName}] is not a TextSurface");
                    }
                }
                AddSurfaceToEntity(_searchSurface);
            }

            public void SetLCDContent(ContentType type)
            {
                foreach (LCDEntity lcd in LcdEntitys)
                {
                    foreach(SurfaceEntity surface in lcd.Surfaces)
                    {
                        surface.Surface.ContentType = type;
                    }
                }
            }

            public void SetDefaultFont(FontColor color)
            {
                switch(color)
                {
                    case FontColor.Blue:
                        SetFont(new Color(0, 0, 0, 255), new Color(0, 0, 255, 255), 1F);
                        break;
                    case FontColor.Green:
                        SetFont(new Color(0, 0, 0, 255), new Color(0, 255, 0, 255), 1F);
                        break;
                    case FontColor.Red:
                        SetFont(new Color(0, 0, 0, 255), new Color(255, 0, 0, 255), 1F);
                        break;
                }
            }   

            public void SetFont(Color backroundColor, Color fontColor, float fontSize)
            {
                foreach (LCDDrawEntity lcd in LcdEntitys)
                {
                    foreach (SurfaceEntity surface in lcd.Surfaces)
                    {
                        surface.Surface.BackgroundColor = backroundColor;
                        surface.Surface.FontColor = fontColor;
                        surface.Surface.FontSize = fontSize;
                    }
                }
            }
        }
    }
}
