namespace RealEstateApp.Core.Application.DTOs.Client
{
    public class ClientDTO
    {
        public string Id { get; set; } = null!;               // ID del cliente
        public string FullName { get; set; } = null!;         // Nombre completo
        public string? ProfileImageUrl { get; set; }          // URL imagen perfil, puede ser null
        public string Email { get; set; } = null!;            // Email cliente (opcional)
        // Otros campos que necesites, como Teléfono, Dirección, etc.
    }
}
