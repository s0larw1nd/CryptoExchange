using UserAuth.Models;

namespace UserAuth.DAL.Interfaces;

public interface IUserRepository
{
    Task<UserDal[]> Insert(UserDal model, CancellationToken token);
    
    Task<UserDal[]> Query(QueryUserDalModel model, CancellationToken token);
}