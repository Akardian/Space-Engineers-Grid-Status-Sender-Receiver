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
                _debug = new LCDUtil(this);
                Echo("");
                Echo = _debug.Echo;

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
            _debug.Add(_ini.Data.DebugLCD);
            _debug.TextContentOn();
            _debug.SetFont(LCDUtil.FontColor.Green, 1f);
            _debug.Echo("", false);

            if (_ini.Data.Sender)
            {
                _sender = new Sender(this, _ini);
            }
            else
            {
                _reciever = new Receiver(this, _ini);
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
                _sender.Run();
             }
             else
             {
                _reciever.Run();
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
