using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Injuries;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.Injuries
{
    public sealed record GetAllInjuriesQuery() : IRequest<List<InjuryDto>>;

    public sealed class GetAllInjuriesQueryHandler : IRequestHandler<GetAllInjuriesQuery, List<InjuryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllInjuriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<InjuryDto>> Handle(GetAllInjuriesQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<List<InjuryDto>>(_unitOfWork.Injuries.GetAllWithPlayer()));
    }

    public sealed record GetInjuryByIdQuery(int Id) : IRequest<InjuryDto>;

    public sealed class GetInjuryByIdQueryHandler : IRequestHandler<GetInjuryByIdQuery, InjuryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetInjuryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<InjuryDto> Handle(GetInjuryByIdQuery request, CancellationToken cancellationToken)
        {
            var injury = _unitOfWork.Injuries.GetByIdWithPlayer(request.Id)
                ?? throw new NotFoundException($"Povreda sa Id={request.Id} nije pronadjena.");

            return Task.FromResult(_mapper.Map<InjuryDto>(injury));
        }
    }

    public sealed record GetActiveInjuriesQuery() : IRequest<List<InjuryDto>>;

    public sealed class GetActiveInjuriesQueryHandler : IRequestHandler<GetActiveInjuriesQuery, List<InjuryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetActiveInjuriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<InjuryDto>> Handle(GetActiveInjuriesQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<List<InjuryDto>>(_unitOfWork.Injuries.GetActiveInjuries()));
    }

    public sealed record CreateInjuryCommand(CreateInjuryRequest Request) : IRequest<InjuryDto>;

    public sealed class CreateInjuryCommandHandler : IRequestHandler<CreateInjuryCommand, InjuryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateInjuryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<InjuryDto> Handle(CreateInjuryCommand request, CancellationToken cancellationToken)
        {
            if (_unitOfWork.Players.GetById(request.Request.PlayerId) == null)
            {
                throw new BadRequestException($"Igrac sa Id={request.Request.PlayerId} ne postoji.");
            }

            var injury = _mapper.Map<Injury>(request.Request);
            _unitOfWork.Injuries.Add(injury);
            _unitOfWork.SaveChanges();

            var createdInjury = _unitOfWork.Injuries.GetByIdWithPlayer(injury.Id)
                ?? throw new NotFoundException($"Povreda sa Id={injury.Id} nije pronadjena.");

            return Task.FromResult(_mapper.Map<InjuryDto>(createdInjury));
        }
    }

    public sealed record UpdateInjuryCommand(int Id, UpdateInjuryRequest Request) : IRequest<InjuryDto>;

    public sealed class UpdateInjuryCommandHandler : IRequestHandler<UpdateInjuryCommand, InjuryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateInjuryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<InjuryDto> Handle(UpdateInjuryCommand request, CancellationToken cancellationToken)
        {
            var injury = _unitOfWork.Injuries.GetById(request.Id)
                ?? throw new NotFoundException($"Povreda sa Id={request.Id} nije pronadjena.");

            if (_unitOfWork.Players.GetById(request.Request.PlayerId) == null)
            {
                throw new BadRequestException($"Igrac sa Id={request.Request.PlayerId} ne postoji.");
            }

            _mapper.Map(request.Request, injury);
            _unitOfWork.Injuries.Update(injury);
            _unitOfWork.SaveChanges();

            var updatedInjury = _unitOfWork.Injuries.GetByIdWithPlayer(request.Id)
                ?? throw new NotFoundException($"Povreda sa Id={request.Id} nije pronadjena.");

            return Task.FromResult(_mapper.Map<InjuryDto>(updatedInjury));
        }
    }

    public sealed record DeleteInjuryCommand(int Id) : IRequest<Unit>;

    public sealed class DeleteInjuryCommandHandler : IRequestHandler<DeleteInjuryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteInjuryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Unit> Handle(DeleteInjuryCommand request, CancellationToken cancellationToken)
        {
            var injury = _unitOfWork.Injuries.GetById(request.Id)
                ?? throw new NotFoundException($"Povreda sa Id={request.Id} nije pronadjena.");

            _unitOfWork.Injuries.Remove(injury);
            _unitOfWork.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
