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
using System.Windows.Shapes;
using Whist.GameLogic.ControlEntities;
using Whist_GUI.ViewLogic;

namespace Whist_GUI
{
    /// <summary>
    /// Interaction logic for BiddingWindow.xaml
    /// </summary>
    public partial class BiddingWindow : Window
    {
        public BiddingWindow(IEnumerable<Action> actions, BaseGameViewModel baseVM)
        {
            InitializeComponent();
            BiddingView.DataContext = new BiddingViewModel(actions, baseVM);
        }
    }
}
