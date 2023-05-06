using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using PathFinderClass;
using Grasshopper.Kernel.Types;
using static RICK_GH_Tool.WiFi_Stamper;
using static PathFinderClass.PathPoint;

namespace RICK_GH_Tool
{
    public class PathParser : GH_Component
    {
        
        public PathParser()
          : base("PathParser", "PParser",
              "Parse line to path",
              "RICK_Tool", "PathFinder")
        {
        }

        PathPointPool pool;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Path Curves", "PCs", "Curves to be identified as path", GH_ParamAccess.list);
            pManager.AddGenericParameter("ID info", "ID info", "set wifi tracker id", GH_ParamAccess.list);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Paths", "Paths", "", GH_ParamAccess.list);
            pManager.AddGenericParameter("PathPoints", "PPoints", "", GH_ParamAccess.list);
            pManager.AddGenericParameter("Path_ID", "Path_ID", "", GH_ParamAccess.list);
            pManager.AddGenericParameter("Path_Pos", "Path_Pos", "", GH_ParamAccess.list);
            pManager.AddGenericParameter("WiFi_Pos", "WiFi_Pos", "", GH_ParamAccess.list);
        }

        
        
        
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            pool = PathPointPool.CreateInstance();

            List<Curve> curves = new List<Curve>();
            List<IGH_GeometricGoo> path_c = new List<IGH_GeometricGoo>();
            DA.GetDataList("Path Curves", path_c);
            DA.GetDataList("Path Curves", curves);

            List<Path> paths = new List<Path>();

            List<IDinfo> infos = new List<IDinfo>();
            DA.GetDataList(1, infos);

            foreach(Curve c in curves)
            {
                Curve[] cs = c.DuplicateSegments();
                if (cs.Length > 0)
                {
                    foreach (Curve cc in cs)
                    {
                        paths.Add(new Path(cc));
                    }
                }
                else
                {
                    paths.Add(new Path(c));
                }
            }

            foreach(IDinfo i in infos)
            {
                if(i != null)
                {
                    
                    PathPoint p = pool.GetPathPoint(i.pos);
                    p.ID = i.ID;
                    p.IsTracker = true;
                }
            }

            //计算最短路径
            List<string> path_ID = new List<string>();
            List<string> path_Pos = new List<string>();
            CalculatePath(out path_ID,out path_Pos);

            //导出wifi位置
            List<string> wifi_pos = ExportWifiPos();

            DA.SetDataList("Paths", paths);
            DA.SetDataList("PathPoints", pool.pool);
            DA.SetDataList("Path_ID", path_ID);
            DA.SetDataList("Path_Pos", path_Pos);
            DA.SetDataList("WiFi_Pos", wifi_pos);
            
        }

        private List<string> ExportWifiPos()
        {
            List<string> pos = new List<string>();
            pos.Add("wifi" + "," + "X" + "," + "Y");
            foreach(PathPoint p in pool.pool)
            {
                if (p.IsTracker)
                {
                    string str = p.ID.ToString() + "," + p.opt.X.ToString("0.000") + "," + p.opt.Y.ToString("0.000");
                    pos.Add(str);
                }
            }
            return pos;
        }

        //Dijkstra
        private void CalculatePath(out List<string> path_ID, out List<string> path_Pos)
        {
            List<string> p_ID = new List<string>();
            List<string> p_Pos = new List<string>();
            for(int i = 0; i < pool.pool.Count; i++)
            {
                PathPoint pNow = pool.pool[i];
                //向pNow中的recorders添加其他点的记录
                
                for(int j = 0; j< pool.pool.Count; j++)
                {
                    if (j == i) continue;
                    pNow.recorders.Add(new PathPoint.PathRecorder(pNow, pool.pool[j]));
                    //pNow.listToFind.Add(pool.pool[j]);
                }
                //pNow.listHasFind.Add(pNow);
                Dijkstra(pNow);
                //now pNow must have known the path to every point
            }

            int posPath_count = 0;

            //遍历每个点，如果是wifi point则导出其他wifi point到该点的最短路径
            List<PathPoint> ps = pool.pool;
            for(int i = 0; i < ps.Count; i++)
            {
                if (ps[i].IsTracker)
                {
                    for(int j = 0; j < ps.Count; j++)
                    {
                        if (j == i || !ps[j].IsTracker) continue;
                        List<string> pIDNow = new List<string>();
                        List<string> pPosNow = new List<string>();

                        string index = ps[i].ID.ToString() + "->" + ps[j].ID.ToString();
                        pIDNow.Add(index);
                        pPosNow.Add(index);


                        PathRecorder r = null;
                        bool arrived = false;
                        PathPoint pp = ps[j];
                        while (!arrived)
                        {
                            //找到ps[i]中记录ps[j]的recorder
                            r = ps[i].GetRecorder(pp);
                            if (r.target.IsTracker)
                            {
                                pIDNow.Insert(1, r.target.ID.ToString());
                            }
                            pPosNow.Insert(1, r.target.opt.X.ToString("0.000") + ":" + r.target.opt.Y.ToString("0.000"));
                            pp = r.preP;
                            if (pp.IsEqual(ps[i]))
                            {
                                arrived = true;
                                pPosNow.Insert(1, pp.opt.X.ToString("0.000") + ":" + pp.opt.Y.ToString("0.000"));
                                pIDNow.Insert(1, pp.ID.ToString());
                            }

                        }

                        if (posPath_count < pPosNow.Count) posPath_count = pPosNow.Count;

                        string idpath = string.Join(",", pIDNow);
                        string pospath = string.Join(",", pPosNow);

                        p_ID.Add(idpath);
                        p_Pos.Add(pospath);

                    }
                }
            }

            //增加表头
            List<string> head = new List<string>();
            head.Add("path");
            for(int i = 1; i < posPath_count+1; i++)
            {
                head.Add(i.ToString());
            }
            string h = string.Join(",", head);
            p_Pos.Insert(0, h);

            path_ID = p_ID;
            path_Pos = p_Pos;
        }

        private void Dijkstra(PathPoint pNow)
        {
            pNow.Passed = true;
            int count = pool.pool.Count - 1;
            while (count > 0)
            {
                //寻找距离起点最近的点
                double tempDis = 10000000;
                int id = -1;
                for (int m = 0; m < pNow.recorders.Count; m++)
                {
                    if (pNow.recorders[m].distance < tempDis && !pNow.recorders[m].gotPath)
                    {
                        tempDis = pNow.recorders[m].distance;
                        id = m;
                    }
                }

                if (id == -1) break;//找不到点了，溜

                //找到点，并更新与该点有路径相接的别的点与起始点的距离
                pNow.recorders[id].gotPath = true;
                pNow.recorders[id].target.Passed = true;
                count--;

                PathPoint pp = pNow.recorders[id].target;
                foreach (Path path in pp.Paths)
                {
                    PathPoint ppp = path.GetAnotherPathPoint(pp);

                    if (ppp.Passed) continue;

                    double tDis = path.Length + pNow.recorders[id].distance;
                    //找到这个点的recorder
                    PathRecorder recorder = pNow.GetRecorder(ppp);
                    

                    if (!recorder.gotPath && recorder.distance > tDis)
                    {
                        recorder.distance = tDis;//更新该点到起点的距离
                        recorder.preP = pp;//更新前驱
                    }
                }
            }
            pool.ResetPassedPoint();

            
        }

        protected override void BeforeSolveInstance()
        {
            PathPointPool.Refresh();
        }

        
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return RICK_GH_Tool.Properties.Resource1.PathParser;
            }
        }

        
        public override Guid ComponentGuid
        {
            get { return new Guid("17196aff-9df5-4bb9-af69-f92990651e02"); }
        }
    }
}
