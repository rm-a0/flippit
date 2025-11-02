using System;

namespace Flippit.Common
{
    public interface IWithId
    {
        Guid Id { get; init; }
    }
}
