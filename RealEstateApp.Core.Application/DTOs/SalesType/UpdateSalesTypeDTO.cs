using System;

namespace RealEstateApp.Core.Application.DTOs.SalesType
{
    public class UpdateSalesTypeDTO
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
