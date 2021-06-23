using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gladiator_Ships
{
    class Team : DrawObject
    {
        private Color teamcol;
        public Ship design;
        public List<Ship> ships;
        private Random rand;
        private AIType type;

        public Team(int money, int seed)
        {
            rand = new Random(seed);
            int colortrack = 300;
            int r = rand.Next(255);
            colortrack -= r;
            int g = rand.Next(colortrack > 255 ? 255 : colortrack);
            colortrack -= g;
            int b = colortrack > 255 ? 255 : colortrack;
            teamcol = Color.FromArgb(r, g, b);

            do
            {
                design = generate();
            } while (design.cost > money);
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

        public void initships(int money)
        {
            ships = new List<Ship>();
            for (int i = 0; i < money / design.cost; i++)
            {
                ships.Add(new Ship(design, rand.Next(10000), type));
            }
        }

        public void draw(graphicsbox g)
        {
            foreach (Ship s in ships)
                s.draw(g);
        }

        public void clear(graphicsbox g)
        {
            foreach (Ship s in ships)
                s.clear(g);
        }

        public void runai(Team enemy)
        {
            foreach (Ship s in ships)
            {
                s.brain.runai(enemy);
            }
        }

        public void move_phys(graphicsbox g)
        {
            foreach (Ship s in ships)
            {
                s.move_phys(g);
            }
        }

        private Ship generate()
        {
            PartType[,] shipboard = new PartType[19, 19];
            shipboard[9, 9] = PartType.Engine;
            for (int i = 0; i < rand.Next(0, 20); i++)
            {
                fillbox(shipboard);
            }

            for (int i = 0; i < rand.Next(0,20); i++)
            {
                branch(shipboard);
            }
            double[,] angles = new double[19, 19];
            changetype(shipboard, angles);
            for (int i = 0; i < rand.Next(1, 20); i++)
            {
                placegun(shipboard, angles);
            }
            shipboard[9, 9] = PartType.Engine;
            
            Ship newshipdesign = new Ship((rand.NextDouble() + 0.5) * 3, teamcol);
            for (int x = 0; x < 19; x++)
            {
                for (int y = 0; y < 19; y++)
                {
                    if (shipboard[x, y] != PartType.Empty)
                        newshipdesign.parts.Add(new ShipPart(shipboard[x,y], new Point(x - 9, y - 9), angles[x,y], newshipdesign, rand.Next(10000)));
                }
            }
            newshipdesign.setpartneighbors();
            return newshipdesign;
        }


        private void setsquare(int x, int y, PartType[,] shipboard)
        {
            if (x >= 0 && x < 19 && y > 0 && y < 19)
                shipboard[x, y] = PartType.Square;
            if ((18 - x) >= 0 && (18 - x) < 19 && y > 0 && y < 19)
                shipboard[18 - x, y] = PartType.Square;
        }

        private void setcircle(int x, int y, PartType[,] shipboard)
        {
            if (x >= 0 && x < 19 && y > 0 && y < 19)
                shipboard[x, y] = PartType.Circle;
            if ((18 - x) >= 0 && (18 - x) < 19 && y > 0 && y < 19)
                shipboard[18 - x, y] = PartType.Circle;
        }

        private void settriangle(int x, int y, PartType[,] shipboard, double[,] angles)
        {
            double angle;
            if (x == 9)
            {
                angle = rand.Next(2) * (Math.PI);
            }
            else
                angle = rand.Next(4) * (Math.PI / 2);
            if (x >= 0 && x < 19 && y > 0 && y < 19)
            {
                shipboard[x, y] = PartType.Triangle;
                angles[x, y] = angle;
            }
            if ((18 - x) >= 0 && (18 - x) < 19 && y > 0 && y < 19)
            {
                shipboard[18 - x, y] = PartType.Triangle;
                angles[18 - x, y] = (angle + (angle % Math.PI == 0 ? 0 : Math.PI)) % (Math.PI * 2);
            }
        }

        private void placegun(PartType[,] shipboard, double[,] angles)
        {
            Point spot = new Point(0,0);
            int error = 0;
            while (true && error < 100)
            {
                spot = new Point(rand.Next(9, 19), rand.Next(19));
                if (countneighbors_nocorner(spot, shipboard) > 0 && shipboard[spot.X, spot.Y] == PartType.Empty)
                    break;
                error++;
            }
            if (error >= 100)
                return;
            double angle = 0;
            if (spot.Y + 1 > 0 && spot.Y + 1 < 19)
            {
                if (shipboard[spot.X, spot.Y + 1] != PartType.Empty && shipboard[spot.X, spot.Y + 1] != PartType.Gun)
                    angle = Math.PI;
            }
            if (spot.X - 1 > 0 && spot.X - 1 < 19)
            {
                if (shipboard[spot.X - 1, spot.Y] != PartType.Empty && shipboard[spot.X - 1, spot.Y] != PartType.Gun)
                    angle = - Math.PI  - (Math.PI / 2);
            }
            if (spot.X + 1 > 0 && spot.X + 1 < 19)
            {
                if (shipboard[spot.X + 1, spot.Y] != PartType.Empty && shipboard[spot.X + 1, spot.Y] != PartType.Gun)
                    angle = Math.PI + (Math.PI / 2);
            }
            if (spot.Y - 1 > 0 && spot.Y - 1 < 19)
            {
                if (shipboard[spot.X, spot.Y - 1] != PartType.Empty && shipboard[spot.X, spot.Y - 1] != PartType.Gun)
                    angle = 0;
            }

            if (spot.X >= 0 && spot.X < 19 && spot.Y > 0 && spot.Y < 19)
            {
                shipboard[spot.X, spot.Y] = PartType.Gun;
                angles[spot.X, spot.Y] = angle;
            }
            if ((18 - spot.X) >= 0 && (18 - spot.X) < 19 && spot.Y > 0 && spot.Y < 19)
            {
                shipboard[18 - spot.X, spot.Y] = PartType.Gun;
                angles[18 - spot.X, spot.Y] = (angle + (angle % Math.PI == 0 ? 0 : Math.PI)) % (Math.PI * 2);
            }
        }

        private void changetype(PartType[,] shipboard, double[,] angles)
        {
            for (int i = 0; i < rand.Next(0, 20); i++)
            {
                Point spot = new Point(0, 0);
                int error = 0;
                while (error < 100)
                {
                    spot = new Point(rand.Next(9, 18), rand.Next(18));
                    if (shipboard[spot.X, spot.Y] != PartType.Empty)
                        break;
                    error++;
                }
                if (error >= 100)
                    return;
                setcircle(spot.X, spot.Y, shipboard);
            }

            for (int i = 0; i < rand.Next(0, 20); i++)
            {
                Point spot;
                int error = 0;
                while (error < 100)
                {
                    spot = new Point(rand.Next(9, 19), rand.Next(19));
                    if (shipboard[spot.X, spot.Y] != PartType.Empty)
                    {
                        if (countneighbors(spot, shipboard) == 2)
                            settriangle(spot.X, spot.Y, shipboard, angles);
                        break;
                    }
                    error++;
                }
            }
        }

        private int countneighbors(Point pos, PartType[,] shipboard)
        {
            int count = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (pos.X + x > 0 && pos.X + x < 19 && pos.Y + y > 0 && pos.Y + y < 19)
                    {
                        if (shipboard[pos.X + x, pos.Y + y] != PartType.Empty)
                            count++;
                    }
                }
            }
            return count;
        }

        private int countneighbors_nocorner(Point pos, PartType[,] shipboard)
        {
            int count = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (Math.Abs(x) + Math.Abs(y) != 2)
                    {
                        if (pos.X + x > 0 && pos.X + x < 19 && pos.Y + y > 0 && pos.Y + y < 19)
                        {
                            if (shipboard[pos.X + x, pos.Y + y] != PartType.Empty && shipboard[pos.X + x, pos.Y + y] != PartType.Gun)
                                count++;
                        }
                    }
                }
            }
            return count;
        }

        private void fillbox(PartType[,] shipboard)
        {
            Point spot = new Point(0, 0);
            int error = 0;
            while (error < 100)
            {
                spot = new Point(rand.Next(9, 19), rand.Next(19));
                if (shipboard[spot.X, spot.Y] != PartType.Empty)
                    break;
                error++;
            }
            if (error >= 100)
                return;
            int dir = rand.Next(10);
            int length = rand.Next(19);
            for (int x = spot.X; x < rand.Next(spot.X, 19); x++)
            {
                if (dir < 5)
                {
                    for (int y = spot.Y; y < spot.Y + length; y++)
                    {
                        setsquare(x, y, shipboard);
                    }
                }
                else
                {
                    for (int y = spot.Y; y > spot.Y - length; y--)
                    {
                        setsquare(x, y, shipboard);
                    }
                }
            }
        }

        private void branch(PartType[,] shipboard)
        {
            Point spot = new Point(0, 0);
            int error = 0;
            while(error < 100)
            {
                spot = new Point(rand.Next(9,18), rand.Next(19));
                if (shipboard[spot.X, spot.Y] != PartType.Empty)
                    break;
                error++;
            }
            if (error >= 100)
                return;
            int length = rand.Next(2, 5);
            switch (rand.Next(3))
            {
                case 0:
                    for (int i = 0; i < length; i++)
                        setsquare(spot.X, spot.Y + i, shipboard);
                    break;
                case 1:
                    for (int i = 0; i < length; i++)
                        setsquare(spot.X, spot.Y - i, shipboard);
                    break;
                case 2:
                    for (int i = 0; i < length; i++)
                        setsquare(spot.X + i, spot.Y, shipboard);
                    break;
            }
        }
    }
}
