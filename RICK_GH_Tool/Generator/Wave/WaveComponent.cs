using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using RICK_GH_Tool.Generator.Wave;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RICK_GH_Tool.Generator
{
    public class WaveComponent : GH_Component
    {
        private bool _removeFloatingWave = true;
        private double REMOVE_FTRACK_Threshold = 0.005;
        private double REMOVE_DIS;

        private double REMOVE_SHORT_Threshold = 0.1;

        private bool _removeWaveInOneEdge = false;



        /// <summary>
        /// Initializes a new instance of the Wave class.
        /// </summary>
        public WaveComponent()
          : base("Wave", "Wave",
              "Create wave using input point, based on simplex noise",
              "Rick_TOOL", "Generator")
        {
        }

        

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "Pts", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Iteration Start", "Itr st", "", GH_ParamAccess.list,0);
            pManager.AddIntegerParameter("Iteration End", "Itr end", "", GH_ParamAccess.list,50);
            pManager.AddCurveParameter("Wave Range", "wRange", 
                "If no input,range will be the boundingbox of given points", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddGenericParameter("factors", "factors", "", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddBooleanParameter("Wrap?", "Wrap?", "always constrain particles in region", GH_ParamAccess.item,false);
            //pManager.AddIntegerParameter("Seed", "seed", "", GH_ParamAccess.item,1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Wave Points", "wPoints", "", GH_ParamAccess.tree);
            pManager.AddCurveParameter("Wave Curves", "wCurves", "", GH_ParamAccess.list);
            pManager.AddPointParameter("(All Points)", "(APt)", "", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> pts = new List<Point3d>();
            if (!DA.GetDataList("Points", pts)) return;
           
            

            
            List<int> itrsts = new List<int>();
            List<int> itrends = new List<int>();
            if (!DA.GetDataList("Iteration Start", itrsts) || !DA.GetDataList("Iteration End", itrends)) return;
            //check itr num
            if(itrsts.Count != itrends.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "iteration start end end list length must match");
                return;
            }
            //start num
            int minStart = int.MaxValue;
            foreach(int num in itrsts)
            {
                if (num < minStart) minStart = num;
            }
            if (minStart < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "iteration starts cannot have negative number");
                return;
            }
            //end num
            for(int i = 0;i< itrsts.Count; i++)
            {
                if (itrends[i] < itrsts[i])
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                        "iteration end of a particle cannot smaller than its iteration start");
                    return;
                }
            }

            //get range
            Curve range = null;
            if(!DA.GetData("Wave Range", ref range))
            {
                BoundingBox b = new BoundingBox(pts);
                Point3d[] boxPts = b.GetCorners();

                LineCurve la = new LineCurve(boxPts[0], boxPts[1]);
                LineCurve laa = new LineCurve(boxPts[2], boxPts[3]);
                LineCurve lb = new LineCurve(boxPts[1], boxPts[2]);
                LineCurve lbb = new LineCurve(boxPts[3], boxPts[0]);
                List<LineCurve> boxLines = new List<LineCurve>() { la, lb, laa, lbb };
                range = Curve.JoinCurves(boxLines)[0];
            }
            
            WaveVariables.Range = range;

            REMOVE_DIS = (range.GetLength() / 4) * REMOVE_FTRACK_Threshold;


            //get factors
            WaveVariableContainer factors = new WaveVariableContainer();
            DA.GetData("factors", ref factors);
            if (factors.xIndex != 0) WaveVariables.xFactor = factors.xIndex;
            if (factors.yIndex != 0) WaveVariables.yIndex = factors.yIndex;
            if (factors.incr != 0) WaveVariables.incr = factors.incr;
            WaveVariables.scale =  factors.scale;
            //wrap?
            bool wrap = false;
            DA.GetData("Wrap?", ref wrap);
            WaveVariables.wrap = wrap;

            //Seed
            /*
            int seed = 0;
            if(DA.GetData("Seed", ref seed))
            {
                WaveVariables.seed = seed;
            }*/
            

            //////////////////////////////////////////////////////////////////////

            //set particles
            int pcount = pts.Count;
            List<WaveParticle> particles = new List<WaveParticle>();
            
            for(int i = 0; i < pcount; i++)
            {
                Point3d pnow = pts[i];
                particles.Add(new WaveParticle(pnow));
            }

            //get the largest iteration num
            int maxEnd = int.MinValue;
            foreach(int num in itrends)
            {
                if (num > maxEnd) maxEnd = num;
            }



            //get wave(no matter how much iter start and end of each particle is,
            //will calculate according to the largest iteration end)
            for(int i = 0; i < maxEnd; i++)
            {
                for(int j = 0; j < pcount; j++)
                {
                    particles[j].Update2D();
                }
            }


            //output track
            
            DataTree<Point3d> tracks = new DataTree<Point3d>();
            List<Curve> cTracks = new List<Curve>();
            DataTree<Point3d> allpt = new DataTree<Point3d>();
            
            for(int i = 0; i < particles.Count; i++)
            {
                int start = itrsts[i % itrsts.Count];
                int end = itrends[i % itrends.Count];
                
                List<Point3d> trackNow = particles[i].GetRangeTrackPoint(start,end-start);
                cTracks.AddRange(particles[i].GetRangeTrackCurve(start,end-start));


                GH_Path pathNow = new GH_Path(i);
                allpt.AddRange(particles[i].track, pathNow);
                if (wrap)
                {
                    tracks.AddRange(WrapInRange(trackNow, range), pathNow);
                }
                else tracks.AddRange(trackNow, pathNow);
            }

            if (_removeFloatingWave)
            {
                for(int i = 0; i < cTracks.Count; i++)
                {
                    Curve c = cTracks[i];
                    double t1,t2;
                    range.ClosestPoint(c.PointAtStart, out t1);
                    range.ClosestPoint(c.PointAtEnd, out t2);
                    double dis1 = range.PointAt(t1).DistanceTo(c.PointAtStart);
                    double dis2 = range.PointAt(t2).DistanceTo(c.PointAtEnd);



                    if(dis1>REMOVE_DIS || dis2 > REMOVE_DIS || c.GetLength()<(range.GetLength()/4)*REMOVE_SHORT_Threshold)
                    {
                        cTracks.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (_removeWaveInOneEdge)
            {
                for (int i = 0; i < cTracks.Count; i++)
                {
                    Curve c = cTracks[i];
                    double t1, t2;
                    range.ClosestPoint(c.PointAtStart, out t1);
                    range.ClosestPoint(c.PointAtEnd, out t2);
                    Point3d p1 = range.PointAt(t1);
                    Point3d p2 = range.PointAt(t2);



                    if (p1.X == p2.X || p1.Y == p2.Y)
                    {
                        cTracks.RemoveAt(i);
                        i--;
                    }
                }
            }

            




            DA.SetDataTree(0, tracks);
            DA.SetDataList(1, cTracks);
            DA.SetDataTree(2, allpt);

        }

        /// <summary>
        /// return points that only in range
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private List<Point3d> WrapInRange(List<Point3d> pts, Curve c)
        {
            List<Point3d> curpts = new List<Point3d>();
            for(int i = 0; i < pts.Count; i++)
            {
                if (InRange(pts[i], c)) curpts.Add(pts[i]);
            }
            return curpts;
        }

        private bool InRange(Point3d pt,Curve c)
        {
            PointContainment ptc = c.Contains(pt);
            if((int)ptc == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem rft = new ToolStripMenuItem();//remove floating wave
            
            rft.Text = "Remove Floating Wave";
            rft.Click += new EventHandler((o, e) =>
            {
                _removeFloatingWave = !_removeFloatingWave;
                ExpireSolution(true);
            });
            rft.Checked = _removeFloatingWave;
            menu.Items.Add(rft);

            ToolStripMenuItem rwe = new ToolStripMenuItem();//remove wave start and end on one edge
            rwe.Text = "Remove wave start and end on one edge";
            rwe.Click += new EventHandler((o, e) =>
            {
                _removeWaveInOneEdge = !_removeWaveInOneEdge;
                ExpireSolution(true);
            });
            rwe.Checked = _removeWaveInOneEdge;
            menu.Items.Add(rwe);

            
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
                return Properties.Resource1.Wave;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7e52cffe-1b18-4f67-bcdc-c98d96c35d5e"); }
        }
    }
}