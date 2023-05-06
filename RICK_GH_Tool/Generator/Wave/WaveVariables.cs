using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace RICK_GH_Tool.Generator.Wave
{
    public static class WaveVariables
    {
        public static double xFactor = 0.06;
        public static double yIndex = 0.04;
        public static double incr = 0.008;

        public static bool wrap = false;

        private static Curve range = null;
        private static BoundingBox bbox;

        private static double xMin;
        public static double XMin { get => xMin; }

        private static double xMax;
        public static double XMax { get => xMax; }

        private static double yMin;
        public static double YMin { get => yMin; }

        private static double yMax;
        public static double YMax { get => yMax; }

        public static int seed = 1;

        public static double scale = 1;

        public static Curve Range
        {
            set
            {
                range = value;
                bbox = range.GetBoundingBox(true);
                Point3d[] pts = bbox.GetCorners();
                xMin = double.MaxValue;
                xMax = double.MinValue;
                yMin = double.MaxValue;
                yMax = double.MinValue;
                for(int i = 0; i < pts.Length; i++)
                {
                    double xx = pts[i].X;
                    double yy = pts[i].Y;
                    if (xx < xMin) xMin = xx;
                    if (xx > xMax) xMax = xx;
                    if (yy < yMin) yMin = yy;
                    if (yy > yMax) yMax = yy;
                }
            }
            get
            {
                return range;
            }
        }
    }
}
