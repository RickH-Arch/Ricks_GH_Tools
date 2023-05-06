using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderClass
{
    public class SegPoint
    {
        public Point3d opt;

        public Curve fromC;

        public Curve toC;

        private double cornerSize = 0;
        public static double CORNER_ANGLE_LIMIT  = 120;

        public bool isCorner = false;

        public double CornerSize
        {
            get => cornerSize; 
            
        }

        public void SetCornerSize(double size,double angle)
        {
            cornerSize = size;
            if (size < angle) isCorner = true;
            else isCorner = false;
        }

        public SegPoint(Point3d p)
        {
            opt = p;


        }
    }
}
