namespace TGParser.Core.DTO;

public record AddProxyDto(long UserId, string IP, int Port, string UserName, string Password);