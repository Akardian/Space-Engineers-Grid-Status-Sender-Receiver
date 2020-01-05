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
        public class MessageEntity
        {
            public DateTime TimeStamp { get; private set; }
            public long SenderID { get; private set; }
            public string Message { get; private set; }

            private readonly Program _program;

            public MessageEntity(Program program) : this(program, new DateTime(), 0, "") { }
            public MessageEntity(Program program, string message) : this(program, DateTime.Now, 0, message) { }
            public MessageEntity(Program program, DateTime timeStamp, long senderID, string message)
            {
                TimeStamp = timeStamp;
                SenderID = senderID;
                Message = message;
                _program = program;
            }

            public MyTuple<string, string> Serialize()
            {
                return new MyTuple<string, string>(TimeStamp.ToString(), Message);
            }

            public void DeSerialize(MyIGCMessage msg)
            {
                try
                {
                    MyTuple<string, string> msgTuple = (MyTuple<string, string>)msg.Data;

                    TimeStamp = Convert.ToDateTime(msgTuple.Item1);
                    SenderID = msg.Source;
                    Message = msgTuple.Item2;
                }
                catch (Exception e)
                {
                    _program.Echo($"Exception: {e}\n---");
                }
            }
        }
    }
}