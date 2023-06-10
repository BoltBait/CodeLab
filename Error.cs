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

        internal int StartLine { get; }
        internal int StartColumn { get; }
        internal int EndLine { get; }
        internal int EndColumn { get; }
        internal string ErrorNumber { get; }
        internal string ErrorText { get; }
        internal string ErrorUrl { get; }
        internal bool IsWarning { get; }
        internal string FullError => ToString();

        internal static Error NewCodeError(Diagnostic diagnostic)
        {
            LinePositionSpan span = diagnostic.Location.GetLineSpan().Span;

            return new Error(
                ErrorType.CSharp,
                span.Start.Line - ScriptBuilder.LineOffset,
                span.Start.Character - ScriptBuilder.ColumnOffset,
                span.End.Line - ScriptBuilder.LineOffset,
                span.End.Character - ScriptBuilder.ColumnOffset,
                diagnostic.Id,
                diagnostic.GetMessage(),
                diagnostic.Descriptor.HelpLinkUri,
                diagnostic.Severity == DiagnosticSeverity.Warning);
        }

        internal static Error NewShapeError(int line, int column, string errorText)
        {
            return new Error(ErrorType.Xaml, line - 1, column - 1, -1, -1, string.Empty, errorText, null, false);
        }

        internal static Error NewInternalError(string internalError)
        {
            return new Error(ErrorType.Internal, 0, 0, -1, -1, string.Empty, internalError, null, false);
        }

        internal static Error NewExceptionError(Exception exception)
        {
            return new Error(ErrorType.Exception, 0, 0, -1, -1, string.Empty, exception.ToString(), null, false);
        }

        private Error(ErrorType errorType, int startLine, int startColumn, int endLine, int endColumn, string errorNumber, string errorText, string errorUrl, bool isWarning)
        {
            this.errorType = errorType;
            this.StartLine = startLine;
            this.StartColumn = startColumn;
            this.EndLine = endLine;
            this.EndColumn = endColumn;
            this.ErrorNumber = errorNumber;
            this.ErrorText = errorText;
            this.ErrorUrl = errorUrl;
            this.IsWarning = isWarning;
        }

        public override string ToString()
        {
            switch (this.errorType)
            {
                case ErrorType.CSharp:
                    return $"{(this.IsWarning ? "Warning" : "Error")} at line {this.StartLine + 1}: {this.ErrorText} ({this.ErrorNumber})";
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
            return this.StartLine.CompareTo(other.StartLine);
        }

        private enum ErrorType
        {
            CSharp,
            Xaml,
            Internal,
            Exception
        }

        internal static string GetErrorUrl(string errorNumber)
        {
            return $"https://msdn.microsoft.com/query/roslyn.query?appId=roslyn&k=k({errorNumber})";
        }
    }
}
