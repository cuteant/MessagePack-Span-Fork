using System.Runtime.CompilerServices;
using CuteAnt;
using Hyperion;

namespace MessagePack
{
    public static class HyperionFormatterResolverExtensions
    {
        [MethodImpl(InlineMethod.Value)]
        public static Serializer GetHyperionSerializer(this IFormatterResolver formatterResolver)
        {
            var serializer = formatterResolver.GetContextValue<Serializer>(HyperionConstants.HyperionSerializerIdentifier);
            if (null == serializer)
            {
                serializer = formatterResolver.GetContextValue<Serializer>(HyperionConstants.HyperionSerializer);
            }
            return serializer;
        }
    }
}
