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
            trickEndPopup.DataContext = trickEndVM;
            KeyDown += new KeyEventHandler(HandleEnter);

            //round
            model = new BaseGameViewModel(gameManager, infoPanelVM, trickEndVM);
            model.GameStateChanged += start_end_InitialBiddingPhase;

            Whist.DataContext = model;
            BiddingView.DataContext = new BiddingViewModel(model);
        }

        private void HandleEnter(object sender, KeyEventArgs e)
        {
            if(ContinueText.Visibility == Visibility.Visible && e.Key == Key.Enter)
            {
                ContinueText.Visibility = Visibility.Hidden;
                model.EndTrick();
            }
        }

        private void start_end_InitialBiddingPhase(GameState gameState) {
            if (gameState == GameState.BIDDING)
                popup.IsOpen = true;
            else
                popup.IsOpen = false;

            //TransformGroup transformGroup = new TransformGroup();
            //RotateTransform rotate1 = new RotateTransform();
            //rotate1.Angle = 90;
            //transformGroup.Children.Add(rotate1);
            //Comp1SingleCard.i
            //ImageBrush brush = new ImageBrush("Textures\red_back.png");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
