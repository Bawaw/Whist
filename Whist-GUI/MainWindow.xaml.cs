using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;
using Whist_GUI.ViewLogic;

namespace Whist_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BaseGameViewModel model;

        public MainWindow()
        {
            InitializeComponent();

            GameManager gameManager = new GameManager();

            var infoPanelVM = new InfoPanelViewModel(gameManager);
            InfoPanel.DataContext = infoPanelVM;
            model = new BaseGameViewModel(gameManager, infoPanelVM);

            //Hand.DataContext = model.HandVM;
            Whist.DataContext = model;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
