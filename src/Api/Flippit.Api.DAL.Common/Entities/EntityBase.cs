using System;
using Flippit.Api.DAL.Common.Entities.Interfaces;

namespace Flippit.Api.DAL.Common.Entities
{
    public abstract record EntityBase : IEntity
    {
        public required Guid Id { get; init; }
    }
}
