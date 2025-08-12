using System;

namespace RealEstateApp.Core.Application.DTOs.SalesType
{
    public class CreateSalesTypeDTO
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
