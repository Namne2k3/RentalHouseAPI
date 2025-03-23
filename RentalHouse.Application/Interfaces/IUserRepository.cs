using RentalHouse.Application.DTOs;
using RentalHouse.SharedLibrary.Responses;

namespace RentalHouse.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Response> Register(UserDTO userDTO);
        Task<LoginResponse> Login(LoginDTO loginDTO);
        Task<GetUserDTO> GetUser(int userId);
        Task<Response> UpdateUser(ChangeUserDTO changeUserDTO);

        Task<Response> ChangePassword(int userId, string newPassword, string currentPassword);
        Task<IEnumerable<GetUserDTO>> GetAllUsers();
    }
}
