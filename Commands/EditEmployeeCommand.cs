﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfEmployeeDirectory.ViewModels;

namespace WpfEmployeeDirectory.Commands
{
    internal class EditEmployeeCommand : ICommand
    {
        public EditEmployeeCommand(EmployeeViewModel vm)
        {
            _ViewModel = vm;
        }
        private EmployeeViewModel _ViewModel;

        public event System.EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _ViewModel.CanEditEmployee;
        }

        public void Execute(object parameter)
        {
            _ViewModel.ModifyEmployee();
        }
    }
}
