using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gladiator_Ships
{
    public enum AIType
    {
        kamikaze, orbitleft, orbitright, chase, run
    }

    class AI
    {
        Ship target;
        PointF targetpos;
        AIType type;
        int orbitdistance;
        Random rand;
        Ship parent;

        public AI(AIType type, Ship parent, int seed)
        {
            this.type = type;
            this.parent = parent;
            rand = new Random(seed);
        }

        public void runai(Team enemy)
        {
            if (rand.Next(30) == 0)
            {
                switch (rand.Next(5))
                {
                    case 0:
                        type = AIType.chase;
                        break;
                    case 1:
                        type = AIType.kamikaze;
                        break;
                    case 2:
                        type = AIType.orbitleft;
                        break;
                    case 3:
                        type = AIType.orbitright;
                        break;
                    case 4:
                        type = AIType.run;
                        break;
                }
            }

            checkcollide(enemy);

            if (countguns(parent) == 0)
                type = AIType.kamikaze;
            if (target == null)
                gettarget(enemy);
            if (target.parts.Count < 1)
                gettarget(enemy);

            setpos();
            parent.turntowards(targetpos);
            parent.accelerate();
        }

        public void checkcollide(Team enemy)
        {
            foreach (Ship s in enemy.ships)
            {
                if (type != AIType.kamikaze && Math.Sqrt(Math.Pow(s.posx - parent.posx, 2) + Math.Pow(s.posy - parent.posy, 2)) < 50)
                {
                    target = s;
                    type = AIType.run;
                }
            }
        }

        public void gettarget(Team enemy)
        {
            List<Ship> options = new List<Ship>();
            foreach (Ship s in enemy.ships)
            {
                for (int i = 0; i < countguns(s) + 1; i++)
                {
                    options.Add(s);
                }
            }
            if (options.Count > 0)
            {
                target = options[rand.Next(options.Count)];
                orbitdistance = rand.Next(7 * 5, 7 * 8);
            }
        }

        public void setpos()
        {
            double towards = Math.Atan2(target.posy - parent.posy, target.posx - parent.posx);
            double pi = Math.PI;
            double hpi = Math.PI / 2;
            switch (type)
            {
                case AIType.chase:
                    targetpos = new PointF((float)target.posx, (float)target.posy);
                    break;
                case AIType.kamikaze:
                    double distance = Math.Sqrt(Math.Pow(target.posx - parent.posx, 2) + Math.Pow(target.posy - parent.posy, 2));
                    double predictx = target.posx + (target.phys.dx * Math.Cos(target.direction)) * (distance / parent.phys.getspeed());
                    double predicty = target.posy + (target.phys.dy * Math.Sin(target.direction)) * (distance / parent.phys.getspeed());
                    targetpos = new PointF((float)predictx, (float)predicty);
                    break;
                case AIType.run:
                    targetpos = new PointF((float)(parent.posx + (orbitdistance * Math.Cos(towards + pi))), (float)(parent.posy + (orbitdistance * Math.Sin(towards + pi))));
                    break;
                case AIType.orbitleft:
                    targetpos = new PointF((float)(target.posx + (orbitdistance * Math.Cos(towards + hpi))), (float)(target.posy + (orbitdistance * Math.Sin(towards + hpi))));
                    break;
                case AIType.orbitright:
                    targetpos = new PointF((float)(target.posx + (orbitdistance * Math.Cos(towards - hpi))), (float)(target.posy + (orbitdistance * Math.Sin(towards - hpi))));
                    break;
            }
        }

        private int countguns(Ship s)
        {
            int guncount = 0;
            foreach (ShipPart p in s.parts)
            {
                if (p.type == PartType.Gun)
                    guncount++;
            }
            return guncount;
        }
    }
}
