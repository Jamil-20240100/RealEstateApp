using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Core.Application.DTOs.Feature
{
    public class CreateFeatureDTO
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
