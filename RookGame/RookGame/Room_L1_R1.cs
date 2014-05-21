using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace GameOfPawns
{

    class Room_L1_R1 : Room
    {
        
        public override void LoadContent()
        {

            createArea(0, gd.Viewport.Height);

            player.setPosition(150,GameOfPawns.playHeight);
            
            Enemy enemy1 = new Enemy(300, GameOfPawns.playHeight);

            EnemyGenerator genBasic = new EnemyGenerator(300, 0);
            genBasic.setMaxEnemies(1);
            genBasic.addEnemy(enemy1,2.0f);

            area.addObject(0, 0, "street", 16, 2);
            area.addObject(0, 135, "grass", 25, 2);
            area.addObject(0, 110, "sidewalktop2", 25, 1);
            area.addObject(500, 160, "trashcan", 1);
            area.addObject(300, 130, "car1", 1);
            area.addObject(700, 130, "car2", 1);
            area.addObject(600, 20, "manhole", 1);
            area.addObject(genBasic);
            //area.addObject(300, 0, "bathroomtile", 1, 5).setSolid(true);
            area.addObject(player);

            area.LoadContent();

        }

    }
}
