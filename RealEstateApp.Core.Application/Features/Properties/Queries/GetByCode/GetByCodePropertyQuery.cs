using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RealEstateApp.Core.Application.Features.Properties.Queries.GetByCode
{
    /// <summary>
    /// Query to retrieve a property by its unique code
    /// </summary>
    public class GetByCodePropertyQuery : IRequest<PropertyForApiDTO>
    {
        /// <summary>
        /// The unique code of the property
        /// </summary>
        /// <example>PROP-2025-001</example>
        [SwaggerParameter(Description = "The unique code of the property to retrieve")]
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
            Domain.Entities.Property? entity = listEntitiesQuery.FirstOrDefault(fd => fd.Code == query.Code);

            if (entity == null) throw new ApiException("Property not found with this id", (int)HttpStatusCode.NotFound);

            var dto = _mapper.Map<PropertyForApiDTO>(entity);

            return dto;
        }
    }
}
