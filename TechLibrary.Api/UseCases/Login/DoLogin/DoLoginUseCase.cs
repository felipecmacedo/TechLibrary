using TechLibrary.Api.Infraestrutucture.DataAccess;
using TechLibrary.Api.Infraestrutucture.Security.Cryptography;
using TechLibrary.Api.Infraestrutucture.Security.Tokens.Access;
using TechLibrary.Communication.Reponses;
using TechLibrary.Communication.Requests;
using TechLibrary.Exception;

namespace TechLibrary.Api.UseCases.Login.DoLogin;

public class DoLoginUseCase
{
    public ResponseRegisteredUserJson Execute(RequestLoginJson request)
    {
        var dbContext = new TechLibraryDbContext();

        var entity = dbContext.Users.FirstOrDefault(entity => entity.Email.Equals(request.Email));

        if(entity is null)
            throw new InvalidLoginException();

        var cryptography = new BCryptAlgorithm();
        var passwordIsValid = cryptography.Verify(request.Password, entity);
        if (passwordIsValid == false)
            throw new InvalidLoginException();

        var tokenGenerator = new JwtTokenGenerator();

        return new ResponseRegisteredUserJson
        {
            Name = entity.Name,
            AccessToken = tokenGenerator.Generate(entity)
        };
    }
}
