using AutoMapper;
using BasketballFantasy.API.Common.Calculations;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.Common.Exports;
using BasketballFantasy.API.DTOs.FantasyTeams;
using BasketballFantasy.API.Features.Common;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.FantasyTeams
{
    public sealed record GetAllFantasyTeamsQuery() : IRequest<List<FantasyTeamDto>>;

    public sealed class GetAllFantasyTeamsQueryHandler : IRequestHandler<GetAllFantasyTeamsQuery, List<FantasyTeamDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllFantasyTeamsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<FantasyTeamDto>> Handle(GetAllFantasyTeamsQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<List<FantasyTeamDto>>(_unitOfWork.FantasyTeams.GetAllDetailed()));
    }

    public sealed record GetFantasyTeamByIdQuery(int Id) : IRequest<FantasyTeamDetailsDto>;

    public sealed class GetFantasyTeamByIdQueryHandler : IRequestHandler<GetFantasyTeamByIdQuery, FantasyTeamDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetFantasyTeamByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<FantasyTeamDetailsDto> Handle(GetFantasyTeamByIdQuery request, CancellationToken cancellationToken)
        {
            var fantasyTeam = _unitOfWork.FantasyTeams.GetByIdWithDetails(request.Id)
                ?? throw new NotFoundException($"Fantasy tim sa Id={request.Id} nije pronadjen.");

            return Task.FromResult(_mapper.Map<FantasyTeamDetailsDto>(fantasyTeam));
        }
    }

    public sealed record GetFantasyTeamPointsQuery(int Id) : IRequest<FantasyTeamPointsDto>;

    public sealed class GetFantasyTeamPointsQueryHandler : IRequestHandler<GetFantasyTeamPointsQuery, FantasyTeamPointsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFantasyTeamPointsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<FantasyTeamPointsDto> Handle(GetFantasyTeamPointsQuery request, CancellationToken cancellationToken)
        {
            var fantasyTeam = _unitOfWork.FantasyTeams.GetByIdWithDetails(request.Id)
                ?? throw new NotFoundException($"Fantasy tim sa Id={request.Id} nije pronadjen.");

            var totalPoints = fantasyTeam.FantasyTeamPlayers.Sum(player =>
                FantasyPointsCalculator.CalculateTotalFantasyPoints(player.Player?.PlayerStatistics ?? new List<PlayerStatistic>()));

            return Task.FromResult(new FantasyTeamPointsDto
            {
                FantasyTeamId = fantasyTeam.Id,
                FantasyTeamName = fantasyTeam.Name,
                TotalFantasyPoints = Math.Round(totalPoints, 2)
            });
        }
    }

    public sealed record ExportFantasyTeamQuery(int Id) : IRequest<ExportFileResponse>;

    public sealed class ExportFantasyTeamQueryHandler : IRequestHandler<ExportFantasyTeamQuery, ExportFileResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExportFantasyTeamQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<ExportFileResponse> Handle(ExportFantasyTeamQuery request, CancellationToken cancellationToken)
        {
            var fantasyTeam = _unitOfWork.FantasyTeams.GetByIdWithDetails(request.Id)
                ?? throw new NotFoundException($"Fantasy tim sa Id={request.Id} nije pronadjen.");

            return Task.FromResult(new ExportFileResponse
            {
                Content = ExcelExportBuilder.ExportFantasyTeamStatistics(fantasyTeam),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"fantasy-team-{request.Id}.xlsx"
            });
        }
    }
}
