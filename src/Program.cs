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

        private IMyRadioAntenna _radioAntenna;
        private string _tag = "c1";
        private bool _sender = true;
        private long _count = 0;

        public Program()
        {
            try
            {
                //Echo = text => {};
                _textUtil = new TextUtil(this);
                if(_sender)
                {
                    _textUtil.AddLCD("LCD-1");
                    _textUtil.AddLCD("LCD-2");
                    _textUtil.TextContentOn();
                    _textUtil.SetFont(TextUtil.FontColor.Green, 1f);

                    _radioAntenna = GridTerminalSystem.GetBlockWithName("Antenna S") as IMyRadioAntenna;

                    _textUtil.Header = $"LCD Header Sender\nAttached PB: {_radioAntenna.AttachedProgrammableBlock.ToString()}\nProgammable B: {IGC.Me.ToString()}";
                } else
                {
                    _textUtil.AddLCD("LCD-3");
                    _textUtil.AddLCD("LCD-4");
                    _textUtil.TextContentOn();
                    _textUtil.SetFont(TextUtil.FontColor.Green, 1f);

                    _radioAntenna = GridTerminalSystem.GetBlockWithName("Antenna R") as IMyRadioAntenna;

                    _textUtil.Header = $"LCD Header Reciever\nAttached PB: {_radioAntenna.AttachedProgrammableBlock.ToString()}\nProgammable B: {IGC.Me.ToString()}";
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
                IGC.SendBroadcastMessage(_tag, _count.ToString(), TransmissionDistance.TransmissionDistanceMax);
                IGC.RegisterBroadcastListener(_tag);

                _textUtil.Echo(_count.ToString(), false);
                _count++;
            }
            else
            {
                IGC.RegisterBroadcastListener(_tag);

                List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>();
                IGC.GetBroadcastListeners(listeners);

                if (listeners[0].HasPendingMessage)
                {
                    MyIGCMessage message = new MyIGCMessage();

                    message = listeners[0].AcceptMessage();

                    string messagetext = message.Data.ToString();

                    string messagetag = message.Tag;

                    long sender = message.Source;

                    //Do something with the information!
                    _textUtil.Echo("Message received with tag" + messagetag + "\n\r", false);
                    _textUtil.Echo("from address " + sender.ToString() + ": \n\r", true);
                    _textUtil.Echo(messagetext, true);
                }

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
