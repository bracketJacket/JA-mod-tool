﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JA_mod_tool.Properties;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Globalization;
using System.ComponentModel.Design;

namespace JA_mod_tool
{

    public partial class Form1 : Form
    {
        //variables for the manifest.json
        public string stardewDirectory;
        public string modName;
        public string yourName;
        public string uniqueID;
        public string modDesc;

        public int currentCrop;
        public int currentItem;
        public int currentTree;

        public List<Crop> Crops = new List<Crop>();
        public List<Item> Items = new List<Item>();
        public List<Tree> Trees = new List<Tree>();

        public BindingSource cbs = new BindingSource();




        
        //item id list from txt files
        string[] itemNames;
        string[] itemIDs;
        AutoCompleteStringCollection nameSource = new AutoCompleteStringCollection();
        AutoCompleteStringCollection IDSource = new AutoCompleteStringCollection();

        List<Control> cropControls = new List<Control>();
        List<Control> treeControls = new List<Control>();
        List<Control> itemControls = new List<Control>();
        List<Control> controlsWithSmallerFont = new List<Control>();
        List<Control> controlsWithDifferentFont = new List<Control>();

        public Form1()
        {
            InitializeComponent();
        }

        //my methods

        //add all the correct controls to the lists
        public void ControlListSetup(List<Control> _controls)
        {
            if (_controls == cropControls)
            {
                _controls.AddRange(new Control[] { cropName, cropProduct, cropSeasons, cropType, doesCropRegrow, pha1, pha2, pha3, pha4, pha5, regrowthPhase, trellis, scythe, normal, indoors, paddy, colour1, colour2, colour3, bColour1, bColour2, bColour3, minHarvest, maxHarvest, extraHarvestChance, skillIncreaseOnHarvest, seedDesc, seedName, seedBuyPrice, seedSellPrice, seedShop });
            }
            if (_controls == treeControls)
            {
                _controls.AddRange(new Control[] { treeName, treeProduct, treeSeason, sapDesc, sapName, sapBuyPrice, sapSellPrice, sapShop});
            }
            if (_controls == itemControls)
            {
                _controls.AddRange(new Control[] { itemName, itemDesc, itemType, isBuyable, isDrink, isEdible, itemIsColoured, itemSellPrice, itemShop, itemBuyPrice, energy, peopleWhoDislike, peopleWhoLike, peopleWhoHate, peopleWhoLove, customType, hasCustomType, hasCustomTypeColour, TypeColour});
            }
        }

        //called when the user switches between objects in the asset list
        public void SetControlValues(List<Control> _controls, object _object)
        {
            foreach (Control _control in _controls)
            {
                if (_control is TextBox tb)
                {
                    if (_object.GetType().GetProperty(tb.Name).GetValue(_object) == null)
                    {
                        tb.Text = "";
                    }

                    else
                    {
                        tb.Text = _object.GetType().GetProperty(tb.Name).GetValue(_object).ToString();
                        Console.WriteLine(_object.GetType().GetProperty(tb.Name).GetValue(_object).ToString());
                        cbs.ResetBindings(true);

                    }
                }

                if (_control is RadioButton rb)
                {
                    rb.Checked = Convert.ToBoolean(_object.GetType().GetProperty(rb.Name).GetValue(_object));
                }
                if (_control is CheckBox chb)
                {
                    chb.Checked = Convert.ToBoolean(_object.GetType().GetProperty(chb.Name).GetValue(_object));
                }

                if (_control is ComboBox cb)
                {
                    string obb = _object.GetType().GetProperty(cb.Name).GetValue(_object) as string;
                    if (obb == null)
                        return;
                    cb.SelectedItem = obb;
                }

                if (_control is CheckedListBox chlb)
                {
                    List<String> obb = _object.GetType().GetProperty(chlb.Name).GetValue(_object) as List<string>;
                    if (obb == null)
                        return;

                    foreach (var o in obb)
                    {
                        for (int i = 0; i < chlb.Items.Count; i++)
                        {
                            if (chlb.Items[i].ToString() == o)
                                chlb.SetItemChecked(i, true);
                        }
                    }
                }

                if (_control is NumericUpDown nud)
                {
                    var obj = _object.GetType().GetProperty(nud.Name).GetValue(_object);
                    if (obj != null)
                    {
                        int obb = Convert.ToInt32(obj);
                        nud.Value = Convert.ToDecimal(obb);
                    }


                }
            }
        }

        //called when the user inputs data to the controls
        public void UpdateValues(Control _ctrl, object _obj)
        {
            if (_ctrl is TextBox tb)
            {
                _obj.GetType().GetProperty(tb.Name).SetValue(_obj, tb.Text);
                cbs.ResetBindings(true);
            }

            if (_ctrl is RadioButton rb)
            {
                _obj.GetType().GetProperty(rb.Name).SetValue(_obj, rb.Checked);
            }
            if (_ctrl is ComboBox cb)
            {
                _obj.GetType().GetProperty(cb.Name).SetValue(_obj, cb.SelectedItem.ToString());
            }
            if (_ctrl is CheckBox chb)
            {
                _obj.GetType().GetProperty(chb.Name).SetValue(_obj, chb.Checked);
            }
            if (_ctrl is CheckedListBox chlb)
            {
                _obj.GetType().GetProperty(chlb.Name).SetValue(_obj, chlb.CheckedItems);
            }
        }

        public void NewControlValues(List<Control> _controls)
        {
            foreach (Control _control in _controls)
            {
                if (_control is TextBox tb)
                {
                        tb.ResetText();
                        cbs.ResetBindings(true);
                }

                if (_control is RadioButton rb)
                {
                    rb.Checked = false;
                    cbs.ResetBindings(true);
                }
                if (_control is CheckBox chb)
                {
                    chb.Checked = false;
                    cbs.ResetBindings(true);
                }

                if (_control is ComboBox cb)
                {
                    cb.ResetText();
                    cbs.ResetBindings(true);
                }

                if (_control is CheckedListBox chlb)
                {
                    chlb.ClearSelected();
                    cbs.ResetBindings(true);
                }

                if (_control is NumericUpDown nud)
                {
                    nud.ResetText();
                    cbs.ResetBindings(true);
                }
            }
        }


        //loads sprite, puts it into picture box, and sets variable for current item/whatever
        private void LoadSprite(PictureBox _picturebox, Image _image)
        {
                DialogResult dialogResult = spritePick.ShowDialog();
                _image = Image.FromFile(spritePick.FileName);
                _picturebox.Image = _image;
        }

        //Some code I found online to read lines from a resource txt file
        private void LoadResourceTxt()
        {
            string[] ReadAllResourceLines(string resourceText)
            {
                using (StringReader reader = new StringReader(resourceText))
                {
                    return EnumerateLines(reader).ToArray();
                }
            }
            IEnumerable<string> EnumerateLines(TextReader reader)
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
            itemNames = ReadAllResourceLines(Properties.Resources.itemNames);
            itemIDs = ReadAllResourceLines(Properties.Resources.itemIDs);
            nameSource.AddRange(itemNames);
            itemBox1.AutoCompleteCustomSource = nameSource;
        }

        //some more code I found online, this time to help me with adding the custom font from resources
        public Font LoadResourceFont(int _font, int _size)
        {
            MemoryFonts.AddMemoryFont(Properties.Resources.Stardew_Valley);
            return MemoryFonts.GetFont(_font, _size);
        }

        //sets all fonts for labels and controls that need special setup
        public void FontSetup()
        {
            Font F16 = LoadResourceFont(0, 16);

            controlsWithSmallerFont.AddRange(new Control[] { chanceLab, seedPriceLab, seedSellPriceLab, lab1, lab2, lab3, lab4, lab4, phaseLab, sapBuyPriceLab, sapSellPriceLab, normal, indoors, paddy, isBuyable, isEdible, isDrink, peopleWhoDislike, peopleWhoHate, peopleWhoLike, peopleWhoLove, hasCustomType, hasCustomTypeColour, itemIsColoured, LocalizationB, saveFolderB, saveButton, loadB });
            controlsWithDifferentFont.AddRange(new Control[] { cropName, cropProduct, seedDesc, seedName, treeName, treeProduct, sapName, sapDesc, itemName, itemDesc, seedBuyPrice, seedSellPrice, sapBuyPrice, sapSellPrice, itemBuyPrice, itemSellPrice, customType, pha1, pha2, pha3, pha4, pha5, regrowthPhase, minHarvest, maxHarvest, extraHarvestChance, skillIncreaseOnHarvest, modNameBox, modDescBox, idBox, yourNameBox, energy });

            foreach (Control ctrl in controlsWithSmallerFont)
            {
                ctrl.Font = F16;
            }
            foreach (Control ctrl in controlsWithDifferentFont)
            {
                ctrl.Font = new Font("Arial Narrow", 12f);
            }
                
        }

        public void AddToList()
        {
            if (tabs.SelectedIndex == 0)
            {
                    Crops.Add(new Crop { cropName = cropName.Text });
                    currentCrop = Crops.IndexOf(Crops.Last());
                    cbs.ResetBindings(true);
                    cropsList.SelectedIndex = Crops.IndexOf(Crops.Last());
                    NewControlValues(cropControls);
            }

            if (tabs.SelectedIndex == 1)
            {
                Trees.Add(new Tree());
                currentTree = treesList.SelectedIndex;
                SetControlValues(treeControls, treesList.SelectedItem);
            }

            if (tabs.SelectedIndex == 2)
            {
                Items.Add(new Item());
                currentItem = itemsList.SelectedIndex;
                SetControlValues(itemControls, itemsList.SelectedItem);
            }
        }
        public void RemoveFromList()
        {
            if (tabs.SelectedTab.Name == "cropTab")
                Crops.Remove(Crops[currentCrop]);
            if (tabs.SelectedTab.Name == "treeTab")
                Trees.Remove(Trees[currentTree]); ;
            if (tabs.SelectedTab.Name == "itemTab")
                Items.Remove(Items[currentItem]);
        }
        //-------------------------------------------------------------------



        //load form
        public void Form1_Load(object sender, EventArgs e)
        {
            cbs.DataSource = Crops;
            cropsList.DataSource = cbs;

            ControlListSetup(cropControls);
            ControlListSetup(treeControls);
            ControlListSetup(itemControls);

            LoadResourceTxt();
            Font F20 = LoadResourceFont(0, 20);
        }
        //---------------------------------------------------




        //main mod values input
        private void modNameBox_TextChanged(object sender, EventArgs e)
        {
            modName = modNameBox.Text;
        }

        private void yourNameBox_TextChanged(object sender, EventArgs e)
        {
            yourName = yourNameBox.Text;
        }

        private void modDescBox_TextChanged(object sender, EventArgs e)
        {
            modDesc = modDescBox.Text;
        }

        private void idBox_TextChanged(object sender, EventArgs e)
        {
            uniqueID = idBox.Text;
        }
        //-----------------------------------------------------------

        //events outside the tabpages//
        public void LocalizationB_Click(object sender, EventArgs e)
        {
            Localization locForm = new Localization();
        }

        private void addB_Click(object sender, EventArgs e)
        {
            AddToList();
        }

        private void cropsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentCrop = cropsList.SelectedIndex;
            SetControlValues(cropControls, Crops[currentCrop]);
            cbs.ResetBindings(true);
        }
        //---------------------------------------------------



        //crop tab events//

        private void cropName_TextChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void cropProduct_TextChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }


        private void seedName_TextChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }


        private void seedDesc_TextChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }


        private void cropSeasons_SelectedIndexChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }


        private void seedShop_SelectedIndexChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }


        private void cropType_SelectedIndexChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }


        private void seedBuyPrice_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void seedSellPrice_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
           

        private void trellis_CheckedChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void scythe_CheckedChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void normal_CheckedChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void indoors_CheckedChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void paddy_CheckedChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void colour1_CheckedChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void colour2_CheckedChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void colour3_CheckedChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void minHarvest_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void maxHarvest_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void extraHarvestChance_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void skillIncreaseOnHarvest_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
        

        private void pha1_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }
            
        private void pha2_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }

        private void pha3_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }

        private void pha4_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }

        private void pha5_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }

        private void regrowthPhase_ValueChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }

        private void doesCropRegrow_CheckedChanged(object sender, EventArgs e) { if (Crops.Count != 0) { UpdateValues(sender as Control, Crops[currentCrop]); }; }

        private void colourButton1_Click(object sender, EventArgs e)
        {
            DialogResult dialogresult = colourPick1.ShowDialog();
            bColour1.BackColor = colourPick1.Color;
            Crops[currentCrop].RGBA1 = "\u0022" + colourPick1.Color.R.ToString() + ", " + colourPick1.Color.G.ToString() + ", " + colourPick1.Color.B.ToString() + ", " + colourPick1.Color.A.ToString() + "\u0022";
        }

        private void colourButton2_Click(object sender, EventArgs e)
        {
            DialogResult dialogresult = colourPick2.ShowDialog();
            bColour2.BackColor = colourPick2.Color;
            Crops[currentCrop].RGBA2 = "\u0022" + colourPick2.Color.R.ToString() + ", " + colourPick2.Color.G.ToString() + ", " + colourPick2.Color.B.ToString() + ", " + colourPick2.Color.A.ToString() + "\u0022";

        }

        private void colourButton3_Click(object sender, EventArgs e)
        {
            DialogResult dialogresult = colourPick3.ShowDialog();
            bColour3.BackColor = colourPick3.Color;
            Crops[currentCrop].RGBA3 = "\u0022" + colourPick3.Color.R.ToString() + ", " + colourPick3.Color.G.ToString() + ", " + colourPick3.Color.B.ToString() + ", " + colourPick3.Color.A.ToString() + "\u0022";
        }


        //--------------------------------------------------------------------







        private void spritePick_FileOk(object sender, CancelEventArgs e)
        {

        }


        //tree tab events

        private void treeNameBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void treeProductBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void sapNameBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void sapDescBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void treeSeasonBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (treeSeason.CheckedItems.Count > 1)
            {
                treeSeason.SetItemChecked(0, false);
                treeSeason.SetItemChecked(1, false);
                treeSeason.SetItemChecked(2, false);
                treeSeason.SetItemChecked(3, false);
                treeSeason.SetItemChecked(treeSeason.SelectedIndex, true);
            }
        }

        private void itemIsColouredBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void treeShopBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void sapPriceBox_ValueChanged(object sender, EventArgs e)
        {
            //sapPrice = Convert.ToInt32(Math.Round(sapBuyPrice.Value, 0));
        }

        private void sapSellPriceBox_ValueChanged(object sender, EventArgs e)
        {
            //sapSellPrice = Convert.ToInt32(Math.Round(sapSellPrice.Value, 0));
        }


        //store gift tastes to string arrays
        private void iNameBox_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void iTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void itemBox1_TextChanged(object sender, EventArgs e)
        {
            if (itemNames.Contains(itemBox1.Text))
            {
                int nameIndex = Array.IndexOf(itemNames, itemBox1.SelectedText);
                itemidLab.Text = itemIDs.GetValue(nameIndex).ToString();
            }
        }

        private void hatedBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dislikedBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void likedBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lovedBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        //write variables to json files
        //ignore all the comments and stuff, its just from the templates that i copy/pasted from lol
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (tabs.SelectedIndex == 0)
            {
                string phases = Crops[currentCrop].pha1 + ", " + Crops[currentCrop].pha2;

                if (Crops[currentCrop].pha3 > 0)
                {
                    phases = phases + ", " + Crops[currentCrop].pha3;
                    if (Crops[currentCrop].pha4 > 0)
                    {
                        phases = phases + ", " + Crops[currentCrop].pha4;
                        if (Crops[currentCrop].pha5 > 0)
                        {
                            phases = phases + ", " + Crops[currentCrop].pha5;
                        }
                    }

                }


                string regrow = Crops[currentCrop].regrowthPhase.ToString();

                if (!Crops[currentCrop].doesCropRegrow)
                {
                    regrow = "null";
                }


                string cropGrowType = "normal";

                if (Crops[currentCrop].indoors)
                    cropGrowType = "indoors";

                if (Crops[currentCrop].paddy)
                    cropGrowType = "paddy";

                if (!colour1.Checked)
                    Crops[currentCrop].RGBA1 = "null";

                if (!colour2.Checked)
                    Crops[currentCrop].RGBA2 = "null";
                
                if (!colour3.Checked)
                    Crops[currentCrop].RGBA3 = "null";
                

                Directory.CreateDirectory("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName);
                Directory.CreateDirectory("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/Crops/" + Crops[currentCrop].cropName);
                //cropSprite.Save("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/Crops/" + cropName + "/" + "crop.png");
                //seedSprite.Save("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/Crops/" + cropName + "/" + "seeds.png");
                File.WriteAllText("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/manifest.json",

                    "{\n" +
                    "\t\u0022Name\u0022: \u0022" + modName + "\u0022,\n" +
                    "\t\u0022Author\u0022: \u0022" + yourName + "\u0022,\n" +
                    "\t\u0022Description\u0022: \u0022" + modDesc + "\u0022,\n" +
                    "\t\u0022Version\u0022: \u00221.0\u0022,\n" +
                    "\t\u0022UniqueID\u0022: \u0022" + uniqueID + "\u0022,\n" +
                    "\t\t\u0022ContentPackFor\u0022: {\n" +
                    "\t\t\u0022UniqueID\u0022: \u0022spacechase0.JsonAssets\u0022\n" +
                    "\t},\n" +
                    "}");

                File.WriteAllText("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/Crops/" + Crops[currentCrop].cropName + "/" + "crop.json",


                    "{\n" +
                    "\t\u0022Name\u0022: \u0022" + Crops[currentCrop].cropName + "\u0022,\n" +//The name you would like your object to have, this should be identical to the subfolder name.
                    "\t\u0022Product\u0022: \u0022" + Crops[currentCrop].cropProduct + "\u0022,\n" +//Determines what the crop produces. This will correspond to a folder with the same name in Objects (ex. Both folders will be named "Honeysuckle"). (optional) You can produce vanilla items. Instead of a named object you will use the objects ID number and not include a corresponding Objects folder.
                    "\t\u0022SeedName\u0022: \u0022" + Crops[currentCrop].seedName + "\u0022,\n" +//The seed name of the crop. Typically crop name + seeds or starter.
                    "\t\u0022SeedDescription\u0022: \u0022" + Crops[currentCrop].seedDesc + "\u0022,\n" + //Describe what season you plant these in. Also note if it continues to grow after first harvest and how many days it takes to regrow.
                    "\t\u0022Type\u0022: \u0022" + Crops[currentCrop].cropType + "\u0022,\n" + //Available types are Flower, Fruit, Vegetable, Gem, Fish, Egg, Milk, Cooking, Crafting, Mineral, Meat, Metal, Junk, Syrup, MonsterLoot, ArtisanGoods, and Seeds.
                    "\t\u0022CropType\u0022: \u0022" + cropGrowType + "\u0022,\n" + //Available types are Normal, IndoorsOnly, and Paddy. If no `CropType` is specified, Normal will be the default.
                    "\t\u0022Seasons\u0022: [" + Crops[currentCrop].cropSeasons + "],\n" + //Seasons must be in lowercase and in quotation marks, so if you want to make your crop last all year, you'd put in "spring", "summer", "fall", "winter". If you want to make winter plants, you will have to require SpaceCore for your content pack.
                    "\t\u0022Phases\u0022: [" + phases + "],\n" + //Determines how long each phase lasts. Crops can have 2-5 phases, and the numbers in phases refer to how many days a plant spends in that phase. Seeds do not count as a phase. If your crop has regrowth, the last number in this set corresponds to how many days it takes for the crop to regrow. Ex. [1, 2, 3, 4, 3] This crop takes 10 days to grow and 3 days to regrow.
                    "\t\u0022RegrowthPhase\u0022: " + regrow + ",\n" + //If your plant is a one time harvest set this to -1. If it does, this determines which sprite the regrowth starts at. I typically recommend the sprite right before the harvest. Requires additional sprite at the end of the crop.png
                    "\t\u0022HarvestWithScythe\u0022: \u0022" + Crops[currentCrop].scythe.ToString() + "\u0022,\n" + //Set to true or false.
                    "\t\u0022TrellisCrop\u0022: \u0022" + Crops[currentCrop].trellis.ToString() + "\u0022,\n" + //Set to true or false. Determines if you can pass through a crop or not. Flowers cannot grow on trellises and have colors.
                    "\t\u0022Colors\u0022: [\n" +
                        "\t\t" + Crops[currentCrop].RGBA1 + ",\n" + //Colors use RGBA for color picking, set to null if your plant does not have colors.
                        "\t\t" + Crops[currentCrop].RGBA2 + ",\n" + //You can have a maximum of three colors.
                        "\t\t" + Crops[currentCrop].RGBA3 + "\n" +
                    "\t],\n" +
                    "\t\u0022Bonus\u0022:\n" + //This block determines the chance to get multiple crops.
                    "\t{\n" +
                        "\t\t\u0022MinimumPerHarvest\u0022: " + Crops[currentCrop].minHarvest.ToString() + ",\n" + //Minimum number of crops you will get per harvest. Must be one or greater.
                        "\t\t\u0022MaximumPerHarvest\u0022: " + Crops[currentCrop].maxHarvest.ToString() + ",\n" + //Maximum number of crops you will get per harvest. Must be one or greater. Recommended not to exceed 10.
                        "\t\t\u0022MaxIncreasePerFarmLevel\u0022: " + Crops[currentCrop].skillIncreaseOnHarvest.ToString() + ",\n" + //How many farming skill experience points you get from harvesting.
                        "\t\t\u0022ExtraChance\u0022: " + Crops[currentCrop].extraHarvestChance + ",\n" + //Value between 0 and 1.
                    "\t},\n" +
                    "\t\u0022SeedPurchaseRequirements\u0022: null,\n" + //See Event Preconditions (https://stardewvalleywiki.com/Modding:Event_data#Event_preconditions). 
                    "\n" +                                              //If you do not want to have any PurchaseRequirements set this to null
                    "\t\u0022SeedPurchaseFrom\u0022: \u0022" + Crops[currentCrop].seedShop + "\u0022,\n" + //Who you can purchase seeds from. Valid entries are: Willy, Pierre, Robin, Sandy, Krobus, Clint, Harvey, Marlon, and Dwarf. If an NPC isn't listed here they can't be used. Pierre is the default vendor.
                    "\t\u0022SeedPurchasePrice\u0022: " + Crops[currentCrop].seedBuyPrice + ",\n" + //How much you can purchase seeds for.
                    "\t\u0022Price\u0022: " + Crops[currentCrop].seedSellPrice + ",\n" + //How much you can sell seeds for.
                    "\n" +
                    // Localization
                    "\t\u0022SeedNameLocalization\u0022:\n" +
                    "\t{\n" +
                        "\t\t\u0022es\u0022:\u0022" + Crops[currentCrop].spaloc1 + "\u0022,\n" + //Spansih
                        "\t\t\u0022ko\u0022:\u0022" + Crops[currentCrop].korloc1 + "\u0022,\n" + //Korean
                        "\t\t\u0022de\u0022:\u0022" + Crops[currentCrop].gerloc1 + "\u0022,\n" + //German
                        "\t\t\u0022fr\u0022:\u0022" + Crops[currentCrop].freloc1 + "\u0022,\n" + //French
                        "\t\t\u0022hu\u0022:\u0022" + Crops[currentCrop].hunloc1 + "\u0022,\n" + //Hungarian
                        "\t\t\u0022it\u0022:\u0022" + Crops[currentCrop].italoc1 + "\u0022,\n" + //Italian
                        "\t\t\u0022ja\u0022:\u0022" + Crops[currentCrop].japloc1 + "\u0022,\n" + //Japanese
                        "\t\t\u0022pt\u0022:\u0022" + Crops[currentCrop].porloc1 + "\u0022,\n" + //Portuguese
                        "\t\t\u0022ru\u0022:\u0022" + Crops[currentCrop].rusloc1 + "\u0022,\n" + //Russian
                        "\t\t\u0022tr\u0022:\u0022" + Crops[currentCrop].turloc1 + "\u0022,\n" + //Turkish
                        "\t\t\u0022zh\u0022:\u0022" + Crops[currentCrop].chiloc1 + "\u0022,\n" + //Chinese (Simplified)
                    "\t},\n" +
                    "\t\u0022SeedDescriptionLocalization\u0022:\n" +
                    "\t{\n" +
                        "\t\t\u0022es\u0022:\u0022" + Crops[currentCrop].spaloc2 + "\u0022,\n" + //Spansih
                        "\t\t\u0022ko\u0022:\u0022" + Crops[currentCrop].korloc2 + "\u0022,\n" + //Korean
                        "\t\t\u0022de\u0022:\u0022" + Crops[currentCrop].gerloc2 + "\u0022,\n" + //German
                        "\t\t\u0022fr\u0022:\u0022" + Crops[currentCrop].freloc2 + "\u0022,\n" + //French
                        "\t\t\u0022hu\u0022:\u0022" + Crops[currentCrop].hunloc2 + "\u0022,\n" + //Hungarian
                        "\t\t\u0022it\u0022:\u0022" + Crops[currentCrop].italoc2 + "\u0022,\n" + //Italian
                        "\t\t\u0022ja\u0022:\u0022" + Crops[currentCrop].japloc2 + "\u0022,\n" + //Japanese
                        "\t\t\u0022pt\u0022:\u0022" + Crops[currentCrop].porloc2 + "\u0022,\n" + //Portuguese
                        "\t\t\u0022ru\u0022:\u0022" + Crops[currentCrop].rusloc2 + "\u0022,\n" + //Russian
                        "\t\t\u0022tr\u0022:\u0022" + Crops[currentCrop].turloc2 + "\u0022,\n" + //Turkish
                        "\t\t\u0022zh\u0022:\u0022" + Crops[currentCrop].chiloc2 + "\u0022,\n" + //Chinese (Simplified)
                    "\t}\n" +
                    "}"
                    );

            }

            if (tabs.SelectedIndex == 1)
            {

                Directory.CreateDirectory("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName);
                Directory.CreateDirectory("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/FruitTrees/" + Trees[currentTree].treeName);
                //treeSprite.Save("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/FruitTrees/" + treeName + "/" + "tree.png");
                //sapSprite.Save("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/FruitTrees/" + treeName + "/" + "sapling.png");
                File.WriteAllText("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/manifest.json",

                    "{\n" +
                    "\t\u0022Name\u0022: \u0022" + modName + "\u0022,\n" +
                    "\t\u0022Author\u0022: \u0022" + yourName + "\u0022,\n" +
                    "\t\u0022Description\u0022: \u0022" + modDesc + "\u0022,\n" +
                    "\t\u0022Version\u0022: \u00221.0\u0022,\n" +
                    "\t\u0022UniqueID\u0022: \u0022" + uniqueID + "\u0022,\n" +
                    "\t\t\u0022ContentPackFor\u0022: {\n" +
                    "\t\t\u0022UniqueID\u0022: \u0022spacechase0.JsonAssets\u0022\n" +
                    "\t},\n" +
                    "}");

                File.WriteAllText("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/FruitTrees/" + Trees[currentTree].treeName + "/" + "tree.json",


                    "{\n" +
                    "\t\u0022Name\u0022: \u0022" + Trees[currentTree].treeName + "\u0022,\n" +//The name you would like your object to have, this should be identical to the subfolder name.
                    "\t\u0022Product\u0022: \u0022" + Trees[currentTree].treeProduct + "\u0022,\n" +
                    "\t\u0022SaplingName\u0022: \u0022" + Trees[currentTree].sapName + "\u0022,\n" +//The seed name of the crop. Typically crop name + seeds or starter.
                    "\t\u0022SaplingDescription\u0022: \u0022" + Trees[currentTree].sapDesc + "\u0022,\n" + //Describe what season you plant these in. Also note if it continues to grow after first harvest and how many days it takes to regrow.
                    "\t\u0022Season\u0022: " + Trees[currentTree].treeSeason + ",\n" + //Seasons must be in lowercase and in quotation marks, so if you want to make your crop last all year, you'd put in "spring", "summer", "fall", "winter". If you want to make winter plants, you will have to require SpaceCore for your content pack.
                    "\t\u0022SaplingPurchasePrice\u0022:" + Trees[currentTree].sapBuyPrice + ",\n" +

                    "\t\u0022SaplingPurchaseRequirements\u0022: null,\n" + //See Event Preconditions (https://stardewvalleywiki.com/Modding:Event_data#Event_preconditions). 
                    "\n" +                                              //If you do not want to have any PurchaseRequirements set this to null
                    "\t\u0022SaplingPurchaseFrom\u0022: \u0022" + Trees[currentTree].sapShop + "\u0022,\n" + //Who you can purchase seeds from. Valid entries are: Willy, Pierre, Robin, Sandy, Krobus, Clint, Harvey, Marlon, and Dwarf. If an NPC isn't listed here they can't be used. Pierre is the default vendor.
                    "\t\u0022Price\u0022: " + Trees[currentTree].sapSellPrice + ",\n" + //How much you can sell seeds for.
                    "\n" +
                    // Localization
                    "\t\u0022NameLocalization\u0022:\n" +
                    "\t{\n" +
                        "\t\t\u0022es\u0022:\u0022" + Trees[currentTree].spaloc1 + "\u0022,\n" + //Spansih
                        "\t\t\u0022ko\u0022:\u0022" + Trees[currentTree].korloc1 + "\u0022,\n" + //Korean
                        "\t\t\u0022de\u0022:\u0022" + Trees[currentTree].gerloc1 + "\u0022,\n" + //German
                        "\t\t\u0022fr\u0022:\u0022" + Trees[currentTree].freloc1 + "\u0022,\n" + //French
                        "\t\t\u0022hu\u0022:\u0022" + Trees[currentTree].hunloc1 + "\u0022,\n" + //Hungarian
                        "\t\t\u0022it\u0022:\u0022" + Trees[currentTree].italoc1 + "\u0022,\n" + //Italian
                        "\t\t\u0022ja\u0022:\u0022" + Trees[currentTree].japloc1 + "\u0022,\n" + //Japanese
                        "\t\t\u0022pt\u0022:\u0022" + Trees[currentTree].porloc1 + "\u0022,\n" + //Portuguese
                        "\t\t\u0022ru\u0022:\u0022" + Trees[currentTree].rusloc1 + "\u0022,\n" + //Russian
                        "\t\t\u0022tr\u0022:\u0022" + Trees[currentTree].turloc1 + "\u0022,\n" + //Turkish
                        "\t\t\u0022zh\u0022:\u0022" + Trees[currentTree].chiloc1 + "\u0022,\n" + //Chinese (Simplified)
                    "\t},\n" +
                    "\t\u0022DescriptionLocalization\u0022:\n" +
                    "\t{\n" +
                        "\t\t\u0022es\u0022:\u0022" + Trees[currentTree].spaloc2 + "\u0022,\n" + //Spansih
                        "\t\t\u0022ko\u0022:\u0022" + Trees[currentTree].korloc2 + "\u0022,\n" + //Korean
                        "\t\t\u0022de\u0022:\u0022" + Trees[currentTree].gerloc2 + "\u0022,\n" + //German
                        "\t\t\u0022fr\u0022:\u0022" + Trees[currentTree].freloc2 + "\u0022,\n" + //French
                        "\t\t\u0022hu\u0022:\u0022" + Trees[currentTree].hunloc2 + "\u0022,\n" + //Hungarian
                        "\t\t\u0022it\u0022:\u0022" + Trees[currentTree].italoc2 + "\u0022,\n" + //Italian
                        "\t\t\u0022ja\u0022:\u0022" + Trees[currentTree].japloc2 + "\u0022,\n" + //Japanese
                        "\t\t\u0022pt\u0022:\u0022" + Trees[currentTree].porloc2 + "\u0022,\n" + //Portuguese
                        "\t\t\u0022ru\u0022:\u0022" + Trees[currentTree].rusloc2 + "\u0022,\n" + //Russian
                        "\t\t\u0022tr\u0022:\u0022" + Trees[currentTree].turloc2 + "\u0022,\n" + //Turkish
                        "\t\t\u0022zh\u0022:\u0022" + Trees[currentTree].chiloc2 + "\u0022,\n" + //Chinese (Simplified)
                    "\t}\n" +
                    "}"
                    );

            }

            if (tabs.SelectedIndex == 2)
            {

                string loveList = string.Join("\t\u0022, \t\u0022", Items[currentItem].peopleWhoLove);
                string hateList = string.Join("\t\u0022, \t\u0022", Items[currentItem].peopleWhoHate);
                string likeList = string.Join("\t\u0022, \t\u0022", Items[currentItem].peopleWhoLike);
                string dislikeList = string.Join("\t\u0022, \t\u0022", Items[currentItem].peopleWhoDislike);

                Directory.CreateDirectory("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName);
                Directory.CreateDirectory("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/Objects/" + itemName);
                //iSprite.Save("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/Objects/" + itemName + "/" + "object.png");

                if (Items[currentItem].itemIsColoured)
                {
                    //iSpriteColoured.Save("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/Objects/" + iName + "/" + "color.png");
                }

                File.WriteAllText("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/manifest.json",

                    "{\n" +
                    "\t\u0022Name\u0022: \u0022" + modName + "\u0022,\n" +
                    "\t\u0022Author\u0022: \u0022" + yourName + "\u0022,\n" +
                    "\t\u0022Description\u0022: \u0022" + modDesc + "\u0022,\n" +
                    "\t\u0022Version\u0022: \u00221.0\u0022,\n" +
                    "\t\u0022UniqueID\u0022: \u0022" + uniqueID + "\u0022,\n" +
                    "\t\t\u0022ContentPackFor\u0022: {\n" +
                    "\t\t\u0022UniqueID\u0022: \u0022spacechase0.JsonAssets\u0022\n" +
                    "\t},\n" +
                    "}");

                File.WriteAllText("C:/Program Files (x86)/Steam/steamapps/common/Stardew Valley/Mods/[JA]" + modName + "/Objects/" + Items[currentItem].itemName + "/" + "object.json",


                    "{\n" +
                    "\t\u0022Name\u0022: \u0022" + Items[currentItem].itemName + "\u0022,\n" +//The name you would like your object to have, this should be identical to the subfolder name
                    "\t\u0022Description\u0022: \u0022" + Items[currentItem].itemDesc + "\u0022,\n" + 
                    "\t\u0022Category\u0022: " + Items[currentItem].itemType + ",\n" +
                    "\t\u0022CategoryTextOverride\u0022: " + Items[currentItem].customType + ",\n" +
                    "\t\u0022CategoryColorOverride\u0022: " + Items[currentItem].typeColour + ",\n" +
                    "\t\u0022Edibility\u0022:" + Items[currentItem].energy + ",\n" +
                    "\t\u0022EdibleIsDrink\u0022:" + Items[currentItem].isDrink + ",\n" +
                    "\t\u0022Price\u0022: " + Items[currentItem].itemSellPrice + ",\n" + //See Event Preconditions (https://stardewvalleywiki.com/Modding:Event_data#Event_preconditions). 
                    "\n" +
                    "\t\u0022CanPurchase\u0022:" + Items[currentItem].isBuyable + ",\n" +//If you do not want to have any PurchaseRequirements set this to null
                    "\t\u0022PurchaseFrom\u0022: \u0022" + Items[currentItem].itemShop + "\u0022,\n" + //Who you can purchase seeds from. Valid entries are: Willy, Pierre, Robin, Sandy, Krobus, Clint, Harvey, Marlon, and Dwarf. If an NPC isn't listed here they can't be used. Pierre is the default vendor.
                    "\t\u0022PurchasePrice\u0022: " + Items[currentItem].itemBuyPrice + ",\n" +
                    "\t\u0022PurchaseRequirements\u0022: null,\n" +
                    "\n" +
                    "\t\u0022Recipe\u0022: null,\n" +
                    "\t\u0022IsColored\u0022:" + Items[currentItem].itemIsColoured + ",\n" +

                    "\t\u0022GiftTastes\t\u0022:\n" + //If an NPC isn't listed their gift taste will be neautral. If every NPC is specified you may remove unused categories.
    "\t{\n" +
                    "\t\t\u0022Love\t\u0022: [\u0022" + loveList + "],\n" +
                    "\t\t\u0022Like\t\u0022: [\u0022" + likeList + "],\n" +
                    "\t\t\u0022Dislike\t\u0022: [\u0022" + dislikeList + "],\n" +
                    "\t\t\u0022Hate\t\u0022: [\u0022" + hateList + "]\n" +
    "\t{\n" +
                    "\t\u0022NameLocalization\u0022:\n" +
                    "\t{\n" +
                        "\t\t\u0022es\u0022:\u0022" + Items[currentItem].spaloc1 + "\u0022,\n" + //Spansih
                        "\t\t\u0022ko\u0022:\u0022" + Items[currentItem].korloc1 + "\u0022,\n" + //Korean
                        "\t\t\u0022de\u0022:\u0022" + Items[currentItem].gerloc1 + "\u0022,\n" + //German
                        "\t\t\u0022fr\u0022:\u0022" + Items[currentItem].freloc1 + "\u0022,\n" + //French
                        "\t\t\u0022hu\u0022:\u0022" + Items[currentItem].hunloc1 + "\u0022,\n" + //Hungarian
                        "\t\t\u0022it\u0022:\u0022" + Items[currentItem].italoc1 + "\u0022,\n" + //Italian
                        "\t\t\u0022ja\u0022:\u0022" + Items[currentItem].japloc1 + "\u0022,\n" + //Japanese
                        "\t\t\u0022pt\u0022:\u0022" + Items[currentItem].porloc1 + "\u0022,\n" + //Portuguese
                        "\t\t\u0022ru\u0022:\u0022" + Items[currentItem].rusloc1 + "\u0022,\n" + //Russian
                        "\t\t\u0022tr\u0022:\u0022" + Items[currentItem].turloc1 + "\u0022,\n" + //Turkish
                        "\t\t\u0022zh\u0022:\u0022" + Items[currentItem].chiloc1 + "\u0022,\n" + //Chinese (Simplified)
                    "\t},\n" +
                    "\t\u0022DescriptionLocalization\u0022:\n" +
                    "\t{\n" +
                        "\t\t\u0022es\u0022:\u0022" + Items[currentItem].spaloc2 + "\u0022,\n" + //Spansih
                        "\t\t\u0022ko\u0022:\u0022" + Items[currentItem].korloc2 + "\u0022,\n" + //Korean
                        "\t\t\u0022de\u0022:\u0022" + Items[currentItem].gerloc2 + "\u0022,\n" + //German
                        "\t\t\u0022fr\u0022:\u0022" + Items[currentItem].freloc2 + "\u0022,\n" + //French
                        "\t\t\u0022hu\u0022:\u0022" + Items[currentItem].hunloc2 + "\u0022,\n" + //Hungarian
                        "\t\t\u0022it\u0022:\u0022" + Items[currentItem].italoc2 + "\u0022,\n" + //Italian
                        "\t\t\u0022ja\u0022:\u0022" + Items[currentItem].japloc2 + "\u0022,\n" + //Japanese
                        "\t\t\u0022pt\u0022:\u0022" + Items[currentItem].porloc2 + "\u0022,\n" + //Portuguese
                        "\t\t\u0022ru\u0022:\u0022" + Items[currentItem].rusloc2 + "\u0022,\n" + //Russian
                        "\t\t\u0022tr\u0022:\u0022" + Items[currentItem].turloc2 + "\u0022,\n" + //Turkish
                        "\t\t\u0022zh\u0022:\u0022" + Items[currentItem].chiloc2 + "\u0022,\n" + //Chinese (Simplified)
                    "\t}\n" +
                    "}"
                    );

            }
        }



        private void saveFolderB_Click(object sender, EventArgs e)
        {

        }



        //when image boxes are double clicked, open a file browser and load the image into the picture box

        private void cropSpriteBox_DoubleClick(object sender, EventArgs e)
        {
            PictureBox ctrl = sender as PictureBox;
            LoadSprite(ctrl, Crops[currentItem].sprite1);
        }

        private void seedSpriteBox_DoubleClick(object sender, EventArgs e)
        {
            PictureBox ctrl = sender as PictureBox;
            LoadSprite(ctrl, Crops[currentCrop].sprite2);
        }

        private void treeSpriteBox_DoubleClick(object sender, EventArgs e)
        {
            PictureBox ctrl = sender as PictureBox;
            LoadSprite(ctrl, Trees[currentTree].sprite1);
        }

        private void sapSpriteBox_DoubleClick(object sender, EventArgs e)
        {
            PictureBox ctrl = sender as PictureBox;
            LoadSprite(ctrl, Trees[currentTree].sprite2);
        }

        private void iSpriteBox_DoubleClick(object sender, EventArgs e)
        {
            PictureBox ctrl = sender as PictureBox;
            LoadSprite(ctrl, Items[currentItem].sprite1);
        }

        private void iColourSpriteBox_DoubleClick(object sender, EventArgs e)
        {
            PictureBox ctrl = sender as PictureBox;
            LoadSprite(ctrl, Items[currentItem].sprite2);
        }


    }
}

