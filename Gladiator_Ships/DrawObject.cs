using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gladiator_Ships
{
    interface DrawObject
    {
        void draw(graphicsbox g);
        void clear(graphicsbox g);
    }
}
