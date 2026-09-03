using MediatR;

namespace Auction.Application.Users.GetMe;

public record GetMeQuery(string UserId) : IRequest<GetMeResult?>;
