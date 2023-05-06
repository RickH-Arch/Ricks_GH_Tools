using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RICK_GH_Tool.Generator.Wave;

namespace RICK_GH_Tool.Tools
{
    public class PerlinNoise
    {
        static int PERLIN_YWRAPB = 4;
        static int PERLIN_YWRAP = 1 << PERLIN_YWRAPB;
        static int PERLIN_ZWRAPB = 8;
        static int PERLIN_ZWRAP = 1 << PERLIN_ZWRAPB;
        static int PERLIN_SIZE = 4095;

        private static double DEG_TO_RAD = Math.PI / 180.0d;
        private static double RAD_TO_DEG = 180.0d / Math.PI;
        private static double SINCOS_PRECISION = 0.5d;
        private static int SINCOS_LENGTH = (int)(360d / SINCOS_PRECISION);
        private static double[] cosLUT = new double[SINCOS_LENGTH];
        

        static int perlin_octaves = 4;
        static float perlin_amp_falloff = 0.5f;

        static int perlin_TWOPI, perlin_PI;
        static double[] perlin_cosTable;
        static double[] perlin;

        Random perlinRandom;

        public PerlinNoise() {
            for(int i = 0; i < SINCOS_LENGTH; i++)
            {
                cosLUT[i] = (double)Math.Cos(i * DEG_TO_RAD * SINCOS_PRECISION);
            }
        }

        public double Noise(double x)
        {
            return Noise(x, 0d, 0d);
        }

        public double Noise(double x, double y)
        {
            return Noise(x, y, 0d);
        }

        public double Noise(double x,double y,double z)
        {
            if(perlin == null)
            {
                if(perlinRandom == null)
                {
                    perlinRandom = new Random(WaveVariables.seed);
                    
                }
                perlin = new double[PERLIN_SIZE + 1];
                for(int i = 0; i < PERLIN_SIZE + 1; i++)
                {
                    perlin[i] = perlinRandom.NextDouble();
                }

                perlin_cosTable = cosLUT;
                perlin_TWOPI = perlin_PI = SINCOS_LENGTH;
                perlin_PI >>= 1;
            }

            if (x < 0) x = -x;
            if (y < 0) y = -y;
            if (z < 0) z = -z;

            int xi = (int)x, yi = (int)y, zi = (int)z;
            double xd = x - xi, yd = y - yi, zd = z - zi;
            double rxd, ryf;

            double r = 0;
            double ampl = 0.5d;

            double n1, n2, n3;

            for(int i = 0; i < perlin_octaves; i++)
            {
                int of = xi + (yi << PERLIN_YWRAPB) + (zi << PERLIN_ZWRAPB);

                rxd = Noise_fsc(xd);
                ryf = Noise_fsc(yd);

                n1 = perlin[of & PERLIN_SIZE];
                n1 += rxd * (perlin[(of + 1) & PERLIN_SIZE] - n1);
                n2 = perlin[(of + PERLIN_YWRAP) & PERLIN_SIZE];
                n2 += rxd * (perlin[(of + PERLIN_YWRAP + 1) & PERLIN_SIZE] - n2);
                n1 += ryf * (n2 - n1);

                of += PERLIN_ZWRAP;
                n2 = perlin[of & PERLIN_SIZE];
                n2 += rxd * (perlin[(of + 1) & PERLIN_SIZE] - n2);
                n3 = perlin[(of + PERLIN_YWRAP) & PERLIN_SIZE];
                n3 += rxd * (perlin[(of + PERLIN_YWRAP + 1) & PERLIN_SIZE] - n3);
                n2 += ryf * (n3 - n2);

                n1 += Noise_fsc(zd) * (n2 - n1);

                r += n1 * ampl;
                ampl *= perlin_amp_falloff;
                xi <<= 1; xd *= 2;
                yi <<= 1; yd *= 2;
                zi <<= 1; zd *= 2;

                if (xd >= 1.0f) { xi++; xd--; }
                if (yd >= 1.0f) { yi++; yd--; }
                if (zd >= 1.0f) { zi++; zd--; }
            }
            return r;

        }

        private double Noise_fsc(double i)
        {
            return 0.5d * (1.0d - perlin_cosTable[(int)(i * perlin_PI) % perlin_TWOPI]);
        }

        public void NoiseDetail(int lod)
        {
            if (lod > 0) perlin_octaves = lod;
        }

        public void noiseSeed(int seed)
        {
            perlinRandom = new Random(seed);
        }
    }
}
