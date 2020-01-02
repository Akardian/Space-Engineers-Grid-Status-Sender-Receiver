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
        public class Sender
        {
            private GridCommunication _gridCommunication;
            private LCDUtil _lcdUtil;

            private int _count;
            private int[] _lineLocation;

            private readonly Program _program;
            public Sender(Program program, CustomDataIni ini)
            {
                _program = program;

                _count = 0;

                _lcdUtil = new LCDUtil(_program);
                _lcdUtil.Add(ini.Data.LcdOutputList);
                _lcdUtil.TextContentOn();
                _lcdUtil.SetFont(LCDUtil.FontColor.Green, 1f);
                _lcdUtil.Header = $"-- Sender --\n";

                _gridCommunication = new GridCommunication(_program, GridCommunication.ClientType.Sender, ini.Data.Channel);

                _lineLocation = new int[] {
                    _lcdUtil.Write("TimeStamp"),
                    _lcdUtil.Write("SenderID"),
                    _lcdUtil.Write("Message:"),
                   _lcdUtil.Write("")
                };
                
            }
            public void Run()
            {
                MessageEntity msgNew = new MessageEntity(_program, _count.ToString());

                _gridCommunication.Send(msgNew);

                int index = 0;
                index = _lcdUtil.Replace(_lineLocation[0], msgNew.TimeStamp.ToString());
                index = _lcdUtil.Replace(_lineLocation[1], msgNew.SenderID.ToString());
                index = _lcdUtil.Replace(_lineLocation[3], msgNew.Message);
                _lcdUtil.Update();

                if(index < 0)
                {
                    throw new Exception($"ERROR: Could not replace LCD line. CODE:{index}");
                }

                _count++;
            }
        }
    }
}
