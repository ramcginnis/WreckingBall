using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WreckingBall
{
    public class Brick
    {
        private static int totalBricks = 0;
        public Texture2D Texture;
        public Rectangle rectangle;
        public int originalState;
        public int currentState;
        public int id;

        public Brick(Texture2D texture, int x, int y, int state)
        {
            Texture = texture;
            rectangle = new Rectangle(x, y, 50, 30);
            originalState = state;
            currentState = state;
            id = totalBricks;
            totalBricks++;
        }

        public void UpdateTexture(Texture2D texture)
        {
            Texture = texture;
        }
    }
}