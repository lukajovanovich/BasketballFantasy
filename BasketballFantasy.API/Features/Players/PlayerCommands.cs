using AutoMapper;
using BasketballFantasy.API.Common.Exceptions;
using BasketballFantasy.API.DTOs.Players;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using MediatR;

namespace BasketballFantasy.API.Features.Players
{
    public sealed record CreatePlayerCommand(CreatePlayerRequest Request) : IRequest<PlayerDto>;

    public sealed class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, PlayerDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreatePlayerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<PlayerDto> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            if (_unitOfWork.Teams.GetById(request.Request.TeamId) == null)
            {
                throw new BadRequestException($"Tim sa Id={request.Request.TeamId} ne postoji.");
            }

            var player = _mapper.Map<Player>(request.Request);
            _unitOfWork.Players.Add(player);
            _unitOfWork.SaveChanges();

            var createdPlayer = _unitOfWork.Players.GetByIdWithTeam(player.Id)
                ?? throw new NotFoundException($"Igrac sa Id={player.Id} nije pronadjen.");

            return Task.FromResult(_mapper.Map<PlayerDto>(createdPlayer));
        }
    }

    public sealed record UpdatePlayerCommand(int Id, UpdatePlayerRequest Request) : IRequest<PlayerDto>;

    public sealed class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, PlayerDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdatePlayerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<PlayerDto> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            var player = _unitOfWork.Players.GetById(request.Id)
                ?? throw new NotFoundException($"Igrac sa Id={request.Id} nije pronadjen.");

            if (_unitOfWork.Teams.GetById(request.Request.TeamId) == null)
            {
                throw new BadRequestException($"Tim sa Id={request.Request.TeamId} ne postoji.");
            }

            _mapper.Map(request.Request, player);
            _unitOfWork.Players.Update(player);
            _unitOfWork.SaveChanges();

            var updatedPlayer = _unitOfWork.Players.GetByIdWithTeam(request.Id)
                ?? throw new NotFoundException($"Igrac sa Id={request.Id} nije pronadjen.");

            return Task.FromResult(_mapper.Map<PlayerDto>(updatedPlayer));
        }
    }

    public sealed record DeletePlayerCommand(int Id) : IRequest<Unit>;

    public sealed class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePlayerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Unit> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
        {
            var player = _unitOfWork.Players.GetById(request.Id)
                ?? throw new NotFoundException($"Igrac sa Id={request.Id} nije pronadjen.");

            _unitOfWork.Players.Remove(player);
            _unitOfWork.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
