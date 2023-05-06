using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RICK_GH_Tool
{
    public class MeshDual : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshDual class.
        /// </summary>
        public MeshDual()
          : base("MeshDual", "MashDual",
              "",
              "Rick_Tool", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Input Mesh",GH_ParamAccess.item);
            pManager.AddIntegerParameter("Center", "C", "Center type (0=circumcenter, 1=barycenter)", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Dual", "D", "Polylines of dual mesh faces", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh destination = new Mesh();
            int destination2 = 0;
            if(!DA.GetData(0,ref destination)||!DA.GetData(1,ref destination2))
            {
                return;
            }

            

            bool[] nakedEdgePointStatus = destination.GetNakedEdgePointStatus();
            List<Polyline> list = new List<Polyline>();
            for(int i = 0; i < destination.Vertices.Count; i++)
            {
                if (nakedEdgePointStatus[i])
                {
                    continue;
                }

                //获得所有与现在顶点相连的顶点的index，过滤掉重合顶点
                int[] array = new int[0];
                array = destination.TopologyVertices.ConnectedTopologyVertices(destination.TopologyVertices.TopologyVertexIndex(i), sorted: true);
               
                int[] array2 = new int[array.Length];
                for (int j = 0; j < array.Length; j++)
                {
                    array2[j] = destination.TopologyVertices.MeshVertexIndices(array[j])[0];
                }
                List<Point3d> list2 = new List<Point3d>();
                
                
                //将对应顶点添加进list2
                for (int j = 0; j < array2.Length; j++)
                {
                    if (array2[j] != i)
                    {
                        list2.Add(destination.Vertices[array2[j]]);
                    }
                }

                //选取与当前顶点相邻的两点，三点画圆取中点
                Polyline polyline = new Polyline();
                for (int j = 0; j <= list2.Count; j++)
                {
                    Point3d point3d = default(Point3d);
                    if (destination2 == 0)
                    {
                        point3d = new Circle(list2[j % list2.Count], list2[(j + 1) % list2.Count], destination.Vertices[i]).Center;
                    }
                    else
                    {
                        point3d = list2[j % list2.Count] + list2[(j + 1) % list2.Count] + destination.Vertices[i];
                        point3d *= 1.0 / 3.0;
                    }
                    polyline.Add(point3d);
                }
                list.Add(polyline);
            }
            DA.SetDataList(0, list);
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
                return Properties.Resource1.MeshDual;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("09d62328-bdbf-4f17-994b-1d079d31474f"); }
        }
    }
}