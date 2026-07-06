using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Coaches;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.Coaches
{
    public sealed record GetAllCoachesQuery() : IRequest<List<CoachDto>>;

    public sealed class GetAllCoachesQueryHandler : IRequestHandler<GetAllCoachesQuery, List<CoachDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCoachesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<CoachDto>> Handle(GetAllCoachesQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<List<CoachDto>>(_unitOfWork.Coaches.GetAllWithTeam()));
    }

    public sealed record GetCoachByIdQuery(int Id) : IRequest<CoachDto>;

    public sealed class GetCoachByIdQueryHandler : IRequestHandler<GetCoachByIdQuery, CoachDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCoachByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<CoachDto> Handle(GetCoachByIdQuery request, CancellationToken cancellationToken)
        {
            var coach = _unitOfWork.Coaches.GetByIdWithTeam(request.Id)
                ?? throw new NotFoundException($"Trener sa Id={request.Id} nije pronadjen.");

            return Task.FromResult(_mapper.Map<CoachDto>(coach));
        }
    }
}
