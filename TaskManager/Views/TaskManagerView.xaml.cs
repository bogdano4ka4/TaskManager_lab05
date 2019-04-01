using System.Windows.Controls;
using TaskManager.Tools.Navigation;
using TaskManager.ViewModels;

namespace TaskManager.Views
{
   
    public partial class TaskManagerView : UserControl, INavigatable
    {
        public TaskManagerView()
        {
            InitializeComponent();
            DataContext = new TaskManagerViewModel();
        }
    }
}
