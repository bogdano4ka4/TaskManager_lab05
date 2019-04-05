using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Tools.Managers;
using TaskManager.Tools.Navigation;
using TaskManager.ViewModels;
namespace TaskManager
{
    public partial class MainWindow : Window, IContentOwner
    {
        public ContentControl ContentControl => _contentControl;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TaskManagerViewModel();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
           
            NavigationManager.Instance.Initialize(new InitializationNavigationModel(this));
            NavigationManager.Instance.Navigate(ViewType.TaskManager,null);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            StationManager.CloseApp();
        }
    }
}
