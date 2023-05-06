using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderClass
{
    public class SegPointPool
    {
        private SegPointPool() { }
        private static SegPointPool _Singleton = null;
        public static double COMPARE_LIMIT = 0.001;

        public List<SegPoint> pool = new List<SegPoint>();
        public static SegPointPool CreateInstance()
        {
            if(_Singleton == null)
            {
                _Singleton = new SegPointPool();
                
            }
            return _Singleton;
            
        }

        public SegPoint GetSegPoint(Point3d pt)
        {
            SegPoint sp = CompareExisting(pt);
            if(sp == null)
            {
                sp = new SegPoint(pt);
                pool.Add(sp);
            }
            return sp;
        }

        private SegPoint CompareExisting(Point3d pt)
        {
            for(int i = 0; i< pool.Count; i++)
            {
                if (pt.DistanceTo(pool[i].opt) < COMPARE_LIMIT)
                {
                    return pool[i];
                }

            }
            return null;
        }

        public void Refresh()
        {
            pool = new List<SegPoint>();
        }
    }
}
