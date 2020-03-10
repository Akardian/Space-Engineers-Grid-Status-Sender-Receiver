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

            private readonly int MAX_SENDER_ON_LCD;

            public Receiver(Program program, CustomDataIni ini)
            {
                UPDATE_SENDER = DrawNewSprite;

                _program = program;
                MAX_SENDER_ON_LCD = ini.Data.MaxSenderOnLCD;

                _lcd = new LCDDraw(program, ini.Data.MaxSenderOnLCD);
                _lcd.AddLCD(ini.Data.LcdOutputList);
                _lcd.SetLCDContent(ContentType.SCRIPT);
                _lcd.Script("");
                _lcd.ScriptColor(new Color(0, 0, 0, 255), new Color(0, 255, 0, 255));

                _gridCommunication = new GridCommunication(program, GridCommunication.ClientType.Reciever, ini.Data.Channel);
                _senderList = new List<SenderEntity>();

                _timeoutTime = ini.Data.TimeOutTime;
                _program.Echo($"TimeoutTime: {_timeoutTime}, MaxSenderOnLCD: {MAX_SENDER_ON_LCD}");
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
                    //CheckConnection(UPDATE_SENDER);
                }

                _lcd.UpdateDraw();
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

                MyTuple<LCDDrawEntity, int[]> lcdTuple = _lcd.ReserveLCD(msg.SenderID);
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

            private void DrawNewSprite(MessageEntity msg, SenderEntity sender) {

                if (sender.LCD.Sprites.ContainsKey(sender.LineNumber[0]))
                { //Change existing Sprite
                    sender.LCD.Sprites[sender.LineNumber[0]] = DrawSprites(msg, sender.LCD.MinViewpoint, sender.LineNumber[0]);
                } else
                { // Add new Sprite
                    sender.LCD.Sprites.Add(sender.LineNumber[0], DrawSprites(msg, sender.LCD.MinViewpoint, sender.LineNumber[0]));
                }
                
            }
 
            private List<MySprite> DrawSprites(MessageEntity msg, RectangleF viewport, int lcdPlace)
            {
                //Calculate Percent
                float yP = (viewport.Height / MAX_SENDER_ON_LCD) / 100;
                float xP = viewport.Width / 100;

                //Set Start Possition
                Vector2 startPosition = viewport.Position + new Vector2(xP * 2, yP * 2);
                startPosition += new Vector2(0, (yP * 100) * (lcdPlace - 1));

                List<MySprite> sprite = new List<MySprite>();
                if (msg != null)
                {
                    sprite = DrawConnection(startPosition, msg, xP, yP);
                }

                return sprite;
            }

            public List<MySprite> DrawConnection(Vector2 position, MessageEntity msg, float xP, float yP)
            {
                // Bar length
                Vector2 barSize = new Vector2(xP * 55, yP * 10);
                List<MySprite> sprite = new List<MySprite>
                {
                    //Draw: Name
                    DrawText(msg.SenderName,
                                    position + new Vector2(0, 0),
                                    (yP * 0.4F),
                                    Color.Green),
                    //Draw: TimeStamp
                    DrawText(msg.TimeStamp.ToString(),
                                    position + new Vector2(0, yP * 10),
                                    (yP * 0.35F),
                                    Color.Green),
                    //Draw: Line
                    DrawBar(100, 100,
                                    new Vector2(xP * 96, 1),
                                    position + new Vector2(0, yP * 20),
                                    Color.Aqua),
                

                    //Battery Status
                    DrawText("Battery",
                                    position + new Vector2(0, yP * 20),
                                    (yP * 0.35F),
                                    Color.Green),
                    DrawBar( msg.CurrentBatteryPower, msg.MaxBatteryPower,
                                    barSize,
                                    position + new Vector2((xP * 96 - barSize.X), yP * 25),
                                    Color.Red),

                     //Hydro
                    DrawText("Hydrogen",
                                    position + new Vector2(0, yP * 31),
                                    (yP * 0.35F),
                                    Color.Green),
                    DrawBar( msg.CurrentHydrogen, msg.MaxHydrogen,
                                    barSize,
                                    position + new Vector2((xP * 96 - barSize.X), yP * 36),
                                    Color.Yellow),

                    //Draw: BarGrid
                    DrawBar(100, 100,
                                    new Vector2(1, yP * 35),
                                    position + new Vector2((xP * 96 - barSize.X), yP * 20 + (yP * 17.5F)),
                                    Color.Aqua),
                    DrawBar(100, 100,
                                    new Vector2(1, yP * 35),
                                    position + new Vector2((xP * 96 - barSize.X) + (barSize.X / 2), yP * 20 + (yP * 17.5F)),
                                    Color.Aqua),
                    DrawBar(100, 100,
                                    new Vector2(1, yP * 35),
                                    position + new Vector2((xP * 96 - barSize.X) + barSize.X, yP * 20 + (yP * 17.5F)),
                                    Color.Aqua)
                };

                return sprite;
            }

            private MySprite DrawBar(double current, double max, Vector2 surfaceSize, Vector2 position, Color color)
            {
                float x = surfaceSize.X / 100.0F * (float)(current / max * 100.0F);

                return new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "White screen",
                    Position = position,
                    Size = new Vector2(x, surfaceSize.Y),
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

            private MySprite DrawSprite(string sprite, Vector2 size, Vector2 position, Color color)
            {
                return new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = sprite,
                    Position = position,
                    Size = size,
                    Color = color,
                    Alignment = TextAlignment.LEFT,
                };
            }
        }
    }
}
