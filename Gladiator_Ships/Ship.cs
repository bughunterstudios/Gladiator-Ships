using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gladiator_Ships
{
    class Ship : DrawObject
    {
        public int cost;
        public double posx;
        public double posy;
        public double direction;
        public Color col;
        public Physics phys;
        public Random rand;
        public double speed = 0.1;
        public int turnspeed = 30;

        public List<ShipPart> parts;

        public AI brain;

        public Ship(double maxspeed, Color col)
        {
            cost = 0;
            parts = new List<ShipPart>();
            //parts.Add(new ShipPart(PartType.Engine, new Point(0,0), 0, this));
            phys = new Physics(maxspeed);
            cost += (int)(maxspeed * 10);
            this.col = col;
        }

        public Ship(Ship tocopy, int seed, AIType type)
        {
            rand = new Random(seed);
            cost = 0;
            posx = tocopy.posx;
            posy = tocopy.posy;
            direction = tocopy.direction;
            col = Color.FromArgb(tocopy.col.R, tocopy.col.G, tocopy.col.B);
            phys = tocopy.phys.copy();
            parts = new List<ShipPart>();
            foreach (ShipPart p in tocopy.parts)
            {
                parts.Add(p.copy(this, rand.Next(10000)));
            }
            setpartneighbors();
            brain = new AI(type, this, rand.Next(10000));
        }

        public void setpartneighbors()
        {
            foreach (ShipPart p in parts)
            {
                p.neighbors = new List<ShipPart>();
                foreach (ShipPart p2 in parts)
                {
                    if (p2.pos.X >= p.pos.X - 1 && p2.pos.X <= p.pos.X + 1 && p2.pos.Y >= p.pos.Y - 1 && p2.pos.Y <= p.pos.Y + 1)
                    {
                        if (Math.Abs(p2.pos.X - p.pos.X) + Math.Abs(p2.pos.Y - p.pos.Y) != 2)
                        {
                            if (p2 != p)
                                p.neighbors.Add(p2);
                        }
                    }
                }
            }
        }

        public void accelerate()
        {
            phys.accelerate(direction, speed);
        }

        public void deccelerate()
        {
            phys.accelerate(direction + Math.PI, speed);
        }

        public void turntowards(PointF targetpos)
        {
            turnspeed = (parts.Count()) + 3;

            double dx = targetpos.X - posx;
            double dy = targetpos.Y - posy;
            double d = dx / (Math.Sqrt(Math.Pow(dx, 2.0) + Math.Pow(dy, 2.0)));
            if (d >= -1 && d <= 1)
            {
                double towards = dy < 0 ? -Math.Acos(d) : Math.Acos(d);
                if (towards < 0)
                    towards += 2 * Math.PI;
                double delta = towards - direction;
                if (Math.Abs(delta) > Math.PI)
                {
                    delta = -(2 * Math.PI) + Math.Abs(delta);
                }
                direction += (delta / turnspeed);
            }
        }

        public void move_phys(graphicsbox g)
        {
            posx += phys.dx;
            posy += phys.dy;
            direction += phys.dtheta;
            direction = direction % (Math.PI * 2);
            if (posx < 0)
                posx = g.pbox.Width;
            if (posx > g.pbox.Width)
                posx = 0;
            if (posy < 0)
                posy = g.pbox.Height;
            if (posy > g.pbox.Height)
                posy = 0;
        }

        public void draw(graphicsbox g)
        {
            foreach (ShipPart p in parts)
            {
                p.draw(g);
            }
        }

        public void clear(graphicsbox g)
        {
            foreach (ShipPart p in parts)
            {
                p.clear(g);
            }
        }
    }
}
