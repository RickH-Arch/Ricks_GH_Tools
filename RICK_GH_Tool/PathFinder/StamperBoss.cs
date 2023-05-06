using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using static RICK_GH_Tool.WiFi_Stamper;

namespace RICK_GH_Tool
{
    public class StamperBoss : GH_Component,IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the StamperBoss class.
        /// </summary>
        public StamperBoss()
          : base("StamperBoss", "SBoss",
              "Pack stamper IDinfo",
              "Rick_Tool", "PathFinder")
        {
        }

        List<IDinfo> infos = new List<IDinfo>();

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("IDinfoList", "IDinfoList", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            for(int i = 0;i < Params.Input.Count; i++)
            {
                IDinfo info = null;
                DA.GetData(i, ref info);
                infos.Add(info);
            }

            DA.SetDataList(0, infos);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if(side == GH_ParameterSide.Input)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input && index > 0) return true;
            else return false;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            var o = new Param_GenericObject();
            o.Name = string.Format("ID{0}", index);
            o.NickName = o.Name;
            o.Optional = true;
            o.Access = GH_ParamAccess.item;
            
            return o;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            
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
                return RICK_GH_Tool.Properties.Resource1.StamperBoss;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f08457f4-61ae-4754-84bb-2b99546daa1f"); }
        }
    }
}