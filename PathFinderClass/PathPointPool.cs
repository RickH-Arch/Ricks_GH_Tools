using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderClass
{
    public class PathPointPool
    {
        private PathPointPool() { }
        private static PathPointPool _Singleton = null;
        public static double COMPARE_LIMIT = 0.001;

        public List<PathPoint> pool = new List<PathPoint>();
        

        public static PathPointPool CreateInstance()
        {
            if(_Singleton == null)
            {
                _Singleton = new PathPointPool();
            }
            return _Singleton;
        }

        public static void Refresh()
        {
            _Singleton = new PathPointPool();
        }

        public PathPoint GetPathPoint(Point3d pt)
        {
            PathPoint pp = CompareExisting(pt);
            if(pp == null)
            {
                pp = new PathPoint(pt);
                pool.Add(pp);
            }
            return pp;
        }

        private PathPoint CompareExisting(Point3d pt)
        {
            foreach (PathPoint pp in pool)
            {
                if (pt.DistanceTo(pp.opt) < COMPARE_LIMIT)
                {
                    return pp;
                }
            }
            return null;
        }

        public bool HasPoint(Point3d pt)
        {
            foreach(PathPoint p in pool)
            {
                if (p.opt.DistanceTo(pt) < COMPARE_LIMIT) return true;
            }
            return false;
        }

        public void ResetPassedPoint()
        {
            foreach (PathPoint p in pool) p.Passed = false;
        }

        
    }
}
