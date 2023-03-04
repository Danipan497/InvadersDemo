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
        private int livesLeft = 2;
        private int wave = 0;
        private int framesSkipped = 5;
        private int currentFrame = 0;
        private int score;
        private DateTime waveStarted;

        private Random random = new Random();
        public Rectangle ClientRectangle;

        private PlayerShip playerShip = new PlayerShip();

        public Point ShipLocatoion { get { return playerShip.Location; } }

        private List<Shot> playerShots = new List<Shot>();
        private List<Shot> enemyShots = new List<Shot>();
        private int playerShotsTaken;

        private Direction invaderDirection = Direction.Right;
        private Direction directionInvadersJustMoveIn = Direction.Right;
        private const int invaderMargin = 10;
        private List<Invader> invaders = new List<Invader>();
        public List<Invader> Invaders { get {  return invaders; } }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public Boolean GameOver { get; private set; }

        public void Go(Random random)
        {
            MoveInvaders();
            UpdateShots();
            CheckForInvaderCollisions();
            CheckForPlayerCollisions();
            if (!GameOver)
            {
                if (invaders.Count == 0)
                    NextWave();
                ReturnFire();
            }
        }
        public void setClientRectangle(Rectangle clientRectangle)
        {
            this.ClientRectangle = clientRectangle;
        }

        public void Draw(Graphics g, int animationCell)
        {
            foreach (Invader invader in invaders)
            {
                invader.Draw(g, animationCell);
            }
            playerShip.Draw(g);
            foreach (Shot shot in playerShots)
                shot.Draw(g);
            foreach (Shot shot in enemyShots)
                shot.Draw(g);
            int x = ClientRectangle.X + ClientRectangle.Width - Properties.Resources.player.Width - 20;
            for (int i = livesLeft; i > 0; i--)
            {
                g.DrawImageUnscaled(Properties.Resources.player, new Point(x, ClientRectangle.Bottom - Properties.Resources.player.Height - 5));
                x -= Properties.Resources.player.Width + 10;
            }
            using (Font scoreFont = new Font(FontFamily.GenericSerif, 16, FontStyle.Regular))
                g.DrawString("Score  " + score.ToString(), scoreFont, Brushes.White, 10, 10);
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

        public void NextWave()
        {
            wave++;
            waveStarted = DateTime.Now;
            playerShots.Clear();
            enemyShots.Clear();
            invaders = new List<Invader>();
            InvaderType type = InvaderType.Bug + wave;
            for (int y = 50; y < 250; y += 80) 
            {
                for (int x = 50; x < 1300; x += 80)
                {
                    invaders.Add(new Invader(type, new Point(x, y)));
                }
                type++;
            }
            if (framesSkipped > 0)
            {
                framesSkipped--;
            }
        }

        private void CheckForInvaderCollisions()
        {
            List<Invader> deadInvaders = new List<Invader>();
            List<Shot> usedShots = new List<Shot>();
            foreach (Shot shot in playerShots)
            {
                foreach (Invader invader in invaders)
                {
                    if (invader.Hitbox.Contains(shot.Location))
                    {
                        deadInvaders.Add(invader);
                        usedShots.Add(shot);
                    }
                }
            }
            foreach (Invader invader in deadInvaders)
            {
                Score += invader.Score;
                invaders.Remove(invader);
            }
            foreach (Shot shot in usedShots)
                playerShots.Remove(shot);
        }

        private void CheckForPlayerCollisions()
        {
            List<Shot> usedShots = new List<Shot>();
            foreach (Shot shot in enemyShots)
            {
                if (playerShip.Area.Contains(shot.Location))
                {
                    livesLeft--;
                    usedShots.Add(shot);
                }
            }
            foreach (Shot shot in usedShots)
                enemyShots.Remove(shot);
            if (livesLeft == 0)
                OnGameOver();
        }

        private bool invadersMoveDown { get
            {
                foreach (Invader invader in invaders)
                    if (invader.Hitbox.Right >= ClientRectangle.Right - invaderMargin)
                        return true;
                    else if (invader.Location.X <= invaderMargin)
                        return true;
                return false; } }

        private void MoveInvaders()
        {
            if (framesSkipped > 0)
            {
                currentFrame++;
                if (currentFrame >= framesSkipped)
                    currentFrame = 0;
                else
                    return;
            }
            if (invadersMoveDown && directionInvadersJustMoveIn != Direction.Down)
            {
                invaderDirection = (invaderDirection == Direction.Left) ? Direction.Right : Direction.Left;
                foreach (Invader invader in invaders)
                    invader.Move(Direction.Down);
                directionInvadersJustMoveIn = Direction.Down;
            }
            else
            {
                foreach (Invader invader in invaders)
                    invader.Move(invaderDirection);
                directionInvadersJustMoveIn = invaderDirection;
            }
        }

        private void ReturnFire()
        {
            if (enemyShots.Count >= wave + 1) return;
            if (random.Next(10) < wave) return;
            var invaderGroups = from invader in invaders
                                group invader by invader.Location.X
                                    into invaderGroup
                                    orderby invaderGroup.Key descending
                                    select invaderGroup;
            var chosenInvaderGroup = invaderGroups.ToList()[random.Next(invaderGroups.ToList().Count - 1)];
            Invader chosenInvader = chosenInvaderGroup.Last() as Invader;
            if (wave >= 5 && random.Next(5) <= 1)
                enemyShots.Add(new Shot(new Point(chosenInvader.Location.X + (int)(chosenInvader.Hitbox.Width / 2),
                    chosenInvader.Location.Y + chosenInvader.Hitbox.Height), Direction.Down));
            else
                enemyShots.Add(new Shot(new Point(chosenInvader.Location.X + (int)(chosenInvader.Hitbox.Width / 2),
                    chosenInvader.Location.Y + chosenInvader.Hitbox.Height), Direction.Down));
        }

        public void AddScore(int value)
        {
            Score += value;
        }

        private void OnGameOver()
        {
            wave = 0;
            framesSkipped = 0;
            livesLeft = 2;
            GameOver = false;
            NextWave();
        }
    }
}
