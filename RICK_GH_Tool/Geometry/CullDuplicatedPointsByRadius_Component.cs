using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace RICK_GH_Tool.Geometry
{
    public class CullDuplicatedPointsByRadius_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CullDuplicatedPointsByDistance_Component class.
        /// </summary>
        public CullDuplicatedPointsByRadius_Component()
          : base("CullDuplicatedPointsByRadius", "CullDuPtsByRadius",
              "(when cull between two points, point with larger radius will remain)",
              "Rick_TooL", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "pts", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Radius", "r", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "pts", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Radius", "r", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> pts = new List<Point3d>();
            List<double> radius = new List<double>();
            DA.GetDataList("Points", pts);
            DA.GetDataList("Radius", radius);

            if (pts.Count != radius.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Points number must equal radius number");
                return;
            }
                
            

            bool[] culled = new bool[pts.Count];
            for(int i = 0; i < culled.Length; i++)
            {
                culled[i] = false;
            }

            List<Point3d> pResult = new List<Point3d>();
            List<double> rResult = new List<double>();
            List<int> choosed = new List<int>();
            List<int> cullList = new List<int>();
            int index = -1;
            for (int i = 0; i < pts.Count; i++)
            {
                
                if (culled[i]) continue;
                
                cullList.Add(i);

                //add index to cull list if distance too short
                for (int j = 0; j < pts.Count; j++)
                {
                    if (culled[j]||i==j) continue;
                    if (pts[i].DistanceTo(pts[j]) < radius[i] + radius[j])
                    {
                        cullList.Add(j);
                    }
                }

                //have points to culled
                if (cullList.Count > 1)
                {
                    //find point that has the biggest radius,and its index
                    index = -1;
                    double rad = double.MinValue;
                    foreach(int ind in cullList)
                    {
                        if (radius[ind] > rad)
                        {
                            rad = radius[ind];
                            index = ind;
                        }
                        
                    }
                    //culled other smaller radius points
                    foreach(int ind in cullList)
                    {
                        if (ind != index)
                        {
                            culled[ind] = true;
                            if (choosed.Contains(ind))
                            {
                                choosed.Remove(ind);
                                //int v = pResult.FindIndex((Point3d p)=>p.DistanceTo(pts[ind]) < 0.1d);
                                
                                //pResult.RemoveAt(v);
                                //rResult.RemoveAt(v);
                            }
                        }
                        
                    }

                    if(!choosed.Contains(index)) choosed.Add(index);

                    //pResult.Add(pts[index]);
                    //rResult.Add(radius[index]);
                }
                //no points to culled
                else
                {
                    choosed.Add(i);
                    //pResult.Add(pts[i]);
                    //rResult.Add(radius[i]);
                }
                cullList.Clear();
            }

            foreach(int i in choosed)
            {
                pResult.Add(pts[i]);
                rResult.Add(radius[i]);
            }

            DA.SetDataList(0, pResult);
            DA.SetDataList(1, rResult);
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
                return Properties.Resource1.CullDuplicatedPointsByRadius;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("aaeca83e-718f-4408-829b-b87dbd9a7717"); }
        }
    }
}