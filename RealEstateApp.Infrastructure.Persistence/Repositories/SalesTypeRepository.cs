using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class SalesTypeRepository : GenericRepository<SalesType>, ISalesTypeRepository
    {
        public SalesTypeRepository(RealEstateContext context) : base(context)
        {
        }
    }
}