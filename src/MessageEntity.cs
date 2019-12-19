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
            private DateTime _timeStamp;
            private long _senderID;
            private string _message;

            private readonly Program _program;

            public DateTime TimeStamp
            {
                get { return _timeStamp; }
            }
            public long SenderID
            {
                get { return _senderID;  }
            }
            public string Message
            {
                get { return _message;  }
            }

            public MessageEntity(Program program, object serializedMsg)
            {
                _program = program;
                DeSerialize(serializedMsg.ToString());
            }

            public MessageEntity(Program program, string message) : this (program, program.Me.EntityId, message) { } 

            public MessageEntity(Program program, long senderID, string message) : this(program ,DateTime.Now, senderID, message) { }

            public MessageEntity(Program program, DateTime timeStamp, long senderID, string message)
            {
                _timeStamp = timeStamp;
                _senderID = senderID;
                _message = message;
                _program = program;
            }

            public string Serialize()
            {

                return _timeStamp + ";" + _senderID + ";" + _message;
            }

            public void DeSerialize(String msg)
            {
                try
                {
                    _program.Echo($"DeSerialize: {msg}");
                    String[] strlist = msg.Split(';');
                    _timeStamp = Convert.ToDateTime(strlist[0]);
                    _senderID = Convert.ToInt64(strlist[1]);
                    _message = strlist[2];
                } catch (Exception e)
                {
                    _program.Echo($"Exception: {e}\n---");
                }
            }
        }
    }
}
