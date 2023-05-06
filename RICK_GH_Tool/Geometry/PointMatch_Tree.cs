using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace RICK_GH_Tool
{
    public class PointMatch_Tree : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PointCompair_Tree class.
        /// </summary>
        public PointMatch_Tree()
          : base("PointMatch_Tree", "PM_T",
              "Match Points according to distance based on dataTree, return true when points in listA match any point in listB,vice versa",
              "Rick_Tool", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("PointsA", "PA", "", GH_ParamAccess.tree);
            pManager.AddPointParameter("PointsB", "PB", "", GH_ParamAccess.tree);
            pManager.AddNumberParameter("distance", "dis", "", GH_ParamAccess.item,0.1d);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points Match", "pMatch", "", GH_ParamAccess.tree);
            pManager.AddPointParameter("Points UnMatch", "pUnMatch", "", GH_ParamAccess.tree);
            pManager.AddBooleanParameter("match", "m", "", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<GH_Point> pA;
            GH_Structure<GH_Point> pB;
            double distance = 0;

            DA.GetDataTree("PointsA", out pA);
            DA.GetDataTree("PointsB", out pB);
            DA.GetData("distance", ref distance);

            DataTree<Point3d> pM = new DataTree<Point3d>();
            DataTree<Point3d> pUnM = new DataTree<Point3d>();
            DataTree<bool> match = new DataTree<bool>();

            if(pA.Paths.Count != pB.Paths.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The path number of pA and pB must be equal!");
                return;
            }

            for (int i = 0; i < pA.Paths.Count; i++)
            {
                GH_Path pthA = pA.Paths[i];
                GH_Path pthB = pB.Paths[i];
                for (int j = 0; j < pA[pthA].Count; j++)
                {
                    GH_Point pAnow = pA[pthA][j];
                    Point3d panow = new Point3d(pAnow.Value);
                    for(int m = 0; m < pB[pthB].Count; m++)
                    {
                        GH_Point pBnow = pB[pthB][m];
                        Point3d pbnow = new Point3d(pBnow.Value);

                        if (panow.DistanceTo(pbnow) < distance)
                        {
                            pM.Add(panow, pthA.AppendElement(j));
                            match.Add(true, pthA.AppendElement(j));
                            break;
                        }
                        if(m == pB[pthB].Count - 1)
                        {
                            pUnM.Add(panow, pthA.AppendElement(j));
                            match.Add(false, pthA.AppendElement(j));
                        }
                    }
                }
            }

            DA.SetDataTree(0, pM);
            DA.SetDataTree(1, pUnM);
            DA.SetDataTree(2, match);
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
                return Properties.Resource1.PointMatch_Tree;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d8e90890-8ae0-49a7-a1a1-19e201883bfc"); }
        }
    }
}