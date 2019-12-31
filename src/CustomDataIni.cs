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

            private MyIni _ini;

            private readonly Program _program;
            private readonly IMyProgrammableBlock _me;

            public CustomDataIni(Program program)
            {
                _program = program;
                _me = program.Me;

                _ini = new MyIni();

                Data = new CustomDataEntity(_program);  
            }

            public void Load()
            {
                _ini.TryParse(_program.Storage);
                Data.LoadData(_ini);
            }

            public void Save()
            {
                _ini.Clear();
                Data.SaveData(_ini);
                _program.Storage = _ini.ToString();
            }

            public void SaveToCustomData()
            {
                try {
                    MyIniParseResult result;
                    if (!_ini.TryParse(_me.CustomData, out result))
                        throw new Exception(result.ToString());

                    Data.SaveData(_ini);

                    _me.CustomData = _ini.ToString();
                } catch (Exception)
                {
                    _program.Echo($"ERROR: Parse failed, Duplicated Key?");
                }
            }
            public void LoadCustomData()
            {
                try
                {
                    MyIniParseResult result;
                    if (!_ini.TryParse(_me.CustomData, out result))
                        throw new Exception(result.ToString());

                    Data.LoadData(_ini);
                }
                catch (Exception)
                {
                    _program.Echo($"ERROR: Parse failed, Duplicated Key?");
                }
            }

            public void Clear()
            {
                _ini.Clear();
                Data.LoadData(_ini);

                _program.Storage = _ini.ToString();
                _me.CustomData = _ini.ToString();

                Load();
                SaveToCustomData();
            }
        }
    }
}
