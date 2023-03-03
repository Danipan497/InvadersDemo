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
        public Rectangle ClientRectangle;

        private PlayerShip playerShip = new PlayerShip();

        public Point ShipLocatoion { get { return playerShip.Location; } }

        private List<Shot> playerShots = new List<Shot>();
        private List<Shot> enemyShots = new List<Shot>();
        private int playerShotsTaken;

        public void Go(Random random)
        {
            UpdateShots();
        }
        public void setClientRectangle(Rectangle clientRectangle)
        {
            this.ClientRectangle = clientRectangle;
        }

        public void Draw(Graphics g)
        {
            playerShip.Draw(g);
            foreach (Shot shot in playerShots)
                shot.Draw(g);
            foreach (Shot shot in enemyShots)
                shot.Draw(g);
        }

        private void UpdateShots()
        {
            List<Shot> doomedShots = new List<Shot>();
            foreach (Shot shot in playerShots)
            {
                shot.Move();
                if (shot.Location.Y <= 0 || shot.Location.Y >= ClientRectangle.Y + ClientRectangle.Height)
                {
                    doomedShots.Add(shot);
                }
            }
            foreach (Shot shot in enemyShots)
            {
                shot.Move();
                if (shot.Location.Y <= 0 || shot.Location.Y >= ClientRectangle.Y + ClientRectangle.Height)
                {
                    doomedShots.Add(shot);
                }
            }
            foreach (Shot shot in doomedShots)
            {
                if (playerShots.Contains(shot))
                    playerShots.Remove(shot);
                else
                    enemyShots.Remove(shot);
            }
        }

        public void FireShot()
        {
            if (playerShots.Count < 3)
            {
                playerShots.Add(new Shot(playerShip.NewShotLocation, Direction.Up));
            }
            else
                playerShotsTaken++;
        }

        public void MovePlayer(Direction direction)
        {
            if (direction == Direction.Left || direction == Direction.Right)
                playerShip.Move(direction);
        }
    }
}
