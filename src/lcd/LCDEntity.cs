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
        public class LCDEntity
        {
            public List<SurfaceEntity> Surfaces { get; private set; }
            public RectangleF MinViewpoint { get; private set; }

            protected readonly Program _program;

            public LCDEntity(Program program) 
            {
                Surfaces = new List<SurfaceEntity>();
                _program = program;
            }

            public void Add(IMyTextSurface surface)
            {
                SurfaceEntity surfaceE = new SurfaceEntity(
                    surface, 
                    new RectangleF(
                    (surface.TextureSize - surface.SurfaceSize) / 2f,
                    surface.SurfaceSize));
                Surfaces.Add(surfaceE);

                if(MinViewpoint.Size == new Vector2(0,0) ||
                   (surfaceE.Viewport.Size.X + surfaceE.Viewport.Size.Y) <
                   (MinViewpoint.Size.X + MinViewpoint.Size.Y))
                {
                    MinViewpoint = surfaceE.Viewport;
                }
            }

            public void Echo(string msg)
            {
                Echo(msg, true);
            }

            public void Echo(string msg, bool append)
            {
                foreach (SurfaceEntity surface in Surfaces)
                {
                    surface.Surface?.WriteText($"{msg}\n", append);
                }
            }
        }
    }
}
