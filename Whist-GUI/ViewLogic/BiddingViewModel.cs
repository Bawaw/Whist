using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI.ViewLogic
{
    public class BiddingViewModel : INotifyPropertyChanged
    {
        private BaseGameViewModel baseVM;
        private readonly BidCommand bidCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand BidCmd { get { return bidCommand; } }

        public BiddingViewModel(BaseGameViewModel baseVM)
        {
            bidCommand = new BidCommand(this);
            this.baseVM = baseVM;

            baseVM.PropertyChanged += (sender, e) => PropChanged();
        }

        public void PropChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("BiddingActions"));
            }
        }

        public IEnumerable<Action> BiddingActions
        {
            get { return baseVM.BiddingActions; }
        }
        
        private void ReturnResult(Action action)
        {
            baseVM.ChooseAction(action);
        }


        private class BidCommand : ICommand
        {
            BiddingViewModel bidViewModel;

            public BidCommand(BiddingViewModel bidViewModel)
            {
                this.bidViewModel = bidViewModel;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event System.EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                bidViewModel.ReturnResult((Action) parameter);
            }
        }
    }
}
