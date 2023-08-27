/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright ©2006 Rick Brewster, Tom Jackson. All Rights Reserved.
// Portions Copyright ©2007-2017 BoltBait. All Rights Reserved.
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

using PaintDotNet.Effects;
using System;
using System.Collections.Generic;

namespace PdnCodeLab
{
    public class CodeLabConfigToken : EffectConfigToken
    {
        internal IEffect UserScriptObject;
        internal string UserCode;
        internal List<Exception> LastExceptions;
        internal List<string> Output;
        internal string ScriptName;
        internal string ScriptPath;
        internal bool Dirty;
        internal EffectConfigToken PreviewToken;
        internal IReadOnlyCollection<int> Bookmarks;
        internal ProjectType ProjectType;

        internal CodeLabConfigToken()
        {
            UserScriptObject = null;
            UserCode = "";
            LastExceptions = new List<Exception>();
            Output = new List<string>();
            ScriptName = "Untitled";
            ScriptPath = "";
            Dirty = false;
            PreviewToken = null;
            Bookmarks = Array.Empty<int>();
            ProjectType = ProjectType.Default;
        }

        public override object Clone()
        {
            return new CodeLabConfigToken
            {
                UserCode = this.UserCode,
                UserScriptObject = this.UserScriptObject,
                LastExceptions = this.LastExceptions, //Reference copy INTENDED.
                Output = this.Output,
                ScriptName = this.ScriptName,
                ScriptPath = this.ScriptPath,
                Dirty = this.Dirty,
                PreviewToken = this.PreviewToken,
                Bookmarks = this.Bookmarks,
                ProjectType = this.ProjectType
            };
        }
    }
}
