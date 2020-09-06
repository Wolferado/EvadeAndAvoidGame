using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameJamSubmission
{
    class Hero : PictureBox
    {
        public int HorizontalVelocity { get; set; } = 0;
        public int Step { get; set; } = 8;
        public string Direction { get; set; } = "right";
        public int Jump { get; } = 60;
        public int GravityVelocity { get; set; } = 5;
        public Hero()
        {
            InitializeHero();
        }

        private void InitializeHero()
        {
            this.Size = new Size(40, 40);
            this.BackColor = Color.Transparent;
        }
    }
}
