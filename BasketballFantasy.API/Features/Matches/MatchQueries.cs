using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.Common.Exports;
using BasketballFantasy.API.DTOs.Matches;
using BasketballFantasy.API.Features.Common;
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

        public ExportMatchStatisticsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<ExportFileResponse> Handle(ExportMatchStatisticsQuery request, CancellationToken cancellationToken)
        {
            var match = _unitOfWork.Matches.GetByIdWithStatistics(request.Id)
                ?? throw new NotFoundException($"Utakmica sa Id={request.Id} nije pronadjena.");

            return Task.FromResult(new ExportFileResponse
            {
                Content = ExcelExportBuilder.ExportMatchStatistics(match),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"match-statistics-{request.Id}.xlsx"
            });
        }
    }
}
