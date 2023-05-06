using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace RICK_GH_Tool.Transformer
{
    public class GraphPoint
    {
        public Point3d targetPt;
       
        public Curve oriGraph;
        public Curve targetC;

        public Point3d opt = Point3d.Origin;

        private int graphIndex = -1;
        public int GraphIndex { get; set; }
        public GraphPoint(Point3d targetPt,Curve origraph,Curve targetC)
        {
            this.targetPt = targetPt;
            oriGraph = origraph;
            this.targetC = targetC;
        }

        public GraphPoint(Point3d targetP)
        {
            this.targetPt = targetP;
        }

        public void SetOriGraph(Curve c)
        {
            oriGraph = c;
            opt = c.GetBoundingBox(true).Center;
        }

        public void SetTargetC(Curve c)
        {
            targetC = c;
        }

        public Curve ExportGraph()
        {
            Vector3d move = new Vector3d(targetPt.X - opt.X, targetPt.Y - opt.Y, targetPt.Z - opt.Z);
            //Transform tran = Transform.Translation(move);
            Curve cc = oriGraph.DuplicateCurve();
            cc.Translate(move);
            return cc;
        }
    }
}
