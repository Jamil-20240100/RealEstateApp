using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Core.Application.DTOs.PropertyType
{
    public class CreatePropertyTypeDTO
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
