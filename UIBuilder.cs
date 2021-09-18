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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal partial class UIBuilder : ChildFormBase
    {
        internal string UIControlsText;
        private ColorBgra PC;
        private readonly ProjectType projectType;
        private bool dirty = false;
        private readonly List<UIElement> MasterList = new List<UIElement>();
        private readonly HashSet<string> IDList = new HashSet<string>();
        private string currentID;

        internal UIBuilder(string UserScriptText, ProjectType projectType, ColorBgra PrimaryColor)
        {
            InitializeComponent();

            // PDN Theme
            ControlListView.ForeColor = this.ForeColor;
            ControlListView.BackColor = this.BackColor;
            foreach (Control control in this.Controls)
            {
                if (control is TextBox || control is ComboBox)
                {
                    control.ForeColor = this.ForeColor;
                    control.BackColor = this.BackColor;
                }
            }

            ControlListView.Font = new Font(Settings.FontFamily, ControlListView.Font.SizeInPoints);

            // Populate the ControlType dropdown based on allowed ElementTypes
            List<ControlTypeItem> controlTypes = new List<ControlTypeItem>();
            foreach (ElementType elementType in Enum.GetValues(typeof(ElementType)))
            {
                if (UIElement.IsControlAllowed(elementType, projectType))
                {
                    controlTypes.Add(new ControlTypeItem(elementType));
                }
            }
            this.ControlType.Items.Clear();
            this.ControlType.Items.AddRange(controlTypes.ToArray());

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

            imgList.ImageSize = UIUtil.ScaleSize(16, 16);
            imgList.Images.AddRange(new Image[]
            {
                UIUtil.GetImage("00int"),
                UIUtil.GetImage("01CheckBox"),
                UIUtil.GetImage("02ColorWheel"),
                UIUtil.GetImage("03AngleChooser"),
                UIUtil.GetImage("04PanSlider"),
                UIUtil.GetImage("05TextBox"),
                UIUtil.GetImage("06DoubleSlider"),
                UIUtil.GetImage("07DropDown"),
                UIUtil.GetImage("08BlendOps"),
                UIUtil.GetImage("09Fonts"),
                UIUtil.GetImage("10RadioButton"),
                UIUtil.GetImage("11ReseedButton"),
                UIUtil.GetImage("12MultiTextBox"),
                UIUtil.GetImage("13RollControl"),
                UIUtil.GetImage("14FilenameControl"),
                UIUtil.GetImage("15Uri")
            });

            DefaultColorComboBox.DropDownWidth = DefaultColorComboBox.Width * 2;
            DefaultColorComboBox.Items.Add("None");
            DefaultColorComboBox.Items.Add("PrimaryColor");
            DefaultColorComboBox.Items.Add("SecondaryColor");
            DefaultColorComboBox.Items.AddRange(UIUtil.GetColorNames(false));

            MasterList.AddRange(UIElement.ProcessUIControls(UserScriptText, projectType));

            foreach (UIElement element in MasterList)
            {
                IDList.Add(element.Identifier);
            }
            refreshListView(0);
            dirty = false;
            PC = PrimaryColor;
            this.projectType = projectType;
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
                UIControlsText += uie.ToSourceString(true);
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
            ElementType elementType = (ControlType.SelectedItem is ControlTypeItem item) ? item.ElementType : ElementType.IntSlider;
            string defaultStr = (elementType == ElementType.ColorWheel) ? DefaultColorComboBox.SelectedItem.ToString() : ControlDef.Text;
            if (elementType == ElementType.Uri) defaultStr = OptionsText.Text.Trim();
            string identifier = ControlID.Text.Trim();
            if (identifier.Length == 0 || IDList.Contains(identifier))
            {
                identifier = "Amount" + (MasterList.Count + 1);
            }
            string enableIndentifer = (this.rbEnabledWhen.Checked) ? MasterList[enabledWhenField.SelectedIndex].Identifier : string.Empty;
            MasterList.Add(new UIElement(elementType, ControlName.Text, ControlMin.Text, ControlMax.Text, defaultStr, OptionsText.Text, ControlStyle.SelectedIndex, rbEnabledWhen.Checked, enableIndentifer, (enabledWhenCondition.SelectedIndex != 0), identifier, null));
            IDList.Add(identifier);
            refreshListView(MasterList.Count - 1);
            dirty = false;
        }

        private void ControlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dirty = true;

            // reset options
            OptionsLabel.Text = "Options:";
            toolTip1.SetToolTip(this.OptionsText, "Separate options with the vertical bar character (|)");

            // setup UI based on selected control type
            if (ControlType.SelectedItem is not ControlTypeItem item)
            {
                return;
            }

            switch (item.ElementType)
            {
                case ElementType.IntSlider:
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
                    break;
                case ElementType.Checkbox:
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
                    ControlDef.Text = (int.TryParse(ControlDef.Text, out int result) && result > 0) ? "1" : "0";
                    ControlDef.Enabled = true;
                    StyleLabel.Enabled = false;
                    ControlStyle.Enabled = false;
                    ControlStyle.SelectedIndex = 0;
                    break;
                case ElementType.ColorWheel:
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
                    break;
                case ElementType.AngleChooser:
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
                    break;
                case ElementType.PanSlider:
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
                    ControlDef.Text = "0.0,0.0";
                    StyleLabel.Enabled = false;
                    ControlStyle.Enabled = false;
                    ControlStyle.SelectedIndex = 0;
                    break;
                case ElementType.Textbox:
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
                    break;
                case ElementType.DoubleSlider:
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
                    break;
                case ElementType.DropDown:
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
                    OptionsText.Text = "Option1|Option2";
                    break;
                case ElementType.BinaryPixelOp:
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
                    break;
                case ElementType.FontFamily:
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
                    break;
                case ElementType.RadioButtons:
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
                    OptionsText.Text = "Option1|Option2";
                    break;
                case ElementType.ReseedButton:
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
                    break;
                case ElementType.MultiLineTextbox:
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
                    break;
                case ElementType.RollBall:
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
                    break;
                case ElementType.Filename:
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
                    break;
                case ElementType.Uri:
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
                    OptionsText.Text = "https://www.GetPaint.net";
                    OptionsLabel.Text = "URL:";
                    toolTip1.SetToolTip(this.OptionsText, "URL must begin with 'http://' or 'https://' to be valid.");
                    break;
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

            ElementType elementType = (ControlType.SelectedItem is ControlTypeItem item) ? item.ElementType : ElementType.IntSlider;
            string defaultStr = (elementType == ElementType.ColorWheel) ? DefaultColorComboBox.SelectedItem.ToString() : ControlDef.Text;
            if (elementType == ElementType.Uri) defaultStr = OptionsText.Text.Trim();
            string identifier = !string.IsNullOrWhiteSpace(ControlID.Text) ? ControlID.Text.Trim() : "Amount" + (MasterList.Count + 1);
            string enableIndentifer = (this.rbEnabledWhen.Checked) ? MasterList[enabledWhenField.SelectedIndex].Identifier : string.Empty;
            string typeEnum = (CurrentItem > -1) ? MasterList[CurrentItem].TEnum : null;
            UIElement uiElement = new UIElement(elementType, ControlName.Text, ControlMin.Text, ControlMax.Text, defaultStr, OptionsText.Text, ControlStyle.SelectedIndex, rbEnabledWhen.Checked, enableIndentifer, (enabledWhenCondition.SelectedIndex != 0), identifier, typeEnum);

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
                    if (enabledWhenField.Items[i].ToString().StartsWith(CurrentElement.EnableIdentifier, StringComparison.Ordinal))
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

            ControlType.SelectedIndex = FindControlTypeIndex(CurrentElement.ElementType);

            switch (CurrentElement.ElementType)
            {
                case ElementType.IntSlider:
                    FillStyleDropDown(0);
                    ControlStyle.SelectedIndex = (int)CurrentElement.Style;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.Checkbox:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.ColorWheel:
                    FillStyleDropDown(1);
                    bool alpha = CurrentElement.ColorWheelOptions.HasFlag(ColorWheelOptions.Alpha);
                    bool noReset = CurrentElement.ColorWheelOptions.HasFlag(ColorWheelOptions.NoReset);
                    ControlStyle.SelectedIndex = (noReset && alpha) ? 3 : noReset ? 2 : alpha ? 1 : 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    DefaultColorComboBox.Text = (CurrentElement.StrDefault.Length == 0 ? "None" : CurrentElement.StrDefault);
                    break;
                case ElementType.AngleChooser:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.dMin.ToString();
                    ControlMax.Text = CurrentElement.dMax.ToString();
                    ControlDef.Text = CurrentElement.dDefault.ToString();
                    break;
                case ElementType.PanSlider:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.dMin.ToString();
                    ControlMax.Text = CurrentElement.dMax.ToString();
                    ControlDef.Text = CurrentElement.StrDefault;
                    break;
                case ElementType.Textbox:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.DoubleSlider:
                    FillStyleDropDown(0);
                    ControlStyle.SelectedIndex = (int)CurrentElement.Style;
                    ControlMin.Text = CurrentElement.dMin.ToString();
                    ControlMax.Text = CurrentElement.dMax.ToString();
                    ControlDef.Text = CurrentElement.dDefault.ToString();
                    break;
                case ElementType.DropDown:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    BarLoc = CurrentElement.Name.IndexOf("|", StringComparison.Ordinal);
                    OptionsText.Text = CurrentElement.Name.Substring(BarLoc + 1);
                    ControlName.Text = CurrentElement.ToShortName();
                    break;
                case ElementType.BinaryPixelOp:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.FontFamily:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.RadioButtons:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.StrDefault;
                    BarLoc = CurrentElement.Name.IndexOf("|", StringComparison.Ordinal);
                    OptionsText.Text = CurrentElement.Name.Substring(BarLoc + 1);
                    ControlName.Text = CurrentElement.ToShortName();
                    break;
                case ElementType.ReseedButton:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.MultiLineTextbox:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.RollBall:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    break;
                case ElementType.Filename:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    BarLoc = CurrentElement.Name.IndexOf("|", StringComparison.Ordinal);
                    OptionsText.Text = CurrentElement.Name.Substring(BarLoc + 1);
                    ControlName.Text = CurrentElement.ToShortName();
                    break;
                case ElementType.Uri:
                    ControlStyle.SelectedIndex = 0;
                    ControlMin.Text = CurrentElement.Min.ToString();
                    ControlMax.Text = CurrentElement.Max.ToString();
                    ControlDef.Text = CurrentElement.Default.ToString();
                    OptionsText.Text = CurrentElement.StrDefault;
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

        private void ControlType_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();
            if (this.ControlType.Items[e.Index] is ControlTypeItem item)
            {
                e.Graphics.DrawImage(ControlListView.SmallImageList.Images[(int)item.ElementType], e.Bounds.X + 2, e.Bounds.Y + 1, e.Bounds.Height, e.Bounds.Height - 2);
                Rectangle textBounds = Rectangle.FromLTRB(e.Bounds.Left + ControlListView.SmallImageList.ImageSize.Width + 4, e.Bounds.Top + 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                TextRenderer.DrawText(e.Graphics, item.ToString(), e.Font, textBounds, e.ForeColor, TextFormatFlags.VerticalCenter);
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, this.ControlType.Items[e.Index].ToString(), e.Font, new Point(e.Bounds.Left, e.Bounds.Top), e.ForeColor);
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
            if (ControlType.SelectedItem is ControlTypeItem item && item.ElementType == ElementType.ColorWheel)
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
#if FASTDEBUG
            return;
#endif
            switch (this.projectType)
            {
                case ProjectType.Effect:
                    PreviewEffect();
                    break;
                case ProjectType.FileType:
                    PreviewFileType();
                    break;
            }
        }

        private void PreviewEffect()
        {
            string uiCode = MasterList.Select(uiE => uiE.ToSourceString(false)).Join("");
            if (!ScriptBuilder.BuildUiPreview(uiCode))
            {
                FlexibleMessageBox.Show("Something went wrong, and the Preview can't be displayed.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!ScriptBuilder.BuiltEffect.Options.Flags.HasFlag(EffectFlags.Configurable))
            {
                FlexibleMessageBox.Show("There are no UI controls, so the Preview can't be displayed.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (Surface emptySurface = new Surface(400, 300))
                using (PdnRegion selection = new PdnRegion(emptySurface.Bounds))
                using (EffectEnvironmentParameters enviroParams = new EffectEnvironmentParameters(ColorBgra.Black, Color.White, 0, Document.DefaultResolution, selection, emptySurface))
                {
                    emptySurface.Fill(ColorBgra.White);
                    ScriptBuilder.BuiltEffect.EnvironmentParameters = enviroParams;
                    ScriptBuilder.BuiltEffect.CreateConfigDialog().ShowDialog();
                }
            }
        }

        private void PreviewFileType()
        {
            string code = "#region UICode\r\n";
            code += MasterList.Select(uiE => uiE.ToSourceString(false)).Join("");
            code += "\r\n";
            code += "#endregion\r\n";
            code += "void SaveImage(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)\r\n";
            code += "{}\r\n";
            code += "Document LoadImage(Stream input)\r\n";
            code += "{ return new Document(400, 300); }\r\n";

            if (!ScriptBuilder.BuildFileType(code, false))
            {
                MessageBox.Show("Compilation Failed!");
                return;
            }

            if (!ScriptBuilder.BuiltFileType.SupportsConfiguration)
            {
                MessageBox.Show("No Config!");
                return;
            }

            using (SaveWidgetDialog widgetDialog = new SaveWidgetDialog(
                ScriptBuilder.BuiltFileType.CreateSaveConfigWidget(),
                ScriptBuilder.BuiltFileType.CreateDefaultSaveConfigToken()))
            {
                widgetDialog.ShowDialog();
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

            if (ControlType.SelectedItem is ControlTypeItem item &&
                item.ElementType != ElementType.AngleChooser &&
                item.ElementType != ElementType.DoubleSlider)
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

            if (ControlType.SelectedItem is ControlTypeItem item &&
                item.ElementType != ElementType.AngleChooser &&
                item.ElementType != ElementType.DoubleSlider)
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

            if (ControlType.SelectedItem is ControlTypeItem item &&
                item.ElementType != ElementType.AngleChooser &&
                item.ElementType != ElementType.DoubleSlider)
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
            ControlID.ForeColor = error ? Color.Black : Color.Black;
            ControlID.BackColor = error ? Color.FromArgb(246, 97, 81) : Color.White;
        }

        private void OptionsText_TextChanged(object sender, EventArgs e)
        {
            dirty = true;
            string newOptions = OptionsText.Text.Trim().ToLowerInvariant();
            bool error = false;
            if (ControlType.SelectedItem is ControlTypeItem item && item.ElementType == ElementType.Uri)
            {
                // Make sure the URL is valid.
                if (!newOptions.IsWebAddress())
                {
                    error = true;
                }
                else
                {
                    try
                    {
                        Uri uri = new Uri(newOptions);
                    }
                    catch
                    {
                        error = true;
                    }
                }
            }
            else
            {
                // Make sure it looks like options (should contain at least one | character.
                // Although not TECHNICALLY required... let's make it required anyway.
                if (!newOptions.Contains("|"))
                {
                    error = true;
                }
            }
            OptionsText.ForeColor = error ? Color.Black : Color.Black;
            OptionsText.BackColor = error ? Color.FromArgb(246, 97, 81) : Color.White;
        }

        private int FindControlTypeIndex(ElementType elementType)
        {
            for (int i = 0; i < ControlType.Items.Count; i++)
            {
                if (this.ControlType.Items[i] is ControlTypeItem controlTypeItem && controlTypeItem.ElementType == elementType)
                {
                    return i;
                }
            }

            return -1;
        }

        private class ControlTypeItem : IComparable<ControlTypeItem>
        {
            private readonly string text;
            internal ElementType ElementType { get; }

            internal ControlTypeItem(ElementType elementType)
            {
                this.text = elementType.GetDescription() ?? elementType.ToString();
                this.ElementType = elementType;
            }

            public override string ToString()
            {
                return this.text;
            }

            public int CompareTo(ControlTypeItem other)
            {
                return string.Compare(this.text, other.text, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    internal class UIElement
    {
        private readonly string Description;
        internal readonly string Name;
        internal readonly ElementType ElementType;
        internal readonly int Min;
        internal readonly int Max;
        internal readonly int Default;
        internal readonly string StrDefault;
        internal readonly ColorWheelOptions ColorWheelOptions;
        internal readonly double dMin;
        internal readonly double dMax;
        internal readonly double dDefault;
        internal readonly SliderStyle Style;
        internal readonly bool EnabledWhen;
        internal readonly bool EnableSwap;
        internal readonly string EnableIdentifier;
        internal readonly string Identifier;
        internal readonly string TEnum;

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
            "FilenameControl",          // 14
            "Uri"                       // 15
        };

        internal static UIElement[] ProcessUIControls(string SourceCode, ProjectType projectType)
        {
            string UIControlsText = "";
            Match mcc = Regex.Match(SourceCode, @"\#region UICode(?<sublabel>.*?)\#endregion", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (mcc.Success)
            {
                // We found the standard #region UICode/#endregion block
                UIControlsText = mcc.Groups["sublabel"].Value.Trim();
            }
            else
            {
                // Find standard UI controls from REALLY OLD scripts
                Match ma1 = Regex.Match(SourceCode, @"int\s+Amount1\s*=\s*\-?\d+.*\n", RegexOptions.IgnoreCase);
                if (ma1.Success)
                {
                    UIControlsText = ma1.Value;
                    Match ma2 = Regex.Match(SourceCode, @"int\s+Amount2\s*=\s*\-?\d+.*\n", RegexOptions.IgnoreCase);
                    if (ma2.Success)
                    {
                        UIControlsText += ma2.Value;
                        Match ma3 = Regex.Match(SourceCode, @"int\s+Amount3\s*=\s*\-?\d+.*\n", RegexOptions.IgnoreCase);
                        if (ma3.Success)
                        {
                            UIControlsText += ma3.Value;
                        }
                    }
                }
            }

            if (UIControlsText.Length == 0)
            {
                return Array.Empty<UIElement>();
            }

            // process those UI controls
            string[] SrcLines = UIControlsText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            List<UIElement> UserControls = new List<UIElement>();
            foreach (string Line in SrcLines)
            {
                if (Line.StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }

                UIElement element = UIElement.FromSourceLine(Line);
                if (element != null && IsControlAllowed(element.ElementType, projectType))
                {
                    UserControls.Add(element);
                }
            }

            return UserControls.ToArray();
        }

        internal static bool IsControlAllowed(ElementType elementType, ProjectType projectType)
        {
            if (projectType != ProjectType.FileType)
            {
                return true;
            }

            switch (elementType)
            {
                case ElementType.IntSlider:
                case ElementType.Checkbox:
                case ElementType.Uri:
                case ElementType.Textbox:
                case ElementType.MultiLineTextbox:
                case ElementType.DoubleSlider:
                case ElementType.DropDown:
                case ElementType.RadioButtons:
                    return true;
            }

            return false;
        }

        internal UIElement(ElementType eType, string eName, string eMin, string eMax, string eDefault, string eOptions, int eStyle, bool eEnabled, string targetIdentifier, bool eSwap, string identifier, string typeEnum)
        {
            Name = eName;
            ElementType = eType;
            Identifier = identifier;

            if (!double.TryParse(eMax, out double parsedMax)) parsedMax = 100;
            if (!double.TryParse(eMin, out double parsedMin)) parsedMin = 0;
            if (!double.TryParse(eDefault, out double parsedDefault)) parsedDefault = 0;

            string EnabledDescription = "";
            if (eEnabled)
            {
                EnabledWhen = true;
                EnableIdentifier = targetIdentifier;
                EnableSwap = eSwap;

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
                    Min = (int)parsedMin;
                    Max = (int)parsedMax;
                    Default = (int)parsedDefault;
                    Style = Enum.IsDefined(typeof(SliderStyle), eStyle) ? (SliderStyle)eStyle : 0;
                    Description = eName + " (" + Min.ToString() + ".." + Default.ToString() + ".." + Max.ToString() + ")" + EnabledDescription;
                    break;
                case ElementType.Checkbox:
                    Max = 1;
                    Default = (int)parsedDefault;
                    if (Default != 0)
                    {
                        Default = 1;
                    }
                    Description = eName + ((Default == 0) ? " (unchecked)" : " (checked)") + EnabledDescription;
                    break;
                case ElementType.ColorWheel:
                    StrDefault = (eDefault == "None") ? string.Empty : eDefault;
                    ColorWheelOptions = (eStyle == 3) ? ColorWheelOptions.Alpha | ColorWheelOptions.NoReset : (eStyle == 2) ? ColorWheelOptions.NoReset : (eStyle == 1) ? ColorWheelOptions.Alpha : ColorWheelOptions.None;
                    Description = eName;
                    bool alpha = ColorWheelOptions.HasFlag(ColorWheelOptions.Alpha);
                    Min = alpha ? int.MinValue : 0;
                    Max = alpha ? int.MaxValue : 0xffffff;
                    if (StrDefault.Length > 0)
                    {
                        string alphastyle = alpha ? "?" : "";
                        string resetstyle = ColorWheelOptions.HasFlag(ColorWheelOptions.NoReset) ? "!" : "";
                        Description += " (" + StrDefault + alphastyle + resetstyle + ")";
                    }
                    Description += EnabledDescription;
                    break;
                case ElementType.AngleChooser:
                    dMin = parsedMin.Clamp(-180.0, 360.0);
                    double upperBound = (dMin < 0.0) ? 180.0 : 360;
                    dMax = parsedMax.Clamp(dMin, upperBound);
                    dDefault = parsedDefault.Clamp(dMin, dMax);
                    Description = eName + " (" + dMin.ToString() + ".." + dDefault.ToString() + ".." + dMax.ToString() + ")" + EnabledDescription;
                    break;
                case ElementType.PanSlider:
                    dMin = -1;
                    dMax = 1;
                    StrDefault = eDefault;
                    Description = eName + EnabledDescription;
                    break;
                case ElementType.Textbox:
                    Max = (int)parsedMax;
                    Description = eName + " (" + Max.ToString() + ")" + EnabledDescription;
                    break;
                case ElementType.DoubleSlider:
                    dMin = parsedMin;
                    dMax = parsedMax;
                    dDefault = parsedDefault;
                    Style = Enum.IsDefined(typeof(SliderStyle), eStyle) ? (SliderStyle)eStyle : 0;
                    Description = eName + " (" + dMin.ToString() + ".." + dDefault.ToString() + ".." + dMax.ToString() + ")" + EnabledDescription;
                    break;
                case ElementType.DropDown:
                case ElementType.RadioButtons:
                    Name += "|" + eOptions;
                    int maxValue = Name.Split('|').Length - 2;
                    Default = (int)parsedDefault.Clamp(0, maxValue);
                    StrDefault = eDefault;
                    int nameLength1 = Name.IndexOf("|", StringComparison.Ordinal);
                    if (nameLength1 == -1) nameLength1 = Name.Length;
                    Description = Name.Substring(0, nameLength1) + EnabledDescription;
                    TEnum = typeEnum;
                    break;
                case ElementType.BinaryPixelOp:
                    Description = eName + " (Normal)" + EnabledDescription;
                    break;
                case ElementType.FontFamily:
                    Description = eName + EnabledDescription;
                    break;
                case ElementType.ReseedButton:
                    Max = 255;
                    Description = eName + " (Button)" + EnabledDescription;
                    break;
                case ElementType.MultiLineTextbox:
                    Min = 1;
                    Max = (int)parsedMax;
                    Default = 1;
                    Description = eName + " (" + Max.ToString() + ")" + EnabledDescription;
                    break;
                case ElementType.RollBall:
                    Max = 1;
                    Description = eName + EnabledDescription;
                    break;
                case ElementType.Filename:
                    Name += "|" + eOptions;
                    int nameLength2 = Name.IndexOf("|", StringComparison.Ordinal);
                    if (nameLength2 == -1) nameLength2 = Name.Length;
                    Description = Name.Substring(0, nameLength2) + EnabledDescription;
                    break;
                case ElementType.Uri:
                    Max = 255;
                    StrDefault = eDefault;
                    Description = eName + " (Web Link)" + EnabledDescription;
                    break;
            }
        }

        private static UIElement FromSourceLine(string RawSourceLine)
        {
            Match m = Regex.Match(RawSourceLine, @"\s*(?<type>.*)\s+(?<identifier>.+\b)\s*=\s*(?<default>.*);\s*\/{2}(?<rawcomment>.*)");
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

            string rawComment = m.Groups["rawcomment"].Value;
            Match m0 = Regex.Match(rawComment, @"\s*\[\s*(?<minimum>\-?\d+.*\d*)\s*\,\s*(?<maximum>\-?\d+.*\d*)\s*\,\s*(?<style>\-?\d+.*\d*)\s*\](?<label>.*)");
            if (m0.Success)
            {
                MinimumStr = m0.Groups["minimum"].Value;
                MaximumStr = m0.Groups["maximum"].Value;
                StyleStr = m0.Groups["style"].Value;
                LabelStr = m0.Groups["label"].Value.Trim();
            }
            else
            {
                Match m1 = Regex.Match(rawComment, @"\s*\[\s*(?<minimum>\-?\d+.*\d*)\s*\,\s*(?<maximum>\-?\d+[^]]*\d*)\s*\](?<label>.*)");
                if (m1.Success)
                {
                    MinimumStr = m1.Groups["minimum"].Value;
                    MaximumStr = m1.Groups["maximum"].Value;
                    LabelStr = m1.Groups["label"].Value.Trim();
                }
                else
                {
                    Match m2 = Regex.Match(rawComment, @"\s*\[\s*(?<maximum>\-?\d+.*\d*)\s*\](?<label>.*)");
                    if (m2.Success)
                    {
                        MinimumStr = "0";
                        MaximumStr = m2.Groups["maximum"].Value;
                        LabelStr = m2.Groups["label"].Value.Trim();
                    }
                    else
                    {
                        Match m3 = Regex.Match(rawComment, @"\s*\[\s*(?<defcolor>.*)\s*\](?<label>.*)");
                        if (m3.Success)
                        {
                            DefaultColor = m3.Groups["defcolor"].Value;
                            LabelStr = m3.Groups["label"].Value.Trim();
                        }
                        else
                        {
                            Match m1L = Regex.Match(rawComment, @"\s*(?<label>.*)");
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

            Match me = Regex.Match(LabelStr, @"\s*{(?<swap>\!?)(?<identifier>.+\b)\s*}\s*(?<label>.*)");
            if (me.Success)
            {
                LabelStr = me.Groups["label"].Value.Trim();

                enabled = true;
                targetID = me.Groups["identifier"].Value;
                swap = me.Groups["swap"].Value.Trim() == "!";
            }

            string DefaultStr = m.Groups["default"].Value.Trim();
            ElementType elementType = ElementType.IntSlider;

            string TypeStr = m.Groups["type"].Value.Trim();
            Match mTEnum = Regex.Match(TypeStr, @"(?<Type>\S+)<(?<TEnum>\S+)>");
            if (mTEnum.Success)
            {
                TypeStr = mTEnum.Groups["Type"].Value;
            }

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
            else if (TypeStr == "Uri")
            {
                elementType = ElementType.Uri;
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
                elementType = (int.TryParse(MinimumStr, out int min) && min > 0) ? ElementType.MultiLineTextbox : ElementType.Textbox;
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
                if (int.TryParse(MinimumStr, out int iMin) && (iMin == -180) &&
                    int.TryParse(MaximumStr, out int iMax) && (iMax == 180) &&
                    int.TryParse(DefaultStr, out int iDefault) && (iDefault == 45))
                {
                    elementType = ElementType.AngleChooser;
                }
                else
                {
                    elementType = ElementType.DoubleSlider;
                }
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

            string defaultValue = "";
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
            else if (elementType == ElementType.Uri)
            {
                Match muri = Regex.Match(DefaultStr, @"""(?<uri>.*?[^\\])""");
                if (muri.Success)
                {
                    defaultValue = muri.Groups["uri"].Value;
                }
            }
            else if (elementType == ElementType.Checkbox)
            {
                defaultValue = DefaultStr.Contains("true", StringComparison.OrdinalIgnoreCase) ? "1" : "0";
            }
            else if (elementType == ElementType.PanSlider)
            {
                double x = 0;
                double y = 0;
                Match xyPair = Regex.Match(DefaultStr, @"\bPair.Create\(\s*(?<x>-?\s*\d*.?\d*)\s*,\s*(?<y>-?\s*\d*.?\d*)\s*\)");
                if (xyPair.Success)
                {
                    if (double.TryParse(xyPair.Groups["x"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                    {
                        x = x.Clamp(-1, 1);
                    }
                    if (double.TryParse(xyPair.Groups["y"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                    {
                        y = y.Clamp(-1, 1);
                    }
                }

                defaultValue = x.ToString("F3", CultureInfo.InvariantCulture) + ", " + y.ToString("F3", CultureInfo.InvariantCulture);
            }
            else if (mTEnum.Success && (elementType == ElementType.DropDown || elementType == ElementType.RadioButtons))
            {
                defaultValue = DefaultStr;
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
            string options = (pipeIndex > -1) ? LabelStr.Substring(pipeIndex + 1) : string.Empty;
            string typeEnum = mTEnum.Success ? mTEnum.Groups["TEnum"].Value : null;

            return new UIElement(elementType, name, dMin.ToString(), dMax.ToString(), defaultValue, options, style, enabled, targetID, swap, id, typeEnum);
        }

        public override string ToString()
        {
            return Identifier + " — " + Description;
        }

        internal string ToSourceString(bool useTEnum)
        {
            bool hasTEnum = useTEnum && !TEnum.IsNullOrEmpty();
            string typeEnum = hasTEnum ? $"<{TEnum}>" : string.Empty;
            string SourceCode = NewSourceCodeType[(int)ElementType] + typeEnum + " " + Identifier;
            switch (ElementType)
            {
                case ElementType.IntSlider:
                    SourceCode += " = " + Default.ToString();
                    SourceCode += "; // [" + Min.ToString() + "," + Max.ToString();
                    if (Style > 0)
                    {
                        SourceCode += "," + (int)Style;
                    }
                    SourceCode += "] ";
                    break;
                case ElementType.AngleChooser:
                case ElementType.DoubleSlider:
                    SourceCode += " = " + dDefault.ToString(CultureInfo.InvariantCulture);
                    SourceCode += "; // [" + dMin.ToString(CultureInfo.InvariantCulture) + "," + dMax.ToString(CultureInfo.InvariantCulture);
                    if (Style > 0)
                    {
                        SourceCode += "," + (int)Style;
                    }
                    SourceCode += "] ";
                    break;
                case ElementType.Checkbox:
                    SourceCode += " = " + ((Default == 0) ? "false" : "true");
                    SourceCode += "; // ";
                    break;
                case ElementType.ColorWheel:
                    Color c;
                    if (StrDefault.Length == 0 || StrDefault == "PrimaryColor")
                    {
                        c = Color.Black;
                    }
                    else if (StrDefault == "SecondaryColor")
                    {
                        c = Color.White;
                    }
                    else
                    {
                        c = Color.FromName(StrDefault);
                    }

                    string rgb = c.B.ToString() + ", " + c.G.ToString() + ", " + c.R.ToString();
                    string resetstyle = ColorWheelOptions.HasFlag(ColorWheelOptions.NoReset) ? "!" : "";
                    string alphastyle = "";

                    if (ColorWheelOptions.HasFlag(ColorWheelOptions.Alpha))
                    {
                        alphastyle = "?";
                        SourceCode += " = ColorBgra.FromBgra(" + rgb + ", 255)";
                    }
                    else
                    {
                        SourceCode += " = ColorBgra.FromBgr(" + rgb + ")";
                    }

                    SourceCode += "; // ";

                    string config = StrDefault.Trim() + alphastyle + resetstyle;
                    if (config.Length > 0)
                    {
                        SourceCode += "[" + config + "] ";
                    }
                    break;
                case ElementType.PanSlider:
                    SourceCode += " = Pair.Create(" + StrDefault + "); // ";
                    break;
                case ElementType.Textbox:
                case ElementType.MultiLineTextbox:
                    SourceCode += " = \"\"";
                    SourceCode += "; // [" + Max.ToString() + "] ";
                    break;
                case ElementType.DropDown:
                case ElementType.RadioButtons:
                    string listDefault = hasTEnum ? StrDefault : Default.ToString();
                    SourceCode += " = " + listDefault + "; // ";
                    break;
                case ElementType.BinaryPixelOp:
                    SourceCode += " = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal); // ";
                    break;
                case ElementType.FontFamily:
                    SourceCode += " = new FontFamily(\"Arial\"); // ";
                    break;
                case ElementType.ReseedButton:
                    SourceCode += " = 0; // ";
                    break;
                case ElementType.RollBall:
                    SourceCode += " = Tuple.Create<double, double, double>(0.0 , 0.0 , 0.0)";
                    SourceCode += "; // ";
                    break;
                case ElementType.Filename:
                    SourceCode += " = @\"\"; // ";
                    break;
                case ElementType.Uri:
                    SourceCode += " = new Uri(\"" + StrDefault + "\"); // ";
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
                if (BarLoc == -1) return Array.Empty<string>();
                string Options = Name.Substring(BarLoc + 1);
                return Options.Split('|');
            }
            return Array.Empty<string>();
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
        [Description("Integer Slider")]
        IntSlider,
        [Description("Check Box")]
        Checkbox,
        [Description("Color Wheel")]
        ColorWheel,
        [Description("Angle Chooser")]
        AngleChooser,
        [Description("Pan Slider")]
        PanSlider,
        [Description("String")]
        Textbox,
        [Description("Double Slider")]
        DoubleSlider,
        [Description("Drop-Down List Box")]
        DropDown,
        [Description("BlendOp Types")]
        BinaryPixelOp,
        [Description("Font Names")]
        FontFamily,
        [Description("Radio Button List")]
        RadioButtons,
        [Description("Reseed Button")]
        ReseedButton,
        [Description("Multi-Line String")]
        MultiLineTextbox,
        [Description("3D Roll Control")]
        RollBall,
        [Description("Filename Control")]
        Filename,
        [Description("Web Link")]
        Uri
    }

    internal enum SliderStyle
    {
        Default,
        Hue,
        HueCentered,
        Saturation,
        WhiteBlack,
        BlackWhite,
        CyanRed,
        MagentaGreen,
        YellowBlue,
        CyanOrange,
        WhiteRed,
        WhiteGreen,
        WhiteBlue,
    }

    [Flags]
    internal enum ColorWheelOptions
    {
        None,
        Alpha,
        NoReset
    }
}
