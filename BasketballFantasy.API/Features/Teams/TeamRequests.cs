using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Players;
using BasketballFantasy.API.DTOs.Teams;
using BasketballFantasy.Domain.Entities;
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

    public sealed record CreateTeamCommand(CreateTeamRequest Request) : IRequest<TeamDto>;

    public sealed class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, TeamDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTeamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<TeamDto> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = _mapper.Map<Team>(request.Request);
            _unitOfWork.Teams.Add(team);
            _unitOfWork.SaveChanges();

            return Task.FromResult(_mapper.Map<TeamDto>(team));
        }
    }

    public sealed record UpdateTeamCommand(int Id, UpdateTeamRequest Request) : IRequest<TeamDto>;

    public sealed class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, TeamDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTeamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<TeamDto> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = _unitOfWork.Teams.GetById(request.Id)
                ?? throw new NotFoundException($"Tim sa Id={request.Id} nije pronadjen.");

            _mapper.Map(request.Request, team);
            _unitOfWork.Teams.Update(team);
            _unitOfWork.SaveChanges();

            return Task.FromResult(_mapper.Map<TeamDto>(team));
        }
    }

    public sealed record DeleteTeamCommand(int Id) : IRequest<Unit>;

    public sealed class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTeamCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Unit> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            var team = _unitOfWork.Teams.GetById(request.Id)
                ?? throw new NotFoundException($"Tim sa Id={request.Id} nije pronadjen.");

            var hasPlayers = _unitOfWork.Players.Find(player => player.TeamId == request.Id).Any();
            var hasCoaches = _unitOfWork.Coaches.Find(coach => coach.TeamId == request.Id).Any();
            var hasMatches = _unitOfWork.Matches.Find(match => match.HomeTeamId == request.Id || match.AwayTeamId == request.Id).Any();

            if (hasPlayers || hasCoaches || hasMatches)
            {
                throw new BadRequestException("Tim ne moze da se obrise dok ima igrace, trenere ili utakmice.");
            }

            _unitOfWork.Teams.Remove(team);
            _unitOfWork.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
