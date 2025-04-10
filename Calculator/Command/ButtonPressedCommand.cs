using Calculator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator.Command
{
    class ButtonPressedCommand : ICommand
    {
        #region Fields

        private MainVM _mainVM;
        public event EventHandler? CanExecuteChanged;

        #endregion

        #region Properties

        public MainVM MainVM
        {
            get { return _mainVM; }
            set { _mainVM = value; }
        }

        #endregion

        #region Constructor

        public ButtonPressedCommand(MainVM mainVM)
        {
            MainVM = mainVM;
        }

        #endregion

        #region Command Methods

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            MainVM.GetPressedButton(parameter.ToString());
        }

        #endregion
    }
}
