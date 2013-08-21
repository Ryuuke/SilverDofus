using System.Collections.Generic;
using System.Linq;
using SilverGame.Database;
using SilverGame.Models.Characters;

namespace SilverGame.Models.Items
{
    class StatsManager
    {
        public int Id { get; set; }
        public GeneralStats Initiative = new GeneralStats();
        public GeneralStats Vitality = new GeneralStats();
        public GeneralStats Wisdom = new GeneralStats();
        public GeneralStats Strength = new GeneralStats();
        public GeneralStats Intelligence = new GeneralStats();
        public GeneralStats Chance = new GeneralStats();
        public GeneralStats Agility = new GeneralStats();
        public GeneralStats PdvNow = new GeneralStats();
        public GeneralStats Pa = new GeneralStats();
        public GeneralStats Pm = new GeneralStats();
        public GeneralStats Po = new GeneralStats();
        public GeneralStats Prospection = new GeneralStats();
        public GeneralStats Weight = new GeneralStats();
        public GeneralStats MaxInvoc = new GeneralStats();
        public GeneralStats ReducDamageAir = new GeneralStats();
        public GeneralStats ReduceDamageChance = new GeneralStats();
        public GeneralStats ReduceDamageIntelligence = new GeneralStats();
        public GeneralStats ReduceDamageStrength = new GeneralStats();
        public GeneralStats ReduceDamageNeutral = new GeneralStats();
        public GeneralStats ReducDamagePvpAir = new GeneralStats();
        public GeneralStats ReduceDamagePvpChance = new GeneralStats();
        public GeneralStats ReduceDamagePvpIntelligence = new GeneralStats();
        public GeneralStats ReduceDamagePvpStrength = new GeneralStats();
        public GeneralStats ReduceDamagePvpNeutral = new GeneralStats();
        public GeneralStats ReduceDamagePercentAir = new GeneralStats();
        public GeneralStats ReduceDamagePercentChance = new GeneralStats();
        public GeneralStats ReduceDamagePercentIntelligence = new GeneralStats();
        public GeneralStats ReduceDamagePercentStrenght = new GeneralStats();
        public GeneralStats ReduceDamagePercentNeutral = new GeneralStats();
        public GeneralStats ReduceDamagePercentPvPAir = new GeneralStats();
        public GeneralStats ReduceDamagePercentPvPChance = new GeneralStats();
        public GeneralStats ReduceDamagePercentPvPIntelligence = new GeneralStats();
        public GeneralStats ReduceDamagePercentPvPStrenght = new GeneralStats();
        public GeneralStats ReduceDamagePercentPvPNeutral = new GeneralStats();
        public GeneralStats ReducPhysicalDamage = new GeneralStats();
        public GeneralStats ReducMagicDamage = new GeneralStats();
        public GeneralStats DodgePa = new GeneralStats();
        public GeneralStats DodgePm = new GeneralStats();
        public GeneralStats ReturnDamage = new GeneralStats();
        public GeneralStats CriticalDamage = new GeneralStats();
        public GeneralStats FailDamage = new GeneralStats();
        public GeneralStats Damage = new GeneralStats();
        public GeneralStats PercentDamage = new GeneralStats();
        public GeneralStats PhysicalDamage = new GeneralStats();
        public GeneralStats MagicDamage = new GeneralStats();
        public GeneralStats TrapDamage = new GeneralStats();
        public GeneralStats TrapPercentDamage = new GeneralStats();
        public GeneralStats Heal = new GeneralStats();

        public void Calculate(Character character)
        {
            var items = DatabaseProvider.InventoryItems.FindAll(x => x.Character == character);

            foreach (var stats in from inventoryItem in items
                where
                    (int) inventoryItem.ItemPosition > (int) Position.None &&
                    (int) inventoryItem.ItemPosition < (int) Position.Bar1
                from stats in inventoryItem.Stats
                select stats)
            {
                ParseStats(stats);
            }
        }

        private void ParseStats(ItemStats stats)
        {
            if (WeaponEffect.Contains(stats.Header))
                return;

             // Checking stats Header (Element)
            switch (stats.Header)
            {
                    // Positive basic effects

                case Effect.AddAgilite :
                    Agility.Items += stats.MinValue;
                    break;
                case Effect.AddChance:
                    Chance.Items += stats.MinValue;
                    break;
                case Effect.AddForce:
                    Strength.Items += stats.MinValue;
                    break;
                case Effect.AddIntelligence:
                    Intelligence.Items += stats.MinValue;
                    break;
                case Effect.AddVitalite:
                    Vitality.Items += stats.MinValue;
                    break;
                case Effect.AddSagesse:
                    Wisdom.Items += stats.MinValue;
                    break;
                case Effect.AddLife:
                    PdvNow.Items += stats.MinValue;
                    break;
                case Effect.AddPa:
                    Pa.Items += stats.MinValue;
                    break;
                case Effect.AddPm:
                    Pm.Items += stats.MinValue;
                    break;
                case Effect.AddPo:
                    Po.Items += stats.MinValue;
                    break;
                case Effect.AddInvocationMax:
                    MaxInvoc.Items += stats.MinValue;
                    break;
                case Effect.AddInitiative:
                    Initiative.Items += stats.MinValue;
                    break;
                case Effect.AddProspection:
                    Prospection.Items += stats.MinValue;
                    break;
                case Effect.AddPods:
                    Weight.Items += stats.MinValue;
                    break;

                // Negative basic effects

                case Effect.SubAgilite:
                    Agility.Items -= stats.MinValue;
                    break;
                case Effect.SubChance:
                    Chance.Items -= stats.MinValue;
                    break;
                case Effect.SubForce:
                    Strength.Items -= stats.MinValue;
                    break;
                case Effect.SubIntelligence:
                    Intelligence.Items -= stats.MinValue;
                    break;
                case Effect.SubVitalite:
                    Vitality.Items -= stats.MinValue;
                    break;
                case Effect.SubSagesse:
                    Wisdom.Items -= stats.MinValue;
                    break;
                case Effect.SubPa:
                    Pa.Items -= stats.MinValue;
                    break;
                case Effect.SubPm:
                    Pm.Items -= stats.MinValue;
                    break;
                case Effect.SubPo:
                    Po.Items -= stats.MinValue;
                    break;
                case Effect.SubInitiative:
                    Initiative.Items -= stats.MinValue;
                    break;
                case Effect.SubProspection:
                    Prospection.Items -= stats.MinValue;
                    break;
                case Effect.SubPods:
                    Weight.Items -= stats.MinValue;
                    break;

                // Positive Reduc damage effects

                case Effect.AddReduceDamageAir:
                    ReducDamageAir.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamageEau:
                    ReduceDamageChance.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamageFeu:
                    ReduceDamageIntelligence.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamageTerre:
                    ReduceDamageStrength.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamageNeutre:
                    ReduceDamageNeutral.Items += stats.MinValue;
                    break;

                // Positive Reduc damage Pvp effects

                case Effect.AddReduceDamagePvPAir:
                    ReducDamagePvpAir.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePvPEau:
                    ReduceDamagePvpChance.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePvPFeu:
                    ReduceDamagePvpIntelligence.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePvPTerre:
                    ReduceDamagePvpStrength.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePvPNeutre:
                    ReduceDamagePvpNeutral.Items += stats.MinValue;
                    break;

                // Positive Reduc percent damage effects

                case Effect.AddReduceDamagePourcentAir:
                    ReduceDamagePercentAir.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePourcentEau:
                    ReduceDamagePercentChance.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePourcentFeu:
                    ReduceDamagePercentIntelligence.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePourcentTerre:
                    ReduceDamagePercentStrenght.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePourcentNeutre:
                    ReduceDamagePercentNeutral.Items += stats.MinValue;
                    break;

                // Positive Reduc percent damage Pvp effects

                case Effect.AddReduceDamagePourcentPvPAir:
                    ReduceDamagePercentPvPAir.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePourcentPvPEau:
                    ReduceDamagePercentPvPChance.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePourcentPvPFeu:
                    ReduceDamagePercentPvPIntelligence.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePourcentPvPTerre:
                    ReduceDamagePercentPvPStrenght.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamagePourcentPvpNeutre:
                    ReduceDamagePercentPvPNeutral.Items += stats.MinValue;
                    break;

                // Positive Reduc damages Type

                case Effect.AddReduceDamagePhysic:
                    ReducPhysicalDamage.Items += stats.MinValue;
                    break;
                case Effect.AddReduceDamageMagic:
                    ReducMagicDamage.Items += stats.MinValue;
                    break;

                // Positive Dodge Pa Pm

                case Effect.AddEsquivePa:
                    DodgePa.Items += stats.MinValue;
                    break;
                case Effect.AddEsquivePm:
                    DodgePm.Items += stats.MinValue;
                    break;

                // Negative Dodge Pa Pm

                case Effect.SubEsquivePa:
                    DodgePa.Items -= stats.MinValue;
                    break;
                case Effect.SubEsquivePm:
                    DodgePm.Items -= stats.MinValue;
                    break;

                // Negative Reduc damage effects

                case Effect.SubReduceDamageAir:
                    ReducDamageAir.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamageEau:
                    ReduceDamageChance.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamageFeu:
                    ReduceDamageIntelligence.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamageTerre:
                    ReduceDamageStrength.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamageNeutre:
                    ReduceDamageNeutral.Items -= stats.MinValue;
                    break;

                // Negative Reduc percent damage effects

                case Effect.SubReduceDamagePourcentAir:
                    ReduceDamagePercentAir.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamagePourcentEau:
                    ReduceDamagePercentChance.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamagePourcentFeu:
                    ReduceDamagePercentIntelligence.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamagePourcentTerre:
                    ReduceDamagePercentStrenght.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamagePourcentNeutre:
                    ReduceDamagePercentNeutral.Items -= stats.MinValue;
                    break;

                // Negative Reduc percent damage Pvp effects

                case Effect.SubReduceDamagePourcentPvPAir:
                    ReduceDamagePercentPvPAir.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamagePourcentPvPEau:
                    ReduceDamagePercentPvPChance.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamagePourcentPvPFeu:
                    ReduceDamagePercentPvPIntelligence.Items += stats.MinValue;
                    break;
                case Effect.SubReduceDamagePourcentPvPTerre:
                    ReduceDamagePercentPvPStrenght.Items -= stats.MinValue;
                    break;
                case Effect.SubReduceDamagePourcentPvpNeutre:
                    ReduceDamagePercentPvPNeutral.Items -= stats.MinValue;
                    break;

                // Positive different types of damage

                case Effect.AddRenvoiDamage:
                    ReturnDamage.Items += stats.MinValue;
                    break;
                case Effect.AddDamageCritic:
                    CriticalDamage.Items += stats.MinValue;
                    break;
                case Effect.AddEchecCritic:
                    FailDamage.Items += stats.MinValue;
                    break;
                case Effect.AddDamage:
                    Damage.Items += stats.MinValue;
                    break;
                case Effect.AddDamagePercent:
                    PercentDamage.Items += stats.MinValue;
                    break;
                case Effect.AddDamagePhysic:
                    PhysicalDamage.Items += stats.MinValue;
                    break;
                case Effect.AddDamageMagic:
                    MagicDamage.Items += stats.MinValue;
                    break;
                case Effect.AddDamagePiege:
                    TrapDamage.Items += stats.MinValue;
                    break;
                case Effect.AddDamagePiegePercent:
                    TrapPercentDamage.Items += stats.MinValue;
                    break;
                case Effect.AddSoins:
                    Heal.Items += stats.MinValue;
                    break;

                // Negative different types of damage

                case Effect.SubDamageCritic:
                    CriticalDamage.Items -= stats.MinValue;
                    break;
                case Effect.SubDamage:
                    Damage.Items -= stats.MinValue;
                    break;
                case Effect.SubDamagePhysic:
                    PhysicalDamage.Items -= stats.MinValue;
                    break;
                case Effect.SubDamageMagic:
                    MagicDamage.Items -= stats.MinValue;
                    break;
                case Effect.SubSoins:
                    Heal.Items -= stats.MinValue;
                    break;
            }
        }

        public void AddItemStats(IEnumerable<ItemStats> stats)
        {
            foreach (var itemStatse in stats)
            {
                ParseStats(itemStatse);
            }
        }

        public void RemoveItemStats(IEnumerable<ItemStats> stats)
        {
            var invertedStats = stats.Select(stat => new ItemStats
            {
                Header = stat.Header,
                MinValue = -stat.MinValue,
                MaxValue = stat.MaxValue,
                JetDecimal = stat.JetDecimal
            }).ToList();

            foreach (var invertedStat in invertedStats)
            {
                ParseStats(invertedStat);
            }
        }


        public Effect[] WeaponEffect =
        {
            Effect.VolEau,
            Effect.VolTerre,
            Effect.VolAir,
            Effect.VolFeu,
            Effect.VolNeutre,
            Effect.DamageEau,
            Effect.DamageTerre,
            Effect.DamageAir,
            Effect.DamageFeu,
            Effect.DamageNeutre
        };

        public enum Effect
        {
            None = -1,

            Teleport = 4,
            PushBack = 5,
            PushFront = 6,
            Transpose = 8,

            VolPm = 77,
            VolVie = 82,
            VolPa = 84,

            DamageLifeEau = 85,
            DamageLifeTerre = 86,
            DamageLifeAir = 87,
            DamageLifeFeu = 88,
            DamageLifeNeutre = 89,
            VolEau = 91,
            VolTerre = 92,
            VolAir = 93,
            VolFeu = 94,
            VolNeutre = 95,
            DamageEau = 96,
            DamageTerre = 97,
            DamageAir = 98,
            DamageFeu = 99,
            DamageNeutre = 100,
            AddArmor = 105,
            AddArmorBis = 265,

            AddRenvoiDamage = 107,
            Heal = 108,
            DamageLanceur = 109,
            AddLife = 110,
            AddPa = 111,
            AddDamage = 112,
            MultiplyDamage = 114,

            AddPaBis = 120,
            AddAgilite = 119,
            AddChance = 123,
            AddDamagePercent = 138,
            AddDamageCritic = 115,
            AddDamagePiege = 225,
            AddDamagePiegePercent = 220,
            AddDamagePhysic = 142,
            AddDamageMagic = 143,
            AddEchecCritic = 122,
            AddEsquivePa = 160,
            AddEsquivePm = 161,
            AddForce = 118,
            AddInitiative = 174,
            AddIntelligence = 126,
            AddInvocationMax = 182,
            AddPm = 128,
            AddPo = 117,
            AddPods = 158,
            AddProspection = 176,
            AddSagesse = 124,
            AddSoins = 178,
            AddVitalite = 125,
            SubAgilite = 154,

            SubChance = 152,
            SubDamage = 164,
            SubDamageCritic = 171,
            SubDamageMagic = 172,
            SubDamagePhysic = 173,
            SubEsquivePa = 162,
            SubEsquivePm = 163,
            SubForce = 157,
            SubInitiative = 175,
            SubIntelligence = 155,
            SubPaEsquive = 101,
            SubPmEsquive = 127,
            SubPa = 168,
            SubPm = 169,
            SubPo = 116,
            SubPods = 159,
            SubProspection = 177,
            SubSagesse = 156,
            SubSoins = 179,
            SubVitalite = 153,

            Invocation = 181,

            AddReduceDamagePhysic = 183,
            AddReduceDamageMagic = 184,

            AddReduceDamagePourcentEau = 211,
            AddReduceDamagePourcentTerre = 210,
            AddReduceDamagePourcentAir = 212,
            AddReduceDamagePourcentFeu = 213,
            AddReduceDamagePourcentNeutre = 214,
            AddReduceDamagePourcentPvPEau = 251,
            AddReduceDamagePourcentPvPTerre = 250,
            AddReduceDamagePourcentPvPAir = 252,
            AddReduceDamagePourcentPvPFeu = 253,
            AddReduceDamagePourcentPvpNeutre = 254,

            AddReduceDamageEau = 241,
            AddReduceDamageTerre = 240,
            AddReduceDamageAir = 242,
            AddReduceDamageFeu = 243,
            AddReduceDamageNeutre = 244,
            AddReduceDamagePvPEau = 261,
            AddReduceDamagePvPTerre = 260,
            AddReduceDamagePvPAir = 262,
            AddReduceDamagePvPFeu = 263,
            AddReduceDamagePvPNeutre = 264,

            SubReduceDamagePourcentEau = 216,
            SubReduceDamagePourcentTerre = 215,
            SubReduceDamagePourcentAir = 217,
            SubReduceDamagePourcentFeu = 218,
            SubReduceDamagePourcentNeutre = 219,
            SubReduceDamagePourcentPvPEau = 255,
            SubReduceDamagePourcentPvPTerre = 256,
            SubReduceDamagePourcentPvPAir = 257,
            SubReduceDamagePourcentPvPFeu = 258,
            SubReduceDamagePourcentPvpNeutre = 259,
            SubReduceDamageEau = 246,
            SubReduceDamageTerre = 245,
            SubReduceDamageAir = 247,
            SubReduceDamageFeu = 248,
            SubReduceDamageNeutre = 249,

            Porter = 50,
            Lancer = 51,
            ChangeSkin = 149,
            SpellBoost = 293,
            UseTrap = 400,
            UseGlyph = 401,
            DoNothing = 666,
            DamageLife = 672,
            PushFear = 783,
            AddChatiment = 788,
            AddState = 950,
            LostState = 951,
            Invisible = 150,
            DeleteAllBonus = 132,

            AddSpell = 604,
            AddCharactForce = 607,
            AddCharactSagesse = 678,
            AddCharactChance = 608,
            AddCharactAgilite = 609,
            AddCharactVitalite = 610,
            AddCharactIntelligence = 611,
            AddCharactPoint = 612,
            AddSpellPoint = 613,

            LastEat = 808,

            MountOwner = 995,

            LivingGfxId = 970,
            LivingMood = 971,
            LivingSkin = 972,
            LivingType = 973,
            LivingXp = 974,

            CanBeExchange = 983,
        }

        public enum Position
        {
            None = -1,
            Amulette = 0,
            Arme = 1,
            Anneau1 = 2,
            Ceinture = 3,
            Anneau2 = 4,
            Bottes = 5,
            Coiffe = 6,
            Cape = 7,
            Familier = 8,
            Dofus1 = 9,
            Dofus2 = 10,
            Dofus3 = 11,
            Dofus4 = 12,
            Dofus5 = 13,
            Dofus6 = 14,
            Bouclier = 15,
            Mount = 16,

            Bar1 = 23,
            Bar2 = 24,
            Bar3 = 25,
            Bar4 = 26,
            Bar5 = 27,
            Bar6 = 28,
            Bar7 = 29,
            Bar8 = 30,
            Bar9 = 31,
            Bar10 = 32,
            Bar11 = 33,
            Bar12 = 34,
            Bar13 = 35,
            Bar14 = 36,
        }

        public enum ItemType
        {
            Amulette = 1,
            Arc = 2,
            Baguette = 3,
            Baton = 4,
            Dagues = 5,
            Epee = 6,
            Marteau = 7,
            Pelle = 8,
            Anneau = 9,
            Ceinture = 10,
            Bottes = 11,
            Potion = 12,
            ParchoExp = 13,
            Dons = 14,
            Ressource = 15,
            Coiffe = 16,
            Cape = 17,
            Familier = 18,
            Hache = 19,
            Outil = 20,
            Pioche = 21,
            Faux = 22,
            Dofus = 23,
            Quetes = 24,
            Document = 25,
            FmPotion = 26,
            Transform = 27,
            BoostFood = 28,
            Benediction = 29,
            Malediction = 30,
            RpBuff = 31,
            PersoSuiveur = 32,
            Pain = 33,
            Cereale = 34,
            Fleur = 35,
            Plante = 36,
            Biere = 37,
            Bois = 38,
            Minerais = 39,
            Alliage = 40,
            Poisson = 41,
            Bonbon = 42,
            PotionOublie = 43,
            PotionMetier = 44,
            PotionSort = 45,
            Fruit = 46,
            Os = 47,
            Poudre = 48,
            ComestiPoisson = 49,
            PierrePrecieuse = 50,
            PierreBrute = 51,
            Farine = 52,
            Plume = 53,
            Poil = 54,
            Etoffe = 55,
            Cuir = 56,
            Laine = 57,
            Graine = 58,
            Peau = 59,
            Huile = 60,
            Peluche = 61,
            PoissonVide = 62,
            Viande = 63,
            ViandeConservee = 64,
            Queue = 65,
            Metaria = 66,
            Legume = 68,
            ViandeComestible = 69,
            Teinture = 70,
            EquipAlchimie = 71,
            OeufFamilier = 72,
            Maitrise = 73,
            FeeArtifice = 74,
            ParcheminSort = 75,
            ParcheminCarac = 76,
            CertificatChanil = 77,
            RuneForgemagie = 78,
            Boisson = 79,
            ObjetMission = 80,
            SacDos = 81,
            Bouclier = 82,
            PierreAme = 83,
            Clefs = 84,
            PierreAmePleine = 85,
            PopoOubliPercep = 86,
            ParchoRecherche = 87,
            PierreMagique = 88,
            Cadeaux = 89,
            FantomeFamilier = 90,
            Dragodinde = 91,
            BOUFTOU = 92,
            ObjetElevage = 93,
            ObjetUtilisable = 94,
            Planche = 95,
            Ecorce = 96,
            CertifMonture = 97,
            Racine = 98,
            FiletCapture = 99,
            SacRessource = 100,
            Arbalete = 102,
            Patte = 103,
            Aile = 104,
            Oeuf = 105,
            Oreille = 106,
            Carapace = 107,
            Bourgeon = 108,
            Oeil = 109,
            Gelee = 110,
            Coquille = 111,
            Prisme = 112,
            ObjetVivant = 113,
            ArmeMagique = 114,
            FragmAmeShushu = 115,
            PotionFamilier = 116,
        }
    }
}
