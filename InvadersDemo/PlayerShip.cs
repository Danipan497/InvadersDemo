using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvadersDemo
{
    internal class PlayerShip
    {
        public Point Location;
        public Point NewShotLocation { get { return new Point(Location.X + 26, Location.Y - 11); } }
        public Rectangle Area { get { return new Rectangle(Location, new Size(50, 50)); } }
        public const int MoveIncrement = 5;

        public PlayerShip()
        {
            Location = new Point(625, 750);
        }

        public void Draw(Graphics g)
        {
            g.DrawImageUnscaled(Properties.Resources.player, Location);
        }

        public void Move(Direction direction)
        {
            if (direction == Direction.Left)
            {
                Location.X -= MoveIncrement;
            }
            else
                Location.X += MoveIncrement;
        }
    }
}
