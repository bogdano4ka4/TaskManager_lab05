using System.Windows.Controls;

namespace TaskManager.Tools.Navigation
{
    internal interface IContentOwner
    {
        ContentControl ContentControl { get; }
    }
}
