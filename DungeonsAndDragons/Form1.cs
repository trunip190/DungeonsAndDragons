using DungeonsAndDragons.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace DungeonsAndDragons
{
    public partial class Form1 : Form
    {
        # region declarations
        List<Loot> LootList = new List<Loot>();
        List<PowerClass> PowerList = new List<PowerClass>();
        public int pWidth = 320;
        public int pHeight = 450;
        public int PowerPage = 0;
        # endregion

        public Form1()
        {
            InitializeComponent();

            //Load MRU's
            UpdateRecentFiles("");
        }

        # region Save/Load
        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveForm = new SaveFileDialog();

            saveForm.Filter = "xml files(*.xml)|*.xml";
            //saveForm.ShowDialog();

            if (saveForm.ShowDialog() == DialogResult.OK)
            {
                xmlSave(saveForm.FileName);
                //ExportImage(saveForm.FileName + ".bmp");
            }
        }

        private void xmlSave(string path)
        {
            # region declarations and preparation
            XmlTextWriter streamWrite = new XmlTextWriter(path, null);
            Panel tempPan = panel1;
            TextBox tempTex;

            IComparer<PowerClass> comparer = new OrderPower();

            PowerList.Sort(comparer);

            streamWrite.WriteStartDocument();
            streamWrite.WriteComment("My D&D save file");
            streamWrite.WriteStartElement("Data");
            streamWrite.WriteString("\r\n");
            # endregion

            for (int t = 0; t < 4; t++)
            {
                switch (t)
                {
                    # region Stats
                    case 0:
                        tempPan = panel1;

                        streamWrite.WriteStartElement("Session");

                        # region deathThrows section
                        int d = 0;
                        if (radioButton1.Checked) { d = 1; }
                        if (radioButton2.Checked) { d = 2; }
                        if (radioButton3.Checked) { d = 3; }

                        streamWrite.WriteString("\r\n\t");
                        streamWrite.WriteElementString("DeathThrows", d.ToString());
                        streamWrite.WriteString("\r\n");
                        # endregion

                        # region Second Wind
                        streamWrite.WriteString("\t");
                        streamWrite.WriteElementString("SecondWind", SecondWind.Checked.ToString());
                        streamWrite.WriteString("\r\n");
                        # endregion

                        streamWrite.WriteEndElement();
                        streamWrite.WriteString("\r\n");

                        streamWrite.WriteStartElement("Stats");
                        streamWrite.WriteString("\r\n");
                        goto case 9;
                    # endregion

                    # region Skills
                    case 1:
                        tempPan = panel2;

                        streamWrite.WriteStartElement("Skills");
                        streamWrite.WriteString("\r\n");
                        goto case 9;
                    # endregion

                    # region Loot
                    case 2:
                        streamWrite.WriteStartElement("Loot");
                        streamWrite.WriteString("\r\n");

                        //cycle LootList
                        foreach (Loot l in LootList)
                        {
                            streamWrite.WriteString("\t");
                            streamWrite.WriteStartElement("loot");
                            streamWrite.WriteAttributeString("Name", l.Name);
                            streamWrite.WriteAttributeString("Type", l.type);
                            streamWrite.WriteAttributeString("Count", l.quantity.ToString());
                            streamWrite.WriteAttributeString("Description", l.Description);
                            streamWrite.WriteEndElement();
                            streamWrite.WriteString("\r\n");
                        }

                        streamWrite.WriteEndElement();
                        streamWrite.WriteString("\r\n");
                        break;
                    # endregion

                    # region Powers
                    case 3:
                        streamWrite.WriteStartElement("PowerStats");
                        streamWrite.WriteString("\r\n");

                        foreach (PowerClass p in PowerList)//for each Power Card
                        {
                            streamWrite.WriteString("\t");
                            streamWrite.WriteStartElement("Power");
                            streamWrite.WriteAttributeString("name", p.PName.Text);
                            streamWrite.WriteString("\r\n");

                            foreach (string[] s in p.attributes)//for each attribute
                            {
                                streamWrite.WriteString("\t\t");
                                streamWrite.WriteStartElement("specific");
                                streamWrite.WriteAttributeString("name", s[0]);
                                streamWrite.WriteString(s[1]);
                                streamWrite.WriteEndElement();
                                streamWrite.WriteString("\r\n");
                            }

                            //Write Checked state
                            streamWrite.WriteString("\t\t");
                            streamWrite.WriteStartElement("specific");
                            streamWrite.WriteAttributeString("name", "Checked");
                            streamWrite.WriteString(p.ChkUsed.Checked.ToString());
                            streamWrite.WriteEndElement();
                            streamWrite.WriteString("\r\n\t");

                            //Close current Power
                            streamWrite.WriteEndElement();
                            streamWrite.WriteString("\r\n");
                        }
                        streamWrite.WriteEndElement();
                        streamWrite.WriteString("\r\n");
                        break;
                    # endregion

                    # region Stat/Skill shared
                    case 9:
                        for (int c = 0; c < tempPan.Controls.Count; c++)
                        {
                            if (tempPan.Controls[c].GetType().ToString() == "System.Windows.Forms.TextBox")
                            {
                                tempTex = (TextBox)tempPan.Controls[c];

                                streamWrite.WriteString("\t");
                                streamWrite.WriteStartElement("Stat");
                                streamWrite.WriteAttributeString("Name", tempTex.Name);
                                streamWrite.WriteAttributeString("Value", tempTex.Text);
                                streamWrite.WriteEndElement();
                                streamWrite.WriteString("\r\n");
                            }
                        }
                        streamWrite.WriteEndElement();
                        streamWrite.WriteString("\r\n");
                        break;
                    # endregion
                }

            }

            streamWrite.WriteEndElement();
            streamWrite.WriteEndDocument();
            streamWrite.Close();

        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenChar = new OpenFileDialog();
            OpenChar.InitialDirectory = @"C:\Documents and Settings\Chris\My Documents\Dropbox\Hobbies\Dungeons & Dragons";
            OpenChar.Filter = "DnD4E files(dnd4e)|*.dnd4e|xml files(*.xml)|*.xml";

            if (OpenChar.ShowDialog() == DialogResult.OK)
            {
                openFiles(OpenChar.FileName);
            }

        }

        private void openFiles(string path)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            formClear();

            UpdateRecentFiles(path);

            # region pass path to correct parser
            if (System.IO.Path.GetExtension(path).ToString() == ".dnd4e")
            {
                loadCharDND(path);
            }
            else if (System.IO.Path.GetExtension(path).ToString() == ".xml")
            {
                loadCharXML(path);
            }
            # endregion

            # region cleanup previous data
            updateLootList();
            updateListbox();
            resetPowers();
            # endregion

            sw.Stop();
            Debug.WriteLine(sw.Elapsed);
            this.Text += " - " + sw.Elapsed.Seconds + ":" + sw.Elapsed.Milliseconds;
        }

        private void loadCharXML(string path)
        {
            XmlTextReader streamRead = new XmlTextReader(path);

            while (streamRead.Read())
            {
                # region DeathThrows
                if (streamRead.Name == "DeathThrows")
                {
                    switch (streamRead.ReadElementString())
                    {
                        case "0":
                            break;

                        case "1":
                            radioButton1.Checked = true;
                            break;

                        case "2":
                            radioButton2.Checked = true;
                            break;

                        case "3":
                            radioButton3.Checked = true;
                            break;
                    }
                }
                # endregion
                # region Second Wind
                if (streamRead.Name == "SecondWind")
                {
                    if (streamRead.ReadElementString() == "True")
                    {
                        SecondWind.Checked = true;
                    }
                }
                # endregion
                if (streamRead.HasAttributes)
                {
                    # region Stats and Skills
                    if (streamRead.Name == "Stat")
                    {
                        loadTextbox(streamRead.GetAttribute(0), streamRead.GetAttribute(1));
                    }
                    # endregion
                    # region Loot
                    if (streamRead.Name == "loot")
                    {
                        Loot newLoot = new Loot();
                        newLoot.Name = streamRead.GetAttribute("Name");
                        newLoot.type = streamRead.GetAttribute("Type");
                        newLoot.quantity = int.Parse(streamRead.GetAttribute("Count"));
                        newLoot.Description = streamRead.GetAttribute("Description");

                        LootList.Add(newLoot);
                    }
                    # endregion
                    # region Power
                    if (streamRead.Name == "Power")
                    {
                        if (streamRead.Name == "Power")
                        {
                            XmlReader inner = streamRead.ReadSubtree();
                            PowerClass newClass = readPower(inner);

                            //newClass.initialise(Powers);
                            PowerList.Add(newClass);
                            inner.Close();
                        }
                    }
                    # endregion
                }
            }

            streamRead.Close();
            updateListbox();

        }

        private void loadCharDND(string fileLocation)
        {
            XmlTextReader streamRead = new XmlTextReader(fileLocation);
            string statV = "";


            while (streamRead.Read())
            {
                # region Details
                switch (streamRead.Name)
                {
                    case "name":
                        textName.Text = streamRead.ReadElementContentAsString();
                        this.Text = textName.Text;
                        break;

                    case "Strength":
                        textAStr.Text = streamRead.GetAttribute(0);
                        break;

                    case "Constitution":
                        textACon.Text = streamRead.GetAttribute(0);
                        break;

                    case "Dexterity":
                        textADex.Text = streamRead.GetAttribute(0);
                        break;

                    case "Intelligence":
                        textAInt.Text = streamRead.GetAttribute(0);
                        break;

                    case "Wisdom":
                        textAWis.Text = streamRead.GetAttribute(0);
                        break;

                    case "Charisma":
                        textACha.Text = streamRead.GetAttribute(0);
                        break;

                }
                # endregion

                # region Stats
                if (streamRead.Name == "Stat")
                {
                    if (streamRead.HasAttributes == true)
                    {
                        statV = streamRead.GetAttribute(0);

                        streamRead.ReadToDescendant("alias");


                        switch (streamRead.GetAttribute(0))
                        {
                            #region get attributes

                            case "Hit Points":
                                textHP.Text = statV;
                                break;

                            case "Healing Surges":
                                textSurges.Text = statV;
                                streamRead.ReadToNextSibling("statadd");
                                streamRead.ReadToNextSibling("statadd");
                                textSurgeV.Text = streamRead.GetAttribute(2);
                                break;

                            case "AC":
                                textAC.Text = statV;
                                break;

                            case "Fortitude Defense":
                                textFort.Text = statV;
                                break;

                            case "Reflex Defense":
                                textRefl.Text = statV;
                                break;

                            case "Will Defense":
                                textWill.Text = statV;
                                break;

                            case "_BaseActionPoints":
                                textActionP.Text = statV;
                                break;

                            case "Initiative":
                                textInit.Text = statV;
                                break;

                            case "Speed":
                                textSpeed.Text = statV;
                                break;

                            case "Passive Perception":
                                textPPerc.Text = statV;
                                break;

                            case "Passive Insight":
                                textPIns.Text = statV;
                                break;

                            #endregion

                            # region get skills

                            case "Acrobatics":
                                textSAcr.Text = statV;
                                break;

                            case "Arcana":
                                textSArc.Text = statV;
                                break;

                            case "Athletics":
                                textSAth.Text = statV;
                                break;

                            case "Bluff":
                                textSBlu.Text = statV;
                                break;

                            case "Diplomacy":
                                textSDip.Text = statV;
                                break;

                            case "Dungeoneering":
                                textSDun.Text = statV;
                                break;

                            case "Endurance":
                                textSEnd.Text = statV;
                                break;

                            case "Heal":
                                textSHea.Text = statV;
                                break;

                            case "History":
                                textSHis.Text = statV;
                                break;

                            case "Insight":
                                textSIns.Text = statV;
                                break;

                            case "Intimidate":
                                textSInt.Text = statV;
                                break;

                            case "Nature":
                                textSNat.Text = statV;
                                break;

                            case "Perception":
                                textSPer.Text = statV;
                                break;

                            case "Religion":
                                textSRel.Text = statV;
                                break;

                            case "Stealth":
                                textSSte.Text = statV;
                                break;

                            case "Streetwise":
                                textSStr.Text = statV;
                                break;

                            case "Thievery":
                                textSThi.Text = statV;
                                break;


                            # endregion
                        }
                    }
                }
                # endregion

                # region Loot
                if (streamRead.Name == "Level")
                {
                    streamRead.Skip();
                }

                if (streamRead.Name == "loot" && streamRead.NodeType != XmlNodeType.EndElement)
                {                    
                    int quantity = int.Parse(streamRead.GetAttribute("count"));

                    Loot newLoot = new Loot();

                    streamRead.ReadToDescendant("RulesElement");

                    newLoot.Name = streamRead.GetAttribute("name");
                    newLoot.type = streamRead.GetAttribute("type");
                    newLoot.quantity = quantity;
                    
                    LootList.Add(newLoot);
                    listBox1.Items.Add(newLoot.Display());

                    while (streamRead.Read() && streamRead.NodeType != XmlNodeType.EndElement)
                    {
                        if (streamRead.Name == "specific")
                        {
                            if (streamRead.GetAttribute("name") == "Full Text")
                            {
                                newLoot.Description = streamRead.ReadElementContentAsString();
                                streamRead.Skip();
                            }
                        }
                        else if (streamRead.NodeType == XmlNodeType.Text)
                        {
                            if (newLoot.Description == "Enter descriptive text here")
                            {
                                newLoot.Description = streamRead.ReadContentAsString();
                            }
                            else if(newLoot.type == "Ritual" )
                            {
                                //newLoot.Description += "\r\n" + streamRead.ReadContentAsString();
                                newLoot.Text = streamRead.ReadContentAsString();

                                newLoot.Description += "\r\n\r\n" + "Text" + "\r\n" + newLoot.Text;
                            }
                        }
                    }
                    
                    //Move to next loot item
                    streamRead.ReadToNextSibling("loot");
                }
                # endregion

                # region Powers
                if (streamRead.Name == "RulesElementTally")
                {
                    streamRead.Skip();
                }

                if (streamRead.Name == "Power")
                {
                    XmlReader inner = streamRead.ReadSubtree();
                    PowerClass newClass = readPower(inner);

                    //newClass.initialise(Powers);
                    PowerList.Add(newClass);
                    inner.Close();
                }
                # endregion

                if (streamRead.Name == "textstring")
                {
                    streamRead.Skip();
                }
            }
        }

        private void loadTextbox(string name, string value)
        {
            foreach (TabPage t in tabControl1.Controls)
            {
                foreach (Panel p in t.Controls)
                {
                    foreach (TextBox c in p.Controls)
                    {
                        if (c.Name == name)
                        {
                            c.Text = value;
                        }
                        if (name == "name")
                        {
                            this.textName.Text = value;
                        }
                    }

                }
            }
        }

        private PowerClass readPower(XmlReader reader)
        {
            PowerClass newClass = new PowerClass();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes)
                {
                    if (reader.Name == "Power")
                    {
                        newClass.PName.Text = reader.GetAttribute(0);
                    }
                    # region switch for attribute value
                    else if (reader.Name == "specific")
                    {
                        string[] item = { reader.GetAttribute(0), reader.ReadElementString().Trim().Replace('\t', '\r'), null };
                        switch (item[0])
                        {
                            # region Skip Section
                            case "Class":
                                break;

                            case "Level":
                                break;

                            case "Power Type":
                                break;
                            # endregion

                            # region Checked
                            case "Checked":
                                if (item[1] == "true")
                                {
                                    newClass.ChkUsed.Checked = true;
                                }
                                else
                                {
                                    newClass.ChkUsed.Checked = false;
                                }
                                break;
                            # endregion

                            # region Power Details
                            case "Power Usage":
                                newClass.PUsage.Text = item[1];
                                break;

                            case "Action Type":
                                newClass.ActionType.Text = item[1];
                                break;

                            case "Attack Type":
                                newClass.AttackType.Text = item[1];
                                break;

                            case "Target":
                                newClass.Target.Text = "Target: " + item[1];
                                break;

                            case "Display":
                                newClass.Display.Text = item[1];
                                break;
                            # endregion

                            default: //generic items
                                if (item[0].StartsWith("_") == false)
                                {
                                    newClass.attributes.Add(item);
                                }
                                break;
                        }
                    }
                    # endregion
                    # region Weapon section
                    else if (reader.Name == "Weapon")
                    {
                        if (newClass.Weapon.Text == "Unarmed" || newClass.Weapon.Text == "")
                        {
                            newClass.Weapon.Text = reader.GetAttribute(0);
                        }
                    }
                    # endregion
                }
            }

            return newClass;
        }

        # endregion

        private int[] PowerPos()
        {
            int[] pos = new int[2];
            int count = new int();

            count = Powers.Controls.Count + 1;

            pos[1] = (int)Math.Ceiling((double)count / 3); //x position
            pos[0] = 3 - ((pos[1] * 3) - count); //y position

            pos[0]--;
            pos[1]--;

            pos[0] = 4 + ((320 + 4) * pos[0]);
            pos[1] = 4 + ((450 + 4) * pos[1]);

            return pos;
        }

        # region Activators
        private void button1_Click(object sender, EventArgs e)
        {
            int i = int.Parse(label39.Text);
            int count = int.Parse(numericUpDown1.Value.ToString());
            if (count > 0)
            {
                LootList[i].Name = textLootName.Text;
                LootList[i].type = textLootType.Text;
                LootList[i].quantity = count;
                LootList[i].Description = textLootFlav.Text;
            }
            else
            {
                LootList[i] = null;
            }

            updateLootList();
            updateListbox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Loot newLoot = new Loot();
            newLoot.Name = textLootName.Text;
            newLoot.type = textLootType.Text;
            newLoot.quantity = int.Parse(numericUpDown1.Value.ToString());
            newLoot.Description = textLootFlav.Text;
            LootList.Add(newLoot);

            updateListbox();
        }

        private void buttonPagePlus_Click(object sender, EventArgs e) //"-" Button
        {
            if (PowerPage > 0)
            {
                PowerPage--;
            }
            Debug.WriteLine("Page {0}", PowerPage);
            updatePowerView();
        }

        private void buttonPageMinus_Click(object sender, EventArgs e) //"+" Button
        {
            if (PowerPage < PowerList.Count / 3)
            {
                PowerPage++;
            }
            Debug.WriteLine("Page {0}", PowerPage);
            updatePowerView();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;

            if (LootList[i] != null)
            {
                textLootName.Text = LootList[i].Name;
                textLootType.Text = LootList[i].type;
                numericUpDown1.Value = LootList[i].quantity;
                textLootFlav.Text = LootList[i].Description.Replace("\r", Environment.NewLine).Trim();
                label39.Text = i.ToString();
            }
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            PowerPage = comboBox1.SelectedIndex / 3;

            updatePowerView();
        }

        private void toolStripMenuRecent_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tSender = (ToolStripMenuItem)sender;
            string tempLoc = "";

            # region switch sender
            switch (tSender.Name)
            {
                case "toolStripMenuItem2":
                    tempLoc = Properties.Settings.Default.RecentFile1;
                    break;

                case "toolStripMenuItem3":
                    tempLoc = Properties.Settings.Default.RecentFile2;
                    break;

                case "toolStripMenuItem4":
                    tempLoc = Properties.Settings.Default.RecentFile3;
                    break;

                case "toolStripMenuItem5":
                    tempLoc = Properties.Settings.Default.RecentFile4;
                    break;

                case "toolStripMenuItem6":
                    tempLoc = Properties.Settings.Default.RecentFile5;
                    break;

                default:
                    Debug.WriteLine(tSender.Name);
                    break;
            }
            # endregion

            if (File.Exists(tempLoc))
            {
                openFiles(tempLoc);
            }

        }

        private void clearRecentFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripMenuItem2.Text = "";
            toolStripMenuItem3.Text = "";
            toolStripMenuItem4.Text = "";
            toolStripMenuItem5.Text = "";
            toolStripMenuItem6.Text = "";

            Settings.Default.RecentFile1 = null;
            Settings.Default.RecentFile2 = null;
            Settings.Default.RecentFile3 = null;
            Settings.Default.RecentFile4 = null;
            Settings.Default.RecentFile5 = null;

            Settings.Default.Save();
        }

        # endregion

        # region updating/clearing methods
        private void formClear()
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            SecondWind.Checked = false;

            for (int i = Powers.Controls.Count - 1; i >= 0; i--)
            {
                Powers.Controls[i].Dispose();
            }
            LootList.Clear();
            updateListbox();
            PowerList.Clear();
        }

        private void updateListbox()
        {
            listBox1.Items.Clear();
            for (int i = 0; i < LootList.Count; i++)
            {
                listBox1.Items.Add(LootList[i].Display());
            }
        }

        private void updateLootList()
        {
            List<Loot> temp = new List<Loot>();

            foreach (Loot l in LootList)
            {
                if (l != null && l.quantity > 0)
                {
                    temp.Add(l);
                }
            }
            LootList = temp;

            IComparer<Loot> comparer = new OrderByName();
            LootList.Sort(comparer);
        }

        private static bool LootEmpty(Loot l)
        {
            return l.quantity <= 0;
        }

        private void updatePowerView()
        {
            Powers.Controls.Clear();

            int p = PowerPage * 3;

            for (int i = 0; i < 3 && p + i < PowerList.Count; i++)
            {
                if (PowerList[p + i].PowerImage == null)
                {
                    PowerList[p + i].initialise(Powers);
                }

                Panel TempPan = new Panel
                {
                    Height = 450,
                    Width = 320,
                    BackgroundImage = PowerList[p + i].PowerImage,
                    Top = 0,
                    Left = ((320 + 4) * i)
                };
                Powers.Controls.Add(TempPan);
                TempPan = PowerList[p + i].PowerContainer;
                TempPan.Show();

                //Powers.Controls.Add(PowerList[p + i].PowerContainer);
                //PowerList[p + i].PowerContainer.Left = i * PowerList[p + i].PowerContainer.Width;
            }
        }

        private void resetPowers()
        {
            IComparer<PowerClass> comparer = new OrderPower();
            PowerList.Sort(comparer);

            comboBox1.Items.Clear();
            foreach (PowerClass p in PowerList)
            {
                comboBox1.Items.Add(p.PName.Text);
            }
            updatePowerView();
        }

        private void UpdateRecentFiles(string path)
        {
            # region setup List<string> MRUs for ease
            List<string> MRUs = new List<string>();

            MRUs.Add(Settings.Default.RecentFile5);
            MRUs.Add(Settings.Default.RecentFile4);
            MRUs.Add(Settings.Default.RecentFile3);
            MRUs.Add(Settings.Default.RecentFile2);
            MRUs.Add(Settings.Default.RecentFile1);

            MRUs.RemoveAll(item => item == "");
            MRUs.RemoveAll(item => item == null);
            # endregion

            //check if the new file is already in the list
            if (MRUs.Contains(path))
            {
                MRUs.Remove(path);
            }

            if (File.Exists(path))
            {
                MRUs.Add(path);
            }

            # region sort MRUs
            for (int i = 0; i < 5; i++)
            {
                int c = MRUs.Count - i;

                # region
                switch (i)
                {
                    case 1:
                        if (c >= 0)
                        {
                            Settings.Default.RecentFile1 = MRUs[c];
                        }
                        break;

                    case 2:
                        if (c >= 0)
                        {
                            Settings.Default.RecentFile2 = MRUs[c];
                        }
                        break;

                    case 3:
                        if (c >= 0)
                        {
                            Settings.Default.RecentFile3 = MRUs[c];
                        }
                        break;

                    case 4:
                        if (c >= 0)
                        {
                            Settings.Default.RecentFile4 = MRUs[c];
                        }
                        break;

                    case 5:
                        if (c >= 0)
                        {
                            Settings.Default.RecentFile5 = MRUs[c];
                        }
                        break;
                }
                # endregion
            }
            # endregion

            Settings.Default.Save();

            # region update MRU menu
            toolStripMenuItem2.Text = Path.GetFileName(Settings.Default.RecentFile1);
            toolStripMenuItem3.Text = Path.GetFileName(Settings.Default.RecentFile2);
            toolStripMenuItem4.Text = Path.GetFileName(Settings.Default.RecentFile3);
            toolStripMenuItem5.Text = Path.GetFileName(Settings.Default.RecentFile4);
            toolStripMenuItem6.Text = Path.GetFileName(Settings.Default.RecentFile5);
            # endregion
        }
        # endregion

        private void ExportPower(PowerClass p)
        {
            Bitmap bmp = new Bitmap(p.PowerContainer.Width, p.PowerContainer.Height);

            p.PowerContainer.DrawToBitmap(bmp, new Rectangle(p.PowerContainer.Location, p.PowerContainer.Size));

            bmp.Save(@"C:\Users\Chris\bmp.bmp");
        }

        private void exportImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Powers.Controls.Count; i++)
            {
                Powers.Controls[i].BackgroundImage.Save(@"C:\Users\Chris\Documents\Image" + i + ".jpg");
            }
        }

    }
}

