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

            var trickEndVM = new TrickEndViewModel();
            //trickEndPopup.DataContext = trickEndVM;
            KeyDown += new KeyEventHandler(HandleEnter);

            //round
            model = new BaseGameViewModel(gameManager, infoPanelVM, trickEndVM);
            model.GameStateChanged += OnGameStateChanged;

            Whist.DataContext = model;
            BiddingView.DataContext = new BiddingViewModel(model);
        }

        private void HandleEnter(object sender, KeyEventArgs e)
        {
            /*if(ContinueText.Visibility == Visibility.Visible && e.Key == Key.Enter)
            {
                ContinueText.Visibility = Visibility.Hidden;*/
                //model.EndTrick();
            //}
        }

        private void OnGameStateChanged(GameState gameState) {
            if (gameState == GameState.BIDDING)
                popup.IsOpen = true;
            else
                popup.IsOpen = false;
            if (gameState == GameState.ENDTRICK) {
                TrickWinner.Content = model.LatestTrickWinner + " won the trick!";
                trickEndPopup.IsOpen = true;
                StartTrickButton.Focus();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            trickEndPopup.IsOpen = false;
            model.StartNewTrick();
            model.AI_PlaysCards();
        }
    }
}
