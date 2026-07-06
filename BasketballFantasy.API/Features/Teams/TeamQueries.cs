using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Players;
using BasketballFantasy.API.DTOs.Teams;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.Teams
{
    public sealed record GetAllTeamsQuery() : IRequest<List<TeamDto>>;

    public sealed class GetAllTeamsQueryHandler : IRequestHandler<GetAllTeamsQuery, List<TeamDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllTeamsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<TeamDto>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<List<TeamDto>>(_unitOfWork.Teams.GetAll()));
    }

    public sealed record GetTeamByIdQuery(int Id) : IRequest<TeamDto>;

    public sealed class GetTeamByIdQueryHandler : IRequestHandler<GetTeamByIdQuery, TeamDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTeamByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<TeamDto> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
        {
            var team = _unitOfWork.Teams.GetById(request.Id)
                ?? throw new NotFoundException($"Tim sa Id={request.Id} nije pronadjen.");

            return Task.FromResult(_mapper.Map<TeamDto>(team));
        }
    }

    public sealed record GetTeamPlayersQuery(int Id) : IRequest<List<PlayerDto>>;

    public sealed class GetTeamPlayersQueryHandler : IRequestHandler<GetTeamPlayersQuery, List<PlayerDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTeamPlayersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<PlayerDto>> Handle(GetTeamPlayersQuery request, CancellationToken cancellationToken)
        {
            if (_unitOfWork.Teams.GetById(request.Id) == null)
            {
                throw new NotFoundException($"Tim sa Id={request.Id} nije pronadjen.");
            }

            return Task.FromResult(_mapper.Map<List<PlayerDto>>(_unitOfWork.Players.GetByTeamId(request.Id)));
        }
    }
}
