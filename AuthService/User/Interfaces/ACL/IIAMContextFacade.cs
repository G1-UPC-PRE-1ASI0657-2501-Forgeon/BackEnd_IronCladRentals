namespace AuthService.User.Interfaces.ACL;

public interface IIamContextFacade
{
    Task<Guid> CreateAuthUser(string email, string password,string name,string dni,string phone,DateTime datecreatedat,bool role);
    Task<Guid> FetchAuthUserIdByEmail(string email);
    Task<string> FetchAuthUsernameByUserId(Guid userId);
}