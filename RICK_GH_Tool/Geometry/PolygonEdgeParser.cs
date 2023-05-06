using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using PathFinderClass;

namespace RICK_GH_Tool
{
    public class PolygonEdgeParser : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PolygonEdgeParser class.
        /// </summary>
        public PolygonEdgeParser()
          : base("PolygonEdgeParser", "PolygonEdgeParser",
              "",
              "Rick_Tool", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "crv", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Angle", "agl", "recognize as corner when smaller than this angle", GH_ParamAccess.item, 120f);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Corner Points", "cornerP", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("Edges", "edges", "", GH_ParamAccess.list);
        }

        protected override void BeforeSolveInstance()
        {
            
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            SegPointPool pool = SegPointPool.CreateInstance();
            pool.Refresh();
            Curve c = null;
            double ang = 0;
            DA.GetData("Curve", ref c);
            DA.GetData("Angle", ref ang);
            //SegPoint.CORNER_ANGLE_LIMIT = ang;

            Curve[] segs = c.DuplicateSegments();

            List<Point3d> corners = new List<Point3d>();
            List<Curve> edges = new List<Curve>();

            for (int i = 0; i < segs.Length; i++)
            {
                Vector3d dir1 = -GetSegmentDir(segs[i]);
                Vector3d dir2 = GetSegmentDir(segs[(i + 1) % segs.Length]);
                double angle = Vector3d.VectorAngle(dir1, dir2) / Math.PI * 180;
                SegPoint sp = pool.GetSegPoint(segs[i].PointAtEnd);
                sp.fromC = segs[i];
                sp.toC = segs[(i + 1) % segs.Length];
                sp.SetCornerSize(angle, ang);
                
            }

            bool findCorner = false;
            int count = 0;
            List<Curve> joinC = new List<Curve>();
            for(int i = 0; i < segs.Length;)
            {
                if (!findCorner)
                {
                    if(count > segs.Length - 1)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Has No Corner");
                        return;
                    }
                    Curve cc = segs[count];
                    if (pool.GetSegPoint(cc.PointAtStart).isCorner)
                    {
                        findCorner = true;

                    }
                    else count++;
                }
                else
                {
                    int index = (i + count)%segs.Length;
                    joinC.Add(segs[index]);
                    if (pool.GetSegPoint(segs[index].PointAtEnd).isCorner)
                    {
                        Curve ccc = Curve.JoinCurves(joinC.ToArray())[0];
                        corners.Add(pool.GetSegPoint(ccc.PointAtStart).opt);
                        edges.Add(ccc);
                        joinC = new List<Curve>();
                    }
                    i++;
                }
               

            }

            DA.SetDataList("Corner Points", corners);
            DA.SetDataList("Edges", edges);
        }

        private Vector3d GetSegmentDir(Curve seg)
        {
            Point3d strP = seg.PointAtStart;
            Point3d endP = seg.PointAtEnd;
            Vector3d dir = endP - strP;
            dir.Unitize();
            return dir;
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
                return Properties.Resource1.PolygonEdgeParser;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5217510b-2431-4e8d-ac1b-e6cfccbab994"); }
        }
    }
}