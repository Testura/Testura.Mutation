using System.ComponentModel;
using Unima.Core;

namespace Unima.Wpf.Shared.Models
{
    public class MutationOperatorGridItem : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public bool IsSelected { get; set; }

        public MutationOperators MutationOperator { get; set; }

        public string Description { get; set; }
    }
}
