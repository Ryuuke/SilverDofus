using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Accounts;
using SilverGame.Models.Characters;
using SilverGame.Services;

namespace SilverGame.Database.Repository
{
     static class CharacterRepository
    {
         public static void Create(Character character, int accountId)
         {
             const string query =
                 "INSERT INTO characters SET id=@id, name=@name, classe=@classe, sex=@sex, color1=@color1, color2=@color2," +
                 "color3=@color3, skin=@skin, level=@level, alignmentId=@alignmentId, statsId=@statsId, " +
                 "pdvNow=@pdvNow, mapId=@mapId, cellId=@cellId, direction=@direction, channels=@channels, statsPoints@statsPoints" +
                 "spellsPoints=@spellsPoints";

             Base.Repository.ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                 (command) =>
                 {
                     command.Parameters.Add(new MySqlParameter("@id", character.Id));
                     command.Parameters.Add(new MySqlParameter("@name", character.Name));
                     command.Parameters.Add(new MySqlParameter("@classe", (int) character.Classe));
                     command.Parameters.Add(new MySqlParameter("@sex", character.Sex));
                     command.Parameters.Add(new MySqlParameter("@color1", character.Color1));
                     command.Parameters.Add(new MySqlParameter("@color2", character.Color2));
                     command.Parameters.Add(new MySqlParameter("@color3", character.Color3));
                     command.Parameters.Add(new MySqlParameter("@skin", character.Skin));
                     command.Parameters.Add(new MySqlParameter("@level", character.Level));
                     command.Parameters.Add(new MySqlParameter("@alignmentId", character.Alignment.Id));
                     command.Parameters.Add(new MySqlParameter("@statsId", character.Stats.Id));
                     command.Parameters.Add(new MySqlParameter("@pdvNow", character.PdvNow));
                     command.Parameters.Add(new MySqlParameter("@mapId", character.Map.Id));
                     command.Parameters.Add(new MySqlParameter("@cellId", character.MapCell));
                     command.Parameters.Add(new MySqlParameter("@direction", character.Direction));
                     command.Parameters.Add(new MySqlParameter("@channels", string.Join("", character.Channels)));
                     command.Parameters.Add(new MySqlParameter("@statsPoints", character.StatsPoints));
                     command.Parameters.Add(new MySqlParameter("@spellsPoints", character.SpellPoints));
                 });

             Logs.LogWritter(Constant.GameFolder, string.Format("Création du personnage {0}", character.Name));

             const string query2 =
                 "INSERT INTO characters SET accountId=@accountId, gameserverId=@gameServer, characterName=@name";

             Base.Repository.ExecuteQuery(query2, RealmDbManager.GetDatabaseConnection(),
                 (command) =>
                 {
                     command.Parameters.Add(new MySqlParameter("@accountId", accountId));
                     command.Parameters.Add(new MySqlParameter("@gameServer", DatabaseProvider.ServerId));
                     command.Parameters.Add(new MySqlParameter("@name", character.Name));
                 });

             lock (DatabaseProvider.Characters)
                 DatabaseProvider.Characters.Add(character);

             lock (DatabaseProvider.AccountCharacters)
                 DatabaseProvider.AccountCharacters.Add(new AccountCharacters
                 {
                     Account = DatabaseProvider.Accounts.Find(x => x.Id == accountId),
                     Character = character
                 });
         }

         public static void Update(Character character)
         {
             StatsRepository.Update(character.Stats);
             AlignmentRepository.Update(character.Alignment);
             const string query =
                 "UPDATE characters SET skin=@skin, level=@level, " +
                 "pdvNow=@pdvNow, exp=@exp, mapId=@mapId, cellId=@cellId, direction=@direction, channels=@channels, " +
                 "statsPoints=@statsPoints, spellsPoints=@spellsPoints WHERE id=@id";

             Base.Repository.ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                 (command) =>
                 {
                     command.Parameters.Add(new MySqlParameter("@id", character.Id));
                     command.Parameters.Add(new MySqlParameter("@skin", character.Skin));
                     command.Parameters.Add(new MySqlParameter("@level", character.Level));
                     command.Parameters.Add(new MySqlParameter("@pdvNow", character.PdvNow));
                     command.Parameters.Add(new MySqlParameter("@exp", character.Exp));
                     command.Parameters.Add(new MySqlParameter("@mapId", character.Map.Id));
                     command.Parameters.Add(new MySqlParameter("@cellId", character.MapCell));
                     command.Parameters.Add(new MySqlParameter("@direction", character.Direction));
                     command.Parameters.Add(new MySqlParameter("@channels",
                         string.Format("{0}", string.Join("", character.Channels))));
                     command.Parameters.Add(new MySqlParameter("@statsPoints", character.StatsPoints));
                     command.Parameters.Add(new MySqlParameter("@spellsPoints", character.SpellPoints));

                 });
         }
    }
}