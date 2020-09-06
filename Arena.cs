using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameJamSubmission
{
    class Arena : PictureBox
    {
        public Arena()
        {
            InitializeArena();
        }

        private void InitializeArena()
        {
            this.BackColor = Color.Black;
            this.Size = new Size(800, 500);
            this.Location = new Point(0, 0);
        }
    }
}
