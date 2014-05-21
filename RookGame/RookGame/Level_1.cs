using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace GameOfPawns
{
    class Level_1 : Level
    {

        public override void LoadContent()
        {
            // first room
            room = new Room_L1_R1();
            room.Initialize(content, gd, player);
            room.setSize(4000);
            room.LoadContent();
        }

    }
}
