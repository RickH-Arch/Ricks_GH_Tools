using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace RICK_GH_Tool.Transformer.CurvesToDots
{
    public class CurvesToDotsComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CurvesToDotsComponent class.
        /// </summary>
        public CurvesToDotsComponent()
          : base("CurvesToDots", "CTD",
              "Transfer curves to points,merge close points,export merge number",
              "Rick_Tool", "Transformer")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "cs", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Point Interval", "pInterval", "", GH_ParamAccess.item, 4);
            pManager.AddCurveParameter("outCurves", "outC", "", GH_ParamAccess.list);
            pManager[2].Optional = true;
            
            pManager.AddNumberParameter("Merge distance", "mDis", "", GH_ParamAccess.item,4);
            pManager.AddCurveParameter("awayCurves", "awayC", "", GH_ParamAccess.list);
            pManager[4].Optional = true;
            pManager.AddNumberParameter("awayDistance", "awayDis", "", GH_ParamAccess.item,20);
            //pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "pts", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("MergeNumber", "mNum","", GH_ParamAccess.list);
            
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> cs = new List<Curve>();
            DA.GetDataList("Curves", cs);
            double interval = 4;
            DA.GetData("Point Interval", ref interval);
            List<Curve> outCs = new List<Curve>();
            DA.GetDataList("outCurves", outCs);
            List<Curve> awayCs = new List<Curve>();
            DA.GetDataList("awayCurves", awayCs);
            double awayDis = 0;
            DA.GetData("awayDistance", ref awayDis);
            double mDis = 0;
            DA.GetData("Merge distance", ref mDis);

            List<Point3d> oripts = new List<Point3d>();
            List<bool> merged = new List<bool>();

            //get all Points
            for(int i = 0; i < cs.Count; i++)
            {
                double len = interval;
                while (len < (cs[i].GetLength() - interval))
                {
                    oripts.Add(cs[i].PointAtLength(len));
                    len += interval;
                    merged.Add(false);
                }
            }

            //remove points in outCs
            for(int i = 0; i < oripts.Count; i++)
            {
                for(int j = 0; j < outCs.Count; j++)
                {
                    if(outCs[j].Contains(oripts[i]) == PointContainment.Inside)
                    {
                        oripts.RemoveAt(i);
                        merged.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }


            List<Point3d> pts = new List<Point3d>();
            List<int> mergeNum = new List<int>();

            //merge close points
            for(int i = 0; i < oripts.Count; i++)
            {
                if (merged[i]) continue;
                List<Point3d> ps = new List<Point3d>();
                ps.Add(oripts[i]);
                for(int j = 0; j < oripts.Count; j++)
                {
                    if (i == j||merged[j]) continue;
                    if (oripts[i].DistanceTo(oripts[j]) < mDis)
                    {
                        ps.Add(oripts[j]);
                        merged[j] = true;
                    }
                }
                if (ps.Count > 1)
                {
                    pts.Add(GetAveragePoint(ps));
                    mergeNum.Add(ps.Count);
                }
                else
                {
                    pts.Add(oripts[i]);
                    mergeNum.Add(1);
                }
            }

            for (int i = 0; i < pts.Count; i++)
            {
                for (int j = 0; j < outCs.Count; j++)
                {
                    if (outCs[j].Contains(pts[i]) == PointContainment.Inside)
                    {
                        pts.RemoveAt(i);
                        mergeNum.RemoveAt(i);
                        i--;
                    }
                }
            }

            for(int i = 0; i < pts.Count; i++)
            {
                for(int j = 0; j < awayCs.Count; j++)
                {
                    double t = 0;
                    awayCs[j].ClosestPoint(pts[i], out t);
                    if (awayCs[j].PointAt(t).DistanceTo(pts[i]) < awayDis)
                    {
                        pts.RemoveAt(i);
                        mergeNum.RemoveAt(i);
                        i--;
                    }
                }
            }

            DA.SetDataList(0, pts);
            DA.SetDataList(1, mergeNum);

        }


        private Point3d GetAveragePoint(List<Point3d> pts)
        {
            double x = 0, y = 0,z = 0;
            foreach(Point3d p in pts)
            {
                x += p.X;
                y += p.Y;
                z += p.Z;
            }
            return new Point3d(x / pts.Count, y / pts.Count, z / pts.Count);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resource1.CurvesToDots;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("07636233-587b-46ce-9022-b57cc3451877"); }
        }
    }
}