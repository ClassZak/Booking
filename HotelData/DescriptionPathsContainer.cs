using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking
{
    public class DescriptionPathsContainer
    {
        public List<string> DescriptionPaths { get; set; }
        public DescriptionPathsContainer()
        {
            DescriptionPaths = new List<string>();
        }
        public DescriptionPathsContainer(List<string> descriptionPaths) : this()
        {
            DescriptionPaths = descriptionPaths;
        }
        public DescriptionPathsContainer(DescriptionPathsContainer descriptionPathsContainer) : this()
        {
            foreach(string s in descriptionPathsContainer.DescriptionPaths)
            {
                DescriptionPaths.Add(s);
            }
        }

        public DescriptionPathsContainer(params string [] values) : this()
        {
            DescriptionPaths = new List<string>(values);
        }
    }
}
