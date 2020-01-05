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
        public class GridCommunication 
        {
            private readonly Program _program;

            private readonly ClientType _type;
            private string _tag;
            private int _checkListener;

            private AntennaType _antennaType;
            private IMyLaserAntenna _laserAntenna;
            private IMyRadioAntenna _radioAntenna;

            List<IMyBroadcastListener> _listeners;

            public ClientType Type
            {
                get { return _type; }
            }
            public string Tag
            {
                get { return _tag; }
            }

            public enum ClientType : byte { Sender, Reciever }
            public enum AntennaType : byte { Laser, Radio }

            public GridCommunication(Program program, ClientType type, string tag)
            {
                _program = program;
                _type = type;
                _tag = tag;
                _checkListener = 0;

                _listeners = new List<IMyBroadcastListener>();

                IGC().RegisterBroadcastListener(_tag);
            }

            public void Send(MessageEntity msg)
            {
                IGC().SendBroadcastMessage(_tag, msg.Serialize(), TransmissionDistance.TransmissionDistanceMax);
            }

            public MessageEntity Receive()
            {
                MessageEntity message = null;

                IGC().GetBroadcastListeners(_listeners);

                if(_checkListener >= _listeners.Count)
                {
                    _checkListener = 0;
                }

                if (_listeners[_checkListener].HasPendingMessage)
                {
                    message = new MessageEntity(_program);
                    message.DeSerialize(_listeners[_checkListener].AcceptMessage());
                }
                _checkListener++;

                return message;
            }

            public bool SetAntennaBLock(string name)
            {
                bool isSet = false;
                IMyLaserAntenna laserAntenna;
                IMyRadioAntenna radioAntenna = GridTerminalSystem().GetBlockWithName(name) as IMyRadioAntenna;

                if(radioAntenna == null)
                {
                    laserAntenna = GridTerminalSystem().GetBlockWithName(name) as IMyLaserAntenna;
                    if (laserAntenna == null)
                    {
                        _program.Echo("ERROR: No matching Antenna Found");
                    } else
                    {
                        _program.Echo("Antenna found: Laser Antenna");
                        //_laserAntenna.AttachedProgrammableBlock = _program.Me.EntityId;
                        _antennaType = AntennaType.Laser;
                        _laserAntenna = laserAntenna;
                        isSet = true;
                    }
                } else
                {
                    _program.Echo("Antenna found: Radio Antenna");
                    //_radioAntenna.AttachedProgrammableBlock = _program.Me.EntityId;
                    _antennaType = AntennaType.Radio;
                    _radioAntenna = radioAntenna;
                    isSet = true;
                }
                return isSet;
            }

            private IMyGridTerminalSystem GridTerminalSystem()
            {
                return _program.GridTerminalSystem;
            }

            private IMyIntergridCommunicationSystem IGC()
            {
                return _program.IGC;
            }
        }
    }
}

