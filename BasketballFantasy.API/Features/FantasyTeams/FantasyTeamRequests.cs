using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.FantasyTeams;
using BasketballFantasy.API.Features.Common;
using BasketballFantasy.API.Services;
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
        private readonly FantasyPointsService _fantasyPointsService;

        public GetFantasyTeamPointsQueryHandler(IUnitOfWork unitOfWork, FantasyPointsService fantasyPointsService)
        {
            _unitOfWork = unitOfWork;
            _fantasyPointsService = fantasyPointsService;
        }

        public Task<FantasyTeamPointsDto> Handle(GetFantasyTeamPointsQuery request, CancellationToken cancellationToken)
        {
            var fantasyTeam = _unitOfWork.FantasyTeams.GetByIdWithDetails(request.Id)
                ?? throw new NotFoundException($"Fantasy tim sa Id={request.Id} nije pronadjen.");

            var totalPoints = fantasyTeam.FantasyTeamPlayers.Sum(player =>
                _fantasyPointsService.CalculateTotalFantasyPoints(player.Player?.PlayerStatistics ?? new List<PlayerStatistic>()));

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
        private readonly ExcelExportService _excelExportService;
        private readonly FantasyPointsService _fantasyPointsService;

        public ExportFantasyTeamQueryHandler(
            IUnitOfWork unitOfWork,
            ExcelExportService excelExportService,
            FantasyPointsService fantasyPointsService)
        {
            _unitOfWork = unitOfWork;
            _excelExportService = excelExportService;
            _fantasyPointsService = fantasyPointsService;
        }

        public Task<ExportFileResponse> Handle(ExportFantasyTeamQuery request, CancellationToken cancellationToken)
        {
            var fantasyTeam = _unitOfWork.FantasyTeams.GetByIdWithDetails(request.Id)
                ?? throw new NotFoundException($"Fantasy tim sa Id={request.Id} nije pronadjen.");

            return Task.FromResult(new ExportFileResponse
            {
                Content = _excelExportService.ExportFantasyTeamStatistics(fantasyTeam, _fantasyPointsService),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"fantasy-team-{request.Id}.xlsx"
            });
        }
    }

    public sealed record CreateFantasyTeamCommand(CreateFantasyTeamRequest Request) : IRequest<FantasyTeamDetailsDto>;

    public sealed class CreateFantasyTeamCommandHandler : IRequestHandler<CreateFantasyTeamCommand, FantasyTeamDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateFantasyTeamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<FantasyTeamDetailsDto> Handle(CreateFantasyTeamCommand request, CancellationToken cancellationToken)
        {
            if (_unitOfWork.Coaches.GetById(request.Request.CoachId) == null)
            {
                throw new BadRequestException($"Trener sa Id={request.Request.CoachId} ne postoji.");
            }

            var fantasyTeam = _mapper.Map<FantasyTeam>(request.Request);
            _unitOfWork.FantasyTeams.Add(fantasyTeam);
            _unitOfWork.SaveChanges();

            var createdFantasyTeam = _unitOfWork.FantasyTeams.GetByIdWithDetails(fantasyTeam.Id)
                ?? throw new NotFoundException($"Fantasy tim sa Id={fantasyTeam.Id} nije pronadjen.");

            return Task.FromResult(_mapper.Map<FantasyTeamDetailsDto>(createdFantasyTeam));
        }
    }

    public sealed record UpdateFantasyTeamCommand(int Id, UpdateFantasyTeamRequest Request) : IRequest<FantasyTeamDetailsDto>;

    public sealed class UpdateFantasyTeamCommandHandler : IRequestHandler<UpdateFantasyTeamCommand, FantasyTeamDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateFantasyTeamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<FantasyTeamDetailsDto> Handle(UpdateFantasyTeamCommand request, CancellationToken cancellationToken)
        {
            var fantasyTeam = _unitOfWork.FantasyTeams.GetById(request.Id)
                ?? throw new NotFoundException($"Fantasy tim sa Id={request.Id} nije pronadjen.");

            if (_unitOfWork.Coaches.GetById(request.Request.CoachId) == null)
            {
                throw new BadRequestException($"Trener sa Id={request.Request.CoachId} ne postoji.");
            }

            _mapper.Map(request.Request, fantasyTeam);
            _unitOfWork.FantasyTeams.Update(fantasyTeam);
            _unitOfWork.SaveChanges();

            var updatedFantasyTeam = _unitOfWork.FantasyTeams.GetByIdWithDetails(request.Id)
                ?? throw new NotFoundException($"Fantasy tim sa Id={request.Id} nije pronadjen.");

            return Task.FromResult(_mapper.Map<FantasyTeamDetailsDto>(updatedFantasyTeam));
        }
    }

    public sealed record AddFantasyTeamPlayerCommand(int FantasyTeamId, AddFantasyTeamPlayerRequest Request) : IRequest<FantasyTeamDetailsDto>;

    public sealed class AddFantasyTeamPlayerCommandHandler : IRequestHandler<AddFantasyTeamPlayerCommand, FantasyTeamDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddFantasyTeamPlayerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<FantasyTeamDetailsDto> Handle(AddFantasyTeamPlayerCommand request, CancellationToken cancellationToken)
        {
            if (_unitOfWork.FantasyTeams.GetById(request.FantasyTeamId) == null)
            {
                throw new NotFoundException($"Fantasy tim sa Id={request.FantasyTeamId} nije pronadjen.");
            }

            if (_unitOfWork.Players.GetById(request.Request.PlayerId) == null)
            {
                throw new BadRequestException($"Igrac sa Id={request.Request.PlayerId} ne postoji.");
            }

            if (_unitOfWork.FantasyTeamPlayers.Find(player =>
                    player.FantasyTeamId == request.FantasyTeamId &&
                    player.PlayerId == request.Request.PlayerId).Any())
            {
                throw new BadRequestException("Isti igrac ne moze biti dodat dva puta u isti fantasy tim.");
            }

            var starterCount = _unitOfWork.FantasyTeamPlayers.Find(player =>
                player.FantasyTeamId == request.FantasyTeamId &&
                player.Role == FantasyRole.Starter).Count();
            var benchCount = _unitOfWork.FantasyTeamPlayers.Find(player =>
                player.FantasyTeamId == request.FantasyTeamId &&
                player.Role == FantasyRole.Bench).Count();

            if (request.Request.Role == FantasyRole.Starter && starterCount >= 5)
            {
                throw new BadRequestException("Fantasy tim ne moze imati vise od 5 startera.");
            }

            if (request.Request.Role == FantasyRole.Bench && benchCount >= 4)
            {
                throw new BadRequestException("Fantasy tim ne moze imati vise od 4 igraca na klupi.");
            }

            var fantasyTeamPlayer = new FantasyTeamPlayer
            {
                FantasyTeamId = request.FantasyTeamId,
                PlayerId = request.Request.PlayerId,
                Role = request.Request.Role
            };

            _unitOfWork.FantasyTeamPlayers.Add(fantasyTeamPlayer);
            _unitOfWork.SaveChanges();

            var updatedFantasyTeam = _unitOfWork.FantasyTeams.GetByIdWithDetails(request.FantasyTeamId)
                ?? throw new NotFoundException($"Fantasy tim sa Id={request.FantasyTeamId} nije pronadjen.");

            return Task.FromResult(_mapper.Map<FantasyTeamDetailsDto>(updatedFantasyTeam));
        }
    }

    public sealed record DeleteFantasyTeamCommand(int Id) : IRequest<Unit>;

    public sealed class DeleteFantasyTeamCommandHandler : IRequestHandler<DeleteFantasyTeamCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFantasyTeamCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Unit> Handle(DeleteFantasyTeamCommand request, CancellationToken cancellationToken)
        {
            var fantasyTeam = _unitOfWork.FantasyTeams.GetById(request.Id)
                ?? throw new NotFoundException($"Fantasy tim sa Id={request.Id} nije pronadjen.");

            _unitOfWork.FantasyTeams.Remove(fantasyTeam);
            _unitOfWork.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
