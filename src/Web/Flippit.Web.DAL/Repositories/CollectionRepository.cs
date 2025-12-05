using Flippit.Common.Models.Collection;

namespace Flippit.Web.DAL.Repositories
{
    public class CollectionRepository : RepositoryBase<CollectionDetailModel>
    {
        public override string TableName { get; } = "collections";

        public CollectionRepository(LocalDb localDb)
            : base(localDb)
        {
        }
    }
}

