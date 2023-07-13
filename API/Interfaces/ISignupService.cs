using API.Request;
using API.Response;

namespace API.Interfaces;

public interface ISignupService
{
    Task<SignupResponse> Signup(SignupRequest signupRequest);
}