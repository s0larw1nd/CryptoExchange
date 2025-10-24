using Microsoft.AspNetCore.Mvc;
using UserAuth.BLL.Models;
using UserAuth.BLL.Services;
using UserAuth.DTO.V1.Requests;
using UserAuth.DTO.V1.Responses;

namespace UserAuth.Controllers;

[Route("api/auth")]
public class UserController(UserService userService): ControllerBase
{
    [HttpPost("insert")]
    public async Task<ActionResult<InsertUserResponse>> V1BatchCreate([FromBody] InsertUserRequest request, CancellationToken token)
    {
        var user = await userService.Insert(new UserUnit
        {
            Username = request.Username,
            Password = request.Password,
        }, token);

        return Ok(new InsertUserResponse
        {
            User = user[0]
        });
    }
    
    [HttpPost("query")]
    public async Task<ActionResult<QueryUserResponse>> QueryUsers([FromBody] QueryUserRequest request,
        CancellationToken token)
    {
        var user = await userService.GetUser(new QueryUserModel
        {
            Username = request.Username,
            Password = request.Password
        }, token);
        
        return Ok(new QueryUserResponse
        {
            User = user[0]
        });
    }
}