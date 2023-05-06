using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICK_GH_Tool.Tools
{
    public sealed class SimplexNoise : AbstractNoise
    {
        private const double SQRT_3 = 1.7320508075688772;

        private const double SQRT_5 = 2.23606797749979;

        private const double F2 = 0.3660254037844386;

        private const double G2 = 0.21132486540518713;

        private const double G22 = -0.57735026918962573;

        private const double F3 = 1.0 / 3.0;

        private const double G3 = 1.0 / 6.0;

        private const double F4 = 0.30901699437494745;

        private const double G4 = 0.1381966011250105;

        private const double G42 = 0.276393202250021;

        private const double G43 = 0.41458980337503148;

        private const double G44 = -0.447213595499958;

        private static SimplexNoise _instance;

        public static SimplexNoise Instance => _instance;

        static SimplexNoise()
        {
            _instance = new SimplexNoise();
        }

		public override double Noise(double x, double y)
		{
			double num = 0.0;
			double num2 = (x + y) * 0.3660254037844386;
			int num3 = FastFloor(x + num2);
			int num4 = FastFloor(y + num2);
			double num5 = (double)(num3 + num4) * 0.21132486540518713;
			double num6 = x - ((double)num3 - num5);
			double num7 = y - ((double)num4 - num5);
			int num8;
			int num9;
			if (num6 > num7)
			{
				num8 = 1;
				num9 = 0;
			}
			else
			{
				num8 = 0;
				num9 = 1;
			}
			double num10 = num6 - (double)num8 + 0.21132486540518713;
			double num11 = num7 - (double)num9 + 0.21132486540518713;
			double num12 = num6 + -0.57735026918962573;
			double num13 = num7 + -0.57735026918962573;
			int num14 = num3 & 0xFF;
			int num15 = num4 & 0xFF;
			double num16 = 0.5 - num6 * num6 - num7 * num7;
			if (num16 > 0.0)
			{
				num16 *= num16;
				int num17 = _perm[num14 + _perm[num15]] % 12;
				num += num16 * num16 * Dot(_grad3[num17], num6, num7);
			}
			double num18 = 0.5 - num10 * num10 - num11 * num11;
			if (num18 > 0.0)
			{
				num18 *= num18;
				int num19 = _perm[num14 + num8 + _perm[num15 + num9]] % 12;
				num += num18 * num18 * Dot(_grad3[num19], num10, num11);
			}
			double num20 = 0.5 - num12 * num12 - num13 * num13;
			if (num20 > 0.0)
			{
				num20 *= num20;
				int num21 = _perm[num14 + 1 + _perm[num15 + 1]] % 12;
				num += num20 * num20 * Dot(_grad3[num21], num12, num13);
			}
			return 70.0 * num;
		}

		
        public override double Noise(double x, double y, double z)
        {
			double num = 0.0;
			double num2 = (x + y + z) * (1.0 / 3.0);
			int num3 = FastFloor(x + num2);
			int num4 = FastFloor(y + num2);
			int num5 = FastFloor(z + num2);
			double num6 = (double)(num3 + num4 + num5) * (1.0 / 6.0);
			double num7 = x - ((double)num3 - num6);
			double num8 = y - ((double)num4 - num6);
			double num9 = z - ((double)num5 - num6);
			int num10;
			int num11;
			int num12;
			int num13;
			int num14;
			int num15;
			if (num7 >= num8)
			{
				if (num8 >= num9)
				{
					num10 = 1;
					num11 = 0;
					num12 = 0;
					num13 = 1;
					num14 = 1;
					num15 = 0;
				}
				else if (num7 >= num9)
				{
					num10 = 1;
					num11 = 0;
					num12 = 0;
					num13 = 1;
					num14 = 0;
					num15 = 1;
				}
				else
				{
					num10 = 0;
					num11 = 0;
					num12 = 1;
					num13 = 1;
					num14 = 0;
					num15 = 1;
				}
			}
			else if (num8 < num9)
			{
				num10 = 0;
				num11 = 0;
				num12 = 1;
				num13 = 0;
				num14 = 1;
				num15 = 1;
			}
			else if (num7 < num9)
			{
				num10 = 0;
				num11 = 1;
				num12 = 0;
				num13 = 0;
				num14 = 1;
				num15 = 1;
			}
			else
			{
				num10 = 0;
				num11 = 1;
				num12 = 0;
				num13 = 1;
				num14 = 1;
				num15 = 0;
			}
			double num16 = num7 - (double)num10 + 1.0 / 6.0;
			double num17 = num8 - (double)num11 + 1.0 / 6.0;
			double num18 = num9 - (double)num12 + 1.0 / 6.0;
			double num19 = num7 - (double)num13 + 1.0 / 3.0;
			double num20 = num8 - (double)num14 + 1.0 / 3.0;
			double num21 = num9 - (double)num15 + 1.0 / 3.0;
			double num22 = num7 - 0.5;
			double num23 = num8 - 0.5;
			double num24 = num9 - 0.5;
			int num25 = num3 & 0xFF;
			int num26 = num4 & 0xFF;
			int num27 = num5 & 0xFF;
			double num28 = 0.6 - num7 * num7 - num8 * num8 - num9 * num9;
			if (num28 > 0.0)
			{
				num28 *= num28;
				int num29 = _perm[num25 + _perm[num26 + _perm[num27]]] % 12;
				num += num28 * num28 * Dot(_grad3[num29], num7, num8, num9);
			}
			double num30 = 0.6 - num16 * num16 - num17 * num17 - num18 * num18;
			if (num30 > 0.0)
			{
				num30 *= num30;
				int num31 = _perm[num25 + num10 + _perm[num26 + num11 + _perm[num27 + num12]]] % 12;
				num += num30 * num30 * Dot(_grad3[num31], num16, num17, num18);
			}
			double num32 = 0.6 - num19 * num19 - num20 * num20 - num21 * num21;
			if (num32 > 0.0)
			{
				num32 *= num32;
				int num33 = _perm[num25 + num13 + _perm[num26 + num14 + _perm[num27 + num15]]] % 12;
				num += num32 * num32 * Dot(_grad3[num33], num19, num20, num21);
			}
			double num34 = 0.6 - num22 * num22 - num23 * num23 - num24 * num24;
			if (num34 > 0.0)
			{
				num34 *= num34;
				int num35 = _perm[num25 + 1 + _perm[num26 + 1 + _perm[num27 + 1]]] % 12;
				num += num34 * num34 * Dot(_grad3[num35], num22, num23, num24);
			}
			return 32.0 * num;
		}

        public override double Noise(double x, double y, double z, double w)
        {
			double num = 0.0;
			double num2 = (x + y + z + w) * 0.30901699437494745;
			int num3 = FastFloor(x + num2);
			int num4 = FastFloor(y + num2);
			int num5 = FastFloor(z + num2);
			int num6 = FastFloor(w + num2);
			double num7 = (double)(num3 + num4 + num5 + num6) * 0.1381966011250105;
			double num8 = x - ((double)num3 - num7);
			double num9 = y - ((double)num4 - num7);
			double num10 = z - ((double)num5 - num7);
			double num11 = w - ((double)num6 - num7);
			int num12 = 0;
			if (num8 > num9)
			{
				num12 = 32;
			}
			if (num8 > num10)
			{
				num12 |= 0x10;
			}
			if (num9 > num10)
			{
				num12 |= 8;
			}
			if (num8 > num11)
			{
				num12 |= 4;
			}
			if (num9 > num11)
			{
				num12 |= 2;
			}
			if (num10 > num11)
			{
				num12 |= 1;
			}
			int[] array = _simplex[num12];
			int num13 = ((array[0] >= 3) ? 1 : 0);
			int num14 = ((array[1] >= 3) ? 1 : 0);
			int num15 = ((array[2] >= 3) ? 1 : 0);
			int num16 = ((array[3] >= 3) ? 1 : 0);
			int num17 = ((array[0] >= 2) ? 1 : 0);
			int num18 = ((array[1] >= 2) ? 1 : 0);
			int num19 = ((array[2] >= 2) ? 1 : 0);
			int num20 = ((array[3] >= 2) ? 1 : 0);
			int num21 = ((array[0] >= 1) ? 1 : 0);
			int num22 = ((array[1] >= 1) ? 1 : 0);
			int num23 = ((array[2] >= 1) ? 1 : 0);
			int num24 = ((array[3] >= 1) ? 1 : 0);
			double num25 = num8 - (double)num13 + 0.1381966011250105;
			double num26 = num9 - (double)num14 + 0.1381966011250105;
			double num27 = num10 - (double)num15 + 0.1381966011250105;
			double num28 = num11 - (double)num16 + 0.1381966011250105;
			double num29 = num8 - (double)num17 + 0.276393202250021;
			double num30 = num9 - (double)num18 + 0.276393202250021;
			double num31 = num10 - (double)num19 + 0.276393202250021;
			double num32 = num11 - (double)num20 + 0.276393202250021;
			double num33 = num8 - (double)num21 + 0.41458980337503148;
			double num34 = num9 - (double)num22 + 0.41458980337503148;
			double num35 = num10 - (double)num23 + 0.41458980337503148;
			double num36 = num11 - (double)num24 + 0.41458980337503148;
			double num37 = num8 + -0.447213595499958;
			double num38 = num9 + -0.447213595499958;
			double num39 = num10 + -0.447213595499958;
			double num40 = num11 + -0.447213595499958;
			int num41 = num3 & 0xFF;
			int num42 = num4 & 0xFF;
			int num43 = num5 & 0xFF;
			int num44 = num6 & 0xFF;
			double num45 = 0.6 - num8 * num8 - num9 * num9 - num10 * num10 - num11 * num11;
			if (num45 > 0.0)
			{
				num45 *= num45;
				int num46 = _perm[num41 + _perm[num42 + _perm[num43 + _perm[num44]]]] % 32;
				num += num45 * num45 * Dot(_grad4[num46], num8, num9, num10, num11);
			}
			double num47 = 0.6 - num25 * num25 - num26 * num26 - num27 * num27 - num28 * num28;
			if (num47 > 0.0)
			{
				num47 *= num47;
				int num48 = _perm[num41 + num13 + _perm[num42 + num14 + _perm[num43 + num15 + _perm[num44 + num16]]]] % 32;
				num += num47 * num47 * Dot(_grad4[num48], num25, num26, num27, num28);
			}
			double num49 = 0.6 - num29 * num29 - num30 * num30 - num31 * num31 - num32 * num32;
			if (num49 > 0.0)
			{
				num49 *= num49;
				int num50 = _perm[num41 + num17 + _perm[num42 + num18 + _perm[num43 + num19 + _perm[num44 + num20]]]] % 32;
				num += num49 * num49 * Dot(_grad4[num50], num29, num30, num31, num32);
			}
			double num51 = 0.6 - num33 * num33 - num34 * num34 - num35 * num35 - num36 * num36;
			if (num51 > 0.0)
			{
				num51 *= num51;
				int num52 = _perm[num41 + num21 + _perm[num42 + num22 + _perm[num43 + num23 + _perm[num44 + num24]]]] % 32;
				num += num51 * num51 * Dot(_grad4[num52], num33, num34, num35, num36);
			}
			double num53 = 0.6 - num37 * num37 - num38 * num38 - num39 * num39 - num40 * num40;
			if (num53 > 0.0)
			{
				num53 *= num53;
				int num54 = _perm[num41 + 1 + _perm[num42 + 1 + _perm[num43 + 1 + _perm[num44 + 1]]]] % 32;
				num += num53 * num53 * Dot(_grad4[num54], num37, num38, num39, num40);
			}
			return 27.0 * num;
		}
    }
}
