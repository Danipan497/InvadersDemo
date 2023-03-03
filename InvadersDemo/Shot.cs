using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvadersDemo
{
    internal class Shot
    {
        public Point Location;
        internal Direction upOrDown;
        public static Size size = new Size(4, 12);
        public static int moveInterval = 3;

        public Shot(Point location, Direction direction)
        {
            this.Location = location;
            this.upOrDown = direction;
        }

        public virtual void Draw(Graphics g)
        {
            g.FillRectangle((upOrDown == Direction.Up) ? Brushes.Green : Brushes.Red, Location.X - (int)(size.Width / 2),
                Location.Y - (int)(size.Height / 2), size.Width, size.Height);
        }

        public virtual void Move()
        {
            if (upOrDown == Direction.Up)
                Location.Y -= moveInterval;
            else
                Location.Y += moveInterval;
        }
    }
}
