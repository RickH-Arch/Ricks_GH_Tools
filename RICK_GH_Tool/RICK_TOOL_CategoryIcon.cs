using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;

namespace RICK_GH_Tool
{
    public class RICK_TOOL_CategoryIcon:GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Instances.ComponentServer.AddCategoryIcon("Rick_Tool", Properties.Resource1.RickTool2);
            Instances.ComponentServer.AddCategorySymbolName("Rick_Tool", 'R');
            return GH_LoadingInstruction.Proceed;
        }
    }
}
