using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace RICK_GH_Tool.Generator.Wave
{
    public class WaveVariableComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the WaveVariableComponent class.
        /// </summary>
        public WaveVariableComponent()
          : base("WaveVariable", "WaveVariable",
              "Change indexs that control the wave",
              "Rick_TooL", "Generator")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("xFactor", "xFactor", "invalid if no input", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("yFactor", "yFactor", "invalid if no input", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("incr", "incr", "invalid if no input", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("scale", "scl", "", GH_ParamAccess.item,1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("factors", "facs", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double xfac = 0, yfac = 0, incr = 0, scale = 1;
            DA.GetData(0, ref xfac);
            DA.GetData(1, ref yfac);
            DA.GetData(2, ref incr);
            DA.GetData(3, ref scale);


            xfac *= 0.001;
            yfac *= 0.001;
            incr *= 0.001;
            WaveVariableContainer factors = new WaveVariableContainer(xfac, yfac, incr);
            factors.scale = scale;

            DA.SetData(0, factors);
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
                return Properties.Resource1.WaveVariable;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e66296f0-ccfa-4d08-b518-940cf9875378"); }
        }
    }
}