using MediatR;

namespace Auction.Application.Users.GetMe;

public class GetMeHandler(IUserRepository userRepository) : IRequestHandler<GetMeQuery, GetMeResult?>
{
    public async Task<GetMeResult?> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        return user is null
            ? null
            : new GetMeResult(user.Id, user.Email, user.Name, user.AvatarUrl);
    }
}
