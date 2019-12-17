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
    partial class Program : MyGridProgram
    {
        private const UpdateType COMMAND_UPDATE = UpdateType.Trigger | UpdateType.Terminal;

        private TextUtil _textUtil;

        private GridCommunication _gridCommunication;
        
        private bool _sender = true;
        private long _count = 0;

        public Program()
        {
            try
            {
                //Echo = text => {};
                _textUtil = new TextUtil(this);
                if (_sender)
                {
                    _textUtil.AddLCD("LCD-1");
                    _textUtil.AddLCD("LCD-2");
                    _textUtil.TextContentOn();
                    _textUtil.SetFont(TextUtil.FontColor.Green, 1f);

                    _gridCommunication = new GridCommunication(this, GridCommunication.ClientType.Sender, "channel-1");
                    _gridCommunication.SetAntennaBLock("Antenna S");

                    _textUtil.Header = $"LCD Header Sender\n";
                } else
                {
                    _textUtil.AddLCD("LCD-3");
                    _textUtil.AddLCD("LCD-4");
                    _textUtil.TextContentOn();
                    _textUtil.SetFont(TextUtil.FontColor.Green, 1f);

                    _gridCommunication = new GridCommunication(this, GridCommunication.ClientType.Reciever, "channel-1");
                    _gridCommunication.SetAntennaBLock("Antenna R");

                    _textUtil.Header = $"LCD Header Receiver\n";
                }

                Runtime.UpdateFrequency = UpdateFrequency.Update100;
            }
            catch (Exception e)
            {
                Echo($"Exception: {e}\n---");
                throw;
            }
        }

        public void Save()
        {
        }

        void RunContinuousLogic()
        {
             if (_sender)
             {
                _gridCommunication.Send(_count.ToString());

                _textUtil.Echo(_count.ToString(), false);
                _count++;
             }
             else
             {
                string msg = _gridCommunication.Receive();

                _textUtil.Echo(msg, false);
             }
        }

        void RunCommand(string argument)
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {

            try
            {
                if ((updateSource & COMMAND_UPDATE) != 0)
                {
                    RunCommand(argument);
                }
                if ((updateSource & UpdateType.Update100) != 0)
                {
                    RunContinuousLogic();
                }
            }
            catch (Exception e)
            {
                Echo($"Exception: {e}\n---");
                throw;
            }
        }
    }
}
