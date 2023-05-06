using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using PathFinderClass;
using Grasshopper.Kernel.Types;
using System.Drawing;

namespace RICK_GH_Tool
{
    public class PathViewer : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PathViewer class.
        /// </summary>
        public PathViewer()
          : base("PathViewer", "PathViewer",
              "View Paths and PathPoints",
              "RICK_Tool", "PathFinder")
        {
        }

        private List<Circle> _regularPoints = new List<Circle>();
        private List<Circle> _forkPoints = new List<Circle>();
        private List<Circle> _blockedPoints = new List<Circle>();
        private List<Circle> _trackerPoints = new List<Circle>();
        private List<Curve> _Paths = new List<Curve>();

        private List<TextEntity> _texts = new List<TextEntity>();
       
        

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Paths", "Paths", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("DisplaySize", "DisplaySize", "", GH_ParamAccess.item,1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void BeforeSolveInstance()
        {
            _regularPoints = new List<Circle>();
             _forkPoints = new List<Circle>();
             _blockedPoints = new List<Circle>();
             _trackerPoints = new List<Circle>();
            _Paths = new List<Curve>();
            _texts = new List<TextEntity>();

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double size = 3;
            DA.GetData("DisplaySize", ref size);

            PathPointPool pool = PathPointPool.CreateInstance();
            List<Path> paths = new List<Path>();
            DA.GetDataList("Paths", paths);
            foreach(Path p in paths)
            {
                _Paths.Add(p.oriCurve);
            }

            List<PathPoint> ps = pool.pool;
            foreach(PathPoint p in ps)
            {
                if (p.IsTracker) _trackerPoints.Add(new Circle(p.opt, size));
                else if (p.Blocked) _blockedPoints.Add(new Circle(p.opt, size));
                else if (p.Isfork) _forkPoints.Add(new Circle(p.opt, size));
                else _regularPoints.Add(new Circle(p.opt, size));

                if(p.ID != -1)
                {
                    TextEntity text = new TextEntity();
                    text.PlainText = p.ID.ToString();
                    text.Plane = new Plane(p.opt,Vector3d.ZAxis);
                    text.TextHeight = size;
                    double a = text.TextModelWidth;
                    _texts.Add(text);
                }
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);

            foreach(Circle c in _trackerPoints)
            {
                args.Display.DrawCircle(c, Color.Yellow,5);
            }

            foreach (Circle c in _forkPoints)
            {
                args.Display.DrawCircle(c, Color.DarkGreen,5);
            }

            foreach (Circle c in _blockedPoints)
            {
                args.Display.DrawCircle(c, Color.Red,5);
            }

            foreach (Circle c in _regularPoints)
            {
                args.Display.DrawCircle(c, Color.LightGreen,5);
            }

            foreach (Curve c in _Paths)
            {
                args.Display.DrawCurve(c, Color.Orange,4);
            }

            foreach(TextEntity t in _texts)
            {
                args.Display.DrawText(t, Color.Red);
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
                return RICK_GH_Tool.Properties.Resource1.PathViewer;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("483ea5b1-4b86-40a8-af3a-0c9eba0cb870"); }
        }
    }
}