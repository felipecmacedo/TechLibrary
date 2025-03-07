﻿using FluentValidation.Results;
using TechLibrary.Api.Domain.Entities;
using TechLibrary.Api.Infraestrutucture.DataAccess;
using TechLibrary.Api.Infraestrutucture.Security.Cryptography;
using TechLibrary.Api.Infraestrutucture.Security.Tokens.Access;
using TechLibrary.Communication.Reponses;
using TechLibrary.Communication.Requests;
using TechLibrary.Exception;

namespace TechLibrary.Api.UseCases.Users.Register;

public class RegisterUserUseCase
{
    public ResponseRegisteredUserJson Execute(RequestUserJson request)
    {
        var dbContext = new TechLibraryDbContext();
        Validate(request, dbContext);

        var cryptography = new BCryptAlgorithm();
        var entity = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = cryptography.HashPassword(request.Password)
        };

        dbContext.Users.Add(entity);
        dbContext.SaveChanges();

        var tokenGenerator = new JwtTokenGenerator();

        return new ResponseRegisteredUserJson
        {
            Name = entity.Name,
            AccessToken = tokenGenerator.Generate(entity)
        };
    }

    private void Validate(RequestUserJson request, TechLibraryDbContext dbContext)
    {
        var validator = new RegisterUserValidator();

        var result = validator.Validate(request);

        var existsUserWithEmail = dbContext.Users.Any(user => user.Email.Equals(request.Email));

        if (existsUserWithEmail)
            result.Errors.Add(new ValidationFailure("E-mail", "E-mail já registrado na plataforma!"));

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
