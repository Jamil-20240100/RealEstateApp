using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.Properties.Queries.GetById
{
    public class GetByIdPropertyQuery : IRequest<PropertyForApiDTO>
    {
        public required int Id { get; set; }
    }

    public class GetByIdPropertyQueryHandler : IRequestHandler<GetByIdPropertyQuery, PropertyForApiDTO>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public GetByIdPropertyQueryHandler(IPropertyRepository propertyRepository, IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<PropertyForApiDTO> Handle(GetByIdPropertyQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _propertyRepository.GetAllQueryWithInclude(["PropertyType", "SalesType", "Features"]);
            Domain.Entities.Property? entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == query.Id, cancellationToken: cancellationToken);

            if (entity == null) throw new ArgumentException("Property not found with this id");

            var dto = _mapper.Map<PropertyForApiDTO>(entity);

            return dto;
        }
    }
}
