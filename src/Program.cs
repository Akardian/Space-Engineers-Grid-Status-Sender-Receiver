using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
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
        private TextUtil _debug;

        private CustomDataIni _ini;

        private GridCommunication _gridCommunication;
        
        private long _count = 0;

        private MessageEntity _entity1;
        private MessageEntity _entity2;

        public Program()
        {
            try
            {
                _debug = new TextUtil(this);

                _ini = new CustomDataIni(this);
                _ini.Load();
                _ini.SaveToCustomData();                

                Init();

                Runtime.UpdateFrequency = UpdateFrequency.Update100;
            }
            catch (Exception e)
            {
                Echo($"Exception: {e}\n---");
                throw;
            }
        }

        private void Init()
        {
            _debug.AddLCD(_ini.Data.DebugLCD);
            _debug.TextContentOn();
            _debug.SetFont(TextUtil.FontColor.Green, 1f);
            _debug.Header = "Debug";

            Echo("");
            Echo = _debug.Echo;

            _entity1 = new MessageEntity(this, "start");
            _entity2 = new MessageEntity(this, "start");

            _textUtil = new TextUtil(this);
            _textUtil.AddLCD(_ini.Data.LcdOutputList);
            _textUtil.TextContentOn();
            _textUtil.SetFont(TextUtil.FontColor.Green, 1f);

            if (_ini.Data.Sender)
            {
                _gridCommunication = new GridCommunication(this, GridCommunication.ClientType.Sender, _ini.Data.Channel);
                _textUtil.Header = $"LCD Header Sender\n";
            }
            else
            {
                _gridCommunication = new GridCommunication(this, GridCommunication.ClientType.Reciever, _ini.Data.Channel);
                _textUtil.Header = $"LCD Header Receiver\n";
            }
        }

        public void Save()
        {
            _ini.Save();
        }

        void RunContinuousLogic()
        {
             if (_ini.Data.Sender)
             {
                MessageEntity msg = new MessageEntity(this, _count.ToString()); 
                
                _gridCommunication.Send(msg);

                _textUtil.Echo(msg.TimeStamp.ToString(), false);
                _textUtil.Echo(msg.SenderID.ToString(), true);
                _textUtil.Echo(msg.Message, true);

                _count++;
             }
             else
             {
                MessageEntity msg = _gridCommunication.Receive();
                while (msg != null)
                {
                    if (msg.SenderID == 77207791465205788 && msg.TimeStamp > _entity2.TimeStamp)
                    {
                        _textUtil.Echo(_entity1.TimeStamp.ToString(), false);
                        _textUtil.Echo(_entity1.SenderID.ToString(), true);
                        _textUtil.Echo(_entity1.Message, true);
                        _textUtil.Echo("", true);
                        _textUtil.Echo(msg.TimeStamp.ToString(), true);
                        _textUtil.Echo(msg.SenderID.ToString(), true);
                        _textUtil.Echo(msg.Message, true);
                        _entity2 = msg;
                    }

                    if (msg.SenderID == 98924271274665271 && msg.TimeStamp > _entity1.TimeStamp)
                    {
                        _textUtil.Echo(msg.TimeStamp.ToString(), false);
                        _textUtil.Echo(msg.SenderID.ToString(), true);
                        _textUtil.Echo(msg.Message, true);
                        _textUtil.Echo("", true);
                        _textUtil.Echo(_entity2.TimeStamp.ToString(), true);
                        _textUtil.Echo(_entity2.SenderID.ToString(), true);
                        _textUtil.Echo(_entity2.Message, true);
                        _entity1 = msg;
                    }

                    msg = _gridCommunication.Receive();
                }
            }
        }

        void RunCommand(string argument)
        {
            if(argument.Equals("load"))
            {
                _ini.LoadCustomData();
                Init();
            }
            if (argument.Equals("clear"))
            {
                _ini.Clear();
                Init();
            }
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
