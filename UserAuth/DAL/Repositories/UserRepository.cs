using Dapper;
using UserAuth.DAL.Interfaces;
using UserAuth.Models;

namespace UserAuth.DAL.Repositories;

public class UserRepository(UnitOfWork unitOfWork) : IUserRepository
{
    public async Task<UserDal[]> Insert(UserDal[] model, CancellationToken token)
    {
        var sql = @"
           INSERT INTO users
           (
                username,
                password
           )
           
           SELECT
                username,
                password
           FROM unnest(@User)
           
           RETURNING
                id,
                username,
                password
        ";
        
        var conn = await unitOfWork.GetConnectionPostgreSql(token);
        var res = await conn.QueryAsync<UserDal>(new CommandDefinition(
            sql, new {User = model}, cancellationToken: token));
        
        return res.ToArray();
    }

    public async Task<UserDal[]> Query(QueryUserDalModel model, CancellationToken token)
    {
        var sql = @"
            SELECT 
                id,
                username, 
                password
            FROM users
            WHERE username = @Username
        ";

        if (model.Password != null)
        {
            sql += " AND password = @Password";
        }
        
        var conn = await unitOfWork.GetConnectionPostgreSql(token);
        var res = await conn.QueryAsync<UserDal>(
            new CommandDefinition(
                sql,
                new { model.Username, model.Password },
                cancellationToken: token
            ));
        
        return res.ToArray();
    }
}