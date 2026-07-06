using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.PlayerStatistics;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.PlayerStatistics
{
    public sealed record GetAllPlayerStatisticsQuery() : IRequest<List<PlayerStatisticDto>>;

    public sealed class GetAllPlayerStatisticsQueryHandler : IRequestHandler<GetAllPlayerStatisticsQuery, List<PlayerStatisticDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPlayerStatisticsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<PlayerStatisticDto>> Handle(GetAllPlayerStatisticsQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<List<PlayerStatisticDto>>(_unitOfWork.PlayerStatistics.GetAllDetailed()));
    }

    public sealed record GetPlayerStatisticByIdQuery(int Id) : IRequest<PlayerStatisticDto>;

    public sealed class GetPlayerStatisticByIdQueryHandler : IRequestHandler<GetPlayerStatisticByIdQuery, PlayerStatisticDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPlayerStatisticByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<PlayerStatisticDto> Handle(GetPlayerStatisticByIdQuery request, CancellationToken cancellationToken)
        {
            var statistic = _unitOfWork.PlayerStatistics.GetByIdDetailed(request.Id)
                ?? throw new NotFoundException($"Statistika sa Id={request.Id} nije pronadjena.");

            return Task.FromResult(_mapper.Map<PlayerStatisticDto>(statistic));
        }
    }

    public sealed record GetTopScorersQuery(int Count) : IRequest<List<TopScorerDto>>;

    public sealed class GetTopScorersQueryHandler : IRequestHandler<GetTopScorersQuery, List<TopScorerDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTopScorersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<TopScorerDto>> Handle(GetTopScorersQuery request, CancellationToken cancellationToken)
        {
            if (request.Count <= 0)
            {
                throw new BadRequestException("Count mora biti veci od 0.");
            }

            var topScorers = _unitOfWork.PlayerStatistics.GetAllDetailed()
                .GroupBy(statistic => new
                {
                    statistic.PlayerId,
                    FirstName = statistic.Player?.FirstName ?? string.Empty,
                    LastName = statistic.Player?.LastName ?? string.Empty,
                    TeamName = statistic.Player?.Team?.Name ?? string.Empty
                })
                .Select(group => new TopScorerDto
                {
                    PlayerId = group.Key.PlayerId,
                    PlayerName = $"{group.Key.FirstName} {group.Key.LastName}".Trim(),
                    TeamName = group.Key.TeamName,
                    GamesPlayed = group.Count(),
                    TotalPoints = group.Sum(item => item.Points)
                })
                .OrderByDescending(item => item.TotalPoints)
                .Take(request.Count)
                .ToList();

            return Task.FromResult(topScorers);
        }
    }

    public sealed record CreatePlayerStatisticCommand(CreatePlayerStatisticRequest Request) : IRequest<PlayerStatisticDto>;

    public sealed class CreatePlayerStatisticCommandHandler : IRequestHandler<CreatePlayerStatisticCommand, PlayerStatisticDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreatePlayerStatisticCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<PlayerStatisticDto> Handle(CreatePlayerStatisticCommand request, CancellationToken cancellationToken)
        {
            if (_unitOfWork.Players.GetById(request.Request.PlayerId) == null)
            {
                throw new BadRequestException($"Igrac sa Id={request.Request.PlayerId} ne postoji.");
            }

            if (_unitOfWork.Matches.GetById(request.Request.MatchId) == null)
            {
                throw new BadRequestException($"Utakmica sa Id={request.Request.MatchId} ne postoji.");
            }

            if (_unitOfWork.PlayerStatistics.Find(statistic =>
                    statistic.PlayerId == request.Request.PlayerId &&
                    statistic.MatchId == request.Request.MatchId).Any())
            {
                throw new BadRequestException("Statistika za ovog igraca i ovu utakmicu vec postoji.");
            }

            var statistic = _mapper.Map<PlayerStatistic>(request.Request);
            _unitOfWork.PlayerStatistics.Add(statistic);
            _unitOfWork.SaveChanges();

            var createdStatistic = _unitOfWork.PlayerStatistics.GetByIdDetailed(statistic.Id)
                ?? throw new NotFoundException($"Statistika sa Id={statistic.Id} nije pronadjena.");

            return Task.FromResult(_mapper.Map<PlayerStatisticDto>(createdStatistic));
        }
    }

    public sealed record UpdatePlayerStatisticCommand(int Id, UpdatePlayerStatisticRequest Request) : IRequest<PlayerStatisticDto>;

    public sealed class UpdatePlayerStatisticCommandHandler : IRequestHandler<UpdatePlayerStatisticCommand, PlayerStatisticDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdatePlayerStatisticCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<PlayerStatisticDto> Handle(UpdatePlayerStatisticCommand request, CancellationToken cancellationToken)
        {
            var statistic = _unitOfWork.PlayerStatistics.GetById(request.Id)
                ?? throw new NotFoundException($"Statistika sa Id={request.Id} nije pronadjena.");

            if (_unitOfWork.Players.GetById(request.Request.PlayerId) == null)
            {
                throw new BadRequestException($"Igrac sa Id={request.Request.PlayerId} ne postoji.");
            }

            if (_unitOfWork.Matches.GetById(request.Request.MatchId) == null)
            {
                throw new BadRequestException($"Utakmica sa Id={request.Request.MatchId} ne postoji.");
            }

            if (_unitOfWork.PlayerStatistics.Find(existingStatistic =>
                    existingStatistic.PlayerId == request.Request.PlayerId &&
                    existingStatistic.MatchId == request.Request.MatchId &&
                    existingStatistic.Id != request.Id).Any())
            {
                throw new BadRequestException("Statistika za ovog igraca i ovu utakmicu vec postoji.");
            }

            _mapper.Map(request.Request, statistic);
            _unitOfWork.PlayerStatistics.Update(statistic);
            _unitOfWork.SaveChanges();

            var updatedStatistic = _unitOfWork.PlayerStatistics.GetByIdDetailed(request.Id)
                ?? throw new NotFoundException($"Statistika sa Id={request.Id} nije pronadjena.");

            return Task.FromResult(_mapper.Map<PlayerStatisticDto>(updatedStatistic));
        }
    }

    public sealed record DeletePlayerStatisticCommand(int Id) : IRequest<Unit>;

    public sealed class DeletePlayerStatisticCommandHandler : IRequestHandler<DeletePlayerStatisticCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePlayerStatisticCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Unit> Handle(DeletePlayerStatisticCommand request, CancellationToken cancellationToken)
        {
            var statistic = _unitOfWork.PlayerStatistics.GetById(request.Id)
                ?? throw new NotFoundException($"Statistika sa Id={request.Id} nije pronadjena.");

            _unitOfWork.PlayerStatistics.Remove(statistic);
            _unitOfWork.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
