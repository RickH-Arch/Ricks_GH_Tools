using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using RICK_GH_Tool.Generator.Wave;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RICK_GH_Tool.Transformer
{
    public class SplitCurvesByCurvesComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SplitCurvesByCurvesComponent class.
        /// </summary>
        public SplitCurvesByCurvesComponent()
          : base("SplitCurvesByCurves", "SplitCurvesByCurves",
              "",
              "Rick_Tool", "Transformer")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves be split", "cbs", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curves to split", "cts", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "cs", "", GH_ParamAccess.tree);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> cbs = new List<Curve>();
            List<Curve> cts = new List<Curve>();
            DA.GetDataList(0, cbs);
            DA.GetDataList(1, cts);

            foreach(Curve c in cbs)
            {
                if (!c.IsClosed)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "all curves be split must be closed");
                    return;
                }
            }

            foreach(Curve c in cts)
            {
                if (!c.IsClosed)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "all curves be split must be closed");
                    return;
                }
            }


            DataTree<Curve> result = new DataTree<Curve>();
            for(int i = 0; i < cts.Count; i++)
            {
                List<Curve> splitC = new List<Curve>();
                Curve c = cts[i];
                for(int j = 0; j < cbs.Count; j++)
                {
                    Curve cnow = cbs[j];
                    int status;
                    CurveRelation(c, cnow, out status);
                    if (status == 0) splitC.Add(cnow);
                    else if(status == 1)
                    {
                        Curve[] cs  = Curve.CreateBooleanIntersection(c, cnow);
                        Curve cc = Curve.JoinCurves(cs)[0];
                        splitC.Add(cc);
                    }
                }

                GH_Path pathnow = new GH_Path(i);
                result.AddRange(splitC, pathnow);
                
                
            }

            DA.SetDataTree(0, result);


        }

        /*
        /// <summary>
        /// return curves contains in father curve, autoly close split curve
        /// </summary>
        /// <param name="fc"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        private Curve SplitCurvebyCurve(Curve fc,Curve cc)
        {
            Curve[] segs = cc.DuplicateSegments();
            List<Curve> result = new List<Curve>();
            for(int i = 0; i < segs.Length; i++)
            {
                Point3d stp = segs[i].PointAtStart;
                Point3d edp = segs[i].PointAtEnd;
                PointContainment pc1 = fc.Contains(stp);
                PointContainment pc2 = fc.Contains(edp);
                if(pc1 == PointContainment.Inside && pc2 == PointContainment.Inside)
                {
                    result.Add(segs[i]);
                }
                else if(pc1 == PointContainment.Inside)
                {
                    
                }
            }
        }*/

        /// <summary>
        /// 0-> all in; 1-> part in; 2-> all out; 3-> error
        /// </summary>
        /// <param name="fc"></param>father curve
        /// <param name="cc"></param>child curve
        /// <returns></returns>
        private void CurveRelation(Curve fc,Curve cc,out int status)
        {
            status = 3;
            Curve[] segs = cc.DuplicateSegments();
            List<Point3d> pts = new List<Point3d>();
            foreach(Curve seg in segs)
            {
                pts.Add(seg.PointAtStart);
            }

            bool allout = true;
            bool allin = true;
            foreach(Point3d p in pts)
            {
                PointContainment pc = fc.Contains(p);
                if (pc == PointContainment.Inside) allout = false;
                else if (pc == PointContainment.Outside) allin = false;
            }

            if (!allin && !allout) status = 1;
            else if (allin) status = 0;
            else if (allout) status = 2;
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
                return Properties.Resource1.SplitCurvesByCurves;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9ecacc72-680d-4897-99ce-21b9ddc27069"); }
        }
    }
}