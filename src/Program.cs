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

        private LCDUtil _debug;

        private CustomDataIni _ini;

        private Sender _sender;
        private Receiver _reciever;

        public Program()
        {
            try
            {
                if(Init())
                {
                    Runtime.UpdateFrequency = UpdateFrequency.Update100;
                }                
            }
            catch (Exception e)
            {
                Echo($"Exception: {e}\n---");
                throw;
            }
        }

        public bool Init()
        {
            _ini = new CustomDataIni(this);
            if (_ini.LoadCustomData())
            {
                if (!_ini.SaveToCustomData())
                {
                    throw new Exception($"Exception: Could not save to custom data\n---");
                }

                _debug = new LCDUtil(this);
                _debug.Add(_ini.Data.DebugLCD);
                _debug.TextContentOn();
                _debug.SetDefaultFont(LCDUtil.FontColor.Green, 1f);
                _debug.Echo("", false);

                Echo = _debug.Echo;

                Echo($"Init: Set client type to {_ini.Data.ClientType}");
                if (_ini.Data.ClientType == CustomDataEntity.ClientTypes.Sender)
                {
                    _sender = new Sender(this, _ini);
                }
                else if (_ini.Data.ClientType == CustomDataEntity.ClientTypes.Reciever)
                {
                    _reciever = new Receiver(this, _ini);
                }

                return true;
            } else
            {
                Echo($"\nUse the Argument \"default\" to reset\n the custom data");
                return false;
            }
        }

        void RunContinuousLogic()
        {
            if (_ini.Data.ClientType == CustomDataEntity.ClientTypes.Sender)
            {
                _sender.Run();
            }
            else if (_ini.Data.ClientType == CustomDataEntity.ClientTypes.Reciever)
            {
                _reciever.Run();
            } else
            {
                Echo("No Client Type set.");
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
        }

        void RunCommand(string argument)
        {
            if(argument.Equals("default"))
            {
                _ini.SetDefaultValues();
                Init();
            }
        }

        public void Save()
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
