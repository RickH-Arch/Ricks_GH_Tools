using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Microsoft.CSharp;

namespace RICK_GH_Tool
{
    public class HexagonalWeaving : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the HexagonalWeaving class.
        /// </summary>
        public HexagonalWeaving()
          : base("HexagonalWeaving", "HexWeaving",
              "Turn a Geometry into hexagonal bamboo weaving",
              "Rick_TOOL", "Generator")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Geometry", "geo", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Weave", "weave", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GeometryBase geo = null;
            DA.GetData("Geometry", ref geo);
            

            var triremesh = Rhino.NodeInCode.Components.FindComponent("Kangaroo2Component_TriRemesh");
            if(triremesh == null)
            {
                throw new ArgumentException("cannot find triremesh component");
            }

            var triremesh_fun = triremesh.Delegate as dynamic;
            Mesh mm = null; bool bb = true;Curve cc = null;
            //Mesh m = triremesh_fun(geo, mm, bb, cc, 10d, 25)[0];
           // Mesh m = triremesh_fun(geo)[0];
            Mesh m = null;
            DA.SetData(0, m);

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
                return Properties.Resource1.HexagonalWeaving;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("dda30784-f393-405c-a752-e1064785e22c"); }
        }
    }
}