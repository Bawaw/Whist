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

            Player[] players = new Player[]
            {
                new Player("Player"),
                new Player("Comp 1"),
                new Player("Comp 2"),
                new Player("Comp 3")
            };
            Round round = new Round(players);

            var infoPanelVM = new InfoPanelViewModel(round, round.Players[0]);//round.Teams, round.Players[0], round.Trump, round.GameCase.ToString());
            InfoPanel.DataContext = infoPanelVM;
            model = new BaseGameViewModel(round, infoPanelVM);

            //Hand.DataContext = model.HandVM;
            Whist.DataContext = model;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
