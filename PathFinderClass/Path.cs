using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace PathFinderClass
{
    public class Path
    {
        public Curve oriCurve;
        private double length;
        public double Length { get => length; }

        private List<PathPoint> pathPoints = new List<PathPoint>();
        public List<PathPoint> PathPoints { get => pathPoints; }

        public Vector3d pathDir;

        public Path(Curve oriC)
        {
            oriCurve = oriC;
            length = oriC.GetLength();
            //匹配PathPoint
            Point3d p1 = oriC.PointAtStart;
            Point3d p2 = oriC.PointAtEnd;
            PathPointPool pool = PathPointPool.CreateInstance();
            PathPoint pp1 = pool.GetPathPoint(p1);
            PathPoint pp2 = pool.GetPathPoint(p2);
            AddPathPoint2List(pp1,pp2);
            


        }

        private void AddPathPoint2List(PathPoint p1 ,PathPoint p2)
        {
            p1.AddPath(this);
            p2.AddPath(this);
            pathPoints.Add(p1);
            pathPoints.Add(p2);
        }

        public PathPoint GetAnotherPathPoint(PathPoint p)
        {
            //first makesure p is the point of this path
            int count = 0;
            foreach(PathPoint pp in pathPoints)
            {
                if (pp.IsEqual(p)) count++;
            }
            if (count == 0) return null;

            foreach(PathPoint pp in pathPoints)
            {
                if (!pp.IsEqual(p)) return pp;
            }
            return null;
        }
        
    }
}
