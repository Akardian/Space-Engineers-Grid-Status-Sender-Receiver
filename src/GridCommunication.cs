﻿using Sandbox.Game.EntityComponents;
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

            private AntennaType _antennaType;
            private IMyLaserAntenna _laserAntenna;
            private IMyRadioAntenna _radioAntenna;

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

                IGC().RegisterBroadcastListener(_tag);
            }

            public void Send(string msg)
            {
                IGC().SendBroadcastMessage(_tag, msg, TransmissionDistance.TransmissionDistanceMax);
            }

            public string Receive()
            {
                string message = "";

                List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>();
                IGC().GetBroadcastListeners(listeners);
                if (listeners[0].HasPendingMessage)
                {
                    MyIGCMessage data = new MyIGCMessage();
                    data = listeners[0].AcceptMessage();

                    message = data.Data.ToString();
                }
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
                        _antennaType = AntennaType.Laser;
                        _laserAntenna = laserAntenna;
                        isSet = true;
                    }
                } else
                {
                    _program.Echo("Antenna found: Radio Antenna");
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

