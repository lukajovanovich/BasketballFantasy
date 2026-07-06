using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Injuries;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.Injuries
{
    public sealed record GetAllInjuriesQuery() : IRequest<List<InjuryDto>>;

    public sealed class GetAllInjuriesQueryHandler : IRequestHandler<GetAllInjuriesQuery, List<InjuryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllInjuriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<InjuryDto>> Handle(GetAllInjuriesQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<List<InjuryDto>>(_unitOfWork.Injuries.GetAllWithPlayer()));
    }

    public sealed record GetInjuryByIdQuery(int Id) : IRequest<InjuryDto>;

    public sealed class GetInjuryByIdQueryHandler : IRequestHandler<GetInjuryByIdQuery, InjuryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetInjuryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<InjuryDto> Handle(GetInjuryByIdQuery request, CancellationToken cancellationToken)
        {
            var injury = _unitOfWork.Injuries.GetByIdWithPlayer(request.Id)
                ?? throw new NotFoundException($"Povreda sa Id={request.Id} nije pronadjena.");

            return Task.FromResult(_mapper.Map<InjuryDto>(injury));
        }
    }

    public sealed record GetActiveInjuriesQuery() : IRequest<List<InjuryDto>>;

    public sealed class GetActiveInjuriesQueryHandler : IRequestHandler<GetActiveInjuriesQuery, List<InjuryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetActiveInjuriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<InjuryDto>> Handle(GetActiveInjuriesQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<List<InjuryDto>>(_unitOfWork.Injuries.GetActiveInjuries()));
    }
}
