using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gladiator_Ships
{
    public partial class Form1 : Form
    {
        Game gm;

        public Form1()
        {
            InitializeComponent();
            gm = new Game(2000);
            gm.g = new graphicsbox(picturebox, Color.FromArgb(20, 0, 40));
            timer.Start();
            stopfight.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            gm.run();
            if (gm.rs)
            {
                restarttimer.Start();
                stopfight.Stop();
            }
        }

        private void restarttimer_Tick(object sender, EventArgs e)
        {
            gm.reset();
            restarttimer.Stop();
            stopfight.Start();
        }

        private void stopfight_Tick(object sender, EventArgs e)
        {
            int team1parts = 0;
            int team2parts = 0;
            foreach (Ship s in gm.team1.ships)
            {
                team1parts += s.parts.Count();
            }
            foreach (Ship s in gm.team2.ships)
            {
                team2parts += s.parts.Count();
            }

            List<Ship> destroyships = (team1parts > team2parts) ? gm.team2.ships : gm.team1.ships;

            foreach (Ship s in destroyships)
            {
                foreach (ShipPart p in s.parts)
                {
                    p.lives = 0;
                }
                gm.boom(s);
            }
        }
    }
}
