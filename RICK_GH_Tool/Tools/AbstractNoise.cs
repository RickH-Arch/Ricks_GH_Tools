﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICK_GH_Tool.Tools
{
    public abstract class AbstractNoise
    {
		protected const int HEX_FF = 255;

		protected const int TWO_TO_9 = 512;

		protected readonly int[][] _grad3 = new int[12][]
		{
			new int[3] { 1, 1, 0 },
			new int[3] { -1, 1, 0 },
			new int[3] { 1, -1, 0 },
			new int[3] { -1, -1, 0 },
			new int[3] { 1, 0, 1 },
			new int[3] { -1, 0, 1 },
			new int[3] { 1, 0, -1 },
			new int[3] { -1, 0, -1 },
			new int[3] { 0, 1, 1 },
			new int[3] { 0, -1, 1 },
			new int[3] { 0, 1, -1 },
			new int[3] { 0, -1, -1 }
		};

		protected readonly int[][] _grad4 = new int[32][]
		{
			new int[4] { 0, 1, 1, 1 },
			new int[4] { 0, 1, 1, -1 },
			new int[4] { 0, 1, -1, 1 },
			new int[4] { 0, 1, -1, -1 },
			new int[4] { 0, -1, 1, 1 },
			new int[4] { 0, -1, 1, -1 },
			new int[4] { 0, -1, -1, 1 },
			new int[4] { 0, -1, -1, -1 },
			new int[4] { 1, 0, 1, 1 },
			new int[4] { 1, 0, 1, -1 },
			new int[4] { 1, 0, -1, 1 },
			new int[4] { 1, 0, -1, -1 },
			new int[4] { -1, 0, 1, 1 },
			new int[4] { -1, 0, 1, -1 },
			new int[4] { -1, 0, -1, 1 },
			new int[4] { -1, 0, -1, -1 },
			new int[4] { 1, 1, 0, 1 },
			new int[4] { 1, 1, 0, -1 },
			new int[4] { 1, -1, 0, 1 },
			new int[4] { 1, -1, 0, -1 },
			new int[4] { -1, 1, 0, 1 },
			new int[4] { -1, 1, 0, -1 },
			new int[4] { -1, -1, 0, 1 },
			new int[4] { -1, -1, 0, -1 },
			new int[4] { 1, 1, 1, 0 },
			new int[4] { 1, 1, -1, 0 },
			new int[4] { 1, -1, 1, 0 },
			new int[4] { 1, -1, -1, 0 },
			new int[4] { -1, 1, 1, 0 },
			new int[4] { -1, 1, -1, 0 },
			new int[4] { -1, -1, 1, 0 },
			new int[4] { -1, -1, -1, 0 }
		};

		protected readonly int[] _p = new int[256]
		{
			151, 160, 137, 91, 90, 15, 131, 13, 201, 95,
			96, 53, 194, 233, 7, 225, 140, 36, 103, 30,
			69, 142, 8, 99, 37, 240, 21, 10, 23, 190,
			6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
			94, 252, 219, 203, 117, 35, 11, 32, 57, 177,
			33, 88, 237, 149, 56, 87, 174, 20, 125, 136,
			171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
			27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
			60, 211, 133, 230, 220, 105, 92, 41, 55, 46,
			245, 40, 244, 102, 143, 54, 65, 25, 63, 161,
			1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
			18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
			164, 100, 109, 198, 173, 186, 3, 64, 52, 217,
			226, 250, 124, 123, 5, 202, 38, 147, 118, 126,
			255, 82, 85, 212, 207, 206, 59, 227, 47, 16,
			58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
			119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
			101, 155, 167, 43, 172, 9, 129, 22, 39, 253,
			19, 98, 108, 110, 79, 113, 224, 232, 178, 185,
			112, 104, 218, 246, 97, 228, 251, 34, 242, 193,
			238, 210, 144, 12, 191, 179, 162, 241, 81, 51,
			145, 235, 249, 14, 239, 107, 49, 192, 214, 31,
			181, 199, 106, 157, 184, 84, 204, 176, 115, 121,
			50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
			222, 114, 67, 29, 24, 72, 243, 141, 128, 195,
			78, 66, 215, 61, 156, 180
		};

		protected readonly int[] _perm = new int[512];

		protected readonly int[][] _simplex = new int[64][]
		{
			new int[4] { 0, 1, 2, 3 },
			new int[4] { 0, 1, 3, 2 },
			new int[4],
			new int[4] { 0, 2, 3, 1 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 1, 2, 3, 0 },
			new int[4] { 0, 2, 1, 3 },
			new int[4],
			new int[4] { 0, 3, 1, 2 },
			new int[4] { 0, 3, 2, 1 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 1, 3, 2, 0 },
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 1, 2, 0, 3 },
			new int[4],
			new int[4] { 1, 3, 0, 2 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 2, 3, 0, 1 },
			new int[4] { 2, 3, 1, 0 },
			new int[4] { 1, 0, 2, 3 },
			new int[4] { 1, 0, 3, 2 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 2, 0, 3, 1 },
			new int[4],
			new int[4] { 2, 1, 3, 0 },
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 2, 0, 1, 3 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 3, 0, 1, 2 },
			new int[4] { 3, 0, 2, 1 },
			new int[4],
			new int[4] { 3, 1, 2, 0 },
			new int[4] { 2, 1, 0, 3 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 3, 1, 0, 2 },
			new int[4],
			new int[4] { 3, 2, 0, 1 },
			new int[4] { 3, 2, 1, 0 }
		};

		protected AbstractNoise()
		{
			for (int i = 0; i < 512; i++)
			{
				_perm[i] = _p[i & 0xFF];
			}
		}

		public abstract double Noise(double x, double y);

		public abstract double Noise(double x, double y, double z);

		public abstract double Noise(double x, double y, double z, double w);

		protected double Dot(int[] g, double x, double y)
		{
			return (double)g[0] * x + (double)g[1] * y;
		}

		protected double Dot(int[] g, double x, double y, double z)
		{
			return (double)g[0] * x + (double)g[1] * y + (double)g[2] * z;
		}

		protected double Dot(int[] g, double x, double y, double z, double w)
		{
			return (double)g[0] * x + (double)g[1] * y + (double)g[2] * z + (double)g[3] * w;
		}

		protected int FastFloor(double x)
		{
			if (!(x > 0.0))
			{
				return (int)x - 1;
			}
			return (int)x;
		}
	}
}
