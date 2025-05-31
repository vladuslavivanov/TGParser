using TGParser.API.Controllers.Messages.Interfaces;

namespace TGParser.API.Controllers.Messages.ChatShared.Interfaces;

public interface IUsersSharedMessage : IMessage 
{
    int RequestId { get; }
}
