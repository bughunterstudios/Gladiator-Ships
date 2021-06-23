using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gladiator_Ships
{
    class Physics
    {
        public double dx;
        public double dy;
        public double dtheta;
        public double maxspeed;

        public Physics()
        {

        }

        public Physics(double maxspeed)
        {
            dx = 0;
            dy = 0;
            dtheta = 0;
            this.maxspeed = maxspeed;
        }

        public void accelerate(double direction, double val)
        {
            dx += val * Math.Cos(direction);
            dy += val * Math.Sin(direction);
            if (!goodspeed())
            {
                dx -= val * Math.Cos(direction);
                dy -= val * Math.Sin(direction);
            }
        }

        private bool goodspeed()
        {
            if (getspeed() < maxspeed)
                return true;
            return false;
        }

        public double getspeed()
        {
            return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        }

        public Physics copy()
        {
            Physics newphys = new Physics();
            newphys.dx = this.dx;
            newphys.dy = this.dy;
            newphys.dtheta = this.dtheta;
            newphys.maxspeed = this.maxspeed;
            return newphys;
        }
    }
}
