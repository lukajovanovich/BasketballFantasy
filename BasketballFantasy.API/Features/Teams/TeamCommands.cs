using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Teams;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.Teams
{
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
