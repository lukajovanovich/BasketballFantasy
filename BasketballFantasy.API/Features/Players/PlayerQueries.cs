using AutoMapper;
using BasketballFantasy.API.Common.Calculations;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Players;
using BasketballFantasy.API.DTOs.PlayerStatistics;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.Players
{
    public sealed record GetAllPlayersQuery() : IRequest<List<PlayerDto>>;

    public sealed class GetAllPlayersQueryHandler : IRequestHandler<GetAllPlayersQuery, List<PlayerDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPlayersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<PlayerDto>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<List<PlayerDto>>(_unitOfWork.Players.GetAllWithTeam()));
    }

    public sealed record GetPlayerByIdQuery(int Id) : IRequest<PlayerDto>;

    public sealed class GetPlayerByIdQueryHandler : IRequestHandler<GetPlayerByIdQuery, PlayerDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPlayerByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<PlayerDto> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            var player = _unitOfWork.Players.GetByIdWithTeam(request.Id)
                ?? throw new NotFoundException($"Igrac sa Id={request.Id} nije pronadjen.");

            return Task.FromResult(_mapper.Map<PlayerDto>(player));
        }
    }

    public sealed record GetPlayerStatisticsQuery(int Id) : IRequest<List<PlayerStatisticDto>>;

    public sealed class GetPlayerStatisticsQueryHandler : IRequestHandler<GetPlayerStatisticsQuery, List<PlayerStatisticDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPlayerStatisticsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<PlayerStatisticDto>> Handle(GetPlayerStatisticsQuery request, CancellationToken cancellationToken)
        {
            if (_unitOfWork.Players.GetById(request.Id) == null)
            {
                throw new NotFoundException($"Igrac sa Id={request.Id} nije pronadjen.");
            }

            return Task.FromResult(_mapper.Map<List<PlayerStatisticDto>>(_unitOfWork.PlayerStatistics.GetByPlayerId(request.Id)));
        }
    }

    public sealed record GetPlayerTotalsQuery(int Id) : IRequest<PlayerTotalsDto>;

    public sealed class GetPlayerTotalsQueryHandler : IRequestHandler<GetPlayerTotalsQuery, PlayerTotalsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPlayerTotalsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<PlayerTotalsDto> Handle(GetPlayerTotalsQuery request, CancellationToken cancellationToken)
        {
            var player = _unitOfWork.Players.GetByIdWithStatistics(request.Id)
                ?? throw new NotFoundException($"Igrac sa Id={request.Id} nije pronadjen.");

            var statistics = player.PlayerStatistics;

            var totals = new PlayerTotalsDto
            {
                PlayerId = player.Id,
                PlayerName = $"{player.FirstName} {player.LastName}",
                GamesPlayed = statistics.Count,
                Points = statistics.Sum(statistic => statistic.Points),
                Assists = statistics.Sum(statistic => statistic.Assists),
                Rebounds = statistics.Sum(statistic => statistic.Rebounds),
                Minutes = statistics.Sum(statistic => statistic.Minutes),
                Steals = statistics.Sum(statistic => statistic.Steals),
                Blocks = statistics.Sum(statistic => statistic.Blocks),
                Turnovers = statistics.Sum(statistic => statistic.Turnovers),
                FantasyPoints = FantasyPointsCalculator.CalculateTotalFantasyPoints(statistics)
            };

            return Task.FromResult(totals);
        }
    }
}
