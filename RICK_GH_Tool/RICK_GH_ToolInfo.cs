using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace RICK_GH_Tool
{
    public class RICK_GH_ToolInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "RICK_Tool";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Properties.Resource1.RickTool2;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("a4ddd225-31e3-43f6-909a-ca182fe4ec63");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Huang Rick";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "ricksemail@163.com";
            }
        }
    }
}
