using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace l_application_pour_diploma{
    internal class Classes{
        public class Points{
            public int xl, yl;
            public decimal dest;
            public Points(int x, int y, decimal d){
                xl = x;
                yl = y;
                dest = d;
            }
        }
        public class Points2{
            public Point point;
            public decimal dest;
            public Points2(int x, int y, decimal d){
                point = new Point(x, y);
                dest = d;
            }
            public Points2(Point p, decimal d){
                point = p;
                dest = d;
            }
        }

    }
}
