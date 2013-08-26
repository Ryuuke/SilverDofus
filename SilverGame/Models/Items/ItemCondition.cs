using System.Linq;
using SilverGame.Database;
using SilverGame.Models.Characters;

namespace SilverGame.Models.Items
{
    static class ItemCondition
    {

        public static bool VerifyIfCharacterMeetItemCondition(Character character, string conditionString)
        {
            if (conditionString.Equals(string.Empty))
                return true;

            if (!conditionString.Contains("&") && !conditionString.Contains("|"))
            {
                conditionString = conditionString.Replace("(", "").Replace(")", "");

                return ParseItemConditionWithoutSpliter(character, conditionString);
            }

            if (!conditionString.Contains("&") && conditionString.Contains("|"))
            {
                return ParseItemConditionWithOrSpliter(character, conditionString);
            }

            return ParseItemConditionWithAndSpliter(character, conditionString);
        }

        private static bool ParseItemConditionWithoutSpliter(Character character, string conditionString)
        {
            return Parse(conditionString, character);
        }

        private static bool ParseItemConditionWithAndSpliter(Character character, string conditionString)
        {
            var conditions = conditionString.Split('&');

            return conditions.All(condition => Parse(condition, character) == true);
        }

        private static bool ParseItemConditionWithOrSpliter(Character character, string conditionString)
        {
            conditionString = conditionString.Replace("(", "").Replace(")", "");

            var conditions = conditionString.Split('|');

            return conditions.Any(condition => Parse(condition, character) == true);
        }

        private static bool Parse(string condition, Character character)
        {
            if (condition.Contains("("))
                return ParseItemConditionWithOrSpliter(character, condition);

            var header = condition.Substring(0, 2);

            var balance = condition.Substring(2, 1);

            var value = int.Parse(condition.Substring(3));

            int characterStatsValue;

            bool avaliable;

            switch (header.Substring(0, 1))
            {
                case "C":

                    switch (header.Substring(1, 1))
                    {
                        case "a":
                            characterStatsValue = character.Stats.Agility.Base;
                            break;

                        case "i":
                            characterStatsValue = character.Stats.Intelligence.Base;
                            break;

                        case "c":
                            characterStatsValue = character.Stats.Chance.Base;
                            break;

                        case "s":
                            characterStatsValue = character.Stats.Strength.Base;
                            break;

                        case "v":
                            characterStatsValue = character.Stats.Vitality.Base;
                            break;

                        case "w":
                            characterStatsValue = character.Stats.Wisdom.Base;
                            break;

                        case "A":
                            characterStatsValue = character.Stats.Agility.GetTotal();
                            break;

                        case "I":
                            characterStatsValue = character.Stats.Intelligence.GetTotal();
                            break;

                        case "C":
                            characterStatsValue = character.Stats.Chance.GetTotal();
                            break;

                        case "S":
                            characterStatsValue = character.Stats.Strength.GetTotal();
                            break;

                        case "V":
                            characterStatsValue = character.Stats.Vitality.GetTotal();
                            break;

                        case "W":
                            characterStatsValue = character.Stats.Wisdom.GetTotal();
                            break;

                        default:
                            return true;
                    }

                    break;

                case "P":

                    switch (header.Substring(1, 1))
                    {
                        case "G":
                            characterStatsValue = (int) character.Classe;
                            break;
                        case "L":
                            characterStatsValue = character.Level;
                            break;
                        case "K":
                            characterStatsValue = character.Kamas;
                            break;
                        case "S":
                            characterStatsValue = character.Sex;
                            break;
                        case "X":
                            characterStatsValue = DatabaseProvider.AccountCharacters.Find(x => x.Character == character).Account.GmLevel;
                            break;
                        case "W":
                            characterStatsValue = character.GetMaxWeight();
                            break;
                        case "s":
                            characterStatsValue = character.Alignment.Type;
                            break;
                        case "a":
                            characterStatsValue = character.Alignment.Level;
                            break;
                        case "P":
                            characterStatsValue = character.Alignment.Grade;
                            break;

                        default:
                            return true;
                    }

                    break;

                default:
                    return true;
            }

            if (balance == "") return false;

            switch (balance)
            {

                case "<":
                    avaliable = (characterStatsValue < value ? true : false);
                    break;

                case ">":
                    avaliable = (characterStatsValue > value ? true : false);
                    break;

                case "=":
                case "~":
                    avaliable = (characterStatsValue == value ? true : false);
                    break;

                case "!":
                    avaliable = (characterStatsValue != value ? true : false);
                    break;

                default:
                    return true;
            }

            return avaliable;
        }
    }
}