using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Items;

namespace SilverGame.Database.Repository
{
    internal class InventoryItemRepository : Abstract.Repository
    {
        public static void Create(InventoryItem inventoryItem)
        {
            const string query =
                "INSERT INTO inventory_items SET id=@id, characterId=@characterId, itemId=@itemId, position=@position," +
                "stats=@stats, quantity=@quantity";

            ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                (command) =>
                {
                    command.Parameters.Add(new MySqlParameter("@id", inventoryItem.Id));
                    command.Parameters.Add(new MySqlParameter("@characterId", inventoryItem.Character.Id));
                    command.Parameters.Add(new MySqlParameter("@itemId", inventoryItem.ItemInfos.Id));
                    command.Parameters.Add(new MySqlParameter("@position", (int) inventoryItem.ItemPosition));
                    command.Parameters.Add(new MySqlParameter("@stats", string.Join(",", inventoryItem.Stats)));
                    command.Parameters.Add(new MySqlParameter("@quantity", inventoryItem.Quantity));
                });

            lock (DatabaseProvider.InventoryItems)
                DatabaseProvider.InventoryItems.Add(inventoryItem);
        }

        public static void Update(InventoryItem inventoryItem)
        {
            const string query =
                "UPDATE inventory_items SET id=@id, characterId=@characterId, itemId=@itemId, position=@position," +
                "stats=@stats, quantity=@quantity WHERE id=@id";

            ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                (command) =>
                {
                    command.Parameters.Add(new MySqlParameter("@id", inventoryItem.Id));
                    command.Parameters.Add(new MySqlParameter("@characterId", inventoryItem.Character.Id));
                    command.Parameters.Add(new MySqlParameter("@itemId", inventoryItem.ItemInfos.Id));
                    command.Parameters.Add(new MySqlParameter("@position", (int) inventoryItem.ItemPosition));
                    command.Parameters.Add(new MySqlParameter("@stats", string.Join(",", inventoryItem.Stats)));
                    command.Parameters.Add(new MySqlParameter("@quantity", inventoryItem.Quantity));
                });
        }

        public static void Remove(InventoryItem inventoryItem)
        {
            const string query =
                "DELETE FROM Inventory_items WHERE id=@id";

            ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                (command) => command.Parameters.Add(new MySqlParameter("@id", inventoryItem.Id)));

            lock (DatabaseProvider.InventoryItems)
                DatabaseProvider.InventoryItems.Remove(inventoryItem);
        }
    }
}