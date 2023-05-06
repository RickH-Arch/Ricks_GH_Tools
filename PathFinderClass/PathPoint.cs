using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderClass
{
    public class PathPoint
    {
        public Point3d opt;

        public int ID = -1;

        bool start = false;//是否是起始地
        public bool Start { get => start; }

        bool destination = false;//是否是目的地
        public bool Destination { get => destination; set => destination = value; }

        bool passed = false;//是否已经经过

        public bool Passed { get => passed; set => passed = value; }

        bool isfork = false;//是否是岔路口

        public bool Isfork { get => isfork; }

        bool isTracker = false;//是否是wifi探针
        public bool IsTracker { get => isTracker; set => isTracker = value; }

        bool blocked = false;//是否被堵住
        public bool Blocked { get => blocked; set => blocked = value; }

        double dis2Destination = 0;//距离终点的直线距离
        public double Dis2Destination { get => dis2Destination; set => dis2Destination = value; }

        List<Path> paths = new List<Path>();//与该点相接的路径
        public List<Path> Paths { get => paths; }

        public List<PathRecorder> recorders = new List<PathRecorder>();
        public List<PathPoint> listToFind = new List<PathPoint>();
        public List<PathPoint> listHasFind = new List<PathPoint>();
        
       

        public PathPoint(Point3d pt)
        {
            opt = pt;
        }



        public void AddPath(Path path)
        {
            paths.Add(path);
            if (paths.Count > 2 && !isfork) isfork = true;
        }

        public void SetStart() {
            if (!destination)
                start = true;
        }

        /// <summary>
        /// set this point as destination, and count every pathPoints' distance to this point
        /// </summary>
        public void SetDestination() {
            if (!start)
                destination = true;
            PathPointPool ppPool = PathPointPool.CreateInstance();
            foreach (PathPoint p in ppPool.pool)
            {
                if (!p.Start && !p.Destination && !p.Blocked)
                {
                    p.Dis2Destination = this.opt.DistanceTo(p.opt);
                }
            }
        }

        public void SetTracker()
        {
            isTracker = true;
        }

        public PathRecorder GetRecorder(PathPoint p)
        {
            foreach(PathRecorder r in recorders)
            {
                if (r.target.IsEqual(p)) return r;
            }
            return null;
        }


        public void Refresh()
        {
            start = false;
            destination = false;
            passed = false;
        }

        public bool IsEqual(PathPoint pp)
        {
            return pp.opt.DistanceTo(this.opt) < 0.001 ? true:false;
        }

        public class PathRecorder
        {
            public PathPoint oriP;
            public PathPoint target;
            
            public double distance = double.MaxValue;//如果初始状态没有路径直接相连，则距离为无穷远

            public bool gotPath = false;

            public PathPoint preP;//初始状态设置每个点的前驱为自身

            public PathRecorder(PathPoint oriP,PathPoint target)
            {
                this.oriP = oriP;
                this.target = target;

                preP = target;

                foreach(Path path in oriP.Paths)
                {
                    PathPoint pp = path.GetAnotherPathPoint(oriP);
                    //如果起点与终点有直接的路径相连，则设置二者距离为路径距离，
                    //但这是还没有确定该路径是否就是最短距离，所以gotPath还是为复数
                    if(pp != null && pp.IsEqual(target) && !pp.Blocked)
                    {
                        distance = path.Length; 
                        preP = oriP;//设置前驱点为起点
                    }
                }
            }
        }
    }
}
