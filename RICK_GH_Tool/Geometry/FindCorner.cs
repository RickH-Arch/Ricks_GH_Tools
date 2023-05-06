using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace RICK_GH_Tool
{
    public class FindCorner : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FindCorner class.
        /// </summary>
        public FindCorner()
          : base("FindCorner", "FindCorner",
              "Find Corner of a polygon face",
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
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve c = null;
            double ang = 0;
            DA.GetData("Curve",ref c);
            DA.GetData("Angle", ref ang);
            Curve[] segs = c.DuplicateSegments();

            List<Point3d> corners = new List<Point3d>();

            for(int i = 0; i < segs.Length; i++)
            {
                Vector3d dir1 = -GetSegmentDir(segs[i]);
                Vector3d dir2 = GetSegmentDir(segs[(i + 1) % segs.Length]);
                double angle = Vector3d.VectorAngle(dir1, dir2)/Math.PI*180;
                if (angle < ang)
                {
                    corners.Add(segs[i].PointAtEnd);
                }
            }

            DA.SetDataList("Corner Points", corners);

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
                return Properties.Resource1.FindCorner;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2b21d235-5b18-4dfe-a681-14cf3c8f65f2"); }
        }
    }
}