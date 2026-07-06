using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.PlayerStatistics;
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
}
