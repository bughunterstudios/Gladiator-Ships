using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gladiator_Ships
{
    class Bullet : DrawObject
    {
        public double posx;
        public double posy;
        public double direction;
        public Color col;
        public Ship parent;
        public Physics phys;

        public Bullet(double posx, double posy, double direction, Ship parent)
        {
            this.posx = posx;
            this.posy = posy;
            this.direction = direction;
            this.parent = parent;
            col = parent.col;
            phys = new Physics(100);
            phys.dx = parent.phys.dx;
            phys.dy = parent.phys.dy;
            phys.accelerate(direction, 3);
        }

        public void move_phys()
        {
            posx += phys.dx;
            posy += phys.dy;
            direction += phys.dtheta;
        }

        public void draw(graphicsbox g)
        {
            g.g.DrawLine(new Pen(new SolidBrush(col), 2), (float)posx, (float)posy, (float)(posx + 2 * Math.Cos(direction)), (float)(posy + 2 * Math.Sin(direction)));
        }

        public void clear(graphicsbox g)
        {
            g.g.FillRectangle(new SolidBrush(g.background), (float) posx - 4, (float)posy - 4, 8, 8);
        }
    }
}
