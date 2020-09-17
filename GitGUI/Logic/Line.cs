using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class Line
    {
        public Point A { get; private set; }
        public Point B { get; private set; }

        public Line(Point a, Point b)
        {
            A = a;
            B = b;
        }

        public Point? Intersection(Line l)
        {
            double x1 = B.X - A.X, y1 = B.Y - A.Y,
                x2 = l.B.X - l.A.X, y2 = l.B.Y - l.A.Y;
            double a;
            if (x2 != 0)
            {
                a = l.A.Y - A.Y + y2 / x2 * (A.X - l.A.X);
                a /= y1 - x1 * y2 / x2;
            }
            else
            {
                a = (l.A.X - A.X) / x1;
            }
            if (a < 0 || a > 1)
                return null;
            return new Point(A.X + a * x1, A.Y + a * x2);
        }
    }
}
