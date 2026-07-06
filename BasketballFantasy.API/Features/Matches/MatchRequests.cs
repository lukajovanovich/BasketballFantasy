using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Matches;
using BasketballFantasy.API.Features.Common;
using BasketballFantasy.API.Services;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.Matches
{
    public sealed record GetAllMatchesQuery() : IRequest<List<MatchDto>>;

    public sealed class GetAllMatchesQueryHandler : IRequestHandler<GetAllMatchesQuery, List<MatchDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllMatchesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<MatchDto>> Handle(GetAllMatchesQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<List<MatchDto>>(_unitOfWork.Matches.GetAllWithTeams()));
    }

    public sealed record GetMatchByIdQuery(int Id) : IRequest<MatchDto>;

    public sealed class GetMatchByIdQueryHandler : IRequestHandler<GetMatchByIdQuery, MatchDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMatchByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<MatchDto> Handle(GetMatchByIdQuery request, CancellationToken cancellationToken)
        {
            var match = _unitOfWork.Matches.GetByIdWithDetails(request.Id)
                ?? throw new NotFoundException($"Utakmica sa Id={request.Id} nije pronadjena.");

            return Task.FromResult(_mapper.Map<MatchDto>(match));
        }
    }

    public sealed record ExportMatchStatisticsQuery(int Id) : IRequest<ExportFileResponse>;

    public sealed class ExportMatchStatisticsQueryHandler : IRequestHandler<ExportMatchStatisticsQuery, ExportFileResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ExcelExportService _excelExportService;
        private readonly FantasyPointsService _fantasyPointsService;

        public ExportMatchStatisticsQueryHandler(
            IUnitOfWork unitOfWork,
            ExcelExportService excelExportService,
            FantasyPointsService fantasyPointsService)
        {
            _unitOfWork = unitOfWork;
            _excelExportService = excelExportService;
            _fantasyPointsService = fantasyPointsService;
        }

        public Task<ExportFileResponse> Handle(ExportMatchStatisticsQuery request, CancellationToken cancellationToken)
        {
            var match = _unitOfWork.Matches.GetByIdWithStatistics(request.Id)
                ?? throw new NotFoundException($"Utakmica sa Id={request.Id} nije pronadjena.");

            return Task.FromResult(new ExportFileResponse
            {
                Content = _excelExportService.ExportMatchStatistics(match, _fantasyPointsService),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"match-statistics-{request.Id}.xlsx"
            });
        }
    }

    public sealed record CreateMatchCommand(CreateMatchRequest Request) : IRequest<MatchDto>;

    public sealed class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, MatchDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateMatchCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<MatchDto> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
        {
            if (_unitOfWork.Teams.GetById(request.Request.HomeTeamId) == null ||
                _unitOfWork.Teams.GetById(request.Request.AwayTeamId) == null)
            {
                throw new BadRequestException("Jedan ili oba tima ne postoje.");
            }

            var match = _mapper.Map<Match>(request.Request);
            _unitOfWork.Matches.Add(match);
            _unitOfWork.SaveChanges();

            var createdMatch = _unitOfWork.Matches.GetByIdWithDetails(match.Id)
                ?? throw new NotFoundException($"Utakmica sa Id={match.Id} nije pronadjena.");

            return Task.FromResult(_mapper.Map<MatchDto>(createdMatch));
        }
    }

    public sealed record UpdateMatchCommand(int Id, UpdateMatchRequest Request) : IRequest<MatchDto>;

    public sealed class UpdateMatchCommandHandler : IRequestHandler<UpdateMatchCommand, MatchDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateMatchCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<MatchDto> Handle(UpdateMatchCommand request, CancellationToken cancellationToken)
        {
            var match = _unitOfWork.Matches.GetById(request.Id)
                ?? throw new NotFoundException($"Utakmica sa Id={request.Id} nije pronadjena.");

            if (_unitOfWork.Teams.GetById(request.Request.HomeTeamId) == null ||
                _unitOfWork.Teams.GetById(request.Request.AwayTeamId) == null)
            {
                throw new BadRequestException("Jedan ili oba tima ne postoje.");
            }

            _mapper.Map(request.Request, match);
            _unitOfWork.Matches.Update(match);
            _unitOfWork.SaveChanges();

            var updatedMatch = _unitOfWork.Matches.GetByIdWithDetails(request.Id)
                ?? throw new NotFoundException($"Utakmica sa Id={request.Id} nije pronadjena.");

            return Task.FromResult(_mapper.Map<MatchDto>(updatedMatch));
        }
    }

    public sealed record DeleteMatchCommand(int Id) : IRequest<Unit>;

    public sealed class DeleteMatchCommandHandler : IRequestHandler<DeleteMatchCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMatchCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Unit> Handle(DeleteMatchCommand request, CancellationToken cancellationToken)
        {
            var match = _unitOfWork.Matches.GetById(request.Id)
                ?? throw new NotFoundException($"Utakmica sa Id={request.Id} nije pronadjena.");

            _unitOfWork.Matches.Remove(match);
            _unitOfWork.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
