using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.Properties.Queries.GetByCode
{
    public class GetByCodePropertyQuery : IRequest<PropertyForApiDTO>
    {
        public required string Code { get; set; }
    }

    public class GetByCodePropertyQueryHandler : IRequestHandler<GetByCodePropertyQuery, PropertyForApiDTO>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public GetByCodePropertyQueryHandler(IPropertyRepository propertyRepository, IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<PropertyForApiDTO> Handle(GetByCodePropertyQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _propertyRepository.GetAllQueryWithInclude(["PropertyType", "SalesType", "Features"]);
            Domain.Entities.Property? entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Code == query.Code, cancellationToken: cancellationToken);

            if (entity == null) throw new ArgumentException("Property not found with this code");

            var dto = _mapper.Map<PropertyForApiDTO>(entity);

            return dto;
        }
    }
}
