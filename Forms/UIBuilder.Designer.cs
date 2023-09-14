namespace PdnCodeLab
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
            components = new System.ComponentModel.Container();
            label22 = new System.Windows.Forms.Label();
            MoveUp = new System.Windows.Forms.Button();
            MoveDown = new System.Windows.Forms.Button();
            Delete = new System.Windows.Forms.Button();
            Cancel = new System.Windows.Forms.Button();
            OK = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            ControlName = new System.Windows.Forms.TextBox();
            ControlMin = new System.Windows.Forms.TextBox();
            ControlMax = new System.Windows.Forms.TextBox();
            ControlDef = new System.Windows.Forms.TextBox();
            OptionsText = new System.Windows.Forms.TextBox();
            UpdateBtn = new System.Windows.Forms.Button();
            Add = new System.Windows.Forms.Button();
            ControlID = new System.Windows.Forms.TextBox();
            ControlListView = new System.Windows.Forms.ListView();
            imgList = new System.Windows.Forms.ImageList(components);
            PreviewButton = new System.Windows.Forms.Button();
            tabControl1 = new System.Windows.Forms.TabControl();
            controlTabPage = new System.Windows.Forms.TabPage();
            ControlType = new ControlTypeComboBox();
            labelIdent = new System.Windows.Forms.Label();
            MinimumLabel = new System.Windows.Forms.Label();
            DefaultLabel = new System.Windows.Forms.Label();
            MaximumLabel = new System.Windows.Forms.Label();
            OptionsLabel = new System.Windows.Forms.Label();
            rbEnabled = new System.Windows.Forms.RadioButton();
            rbEnabledWhen = new System.Windows.Forms.RadioButton();
            enabledWhenField = new System.Windows.Forms.ComboBox();
            enabledWhenCondition = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            StyleLabel = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            ControlStyle = new System.Windows.Forms.ComboBox();
            DefaultColorComboBox = new System.Windows.Forms.ComboBox();
            panel1 = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            tabControl1.SuspendLayout();
            controlTabPage.SuspendLayout();
            SuspendLayout();
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new System.Drawing.Point(12, 10);
            label22.Name = "label22";
            label22.Size = new System.Drawing.Size(130, 15);
            label22.TabIndex = 10;
            label22.Text = "User Interface Controls:";
            // 
            // MoveUp
            // 
            MoveUp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            MoveUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
            MoveUp.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            MoveUp.Location = new System.Drawing.Point(410, 28);
            MoveUp.Name = "MoveUp";
            MoveUp.Size = new System.Drawing.Size(23, 23);
            MoveUp.TabIndex = 1;
            MoveUp.Text = "▲";
            toolTip1.SetToolTip(MoveUp, "Move Up");
            MoveUp.UseVisualStyleBackColor = true;
            MoveUp.Click += MoveUp_Click;
            // 
            // MoveDown
            // 
            MoveDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            MoveDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
            MoveDown.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            MoveDown.Location = new System.Drawing.Point(410, 56);
            MoveDown.Name = "MoveDown";
            MoveDown.Size = new System.Drawing.Size(23, 23);
            MoveDown.TabIndex = 2;
            MoveDown.Text = "▼";
            toolTip1.SetToolTip(MoveDown, "Move Down");
            MoveDown.UseVisualStyleBackColor = true;
            MoveDown.Click += MoveDown_Click;
            // 
            // Delete
            // 
            Delete.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            Delete.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            Delete.ForeColor = System.Drawing.Color.Red;
            Delete.Location = new System.Drawing.Point(410, 100);
            Delete.Name = "Delete";
            Delete.Size = new System.Drawing.Size(23, 23);
            Delete.TabIndex = 3;
            Delete.Text = "✗";
            toolTip1.SetToolTip(Delete, "Delete");
            Delete.UseVisualStyleBackColor = true;
            Delete.Click += Delete_Click;
            // 
            // Cancel
            // 
            Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            Cancel.Location = new System.Drawing.Point(358, 392);
            Cancel.Name = "Cancel";
            Cancel.Size = new System.Drawing.Size(76, 24);
            Cancel.TabIndex = 9;
            Cancel.Text = "Cancel";
            toolTip1.SetToolTip(Cancel, "Cancel your changes");
            Cancel.UseVisualStyleBackColor = true;
            // 
            // OK
            // 
            OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            OK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            OK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            OK.Location = new System.Drawing.Point(277, 392);
            OK.Name = "OK";
            OK.Size = new System.Drawing.Size(76, 24);
            OK.TabIndex = 8;
            OK.Text = "OK";
            toolTip1.SetToolTip(OK, "Save your changes");
            OK.UseVisualStyleBackColor = true;
            OK.Click += OK_Click;
            // 
            // ControlName
            // 
            ControlName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ControlName.Location = new System.Drawing.Point(85, 7);
            ControlName.Name = "ControlName";
            ControlName.Size = new System.Drawing.Size(158, 23);
            ControlName.TabIndex = 1;
            toolTip1.SetToolTip(ControlName, "Name to be displayed in the UI to your users");
            ControlName.TextChanged += ControlName_TextChanged;
            // 
            // ControlMin
            // 
            ControlMin.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ControlMin.Location = new System.Drawing.Point(85, 63);
            ControlMin.Name = "ControlMin";
            ControlMin.Size = new System.Drawing.Size(49, 23);
            ControlMin.TabIndex = 10;
            ControlMin.Text = "0";
            toolTip1.SetToolTip(ControlMin, "Minimum value");
            ControlMin.Leave += ControlMin_Leave;
            // 
            // ControlMax
            // 
            ControlMax.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ControlMax.Location = new System.Drawing.Point(356, 63);
            ControlMax.Name = "ControlMax";
            ControlMax.Size = new System.Drawing.Size(55, 23);
            ControlMax.TabIndex = 15;
            ControlMax.Text = "100";
            toolTip1.SetToolTip(ControlMax, "Maximum value");
            ControlMax.Leave += ControlMax_Leave;
            // 
            // ControlDef
            // 
            ControlDef.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ControlDef.Location = new System.Drawing.Point(190, 63);
            ControlDef.Name = "ControlDef";
            ControlDef.Size = new System.Drawing.Size(56, 23);
            ControlDef.TabIndex = 9;
            ControlDef.Text = "0";
            toolTip1.SetToolTip(ControlDef, "Default value");
            ControlDef.Leave += ControlDef_Leave;
            // 
            // OptionsText
            // 
            OptionsText.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            OptionsText.Location = new System.Drawing.Point(85, 63);
            OptionsText.Name = "OptionsText";
            OptionsText.Size = new System.Drawing.Size(325, 23);
            OptionsText.TabIndex = 11;
            toolTip1.SetToolTip(OptionsText, "Separate options with the vertical bar character (|)");
            OptionsText.Visible = false;
            OptionsText.TextChanged += OptionsText_TextChanged;
            // 
            // UpdateBtn
            // 
            UpdateBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            UpdateBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            UpdateBtn.Location = new System.Drawing.Point(374, 233);
            UpdateBtn.Name = "UpdateBtn";
            UpdateBtn.Size = new System.Drawing.Size(59, 24);
            UpdateBtn.TabIndex = 5;
            UpdateBtn.Text = "Update";
            toolTip1.SetToolTip(UpdateBtn, "Update the selected control");
            UpdateBtn.UseVisualStyleBackColor = true;
            UpdateBtn.Click += Update_Click;
            // 
            // Add
            // 
            Add.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            Add.FlatStyle = System.Windows.Forms.FlatStyle.System;
            Add.Location = new System.Drawing.Point(325, 233);
            Add.Name = "Add";
            Add.Size = new System.Drawing.Size(43, 24);
            Add.TabIndex = 4;
            Add.Text = "Add";
            toolTip1.SetToolTip(Add, "Add new control");
            Add.UseVisualStyleBackColor = true;
            Add.Click += Add_Click;
            // 
            // ControlID
            // 
            ControlID.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ControlID.Location = new System.Drawing.Point(310, 7);
            ControlID.Name = "ControlID";
            ControlID.Size = new System.Drawing.Size(101, 23);
            ControlID.TabIndex = 3;
            toolTip1.SetToolTip(ControlID, "Variable name to be used in your script");
            ControlID.TextChanged += ControlID_TextChanged;
            // 
            // ControlListView
            // 
            ControlListView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            ControlListView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ControlListView.AutoArrange = false;
            ControlListView.Location = new System.Drawing.Point(13, 29);
            ControlListView.MultiSelect = false;
            ControlListView.Name = "ControlListView";
            ControlListView.ShowGroups = false;
            ControlListView.Size = new System.Drawing.Size(391, 193);
            ControlListView.SmallImageList = imgList;
            ControlListView.TabIndex = 0;
            ControlListView.UseCompatibleStateImageBehavior = false;
            ControlListView.View = System.Windows.Forms.View.List;
            ControlListView.SelectedIndexChanged += ControlListView_SelectedIndexChanged;
            // 
            // imgList
            // 
            imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            imgList.ImageSize = new System.Drawing.Size(16, 16);
            imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // PreviewButton
            // 
            PreviewButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            PreviewButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            PreviewButton.Location = new System.Drawing.Point(12, 392);
            PreviewButton.Name = "PreviewButton";
            PreviewButton.Size = new System.Drawing.Size(76, 24);
            PreviewButton.TabIndex = 7;
            PreviewButton.Text = "Preview UI";
            PreviewButton.UseVisualStyleBackColor = true;
            PreviewButton.Click += PreviewButton_Click;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(controlTabPage);
            tabControl1.Location = new System.Drawing.Point(12, 239);
            tabControl1.Margin = new System.Windows.Forms.Padding(0);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new System.Drawing.Point(0, 0);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(421, 142);
            tabControl1.TabIndex = 6;
            // 
            // controlTabPage
            // 
            controlTabPage.Controls.Add(ControlType);
            controlTabPage.Controls.Add(labelIdent);
            controlTabPage.Controls.Add(MinimumLabel);
            controlTabPage.Controls.Add(ControlID);
            controlTabPage.Controls.Add(DefaultLabel);
            controlTabPage.Controls.Add(MaximumLabel);
            controlTabPage.Controls.Add(OptionsLabel);
            controlTabPage.Controls.Add(rbEnabled);
            controlTabPage.Controls.Add(ControlMax);
            controlTabPage.Controls.Add(rbEnabledWhen);
            controlTabPage.Controls.Add(ControlMin);
            controlTabPage.Controls.Add(enabledWhenField);
            controlTabPage.Controls.Add(enabledWhenCondition);
            controlTabPage.Controls.Add(label3);
            controlTabPage.Controls.Add(StyleLabel);
            controlTabPage.Controls.Add(label1);
            controlTabPage.Controls.Add(ControlStyle);
            controlTabPage.Controls.Add(ControlName);
            controlTabPage.Controls.Add(DefaultColorComboBox);
            controlTabPage.Controls.Add(ControlDef);
            controlTabPage.Controls.Add(OptionsText);
            controlTabPage.ForeColor = System.Drawing.SystemColors.WindowText;
            controlTabPage.Location = new System.Drawing.Point(4, 24);
            controlTabPage.Margin = new System.Windows.Forms.Padding(0);
            controlTabPage.Name = "controlTabPage";
            controlTabPage.Size = new System.Drawing.Size(413, 114);
            controlTabPage.TabIndex = 0;
            controlTabPage.Text = "Control";
            controlTabPage.UseVisualStyleBackColor = true;
            // 
            // ControlType
            // 
            ControlType.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ControlType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            ControlType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ControlType.FormattingEnabled = true;
            ControlType.Items.AddRange(new object[] { "Don't add items here" });
            ControlType.Location = new System.Drawing.Point(85, 34);
            ControlType.MaxDropDownItems = 12;
            ControlType.Name = "ControlType";
            ControlType.Size = new System.Drawing.Size(158, 24);
            ControlType.TabIndex = 5;
            ControlType.SelectedIndexChanged += ControlType_SelectedIndexChanged;
            // 
            // labelIdent
            // 
            labelIdent.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            labelIdent.AutoSize = true;
            labelIdent.Location = new System.Drawing.Point(250, 10);
            labelIdent.Name = "labelIdent";
            labelIdent.Size = new System.Drawing.Size(51, 15);
            labelIdent.TabIndex = 2;
            labelIdent.Text = "Variable:";
            // 
            // MinimumLabel
            // 
            MinimumLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            MinimumLabel.AutoSize = true;
            MinimumLabel.Location = new System.Drawing.Point(17, 67);
            MinimumLabel.Name = "MinimumLabel";
            MinimumLabel.Size = new System.Drawing.Size(63, 15);
            MinimumLabel.TabIndex = 8;
            MinimumLabel.Text = "Minimum:";
            // 
            // DefaultLabel
            // 
            DefaultLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            DefaultLabel.AutoSize = true;
            DefaultLabel.Location = new System.Drawing.Point(140, 67);
            DefaultLabel.Name = "DefaultLabel";
            DefaultLabel.Size = new System.Drawing.Size(48, 15);
            DefaultLabel.TabIndex = 12;
            DefaultLabel.Text = "Default:";
            // 
            // MaximumLabel
            // 
            MaximumLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            MaximumLabel.AutoSize = true;
            MaximumLabel.Location = new System.Drawing.Point(285, 67);
            MaximumLabel.Name = "MaximumLabel";
            MaximumLabel.Size = new System.Drawing.Size(65, 15);
            MaximumLabel.TabIndex = 14;
            MaximumLabel.Text = "Maximum:";
            // 
            // OptionsLabel
            // 
            OptionsLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            OptionsLabel.AutoSize = true;
            OptionsLabel.Location = new System.Drawing.Point(21, 67);
            OptionsLabel.Name = "OptionsLabel";
            OptionsLabel.Size = new System.Drawing.Size(52, 15);
            OptionsLabel.TabIndex = 9;
            OptionsLabel.Text = "Options:";
            OptionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            OptionsLabel.Visible = false;
            // 
            // rbEnabled
            // 
            rbEnabled.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            rbEnabled.AutoSize = true;
            rbEnabled.Checked = true;
            rbEnabled.Location = new System.Drawing.Point(5, 91);
            rbEnabled.Name = "rbEnabled";
            rbEnabled.Size = new System.Drawing.Size(67, 19);
            rbEnabled.TabIndex = 16;
            rbEnabled.TabStop = true;
            rbEnabled.Text = "Enabled";
            rbEnabled.UseVisualStyleBackColor = true;
            rbEnabled.CheckedChanged += rbEnabled_CheckedChanged;
            // 
            // rbEnabledWhen
            // 
            rbEnabledWhen.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            rbEnabledWhen.AutoSize = true;
            rbEnabledWhen.Location = new System.Drawing.Point(76, 91);
            rbEnabledWhen.Name = "rbEnabledWhen";
            rbEnabledWhen.Size = new System.Drawing.Size(99, 19);
            rbEnabledWhen.TabIndex = 17;
            rbEnabledWhen.TabStop = true;
            rbEnabledWhen.Text = "Enabled when";
            rbEnabledWhen.UseVisualStyleBackColor = true;
            rbEnabledWhen.CheckedChanged += rbEnabledWhen_CheckedChanged;
            // 
            // enabledWhenField
            // 
            enabledWhenField.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            enabledWhenField.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            enabledWhenField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            enabledWhenField.DropDownWidth = 150;
            enabledWhenField.FormattingEnabled = true;
            enabledWhenField.Items.AddRange(new object[] { "Amount99" });
            enabledWhenField.Location = new System.Drawing.Point(180, 90);
            enabledWhenField.Name = "enabledWhenField";
            enabledWhenField.Size = new System.Drawing.Size(113, 24);
            enabledWhenField.TabIndex = 18;
            enabledWhenField.DrawItem += enabledWhenField_DrawItem;
            enabledWhenField.SelectedIndexChanged += enabledWhenField_SelectedIndexChanged;
            // 
            // enabledWhenCondition
            // 
            enabledWhenCondition.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            enabledWhenCondition.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            enabledWhenCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            enabledWhenCondition.FormattingEnabled = true;
            enabledWhenCondition.Items.AddRange(new object[] { "is checked, 0", "not checked, 0" });
            enabledWhenCondition.Location = new System.Drawing.Point(302, 90);
            enabledWhenCondition.Name = "enabledWhenCondition";
            enabledWhenCondition.Size = new System.Drawing.Size(109, 24);
            enabledWhenCondition.TabIndex = 19;
            enabledWhenCondition.DrawItem += enabledWhenCondition_DrawItem;
            enabledWhenCondition.SelectedIndexChanged += enabledWhenCondition_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(4, 37);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(76, 15);
            label3.TabIndex = 4;
            label3.Text = "Control type:";
            // 
            // StyleLabel
            // 
            StyleLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            StyleLabel.AutoSize = true;
            StyleLabel.Location = new System.Drawing.Point(250, 39);
            StyleLabel.Name = "StyleLabel";
            StyleLabel.Size = new System.Drawing.Size(35, 15);
            StyleLabel.TabIndex = 6;
            StyleLabel.Text = "Style:";
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(-1, 10);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(81, 15);
            label1.TabIndex = 0;
            label1.Text = "Display name:";
            // 
            // ControlStyle
            // 
            ControlStyle.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ControlStyle.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            ControlStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ControlStyle.FormattingEnabled = true;
            ControlStyle.ItemHeight = 17;
            ControlStyle.Items.AddRange(new object[] { "Default", "Hue", "Hue Centered", "Saturation", "White-Black", "Black-White", "Cyan-Red", "Magenta-Green", "Yellow-Blue", "Cyan-Orange", "White-Red", "White-Green", "White-Blue" });
            ControlStyle.Location = new System.Drawing.Point(295, 34);
            ControlStyle.Name = "ControlStyle";
            ControlStyle.Size = new System.Drawing.Size(116, 23);
            ControlStyle.TabIndex = 7;
            ControlStyle.DrawItem += ControlStyle_DrawItem;
            ControlStyle.SelectedIndexChanged += ControlStyle_SelectedIndexChanged;
            // 
            // DefaultColorComboBox
            // 
            DefaultColorComboBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            DefaultColorComboBox.BackColor = System.Drawing.SystemColors.Window;
            DefaultColorComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            DefaultColorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            DefaultColorComboBox.DropDownWidth = 150;
            DefaultColorComboBox.Location = new System.Drawing.Point(190, 62);
            DefaultColorComboBox.MaxDropDownItems = 10;
            DefaultColorComboBox.Name = "DefaultColorComboBox";
            DefaultColorComboBox.Size = new System.Drawing.Size(92, 24);
            DefaultColorComboBox.TabIndex = 13;
            DefaultColorComboBox.DrawItem += DefaultColorComboBox_DrawItem;
            // 
            // panel1
            // 
            panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            panel1.Location = new System.Drawing.Point(432, 258);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(17, 131);
            panel1.TabIndex = 11;
            // 
            // panel2
            // 
            panel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel2.Location = new System.Drawing.Point(12, 384);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(427, 5);
            panel2.TabIndex = 12;
            // 
            // UIBuilder
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(446, 424);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(PreviewButton);
            Controls.Add(ControlListView);
            Controls.Add(OK);
            Controls.Add(Cancel);
            Controls.Add(Delete);
            Controls.Add(UpdateBtn);
            Controls.Add(Add);
            Controls.Add(MoveDown);
            Controls.Add(MoveUp);
            Controls.Add(label22);
            Controls.Add(tabControl1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            IconName = "FormDesigner";
            Location = new System.Drawing.Point(0, 0);
            MinimumSize = new System.Drawing.Size(462, 381);
            Name = "UIBuilder";
            Text = "User Interface Designer";
            tabControl1.ResumeLayout(false);
            controlTabPage.ResumeLayout(false);
            controlTabPage.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private PdnCodeLab.ControlTypeComboBox ControlType;
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