using System.Windows.Controls;
using TaskManager.Models;
using TaskManager.Tools.Navigation;
using TaskManager.ViewModels;

namespace TaskManager.Views
{
    public partial class ShowInfoView : UserControl, INavigatable
    {
        public ShowInfoView(MyProcess selectedProcess)
        {
            InitializeComponent();
            DataContext = new ShowInfoViewModel(selectedProcess);
        }
    }
}
