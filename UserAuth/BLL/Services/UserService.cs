using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using UserAuth.BLL.Models;
using UserAuth.DAL;
using UserAuth.DAL.Interfaces;
using UserAuth.Models;

namespace UserAuth.BLL.Services;

public class UserService(UnitOfWork unitOfWork, IUserRepository userRepository)
{
    public async Task<UserUnit[]> Insert(UserUnit[] userUnits, CancellationToken token)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(token);
        
        try
        {
            UserDal[] userDals = userUnits.Select(u => new UserDal
            {
                Id=u.Id,
                Username=u.Username,
                Password=u.Password,
            }).ToArray();
            
            var users = await userRepository.Insert(userDals, token);
            await transaction.CommitAsync(token);
            return Map(users);
        }
        catch (Exception e) 
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    public async Task<UserUnit[]> GetUser(QueryUserModel model, CancellationToken token)
    {
        var users = await userRepository.Query(new QueryUserDalModel
        {
            Username = model.Username,
            Password = model.Password
        }, token);

        if (users.Length == 0)
        {
            return [];
        }

        return Map(users);
    }
    
    private UserUnit[] Map(UserDal[] users)
    {
        return users.Select(x => new UserUnit
        {
            Id = x.Id,
            Username = x.Username,
            Password = x.Password
        }).ToArray();
    }
}