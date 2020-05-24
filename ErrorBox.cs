using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal sealed class ErrorBox : ListBox
    {
        internal IEnumerable<Error> Errors => Items.OfType<Error>();
        internal int ErrorCount => Errors.Count(error => !error.IsWarning);
        internal int WarningCount => Errors.Count(error => error.IsWarning);
        internal bool HasErrors => Errors.Any(error => !error.IsWarning);
        internal Error SelectedError => (Error)SelectedItem;

        internal void AddError(Error error) => Items.Add(error);
        internal void ClearErrors() => Items.Clear();
    }
}
