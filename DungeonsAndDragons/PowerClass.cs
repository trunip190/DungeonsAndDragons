using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace DungeonsAndDragons
{
    public class PowerClass
    {
        # region declarations
        public Panel PowerContainer = new Panel
        {
            Name = "PowerContainer",
        };
        public List<string[]> attributes = new List<string[]>();
        # region MainCont
        public Panel MainCont = new Panel
            {
                Name = "MainCont",
                Left = 0,
                Top = 48,
                Height = 383,
                Width = 307,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5),
            };
        # endregion
        # region TextCont
        public Panel TextCont = new Panel
            {
                Name = "TextCont",
                Width = 295,
                Left = 5,
                Padding = new Padding(0, 3, 0, 3),
                //Dock = DockStyle.Fill,
                BackgroundImageLayout = ImageLayout.Stretch,
                BackgroundImage = Properties.Resources.BumfContBack
            };
        # endregion
        # region MainText
        RichTextBox MainText = new RichTextBox
        {
            Multiline = true,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None
        };
        # endregion
        # region Textboxes
        public TextBox PName = new TextBox();
        public TextBox PUsage = new TextBox();
        public TextBox Weapon = new TextBox();
        public TextBox ActionType = new TextBox();
        public TextBox AttackType = new TextBox();
        public TextBox Target = new TextBox();
        public TextBox Display = new TextBox();
        # endregion
        public CheckBox ChkUsed = new CheckBox();
        int PWidth = 270;
        bool AltState = false;
        public Color Hstyle = Color.DarkRed;
        Image Icon;
        Image Header;
        public Image PowerImage;
        # endregion

        public void initialise(Panel Parent)
        {
            # region Draw PowerContainer
            # region Panel
            PowerContainer.BorderStyle = BorderStyle.None;
            PowerContainer.BackgroundImageLayout = ImageLayout.Stretch;
            PowerContainer.Width = 320;
            PowerContainer.Height = 450;
            PowerContainer.Show();
            # endregion

            # region attribute switches + positioning
            # region switches
            foreach (string[] s in attributes)
            {
                switch (s[0])
                {
                    # region Flavour
                    case "Flavor":
                        MainText.AppendText(s[1], 2, true, AltState);
                        AltState = !AltState;
                        break;
                    # endregion

                    # region Bottom Section
                    case "Display":
                        Display.Text = s[1];
                        break;
                    # endregion

                    default:
                        s[2] = (MainText.GetLineFromCharIndex(MainText.SelectionStart) * MainText.Font.Height).ToString();
                        MainText.AppendText(s[0] + ": " + s[1], 2, false, AltState);
                        AltState = !AltState;
                        break;
                }
            }
            # endregion

            # region set colours
            switch (PUsage.Text)
            {

                case "At-Will":
                    Hstyle = Color.DarkGreen;
                    Header = Properties.Resources.AtWill;
                    break;

                case "Encounter":
                    Hstyle = Color.DarkRed;
                    Header = Properties.Resources.Encounter;
                    break;

                case "Daily":
                    Hstyle = Color.DimGray;
                    Header = Properties.Resources.Daily;
                    break;

                default:
                    Hstyle = Color.DarkRed;
                    Header = Properties.Resources.Encounter;
                    break;
            }
            # endregion

            # region PName
            if (PName.Text != "")
            {
                PName.Left = 6;
                PName.Top = 4;

                PName.BackColor = Hstyle;
                PName.ForeColor = Color.White;
                PName.Font = new Font(PName.Font.Style.ToString(), 12);
                PName.BorderStyle = BorderStyle.None;
                //PName.Size = TextRenderer.MeasureText(PName.Text, PName.Font);
                PName.Size = getTextSize(PName.Text, PName.Font, PName, false);

                PowerContainer.Controls.Add(PName);
                PName.Show();
            }
            # endregion

            # region PUsage
            if (PUsage.Text != "")
            {
                PUsage.Name = "PUsage";
                PUsage.Left = 6;
                PUsage.Top = assignPos(PowerContainer);
                PUsage.BackColor = Hstyle;
                PUsage.Size = getTextSize(PUsage.Text, PUsage.Font, PUsage, false);
                PowerContainer.Controls.Add(PUsage);

                PUsage.BorderStyle = BorderStyle.None;
                PUsage.ForeColor = Color.White;
                PUsage.Show();
            }
            # endregion

            PowerContainer.Controls.Add(MainCont);
            MainCont.Show();

            # region ActionType
            if (ActionType.Text != "")
            {
                ActionType.Name = "ActionType";
                if (PUsage.Right + 6 < PowerContainer.Width * 1 / 3)
                {
                    ActionType.Left = (PowerContainer.Width * 1 / 3);
                }
                else
                {
                    ActionType.Left = PUsage.Right + 6;
                }
                ActionType.Top = PUsage.Top;
                ActionType.Width = (PowerContainer.Width * 2 / 3) - ActionType.Left - 6;
                ActionType.Height = 20;
                ActionType.BackColor = Hstyle;

                PowerContainer.Controls.Add(ActionType);
                ActionType.BorderStyle = BorderStyle.None;
                ActionType.ForeColor = Color.White;
                ActionType.Show();

            }
            # endregion

            # region Weapon
            if (Weapon.Text != "")
            {
                Weapon.Name = "Weapon";
                Weapon.Left = 6;
                Weapon.Top = 3;
                Weapon.Width = PWidth;
                Weapon.Height = 20;

                MainCont.Controls.Add(Weapon);
                Weapon.BorderStyle = BorderStyle.None;
                Weapon.Show();
            }
            # endregion

            # region AttackType
            if (AttackType.Text != "")
            {
                AttackType.Name = "AttackType";
                AttackType.Left = 6;
                AttackType.Top = assignPos(MainCont);
                AttackType.Multiline = true;

                AttackType.Size = getTextSize(AttackType.Text, AttackType.Font, AttackType, true);

                //AttackType.Size = TextRenderer.MeasureText(AttackType.Text, AttackType.Font);
                //if (AttackType.Right > MainCont.Width / 2)
                //{
                //    AttackType.Width = (MainCont.Width / 2) - AttackType.Left;
                //}

                MainCont.Controls.Add(AttackType);
                AttackType.BorderStyle = BorderStyle.None;
                AttackType.Show();

                if (AttackType.Text.Contains("anged"))
                {
                    Icon = Properties.Resources.Arrow;
                }
                else if (AttackType.Text.Contains("urst"))
                {
                    Icon = Properties.Resources.Burst;
                }
                else if (AttackType.Text.Contains("elee"))
                {
                    Icon = Properties.Resources.sword;
                }
            }
            # endregion

            # region Target
            if (Target.Text != "")
            {
                Target.Name = "Target";

                //Left
                if (AttackType.Right + 6 < (MainCont.Width / 2))
                {
                    Target.Left = MainCont.Width / 2;
                }
                else
                {
                    Target.Left = AttackType.Right + 6;
                }

                Target.Top = AttackType.Top;

                //Width
                Target.Size = getTextSize(Target.Text, Target.Font, Target, true);

                MainCont.Controls.Add(Target);
                Target.BorderStyle = BorderStyle.None;
                Target.Show();
            }
            # endregion

            # region Panels
            TextCont.Top = assignPos(MainCont);
            TextCont.Height = 366 - TextCont.Top;

            MainCont.Controls.Add(TextCont);
            TextCont.Show();
            TextCont.Controls.Add(MainText);
            MainText.Show();
            # endregion
            # endregion

            # region Bottom Section
            # region Display
            if (Display.Text != "")
            {
                Display.Name = "Display";
                Display.Left = 6;
                Display.Top = PowerContainer.Height - 14;
                Display.Width = 127;
                Display.Height = 20;

                PowerContainer.Controls.Add(Display);
                Display.BorderStyle = BorderStyle.None;
                Display.Show();
            }
            # endregion

            # region ChkUsed
            ChkUsed.Height = Display.Height;
            ChkUsed.Width = Display.Height;
            ChkUsed.Top = Display.Top;
            ChkUsed.Left = PowerContainer.Width - Display.Height - 4;
            ChkUsed.CheckStateChanged += new EventHandler(ChkUsed_CheckStateChanged);
            ChkUsed.Show();
            PowerContainer.Controls.Add(ChkUsed);
            # endregion
            # endregion

            PowerContainer.Name = PName.Text;
            PowerContainer.BackgroundImage = DrawBackground();
            # endregion

            # region Draw Card
            # region setup drawing
            Bitmap BMP = new Bitmap(DrawBackground(), PowerContainer.Size);
            Graphics g = Graphics.FromImage(BMP);
            Font FontNormal = new Font(SystemFonts.DefaultFont.Name, 8, FontStyle.Regular);
            Font FontItalic = new Font(SystemFonts.DefaultFont.Name, 8, FontStyle.Italic);
            Size TextRegSize = new Size();
            bool AltDraw = false;
            Brush AltColour = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(MainCont.Width, 0), Color.Silver, Color.Transparent);
            # endregion

            # region draw textboxes
            for (int i = 0; i < PowerContainer.Controls.Count; i++)
            {
                if (PowerContainer.Controls[i].GetType().ToString() == "System.Windows.Forms.TextBox")
                {
                    TextBox t = (TextBox)PowerContainer.Controls[i];

                    g.DrawString(t.Text, t.Font, Brushes.White, t.Location);
                }
            }
            # endregion

            # region setup drawing MainCont
            int[] l = { MainCont.Top + MainCont.Padding.Top, MainCont.Left + MainCont.Padding.Left };
            l[0] += TextCont.Top + TextCont.Padding.Top + 4;
            l[1] += TextCont.Left + TextCont.Padding.Left;
            RectangleF TextRectF = new RectangleF(MainText.Left + l[1], MainText.Top + l[0], MainText.Width, MainText.Height);
            # endregion

            # region draw TextCont + target details
            RectangleF RectTextCont = new RectangleF(TextCont.Left + MainCont.Left, TextCont.Top + MainCont.Top, TextCont.Width, TextCont.Height);
            g.DrawImage(TextCont.BackgroundImage, RectTextCont);

            # region draw Weapon/AttackType/Target
            g.DrawString(Weapon.Text, FontNormal, Brushes.Black, new RectangleF(Weapon.Left + MainCont.Left, Weapon.Top + MainCont.Top, Weapon.Width, Weapon.Height));
            g.DrawString(AttackType.Text, FontNormal, Brushes.Black, new RectangleF(AttackType.Left + MainCont.Left, AttackType.Top + MainCont.Top, AttackType.Width, AttackType.Height));
            g.DrawString(Target.Text, FontNormal, Brushes.Black, new RectangleF(Target.Left + MainCont.Left, Target.Top + MainCont.Top, Target.Width, Target.Height));
            # endregion
            # endregion

            # region draw attributes
            foreach (string[] s in attributes)
            {
                # region Flavor
                if (s[0] == "Flavor")
                {
                    TextRegSize = getTextSize(s[1], FontItalic, MainText);
                    g.DrawString(s[1], FontItalic, Brushes.Black, TextRectF);
                    TextRectF.Height -= TextRegSize.Height;
                    TextRectF.Y += TextRegSize.Height + 6;
                }
                # endregion
                # region Display
                else if (s[0] == "Display")
                {
                    //TextRectF.Y += FontNormal.Height;
                    //TextRectF.Height -= FontNormal.Height;
                }
                # endregion
                # region Normal text
                else
                {
                    if (s[1] != null && s[1] != "")
                    {
                        string[] lines = s[1].Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

                        lines[0] = s[0] + ": " + lines[0];

                        RectangleF BackBox = new RectangleF(MainText.Location, new SizeF(MainText.Width, 0));

                        foreach (string b in lines)
                        {
                            BackBox.Height += getTextSize(b, FontNormal, MainText).Height;
                        }
                        BackBox.Height += FontNormal.Height;

                        # region alternating colour details
                        AltDraw = !AltDraw;
                        if (AltDraw == true)
                        {
                            g.FillRectangle(AltColour, TextRectF.Left, TextRectF.Top, MainText.Width, BackBox.Height);
                        }
                        # endregion


                        RectangleF lineRect = new RectangleF(TextRectF.X, TextRectF.Y, TextRectF.Width, TextRectF.Height);

                        foreach (string t in lines)
                        {
                            if (t.Length > 0)
                            {
                                g.DrawString(t, FontNormal, Brushes.Black, lineRect);
                                lineRect.Y += getTextSize(t, FontNormal, MainText).Height;
                            }
                        }
                        TextRectF.Height -= BackBox.Height;
                        TextRectF.Y += BackBox.Height;
                    }
                }
                # endregion
            }
            # endregion

            # region Draw Display
            g.DrawString(Display.Text, FontNormal, Brushes.Black, new RectangleF(Display.Left/* + MainCont.Left*/, Display.Top/* + MainCont.Top*/, Display.Width, Display.Height));
            # endregion

            # region Dispose of graphics/tools
            AltColour.Dispose();
            g.Dispose();
            # endregion

            PowerImage = BMP;
            # endregion

        }

        public Image DrawBackground()
        {
            Image PBack = Properties.Resources.PowerBack;
            Bitmap BitC = new Bitmap(PBack.Width, PBack.Height); //set image size
            Graphics BackC = Graphics.FromImage(BitC); //open image in graphics editor

            # region Colour Header
            Brush PPen = new SolidBrush(Hstyle); //set colour to paint header
            BackC.FillRectangle(PPen, 0, 0, PBack.Width, PBack.Height); //fill image with colour
            # endregion

            # region Paint Boundaries
            BackC.DrawImage(PBack, 0, 0, PBack.Width, PBack.Height);
            BackC.DrawImage(Header, PBack.Width - Header.Width - 2, 2, Header.Width, Header.Height);
            # endregion

            # region Paint Icon
            if (Icon != null)
            {
                BackC.DrawImage(Icon, PBack.Width - Icon.Width, 3, Icon.Width, Icon.Height);
            }
            # endregion

            BackC.Dispose();

            return BitC;
        }

        private int assignPos(Panel panel)
        {
            int top = 4;
            int count = panel.Controls.Count - 1;

            if (count >= 0)
            {
                top = panel.Controls[count].Bottom + 4;
            }
            else
            {
                top = 4;
            }

            return top;
        }

        void ChkUsed_CheckStateChanged(object sender, EventArgs e)
        {
            if (ChkUsed.CheckState == CheckState.Checked)
            {
                MainCont.Enabled = false;
            }
            else
            {
                MainCont.Enabled = true;
            }
        }

        private Size getTextSize(string s, Font f, Control c)
        {
            Size TextSize = new Size();
            PaintEventArgs e = new PaintEventArgs(c.CreateGraphics(), c.DisplayRectangle);
            string sTemp = s.Replace("\r", "").Replace("\n", "");

            TextSize = e.Graphics.MeasureString(s, f).ToSize();

            while (TextSize.Width > c.Width)
            {
                TextSize.Height += f.Height;
                TextSize.Width -= c.Width;
            }

            return TextSize;
        }

        private Size getTextSize(string s, Font f, Control c, bool b)
        {
            Size TextSize = new Size();
            string sTemp = s.Replace("\r", "").Replace("\n", "");

            TextSize = TextRenderer.MeasureText(s, f);

            if (b)
            {
                c.Width = MainCont.Width / 2;
            }

            while (TextSize.Width > c.Width)
            {
                TextSize.Height += f.Height;
                TextSize.Width -= c.Width;
            }

            TextSize.Width = c.Width; //make sure the width hasn't changed.

            if (b)
            {
                if (c.Right > MainCont.Width)
                {
                    TextSize.Width = (MainCont.Width) - c.Left;
                }
                TextSize.Width -= 6;
            }

            return TextSize;
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, int returns, bool italic, bool alt)
        {
            # region set background colour
            if (alt == true)
            {
                box.SelectionBackColor = Properties.Settings.Default.BackColourAlt;
            }
            else
            {
                box.SelectionBackColor = box.BackColor;
            }
            # endregion

            Font oldFont = box.Font;
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            # region font effects
            if (italic)
            {
                box.SelectionFont = new Font(box.Font, FontStyle.Italic);
            }
            box.AppendText(text);
            while (returns > 0)
            {
                box.AppendText("\r\n");
                returns--;
            }
            box.SelectionFont = oldFont;
            # endregion

            box.SelectionLength = box.TextLength - box.SelectionStart;
        }
    }

    public class Loot
    {
        public string Name = "please give me a name";
        public string type = "Type of item";
        public int quantity = 0;
        public string Description = "Enter descriptive text here";
        public int position = 0; //position in Listbox.
        public string Text = "blank text";

        public string Display()
        {
            return (Name + " x" + quantity);
        }
    }

    public class OrderByName : IComparer<Loot>
    {
        public int Compare(Loot x, Loot y)
        {
            int compareString = x.type.CompareTo(y.type);

            if (compareString == 0)
            {
                compareString = x.Name.CompareTo(y.Name);
            }

            return compareString;
        }
    }

    public class OrderPower : IComparer<PowerClass>
    {
        public int Compare(PowerClass x, PowerClass y)
        {

            # region rank PUsage
            int[] rank = new int[2];

            switch (x.PUsage.Text)
            {
                case "At-Will":
                    rank[0] = 0;
                    break;

                case "Encounter":
                    rank[0] = 1;
                    break;

                case "Daily":
                    rank[0] = 2;
                    break;
            }

            switch (y.PUsage.Text)
            {
                case "At-Will":
                    rank[1] = 0;
                    break;

                case "Encounter":
                    rank[1] = 1;
                    break;

                case "Daily":
                    rank[1] = 2;
                    break;
            }

            # endregion

            int compareString = rank[0].CompareTo(rank[1]);

            if (compareString == 0)
            {
                compareString = x.Display.Text.CompareTo(y.Display.Text);
            }

            if (compareString == 0)
            {
                compareString = x.PName.Text.CompareTo(y.PName.Text);
            }

            return compareString;
        }
    }
}
