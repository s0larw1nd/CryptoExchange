namespace UserAuth.DTO.V1.Requests;

public class InsertUserRequest
{
    public string Username { get; set; }
    public string? Password { get; set; }
}