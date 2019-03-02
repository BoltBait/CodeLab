/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright ©2006 Rick Brewster, Tom Jackson. All Rights Reserved.
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

using System.CodeDom.Compiler;

namespace PaintDotNet.Effects
{
    /// <summary>
    /// Container for a CompilerError object, overrides ToString to a more readable form.
    /// </summary>
    internal class ScriptError : CompilerError
    {
        private readonly bool isInternal;

        internal ScriptError(CompilerError error)
        {
            this.Column = error.Column - ScriptBuilder.ColumnOffset;
            this.ErrorNumber = error.ErrorNumber;
            this.ErrorText = error.ErrorText;
            this.FileName = error.FileName;
            this.IsWarning = error.IsWarning;
            this.Line = error.Line - ScriptBuilder.LineOffset;
            this.isInternal = false;
        }

        internal ScriptError(string internalError)
        {
            this.ErrorText = internalError;
            this.isInternal = true;
        }

        public override string ToString()
        {
            if (this.isInternal)
            {
                return $"Internal Error: {this.ErrorText}";
            }

            return $"{(this.IsWarning ? "Warning" : "Error")} at line {this.Line}: {this.ErrorText} ({this.ErrorNumber})";
        }
    }
}
