/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Portions Copyright ©2007-2017 BoltBait. All Rights Reserved.
// Portions Copyright ©2018 Jason Wendt. All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE CODELAB DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal partial class UIBuilder : Form
    {
        internal string UIControlsText;
        private ColorBgra PC;
        private bool dirty = false;
        private readonly List<UIElement> MasterList = new List<UIElement>();
        private readonly HashSet<string> IDList = new HashSet<string>();
        private string currentID;

        internal UIBuilder(string UserScriptText, ColorBgra PrimaryColor)
        {
            InitializeComponent();

            // PDN Theme
            this.ForeColor = PdnTheme.ForeColor;
            this.BackColor = PdnTheme.BackColor;
            ControlListView.ForeColor = PdnTheme.ForeColor;
            ControlListView.BackColor = PdnTheme.BackColor;
            foreach (Control control in this.Controls)
            {
                if (control is TextBox || control is ComboBox)
                {
                    control.ForeColor = PdnTheme.ForeColor;
                    control.BackColor = PdnTheme.BackColor;
                }
            }


            if (ControlType.ItemHeight < 18)
            {
                ControlType.ItemHeight = 18;
            }
            ControlType.SelectedIndex = 0;

            ControlStyle.ItemHeight = ControlType.ItemHeight;
            ControlStyle.Height = ControlType.Height;
            ControlStyle.SelectedIndex = 0;

            enabledWhenField.DropDownWidth = enabledWhenField.Width * 2;
            enabledWhenField.SelectedIndex = 0;

            enabledWhenCondition.SelectedIndex = 0;

            UpdateEnabledFields();

            SizeF dpi = new SizeF(this.AutoScaleDimensions.Width / 96f, this.AutoScaleDimensions.Height / 96f);
            imgList.ImageSize = new Size((int)Math.Round(16 * dpi.Width), (int)Math.Round(16 * dpi.Height));
            imgList.Images.AddRange(new Image[]
            {
                ResUtil.GetImage("00int"),
                ResUtil.GetImage("01CheckBox"),
                ResUtil.GetImage("02ColorWheel"),
                ResUtil.GetImage("03AngleChooser"),
                ResUtil.GetImage("04PanSlider"),
                ResUtil.GetImage("05TextBox"),
                ResUtil.GetImage("06DoubleSlider"),
                ResUtil.GetImage("07DropDown"),
                ResUtil.GetImage("08BlendOps"),
                ResUtil.GetImage("09Fonts"),
                ResUtil.GetImage("10RadioButton"),
                ResUtil.GetImage("11ReseedButton"),
                ResUtil.GetImage("12MultiTextBox"),
                ResUtil.GetImage("13RollControl"),
                ResUtil.GetImage("14FilenameControl")
            });

            ControlListView.SmallImageList = imgList;

            DefaultColorComboBox.DropDownWidth = DefaultColorComboBox.Width * 2;
            DefaultColorComboBox.Items.Add("None");
            DefaultColorComboBox.Items.Add("PrimaryColor");
            DefaultColorComboBox.Items.Add("SecondaryColor");
            DefaultColorComboBox.Items.AddRange(GetColorNames());

            MasterList = UIElement.ProcessUIControls(UserScriptText);
            foreach (UIElement element in MasterList)
            {
                IDList.Add(element.Identifier);
            }
            refreshListView(0);
            dirty = false;
            PC = PrimaryColor;
        }

        private void refreshListView(int SelectItemIndex)
        {
            ControlListView.Clear();
            enabledWhenField.Items.Clear();

            foreach (UIElement uie in MasterList)
            {
                ControlListView.Items.Add(uie.ToString(), (int)uie.ElementType);
                enabledWhenField.Items.Add(uie.Identifier + " - " + uie.Name);
            }

            if (enabledWhenField.Items.Count > 0)
            {
                enabledWhenField.SelectedIndex = 0;
            }

            if (SelectItemIndex >= 0 && SelectItemIndex < ControlListView.Items.Count)
            {
                ControlListView.Items[SelectItemIndex].Selected = true;
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (dirty)
            {
                Update_Click(null, null);
            }
            UIControlsText = "";
            foreach (UIElement uie in MasterList)
            {
                UIControlsText += uie.ToSourceString();
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            int CurrentItem = (ControlListView.SelectedItems.Count > 0) ? ControlListView.SelectedItems[0].Index : - 1;
            if (CurrentItem > -1)
            {
                IDList.Remove(MasterList[CurrentItem].Identifier);
                MasterList.RemoveAt(CurrentItem);
            }
            if (CurrentItem >= MasterList.Count)
            {
                CurrentItem--;
            }
            refreshListView(CurrentItem);
            dirty = false;
        }

        private void Add_Click(object sender, EventArgs e)
        {
            ElementType elementType = Enum.IsDefined(typeof(ElementType), ControlType.SelectedIndex) ? (ElementType)ControlType.SelectedIndex : ElementType.IntSlider;
            string defaultStr = (elementType == ElementType.ColorWheel) ? DefaultColorComboBox.SelectedItem.ToString() : ControlDef.Text;
            string identifier = ControlID.Text.Trim();
            if (identifier.Length == 0 || IDList.Contains(identifier))
            {
                identifier = "Amount" + (MasterList.Count + 1);
            }
            string enableIndentifer = (this.rbEnabledWhen.Checked) ? MasterList[enabledWhenField.SelectedIndex].Identifier : string.Empty;
            MasterList.Add(new UIElement(elementType, ControlName.Text, ControlMin.Text, ControlMax.Text, defaultStr, OptionsText.Text, ControlStyle.SelectedIndex, rbEnabledWhen.Checked, enableIndentifer, (enabledWhenCondition.SelectedIndex != 0), identifier));
            IDList.Add(identifier);
            refreshListView(MasterList.Count - 1);
            dirty = false;
        }

        private void ControlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dirty = true;
            if (ControlType.Text == "Integer Slider")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = true;
                ControlMax.Enabled = true;
                ControlMin.Enabled = true;
                ControlMax.Text = "100";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = true;
                ControlStyle.Enabled = true;
                FillStyleDropDown(0);
            }
            else if (ControlType.Text == "Check Box")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlMin.Text = "0";
                ControlMin.Enabled = false;
                ControlMax.Text = "1";
                ControlMax.Enabled = false;
                if (int.TryParse(ControlDef.Text, out int result) && result > 0)
                {
                    ControlDef.Text = "1";
                }
                else
                {
                    ControlDef.Text = "0";
                }
                ControlDef.Enabled = true;
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "Color Wheel")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                DefaultColorComboBox.Text = "None";
                DefaultColorComboBox.Visible = true;
                ControlDef.Visible = false;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = false;
                ControlMax.Enabled = false;
                ControlMin.Enabled = false;
                ControlMax.Text = "16777215";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = true;
                ControlStyle.Enabled = true;
                FillStyleDropDown(1);
            }
            else if (ControlType.Text == "Angle Chooser")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = true;
                ControlMax.Enabled = true;
                ControlMin.Enabled = true;
                ControlMax.Text = "180";
                ControlMin.Text = "-180";
                ControlDef.Text = "45";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "Pan Slider")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = false;
                ControlMax.Enabled = false;
                ControlMin.Enabled = false;
                ControlMax.Text = "1";
                ControlMin.Text = "-1";
                ControlDef.Text = "0";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "3D Roll Control")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = false;
                ControlMax.Enabled = false;
                ControlMin.Enabled = false;
                ControlMax.Text = "1";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "String")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = false;
                ControlMax.Enabled = true;
                ControlMin.Enabled = false;
                ControlMax.Text = "255";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "Multi-Line String")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = false;
                ControlMax.Enabled = true;
                ControlMin.Enabled = false;
                ControlMax.Text = "32767";
                ControlMin.Text = "1";
                ControlDef.Text = "1";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "Double Slider")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = true;
                ControlMax.Enabled = true;
                ControlMin.Enabled = true;
                ControlMax.Text = "10";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = true;
                ControlStyle.Enabled = true;
                FillStyleDropDown(0);
            }
            else if (ControlType.Text == "Drop-Down List Box")
            {
                OptionsLabel.Visible = true;
                OptionsText.Visible = true;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = false;
                ControlMax.Visible = false;
                ControlDef.Visible = false;
                MinimumLabel.Visible = false;
                MaximumLabel.Visible = false;
                DefaultLabel.Visible = false;
                ControlDef.Enabled = false;
                ControlMax.Enabled = false;
                ControlMin.Enabled = false;
                ControlMax.Text = "0";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "Radio Button List")
            {
                OptionsLabel.Visible = true;
                OptionsText.Visible = true;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = false;
                ControlMax.Visible = false;
                ControlDef.Visible = false;
                MinimumLabel.Visible = false;
                MaximumLabel.Visible = false;
                DefaultLabel.Visible = false;
                ControlDef.Enabled = false;
                ControlMax.Enabled = false;
                ControlMin.Enabled = false;
                ControlMax.Text = "0";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "BlendOp Types")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = false;
                ControlMax.Enabled = false;
                ControlMin.Enabled = false;
                ControlMax.Text = "0";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "Font Names")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = false;
                ControlMax.Enabled = false;
                ControlMin.Enabled = false;
                ControlMax.Text = "0";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "Reseed Button")
            {
                OptionsLabel.Visible = false;
                OptionsText.Visible = false;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = true;
                ControlMax.Visible = true;
                ControlDef.Visible = true;
                MinimumLabel.Visible = true;
                MaximumLabel.Visible = true;
                DefaultLabel.Visible = true;
                ControlDef.Enabled = false;
                ControlMax.Enabled = false;
                ControlMin.Enabled = false;
                ControlMax.Text = "255";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
            }
            else if (ControlType.Text == "Filename Control")
            {
                OptionsLabel.Visible = true;
                OptionsText.Visible = true;
                DefaultColorComboBox.Visible = false;
                ControlMin.Visible = false;
                ControlMax.Visible = false;
                ControlDef.Visible = false;
                MinimumLabel.Visible = false;
                MaximumLabel.Visible = false;
                DefaultLabel.Visible = false;
                ControlDef.Enabled = false;
                ControlMax.Enabled = false;
                ControlMin.Enabled = false;
                ControlMax.Text = "255";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
                OptionsText.Text = "png|jpg|gif|bmp";
            }
        }

        private void FillStyleDropDown(int Style)
        {
            ControlStyle.Items.Clear();
            switch (Style)
            {
                case 1:
                    ControlStyle.Items.AddRange(new string[] {
                        "Default",
                        "Alpha",
                        "Default no Reset",
                        "Alpha no Reset"
                    });
                    break;
                case 0:
                default:
                    ControlStyle.Items.AddRange(new string[] {
                        "Default",
                        "Hue",
                        "Hue Centered",
                        "Saturation",
                        "White - Black",
                        "Black - White",
                        "Cyan - Red",
                        "Magenta - Green",
                        "Yellow - Blue",
                        "Cyan - Orange",
                        "White - Red",
                        "White - Green",
                        "White - Blue"
                    });
                    break;
            }
            ControlStyle.SelectedIndex = 0;
        }

        private void Update_Click(object sender, EventArgs e)
        {
            int CurrentItem = (ControlListView.SelectedItems.Count > 0) ? ControlListView.SelectedItems[0].Index : -1;

            ElementType elementType = Enum.IsDefined(typeof(ElementType), ControlType.SelectedIndex) ? (ElementType)ControlType.SelectedIndex : ElementType.IntSlider;
            string defaultStr = (elementType == ElementType.ColorWheel) ? DefaultColorComboBox.SelectedItem.ToString() : ControlDef.Text;
            string identifier = !string.IsNullOrWhiteSpace(ControlID.Text) ? ControlID.Text.Trim() : "Amount" + (MasterList.Count + 1);
            string enableIndentifer = (this.rbEnabledWhen.Checked) ? MasterList[enabledWhenField.SelectedIndex].Identifier : string.Empty;
            UIElement uiElement = new UIElement(elementType, ControlName.Text, ControlMin.Text, ControlMax.Text, defaultStr, OptionsText.Text, ControlStyle.SelectedIndex, rbEnabledWhen.Checked, enableIndentifer, (enabledWhenCondition.SelectedIndex != 0), identifier);

            if (CurrentItem > -1)
            {
                MasterList.RemoveAt(CurrentItem);
                MasterList.Insert(CurrentItem, uiElement);
                refreshListView(CurrentItem);
            }
            else
            {
                MasterList.Add(uiElement);
                IDList.Add(identifier);
                refreshListView(MasterList.Count - 1);
            }
            dirty = false;
        }

        private void ControlName_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void MoveUp_Click(object sender, EventArgs e)
        {
            if (ControlListView.SelectedItems.Count == 0)
            {
                return;
            }
            int CurrentItem = ControlListView.SelectedItems[0].Index;
            if (CurrentItem > 0)
            {
                UIElement TargetElement = MasterList[CurrentItem];
                MasterList.RemoveAt(CurrentItem);
                MasterList.Insert(CurrentItem - 1, TargetElement);
                refreshListView(CurrentItem - 1);
            }
        }

        private void MoveDown_Click(object sender, EventArgs e)
        {
            int CurrentItem = (ControlListView.SelectedItems.Count > 0) ? ControlListView.SelectedItems[0].Index : -1;
            if (CurrentItem >= 0 && CurrentItem < MasterList.Count - 1)
            {
                UIElement TargetElement = MasterList[CurrentItem];
                MasterList.RemoveAt(CurrentItem);
                MasterList.Insert(CurrentItem + 1, TargetElement);
                refreshListView(CurrentItem + 1);
            }
        }

        private void ControlListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            dirty = false;
            if (ControlListView.SelectedItems.Count == 0)
            {
                return;
            }

            int CurrentItem = ControlListView.SelectedItems[0].Index;
            if (CurrentItem == -1)
            {
                return;
            }

            UIElement CurrentElement = MasterList[CurrentItem];
            ControlName.Text = CurrentElement.Name;
            if (CurrentElement.EnabledWhen)
            {
                rbEnabled.Checked = false;
                rbEnabledWhen.Checked = true;
                for (int i = 0; i < enabledWhenField.Items.Count; i++)
                {
                    if (enabledWhenField.Items[i].ToString().StartsWith(CurrentElement.EnableIdentifier))
                    {
                        enabledWhenField.SelectedIndex = i;
                        break;
                    }
                }
                enabledWhenCondition.SelectedIndex = (CurrentElement.EnableSwap) ? 1 : 0;
            }
            else
            {
                rbEnabled.Checked = true;
                rbEnabledWhen.Checked = false;
            }

            int BarLoc;
            switch (CurrentElement.ElementType)
            {
                case ElementType.IntSlider:
                    ControlType.Text = "Integer Slider";
                    FillStyleDropDown(0);
                    ControlStyle.SelectedIndex = CurrentElement.Style;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.Checkbox:
                    ControlType.Text = "Check Box";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.ColorWheel:
                    ControlType.Text = "Color Wheel";
                    FillStyleDropDown(1);
                    ControlStyle.SelectedIndex = CurrentElement.Style;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    DefaultColorComboBox.Text = (CurrentElement.ColorDefault.Length == 0 ? "None" : CurrentElement.ColorDefault);
                    break;
                case ElementType.AngleChooser:
                    ControlType.Text = "Angle Chooser";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.dMin.ToString();
                    ControlMax.Text = CurrentElement.dMax.ToString();
                    ControlDef.Text = CurrentElement.dDefault.ToString();
                    break;
                case ElementType.PanSlider:
                    ControlType.Text = "Pan Slider";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.Textbox:
                    ControlType.Text = "String";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.DoubleSlider:
                    ControlType.Text = "Double Slider";
                    FillStyleDropDown(0);
                    ControlStyle.SelectedIndex = CurrentElement.Style;
                    ControlMin.Text = CurrentElement.dMin.ToString();
                    ControlMax.Text = CurrentElement.dMax.ToString();
                    ControlDef.Text = CurrentElement.dDefault.ToString();
                    break;
                case ElementType.DropDown:
                    ControlType.Text = "Drop-Down List Box";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    BarLoc = CurrentElement.Name.IndexOf("|", StringComparison.Ordinal);
                    OptionsText.Text = CurrentElement.Name.Substring(BarLoc + 1);
                    ControlName.Text = CurrentElement.ToShortName();
                    break;
                case ElementType.BinaryPixelOp:
                    ControlType.Text = "BlendOp Types";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.FontFamily:
                    ControlType.Text = "Font Names";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.RadioButtons:
                    ControlType.Text = "Radio Button List";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    BarLoc = CurrentElement.Name.IndexOf("|", StringComparison.Ordinal);
                    OptionsText.Text = CurrentElement.Name.Substring(BarLoc + 1);
                    ControlName.Text = CurrentElement.ToShortName();
                    break;
                case ElementType.ReseedButton:
                    ControlType.Text = "Reseed Button";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.MultiLineTextbox:
                    ControlType.Text = "Multi-Line String";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.RollBall:
                    ControlType.Text = "3D Roll Control";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.Filename:
                    ControlType.Text = "Filename Control";
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    BarLoc = CurrentElement.Name.IndexOf("|", StringComparison.Ordinal);
                    OptionsText.Text = CurrentElement.Name.Substring(BarLoc + 1);
                    ControlName.Text = CurrentElement.ToShortName();
                    break;
                default:
                    break;
            }

            this.currentID = CurrentElement.Identifier;
            ControlID.Text = CurrentElement.Identifier;
        }

        private void DefaultColorComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();
            string colorName = DefaultColorComboBox.Items[e.Index].ToString();

            using (SolidBrush solidBrush = new SolidBrush(e.ForeColor))
            using (Font font = new Font(e.Font, FontStyle.Regular))
            {
                if (colorName == "None" || colorName == "PrimaryColor" || colorName == "SecondaryColor")
                {
                    e.Graphics.DrawString(colorName, font, solidBrush, e.Bounds);
                }
                else
                {
                    solidBrush.Color = Color.FromName(colorName);
                    e.Graphics.FillRectangle(solidBrush, new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Height - 2, e.Bounds.Height - 2));
                    e.Graphics.DrawString(colorName, font, solidBrush, new Rectangle(e.Bounds.X + e.Bounds.Height, e.Bounds.Y + 1, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height));
                }
            }
            e.DrawFocusRectangle();
        }

        private static string[] GetColorNames()
        {
            List<string> names = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                     .Where(prop => prop.PropertyType == typeof(Color) && prop.Name != "Transparent")
                     .Select(prop => prop.Name).ToList();

            names.Sort();

            return names.ToArray();
        }

        private void ControlType_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();
            e.Graphics.DrawImage(ControlListView.SmallImageList.Images[e.Index], e.Bounds.X + ControlType.Margin.Left, e.Bounds.Y + 1, e.Bounds.Height, e.Bounds.Height - 2);
            using (SolidBrush textBrush = new SolidBrush(e.ForeColor))
            using (StringFormat textFormat = new StringFormat { LineAlignment = StringAlignment.Center })
            {
                e.Graphics.DrawString(ControlType.Items[e.Index].ToString(), e.Font, textBrush, new Rectangle(e.Bounds.X + e.Bounds.Height + ControlType.Margin.Left * 2, e.Bounds.Y, e.Bounds.Width - e.Bounds.Height - ControlType.Margin.Left * 2, e.Bounds.Height), textFormat);
            }
            e.DrawFocusRectangle();
        }

        private void rbEnabled_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabledFields();
            dirty = true;
        }

        private void rbEnabledWhen_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabledFields();
            dirty = true;
        }

        private void UpdateEnabledFields()
        {
            if (rbEnabled.Checked)
            {
                enabledWhenField.Enabled = false;
                enabledWhenCondition.Enabled = false;
            }
            else
            {
                enabledWhenField.Enabled = true;
                enabledWhenCondition.Enabled = true;
            }
        }

        private void enabledWhenField_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();
            using (SolidBrush textBrush = new SolidBrush(e.State.HasFlag(DrawItemState.Disabled) ? Color.Gray : e.ForeColor))
            using (StringFormat textFormat = new StringFormat { LineAlignment = StringAlignment.Center })
            {
                e.Graphics.DrawString(enabledWhenField.Items[e.Index].ToString(), e.Font, textBrush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), textFormat);
            }
            e.DrawFocusRectangle();
        }

        private void enabledWhenCondition_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();
            using (SolidBrush textBrush = new SolidBrush(e.State.HasFlag(DrawItemState.Disabled) ? Color.Gray : e.ForeColor))
            using (StringFormat textFormat = new StringFormat { LineAlignment = StringAlignment.Center })
            {
                e.Graphics.DrawString(enabledWhenCondition.Items[e.Index].ToString(), e.Font, textBrush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), textFormat);
            }
            e.DrawFocusRectangle();
        }

        private void ControlStyle_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();
            using (SolidBrush textBrush = new SolidBrush(e.State.HasFlag(DrawItemState.Disabled) ? Color.Gray : e.ForeColor))
            using (StringFormat textFormat = new StringFormat { LineAlignment = StringAlignment.Center })
            {
                e.Graphics.DrawString(ControlStyle.Items[e.Index].ToString(), e.Font, textBrush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), textFormat);
            }
            e.DrawFocusRectangle();
        }

        private void ControlStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            dirty = true;
            if (ControlType.Text == "Color Wheel")
            {
                if (ControlStyle.SelectedIndex == 0 || ControlStyle.SelectedIndex == 2)
                {
                    ControlMax.Text = "‭16777215‬";
                    ControlMin.Text = "0";
                    ControlDef.Text = "0";
                }
                else
                {
                    ControlMax.Text = int.MaxValue.ToString();
                    ControlMin.Text = int.MinValue.ToString();
                    ControlDef.Text = "0";
                }
            }
            else
            {
                if (ControlStyle.SelectedIndex == 1)
                {
                    ControlMax.Text = "360";
                    ControlMin.Text = "0";
                    ControlDef.Text = "0";
                }
                else if (ControlStyle.SelectedIndex == 2)
                {
                    ControlMax.Text = "180";
                    ControlMin.Text = "-180";
                    ControlDef.Text = "0";
                }
                else if (ControlStyle.SelectedIndex >= 6 && ControlStyle.SelectedIndex <= 9)
                {
                    ControlMax.Text = "255";
                    ControlMin.Text = "-255";
                    ControlDef.Text = "0";
                }
                else if (ControlStyle.SelectedIndex != 0)
                {
                    ControlMax.Text = "100";
                    ControlMin.Text = "0";
                    ControlDef.Text = "0";
                }
            }
        }

        private void enabledWhenCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void enabledWhenField_SelectedIndexChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void PreviewButton_Click(object sender, EventArgs e)
        {
            UIControlsText = "";
            foreach (UIElement uie in MasterList)
            {
                UIControlsText += uie.ToSourceString();
            }
            if (!ScriptBuilder.BuildUiPreview(UIControlsText))
            {
                FlexibleMessageBox.Show("Something went wrong, and the Preview can't be displayed.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!ScriptBuilder.UserScriptObject.Options.Flags.HasFlag(EffectFlags.Configurable))
            {
                FlexibleMessageBox.Show("There are no UI controls, so the Preview can't be displayed.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (Surface emptySurface = new Surface(400, 300))
                using (PdnRegion selection = new PdnRegion(emptySurface.Bounds))
                using (EffectEnvironmentParameters enviroParams = new EffectEnvironmentParameters(ColorBgra.Black, Color.White, 0, selection, emptySurface))
                {
                    emptySurface.Clear(ColorBgra.White);
                    ScriptBuilder.UserScriptObject.EnvironmentParameters = enviroParams;
                    ScriptBuilder.UserScriptObject.CreateConfigDialog().ShowDialog();
                }
            }
        }

        private void ControlMax_Leave(object sender, EventArgs e)
        {
            double dMin = 0;
            double dMax = 0;
            double dDef = 0;

            if (!double.TryParse(ControlMax.Text, out dMax)) dMax = 100;
            if (!double.TryParse(ControlMin.Text, out dMin)) dMin = 0;
            if (!double.TryParse(ControlDef.Text, out dDef)) dDef = 0;

            if ((ControlType.Text != "Angle Chooser") && (ControlType.Text != "Double Slider"))
            {
                dMax = Math.Truncate(dMax);
                ControlMax.Text = dMax.ToString();
                dMin = Math.Truncate(dMin);
                ControlMin.Text = dMin.ToString();
                dDef = Math.Truncate(dDef);
                ControlDef.Text = dDef.ToString();
            }

            if (dMax < dMin)
            {
                dMax = dMin;
                ControlMax.Text = dMax.ToString();
            }

            if (dDef > dMax)
            {
                dDef = dMax;
                ControlDef.Text = dDef.ToString();
            }
            dirty = true;
        }

        private void ControlDef_Leave(object sender, EventArgs e)
        {
            double dMin = 0;
            double dMax = 0;
            double dDef = 0;

            if (!double.TryParse(ControlMax.Text, out dMax)) dMax = 100;
            if (!double.TryParse(ControlMin.Text, out dMin)) dMin = 0;
            if (!double.TryParse(ControlDef.Text, out dDef)) dDef = 0;

            if ((ControlType.Text != "Angle Chooser") && (ControlType.Text != "Double Slider"))
            {
                dMax = Math.Truncate(dMax);
                ControlMax.Text = dMax.ToString();
                dMin = Math.Truncate(dMin);
                ControlMin.Text = dMin.ToString();
                dDef = Math.Truncate(dDef);
                ControlDef.Text = dDef.ToString();
            }

            if (dDef < dMin)
            {
                dDef = dMin;
                ControlDef.Text = dDef.ToString();
            }

            if (dDef > dMax)
            {
                dDef = dMax;
                ControlDef.Text = dDef.ToString();
            }
            dirty = true;
        }

        private void ControlMin_Leave(object sender, EventArgs e)
        {
            double dMin = 0;
            double dMax = 0;
            double dDef = 0;

            if (!double.TryParse(ControlMax.Text, out dMax)) dMax = 100;
            if (!double.TryParse(ControlMin.Text, out dMin)) dMin = 0;
            if (!double.TryParse(ControlDef.Text, out dDef)) dDef = 0;

            if ((ControlType.Text != "Angle Chooser") && (ControlType.Text != "Double Slider"))
            {
                dMax = Math.Truncate(dMax);
                ControlMax.Text = dMax.ToString();
                dMin = Math.Truncate(dMin);
                ControlMin.Text = dMin.ToString();
                dDef = Math.Truncate(dDef);
                ControlDef.Text = dDef.ToString();
            }

            if (dMin > dMax)
            {
                dMin = dMax;
                ControlMin.Text = dMin.ToString();
            }

            if (dDef < dMin)
            {
                dDef = dMin;
                ControlDef.Text = dDef.ToString();
            }
            dirty = true;
        }

        private void ControlID_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
            string newID = ControlID.Text.Trim();
            bool error = (newID.Length == 0 || (newID != this.currentID && IDList.Contains(newID)) || !newID.IsCSharpIndentifier());
            ControlID.ForeColor = error ? Color.Black : this.ForeColor;
            ControlID.BackColor = error ? Color.FromArgb(246, 97, 81) : this.BackColor;
        }
    }

    internal class UIElement
    {
        private string Description = "";
        internal string Name = "";
        internal ElementType ElementType = ElementType.IntSlider;
        internal int Min = 0;
        internal int Max = 100;
        internal int Default = 0;
        internal string ColorDefault = "PrimaryColor";
        internal double dMin = 0;
        internal double dMax = 100;
        internal double dDefault = 0;
        internal int Style = 0;
        //   0  Default           Default
        //   1  Hue               Alpha
        //   2  Hue Centered      Default no Reset
        //   3  Saturation        Alpha no Reset
        //   4  White-Black
        //   5  Black-White
        //   6  Cyan-Red
        //   7  Magenta-Green
        //   8  Yellow-Blue
        //   9  Cyan-Orange
        //  10  White-Red
        //  11  White-Green
        //  12  White-Blue
        private const int MaxStyles = 12;
        internal bool EnabledWhen = false;
        internal bool EnableSwap = false;
        internal string EnableIdentifier = string.Empty;
        internal readonly string Identifier = "Amount";

        private static readonly string[] NewSourceCodeType = {
            "IntSliderControl",         // 0
            "CheckboxControl",          // 1
            "ColorWheelControl",        // 2
            "AngleControl",             // 3
            "PanSliderControl",         // 4
            "TextboxControl",           // 5
            "DoubleSliderControl",      // 6
            "ListBoxControl",           // 7
            "BinaryPixelOp",            // 8
            "FontFamily",               // 9
            "RadioButtonControl",       // 10
            "ReseedButtonControl",      // 11
            "MultiLineTextboxControl",  // 12
            "RollControl",              // 13
            "FilenameControl"           // 14
        };

        internal static List<UIElement> ProcessUIControls(string SourceCode)
        {
            string UIControlsText = "";
            Regex REUIControlsContainer = new Regex(@"\#region UICode(?<sublabel>.*?)\#endregion", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mcc = REUIControlsContainer.Match(SourceCode);
            if (mcc.Success)
            {
                // We found the standard #region UICode/#endregion block
                UIControlsText = mcc.Groups["sublabel"].Value.Trim();
            }
            else
            {
                // Find standard UI controls from REALLY OLD scripts
                Regex REAmt1 = new Regex(@"int\s+Amount1\s*=\s*(?<default>\-?\d+)(?<rawcomment>.*)\n", RegexOptions.IgnoreCase);
                Match ma1 = REAmt1.Match(SourceCode);
                if (ma1.Success)
                {
                    UIControlsText = "int Amount1 = " + ma1.Groups["default"].Value.Trim() + ma1.Groups["rawcomment"].Value.Trim();
                    Regex REAmt2 = new Regex(@"int\s+Amount2\s*=\s*(?<default>\-?\d+)(?<rawcomment>.*)\n", RegexOptions.IgnoreCase);
                    Match ma2 = REAmt2.Match(SourceCode);
                    if (ma2.Success)
                    {
                        UIControlsText += "\nint Amount2 = " + ma2.Groups["default"].Value.Trim() + ma2.Groups["rawcomment"].Value.Trim();
                        Regex REAmt3 = new Regex(@"int\s+Amount3\s*=\s*(?<default>\-?\d+)(?<rawcomment>.*)\n", RegexOptions.IgnoreCase);
                        Match ma3 = REAmt3.Match(SourceCode);
                        if (ma3.Success)
                        {
                            UIControlsText += "\nint Amount3 = " + ma3.Groups["default"].Value.Trim() + ma3.Groups["rawcomment"].Value.Trim();
                        }
                    }
                }
                else
                {
                    // No UI controls found
                    UIControlsText = "";
                }
            }
            // process those UI controls
            string[] SrcLines = UIControlsText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<UIElement> UserControls = new List<UIElement>();
            foreach (string s in SrcLines)
            {
                string Line = s.Trim();
                if (Line.Length == 0 || Line.StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }

                UIElement element = UIElement.FromSourceLine(Line);
                if (element != null)
                {
                    UserControls.Add(element);
                }
            }
            return UserControls;
        }

        internal UIElement(ElementType eType, string eName, string eMin, string eMax, string eDefault, string eOptions, int eStyle, bool eEnabled, string targetIdentifier, bool eSwap, string identifier)
        {
            Name = eName;
            ElementType = eType;
            if (!double.TryParse(eMax, out dMax)) dMax = 100;
            if (!double.TryParse(eMin, out dMin)) dMin = 0;
            if (!double.TryParse(eDefault, out dDefault)) dDefault = 0;
            Min = (int)dMin;
            Max = (int)dMax;
            Default = (int)dDefault;
            int NameLength;
            Style = Math.Max(0, Math.Min(MaxStyles, eStyle));
            EnabledWhen = eEnabled;
            EnableIdentifier = targetIdentifier;
            EnableSwap = eSwap;

            string EnabledDescription = "";
            if (EnabledWhen)
            {
                EnabledDescription += " {";
                if (EnableSwap)
                {
                    EnabledDescription += "!";
                }
                EnabledDescription += targetIdentifier + "} ";
            }

            switch (eType)
            {
                case ElementType.IntSlider:
                    Description = eName + " (" + Min.ToString() + ".." + Default.ToString() + ".." + Max.ToString() + ")" + EnabledDescription;
                    break;
                case ElementType.Checkbox:
                    Min = 0;
                    Max = 1;
                    if (Default != 0)
                    {
                        Default = 1;
                    }
                    Description = eName + ((Default == 0) ? " (unchecked)" : " (checked)") + EnabledDescription;
                    Min = 0;
                    Max = 1;
                    break;
                case ElementType.ColorWheel:
                    ColorDefault = (eDefault == "None" ? "" : eDefault);
                    Description = eName;
                    bool alpha = (Style == 1 || Style == 3);
                    Min = alpha ? int.MinValue : 0;
                    Max = alpha ? int.MaxValue : 0xffffff;
                    if (ColorDefault != "")
                    {
                        string alphastyle = alpha ? "?" : "";
                        string resetstyle = (Style == 2 || Style == 3) ? "!" : "";
                        Description += " (" + ColorDefault + alphastyle + resetstyle + ")";
                    }
                    Description += EnabledDescription;
                    break;
                case ElementType.AngleChooser:
                    dMin = dMin.Clamp(-180.0, 360.0);
                    double upperBound = (dMin < 0.0) ? 180.0 : 360;
                    dMax = dMax.Clamp(dMin, upperBound);
                    dDefault = dDefault.Clamp(dMin, dMax);
                    Min = (int)dMin;
                    Max = (int)dMax;
                    Default = (int)dDefault;
                    Description = eName + " (" + dMin.ToString() + ".." + dDefault.ToString() + ".." + dMax.ToString() + ")" + EnabledDescription;
                    break;
                case ElementType.PanSlider:
                    Min = -1;
                    Max = 1;
                    Default = 0;
                    dMin = 0;
                    dMax = 0;
                    Description = eName + EnabledDescription;
                    break;
                case ElementType.Textbox:
                    Min = 0;
                    Default = 0;
                    Description = eName + " (" + Max.ToString() + ")" + EnabledDescription;
                    break;
                case ElementType.DoubleSlider:
                    Description = eName + " (" + dMin.ToString() + ".." + dDefault.ToString() + ".." + dMax.ToString() + ")" + EnabledDescription;
                    break;
                case ElementType.DropDown:
                    Min = 0;
                    Max = 0;
                    Name += "|" + eOptions;
                    int maxDropDown = Name.Split('|').Length - 2;
                    Default = Default.Clamp(0, maxDropDown);
                    NameLength = Name.IndexOf("|", StringComparison.Ordinal);
                    Description = Name.Substring(0, NameLength) + EnabledDescription;
                    break;
                case ElementType.BinaryPixelOp:
                    Min = 0;
                    Max = 0;
                    Default = 0;
                    Description = eName + " (Normal)" + EnabledDescription;
                    break;
                case ElementType.FontFamily:
                    Min = 0;
                    Max = 0;
                    Default = 0;
                    Description = eName + EnabledDescription;
                    break;
                case ElementType.RadioButtons:
                    Min = 0;
                    Max = 0;
                    Name += "|" + eOptions;
                    int maxRadio = Name.Split('|').Length - 2;
                    Default = Default.Clamp(0, maxRadio);
                    NameLength = Name.IndexOf("|", StringComparison.Ordinal);
                    Description = Name.Substring(0, NameLength) + EnabledDescription;
                    break;
                case ElementType.ReseedButton:
                    Min = 0;
                    Max = 255;
                    Default = 0;
                    Description = eName + " (Button)" + EnabledDescription;
                    break;
                case ElementType.MultiLineTextbox:
                    Min = 1;
                    Default = 1;
                    Description = eName + " (" + Max.ToString() + ")" + EnabledDescription;
                    break;
                case ElementType.RollBall:
                    Min = 0;
                    Max = 1;
                    Default = 0;
                    Description = eName + EnabledDescription;
                    break;
                case ElementType.Filename:
                    Min = 0;
                    Max = 0;
                    Default = 0;
                    Name += "|" + eOptions;
                    NameLength = Name.IndexOf("|", StringComparison.Ordinal);
                    if (NameLength == -1) NameLength = Name.Length;
                    Description = Name.Substring(0, NameLength) + EnabledDescription;
                    break;
            }

            Identifier = identifier;
        }

        private static UIElement FromSourceLine(string RawSourceLine)
        {
            Regex REAmt = new Regex(@"\s*(?<type>.*)\s+(?<identifier>.+\b)\s*=\s*(?<default>.*);\s*\/{2}(?<rawcomment>.*)");
            Match m = REAmt.Match(RawSourceLine);
            if (!m.Success)
            {
                // don't understand raw source line
                return null;
            }

            string MinimumStr = "";
            string MaximumStr = "";
            string DefaultColor = "";
            string LabelStr = "";
            string StyleStr = "0";

            Regex REMinMaxStyle = new Regex(@"\s*\[\s*(?<minimum>\-?\d+.*\d*)\s*\,\s*(?<maximum>\-?\d+.*\d*)\s*\,\s*(?<style>\-?\d+.*\d*)\s*\](?<label>.*)");
            Match m0 = REMinMaxStyle.Match(m.Groups["rawcomment"].Value);
            if (m0.Success)
            {
                MinimumStr = m0.Groups["minimum"].Value;
                MaximumStr = m0.Groups["maximum"].Value;
                StyleStr = m0.Groups["style"].Value;
                LabelStr = m0.Groups["label"].Value.Trim();
            }
            else
            {
                Regex REMinMax = new Regex(@"\s*\[\s*(?<minimum>\-?\d+.*\d*)\s*\,\s*(?<maximum>\-?\d+.*\d*)\s*\](?<label>.*)");
                Match m1 = REMinMax.Match(m.Groups["rawcomment"].Value);
                if (m1.Success)
                {
                    MinimumStr = m1.Groups["minimum"].Value;
                    MaximumStr = m1.Groups["maximum"].Value;
                    LabelStr = m1.Groups["label"].Value.Trim();
                }
                else
                {
                    Regex REMaxOnly = new Regex(@"\s*\[\s*(?<maximum>\-?\d+.*\d*)\s*\](?<label>.*)");
                    Match m2 = REMaxOnly.Match(m.Groups["rawcomment"].Value);
                    if (m2.Success)
                    {
                        MinimumStr = "0";
                        MaximumStr = m2.Groups["maximum"].Value;
                        LabelStr = m2.Groups["label"].Value.Trim();
                    }
                    else
                    {
                        Regex REColorOnly = new Regex(@"\s*\[\s*(?<defcolor>.*)\s*\](?<label>.*)");
                        Match m3 = REColorOnly.Match(m.Groups["rawcomment"].Value);
                        if (m3.Success)
                        {
                            DefaultColor = m3.Groups["defcolor"].Value;
                            LabelStr = m3.Groups["label"].Value.Trim();
                        }
                        else
                        {
                            Regex RELabel = new Regex(@"\s*(?<label>.*)");
                            Match m1L = RELabel.Match(m.Groups["rawcomment"].Value);
                            if (m1L.Success)
                            {
                                LabelStr = m1L.Groups["label"].Value.Trim();
                            }
                        }
                    }
                }
            }

            bool enabled = false;
            string targetID = string.Empty;
            bool swap = false;

            Regex REEnabled = new Regex(@"\s*{(?<swap>\!?)(?<identifier>.+\b)\s*}\s*(?<label>.*)");
            Match me = REEnabled.Match(LabelStr);
            if (me.Success)
            {
                LabelStr = me.Groups["label"].Value.Trim();

                enabled = true;
                targetID = me.Groups["identifier"].Value;
                swap = me.Groups["swap"].Value.Trim() == "!";
            }

            string DefaultStr = m.Groups["default"].Value.Trim();
            string TypeStr = m.Groups["type"].Value.Trim();
            ElementType elementType = ElementType.IntSlider;
            if (TypeStr == "IntSliderControl")
            {
                elementType = ElementType.IntSlider;
            }
            else if (TypeStr == "CheckboxControl")
            {
                elementType = ElementType.Checkbox;
            }
            else if (TypeStr == "ColorWheelControl")
            {
                elementType = ElementType.ColorWheel;
            }
            else if (TypeStr == "AngleControl")
            {
                elementType = ElementType.AngleChooser;
            }
            else if (TypeStr == "DoubleSliderControl")
            {
                elementType = ElementType.DoubleSlider;
            }
            else if (TypeStr == "PanSliderControl")
            {
                elementType = ElementType.PanSlider;
            }
            else if (TypeStr == "TextboxControl")
            {
                elementType = ElementType.Textbox;
            }
            else if (TypeStr == "MultiLineTextboxControl")
            {
                elementType = ElementType.MultiLineTextbox;
            }
            else if (TypeStr == "ReseedButtonControl")
            {
                elementType = ElementType.ReseedButton;
            }
            else if (TypeStr == "ListBoxControl")
            {
                elementType = ElementType.DropDown;
            }
            else if (TypeStr == "RadioButtonControl")
            {
                elementType = ElementType.RadioButtons;
            }
            else if (TypeStr == "UserBlendOp" || TypeStr == "BinaryPixelOp")
            {
                elementType = ElementType.BinaryPixelOp;
            }
            else if (TypeStr == "FontFamily")
            {
                elementType = ElementType.FontFamily;
            }
            else if (TypeStr == "RollControl")
            {
                elementType = ElementType.RollBall;
            }
            else if (TypeStr == "FilenameControl")
            {
                elementType = ElementType.Filename;
            }
            #region Detections for legacy scripts
            else if (TypeStr == "bool")
            {
                elementType = ElementType.Checkbox;
            }
            else if (TypeStr == "int")
            {
                elementType = ElementType.IntSlider;
            }
            else if (TypeStr == "ColorBgra")
            {
                elementType = ElementType.ColorWheel;
            }
            else if (TypeStr == "string")
            {
                if (!int.TryParse(MinimumStr, out int min)) min = 0;
                elementType = (min > 0) ? ElementType.MultiLineTextbox : ElementType.Textbox;
            }
            else if (TypeStr == "byte")
            {
                if (!LabelStr.Contains("|") || (MaximumStr == "255"))
                {
                    elementType = ElementType.ReseedButton;
                }
                else if (MaximumStr.Length == 0)
                {
                    elementType = ElementType.DropDown;
                }
                else if (MaximumStr == "1")
                {
                    elementType = ElementType.RadioButtons;
                }
            }
            else if (TypeStr == "Pair<double, double>")
            {
                elementType = ElementType.PanSlider;
            }
            else if (TypeStr == "Tuple<double, double, double>")
            {
                elementType = ElementType.RollBall;
            }
            else if (TypeStr == "double")
            {
                double fMax, fMin, fDefault;
                if (!double.TryParse(MaximumStr, NumberStyles.Float, CultureInfo.InvariantCulture, out fMax)) fMax = 10;
                if (!double.TryParse(MinimumStr, NumberStyles.Float, CultureInfo.InvariantCulture, out fMin)) fMin = 0;
                if (!double.TryParse(DefaultStr, NumberStyles.Float, CultureInfo.InvariantCulture, out fDefault)) fDefault = 0;

                int iMax, iMin, iDefault;
                if (!int.TryParse(MaximumStr, out iMax)) iMax = 180;
                if (!int.TryParse(MinimumStr, out iMin)) iMin = -180;
                if (!int.TryParse(DefaultStr, out iDefault)) iDefault = 45;

                elementType = ((iMin == -180) && (fMin == -180) && (iMax == 180) && (fMax == 180) && (iDefault == 45) && (fDefault == 45)) ?
                    ElementType.AngleChooser :
                    ElementType.DoubleSlider;
            }
            #endregion
            else
            {
                return null;
            }

            if (!int.TryParse(StyleStr, out int style))
            {
                style = 0;
            }

            if (!double.TryParse(MaximumStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double dMax))
            {
                dMax = 10;
            }

            if (!double.TryParse(MinimumStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double dMin))
            {
                dMin = 0;
            }

            string defaultValue;
            if (elementType == ElementType.ColorWheel)
            {
                if (DefaultColor.EndsWith("?!", StringComparison.Ordinal)) // Alpha - No Reset
                {
                    defaultValue = DefaultColor.Substring(0, DefaultColor.Length - 2);
                    style = 3;
                }
                else if (DefaultColor.EndsWith("?", StringComparison.Ordinal)) // Alpha - Reset
                {
                    defaultValue = DefaultColor.Substring(0, DefaultColor.Length - 1);
                    style = 1;
                }
                else if (DefaultColor.EndsWith("!", StringComparison.Ordinal)) // No Alpha - No Reset
                {
                    defaultValue = DefaultColor.Substring(0, DefaultColor.Length - 1);
                    style = 2;
                }
                else // No Alpha - Reset
                {
                    defaultValue = DefaultColor;
                    style = 0;
                }
            }
            else
            {
                if (!double.TryParse(DefaultStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double dDefault))
                {
                    dDefault = 0;
                }
                defaultValue = dDefault.ToString();
            }

            string id = m.Groups["identifier"].Value;

            int pipeIndex = LabelStr.IndexOf('|');
            string name = (pipeIndex > -1) ? LabelStr.Substring(0, pipeIndex) : LabelStr;
            string options = (pipeIndex > -1 && pipeIndex < LabelStr.Length) ?
                LabelStr.Substring(pipeIndex + 1, LabelStr.Length - pipeIndex - 1) :
                string.Empty;

            return new UIElement(elementType, name, dMin.ToString(), dMax.ToString(), defaultValue, options, style, enabled, targetID, swap, id);
        }

        public override string ToString()
        {
            return Identifier + " — " + Description;
        }

        internal string ToSourceString()
        {
            string SourceCode = NewSourceCodeType[(int)ElementType] + " " + Identifier;
            switch (ElementType)
            {
                case ElementType.IntSlider:
                    SourceCode += " = " + Default.ToString();
                    SourceCode += "; // [" + Min.ToString() + "," + Max.ToString();
                    if (Style > 0)
                    {
                        SourceCode += "," + Style.ToString();
                    }
                    SourceCode += "] ";
                    break;
                case ElementType.AngleChooser:
                case ElementType.DoubleSlider:
                    SourceCode += " = " + dDefault.ToString(CultureInfo.InvariantCulture);
                    SourceCode += "; // [" + dMin.ToString(CultureInfo.InvariantCulture) + "," + dMax.ToString(CultureInfo.InvariantCulture);
                    if (Style > 0)
                    {
                        SourceCode += "," + Style.ToString();
                    }
                    SourceCode += "] ";
                    break;
                case ElementType.Checkbox:
                    SourceCode += " = " + ((Default == 0) ? "false" : "true");
                    SourceCode += "; // [" + Min.ToString() + "," + Max.ToString() + "] ";
                    break;
                case ElementType.ColorWheel:
                    Color c;
                    if (ColorDefault.Length == 0 || ColorDefault == "PrimaryColor")
                    {
                        c = Color.Black;
                    }
                    else if (ColorDefault == "SecondaryColor")
                    {
                        c = Color.White;
                    }
                    else
                    {
                        c = Color.FromName(ColorDefault);
                    }
                    string alphastyle = "";
                    string resetstyle = "";
                    if (Style == 1 || Style == 3)
                    {
                        alphastyle = "?";
                    }
                    if (Style == 2 || Style == 3)
                    {
                        resetstyle = "!";
                    }
                    if (alphastyle == "?")
                    {
                        SourceCode += " = ColorBgra.FromBgra(" + c.B.ToString() + "," + c.G.ToString() + "," + c.R.ToString() + ",255)";
                    }
                    else
                    {
                        SourceCode += " = ColorBgra.FromBgr(" + c.B.ToString() + "," + c.G.ToString() + "," + c.R.ToString() + ")";
                    }
                    SourceCode += "; // ";
                    if (ColorDefault.Trim() + alphastyle + resetstyle != "")
                    {
                        SourceCode += "[" + ColorDefault.Trim() + alphastyle + resetstyle + "] ";
                    }
                    break;
                case ElementType.PanSlider:
                    SourceCode += " = Pair.Create(";
                    SourceCode += dMin.ToString("F3", CultureInfo.InvariantCulture);
                    SourceCode += ",";
                    SourceCode += dMax.ToString("F3", CultureInfo.InvariantCulture);
                    SourceCode += "); // ";
                    break;
                case ElementType.Textbox:
                case ElementType.MultiLineTextbox:
                    SourceCode += " = \"\"";
                    SourceCode += "; // [" + Min.ToString() + "," + Max.ToString() + "] ";
                    break;
                case ElementType.DropDown:
                    SourceCode += " = " + Default.ToString() + "; // ";
                    break;
                case ElementType.BinaryPixelOp:
                    SourceCode += " = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal); // ";
                    break;
                case ElementType.FontFamily:
                    SourceCode += " = new FontFamily(\"Arial\"); // ";
                    break;
                case ElementType.RadioButtons:
                    SourceCode += " = " + Default.ToString() + "; // [1] ";
                    break;
                case ElementType.ReseedButton:
                    SourceCode += " = 0; // [255] ";
                    break;
                case ElementType.RollBall:
                    SourceCode += " = Tuple.Create<double, double, double>( 0.0 , 0.0 , 0.0 )";
                    SourceCode += "; // ";
                    break;
                case ElementType.Filename:
                    SourceCode += " = @\"\"; // ";
                    break;
                default:
                    break;
            }

            if (EnabledWhen)
            {
                SourceCode += "{";
                if (EnableSwap)
                {
                    SourceCode += "!";
                }
                SourceCode += EnableIdentifier + "} ";
            }
            SourceCode += Name + "\r\n";

            return SourceCode;
        }

        internal string[] ToOptionArray()
        {
            if ((ElementType == ElementType.DropDown) || (ElementType == ElementType.RadioButtons))
            {
                int BarLoc = Name.IndexOf("|", StringComparison.Ordinal);
                if (BarLoc == -1) return null;
                string Options = Name.Substring(BarLoc + 1);
                return Options.Split('|');
            }
            return null;
        }

        internal string ToShortName()
        {
            if ((ElementType == ElementType.DropDown) || (ElementType == ElementType.RadioButtons) || (ElementType == ElementType.Filename))
            {
                int BarLoc = Name.IndexOf("|", StringComparison.Ordinal);
                if (BarLoc == -1) return Name;
                return Name.Substring(0, BarLoc);
            }
            return Name;
        }

        internal string ToAllowableFileTypes()
        {
            int BarLoc = Name.IndexOf("|", StringComparison.Ordinal);
            if (BarLoc == -1) return null;
            string Options = Name.Substring(BarLoc + 1);
            string[] filetypes = Options.Split('|');
            for (int i=0; i < filetypes.Length ; i++)
            {
                filetypes[i] = "\"" + filetypes[i] + "\"";
            }
            return filetypes.Join(",");
        }
    }

    internal enum ElementType
    {
        IntSlider,
        Checkbox,
        ColorWheel,
        AngleChooser,
        PanSlider,
        Textbox,
        DoubleSlider,
        DropDown,
        BinaryPixelOp,
        FontFamily,
        RadioButtons,
        ReseedButton,
        MultiLineTextbox,
        RollBall,
        Filename
    }
}
