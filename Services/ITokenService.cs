using ZALaw.Api.Models;

namespace ZALaw.Api.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
