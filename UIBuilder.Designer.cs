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
            this.MoveUp = new System.Windows.Forms.Button();
            this.MoveDown = new System.Windows.Forms.Button();
            this.Delete = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ControlName = new System.Windows.Forms.TextBox();
            this.ControlMin = new System.Windows.Forms.TextBox();
            this.ControlMax = new System.Windows.Forms.TextBox();
            this.ControlDef = new System.Windows.Forms.TextBox();
            this.OptionsText = new System.Windows.Forms.TextBox();
            this.UpdateBtn = new System.Windows.Forms.Button();
            this.Add = new System.Windows.Forms.Button();
            this.ControlID = new System.Windows.Forms.TextBox();
            this.ControlListView = new System.Windows.Forms.ListView();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.PreviewButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.controlTabPage = new System.Windows.Forms.TabPage();
            this.ControlType = new System.Windows.Forms.ComboBox();
            this.labelIdent = new System.Windows.Forms.Label();
            this.MinimumLabel = new System.Windows.Forms.Label();
            this.DefaultLabel = new System.Windows.Forms.Label();
            this.MaximumLabel = new System.Windows.Forms.Label();
            this.OptionsLabel = new System.Windows.Forms.Label();
            this.rbEnabled = new System.Windows.Forms.RadioButton();
            this.rbEnabledWhen = new System.Windows.Forms.RadioButton();
            this.enabledWhenField = new System.Windows.Forms.ComboBox();
            this.enabledWhenCondition = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.StyleLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ControlStyle = new System.Windows.Forms.ComboBox();
            this.DefaultColorComboBox = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.controlTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(12, 10);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(118, 13);
            this.label22.TabIndex = 10;
            this.label22.Text = "User Interface Controls:";
            // 
            // MoveUp
            // 
            this.MoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MoveUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveUp.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MoveUp.Location = new System.Drawing.Point(390, 28);
            this.MoveUp.Name = "MoveUp";
            this.MoveUp.Size = new System.Drawing.Size(23, 23);
            this.MoveUp.TabIndex = 1;
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
            this.MoveDown.Location = new System.Drawing.Point(390, 56);
            this.MoveDown.Name = "MoveDown";
            this.MoveDown.Size = new System.Drawing.Size(23, 23);
            this.MoveDown.TabIndex = 2;
            this.MoveDown.Text = "▼";
            this.toolTip1.SetToolTip(this.MoveDown, "Move Down");
            this.MoveDown.UseVisualStyleBackColor = true;
            this.MoveDown.Click += new System.EventHandler(this.MoveDown_Click);
            // 
            // Delete
            // 
            this.Delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Delete.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Delete.ForeColor = System.Drawing.Color.Red;
            this.Delete.Location = new System.Drawing.Point(390, 100);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(23, 23);
            this.Delete.TabIndex = 3;
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
            this.Cancel.Location = new System.Drawing.Point(338, 398);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 9;
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
            this.OK.Location = new System.Drawing.Point(257, 398);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 8;
            this.OK.Text = "OK";
            this.toolTip1.SetToolTip(this.OK, "Save your changes");
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // ControlName
            // 
            this.ControlName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlName.Location = new System.Drawing.Point(75, 7);
            this.ControlName.Name = "ControlName";
            this.ControlName.Size = new System.Drawing.Size(158, 20);
            this.ControlName.TabIndex = 1;
            this.toolTip1.SetToolTip(this.ControlName, "Name to be displayed in the UI to your users");
            this.ControlName.TextChanged += new System.EventHandler(this.ControlName_TextChanged);
            // 
            // ControlMin
            // 
            this.ControlMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlMin.Location = new System.Drawing.Point(75, 63);
            this.ControlMin.Name = "ControlMin";
            this.ControlMin.Size = new System.Drawing.Size(49, 20);
            this.ControlMin.TabIndex = 10;
            this.ControlMin.Text = "0";
            this.toolTip1.SetToolTip(this.ControlMin, "Minimum value");
            this.ControlMin.Leave += new System.EventHandler(this.ControlMin_Leave);
            // 
            // ControlMax
            // 
            this.ControlMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlMax.Location = new System.Drawing.Point(336, 63);
            this.ControlMax.Name = "ControlMax";
            this.ControlMax.Size = new System.Drawing.Size(55, 20);
            this.ControlMax.TabIndex = 15;
            this.ControlMax.Text = "100";
            this.toolTip1.SetToolTip(this.ControlMax, "Maximum value");
            this.ControlMax.Leave += new System.EventHandler(this.ControlMax_Leave);
            // 
            // ControlDef
            // 
            this.ControlDef.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlDef.Location = new System.Drawing.Point(177, 63);
            this.ControlDef.Name = "ControlDef";
            this.ControlDef.Size = new System.Drawing.Size(56, 20);
            this.ControlDef.TabIndex = 9;
            this.ControlDef.Text = "0";
            this.toolTip1.SetToolTip(this.ControlDef, "Default value");
            this.ControlDef.Leave += new System.EventHandler(this.ControlDef_Leave);
            // 
            // OptionsText
            // 
            this.OptionsText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OptionsText.Location = new System.Drawing.Point(75, 63);
            this.OptionsText.Name = "OptionsText";
            this.OptionsText.Size = new System.Drawing.Size(314, 20);
            this.OptionsText.TabIndex = 11;
            this.toolTip1.SetToolTip(this.OptionsText, "Separate options with the vertical bar character (|)");
            this.OptionsText.Visible = false;
            // 
            // UpdateBtn
            // 
            this.UpdateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.UpdateBtn.Location = new System.Drawing.Point(357, 246);
            this.UpdateBtn.Name = "UpdateBtn";
            this.UpdateBtn.Size = new System.Drawing.Size(56, 23);
            this.UpdateBtn.TabIndex = 5;
            this.UpdateBtn.Text = "Update";
            this.toolTip1.SetToolTip(this.UpdateBtn, "Update the selected control");
            this.UpdateBtn.UseVisualStyleBackColor = true;
            this.UpdateBtn.Click += new System.EventHandler(this.Update_Click);
            // 
            // Add
            // 
            this.Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Add.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Add.Location = new System.Drawing.Point(314, 246);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(38, 23);
            this.Add.TabIndex = 4;
            this.Add.Text = "Add";
            this.toolTip1.SetToolTip(this.Add, "Add new control");
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // ControlID
            // 
            this.ControlID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ControlID.Location = new System.Drawing.Point(290, 7);
            this.ControlID.Name = "ControlID";
            this.ControlID.Size = new System.Drawing.Size(101, 20);
            this.ControlID.TabIndex = 3;
            this.toolTip1.SetToolTip(this.ControlID, "Variable name to be used in your script");
            this.ControlID.TextChanged += new System.EventHandler(this.ControlID_TextChanged);
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
            this.ControlListView.Location = new System.Drawing.Point(13, 29);
            this.ControlListView.MultiSelect = false;
            this.ControlListView.Name = "ControlListView";
            this.ControlListView.ShowGroups = false;
            this.ControlListView.Size = new System.Drawing.Size(371, 211);
            this.ControlListView.TabIndex = 0;
            this.ControlListView.UseCompatibleStateImageBehavior = false;
            this.ControlListView.View = System.Windows.Forms.View.List;
            this.ControlListView.SelectedIndexChanged += new System.EventHandler(this.ControlListView_SelectedIndexChanged);
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // PreviewButton
            // 
            this.PreviewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PreviewButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PreviewButton.Location = new System.Drawing.Point(12, 398);
            this.PreviewButton.Name = "PreviewButton";
            this.PreviewButton.Size = new System.Drawing.Size(75, 23);
            this.PreviewButton.TabIndex = 7;
            this.PreviewButton.Text = "Preview UI";
            this.PreviewButton.UseVisualStyleBackColor = true;
            this.PreviewButton.Click += new System.EventHandler(this.PreviewButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.controlTabPage);
            this.tabControl1.Location = new System.Drawing.Point(13, 252);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(401, 139);
            this.tabControl1.TabIndex = 6;
            // 
            // controlTabPage
            // 
            this.controlTabPage.Controls.Add(this.ControlType);
            this.controlTabPage.Controls.Add(this.labelIdent);
            this.controlTabPage.Controls.Add(this.MinimumLabel);
            this.controlTabPage.Controls.Add(this.ControlID);
            this.controlTabPage.Controls.Add(this.DefaultLabel);
            this.controlTabPage.Controls.Add(this.MaximumLabel);
            this.controlTabPage.Controls.Add(this.OptionsLabel);
            this.controlTabPage.Controls.Add(this.rbEnabled);
            this.controlTabPage.Controls.Add(this.ControlMax);
            this.controlTabPage.Controls.Add(this.rbEnabledWhen);
            this.controlTabPage.Controls.Add(this.ControlMin);
            this.controlTabPage.Controls.Add(this.enabledWhenField);
            this.controlTabPage.Controls.Add(this.enabledWhenCondition);
            this.controlTabPage.Controls.Add(this.label3);
            this.controlTabPage.Controls.Add(this.StyleLabel);
            this.controlTabPage.Controls.Add(this.label1);
            this.controlTabPage.Controls.Add(this.ControlStyle);
            this.controlTabPage.Controls.Add(this.ControlName);
            this.controlTabPage.Controls.Add(this.DefaultColorComboBox);
            this.controlTabPage.Controls.Add(this.ControlDef);
            this.controlTabPage.Controls.Add(this.OptionsText);
            this.controlTabPage.ForeColor = System.Drawing.SystemColors.WindowText;
            this.controlTabPage.Location = new System.Drawing.Point(4, 22);
            this.controlTabPage.Margin = new System.Windows.Forms.Padding(0);
            this.controlTabPage.Name = "controlTabPage";
            this.controlTabPage.Size = new System.Drawing.Size(393, 113);
            this.controlTabPage.TabIndex = 0;
            this.controlTabPage.Text = "Control";
            this.controlTabPage.UseVisualStyleBackColor = true;
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
            "Filename Control",
            "Web Link"});
            this.ControlType.Location = new System.Drawing.Point(75, 34);
            this.ControlType.MaxDropDownItems = 12;
            this.ControlType.Name = "ControlType";
            this.ControlType.Size = new System.Drawing.Size(158, 21);
            this.ControlType.TabIndex = 5;
            this.ControlType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ControlType_DrawItem);
            this.ControlType.SelectedIndexChanged += new System.EventHandler(this.ControlType_SelectedIndexChanged);
            // 
            // labelIdent
            // 
            this.labelIdent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelIdent.AutoSize = true;
            this.labelIdent.Location = new System.Drawing.Point(239, 10);
            this.labelIdent.Name = "labelIdent";
            this.labelIdent.Size = new System.Drawing.Size(48, 13);
            this.labelIdent.TabIndex = 2;
            this.labelIdent.Text = "Variable:";
            // 
            // MinimumLabel
            // 
            this.MinimumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MinimumLabel.AutoSize = true;
            this.MinimumLabel.Location = new System.Drawing.Point(21, 67);
            this.MinimumLabel.Name = "MinimumLabel";
            this.MinimumLabel.Size = new System.Drawing.Size(51, 13);
            this.MinimumLabel.TabIndex = 8;
            this.MinimumLabel.Text = "Minimum:";
            // 
            // DefaultLabel
            // 
            this.DefaultLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DefaultLabel.AutoSize = true;
            this.DefaultLabel.Location = new System.Drawing.Point(130, 67);
            this.DefaultLabel.Name = "DefaultLabel";
            this.DefaultLabel.Size = new System.Drawing.Size(44, 13);
            this.DefaultLabel.TabIndex = 12;
            this.DefaultLabel.Text = "Default:";
            // 
            // MaximumLabel
            // 
            this.MaximumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MaximumLabel.AutoSize = true;
            this.MaximumLabel.Location = new System.Drawing.Point(279, 67);
            this.MaximumLabel.Name = "MaximumLabel";
            this.MaximumLabel.Size = new System.Drawing.Size(54, 13);
            this.MaximumLabel.TabIndex = 14;
            this.MaximumLabel.Text = "Maximum:";
            // 
            // OptionsLabel
            // 
            this.OptionsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OptionsLabel.AutoSize = true;
            this.OptionsLabel.Location = new System.Drawing.Point(26, 67);
            this.OptionsLabel.Name = "OptionsLabel";
            this.OptionsLabel.Size = new System.Drawing.Size(46, 13);
            this.OptionsLabel.TabIndex = 9;
            this.OptionsLabel.Text = "Options:";
            this.OptionsLabel.Visible = false;
            // 
            // rbEnabled
            // 
            this.rbEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbEnabled.AutoSize = true;
            this.rbEnabled.Checked = true;
            this.rbEnabled.Location = new System.Drawing.Point(5, 91);
            this.rbEnabled.Name = "rbEnabled";
            this.rbEnabled.Size = new System.Drawing.Size(64, 17);
            this.rbEnabled.TabIndex = 16;
            this.rbEnabled.TabStop = true;
            this.rbEnabled.Text = "Enabled";
            this.rbEnabled.UseVisualStyleBackColor = true;
            this.rbEnabled.CheckedChanged += new System.EventHandler(this.rbEnabled_CheckedChanged);
            // 
            // rbEnabledWhen
            // 
            this.rbEnabledWhen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbEnabledWhen.AutoSize = true;
            this.rbEnabledWhen.Location = new System.Drawing.Point(69, 91);
            this.rbEnabledWhen.Name = "rbEnabledWhen";
            this.rbEnabledWhen.Size = new System.Drawing.Size(93, 17);
            this.rbEnabledWhen.TabIndex = 17;
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
            this.enabledWhenField.Location = new System.Drawing.Point(163, 90);
            this.enabledWhenField.Name = "enabledWhenField";
            this.enabledWhenField.Size = new System.Drawing.Size(113, 21);
            this.enabledWhenField.TabIndex = 18;
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
            this.enabledWhenCondition.Location = new System.Drawing.Point(282, 90);
            this.enabledWhenCondition.Name = "enabledWhenCondition";
            this.enabledWhenCondition.Size = new System.Drawing.Size(109, 21);
            this.enabledWhenCondition.TabIndex = 19;
            this.enabledWhenCondition.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.enabledWhenCondition_DrawItem);
            this.enabledWhenCondition.SelectedIndexChanged += new System.EventHandler(this.enabledWhenCondition_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Control type:";
            // 
            // StyleLabel
            // 
            this.StyleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StyleLabel.AutoSize = true;
            this.StyleLabel.Location = new System.Drawing.Point(239, 39);
            this.StyleLabel.Name = "StyleLabel";
            this.StyleLabel.Size = new System.Drawing.Size(33, 13);
            this.StyleLabel.TabIndex = 6;
            this.StyleLabel.Text = "Style:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-1, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Display name:";
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
            this.ControlStyle.Location = new System.Drawing.Point(275, 34);
            this.ControlStyle.Name = "ControlStyle";
            this.ControlStyle.Size = new System.Drawing.Size(116, 23);
            this.ControlStyle.TabIndex = 7;
            this.ControlStyle.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ControlStyle_DrawItem);
            this.ControlStyle.SelectedIndexChanged += new System.EventHandler(this.ControlStyle_SelectedIndexChanged);
            // 
            // DefaultColorComboBox
            // 
            this.DefaultColorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DefaultColorComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.DefaultColorComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DefaultColorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DefaultColorComboBox.DropDownWidth = 150;
            this.DefaultColorComboBox.Location = new System.Drawing.Point(177, 63);
            this.DefaultColorComboBox.MaxDropDownItems = 10;
            this.DefaultColorComboBox.Name = "DefaultColorComboBox";
            this.DefaultColorComboBox.Size = new System.Drawing.Size(92, 21);
            this.DefaultColorComboBox.TabIndex = 13;
            this.DefaultColorComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.DefaultColorComboBox_DrawItem);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(412, 270);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(17, 125);
            this.panel1.TabIndex = 11;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Location = new System.Drawing.Point(12, 390);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(407, 5);
            this.panel2.TabIndex = 12;
            // 
            // UIBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(426, 430);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PreviewButton);
            this.Controls.Add(this.ControlListView);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.UpdateBtn);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.MoveDown);
            this.Controls.Add(this.MoveUp);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(442, 381);
            this.Name = "UIBuilder";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User Interface Designer";
            this.tabControl1.ResumeLayout(false);
            this.controlTabPage.ResumeLayout(false);
            this.controlTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button MoveUp;
        private System.Windows.Forms.Button MoveDown;
        private System.Windows.Forms.Button Delete;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ListView ControlListView;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.Button PreviewButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage controlTabPage;
        private System.Windows.Forms.ComboBox ControlType;
        private System.Windows.Forms.Label labelIdent;
        private System.Windows.Forms.Label MinimumLabel;
        private System.Windows.Forms.TextBox ControlID;
        private System.Windows.Forms.Label DefaultLabel;
        private System.Windows.Forms.Label MaximumLabel;
        private System.Windows.Forms.Label OptionsLabel;
        private System.Windows.Forms.TextBox OptionsText;
        private System.Windows.Forms.ComboBox DefaultColorComboBox;
        private System.Windows.Forms.TextBox ControlDef;
        private System.Windows.Forms.RadioButton rbEnabled;
        private System.Windows.Forms.TextBox ControlMax;
        private System.Windows.Forms.RadioButton rbEnabledWhen;
        private System.Windows.Forms.TextBox ControlMin;
        private System.Windows.Forms.ComboBox enabledWhenField;
        private System.Windows.Forms.ComboBox enabledWhenCondition;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label StyleLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ControlStyle;
        private System.Windows.Forms.TextBox ControlName;
        private System.Windows.Forms.Button UpdateBtn;
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}