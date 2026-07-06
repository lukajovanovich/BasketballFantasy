using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Coaches;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.Coaches
{
    public sealed record CreateCoachCommand(CreateCoachRequest Request) : IRequest<CoachDto>;

    public sealed class CreateCoachCommandHandler : IRequestHandler<CreateCoachCommand, CoachDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCoachCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<CoachDto> Handle(CreateCoachCommand request, CancellationToken cancellationToken)
        {
            if (_unitOfWork.Teams.GetById(request.Request.TeamId) == null)
            {
                throw new BadRequestException($"Tim sa Id={request.Request.TeamId} ne postoji.");
            }

            var coach = _mapper.Map<Coach>(request.Request);
            _unitOfWork.Coaches.Add(coach);
            _unitOfWork.SaveChanges();

            var createdCoach = _unitOfWork.Coaches.GetByIdWithTeam(coach.Id)
                ?? throw new NotFoundException($"Trener sa Id={coach.Id} nije pronadjen.");

            return Task.FromResult(_mapper.Map<CoachDto>(createdCoach));
        }
    }

    public sealed record UpdateCoachCommand(int Id, UpdateCoachRequest Request) : IRequest<CoachDto>;

    public sealed class UpdateCoachCommandHandler : IRequestHandler<UpdateCoachCommand, CoachDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCoachCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<CoachDto> Handle(UpdateCoachCommand request, CancellationToken cancellationToken)
        {
            var coach = _unitOfWork.Coaches.GetById(request.Id)
                ?? throw new NotFoundException($"Trener sa Id={request.Id} nije pronadjen.");

            if (_unitOfWork.Teams.GetById(request.Request.TeamId) == null)
            {
                throw new BadRequestException($"Tim sa Id={request.Request.TeamId} ne postoji.");
            }

            _mapper.Map(request.Request, coach);
            _unitOfWork.Coaches.Update(coach);
            _unitOfWork.SaveChanges();

            var updatedCoach = _unitOfWork.Coaches.GetByIdWithTeam(request.Id)
                ?? throw new NotFoundException($"Trener sa Id={request.Id} nije pronadjen.");

            return Task.FromResult(_mapper.Map<CoachDto>(updatedCoach));
        }
    }

    public sealed record DeleteCoachCommand(int Id) : IRequest<Unit>;

    public sealed class DeleteCoachCommandHandler : IRequestHandler<DeleteCoachCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCoachCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Unit> Handle(DeleteCoachCommand request, CancellationToken cancellationToken)
        {
            var coach = _unitOfWork.Coaches.GetById(request.Id)
                ?? throw new NotFoundException($"Trener sa Id={request.Id} nije pronadjen.");

            if (_unitOfWork.FantasyTeams.Find(fantasyTeam => fantasyTeam.CoachId == request.Id).Any())
            {
                throw new BadRequestException("Trener ne moze da se obrise dok je vezan za fantasy tim.");
            }

            _unitOfWork.Coaches.Remove(coach);
            _unitOfWork.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
