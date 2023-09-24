using CouponAPI.Models.DTO;

namespace CouponAPI.Repository.IRepository;

public interface IAuthRepository
{
    bool IsUniqueUser(string username);
    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    Task<UserDTO> Register(RegisterationRequestDTO requestDTO);
}