using AutoMapper;
using MediatR;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.Features.Commands.Update
{
    public class UpdateFeatureCommand : IRequest<bool>
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    public class UpdateFeatureCommandHandler : IRequestHandler<UpdateFeatureCommand, bool>
    {
        private readonly IFeatureRepository _repository;

        public UpdateFeatureCommandHandler(IFeatureRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateFeatureCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            entity.Name = request.Name;
            entity.Description = request.Description;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
