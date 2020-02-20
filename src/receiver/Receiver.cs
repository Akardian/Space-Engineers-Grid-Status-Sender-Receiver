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
        public class Receiver
        {
            private readonly LCDDraw _lcd;
            private readonly List<SenderEntity> _senderList;

            private readonly GridCommunication _gridCommunication;
            private readonly int _timeoutTime;

            private int _checkConnection;

            private readonly Program _program;
            private readonly Action<MessageEntity, SenderEntity> UPDATE_SENDER;

            private const int ReservedLCDLines = 5;
            private const int LINE_TIMESTAMP = 1;
            private const int LINE_SENDER_NAME = 2;
            private const int LINE_CONNECTION = 3;
            private const int LINE_STATUS = 4;

            public Receiver(Program program, CustomDataIni ini)
            {
                UPDATE_SENDER = UpdateSenderDraw;

                _program = program;
                int maxLines = ini.Data.MaxSenderOnLCD * ReservedLCDLines;

                _lcd = new LCDDraw(program, ini.Data.MaxSenderOnLCD);
                _lcd.AddLCD(ini.Data.LcdOutputList);
                _lcd.SetLCDContent(ContentType.SCRIPT);
                _lcd.Script("");
                _lcd.ScriptColor(new Color(0, 0, 0, 255), new Color(0, 255, 0, 255));

                _gridCommunication = new GridCommunication(program, GridCommunication.ClientType.Reciever, ini.Data.Channel);
                _senderList = new List<SenderEntity>();

                _timeoutTime = ini.Data.TimeOutTime;
                _program.Echo($"TimeoutTime: {_timeoutTime}, MaxLinesOnLCD: {maxLines}");
            }

            public void Run()
            {
                MessageEntity msg = _gridCommunication.Receive();
                while (msg != null)
                {
                    SenderEntity senderEntity = (
                        from sender in _senderList.AsEnumerable()
                        where sender.ID == msg.SenderID
                        select sender).SingleOrDefault();

                    if (senderEntity == null)
                    {
                        AddSender(msg, UPDATE_SENDER);                        
                    } else
                    {
                        UPDATE_SENDER(msg, senderEntity);
                    }

                    msg = _gridCommunication.Receive();
                }

                if (_senderList.Any())
                {
                    CheckConnection(UPDATE_SENDER);
                }
            }

            private void CheckConnection(Action<MessageEntity, SenderEntity> update)
            {
                if(_senderList.Count <= _checkConnection)
                {
                    _checkConnection = 0;
                }
                
                SenderEntity sender = _senderList[_checkConnection];
                Double time = Math.Abs((DateTime.Now - sender.LastUpdate).TotalSeconds);
                if (time > _timeoutTime)
                {
                    sender.CurrentStatus = SenderEntity.Status.Disconnected;
                    update(null, sender);
                }
                else
                {
                    sender.CurrentStatus = SenderEntity.Status.Connected;
                }

                _checkConnection++;
            }

            private void AddSender(MessageEntity msg, Action<MessageEntity, SenderEntity> update)
            {
                _program.Echo($"Add Sender: {msg.SenderID} ");

                MyTuple<LCDDrawEntity, int[]> lcdTuple = _lcd.ReserveLCD(ReservedLCDLines);
                if (lcdTuple.Item1 != null)
                {
                    SenderEntity sender = new SenderEntity(
                    msg.SenderID,
                    msg.TimeStamp,
                    lcdTuple.Item1,
                    SenderEntity.Status.Connected,
                    lcdTuple.Item2);

                    _senderList.Add(sender);

                    update(msg, sender);
                }
                else
                {
                    _program.Echo($"ERROR: Could not reserve LCD");
                }
            }

            private void UpdateSenderDraw(MessageEntity msg, SenderEntity sender)
            {
                foreach (SurfaceEntity surface in sender.LCD.Surfaces)
                {
                    Vector2 position = new Vector2(20, 20) + surface.Viewport.Position;
                    position.Y = +(80 * sender.LineNumber[0]);

                    MySpriteDrawFrame frame = surface.Surface.DrawFrame();
                    DrawSprites(msg, frame, surface, position);
                    frame.Dispose();
                } 
            }

            private MySprite DrawBar(float current, float max, Vector2 surfaceSize, Vector2 position, Color color)
            {
                float length = surfaceSize.X * (current / max);
                return new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "White screen",
                    Position = position,
                    Size = new Vector2(length, surfaceSize.Y),
                    Color = color,
                    Alignment = TextAlignment.LEFT
                };
            }

            private MySprite DrawText(string msg, Vector2 position, float scale, Color color)
            {
                 return new MySprite()
                 {
                     Type = SpriteType.TEXT,
                     Data = msg,
                     Position = position,
                     RotationOrScale = scale,
                     Color = color,
                     Alignment = TextAlignment.LEFT,
                     FontId = "White"
                 };
            }

            private void DrawSprites(MessageEntity msg, MySpriteDrawFrame frame, SurfaceEntity lcd, Vector2 position)
            {
                if (msg != null)
                {
                   
                    Vector2 barSize = new Vector2((lcd.Viewport.Size.X - 40), 20);

                    frame.Add(DrawText(msg.SenderName, position, 1.0F, Color.Green));
                    position += new Vector2(0, 30);
                    frame.Add(DrawText(msg.TimeStamp.ToString(), position, 1.0F, Color.Green));

                    position += new Vector2(-10, 30);
                    Vector2 graphSize = new Vector2(lcd.Viewport.Size.X - 20, 1);
                    frame.Add(DrawBar(100, 100, graphSize, position, Color.Aqua));

                    position += new Vector2(10, 15);
                    frame.Add(DrawBar(10, 100, barSize, position, Color.Green));
                    position += new Vector2(0, 25);
                    frame.Add(DrawBar(50, 100, barSize, position, Color.Green));
                    position += new Vector2(0, 25);
                    frame.Add(DrawBar(100, 100, barSize, position, Color.Green));
                }
            }
        }
    }
}
