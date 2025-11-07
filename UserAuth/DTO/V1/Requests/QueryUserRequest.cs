namespace UserAuth.DTO.V1.Requests;

public class AuthUserRequest
{
    public string Username { get; set; }
    public string? Password { get; set; }
}