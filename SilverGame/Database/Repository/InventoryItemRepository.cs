using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Items;

namespace SilverGame.Database.Repository
{
    static class InventoryItemRepository
    {
        public static void Create(InventoryItem inventoryItem)
        {
            lock (GameDbManager.Lock)
            {
                const string query = "INSERT INTO inventory_items SET id=@id, characterId=@characterId, itemId=@itemId, position=@position," +
                                     "stats=@stats, quantity=@quantity";

                using (var command = new MySqlCommand(query, GameDbManager.Connection))
                {
                    command.Parameters.Add(new MySqlParameter("@id", inventoryItem.Id));
                    command.Parameters.Add(new MySqlParameter("@characterId", inventoryItem.Character.Id));
                    command.Parameters.Add(new MySqlParameter("@itemId", inventoryItem.ItemInfos.Id));
                    command.Parameters.Add(new MySqlParameter("@position", (int) inventoryItem.ItemPosition));
                    command.Parameters.Add(new MySqlParameter("@stats", string.Join(",", inventoryItem.Stats)));
                    command.Parameters.Add(new MySqlParameter("@quantity", inventoryItem.Quantity));

                    command.ExecuteNonQuery();
                }
            }

            lock (DatabaseProvider.InventoryItems)
                DatabaseProvider.InventoryItems.Add(inventoryItem);
        }

        public static void Update(InventoryItem inventoryItem)
        {
            const string query = "UPDATE inventory_items SET id=@id, characterId=@characterId, itemId=@itemId, position=@position," +
                                     "stats=@stats, quantity=@quantity WHERE id=@id";

            using (var command = new MySqlCommand(query, GameDbManager.Connection))
            {
                command.Parameters.Add(new MySqlParameter("@id", inventoryItem.Id));
                command.Parameters.Add(new MySqlParameter("@characterId", inventoryItem.Character.Id));
                command.Parameters.Add(new MySqlParameter("@itemId", inventoryItem.ItemInfos.Id));
                command.Parameters.Add(new MySqlParameter("@position", (int)inventoryItem.ItemPosition));
                command.Parameters.Add(new MySqlParameter("@stats", string.Join(",",inventoryItem.Stats)));
                command.Parameters.Add(new MySqlParameter("@quantity", inventoryItem.Quantity));

                command.ExecuteNonQuery();
            }
        }

        public static void Remove(InventoryItem inventoryItem)
        {
            
        }
    }
}
