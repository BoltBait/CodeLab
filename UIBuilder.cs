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
// Latest distribution: http://www.BoltBait.com/pdn/codelab
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
        private string ScriptText;
        internal string UIControlsText;
        private ColorBgra PC;
        private bool dirty = false;
        private List<UIElement> MasterList = new List<UIElement>();

        internal UIBuilder(string UserScriptText, ColorBgra PrimaryColor)
        {
            InitializeComponent();

            SizeF dpi;
            using (Graphics g = this.CreateGraphics())
                dpi = new SizeF(g.DpiX / 96f, g.DpiY / 96f);

            imgList.ImageSize = new Size((int)Math.Round(16 * dpi.Width), (int)Math.Round(16 * dpi.Height));
            ControlStyle.ItemHeight = ControlType.ItemHeight;
            ControlStyle.Height = ControlType.Height;
            DefaultColorComboBox.DropDownWidth = DefaultColorComboBox.Width * 2;
            enabledWhenField.DropDownWidth = enabledWhenField.Width * 2;

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

            ScriptText = UserScriptText;
            PC = PrimaryColor;
            enabledWhenCondition.SelectedIndex = 0;
            enabledWhenField.SelectedIndex = 0;
            ControlStyle.SelectedIndex = 0;
            UpdateEnabledFields();
        }

        private void UIBuilder_Load(object sender, EventArgs e)
        {
            if (ControlType.ItemHeight < 18)
            {
                ControlType.ItemHeight = 18;
            }

            ControlListView.View = View.List;
            ControlListView.Items.Clear();
            imgList.Images.Add("00", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.00int.png")));
            imgList.Images.Add("01", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.01CheckBox.png")));
            imgList.Images.Add("02", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.02ColorWheel.png")));
            imgList.Images.Add("03", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.03AngleChooser.png")));
            imgList.Images.Add("04", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.04PanSlider.png")));
            imgList.Images.Add("05", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.05TextBox.png")));
            imgList.Images.Add("06", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.06DoubleSlider.png")));
            imgList.Images.Add("07", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.07DropDown.png")));
            imgList.Images.Add("08", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.08BlendOps.png")));
            imgList.Images.Add("09", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.09Fonts.png")));
            imgList.Images.Add("10", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.10RadioButton.png")));
            imgList.Images.Add("11", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.11ReseedButton.png")));
            imgList.Images.Add("12", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.12MultiTextBox.png")));
            imgList.Images.Add("13", Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.13RollControl.png")));
            ControlListView.SmallImageList = imgList;

            DefaultColorComboBox.Items.Add("None");
            DefaultColorComboBox.Items.Add("PrimaryColor");
            DefaultColorComboBox.Items.Add("SecondaryColor");
            foreach (string c in GetColorNames())
            {
                DefaultColorComboBox.Items.Add(c);
            }

            ControlType.SelectedIndex = 0;
            MasterList.Clear();
            MasterList = ScriptWriter.ProcessUIControls(ScriptText);
            refreshListView(0);
            dirty = false;
        }

        private void refreshListView(int SelectItemIndex)
        {
            ControlListView.Clear();
            enabledWhenField.Items.Clear();
            int Count = 0;
            foreach (UIElement uie in MasterList)
            {
                ControlListView.Items.Add(uie.ToString(), (int)uie.ElementType);
                Count++;
                enabledWhenField.Items.Add("Amount" + Count.ToString() + " - " + uie.Name);
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
            int ElementCount = 0;
            foreach (UIElement uie in MasterList)
            {
                ElementCount++;
                UIControlsText += uie.ToSourceString(ElementCount, PC);
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            int CurrentItem = -1;
            if (ControlListView.SelectedItems.Count > 0)
            {
                CurrentItem = ControlListView.SelectedItems[0].Index;
            }

            if (CurrentItem > -1)
            {
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
            ElementType elementType = (Enum.IsDefined(typeof(ElementType), ControlType.SelectedIndex)) ? (ElementType)ControlType.SelectedIndex : ElementType.IntSlider;
            if (ControlType.SelectedIndex == 2)
            {
                MasterList.Add(new UIElement(elementType, ControlName.Text, ControlMin.Text, ControlMax.Text, DefaultColorComboBox.SelectedItem.ToString(), OptionsText.Text, ControlStyle.SelectedIndex, rbEnabledWhen.Checked, enabledWhenField.SelectedIndex, (enabledWhenCondition.SelectedIndex != 0)));
            }
            else
            {
                MasterList.Add(new UIElement(elementType, ControlName.Text, ControlMin.Text, ControlMax.Text, ControlDef.Text, OptionsText.Text, ControlStyle.SelectedIndex, rbEnabledWhen.Checked, enabledWhenField.SelectedIndex, (enabledWhenCondition.SelectedIndex != 0)));
            }
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
                ControlStyle.SelectedIndex = 0;
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
                StyleLabel.Enabled = false;
                ControlStyle.Enabled = false;
                ControlStyle.SelectedIndex = 0;
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
                ControlMax.Enabled = false;
                ControlMin.Enabled = false;
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
                ControlStyle.SelectedIndex = 0;
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
        }

        private void Update_Click(object sender, EventArgs e)
        {
            int CurrentItem = -1;
            if (ControlListView.SelectedItems.Count > 0)
            {
                CurrentItem = ControlListView.SelectedItems[0].Index;
            }
            ElementType elementType = (Enum.IsDefined(typeof(ElementType), ControlType.SelectedIndex)) ? (ElementType)ControlType.SelectedIndex : ElementType.IntSlider;
            if (CurrentItem > -1)
            {
                MasterList.RemoveAt(CurrentItem);
                if (ControlType.SelectedIndex == 2)
                {
                    MasterList.Insert(CurrentItem, new UIElement(elementType, ControlName.Text, ControlMin.Text, ControlMax.Text, DefaultColorComboBox.SelectedItem.ToString(), OptionsText.Text, ControlStyle.SelectedIndex, rbEnabledWhen.Checked, enabledWhenField.SelectedIndex, (enabledWhenCondition.SelectedIndex != 0)));
                }
                else
                {
                    MasterList.Insert(CurrentItem, new UIElement(elementType, ControlName.Text, ControlMin.Text, ControlMax.Text, ControlDef.Text, OptionsText.Text, ControlStyle.SelectedIndex, rbEnabledWhen.Checked, enabledWhenField.SelectedIndex, (enabledWhenCondition.SelectedIndex != 0)));
                }
                refreshListView(CurrentItem);
            }
            else
            {
                if (ControlType.SelectedIndex == 2)
                {
                    MasterList.Add(new UIElement(elementType, ControlName.Text, ControlMin.Text, ControlMax.Text, DefaultColorComboBox.SelectedItem.ToString(), OptionsText.Text, ControlStyle.SelectedIndex, rbEnabledWhen.Checked, enabledWhenField.SelectedIndex, (enabledWhenCondition.SelectedIndex != 0)));
                }
                else
                {
                    MasterList.Add(new UIElement(elementType, ControlName.Text, ControlMin.Text, ControlMax.Text, ControlDef.Text, OptionsText.Text, ControlStyle.SelectedIndex, rbEnabledWhen.Checked, enabledWhenField.SelectedIndex, (enabledWhenCondition.SelectedIndex != 0)));
                }
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
            int CurrentItem = -1;
            if (ControlListView.SelectedItems.Count > 0)
            {
                CurrentItem = ControlListView.SelectedItems[0].Index;
            }
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
            int CurrentItem = -1;
            if (ControlListView.SelectedItems.Count > 0)
            {
                CurrentItem = ControlListView.SelectedItems[0].Index;
            }
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
            UIElement CurrentElement;
            int BarLoc;
            int CurrentItem = -1;
            if (ControlListView.SelectedItems.Count > 0)
            {
                CurrentItem = ControlListView.SelectedItems[0].Index;
            }
            if (CurrentItem > -1)
            {
                CurrentElement = MasterList[CurrentItem];
                ControlName.Text = CurrentElement.Name;
                if (CurrentElement.EnabledWhen)
                {
                    rbEnabled.Checked = false;
                    rbEnabledWhen.Checked = true;
                    enabledWhenField.SelectedIndex = (CurrentElement.EnableLinkVar > enabledWhenField.Items.Count) ? 0 : CurrentElement.EnableLinkVar;
                    enabledWhenCondition.SelectedIndex = (CurrentElement.EnableSwap) ? 1 : 0;
                }
                else
                {
                    rbEnabled.Checked = true;
                    rbEnabledWhen.Checked = false;
                }
                switch (CurrentElement.ElementType)
                {
                    case ElementType.IntSlider:
                        ControlType.Text = "Integer Slider";
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
                        ControlStyle.SelectedIndex = 0;
                        ControlMin.Text = CurrentElement.Min.ToString();
                        ControlMax.Text = CurrentElement.Max.ToString();
                        ControlDef.Text = CurrentElement.Default.ToString();
                        DefaultColorComboBox.Text = (CurrentElement.ColorDefault == "" ? "None" : CurrentElement.ColorDefault);
                        break;
                    case ElementType.AngleChooser:
                        ControlType.Text = "Angle Chooser";
                        ControlStyle.SelectedIndex = 0;
                        ControlMin.Text = CurrentElement.Min.ToString();
                        ControlMax.Text = CurrentElement.Max.ToString();
                        ControlDef.Text = CurrentElement.Default.ToString();
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
                    default:
                        break;
                }
            }
            dirty = false;
        }

        private void DefaultColorComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (DefaultColorComboBox.Items[e.Index].ToString() == "None" || DefaultColorComboBox.Items[e.Index].ToString() == "PrimaryColor" || DefaultColorComboBox.Items[e.Index].ToString() == "SecondaryColor")
            {
                if (e.Index != -1) e.Graphics.DrawString(DefaultColorComboBox.Items[e.Index].ToString(), new Font(e.Font, FontStyle.Regular), new SolidBrush(e.ForeColor), e.Bounds);
            }
            else
            {
                Color itemColor = Color.FromName(DefaultColorComboBox.Items[e.Index].ToString());
                e.Graphics.FillRectangle(new SolidBrush(itemColor), new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Height - 2, e.Bounds.Height - 2));
                if (e.Index != -1) e.Graphics.DrawString(DefaultColorComboBox.Items[e.Index].ToString(), new Font(e.Font, FontStyle.Bold), new SolidBrush(itemColor), new Rectangle(e.Bounds.X + e.Bounds.Height, e.Bounds.Y + 1, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height));
            }
            e.DrawFocusRectangle();
        }

        private static List<string> GetColorNames()
        {
            List<string> names;

            names = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                     .Where(prop => prop.PropertyType == typeof(Color) && prop.Name != "Transparent")
                     .Select(prop => prop.Name).ToList();

            names.Sort();

            return names;
        }

        private void ControlType_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            ComboBox myBox = (ComboBox)sender;
            //int imgOffset = (int)Math.Round((e.Bounds.Height - ControlListView.SmallImageList.ImageSize.Height) / 2f);
            e.Graphics.DrawImage(ControlListView.SmallImageList.Images[e.Index], e.Bounds.X + myBox.Margin.Left, e.Bounds.Y + 1, e.Bounds.Height, e.Bounds.Height - 2);
            using (SolidBrush textBrush = new SolidBrush(e.ForeColor))
            using (StringFormat textFormat = new StringFormat { LineAlignment = StringAlignment.Center })
                if (e.Index != -1) e.Graphics.DrawString(ControlType.Items[e.Index].ToString(), e.Font, textBrush, new Rectangle(e.Bounds.X + e.Bounds.Height + myBox.Margin.Left * 2, e.Bounds.Y, e.Bounds.Width - e.Bounds.Height - myBox.Margin.Left * 2, e.Bounds.Height), textFormat);
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
            e.DrawBackground();
            using (SolidBrush textBrush = new SolidBrush((((int)e.State & (int)DrawItemState.Disabled) > 0) ? Color.Gray : e.ForeColor))
            using (StringFormat textFormat = new StringFormat { LineAlignment = StringAlignment.Center })
                if (e.Index != -1) e.Graphics.DrawString(enabledWhenField.Items[e.Index].ToString(), e.Font, textBrush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), textFormat);
            e.DrawFocusRectangle();
        }

        private void enabledWhenCondition_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            using (SolidBrush textBrush = new SolidBrush((((int)e.State & (int)DrawItemState.Disabled) > 0) ? Color.Gray : e.ForeColor))
            using (StringFormat textFormat = new StringFormat { LineAlignment = StringAlignment.Center })
                if (e.Index != -1) e.Graphics.DrawString(enabledWhenCondition.Items[e.Index].ToString(), e.Font, textBrush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), textFormat);
            e.DrawFocusRectangle();
        }

        private void ControlStyle_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            using (SolidBrush textBrush = new SolidBrush((((int)e.State & (int)DrawItemState.Disabled) > 0) ? Color.Gray : e.ForeColor))
            using (StringFormat textFormat = new StringFormat { LineAlignment = StringAlignment.Center })
                if (e.Index != -1) e.Graphics.DrawString(ControlStyle.Items[e.Index].ToString(), e.Font, textBrush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), textFormat);
            e.DrawFocusRectangle();
        }

        private void ControlStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            dirty = true;
            if (ControlStyle.SelectedIndex != 0)
            {
                ControlMax.Text = "100";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
            }
            if (ControlStyle.SelectedIndex == 1)
            {
                ControlMax.Text = "360";
                ControlMin.Text = "0";
                ControlDef.Text = "0";
            }
            if (ControlStyle.SelectedIndex == 2)
            {
                ControlMax.Text = "180";
                ControlMin.Text = "-180";
                ControlDef.Text = "0";
            }
            if (ControlStyle.SelectedIndex >= 6 && ControlStyle.SelectedIndex <= 9)
            {
                ControlMax.Text = "255";
                ControlMin.Text = "-255";
                ControlDef.Text = "0";
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
            int ElementCount = 0;
            foreach (UIElement uie in MasterList)
            {
                ElementCount++;
                UIControlsText += uie.ToSourceString(ElementCount, PC);
            }
            if (!ScriptBuilder.BuildUiPreview(UIControlsText))
            {
                MessageBox.Show("Something went wrong, and the Preview can't be displayed.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!ScriptBuilder.UserScriptObject.CheckForEffectFlags(EffectFlags.Configurable))
            {
                MessageBox.Show("There are no UI controls, so the Preview can't be displayed.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (("Angle Chooser" != ControlType.Text) && ("Double Slider" != ControlType.Text))
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

            if (("Angle Chooser" != ControlType.Text) && ("Double Slider" != ControlType.Text))
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

            if (("Angle Chooser" != ControlType.Text) && ("Double Slider" != ControlType.Text))
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
        //   0  Default
        //   1  Hue
        //   2  Hue Centered
        //   3  Saturation
        //   4  White-Black
        //   5  Black-White
        //   6  Cyan-Red
        //   7  Magenta-Green
        //   8  Yellow-Blue
        //   9  Cyan-Orange
        //  10  White-Red
        //  11  White-Green
        //  12  White-Blue
        internal int MaxStyles = 12;
        internal bool EnabledWhen = false;
        internal int EnableLinkVar = 0;
        internal bool EnableSwap = false;
        private readonly string[] NewSourceCodeType = {
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
            "RollControl"               // 13
        };

        internal UIElement(ElementType eType, string eName, string eMin, string eMax, string eDefault, string eOptions, int eStyle, bool eEnabled, int eTargetAmount, bool eSwap)
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
            EnableLinkVar = eTargetAmount;
            EnableSwap = eSwap;

            string EnabledDescription = "";
            if (EnabledWhen)
            {
                EnabledDescription += " {";
                if (EnableSwap)
                {
                    EnabledDescription += "!";
                }
                EnabledDescription += "Amount" + (EnableLinkVar + 1).ToString() + "} ";
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
                    if (Default != 0)
                    {
                        Default = 1;
                    }
                    break;
                case ElementType.ColorWheel:
                    Min = 0;
                    Max = 0xffffff;
                    ColorDefault = (eDefault == "None" ? "" : eDefault);
                    Description = eName;
                    if (ColorDefault != "")
                    {
                        Description += " (" + ColorDefault + ")";
                    }
                    Description += EnabledDescription;
                    break;
                case ElementType.AngleChooser:
                    Min = -180;
                    Max = 180;
                    Description = eName + " (" + Min.ToString() + ".." + Default.ToString() + ".." + Max.ToString() + ")" + EnabledDescription;
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
                    Default = 0;
                    Name += "|" + eOptions;
                    NameLength = Name.IndexOf("|", StringComparison.Ordinal);
                    if (NameLength == -1) NameLength = Name.Length;
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
                    Default = 0;
                    Name += "|" + eOptions;
                    NameLength = Name.IndexOf("|", StringComparison.Ordinal);
                    if (NameLength == -1) NameLength = Name.Length;
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
            }
        }

        internal UIElement(string RawSourceLine)
        {
            Regex REAmt = new Regex(@"\s*(?<type>.*)\s+Amount\d+\s*=\s*(?<default>.*);\s*\/{2}(?<rawcomment>.*)");
            Regex REColorOnly = new Regex(@"\s*\[\s*(?<defcolor>.*)\s*\](?<label>.*)");
            Regex REMaxOnly = new Regex(@"\s*\[\s*(?<maximum>\-?\d+.*\d*)\s*\](?<label>.*)");
            Regex REMinMax = new Regex(@"\s*\[\s*(?<minimum>\-?\d+.*\d*)\s*\,\s*(?<maximum>\-?\d+.*\d*)\s*\](?<label>.*)");
            Regex REMinMaxStyle = new Regex(@"\s*\[\s*(?<minimum>\-?\d+.*\d*)\s*\,\s*(?<maximum>\-?\d+.*\d*)\s*\,\s*(?<style>\-?\d+.*\d*)\s*\](?<label>.*)");
            Regex RELabel = new Regex(@"\s*(?<label>.*)");
            Regex REEnabled = new Regex(@"\s*{(?<swap>\!?)Amount(?<digits>\d+)\s*}\s*(?<label>.*)");

            string MinimumStr = "";
            string MaximumStr = "";
            string DefaultStr = "";
            string DefaultColor = "";
            string LabelStr = "";
            string TypeStr = "";
            string StyleStr = "0";

            int NameLength;

            Match m = REAmt.Match(RawSourceLine);
            if (m.Success)
            {
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
                    Match m1 = REMinMax.Match(m.Groups["rawcomment"].Value);
                    if (m1.Success)
                    {
                        MinimumStr = m1.Groups["minimum"].Value;
                        MaximumStr = m1.Groups["maximum"].Value;
                        LabelStr = m1.Groups["label"].Value.Trim();
                    }
                    else
                    {
                        Match m2 = REMaxOnly.Match(m.Groups["rawcomment"].Value);
                        if (m2.Success)
                        {
                            MinimumStr = "0";
                            MaximumStr = m2.Groups["maximum"].Value;
                            LabelStr = m2.Groups["label"].Value.Trim();
                        }
                        else
                        {
                            Match m3 = REColorOnly.Match(m.Groups["rawcomment"].Value);
                            if (m3.Success)
                            {
                                DefaultColor = m3.Groups["defcolor"].Value;
                                LabelStr = m3.Groups["label"].Value.Trim();
                            }
                            else
                            {
                                Match m1L = RELabel.Match(m.Groups["rawcomment"].Value);
                                if (m1L.Success)
                                {
                                    LabelStr = m1L.Groups["label"].Value.Trim();
                                }
                            }
                        }
                    }
                }
                Match me = REEnabled.Match(LabelStr);
                if (me.Success)
                {
                    string dgts = me.Groups["digits"].Value.Trim();
                    string swap = me.Groups["swap"].Value.Trim();
                    LabelStr = me.Groups["label"].Value.Trim();
                    if (!int.TryParse(dgts, out EnableLinkVar))
                    {
                        EnableLinkVar = 0;
                        EnabledWhen = false;
                        EnableSwap = false;
                    }
                    else
                    {
                        EnableLinkVar--;
                        EnabledWhen = true;
                        EnableSwap = (swap == "!");
                    }
                }
                DefaultStr = m.Groups["default"].Value.Trim();
                TypeStr = m.Groups["type"].Value.Trim();

                if ((TypeStr == "int") || (TypeStr == "IntSliderControl"))
                {
                    ElementType = ElementType.IntSlider;
                    if (!int.TryParse(DefaultStr, out Default)) Default = 0;
                    if (!int.TryParse(MinimumStr, out Min)) Min = 0;
                    if (!int.TryParse(MaximumStr, out Max)) Max = 100;
                    if (!int.TryParse(StyleStr, out Style)) Style = 0;
                    Style = Math.Max(0, Math.Min(MaxStyles, Style));
                    Name = LabelStr;
                }
                else if ((TypeStr == "bool") || (TypeStr == "CheckboxControl"))
                {
                    ElementType = ElementType.Checkbox;
                    if (DefaultStr.Contains("true"))
                    {
                        Default = 1;
                    }
                    else
                    {
                        Default = 0;
                    }
                    Min = 0;
                    Max = 1;
                    Name = LabelStr;
                }
                else if ((TypeStr == "ColorBgra") || (TypeStr == "ColorWheelControl"))
                {
                    ElementType = ElementType.ColorWheel;
                    Default = 0;
                    ColorDefault = DefaultColor;
                    Min = 0;
                    Max = 0xffffff;
                    Name = LabelStr;
                }
                else if ((TypeStr == "double") || (TypeStr == "AngleControl") || (TypeStr == "DoubleSliderControl"))
                {
                    if (!double.TryParse(MaximumStr, NumberStyles.Float, CultureInfo.InvariantCulture, out dMax)) dMax = 10;
                    if (!double.TryParse(MinimumStr, NumberStyles.Float, CultureInfo.InvariantCulture, out dMin)) dMin = 0;
                    if (!double.TryParse(DefaultStr, NumberStyles.Float, CultureInfo.InvariantCulture, out dDefault)) dDefault = 0;

                    if (!int.TryParse(MaximumStr, out Max)) Max = 180;
                    if (!int.TryParse(MinimumStr, out Min)) Min = -180;
                    if (!int.TryParse(DefaultStr, out Default)) Default = 45;

                    if (!int.TryParse(StyleStr, out Style)) Style = 0;
                    Style = Math.Max(0, Math.Min(MaxStyles, Style));

                    if (((TypeStr == "double") && (Min == -180) && (dMin == -180) && (Max == 180) && (dMax == 180) && (Default == 45) && (dDefault == 45)) || (TypeStr == "AngleControl"))
                    {
                        ElementType = ElementType.AngleChooser;
                        Style = 0;
                    }
                    else
                    {
                        ElementType = ElementType.DoubleSlider;
                        Max = (int)dMax;
                        Min = (int)dMin;
                        Default = (int)dDefault;
                    }
                    Name = LabelStr;
                }
                else if ((TypeStr == "Pair<double, double>") || (TypeStr == "PanSliderControl"))
                {
                    ElementType = ElementType.PanSlider;
                    Default = 0;
                    Min = -1;
                    Max = 1;
                    dMin = 0;
                    if (!double.TryParse(DefaultStr.Substring(DefaultStr.IndexOf('(') + 1, DefaultStr.IndexOf(',') - DefaultStr.IndexOf('(') - 1).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out dMin))
                    {
                        dMin = 0;
                    }
                    dMax = 0;
                    if (!double.TryParse(DefaultStr.Substring(DefaultStr.IndexOf(',') + 1, DefaultStr.IndexOf(')') - DefaultStr.IndexOf(',') - 1).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out dMax))
                    {
                        dMax = 0;
                    }
                    Name = LabelStr;
                }
                else if ((TypeStr == "string") || (TypeStr == "TextboxControl") || (TypeStr == "MultiLineTextboxControl"))
                {
                    ElementType = ElementType.Textbox;
                    if (!int.TryParse(DefaultStr, out Default)) Default = 0;
                    if (!int.TryParse(MinimumStr, out Min)) Min = 0;
                    if (!int.TryParse(MaximumStr, out Max)) Max = 255;
                    if ((Min > 0) || (TypeStr == "MultiLineTextboxControl")) ElementType = ElementType.MultiLineTextbox;
                    Name = LabelStr;
                }
                else if ((TypeStr == "byte") || (TypeStr == "ListBoxControl") || (TypeStr == "RadioButtonControl") || (TypeStr == "ReseedButtonControl"))
                {
                    if ((TypeStr == "ListBoxControl") || (MaximumStr == ""))
                    {
                        ElementType = ElementType.DropDown;
                        if (!int.TryParse(DefaultStr, out Default)) Default = 0;
                        Min = 0;
                        Max = 0;
                        Name = LabelStr;
                    }
                    else if ((TypeStr == "RadioButtonControl") || (MaximumStr == "1"))
                    {
                        ElementType = ElementType.RadioButtons;
                        if (!int.TryParse(DefaultStr, out Default)) Default = 0;
                        Min = 0;
                        Max = 0;
                        Name = LabelStr;
                    }
                    else if ((TypeStr == "ReseedButtonControl") || (MaximumStr == "255"))
                    {
                        ElementType = ElementType.ReseedButton;
                        Default = 0;
                        Min = 0;
                        Max = 255;
                        Name = LabelStr;
                    }
                    if (!LabelStr.Contains("|"))
                    {
                        ElementType = ElementType.ReseedButton;
                        Default = 0;
                        Min = 0;
                        Max = 255;
                        Name = LabelStr;
                    }
                }
                else if (TypeStr == "UserBlendOp" || TypeStr == "BinaryPixelOp")
                {
                    ElementType = ElementType.BinaryPixelOp;
                    Default = 0;
                    Min = 0;
                    Max = 0;
                    Name = LabelStr;
                }
                else if (TypeStr == "FontFamily")
                {
                    ElementType = ElementType.FontFamily;
                    Default = 0;
                    Min = 0;
                    Max = 0;
                    Name = LabelStr;
                }
                else if ((TypeStr == "Tuple<double, double, double>") || (TypeStr == "RollControl"))
                {
                    ElementType = ElementType.RollBall;
                    Default = 0;
                    Min = 0;
                    Max = 1;
                    Name = LabelStr;
                }
                else return;

                string EnabledDescription = "";
                if (EnabledWhen)
                {
                    EnabledDescription += " {";
                    if (EnableSwap)
                    {
                        EnabledDescription += "!";
                    }
                    EnabledDescription += "Amount" + (EnableLinkVar + 1).ToString() + "} ";
                }

                switch (ElementType)
                {
                    case ElementType.IntSlider:
                        Description = Name + " (" + Min.ToString() + ".." + Default.ToString() + ".." + Max.ToString() + ")" + EnabledDescription;
                        break;
                    case ElementType.Checkbox:
                        Description = Name + ((Default == 0) ? " (unchecked)" : " (checked)") + EnabledDescription;
                        break;
                    case ElementType.ColorWheel:
                        Description = Name;
                        if (ColorDefault != "")
                        {
                            Description += " (" + ColorDefault + ")";
                        }
                        Description += EnabledDescription;
                        break;
                    case ElementType.AngleChooser:
                        Description = Name + " (" + Min.ToString() + ".." + Default.ToString() + ".." + Max.ToString() + ")" + EnabledDescription;
                        break;
                    case ElementType.PanSlider:
                        Description = Name + EnabledDescription;
                        break;
                    case ElementType.Textbox:
                        Description = Name + " (" + Max.ToString() + ")" + EnabledDescription;
                        break;
                    case ElementType.DoubleSlider:
                        Description = Name + " (" + dMin.ToString() + ".." + dDefault.ToString() + ".." + dMax.ToString() + ")" + EnabledDescription;
                        break;
                    case ElementType.DropDown:
                        NameLength = Name.IndexOf("|", StringComparison.Ordinal);
                        if (NameLength == -1) NameLength = Name.Length;
                        Description = Name.Substring(0, NameLength) + EnabledDescription;
                        break;
                    case ElementType.BinaryPixelOp:
                        Description = Name + " (Normal)" + EnabledDescription;
                        break;
                    case ElementType.FontFamily:
                        Description = Name + EnabledDescription;
                        break;
                    case ElementType.RadioButtons:
                        NameLength = Name.IndexOf("|", StringComparison.Ordinal);
                        if (NameLength == -1) NameLength = Name.Length;
                        Description = Name.Substring(0, NameLength) + EnabledDescription;
                        break;
                    case ElementType.ReseedButton:
                        Description = Name + " (Button)" + EnabledDescription;
                        break;
                    case ElementType.MultiLineTextbox:
                        Description = Name + " (" + Max.ToString() + ")" + EnabledDescription;
                        break;
                    case ElementType.RollBall:
                        Description = Name + EnabledDescription;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // don't understand raw source line
            }
        }

        public override string ToString()
        {
            return Description;
        }

        internal string ToSourceString(int ItemNumber, ColorBgra PC)
        {
            string SourceCode = "";

            SourceCode += NewSourceCodeType[(int)ElementType] + " ";
            SourceCode += "Amount" + ItemNumber.ToString();
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
                    Color c = Color.White;
                    if (ColorDefault == "" || ColorDefault == "PrimaryColor")
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
                    SourceCode += " = ColorBgra.FromBgr(" + c.B.ToString() + "," + c.G.ToString() + "," + c.R.ToString() + ")";
                    SourceCode += "; // ";
                    if (ColorDefault != "")
                    {
                        SourceCode += "[" + ColorDefault + "] ";
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
                    SourceCode += " = 0; // ";
                    break;
                case ElementType.BinaryPixelOp:
                    SourceCode += " = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal); // ";
                    break;
                case ElementType.FontFamily:
                    SourceCode += " = new FontFamily(\"Arial\"); // ";
                    break;
                case ElementType.RadioButtons:
                    SourceCode += " = 0; // [1] ";
                    break;
                case ElementType.ReseedButton:
                    SourceCode += " = 0; // [255] ";
                    break;
                case ElementType.RollBall:
                    SourceCode += " = Tuple.Create<double, double, double>( 0.0 , 0.0 , 0.0 )";
                    SourceCode += "; // ";
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
                SourceCode += "Amount" + (EnableLinkVar + 1).ToString();
                SourceCode += "} ";
            }
            SourceCode += Name;
            SourceCode += "\r\n";

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
            if ((ElementType == ElementType.DropDown) || (ElementType == ElementType.RadioButtons))
            {
                int BarLoc = Name.IndexOf("|", StringComparison.Ordinal);
                if (BarLoc == -1) return Name;
                return Name.Substring(0, BarLoc);
            }
            return Name;
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
        RollBall
    }
}
