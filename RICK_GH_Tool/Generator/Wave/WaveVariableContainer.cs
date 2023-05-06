using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICK_GH_Tool.Generator.Wave
{
    class WaveVariableContainer
    {
        public double xIndex;
        public double yIndex;
        public double incr;
        public double scale;

        public WaveVariableContainer(double xin,double yin,double incr)
        {
            xIndex = xin;
            yIndex = yin;
            this.incr = incr;
        }
        public WaveVariableContainer() {
            xIndex = 0;
            yIndex = 0;
            this.incr = 0;
        }
    }
}
