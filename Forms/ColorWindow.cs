/////////////////////////////////////////////////////////////////////////////////
// ColorWindow for CodeLab
// Copyright 2015 Rob Tauler
// Portions Copyright ©2016 BoltBait. All Rights Reserved.
// Portions Copyright ©2016 Jason Wendt. All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PdnCodeLab
{
    internal partial class ColorWindow : ChildFormBase
    {
        internal ColorWindow()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal Color Color
        {
            get => pdnColor1.Color;
            set => pdnColor1.Color = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool ShowAlpha
        {
            get => pdnColor1.ShowAlpha;
            set => pdnColor1.ShowAlpha = value;
        }
    }
}
