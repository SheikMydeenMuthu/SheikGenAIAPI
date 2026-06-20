using Auth.Application.Features.Auth.Queries;
using Auth.Application.Interfaces;
using MediatR;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
 {
     private readonly IUserRepository _userRepository;
 
     public GetAllUsersQueryHandler(IUserRepository userRepository)
     {
         _userRepository = userRepository;
     }
 
     public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
     {
         var users = await _userRepository.GetAllUsersAsync();
         return users.Select(u => new UserDto
         {
             Id = u.Id,
             FullName = u.FullName,
             Email = u.Email,
             Role = u.Role.Name,
             IsActive = u.IsActive
         });
     }
 }