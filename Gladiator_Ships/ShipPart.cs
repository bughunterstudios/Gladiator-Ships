using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gladiator_Ships
{
    public enum PartType
    {
        Empty, Triangle, Square, Circle, Engine, Gun,
    }

    class ShipPart : DrawObject
    {
        public PartType type;
        public Point pos;
        private double angle;
        private Ship parent;
        public List<ShipPart> neighbors;
        public double posx;
        public double posy;
        public int lives = 5;
        public Random rand;
        public bool flashon;
        private int flashcount;

        public ShipPart target;
        private int bulletcheck;
        private int alternate = 10;

        public ShipPart()
        {

        }

        public ShipPart(PartType type, Point pos, double angle, Ship parent, int seed)
        {
            this.type = type;
            this.pos = new Point(pos.X, pos.Y);
            this.angle = angle;
            this.lives = checklives();
            this.parent = parent;
            parent.cost += cost();
            rand = new Random(seed);
            bulletcheck = rand.Next(alternate);
        }

        public ShipPart copy(Ship newparent, int seed)
        {
            ShipPart copied = new ShipPart();
            copied.type = this.type;
            copied.lives = checklives();
            copied.pos = this.pos;
            copied.angle = this.angle;
            copied.lives = this.lives;
            copied.parent = newparent;
            copied.rand = new Random(seed);
            return copied;
        }

        public int checklives()
        {
            switch (type)
            {
                case PartType.Circle:
                    return 10;
                case PartType.Engine:
                    return 10;
                case PartType.Gun:
                    return 5;
                case PartType.Square:
                    return 5;
                case PartType.Triangle:
                    return 5;
            }
            return 5;
        }

        public int cost()
        {
            switch(type)
            {
                case PartType.Circle:
                    return 20;
                case PartType.Engine:
                    return 40;
                case PartType.Gun:
                    return 30;
                case PartType.Square:
                    return 10;
                case PartType.Triangle:
                    return 15;
            }
            return 0;
        }
        
        public bool connected()
        {
            if (type == PartType.Engine)
                return true;
            //if (neighbors.Count <= 1)
            //    return false;
            List<ShipPart> checkedparts = new List<ShipPart>();
            return connected(checkedparts);
        }

        public bool connected(List<ShipPart> checkedparts)
        {
            checkedparts.Add(this);
            foreach(ShipPart p in neighbors)
            {
                if (!checkedparts.Contains(p))
                {
                    if (p.type == PartType.Engine)
                        return true;
                    if (p.connected(checkedparts))
                        return true;
                }
            }
            return false;
        }

        public void gettarget(Team enemy)
        {
            List<ShipPart> options = new List<ShipPart>();
            foreach (Ship s in enemy.ships)
            {
                foreach (ShipPart p in s.parts)
                {
                    double distance = Math.Sqrt(Math.Pow(p.posx - posx, 2) + Math.Pow(p.posy - posy, 2));
                    double predictx = p.posx + (s.phys.dx * Math.Cos(s.direction)) * (distance / 3);
                    double predicty = p.posy + (s.phys.dy * Math.Sin(s.direction)) * (distance / 3);
                    double tempdir = Math.Atan2(predicty - posy, predictx - posx);
                    if (Math.Abs(((parent.direction + angle) % (Math.PI * 2)) - tempdir) < Math.PI / 4)
                    {
                        options.Add(p);
                        if (p.type == PartType.Engine || p.type == PartType.Gun)
                        {
                            options.Add(p);
                            options.Add(p);
                            options.Add(p);
                            options.Add(p);
                        }
                    }
                }
            }
            if (options.Count < 1)
                options = getmoretargets(enemy);
            if (options.Count > 0)
                target = options[rand.Next(options.Count)];
        }

        public List<ShipPart> getmoretargets(Team enemy)
        {
            List<ShipPart> options = new List<ShipPart>();
            foreach (Ship s in enemy.ships)
            {
                foreach (ShipPart p in s.parts)
                {
                    options.Add(p);
                    if (p.type == PartType.Engine || p.type == PartType.Gun)
                    {
                        options.Add(p);
                        options.Add(p);
                        options.Add(p);
                        options.Add(p);
                    }
                }
            }
            return options;
        }

        public Bullet fire()
        {
            if (target == null)
                return null;
            bulletcheck = (bulletcheck + 1) % alternate;
            if (target.lives < 1)
            {
                target = null;
                return null;
            }
            if (bulletcheck == 0)
            {
                double distance = Math.Sqrt(Math.Pow(target.posx - posx, 2) + Math.Pow(target.posy - posy, 2));
                double predictx = target.posx + (target.parent.phys.dx * Math.Cos(target.parent.direction)) * (distance / 3);
                double predicty = target.posy + (target.parent.phys.dy * Math.Sin(target.parent.direction)) * (distance / 3);
                double tempdir = Math.Atan2(predicty - posy, predictx - posx);
                if (Math.Abs(((parent.direction + angle) % (Math.PI * 2)) - tempdir) < Math.PI / 4)
                {
                    return new Bullet(posx, posy, tempdir, parent);
                }
                else
                {
                    target = null;
                    return null;
                }
            }
            return null;
        }

        public Rectangle boundbox(graphicsbox g)
        {
            Point cent = new Point((int)parent.posx, (int)parent.posy);
            cent.X += (int)((pos.X * g.edge) * Math.Sin(-1 * parent.direction));
            cent.Y += (int)((pos.X * g.edge) * Math.Cos(-1 * parent.direction));
            cent.X += (int)((pos.Y * g.edge) * Math.Cos(parent.direction));
            cent.Y += (int)((pos.Y * g.edge) * Math.Sin(parent.direction));
            posx = cent.X;
            posy = cent.Y;
            Rectangle bounds = new Rectangle(cent.X - (g.edge) - 2, cent.Y - (g.edge) - 2, 2*g.edge + 4, 2*g.edge + 4);
            return bounds;
        }

        public Rectangle hitbox(graphicsbox g)
        {
            Point cent = new Point((int)parent.posx, (int)parent.posy);
            cent.X += (int)((pos.X * g.edge) * Math.Sin(-1 * parent.direction));
            cent.Y += (int)((pos.X * g.edge) * Math.Cos(-1 * parent.direction));
            cent.X += (int)((pos.Y * g.edge) * Math.Cos(parent.direction));
            cent.Y += (int)((pos.Y * g.edge) * Math.Sin(parent.direction));
            posx = cent.X;
            posy = cent.Y;
            Rectangle bounds = new Rectangle(cent.X - (g.edge / 2), cent.Y - (g.edge / 2), g.edge, g.edge);
            return bounds;
        }

        public List<ShipBit> getbits(graphicsbox g)
        {
            List<ShipBit> bits = new List<ShipBit>(); //Give me the bits!
            PointF[] points = getpoints(g, new PointF((float)parent.posx, (float)parent.posy));
            if (points == null)
                return null;
            for (int i = 0; i < points.Length; i++)
            {
                bits.Add(new ShipBit(points[i], points[(i+1) % points.Length], this, parent, rand.Next(10000)));
            }
            return bits;
        }

        public PointF[] getpoints(graphicsbox g, PointF parentpos)
        {
            PointF cent = new PointF(parentpos.X, parentpos.Y);
            cent.X += (float)((pos.X * g.edge) * Math.Sin(-1*parent.direction));
            cent.Y += (float)((pos.X * g.edge) * Math.Cos(-1 * parent.direction));
            cent.X += (float)((pos.Y * g.edge) * Math.Cos(parent.direction));
            cent.Y += (float)((pos.Y * g.edge) * Math.Sin(parent.direction));
            posx = cent.X;
            posy = cent.Y;
            double tempdir = parent.direction + angle;

            PointF[] corners = null;
            if (type == PartType.Triangle)
            {
                corners = new PointF[3];
                double angle1 = tempdir - (3 * (Math.PI / 4));
                double angle2 = tempdir + (3 * (Math.PI / 4));
                double shortedge = Math.Sqrt(Math.Pow(g.edge, 2) / 2);
                corners[0] = new PointF((float)(cent.X + ((g.edge / 2) * Math.Cos(tempdir))), (float)(cent.Y + ((g.edge / 2) * Math.Sin(tempdir))));
                corners[1] = new PointF((float)(cent.X + (shortedge * Math.Cos(angle1))), (float)(cent.Y + (shortedge * Math.Sin(angle1))));
                corners[2] = new PointF((float)(cent.X + (shortedge * Math.Cos(angle2))), (float)(cent.Y + (shortedge * Math.Sin(angle2))));
            }
            if (type == PartType.Square || type == PartType.Engine)
            {
                corners = new PointF[4];
                double angle1 = tempdir - (3 * (Math.PI / 4));
                double angle2 = tempdir + (3 * (Math.PI / 4));
                double angle3 = tempdir + (Math.PI / 4);
                double angle4 = tempdir - (Math.PI / 4);
                double shortedge = Math.Sqrt(Math.Pow(g.edge, 2) / 2);
                corners[0] = new PointF((float)(cent.X + (shortedge * Math.Cos(angle1))), (float)(cent.Y + (shortedge * Math.Sin(angle1))));
                corners[1] = new PointF((float)(cent.X + (shortedge * Math.Cos(angle2))), (float)(cent.Y + (shortedge * Math.Sin(angle2))));
                corners[2] = new PointF((float)(cent.X + (shortedge * Math.Cos(angle3))), (float)(cent.Y + (shortedge * Math.Sin(angle3))));
                corners[3] = new PointF((float)(cent.X + (shortedge * Math.Cos(angle4))), (float)(cent.Y + (shortedge * Math.Sin(angle4))));
            }
            if (type == PartType.Gun)
            {
                corners = new PointF[3];
                double angle1 = tempdir - (Math.PI - Math.Atan(1.0 / 2.0));
                double angle2 = tempdir + (Math.PI - Math.Atan(1.0 / 2.0));
                double shortedge = Math.Sqrt((5 * Math.Pow(g.edge, 2)) / 16);
                corners[0] = new PointF(cent.X, cent.Y);
                corners[1] = new PointF((float)(cent.X + (shortedge * Math.Cos(angle1))), (float)(cent.Y + (shortedge * Math.Sin(angle1))));
                corners[2] = new PointF((float)(cent.X + (shortedge * Math.Cos(angle2))), (float)(cent.Y + (shortedge * Math.Sin(angle2))));
            }
            return corners;
        }

        public void draw(graphicsbox g)
        {
            if (flashon)
            {
                flash(g);
                if (flashcount == -1)
                {
                    flashcount = 5;
                }
                if (flashcount == 0)
                    flashon = false;
                flashcount--;
            }
            else
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        drawreverse(g, x, y);
                    }
                }
            }
        }

        public void drawreverse(graphicsbox g, int minx, int miny)
        {
            PointF cent = new Point((int)parent.posx + (g.pbox.Width * minx), (int)parent.posy + (g.pbox.Height * miny));
            cent.X += (float)((pos.X * g.edge) * Math.Sin(-1 * parent.direction));
            cent.Y += (float)((pos.X * g.edge) * Math.Cos(-1 * parent.direction));
            cent.X += (float)((pos.Y * g.edge) * Math.Cos(parent.direction));
            cent.Y += (float)((pos.Y * g.edge) * Math.Sin(parent.direction));

            PointF[] corners = getpoints(g, new PointF((float)parent.posx + (g.pbox.Width * minx), (float)parent.posy + (g.pbox.Height * miny)));
            if (corners != null)
            {
                g.g.DrawPolygon(new Pen(parent.col, g.linewidth), corners);
            }
            if (type == PartType.Engine)
            {
                g.g.FillPolygon(new SolidBrush(parent.col), corners);
            }
            if (type == PartType.Circle)
            {
                g.g.DrawEllipse(new Pen(parent.col, g.linewidth), cent.X - (g.edge / 2) + 1, cent.Y - (g.edge / 2) + 1, g.edge - 2, g.edge - 2);
            }
        }

        public void flash(graphicsbox g)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    flashreverse(g, x, y);
                }
            }
        }

        public void flashreverse(graphicsbox g, int minx, int miny)
        {
            PointF cent = new Point((int)parent.posx + (g.pbox.Width * minx), (int)parent.posy + (g.pbox.Height * miny));
            cent.X += (float)((pos.X * g.edge) * Math.Sin(-1 * parent.direction));
            cent.Y += (float)((pos.X * g.edge) * Math.Cos(-1 * parent.direction));
            cent.X += (float)((pos.Y * g.edge) * Math.Cos(parent.direction));
            cent.Y += (float)((pos.Y * g.edge) * Math.Sin(parent.direction));

            PointF[] corners = getpoints(g, new PointF((float)parent.posx + (g.pbox.Width * minx), (float)parent.posy + (g.pbox.Height * miny)));
            if (corners != null)
            {
                g.g.FillPolygon(new SolidBrush(Color.White), corners);
            }
            if (type == PartType.Engine)
            {
                g.g.FillPolygon(new SolidBrush(Color.White), corners);
            }
            if (type == PartType.Circle)
            {
                g.g.FillEllipse(new SolidBrush(Color.White), cent.X - (g.edge / 2) + 1, cent.Y - (g.edge / 2) + 1, g.edge - 2, g.edge - 2);
            }
        }

        public void clear(graphicsbox g)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    clearreverse(g, x, y);
                }
            }
        }

        public void clearreverse(graphicsbox g, int minx, int miny)
        {
            Rectangle reverse = boundbox(g);
            reverse.X = reverse.X + (g.pbox.Width * minx);
            reverse.Y = reverse.Y + (g.pbox.Height * miny);
            g.g.FillRectangle(new SolidBrush(g.background), reverse);
        }
    }
}
