using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using UserAuth.BLL.Models;
using UserAuth.BLL.Services;
using UserAuth.DTO.V1.Requests;
using UserAuth.DTO.V1.Responses;
using UserAuth.Validators;

namespace UserAuth.Controllers;

[Route("api/auth")]
public class UserController(UserService userService, TokenService tokenService, ValidatorFactory validatorFactory): ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<InsertUserResponse>> V1BatchCreate([FromBody] InsertUserRequest request, CancellationToken token)
    {
        var validationResult = await validatorFactory.GetValidator<InsertUserRequest>().ValidateAsync(request, token);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }
        
        try
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
                
                var user = await userService.Insert([
                    new UserUnit
                    {
                        Username = request.Username,
                        Password = Convert.ToBase64String(bytes)
                    }
                ],token);

                var userToken = await tokenService.CreateTokenAsync(user[0].Id.ToString());
                
                return Ok(new InsertUserResponse
                {
                    Token = userToken
                });
            }
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            return BadRequest("Ошибка: пользователь с таким именем уже существует.");
        }
    }
    
    [HttpPost("auth")]
    public async Task<ActionResult<QueryUserResponse>> QueryUsers([FromBody] AuthUserRequest request,
        CancellationToken token)
    {
        var validationResult = await validatorFactory.GetValidator<AuthUserRequest>().ValidateAsync(request, token);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }
        
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            
            var user = await userService.GetUser(new QueryUserModel
            {
                Username = request.Username,
                Password = Convert.ToBase64String(bytes)
            }, token);

            if (user.Length != 0)
            {
                Console.WriteLine(user[0].Id.ToString());
                var userToken = await tokenService.CreateTokenAsync(user[0].Id.ToString());
                return Ok(new QueryUserResponse
                {
                    Token = userToken
                });
            }
        
            return NotFound("Пользователь с такими данными не найден.");
        }
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ValidateTokenResponse>> ValidateToken([FromBody] ValidateTokenRequest request,
        CancellationToken token)
    {
        var validationResult = await validatorFactory.GetValidator<ValidateTokenRequest>().ValidateAsync(request, token);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }
        
        var userID = await tokenService.ValidateTokenAsync(request.Token);

        if (userID != null)
        {
            return Ok(new ValidateTokenResponse
            {
                UserID = userID
            });
        }

        return NotFound("Токен не найден.");
    }
}