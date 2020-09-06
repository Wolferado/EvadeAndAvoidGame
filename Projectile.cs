using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameJamSubmission.Properties;

namespace GameJamSubmission
{
    class Projectile : PictureBox
    {
        Timer animateProjectileTimer = new Timer();
        private int spriteCounter = 1;
        public int FallingSpeed { get; set; } = 10;
        public Projectile()
        {
            InitializeFire();
            InitializeProjectileAnimation();
        }

        private void InitializeFire()
        {
            this.Size = new Size(20, 40);
            this.BackColor = Color.Transparent;
        }

        private void InitializeProjectileAnimation()
        {
            animateProjectileTimer.Interval = 200;
            animateProjectileTimer.Tick += AnimateProjectile_Tick;
            animateProjectileTimer.Start();
        }

        private void AnimateProjectile_Tick(object sender, EventArgs e)
        {
            AnimateProjectiles();
        }

        private void AnimateProjectiles()
        {
            string imageName = "Projectile_" + spriteCounter.ToString() + "_w";
            this.Image = (Image)Resources.ResourceManager.GetObject(imageName);
            spriteCounter++;
            if(spriteCounter > 2)
            {
                spriteCounter = 1;
            }
        }
    }
}
