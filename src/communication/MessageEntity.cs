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
using System.Collections.Immutable;

namespace IngameScript
{
    partial class Program
    {
        public class MessageEntity
        {
            public DateTime TimeStamp { get; private set; }
            public long SenderID { get; private set; }
            public string SenderName { get; private set; }
            public float CurrentBatteryPower { get; private set; }
            public float MaxBatteryPower { get; private set; }
            public double CurrentHydrogen { get; private set; }
            public double MaxHydrogen { get; private set; }

            private readonly Program _program;

            public MessageEntity(Program program) : this(program, "", -1, -1, -1, -1) { }
            public MessageEntity(Program program, string senderName,
                                 float currentBatteryPower, float maxBatteryPower,
                                 double currentHydrogen, double maxHydrogen)
            {
                _program = program;

                TimeStamp = DateTime.Now;
                SenderID = -1;
                SenderName = senderName;

                CurrentBatteryPower = currentBatteryPower;
                MaxBatteryPower = maxBatteryPower;

                CurrentHydrogen = currentHydrogen;
                MaxHydrogen = maxHydrogen;
            }

            public MyTuple<string, string, ImmutableArray<float>, ImmutableArray<double>> Serialize()
            {
                return new MyTuple<string, string, ImmutableArray<float>, ImmutableArray<double>> (
                    TimeStamp.ToString(),
                    SenderName,
                    ImmutableArray.Create(CurrentBatteryPower, MaxBatteryPower),
                    ImmutableArray.Create(CurrentHydrogen, MaxHydrogen)
                   );
            }

            public void DeSerialize(MyIGCMessage msg)
            {
                try
                {
                    MyTuple<string, string, ImmutableArray<float>, ImmutableArray<double>> msgTuple = 
                    (MyTuple<string, string, ImmutableArray<float>, ImmutableArray<double>>)msg.Data;

                    SenderID = msg.Source;

                    TimeStamp = Convert.ToDateTime(msgTuple.Item1);
                    SenderName = msgTuple.Item2;

                    CurrentBatteryPower = msgTuple.Item3[0];
                    MaxBatteryPower = msgTuple.Item3[1];

                    CurrentHydrogen = msgTuple.Item4[0];
                    MaxHydrogen = msgTuple.Item4[1];
                }
                catch (Exception e)
                {
                    _program.Echo($"Exception: {e}\n---");
                }
            }
        }
    }
}