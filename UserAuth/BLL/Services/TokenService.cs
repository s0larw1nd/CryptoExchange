using UserAuth.DAL;

namespace UserAuth.BLL.Services;

public class TokenService(UnitOfWork unitOfWork)
{
    public async Task<string> CreateTokenAsync(string userId, int expiresIn=1)
    { 
        var db = await unitOfWork.GetConnectionRedis();
        
        var token = Guid.NewGuid().ToString();
        
        TimeSpan expiry = TimeSpan.FromHours(expiresIn);
        
        string key = $"auth:token:{token}";

        await db.StringSetAsync(key, userId, expiry);

        return token;
    }
    
    public async Task<string?> ValidateTokenAsync(string token)
    {
        var db = await unitOfWork.GetConnectionRedis();
        
        string key = $"auth:token:{token}";
        var userId = await db.StringGetAsync(key);

        return userId.HasValue ? userId.ToString() : null;
    }
}