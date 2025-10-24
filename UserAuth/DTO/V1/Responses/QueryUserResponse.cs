using UserAuth.BLL.Models;

namespace UserAuth.DTO.V1.Responses;

public class QueryUserResponse
{
    public UserUnit User { get; set; }
}