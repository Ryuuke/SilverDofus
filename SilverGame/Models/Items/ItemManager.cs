using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverGame.Models.Items
{
    class ItemManager
    {
        //public static int GetItemPosition(Type itemType)
        //{
        //    if (itemType == Type.Arc ||
        //        itemType == Type.Baguette ||
        //        itemType == Type.Baton ||
        //        itemType == Type.Dagues ||
        //        itemType == Type.Epee ||
        //        itemType == Type.Marteau ||
        //        itemType == Type.Pelle ||
        //        itemType == Type.Hache ||
        //        itemType == Type.Outil ||
        //        itemType == Type.Pioche ||
        //        itemType == Type.Faux)
        //    {
                
        //    }
        //}

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
