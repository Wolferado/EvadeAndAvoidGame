using GameJamSubmission.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace GameJamSubmission
{
    public partial class EvadeAndAvoid : Form
    {
        Arena arena = new Arena();
        Hero hero = new Hero();
        Random rand = new Random();
        Label timerLabel = new Label();
        Label tutorialLabel = new Label();
        PictureBox gameLogo = new PictureBox();

        Button startGameButton = new Button();
        Button exitGameButton = new Button();
        Button howToPlayButton = new Button();
        Button retryButton = new Button();
        Button mainMenuButton = new Button();

        Platform leftPlatform = new Platform();
        Platform centerPlatform = new Platform();
        Platform rightPlatform = new Platform();

        CollisionDetector leftDetector = new CollisionDetector();
        CollisionDetector centerDetector = new CollisionDetector();
        CollisionDetector rightDetector = new CollisionDetector();

        List<Projectile> projectiles = new List<Projectile>();
        List<Platform> platforms = new List<Platform>();
        List<Button> buttons = new List<Button>();

        Timer gravityTimer = new Timer();
        Timer collisionTimer = new Timer();
        Timer projectileMoveTimer = new Timer();
        Timer gameTimer = new Timer();
        Timer moveTimer = new Timer();
        Timer gameOverTimer = new Timer();
        Timer readTimer = new Timer();

        private bool firstJump = true;
        private bool secondJump = true;
        private bool sKeyPressed = false;
        private int initialProjectileCount = 6;
        private int gameOverSpriteCounter = 1;
        private int readLineCount = 0;

        //Variables for in-game timer
        private int decSeconds = 0;
        private int seconds = 0;
        private int minutes = 0;
        private int hours = 0;

        public EvadeAndAvoid()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            StartGame();
            this.BackColor = Color.Black;
        }

        private void StartGame()
        {
            this.Text = "Evade and Avoid";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;

            this.Controls.Add(startGameButton);
            buttons.Add(startGameButton);
            startGameButton.Click += BeginGame;
            startGameButton.Image = (Image)Resources.ButtonStartGame;
            startGameButton.Size = new Size(75, 25);
            startGameButton.Location = new Point(150, 270);

            this.Controls.Add(howToPlayButton);
            buttons.Add(howToPlayButton);
            howToPlayButton.Click += Tutorial;
            howToPlayButton.Image = (Image)Resources.ButtonHowToPlay;
            howToPlayButton.Size = new Size(75, 25);
            howToPlayButton.Location = new Point(175, 300);

            this.Controls.Add(exitGameButton);
            buttons.Add(exitGameButton);
            exitGameButton.Click += ExitGame;
            exitGameButton.Image = (Image)Resources.ButtonExit;
            exitGameButton.Size = new Size(75, 25);
            exitGameButton.Location = new Point(200, 330);

            this.Controls.Add(gameLogo);
            gameLogo.Image = (Image)Resources.Title;
            gameLogo.Size = new Size(400, 200);
            gameLogo.Location = new Point(50, 10);
            gameLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            gameLogo.BringToFront();
        }

        private void BeginGame(object sender, EventArgs e)
        {
            foreach(var button in buttons)
            {
                this.Controls.Remove(button);
            }

            this.Controls.Remove(gameLogo);
            InitializeGravityTimer();
            InitializeCollisionTimer();
            InitializeProjectileMoveTimer();
            InitializeMoveTimer();

            this.KeyDown += HeroControlWASD;
            this.KeyUp += KeyUp_Control;

            AddArena();
            AddPlatforms();
            AddCollisionDetectors();
            AddHero();
            AddProjectiles();

            InitializeGameTimer();
        }

        private void InitializeReadTimer()
        {
            readTimer.Interval = 2000;
            readTimer.Tick += ReadTimer_Tick;
            readTimer.Start();
        }

        private void ReadTimer_Tick(object sender, EventArgs e)
        {
            if(readLineCount == 0)
            {
                tutorialLabel.Text = "Your main goal is to survive as long as you can.";
            }
            else if(readLineCount == 1)
            {
                tutorialLabel.Text = "You are able to slide to both sides and make double jumps.";
            }
            else if (readLineCount == 2)
            {
                tutorialLabel.Text = "Evade and avoid projectiles.";
            }
            else if(readLineCount == 3)
            {
                tutorialLabel.Text = "Good Luck!";
            }
            else
            {
                readTimer.Stop();
                tutorialLabel.Hide();
                AddProjectiles();
                InitializeProjectileMoveTimer();
                InitializeGameTimer();
            }
            readLineCount++;
        }

        private void Tutorial(object sender, EventArgs e) 
        {
            foreach (var button in buttons)
            {
                this.Controls.Remove(button);
            }

            gameLogo.Hide();
            InitializeReadTimer();
            InitializeGravityTimer();
            InitializeCollisionTimer();
            InitializeMoveTimer();

            this.KeyDown += HeroControlWASD;
            this.KeyUp += KeyUp_Control;

            AddArena();
            AddPlatforms();
            AddCollisionDetectors();
            AddHero();

            this.Controls.Add(tutorialLabel);
            tutorialLabel.Font = new Font("Consolas", 15f);
            tutorialLabel.Size = new Size(750, 150);
            tutorialLabel.ForeColor = Color.White;
            tutorialLabel.BackColor = Color.Transparent;
            tutorialLabel.Location = new Point(75, 50);
            tutorialLabel.Parent = arena;
            tutorialLabel.BringToFront();
        }

        private void AddArena()
        {
            this.Controls.Add(arena);
            arena.Image = Resources.Arena;
            arena.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void AddHero()
        {
            this.Controls.Add(hero);
            hero.Parent = arena;
            hero.BringToFront();
            hero.Location = new Point(400, 250);
        }

        private void AddPlatforms()
        {
            this.Controls.Add(leftPlatform);
            leftPlatform.BackColor = Color.Transparent;
            platforms.Add(leftPlatform);
            leftPlatform.Parent = arena;
            leftPlatform.Location = new Point(50, 400);
            leftPlatform.Size = new Size(150, 25);

            this.Controls.Add(centerPlatform);
            centerPlatform.BackColor = Color.Transparent;
            platforms.Add(centerPlatform);
            centerPlatform.Parent = arena;
            centerPlatform.Location = new Point(250, 350);
            centerPlatform.Size = new Size(300, 30);

            this.Controls.Add(rightPlatform);
            rightPlatform.BackColor = Color.Transparent;
            platforms.Add(rightPlatform);
            rightPlatform.Parent = arena;
            rightPlatform.Location = new Point(600, 400);
            rightPlatform.Size = new Size(150, 25);
        }

        private void AddCollisionDetectors()
        {
            leftDetector.Location = new Point(55, 400);
            leftDetector.Size = new Size(130, 0);
            leftDetector.Parent = leftPlatform;

            centerDetector.Location = new Point(255, 350);
            centerDetector.Size = new Size(290, 0);
            centerDetector.Parent = centerPlatform;

            rightDetector.Location = new Point(605, 400);
            rightDetector.Size = new Size(140, 1);
            rightDetector.Parent = rightPlatform;
        }

        private void AddProjectiles()
        {
            var test = Graphics.FromImage((Image)Resources.CenterPlatform_w);
            Projectile projectile;
            for(int i = 0; i < initialProjectileCount; i++)
            {
                projectile = new Projectile();
                projectiles.Add(projectile);
                projectiles[i].Location = new Point(rand.Next(0, 40) * 20, rand.Next(-100, -20));
                this.Controls.Add(projectile);
                projectile.Parent = arena;
                projectile.BringToFront();
            }
        }

        private void InitializeGameTimer()
        {
            timerLabel.Font = new Font("Consolas", 12f);
            timerLabel.ForeColor = Color.White;
            timerLabel.BackColor = Color.Transparent;
            timerLabel.Location = new Point(363, 75);
            timerLabel.Parent = arena;
            timerLabel.BringToFront();

            gameTimer.Interval = 90;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            decSeconds += 1;

            if(decSeconds == 10)
            {
                seconds++;
                decSeconds = 0;
            }
            else if(seconds == 60)
            {
                minutes++;
                seconds = 0;
            }
            else if(minutes == 60)
            {
                hours++;
                minutes = 0;
            }

            timerLabel.Text = hours + ":" + minutes + ":" + seconds + "." + decSeconds;
        }

        private void InitializeProjectileMoveTimer()
        {
            projectileMoveTimer.Interval = 100;
            projectileMoveTimer.Tick += ProjectileMoveTimer_Tick;
            projectileMoveTimer.Start();
        }

        private void ProjectileMoveTimer_Tick(object sender, EventArgs e)
        {
            foreach(var projectile in projectiles)
            {
                projectile.Top += projectile.FallingSpeed;
                CheckForGameOver();
                if(projectile.Top > this.Height + projectile.Width)
                {
                    projectile.Location = new Point(rand.Next(0, 40) * 20, rand.Next(-100, -20));
                    if(projectile.FallingSpeed < 20)
                    {
                        projectile.FallingSpeed += 2;
                    }
                }
            }
        }

        private void InitializeCollisionTimer()
        {
            collisionTimer.Interval = 1;
            collisionTimer.Tick += HeroPlatformCollision;
            collisionTimer.Start();
        }

        private void HeroPlatformCollision(object sender, EventArgs e)
        {
            if (hero.Bounds.IntersectsWith(leftDetector.Bounds))
            {
                gravityTimer.Stop();
                firstJump = false;
                secondJump = false;
                hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_" + hero.Direction);
            }
            else if (hero.Bounds.IntersectsWith(centerDetector.Bounds))
            {
                gravityTimer.Stop();
                firstJump = false;
                secondJump = false;
                hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_" + hero.Direction);
            }
            else if (hero.Bounds.IntersectsWith(rightDetector.Bounds))
            {
                gravityTimer.Stop();
                firstJump = false;
                secondJump = false;
                hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_" + hero.Direction);
            }
            else
            {
                gravityTimer.Start();
            }
        }

        private void InitializeGravityTimer()
        {
            gravityTimer.Interval = 50;
            gravityTimer.Tick += GravityTimer_Tick;
            gravityTimer.Start();
        }

        private void GravityTimer_Tick(object sender, EventArgs e)
        {
            hero.Top += hero.GravityVelocity;
            hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_jump_" + hero.Direction);
        }

        private void InitializeMoveTimer()
        {
            moveTimer.Interval = 100;
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            hero.Left += hero.HorizontalVelocity;
        }

        private void HeroControlWASD(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    if(firstJump == true && secondJump == true)
                    {
                        hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_jump_" + hero.Direction);
                    }
                    else if (firstJump == false)
                    {
                        hero.Top -= hero.Jump;
                        firstJump = true;
                        hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_jump_" + hero.Direction);
                    }
                    else if (secondJump == false)
                    {
                        hero.Top -= hero.Jump;
                        secondJump = true;
                        hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_jump_" + hero.Direction);
                    }
                    break;
                case Keys.S:
                    if (sKeyPressed == true)
                    {

                    }
                    else
                    {
                        hero.HorizontalVelocity = 0;
                        hero.Size = new Size(40, 20);
                        hero.Location = new Point(hero.Left, hero.Top + 20);
                        sKeyPressed = true;
                        hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_down_" + hero.Direction);
                    }
                    break;
                case Keys.A:
                    //hero.Left -= hero.Step;
                    hero.HorizontalVelocity = -hero.Step;
                    hero.Direction = "left";
                    hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_" + hero.Direction);
                    break;
                case Keys.D:
                    //hero.Left += hero.Step;
                    hero.HorizontalVelocity = hero.Step;
                    hero.Direction = "right";
                    hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_" + hero.Direction);
                    break;
            }
        }

        private void KeyUp_Control(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.S:
                    hero.Size = new Size(40, 40);
                    hero.Location = new Point(hero.Left, hero.Top - 20);
                    sKeyPressed = false;
                    hero.Image = (Image)Resources.ResourceManager.GetObject("Hero_" + hero.Direction);
                    break;
            }
        }

        private void CheckForGameOver()
        {
            foreach(var projectile in projectiles)
            {
                if (hero.Bounds.IntersectsWith(projectile.Bounds))
                {
                    GameOver();
                }
            }

            if(hero.Top > this.Height)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            gravityTimer.Stop();
            collisionTimer.Stop();
            projectileMoveTimer.Stop();
            moveTimer.Stop();
            gameTimer.Stop();
            arena.BringToFront();
            arena.Image = (Image)Resources.BG_b;
            this.KeyDown -= HeroControlWASD;
            this.KeyUp -= KeyUp_Control;

            foreach(var platform in platforms)
            {
                platform.Parent = null;
            }

            foreach(var projectile in projectiles)
            {
                projectile.Parent = null;
            }

            InitializeGameOverTimer();
        }

        private void InitializeGameOverTimer()
        {
            gameOverTimer.Interval = 100;
            gameOverTimer.Tick += GameOverTimer_Tick;
            gameOverTimer.Start();
        }

        private void GameOverTimer_Tick(object sender, EventArgs e)
        {
            DeathScene();
        }

        private void DeathScene()
        {
            hero.Image = (Image)Resources.ResourceManager.GetObject("GameOver_" + hero.Direction + "_" + gameOverSpriteCounter.ToString());
            gameOverSpriteCounter++;
            if (gameOverSpriteCounter > 9)
            {
                gameOverTimer.Stop();
                AfterMath();
            }
        }

        private void AfterMath()
        {
            Label result = new Label();
            result.Font = new Font("Consolas", 12f);
            result.Text = "You have lasted for " + hours + " hours, " + minutes + " minutes, " + seconds + " seconds and " + decSeconds + " decimal seconds.";
            result.Size = new Size(800, 200);
            result.Location = new Point(35, 20);
            result.Parent = arena;
            result.ForeColor = Color.White;
            result.BackColor = Color.Transparent;
            result.BringToFront();

            this.Controls.Add(mainMenuButton);
            buttons.Add(mainMenuButton);
            mainMenuButton.Click += GoToMainMenu;
            mainMenuButton.Image = (Image)Resources.ButtonMainMenu;
            mainMenuButton.Size = new Size(75, 25);
            mainMenuButton.Location = new Point(345, 350);
            mainMenuButton.BringToFront();
        }

        private void GoToMainMenu(object sender, EventArgs e)
        {
            Application.Restart();
        }
        private void ExitGame(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}