using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Core.Domain.Entities
{
    public class PropertyImprovement
    {
        public int PropertyId { get; set; }
        public int ImprovementId { get; set; }

        public virtual Property Property { get; set; }
        public virtual Improvement Improvement { get; set; }
    }
}
