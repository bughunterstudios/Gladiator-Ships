using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gladiator_Ships
{
    class ShipBit : DrawObject
    {
        public double posx;
        public double posy;
        private double direction;
        private Color col;
        private int length;
        private Physics phys;

        public ShipBit(PointF one, PointF two, ShipPart parentpart, Ship parentship, int seed)
        {
            col = parentship.col;
            posx = ((two.X - one.X) / 2) + one.X;
            posy = ((two.Y - one.Y) / 2) + one.Y;
            direction = Math.Atan2(two.X - one.X, two.Y - one.Y);
            length = (int) (Math.Sqrt(Math.Pow(two.X - one.X, 2) + Math.Pow(two.Y - one.Y, 2)) / 2.0);

            phys = parentship.phys.copy();
            phys.maxspeed = 100;
            double launchdirection = Math.Atan2(parentpart.posy - posy, parentpart.posx - posx);
            Random rand = new Random(seed);
            launchdirection += Math.PI + ((rand.NextDouble() - 0.5) * (Math.PI / 8));
            phys.accelerate(launchdirection, ((rand.NextDouble() / 2) + 0.5) * 3);
            phys.dtheta = (rand.NextDouble() - 0.5);
        }

        public void move_phys()
        {
            posx += phys.dx;
            posy += phys.dy;
            direction += phys.dtheta;
        }

        public void draw(graphicsbox g)
        {
            g.g.DrawLine(new Pen(new SolidBrush(col), 2), (float)(posx - length * Math.Cos(direction)), (float)(posy - length * Math.Sin(direction)), (float)(posx + length * Math.Cos(direction)), (float)(posy + length * Math.Sin(direction)));
        }

        public void clear(graphicsbox g)
        {
            g.g.DrawLine(new Pen(new SolidBrush(g.background), 4), (float)(posx - (length + 2) * Math.Cos(direction)), (float)(posy - (length + 2) * Math.Sin(direction)), (float)(posx + (length + 2) * Math.Cos(direction)), (float)(posy + (length + 2) * Math.Sin(direction)));
        }
    }
}
