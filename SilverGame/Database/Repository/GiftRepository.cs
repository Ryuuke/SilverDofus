using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Gifts;

namespace SilverGame.Database.Repository
{
    static class GiftRepository
    {
        public static void Create(Gift gift)
        {

        }

        public static void Update(Gift gift)
        {

        }

        public static void Remove(Gift gift)
        {
            const string query = "DELETE FROM inventory_items WHERE gift=@giftId";

            Base.Repository.ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                (command) => command.Parameters.Add(new MySqlParameter("@id", gift.Id)));

            lock (DatabaseProvider.Gifts)
                DatabaseProvider.Gifts.Remove(gift);
        }

        public static void RemoveFromAccount(int giftId, int accountId)
        {
            const string query = "DELETE FROM account_gifts WHERE giftId=@giftId AND accountId=@accountId";

            Base.Repository.ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                (command) =>
                {
                    command.Parameters.Add(new MySqlParameter("@giftId", giftId));
                    command.Parameters.Add(new MySqlParameter("@accountId", accountId));
                });

            lock (DatabaseProvider.AccountGifts)
                DatabaseProvider.AccountGifts.Remove(
                    DatabaseProvider.AccountGifts.Find(x => x.Key.Id == accountId && x.Value.Id == giftId));
        }
    }
}