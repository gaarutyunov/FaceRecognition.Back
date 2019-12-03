using System.Threading.Tasks;
using FaceRecognition.Back.Api.Dtos;
using FaceRecognition.Back.Api.Responses;

namespace FaceRecognition.Back.Api.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> Register(CreateUserDto createUserDto);
        Task<UserResponse> Login(LoginUserDto dto);
    }
}