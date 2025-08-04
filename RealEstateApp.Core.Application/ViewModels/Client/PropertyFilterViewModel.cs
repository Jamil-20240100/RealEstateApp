using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Core.Application.ViewModels.Client
{
    public class PropertyFilterViewModel
    {
        public int? PropertyTypeId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Precio mínimo inválido")]
        public decimal? MinPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Precio máximo inválido")]
        public decimal? MaxPrice { get; set; }

        public int? Bathrooms { get; set; }
        public int? Bedrooms { get; set; }

        public string Code { get; set; }
    }

}
