using System;
using System.Collections.Generic;
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

        public static IEnumerable<Gift> FindAll(int accountId)
        {
            var giftItems = new List<GiftItems>();

            const string query =
                "SELECT gi.giftId, gi.itemId, gi.quantity FROM gift_items gi, account_gifts ag WHERE ag.giftId = gi.giftId AND ag.accountId = @accountId";
            const string query2 =
                "SELECT g.id, g.description, g.pictureUrl, g.title FROM gift g, account_gifts a WHERE a.giftId = g.id AND a.accountId = @accountId;";

            using (var connection = GameDbManager.GetDatabaseConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add(new MySqlParameter("@accountId", accountId));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            giftItems.Add(new GiftItems
                            {
                                GiftId = reader.GetInt16("giftId"),
                                Item = DatabaseProvider.ItemsInfos.Find(x => x.Id == reader.GetInt16("itemId")),
                                Quantity = reader.GetInt16("quantity")
                            });
                        }
                    }
                }

                using (var command = new MySqlCommand(query2, connection))
                {
                    command.Parameters.Add(new MySqlParameter("@accountId", accountId));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new Gift
                            {
                                Id = reader.GetInt16("id"),
                                Description = reader.GetString("description"),
                                Items = giftItems,
                                PictureUrl = reader.GetString("pictureUrl"),
                                Title = reader.GetString("title")
                            };
                        }
                    }
                }
            }
        }

        public static void Update(Gift gift)
        {

        }

        public static void Remove(Gift gift)
        {
            const string query = "DELETE FROM inventory_items WHERE gift=@giftId";

            Base.Repository.ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                (command) => command.Parameters.Add(new MySqlParameter("@id", gift.Id)));
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
        }
    }
}