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
        public class LCDTextEntity : LCDEntity
        {
            public List<string> LcdLines { get; private set; }

            public LCDTextEntity(Program program) : base(program) 
            {
                LcdLines = new List<string>();
            }

            public int[] ReserveLines(int count)
            {
                int[] lineIndex = new int[count];
                string debugText = "";
                for (int i = 0; i < count; i++)
                {
                    LcdLines.Add("");
                    lineIndex[i] = LcdLines.Count() - 1;
                    debugText += $"{lineIndex[i]}, ";
                }

                _program.Echo($"Reserverd Lines: {debugText}");
                return lineIndex;
            }

            public void Remove(string line)
            {
                LcdLines.Remove(line);
            }

            public void RemoveAt(int index)
            {
                LcdLines.RemoveAt(index);
            }

            public int Write(string oldLine, string newLine)
            {
                int index = LcdLines.IndexOf(oldLine);
                Write(index, newLine);

                return index;
            }

            public void Write(int index, string newLine)
            {
                if (LcdLines.Count >= 0 && LcdLines.Count > index)
                {
                    LcdLines[index] = newLine;
                }
            }

            public void Update(string header)
            {
                string msg = $"{header}\n";

                foreach (string line in LcdLines)
                {
                    msg += $"{line}\n";
                }

                Echo(msg, false);
            }
        }
    }
}
