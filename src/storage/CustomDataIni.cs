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
        public class CustomDataIni
        {
            public CustomDataEntity Data { get; private set; }

            private readonly MyIni _ini;

            private readonly Program _program;
            private readonly IMyProgrammableBlock _me;

            public CustomDataIni(Program program)
            {
                _program = program;
                _me = program.Me;

                _ini = new MyIni();

                Data = new CustomDataEntity(_program);  
            }

            public void Save()
            {
                _ini.Clear();
                Data.SaveData(_ini);
                _program.Storage = _ini.ToString();
            }

            public void Load()
            {
                _ini.TryParse(_program.Storage);
                Data.LoadData(_ini);
            }

            public bool SaveToCustomData()
            {
                MyIniParseResult result;
                bool succes = _ini.TryParse(_me.CustomData, out result);
                if (succes)
                {
                    Data.SaveData(_ini);

                    _me.CustomData = _ini.ToString();
                }
                else
                {
                    _program.Echo($"ERROR: Parse failed, Duplicated LCD Key?");
                }
                return succes;
            }
            public bool LoadCustomData()
            {
                MyIniParseResult result;
                bool succes = _ini.TryParse(_me.CustomData, out result);
                if (succes)
                {
                    Data.LoadData(_ini);
                } else
                {
                    _program.Echo($"ERROR: Parse failed, Duplicated LCD Key?");
                }
                return succes;
            }

            public void SetDefaultValues()
            {
                    _ini.Clear();
                    Data.SetDefault(_ini);
                    _me.CustomData = "";
                    _me.CustomData = _ini.ToString();
            }
        }
    }
}
