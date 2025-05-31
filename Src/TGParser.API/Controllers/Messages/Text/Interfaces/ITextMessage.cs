using TGParser.API.Controllers.Messages.Interfaces;

namespace TGParser.API.Controllers.Messages.ChatShared.Interfaces;

public interface ITextMessage : IMessage 
{
    string Name { get; }
}
