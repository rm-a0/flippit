using Flippit.Common.Models.Card;

namespace Flippit.Web.DAL.Repositories
{
    public class CardRepository : RepositoryBase<CardDetailModel>
    {
        public override string TableName { get; } = "cards";

        public CardRepository(LocalDb localDb)
            : base(localDb)
        {
        }
    }
}
