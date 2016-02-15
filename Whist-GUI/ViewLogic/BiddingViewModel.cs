using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI.ViewLogic
{
    public class BiddingViewModel
    {
        private BaseGameViewModel baseVM;
        private readonly BidCommand bidCommand;
        public ICommand BidCmd { get { return bidCommand; } }

        public BiddingViewModel(IEnumerable<Action> actions, BaseGameViewModel baseVM)
        {
            BiddingActions = actions;
            bidCommand = new BidCommand(this);
            this.baseVM = baseVM;
        }

        public IEnumerable<Action> BiddingActions
        {
            get;
            private set;
        }
        /*
        public Action ChosenAction
        {
            get;
            private set;
        }*/

        private void ReturnResult(Action action)
        {
            //ChosenAction = action;
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
