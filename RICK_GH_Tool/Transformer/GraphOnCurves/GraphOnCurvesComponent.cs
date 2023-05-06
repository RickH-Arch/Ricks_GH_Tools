using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Rhino.Geometry.Intersect;

namespace RICK_GH_Tool.Transformer
{
    public class GraphOnCurvesComponent : GH_Component
    {
        private bool _unoverlap = true;
        
        private bool _trimStyle1 = false;
        private bool _trimStyle2 = true;

        List<Curve> graphs = new List<Curve>();
        List<Curve> tcs = new List<Curve>();
        List<double> prop = new List<double>();
        int seed = 0;
        int graphTotalNum = 0;

        GraphPointPool pool = GraphPointPool.CreateInstance();

        /// <summary>
        /// Initializes a new instance of the GraphOnCurvesComponent class.
        /// </summary>
        public GraphOnCurvesComponent()
          : base("GraphOnCurves", "GOCs",
              "Put Graphs onto given curves",
              "Rick_Tool", "Transformer")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Graph", "G", "Graph that will put onto curves", GH_ParamAccess.list);
            pManager.AddNumberParameter("GraphProportion", "gPropotion",
                "Should be as list,the proportion of a number compared with mass addition represent the proportion of" +
                "the graph with same list index", GH_ParamAccess.list,1);
            pManager.AddIntegerParameter("GraphTotalNumber", "gTotalNum", "", GH_ParamAccess.item,100);
            pManager.AddCurveParameter("TargetCurves", "tCs", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("GraphMinDistance", "gMinDis", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Seed", "seed", "", GH_ParamAccess.item,1);
            pManager.AddCurveParameter("InCurve", "inC", "", GH_ParamAccess.item);
            pManager.AddCurveParameter("OutCurve", "outC", "", GH_ParamAccess.list);
            pManager[6].Optional = true;
            pManager[7].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("GraphPosition", "gP", "", GH_ParamAccess.tree);
            pManager.AddCurveParameter("Graphs", "g", "", GH_ParamAccess.tree);
            pManager.AddCurveParameter("TargetCurves", "tCs", "", GH_ParamAccess.list);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem Unoverlap = new ToolStripMenuItem();//Unoverlap

            Unoverlap.Text = "Unoverlap";
            Unoverlap.Click += new EventHandler((o, e) =>
            {
                _unoverlap = !_unoverlap;
                ExpireSolution(true);
            });
            Unoverlap.Checked = _unoverlap;
            menu.Items.Add(Unoverlap);

            ToolStripMenuItem Trim = new ToolStripMenuItem();//trim

            Trim.Text = "TrimStyle1";
            
            Trim.Click += new EventHandler((o, e) =>
            {
                _trimStyle1 = !_trimStyle1;
                if (_trimStyle2) _trimStyle2 = false;
                ExpireSolution(true);
            });
            Trim.Checked = _trimStyle1;
            menu.Items.Add(Trim);

            ToolStripMenuItem Trim2 = new ToolStripMenuItem();//trim2

            Trim2.Text = "TrimStyle2";

            Trim2.Click += new EventHandler((o, e) =>
            {
                _trimStyle2 = !_trimStyle2;
                if (_trimStyle1) _trimStyle1 = false;
                ExpireSolution(true);
            });
            Trim2.Checked = _trimStyle2;
            menu.Items.Add(Trim2);
        }

        protected override void BeforeSolveInstance()
        {
            pool.Refresh();
            graphs = new List<Curve>();
            tcs = new List<Curve>();
            prop = new List<double>();
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve inC = null;
            List<Curve> outC = new List<Curve>();

            DA.GetDataList("Graph", graphs);
            DA.GetDataList("TargetCurves", tcs);
            DA.GetDataList("GraphProportion", prop);
            DA.GetData("Seed", ref seed);
            DA.GetData("GraphTotalNumber", ref graphTotalNum);
            DA.GetData("InCurve", ref inC);
            DA.GetDataList("OutCurve", outC);

            double minDis = 0;
            DA.GetData("GraphMinDistance", ref minDis);
            pool.MINDIS = minDis;

            if(graphs.Count != prop.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    "graph list length must equal the proportion list length");
                return;
            }

            ///////////////////////////////////////////////////////////////////////////
            

            

            Random randomChoosePoint = new Random(seed);
            Random randomChooseGraph = new Random(seed);

            //find the langest curve and calculate the proper interval of graph
            //int index = -1;
            double len = 0;
            for(int i = 0; i < tcs.Count; i++)
            {
                if (tcs[i].GetLength() > len)
                {
                    len = tcs[i].GetLength();
                    //index = i;
                }
            }
            int interval = (int)len / 20;

            //get all the point that can be chosen
            bool addable = true;
            List<Point3d> pts = new List<Point3d>();
            foreach(Curve c in tcs)
            {
                int l = 0;
                while (l < c.GetLength()-interval)
                {
                    if(inC != null && inC.Contains(c.PointAtLength(l)) != PointContainment.Inside)
                    {
                         addable = false;
                    }
                    
                    
                    foreach(Curve cc in outC)
                    {
                        if(cc != null && cc.Contains(c.PointAtLength(l)) != PointContainment.Outside)
                        {
                            addable = false;
                        }
                    }
                    
                    
                    if(addable)
                        pts.Add(c.PointAtLength(l));

                    addable = true;
                    
                    l += interval;
                }
            }

            
            



            //get all graph point
            int totalNum = graphTotalNum;
            List<int> chosenList = new List<int>();
            bool chosen = false;
            int choose = -1;

            //int chooseNum = 0;

            while (totalNum > 0)
            {
                while (!chosen)
                {
                    choose = (int)(randomChoosePoint.NextDouble() * pts.Count);
                    if (!chosenList.Contains(choose)) chosen = true;
                }

                Point3d p = pts[choose];

                if (_unoverlap)
                {
                    if (pool.AddAGraphPointUnoverlap(p))
                    {
                        totalNum--;
                    }
                }
                else
                {
                    pool.AddAGraphPoint(p);
                }
                
                chosen = false;
            }


            



            /////////////////////////////////////////////////////////////////
            //engage graph to each graphPoint

            //calculate the proportion
            double totalprop = 0;
            foreach (double p in prop)
            {
                totalprop = totalprop + p;
            }
            for (int i = 0; i < prop.Count; i++)
            {
                prop[i] = prop[i] / totalprop;
            }

            double[] propStage = new double[prop.Count];
            for (int i = 0; i < prop.Count; i++)
            {
                double p = 0;
                for (int j = 0; j < i+1; j++)
                {
                    p += prop[j];
                }
                propStage[i] = p;
            }

            foreach(GraphPoint gp in pool.pool)
            {
                double d = randomChooseGraph.NextDouble();
                for(int i = 0; i < propStage.Length; i++)
                {
                    if (d < propStage[i])
                    {
                        gp.SetOriGraph(graphs[i]);
                        gp.GraphIndex = i;
                        break;
                    }
                }
            }

           

            /////////////////////////////////////////////////////////////////
            //export graph point tree and graph tree
            DataTree<Point3d> graphPtTree = new DataTree<Point3d>();
            DataTree<Curve> graphsTree = new DataTree<Curve>();
            List<Curve> allgraphs = new List<Curve>();
            foreach(GraphPoint gp in pool.pool)
            {
                for(int i = 0; i < graphs.Count; i++)
                {
                    if(gp.GraphIndex == i)
                    {
                        GH_Path pNow = new GH_Path(i);
                        graphPtTree.Add(gp.targetPt,pNow );
                        graphsTree.Add(gp.ExportGraph(), pNow);
                        allgraphs.Add(gp.ExportGraph());
                    }
                }
            }



            //if trim == true,trim target curves by graphs
            if (_trimStyle1 || _trimStyle2)
            {
                tcs = CurveListTrim(tcs, allgraphs);
            }

            DA.SetDataTree(0, graphPtTree);
            DA.SetDataTree(1, graphsTree);
            DA.SetDataList(2, tcs);

        }


        private List<Curve> CurveListTrim( List<Curve> listBeTrimed,List<Curve> listToTrim)
        {
            List<Curve> result = new List<Curve>();
            
            for(int i = 0; i < listBeTrimed.Count; i++)
            {
                List<double> trimPos = new List<double>();
                List<Curve> cs = new List<Curve>();
                Curve c = listBeTrimed[i];
                for(int j = 0; j < listToTrim.Count; j++)
                {
                    CurveIntersections intersec = Intersection.CurveCurve(c, listToTrim[j], 0.1, 0.1);
                    if(intersec.Count == 2)
                    {
                        trimPos.Add(intersec[0].ParameterA);
                        trimPos.Add(intersec[1].ParameterA);
                    }
                }
                if (trimPos.Count < 2)
                {
                    result.Add(c);
                    continue;
                }

                trimPos.Sort();
                for(int j = 0; j < trimPos.Count; j++)
                {
                    if (j == 0) cs.Add(c.Trim(0d, trimPos[j]));
                    else if (j % 2 == 0) cs.Add(c.Trim(trimPos[j - 1], trimPos[j]));
                    else if (j == trimPos.Count - 1&&_trimStyle2) cs.Add(c.Trim(trimPos[j], c.Domain.Max));
                }
                result.AddRange(cs);
            }
            return result;
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
                return Properties.Resource1.GraphOnCurves;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0c5e487b-8dcb-4f79-9ba3-8761d2237d94"); }
        }

        
    }
}