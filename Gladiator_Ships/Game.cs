using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gladiator_Ships
{
    class Game
    {
        int money;
        public graphicsbox g;
        public Team team1;
        public Team team2;
        public List<Bullet> bullets;
        public List<ShipBit> debris;
        Random rand;

        public bool rs;

        public Game(int money)
        {
            this.money = money;
            rand = new Random();
            rs = false;
            team1 = null;
            team2 = null;
        }

        public void run()
        {
            checkreset();
            team1.clear(g);
            team2.clear(g);
            foreach (Bullet b in bullets)
            {
                b.clear(g);
            }
            foreach (ShipBit s in debris)
            {
                s.clear(g);
            }

            team1.move_phys(g);
            team2.move_phys(g);
            team1.runai(team2);
            team2.runai(team1);
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i] != null)
                {
                    bullets[i].move_phys();
                    if (!g.inbounds(new Point((int)bullets[i].posx, (int)bullets[i].posy), 3))
                    {
                        bullets.Remove(bullets[i]);
                    }
                }
            }
            for (int i = 0; i < debris.Count; i++)
            {
                if (debris[i] != null)
                {
                    debris[i].move_phys();
                    if (!g.inbounds(new Point((int)debris[i].posx, (int)debris[i].posy), g.edge))
                    {
                        debris.Remove(debris[i]);
                    }
                }
            }

            calculatebullets();
            checkhits();
            checkcollide();
            checkdestroypart();
            checkdead();

            team1.draw(g);
            team2.draw(g);
            foreach (Bullet b in bullets)
            {
                b.draw(g);
            }
            foreach (ShipBit s in debris)
            {
                s.draw(g);
            }

            g.showscreen();
        }

        private void checkdead()
        {
            for (int i = 0; i < team1.ships.Count; i++)
            {
                if (team1.ships[i] != null)
                {
                    if (team1.ships[i].parts.Count < 1)
                    {
                        team1.ships.Remove(team1.ships[i]);
                    }
                }
            }

            for (int i = 0; i < team2.ships.Count; i++)
            {
                if (team2.ships[i] != null)
                {
                    if (team2.ships[i].parts.Count < 1)
                    {
                        team2.ships.Remove(team2.ships[i]);
                    }
                }
            }
        }

        private void checkreset()
        {
            if (team1 == null || team2 == null)
            {
                reset();
            }
            else
            {
                if (team1.ships != null && team2.ships != null)
                {
                    if (team1.ships.Count < 1)
                    {
                        rs = true;
                    }
                    if (team2.ships.Count < 1)
                    {
                        rs = true;
                    }
                }
            }
        }

        public void reset()
        {
            bool set1 = false;
            bool set2 = false;
            if (team1 == null || team2 == null)
            {
                set1 = true;
                set2 = true;
            }
            else
            {
                if (team1.ships != null && team2.ships != null)
                {
                    if (team1.ships.Count < 1)
                    {
                        set1 = true;
                    }
                    if (team2.ships.Count < 1)
                    {
                        set2 = true;
                    }
                }
            }

            if (set1 || set2)
            {
                if (team1 != null && team2 != null)
                {
                    team1.clear(g);
                    team2.clear(g);

                    foreach (Bullet b in bullets)
                    {
                        b.clear(g);
                    }
                    foreach (ShipBit s in debris)
                    {
                        s.clear(g);
                    }
                }

                if (set1)
                    reset1();
                if (set2)
                    reset2();
            }
            initships();
            rs = false;
        }

        private void checkhits()
        {
            foreach (Ship s in team1.ships.Concat(team2.ships))
            {
                for (int i = 0; i < s.parts.Count; i++)
                {
                    for (int j = 0; j < bullets.Count; j++)
                    {
                        if (s.parts[i] != null && bullets[j] != null)
                        {
                            if (s.col != bullets[j].col && s.parts[i].hitbox(g).Contains((int)bullets[j].posx, (int)bullets[j].posy))
                            {
                                s.parts[i].lives -= 2;
                                s.parts[i].flashon = true;
                                bullets.Remove(bullets[j]);
                            }
                        }
                    }
                }
            }
        }

        private void checkcollide()
        {
            foreach (Ship s1 in team1.ships)
            {
                foreach (Ship s2 in team2.ships)
                {
                    foreach (ShipPart p1 in s1.parts)
                    {
                        foreach (ShipPart p2 in s2.parts)
                        {
                            if (p1.hitbox(g).IntersectsWith(p2.hitbox(g)))
                            {
                                double tempdir = Math.Atan2(p2.posy - p1.posy, p2.posx - p1.posx); //towards p2
                                s1.phys.accelerate(tempdir + Math.PI, 0.1);
                                s2.phys.accelerate(tempdir, 0.1);
                                p1.lives--;
                                p2.lives--;
                                p1.flashon = true;
                                p2.flashon = true;
                                if (p2.type == PartType.Triangle && p1.type != PartType.Triangle)
                                    p1.lives -= 2;
                                if (p1.type == PartType.Triangle && p2.type != PartType.Triangle)
                                    p2.lives -= 2;
                            }
                        }
                    }
                }
            }
        }

        private void checkdestroypart()
        {
            foreach (Ship s in team1.ships.Concat(team2.ships))
            {
                checkconnected(s);
                for (int i = 0; i < s.parts.Count; i++)
                {
                    if (s.parts[i] != null)
                    {
                        if (s.parts[i].lives < 1)
                        {
                            if (s.parts[i].type == PartType.Engine)
                            {
                                boom(s);
                                break;
                            }
                            List<ShipBit> thebits = s.parts[i].getbits(g);
                            if (thebits != null)
                                debris.AddRange(thebits);
                            s.parts.Remove(s.parts[i]);
                            checkconnected(s);
                        }
                    }
                }
            }
        }

        public void boom(Ship s)
        {
            for (int i = 0; i < s.parts.Count; i++)
            {
                if (s.parts[i] != null)
                {
                    List<ShipBit> thebits = s.parts[i].getbits(g);
                    if (thebits != null)
                        debris.AddRange(thebits);
                    s.parts.Remove(s.parts[i]);
                }
            }
        }

        private void checkconnected(Ship s)
        {
            s.setpartneighbors();
            for (int i = 0; i < s.parts.Count; i++)
            {
                if (s.parts[i] != null)
                {
                    if (!s.parts[i].connected())
                    {
                        List<ShipBit> thebits = s.parts[i].getbits(g);
                        if (thebits != null)
                            debris.AddRange(thebits);
                        s.parts.Remove(s.parts[i]);
                        s.setpartneighbors();
                    }
                }
            }
        }

        private void calculatebullets()
        {
            foreach (Ship s in team1.ships)
            {
                foreach (ShipPart p in s.parts)
                {
                    if (p.type == PartType.Gun)
                    {
                        if (p.target == null || rand.Next(20) == 0 || p.target.lives < 1)
                            p.gettarget(team2);
                        Bullet newb = p.fire();
                        if (newb != null)
                            bullets.Add(newb);
                    }
                }
            }

            foreach (Ship s in team2.ships)
            {
                foreach (ShipPart p in s.parts)
                {
                    if (p.type == PartType.Gun)
                    {
                        if (p.target == null || rand.Next(20) == 0 || p.target.lives < 1)
                            p.gettarget(team1);
                        Bullet newb = p.fire();
                        if (newb != null)
                            bullets.Add(newb);
                    }
                }
            }
        }

        private void reset1()
        {
            team1 = new Team(money, rand.Next(10000));
        }

        private void reset2()
        {
            team2 = new Team(money, rand.Next(10000));
        }

        private void initships()
        {
            team1.initships(money);
            team2.initships(money);
            bullets = new List<Bullet>();
            debris = new List<ShipBit>();

            int i = 1;
            foreach (Ship s in team1.ships)
            {
                s.posx = 30;
                s.posy = (g.pbox.Height / (team1.ships.Count + 1)) * i;
                //s.phys.dtheta = (rand.NextDouble() - 0.5) / 10;
                i++;
            }

            i = 1;
            foreach (Ship s in team2.ships)
            {
                s.posx = g.pbox.Width - 30;
                s.direction = Math.PI;
                s.posy = (g.pbox.Height / (team2.ships.Count + 1)) * i;
                //s.phys.dtheta = (rand.NextDouble() - 0.5) / 10;
                i++;
            }
        }
    }
}
