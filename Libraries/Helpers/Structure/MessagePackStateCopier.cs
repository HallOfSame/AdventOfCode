using Helpers.Interfaces;
using MessagePack;

namespace Helpers.Structure;

public class MessagePackStateCopier : IStateCopier
{
    public TState Copy<TState>(TState state)
    {
        var bin = MessagePackSerializer.Serialize(
                                                  state,
                                                  MessagePack.Resolvers.ContractlessStandardResolver.Options);

        return MessagePackSerializer.Deserialize<TState>(bin,
                                                         MessagePack.Resolvers.ContractlessStandardResolver.Options);
    }
}