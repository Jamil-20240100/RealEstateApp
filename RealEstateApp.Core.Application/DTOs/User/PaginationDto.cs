using RealEstateApp.Core.Application.DTOs.User;

namespace RealEstateApp.Core.Application.DTOs.User
{
    public class PaginationDto<T>
    {
        public List<T> Data { get; set; } = new();
        public PaginationInfoDto Paginacion { get; set; } = new();
    }
}
