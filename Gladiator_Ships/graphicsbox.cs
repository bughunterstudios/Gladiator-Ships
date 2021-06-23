using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gladiator_Ships
{
    class graphicsbox
    {
        private Bitmap DrawArea;
        public PictureBox pbox;
        public Color background;
        public Graphics g;
        public int edge = 7;
        public int linewidth = 1;

        public graphicsbox(PictureBox pbox, Color background)
        {
            this.pbox = pbox;
            this.background = background;
            DrawArea = new Bitmap(pbox.Size.Width, pbox.Size.Height);
            g = Graphics.FromImage(DrawArea);
            pbox.SizeChanged += new EventHandler(resized);
            g.Clear(background);
            pbox.Image = DrawArea;
        }

        private void resized(object sender, EventArgs e)
        {
            DrawArea = new Bitmap(pbox.Size.Width, pbox.Size.Height);
            g = Graphics.FromImage(DrawArea);
            g.Clear(background);
            pbox.Image = DrawArea;
        }

        public void draw(DrawObject obj)
        {
            try
            {
                obj.draw(this);
            }
            catch (Exception exc)
            {

            }
        }

        public void draw(List<DrawObject> objs)
        {
            foreach (DrawObject obj in objs)
            {
                draw(obj);
            }
        }

        public void clear(DrawObject obj)
        {
            try
            {
                obj.clear(this);
            }
            catch (Exception exc)
            {

            }
        }

        public void clear(List<DrawObject> objs)
        {
            foreach (DrawObject obj in objs)
            {
                clear(obj);
            }
        }

        public void showscreen()
        {
            pbox.Image = DrawArea;
        }

        public bool inbounds(Point p, int edge)
        {
            bool test = true;
            if (p.X < -edge)
                test = false;
            if (p.X > pbox.Width + edge)
                test = false;
            if (p.Y < -edge)
                test = false;
            if (p.Y > pbox.Height + edge)
                test = false;
            return test;
        }
    }
}
