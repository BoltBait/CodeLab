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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;

namespace PaintDotNet.Effects
{
    internal sealed class Error : IComparable<Error>
    {
        private readonly ErrorType errorType;

        internal int Line { get; }
        internal int Column { get; }
        internal string ErrorNumber { get; }
        internal string ErrorText { get; }
        internal bool IsWarning { get; }

        internal static Error NewCodeError(Diagnostic diagnostic)
        {
            LinePosition linePosition = diagnostic.Location.GetLineSpan().StartLinePosition;

            return new Error(
                ErrorType.CSharp,
                linePosition.Line - ScriptBuilder.LineOffset + 1,
                linePosition.Character - ScriptBuilder.ColumnOffset + 1,
                diagnostic.Id,
                diagnostic.GetMessage(),
                diagnostic.Severity == DiagnosticSeverity.Warning);
        }

        internal static Error NewShapeError(int line, int column, string errorText)
        {
            return new Error(ErrorType.Xaml, line, column, string.Empty, errorText, false);
        }

        internal static Error NewInternalError(string internalError)
        {
            return new Error(ErrorType.Internal, 0, 0, string.Empty, internalError, false);
        }

        internal static Error NewExceptionError(Exception exception)
        {
            return new Error(ErrorType.Exception, 0, 0, string.Empty, exception.ToString(), false);
        }

        private Error(ErrorType errorType, int line, int column, string errorNumber, string errorText, bool isWarning)
        {
            this.errorType = errorType;
            this.Line = line;
            this.Column = column;
            this.ErrorNumber = errorNumber;
            this.ErrorText = errorText;
            this.IsWarning = isWarning;
        }

        public override string ToString()
        {
            switch (this.errorType)
            {
                case ErrorType.CSharp:
                    return $"{(this.IsWarning ? "Warning" : "Error")} at line {this.Line}: {this.ErrorText} ({this.ErrorNumber})";
                case ErrorType.Xaml:
                    return this.ErrorText;
                case ErrorType.Internal:
                    return $"Internal Error: {this.ErrorText}";
                case ErrorType.Exception:
                    return this.ErrorText;
            }

            return string.Empty;
        }

        public int CompareTo(Error other)
        {
            return this.Line.CompareTo(other.Line);
        }

        private enum ErrorType
        {
            CSharp,
            Xaml,
            Internal,
            Exception
        }
    }
}
