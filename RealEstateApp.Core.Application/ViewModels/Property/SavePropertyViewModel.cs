using Microsoft.AspNetCore.Http;
using RealEstateApp.Core.Application.ViewModels.Feature;
using RealEstateApp.Core.Application.ViewModels.PropertyType;
using RealEstateApp.Core.Application.ViewModels.SalesType;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class SavePropertyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de propiedad es obligatorio.")]
        public int PropertyTypeId { get; set; }

        [Required(ErrorMessage = "El tipo de venta es obligatorio.")]
        public int SalesTypeId { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "El tamaño en metros es obligatorio.")]
        [Range(1, double.MaxValue, ErrorMessage = "El tamaño debe ser mayor a 0.")]
        public decimal SizeInMeters { get; set; }

        [Required(ErrorMessage = "El número de habitaciones es obligatorio.")]
        [Range(1, 100, ErrorMessage = "Debe tener al menos una habitación.")]
        public int NumberOfRooms { get; set; }

        [Required(ErrorMessage = "El número de baños es obligatorio.")]
        [Range(1, 100, ErrorMessage = "Debe tener al menos un baño.")]
        public int NumberOfBathrooms { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos una característica.")]
        public List<int> SelectedFeatures { get; set; } = new();

        [Required(ErrorMessage = "Debe agregar al menos una imagen.")]
        public List<string> Images { get; set; } = new();
        public string? AgentId { get; set; }
        public List<PropertyTypeViewModel> PropertyTypes { get; set; } = new();
        public List<SalesTypeViewModel> SalesTypes { get; set; } = new();
        public List<FeatureViewModel> AvailableFeatures { get; set; } = new();
        public List<IFormFile>? ImagesFile { get; set; }
    }
}
