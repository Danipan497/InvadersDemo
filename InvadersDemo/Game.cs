using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvadersDemo
{
    internal class Game
    {
        private PlayerShip playerShip = new PlayerShip();

        public void Draw(Graphics g)
        {
            playerShip.Draw(g);
        }

        public void MovePlayer(Direction direction)
        {
            if (direction == Direction.Left || direction == Direction.Right)
                playerShip.Move(direction);
        }
    }
}
