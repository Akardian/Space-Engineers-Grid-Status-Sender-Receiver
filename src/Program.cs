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
        private TextUtil _textUtil;

        private IMyRadioAntenna _radioAntenna;
        private string _channel = "c1";

        public Program()
        {
            try
            {
                //Echo = text => {};
                _textUtil = new TextUtil(this);
                _textUtil.AddLCD("LCD-1");
                _textUtil.AddLCD("LCD-2");
                _textUtil.AddLCD("Not Found LCD");
                _textUtil.TextContentOn();
                _textUtil.SetFont(TextUtil.FontColor.Green, 1f);
                _textUtil.Header = "LCD Header";

                _radioAntenna = GridTerminalSystem.GetBlockWithName("Antenna S") as IMyRadioAntenna;

                _textUtil.Echo($"Attached PB: {_radioAntenna.AttachedProgrammableBlock.ToString()}");
                _textUtil.Echo($"Progammable B: {IGC.Me.ToString()}");
                
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

        public void Main(string argument, UpdateType updateSource)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                Echo($"Exception: {e}\n---");
                throw;
            }
        }
    }
}
