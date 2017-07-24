using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKPlatformerExample
{
    class Enemy
    {
        //Slime = 0
        //Snail = 1
        //GrassBlock = 2
        //Ghost = 3
        //EventGhost = 4
        //Piranha = 5
        public int enemyType;
        public static bool noGhostattack = true;
        public object enemyClass;
        public int hash; //Identify the enemy thru a fast list check
        public bool update;
        public Vector2 position; //The start position
        public Enemy(object Enemy,int Type)
        {
            enemyClass = Enemy;
            enemyType = Type;
            if (enemyType == 0)
            {
                this.hash = ((Slime)enemyClass).hash;
                this.position = ((Slime)enemyClass).position;
            }
            else if (enemyType == 1)
            {
                this.hash = ((Snail)enemyClass).hash;
                this.position = ((Snail)enemyClass).position;
            }
            else if (enemyType == 2)
            {
                this.hash = ((GrassBlock)enemyClass).hash;
                this.position = ((GrassBlock)enemyClass).position;
            }else if (enemyType == 3)
            {
                this.hash = ((Ghost)enemyClass).hash;
                this.position = ((Ghost)enemyClass).position;
            }else if (enemyType == 4)
            {
                this.hash = ((EventGhost)enemyClass).hash;
                this.position = ((EventGhost)enemyClass).position;
            }else if (enemyType == 5)
            {
                this.hash = ((Piranha)enemyClass).hash;
                this.position = ((Piranha)enemyClass).position;
            }
        }
        public void Draw()
        {
            if (enemyType == 0)
            {
                ((Slime)enemyClass).Draw();
            }else if (enemyType == 1)
            {
                ((Snail)enemyClass).Draw();
            }
            else if (enemyType == 2)
            {
                ((GrassBlock)enemyClass).Draw();
            }else if (enemyType == 3)
            {
                ((Ghost)enemyClass).Draw();
            }
            else if (enemyType == 4)
            {
                ((EventGhost)enemyClass).Draw();
            }
            else if (enemyType == 5)
            {
                ((Piranha)enemyClass).Draw();
            }
        }
        public void Update(ref Level level, Player player)
        {
            if (!update)  
                return;
            
            if (enemyType == 0)
            {
                ((Slime)enemyClass).Update(ref level,player);
            }
            else if (enemyType == 1)
            {
                ((Snail)enemyClass).Update(ref level, player);
            }
            else if (enemyType == 2)
            {
                ((GrassBlock)enemyClass).Update(ref level, player);
            }else if (enemyType == 3)
            {
                ((Ghost)enemyClass).Update(ref level, player);
            }
            else if (enemyType == 4)
            {
                ((EventGhost)enemyClass).Update(ref level, player);
                if (Game.rand.Next(0, 50) == 49 && noGhostattack)
                {
                    ((EventGhost)enemyClass).Attack(player);
                    noGhostattack = false;
                }
            }
            else if (enemyType == 5)
            {
                ((Piranha)enemyClass).Update(ref level, player);
            }

        }
    }
}
