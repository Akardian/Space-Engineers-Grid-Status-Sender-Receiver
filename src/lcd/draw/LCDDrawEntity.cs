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
        public class LCDDrawEntity : LCDEntity
        {
            public int Count { get; private set; }
            public Dictionary<float, int> Position { get; private set; }
            public Dictionary<int, List<MySprite>> Sprites { get; set; }

            public LCDDrawEntity(Program program) : base(program)
            {
                Count = 0;
                Position = new Dictionary<float, int>();
                Sprites = new Dictionary<int, List<MySprite>>();
            }

            public int ReserveSpace(float senderID)
            {
                Count++;
                Position.Add(senderID, Count);
                return Count;
            }

            public void Draw()
            {
                foreach (SurfaceEntity surface in Surfaces)
                {
                    MySpriteDrawFrame frame = surface.Surface.DrawFrame();
                    foreach (KeyValuePair<int, List<MySprite>> pair in Sprites)
                    {
                        foreach (MySprite sprite in pair.Value)
                        {
                            frame.Add(sprite);
                        }
                    }
                    frame.Dispose();
                }
            }


        }
    }
}
