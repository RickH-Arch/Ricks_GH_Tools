using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace RICK_GH_Tool
{
    public class WiFi_Stamper : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the WiFi_Stamper class.
        /// </summary>
        public WiFi_Stamper()
          : base("WiFi_Stamper", "wifistamper",
              "stamp wifi trcker",
              "RICK_Tool", "PathFinder")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("ID", "ID", "Point ID", GH_ParamAccess.item);
            pManager.AddPointParameter("POS", "POS", "Set ID postion", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("IDinfo", "IDinfo", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int id = 0;
            Point3d pos = Point3d.Origin;
            DA.GetData("ID", ref id);
            DA.GetData("POS", ref pos);
            IDinfo info = new IDinfo(id, pos);

            DA.SetData("IDinfo", info);
        }

        public class IDinfo
        {
            public Point3d pos;
            public int ID;
            public IDinfo(int ID, Point3d pos)
            {
                this.ID = ID;
                this.pos = pos;
            }
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
                return RICK_GH_Tool.Properties.Resource1.WiFiStamper;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b755fb52-1503-4fee-b729-b2b4f58cb373"); }
        }
    }
}