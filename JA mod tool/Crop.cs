using System.Collections.Generic;
using System.Drawing;

namespace JA_mod_tool
{
    public class Crop
    {
        public string cropName { get; set; }
        public string seedName { get; set; }
        public string seedDesc { get; set; }
        public string cropProduct { get; set; }
        
        public Image sprite1 { get; set; }
        public Image sprite2 { get; set; }

        public string seedShop { get; set; }
        public string cropType { get; set; }
        public int seedBuyPrice { get; set; }
        public int seedSellPrice { get; set; }

        public string spaloc1{ get; set; }
        public string korloc1{ get; set; }
        public string gerloc1{ get; set; }
        public string freloc1{ get; set; }
        public string hunloc1{ get; set; }
        public string italoc1{ get; set; }
        public string japloc1{ get; set; }
        public string porloc1{ get; set; }
        public string rusloc1{ get; set; }
        public string turloc1{ get; set; }
        public string chiloc1 { get; set; }

        public string spaloc2{ get; set; }
        public string korloc2{ get; set; }
        public string gerloc2{ get; set; }
        public string freloc2{ get; set; }
        public string hunloc2{ get; set; }
        public string italoc2{ get; set; }
        public string japloc2{ get; set; }
        public string porloc2{ get; set; }
        public string rusloc2{ get; set; }
        public string turloc2{ get; set; }
        public string chiloc2 { get; set; }

        public int pha1 { get; set; }
        public int pha2 { get; set; }
        public int pha3 { get; set; }
        public int pha4 { get; set; }
        public int pha5 { get; set; }
        public int regrowthPhase { get; set; }
        public bool doesCropRegrow { get; set; }

        public bool indoors { get; set; }
        public bool normal { get; set; }
        public bool paddy { get; set; }

        public List<string> cropSeasons { get; set; }

        public bool scythe { get; set; }
        public bool trellis { get; set; }

        public int maxHarvest { get; set; }
        public int minHarvest { get; set; }
        public int skillIncreaseOnHarvest { get; set; }
        public float extraHarvestChance { get; set; }

        public string RGBA1 { get; set; }
        public string RGBA2 { get; set; }
        public string RGBA3 { get; set; }

        public Color colour1 { get; set; }
        public Color colour2 { get; set; }
        public Color colour3 { get; set; }
    }

    public class Tree
    {
        public string treeName { get; set; }
        public string sapName { get; set; }
        public string sapDesc { get; set; }
        public string treeProduct { get; set; }

        public Image sprite1 { get; set; }
        public Image sprite2 { get; set; }

        public string sapShop { get; set; }
        public int sapBuyPrice { get; set; }
        public int sapSellPrice { get; set; }

        public string spaloc1 { get; set; }
        public string korloc1 { get; set; }
        public string gerloc1 { get; set; }
        public string freloc1 { get; set; }
        public string hunloc1 { get; set; }
        public string italoc1 { get; set; }
        public string japloc1 { get; set; }
        public string porloc1 { get; set; }
        public string rusloc1 { get; set; }
        public string turloc1 { get; set; }
        public string chiloc1 { get; set; }

        public string spaloc2 { get; set; }
        public string korloc2 { get; set; }
        public string gerloc2 { get; set; }
        public string freloc2 { get; set; }
        public string hunloc2 { get; set; }
        public string italoc2 { get; set; }
        public string japloc2 { get; set; }
        public string porloc2 { get; set; }
        public string rusloc2 { get; set; }
        public string turloc2 { get; set; }
        public string chiloc2 { get; set; }

        public string treeSeason { get; set; }
    }

    public class Item
    {
        public string itemName { get; set; }
        public string itemDesc { get; set; }
        public string itemType { get; set; }
        public Image sprite1 { get; set; }
        public Image sprite2 { get; set; }

        public string itemShop { get; set; }
        public int itemBuyPrice { get; set; }
        public int itemSellPrice { get; set; }

        public string spaloc1 { get; set; }
        public string korloc1 { get; set; }
        public string gerloc1 { get; set; }
        public string freloc1 { get; set; }
        public string hunloc1 { get; set; }
        public string italoc1 { get; set; }
        public string japloc1 { get; set; }
        public string porloc1 { get; set; }
        public string rusloc1 { get; set; }
        public string turloc1 { get; set; }
        public string chiloc1 { get; set; }

        public string spaloc2 { get; set; }
        public string korloc2 { get; set; }
        public string gerloc2 { get; set; }
        public string freloc2 { get; set; }
        public string hunloc2 { get; set; }
        public string italoc2 { get; set; }
        public string japloc2 { get; set; }
        public string porloc2 { get; set; }
        public string rusloc2 { get; set; }
        public string turloc2 { get; set; }
        public string chiloc2 { get; set; }

        public bool isColoured { get; set; }

        public bool hasCustomType { get; set; }
        public bool hasCustomTypeColour { get; set; }
        public string customType { get; set; }

        public Color typeColour { get; set; }
        public string typeRGBA { get; set; }

        public List<string> peopleWhoLove { get; set; }
        public List<string> peopleWhoLike { get; set; }
        public List<string> peopleWhoDislike { get; set; }
        public List<string> peopleWhoHate { get; set; }

        public bool isBuyable { get; set; }

        public bool isEdible { get; set; }
        public int energy { get; set; }
        public bool isDrink { get; set; }
        public bool itemIsColoured { get; internal set; }
    }
}
