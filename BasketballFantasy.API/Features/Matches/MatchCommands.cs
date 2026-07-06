using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Matches;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.Matches
{
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
