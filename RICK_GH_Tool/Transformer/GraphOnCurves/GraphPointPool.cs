using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace RICK_GH_Tool.Transformer
{
    
    public class GraphPointPool
    {
        private GraphPointPool() { }
        private static GraphPointPool _Singleton = null;
        public double MINDIS = 10;

        public static GraphPointPool CreateInstance()
        {
            if(_Singleton == null)
            {
                _Singleton = new GraphPointPool();
            }
            return _Singleton;
        }

        public List<GraphPoint> pool = new List<GraphPoint>();

        public void Refresh()
        {
            _Singleton.pool.Clear();
        }


        /// <summary>
        /// if return true, pool can add a graphpoint on this position
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool CompareDistance(Point3d p)
        {
            foreach(GraphPoint gp in pool)
            {
                if (gp.targetPt.DistanceTo(p) < MINDIS) return false;
            }
            return true;
        }

        public bool AddAGraphPointUnoverlap(Point3d p)
        {
            if (CompareDistance(p))
            {
                pool.Add(new GraphPoint(p));
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddAGraphPoint(Point3d p)
        {
            pool.Add(new GraphPoint(p));
            return true;
        }




    }
}
