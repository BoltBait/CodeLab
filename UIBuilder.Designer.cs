namespace PaintDotNet.Effects
{
    partial class UIBuilder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label22 = new System.Windows.Forms.Label();
            this.ControlMax = new System.Windows.Forms.TextBox();
            this.MaximumLabel = new System.Windows.Forms.Label();
            this.ControlDef = new System.Windows.Forms.TextBox();
            this.DefaultLabel = new System.Windows.Forms.Label();
            this.MinimumLabel = new System.Windows.Forms.Label();
            this.ControlMin = new System.Windows.Forms.TextBox();
            this.ControlName = new System.Windows.Forms.TextBox();
            this.ControlType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.MoveUp = new System.Windows.Forms.Button();
            this.MoveDown = new System.Windows.Forms.Button();
            this.Add = new System.Windows.Forms.Button();
            this.Delete = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.UpdateBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.OptionsText = new System.Windows.Forms.TextBox();
            this.OptionsLabel = new System.Windows.Forms.Label();
            this.ControlListView = new System.Windows.Forms.ListView();
            this.DefaultColorComboBox = new System.Windows.Forms.ComboBox();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.rbEnabled = new System.Windows.Forms.RadioButton();
            this.rbEnabledWhen = new System.Windows.Forms.RadioButton();
            this.enabledWhenField = new System.Windows.Forms.ComboBox();
            this.enabledWhenCondition = new System.Windows.Forms.ComboBox();
            this.StyleLabel = new System.Windows.Forms.Label();
            this.ControlStyle = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.PreviewButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(24, 20);
            this.label22.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(238, 25);
            this.label22.TabIndex = 78;
            this.label22.Text = "User Interface Controls:";
            // 
            // ControlMax
            // 
            this.ControlMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlMax.Location = new System.Drawing.Point(618, 726);
            this.ControlMax.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ControlMax.Name = "ControlMax";
            this.ControlMax.Size = new System.Drawing.Size(92, 31);
            this.ControlMax.TabIndex = 11;
            this.ControlMax.Text = "100";
            this.toolTip1.SetToolTip(this.ControlMax, "Maximum value");
            this.ControlMax.Leave += new System.EventHandler(this.ControlMax_Leave);
            // 
            // MaximumLabel
            // 
            this.MaximumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MaximumLabel.AutoSize = true;
            this.MaximumLabel.Location = new System.Drawing.Point(512, 734);
            this.MaximumLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.MaximumLabel.Name = "MaximumLabel";
            this.MaximumLabel.Size = new System.Drawing.Size(110, 25);
            this.MaximumLabel.TabIndex = 10;
            this.MaximumLabel.Text = "Maximum:";
            // 
            // ControlDef
            // 
            this.ControlDef.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlDef.Location = new System.Drawing.Point(372, 726);
            this.ControlDef.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ControlDef.Name = "ControlDef";
            this.ControlDef.Size = new System.Drawing.Size(80, 31);
            this.ControlDef.TabIndex = 9;
            this.ControlDef.Text = "0";
            this.toolTip1.SetToolTip(this.ControlDef, "Default value");
            this.ControlDef.Leave += new System.EventHandler(this.ControlDef_Leave);
            // 
            // DefaultLabel
            // 
            this.DefaultLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DefaultLabel.AutoSize = true;
            this.DefaultLabel.Location = new System.Drawing.Point(264, 734);
            this.DefaultLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.DefaultLabel.Name = "DefaultLabel";
            this.DefaultLabel.Size = new System.Drawing.Size(86, 25);
            this.DefaultLabel.TabIndex = 7;
            this.DefaultLabel.Text = "Default:";
            // 
            // MinimumLabel
            // 
            this.MinimumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MinimumLabel.AutoSize = true;
            this.MinimumLabel.Location = new System.Drawing.Point(78, 734);
            this.MinimumLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.MinimumLabel.Name = "MinimumLabel";
            this.MinimumLabel.Size = new System.Drawing.Size(104, 25);
            this.MinimumLabel.TabIndex = 54;
            this.MinimumLabel.Text = "Minimum:";
            // 
            // ControlMin
            // 
            this.ControlMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlMin.Location = new System.Drawing.Point(176, 726);
            this.ControlMin.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ControlMin.Name = "ControlMin";
            this.ControlMin.Size = new System.Drawing.Size(60, 31);
            this.ControlMin.TabIndex = 6;
            this.ControlMin.Text = "0";
            this.toolTip1.SetToolTip(this.ControlMin, "Minimum value");
            this.ControlMin.Leave += new System.EventHandler(this.ControlMin_Leave);
            // 
            // ControlName
            // 
            this.ControlName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlName.Location = new System.Drawing.Point(112, 614);
            this.ControlName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ControlName.Name = "ControlName";
            this.ControlName.Size = new System.Drawing.Size(296, 31);
            this.ControlName.TabIndex = 4;
            this.toolTip1.SetToolTip(this.ControlName, "Name to be displayed");
            this.ControlName.TextChanged += new System.EventHandler(this.ControlName_TextChanged);
            // 
            // ControlType
            // 
            this.ControlType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ControlType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ControlType.FormattingEnabled = true;
            this.ControlType.Items.AddRange(new object[] {
            "Integer Slider",
            "Check Box",
            "Color Wheel",
            "Angle Chooser",
            "Pan Slider",
            "String",
            "Double Slider",
            "Drop-Down List Box",
            "BlendOp Types",
            "Font Names",
            "Radio Button List",
            "Reseed Button",
            "Multi-Line String",
            "3D Roll Control",
            "Filename Control"});
            this.ControlType.Location = new System.Drawing.Point(112, 668);
            this.ControlType.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ControlType.MaxDropDownItems = 12;
            this.ControlType.Name = "ControlType";
            this.ControlType.Size = new System.Drawing.Size(296, 32);
            this.ControlType.TabIndex = 1;
            this.ControlType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ControlType_DrawItem);
            this.ControlType.SelectedIndexChanged += new System.EventHandler(this.ControlType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 678);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 25);
            this.label3.TabIndex = 51;
            this.label3.Text = "Type:";
            // 
            // MoveUp
            // 
            this.MoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MoveUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveUp.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MoveUp.Location = new System.Drawing.Point(682, 56);
            this.MoveUp.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MoveUp.Name = "MoveUp";
            this.MoveUp.Size = new System.Drawing.Size(46, 46);
            this.MoveUp.TabIndex = 18;
            this.MoveUp.Text = "▲";
            this.toolTip1.SetToolTip(this.MoveUp, "Move Up");
            this.MoveUp.UseVisualStyleBackColor = true;
            this.MoveUp.Click += new System.EventHandler(this.MoveUp_Click);
            // 
            // MoveDown
            // 
            this.MoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MoveDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveDown.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MoveDown.Location = new System.Drawing.Point(682, 112);
            this.MoveDown.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MoveDown.Name = "MoveDown";
            this.MoveDown.Size = new System.Drawing.Size(46, 46);
            this.MoveDown.TabIndex = 19;
            this.MoveDown.Text = "▼";
            this.toolTip1.SetToolTip(this.MoveDown, "Move Down");
            this.MoveDown.UseVisualStyleBackColor = true;
            this.MoveDown.Click += new System.EventHandler(this.MoveDown_Click);
            // 
            // Add
            // 
            this.Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Add.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Add.Location = new System.Drawing.Point(524, 594);
            this.Add.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(76, 46);
            this.Add.TabIndex = 2;
            this.Add.Text = "Add";
            this.toolTip1.SetToolTip(this.Add, "Add new control");
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // Delete
            // 
            this.Delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Delete.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Delete.ForeColor = System.Drawing.Color.Red;
            this.Delete.Location = new System.Drawing.Point(682, 534);
            this.Delete.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(46, 46);
            this.Delete.TabIndex = 20;
            this.Delete.Text = "✗";
            this.toolTip1.SetToolTip(this.Delete, "Delete");
            this.Delete.UseVisualStyleBackColor = true;
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Cancel.Location = new System.Drawing.Point(578, 860);
            this.Cancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(150, 46);
            this.Cancel.TabIndex = 17;
            this.Cancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.Cancel, "Cancel your changes");
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.OK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OK.Location = new System.Drawing.Point(416, 860);
            this.OK.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(150, 46);
            this.OK.TabIndex = 16;
            this.OK.Text = "OK";
            this.toolTip1.SetToolTip(this.OK, "Save your changes");
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // UpdateBtn
            // 
            this.UpdateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.UpdateBtn.Location = new System.Drawing.Point(616, 594);
            this.UpdateBtn.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.UpdateBtn.Name = "UpdateBtn";
            this.UpdateBtn.Size = new System.Drawing.Size(112, 46);
            this.UpdateBtn.TabIndex = 3;
            this.UpdateBtn.Text = "Update";
            this.toolTip1.SetToolTip(this.UpdateBtn, "Update the selected control");
            this.UpdateBtn.UseVisualStyleBackColor = true;
            this.UpdateBtn.Click += new System.EventHandler(this.Update_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 618);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 25);
            this.label1.TabIndex = 87;
            this.label1.Text = "Name:";
            // 
            // OptionsText
            // 
            this.OptionsText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OptionsText.Location = new System.Drawing.Point(136, 726);
            this.OptionsText.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.OptionsText.Name = "OptionsText";
            this.OptionsText.Size = new System.Drawing.Size(574, 31);
            this.OptionsText.TabIndex = 6;
            this.toolTip1.SetToolTip(this.OptionsText, "Separate options with the vertical bar character (|)");
            this.OptionsText.Visible = false;
            // 
            // OptionsLabel
            // 
            this.OptionsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OptionsLabel.AutoSize = true;
            this.OptionsLabel.Location = new System.Drawing.Point(32, 734);
            this.OptionsLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.OptionsLabel.Name = "OptionsLabel";
            this.OptionsLabel.Size = new System.Drawing.Size(92, 25);
            this.OptionsLabel.TabIndex = 88;
            this.OptionsLabel.Text = "Options:";
            this.OptionsLabel.Visible = false;
            // 
            // ControlListView
            // 
            this.ControlListView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.ControlListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlListView.AutoArrange = false;
            this.ControlListView.Font = new System.Drawing.Font("Courier New", 9F);
            this.ControlListView.HideSelection = false;
            this.ControlListView.Location = new System.Drawing.Point(26, 58);
            this.ControlListView.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ControlListView.MultiSelect = false;
            this.ControlListView.Name = "ControlListView";
            this.ControlListView.ShowGroups = false;
            this.ControlListView.Size = new System.Drawing.Size(636, 518);
            this.ControlListView.TabIndex = 0;
            this.ControlListView.UseCompatibleStateImageBehavior = false;
            this.ControlListView.View = System.Windows.Forms.View.List;
            this.ControlListView.SelectedIndexChanged += new System.EventHandler(this.ControlListView_SelectedIndexChanged);
            // 
            // DefaultColorComboBox
            // 
            this.DefaultColorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DefaultColorComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.DefaultColorComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DefaultColorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DefaultColorComboBox.DropDownWidth = 150;
            this.DefaultColorComboBox.Location = new System.Drawing.Point(346, 726);
            this.DefaultColorComboBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.DefaultColorComboBox.MaxDropDownItems = 10;
            this.DefaultColorComboBox.Name = "DefaultColorComboBox";
            this.DefaultColorComboBox.Size = new System.Drawing.Size(158, 32);
            this.DefaultColorComboBox.TabIndex = 8;
            this.DefaultColorComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.DefaultColorComboBox_DrawItem);
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // rbEnabled
            // 
            this.rbEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbEnabled.AutoSize = true;
            this.rbEnabled.Checked = true;
            this.rbEnabled.Location = new System.Drawing.Point(36, 789);
            this.rbEnabled.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbEnabled.Name = "rbEnabled";
            this.rbEnabled.Size = new System.Drawing.Size(122, 29);
            this.rbEnabled.TabIndex = 12;
            this.rbEnabled.TabStop = true;
            this.rbEnabled.Text = "Enabled";
            this.rbEnabled.UseVisualStyleBackColor = true;
            this.rbEnabled.CheckedChanged += new System.EventHandler(this.rbEnabled_CheckedChanged);
            // 
            // rbEnabledWhen
            // 
            this.rbEnabledWhen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbEnabledWhen.AutoSize = true;
            this.rbEnabledWhen.Location = new System.Drawing.Point(164, 789);
            this.rbEnabledWhen.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbEnabledWhen.Name = "rbEnabledWhen";
            this.rbEnabledWhen.Size = new System.Drawing.Size(179, 29);
            this.rbEnabledWhen.TabIndex = 13;
            this.rbEnabledWhen.TabStop = true;
            this.rbEnabledWhen.Text = "Enabled when";
            this.rbEnabledWhen.UseVisualStyleBackColor = true;
            this.rbEnabledWhen.CheckedChanged += new System.EventHandler(this.rbEnabledWhen_CheckedChanged);
            // 
            // enabledWhenField
            // 
            this.enabledWhenField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.enabledWhenField.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.enabledWhenField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.enabledWhenField.DropDownWidth = 150;
            this.enabledWhenField.FormattingEnabled = true;
            this.enabledWhenField.Items.AddRange(new object[] {
            "Amount99"});
            this.enabledWhenField.Location = new System.Drawing.Point(342, 780);
            this.enabledWhenField.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.enabledWhenField.Name = "enabledWhenField";
            this.enabledWhenField.Size = new System.Drawing.Size(154, 32);
            this.enabledWhenField.TabIndex = 14;
            this.enabledWhenField.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.enabledWhenField_DrawItem);
            this.enabledWhenField.SelectedIndexChanged += new System.EventHandler(this.enabledWhenField_SelectedIndexChanged);
            // 
            // enabledWhenCondition
            // 
            this.enabledWhenCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.enabledWhenCondition.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.enabledWhenCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.enabledWhenCondition.FormattingEnabled = true;
            this.enabledWhenCondition.Items.AddRange(new object[] {
            "is checked, 0",
            "not checked, 0"});
            this.enabledWhenCondition.Location = new System.Drawing.Point(512, 780);
            this.enabledWhenCondition.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.enabledWhenCondition.Name = "enabledWhenCondition";
            this.enabledWhenCondition.Size = new System.Drawing.Size(198, 32);
            this.enabledWhenCondition.TabIndex = 15;
            this.enabledWhenCondition.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.enabledWhenCondition_DrawItem);
            this.enabledWhenCondition.SelectedIndexChanged += new System.EventHandler(this.enabledWhenCondition_SelectedIndexChanged);
            // 
            // StyleLabel
            // 
            this.StyleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StyleLabel.AutoSize = true;
            this.StyleLabel.Location = new System.Drawing.Point(424, 678);
            this.StyleLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.StyleLabel.Name = "StyleLabel";
            this.StyleLabel.Size = new System.Drawing.Size(66, 25);
            this.StyleLabel.TabIndex = 89;
            this.StyleLabel.Text = "Style:";
            // 
            // ControlStyle
            // 
            this.ControlStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlStyle.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ControlStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ControlStyle.FormattingEnabled = true;
            this.ControlStyle.ItemHeight = 17;
            this.ControlStyle.Items.AddRange(new object[] {
            "Default",
            "Hue",
            "Hue Centered",
            "Saturation",
            "White-Black",
            "Black-White",
            "Cyan-Red",
            "Magenta-Green",
            "Yellow-Blue",
            "Cyan-Orange",
            "White-Red",
            "White-Green",
            "White-Blue"});
            this.ControlStyle.Location = new System.Drawing.Point(494, 668);
            this.ControlStyle.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ControlStyle.Name = "ControlStyle";
            this.ControlStyle.Size = new System.Drawing.Size(216, 23);
            this.ControlStyle.TabIndex = 5;
            this.ControlStyle.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ControlStyle_DrawItem);
            this.ControlStyle.SelectedIndexChanged += new System.EventHandler(this.ControlStyle_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(26, 596);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(430, 2);
            this.label2.TabIndex = 90;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(26, 840);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(700, 2);
            this.label4.TabIndex = 91;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(454, 650);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(272, 2);
            this.label5.TabIndex = 92;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Location = new System.Drawing.Point(26, 596);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(2, 248);
            this.label6.TabIndex = 93;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Location = new System.Drawing.Point(454, 596);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(2, 58);
            this.label7.TabIndex = 94;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Location = new System.Drawing.Point(724, 650);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(2, 194);
            this.label8.TabIndex = 95;
            // 
            // PreviewButton
            // 
            this.PreviewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PreviewButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PreviewButton.Location = new System.Drawing.Point(24, 860);
            this.PreviewButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PreviewButton.Name = "PreviewButton";
            this.PreviewButton.Size = new System.Drawing.Size(150, 46);
            this.PreviewButton.TabIndex = 96;
            this.PreviewButton.Text = "Preview UI";
            this.PreviewButton.UseVisualStyleBackColor = true;
            this.PreviewButton.Click += new System.EventHandler(this.PreviewButton_Click);
            // 
            // UIBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(750, 924);
            this.Controls.Add(this.PreviewButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ControlDef);
            this.Controls.Add(this.ControlMax);
            this.Controls.Add(this.ControlMin);
            this.Controls.Add(this.ControlType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ControlName);
            this.Controls.Add(this.ControlStyle);
            this.Controls.Add(this.StyleLabel);
            this.Controls.Add(this.enabledWhenCondition);
            this.Controls.Add(this.enabledWhenField);
            this.Controls.Add(this.rbEnabledWhen);
            this.Controls.Add(this.rbEnabled);
            this.Controls.Add(this.DefaultColorComboBox);
            this.Controls.Add(this.ControlListView);
            this.Controls.Add(this.OptionsText);
            this.Controls.Add(this.OptionsLabel);
            this.Controls.Add(this.UpdateBtn);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.MoveDown);
            this.Controls.Add(this.MoveUp);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.MaximumLabel);
            this.Controls.Add(this.DefaultLabel);
            this.Controls.Add(this.MinimumLabel);
            this.Controls.Add(this.label4);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(756, 567);
            this.Name = "UIBuilder";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User Interface Designer";
            this.Load += new System.EventHandler(this.UIBuilder_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox ControlMax;
        private System.Windows.Forms.Label MaximumLabel;
        private System.Windows.Forms.TextBox ControlDef;
        private System.Windows.Forms.Label DefaultLabel;
        private System.Windows.Forms.Label MinimumLabel;
        private System.Windows.Forms.TextBox ControlMin;
        private System.Windows.Forms.TextBox ControlName;
        private System.Windows.Forms.ComboBox ControlType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button MoveUp;
        private System.Windows.Forms.Button MoveDown;
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.Button Delete;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button UpdateBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label OptionsLabel;
        private System.Windows.Forms.TextBox OptionsText;
        private System.Windows.Forms.ListView ControlListView;
        private System.Windows.Forms.ComboBox DefaultColorComboBox;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.RadioButton rbEnabled;
        private System.Windows.Forms.RadioButton rbEnabledWhen;
        private System.Windows.Forms.ComboBox enabledWhenField;
        private System.Windows.Forms.ComboBox enabledWhenCondition;
        private System.Windows.Forms.Label StyleLabel;
        private System.Windows.Forms.ComboBox ControlStyle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button PreviewButton;
    }
}