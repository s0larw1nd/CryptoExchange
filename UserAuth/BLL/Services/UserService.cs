using UserAuth.BLL.Models;
using UserAuth.DAL;
using UserAuth.DAL.Interfaces;
using UserAuth.Models;

namespace UserAuth.BLL.Services;

public class UserService(UnitOfWork unitOfWork, IUserRepository userRepository)
{
    public async Task<UserUnit[]> Insert(UserUnit userUnit, CancellationToken token)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(token);
        
        try
        {
            UserDal userDal = new UserDal
            {
                Id = userUnit.Id,
                Username = userUnit.Username,
                Password = userUnit.Password
            };
            var prices = await userRepository.Insert(userDal, token);
            
            await transaction.CommitAsync(token);
            return Map(prices);
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

        if (users.Length is 0)
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