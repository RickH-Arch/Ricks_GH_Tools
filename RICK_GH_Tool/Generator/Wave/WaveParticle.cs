using RICK_GH_Tool.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace RICK_GH_Tool.Generator.Wave
{
    public class WaveParticle
    {
        double  posX,posY,posZ,incr, theta;
        //SimplexNoise noiseInstance = SimplexNoise.Instance;
        PerlinNoise pNoise = new PerlinNoise();
        public Point3d opt;
        public Point3d curpt;
        public List<Point3d> track;
        public List<bool> wraped = new List<bool>();
        public WaveParticle(Point3d opt)
        {
            this.opt = opt;
            track = new List<Point3d>();
            track.Add(opt);
            incr = 0;
        }


        public void Update2D()
        {
            incr += WaveVariables.incr;
            posX = track[track.Count - 1].X;
            posY = track[track.Count - 1].Y;
            posZ = track[track.Count - 1].Z;
            double n = pNoise.Noise(posX * WaveVariables.xFactor/(10*WaveVariables.scale), 
                posY * WaveVariables.yIndex/(10*WaveVariables.scale), incr);
            //use when using simplexNoise class
            //n = (n + 1) / 2;
            theta = n * 2 * Math.PI;
            posX += 40 * Math.Cos(theta);
            posY += 40 * Math.Sin(theta);

            if (WaveVariables.wrap)
            {
                if (Wrap())
                {
                    curpt = new Point3d(posX, posY, posZ);
                    track.Add(curpt);
                    wraped.Add(true);
                }
                else
                {
                    curpt = new Point3d(posX, posY, posZ);
                    track.Add(curpt);
                    wraped.Add(false);
                }
            }
            else
            {
                curpt = new Point3d(posX, posY, posZ);
                track.Add(curpt);
                wraped.Add(false);
            }
            
            
        }

        public Point3d GetPosition()
        {
            return new Point3d(posX, posY, 0);
        }

        private bool Wrap()
        {
            bool w = false;
            double xMin = WaveVariables.XMin;
            double xMax = WaveVariables.XMax;
            double yMin = WaveVariables.YMin;
            double yMax = WaveVariables.YMax;
            if (posX < xMin)
            {
                posX = xMax;
                w = true;
            }
            if (posX > xMax)
            {
                posX = xMin;
                w = true;
            }
            if (posY < yMin)
            {
                posY = yMax;
                w = true;
            }
            if (posY > yMax)
            {
                posY = yMin;
                w = true;
            }

            return w;
        }

        public List<Point3d> GetRangeTrackPoint(int start, int count)
        {
            return track.GetRange(start, count);
        }

        public List<Curve> GetRangeTrackCurve(int start,int count)
        {
            List<Curve> tCurves = new List<Curve>();

            List<Point3d> pts = GetRangeTrackPoint(start, count);
            List<bool> w = wraped.GetRange(start, count);

            List<Point3d> ptsNow = new List<Point3d>();
            for(int i = 0; i < pts.Count; i++)
            {
                if (!w[i]){
                    ptsNow.Add(pts[i]);
                    if(i == pts.Count - 1)
                    {
                        Curve cc = Curve.CreateControlPointCurve(ptsNow);
                        if (cc != null) tCurves.Add(cc);
                        ptsNow.Clear();
                    }

                }
                else//a curve has reached his end in range box
                {
                    //find the last point reach the end
                    //make the curve end on region edge
                    if (i > 2)
                    {
                        if (!w[i - 1])
                        {
                            Point3d pa = pts[i - 1];
                            Point3d pb = pts[i];
                            LineCurve lc = new LineCurve(pa, pb);

                            Curve c = lc.Extend(CurveEnd.End, 500, CurveExtensionStyle.Smooth);
                            CurveIntersections cinter =  Intersection.CurveCurve(c, WaveVariables.Range, 0.1, 0.1);
                            if (cinter.Count != 0)
                            {
                                Point3d pc = cinter[0].PointA;
                                ptsNow.Add(pc);
                            }
                            
                        }
                    }
                    
                    
                    if(ptsNow.Count < 4)
                    {
                        ptsNow.Clear();
                    }
                    else
                    {
                        Curve cc = Curve.CreateControlPointCurve(ptsNow);
                        if(cc!=null)tCurves.Add(cc);
                        ptsNow.Clear();
                    }
                    
                    
                }
            }
            return tCurves;
        }
    }
}
