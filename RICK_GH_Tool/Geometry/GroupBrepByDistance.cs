using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace RICK_GH_Tool
{
    public class GroupBrepByDistance : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GroupByDistance class.
        /// </summary>
        public GroupBrepByDistance()
          : base("GroupByDistance", "Group_Dis",
              "Group objects by distance, return datatree",
              "Rick_Tool", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Objects", "objs", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Threshold", "thr", 
                "objects distence less than this number will be put in one group", GH_ParamAccess.item);
            pManager.AddPointParameter("Cursors", "curs",
                "Cursor of each objects to calculate distance, set cursor to geometry center by default", GH_ParamAccess.list);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Groups", "grps", "", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> objs = new List<Brep>();
            
            List<Point3d> curs = new List<Point3d>();
            double DIS = 0;
            DA.GetDataList("Objects", objs);
            DA.GetDataList("Cursors", curs);
            DA.GetData("Threshold", ref DIS);

            if(curs.Count == 0)
            {
                for(int i = 0; i < objs.Count; i++)
                {
                    curs.Add(AreaMassProperties.Compute(objs[i]).Centroid);
                }
            }

            DataTree<Brep> groups = new DataTree<Brep>();
            

            List<bool> getGroup = new List<bool>();
            for(int i = 0; i < objs.Count; i++)
            {
                getGroup.Add(false);
            }

            int pathCount = 0;
            
            for(int i = 0; i < objs.Count; i++)
            {
                if (!getGroup[i])
                {
                    getGroup[i] = true;
                    GH_Path path = new GH_Path(pathCount);
                    pathCount++;
                    groups.Add(objs[i], path);
                    for(int j = 0; j < objs.Count; j++)
                    {
                        if (i == j) continue;
                        if (!getGroup[j] && curs[i].DistanceTo(curs[j]) <DIS)
                        {
                            getGroup[j] = true;
                            groups.Add(objs[j], path);
                        }
                    }
                }
            }

            DA.SetDataTree(0, groups);
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
                return Properties.Resource1.GroupBrepByDistance;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8870bde1-9b48-4e68-aa65-b0cf8a0261c9"); }
        }
    }
}