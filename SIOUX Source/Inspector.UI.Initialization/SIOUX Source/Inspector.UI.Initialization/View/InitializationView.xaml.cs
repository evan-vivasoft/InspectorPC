/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Windows;
using System.Windows.Controls;
using Inspector.UI.Initialization.ViewModels;

namespace Inspector.UI.Initialization.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class InitializationView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationView"/> class.
        /// </summary>
        public InitializationView()
        {
            InitializeComponent();
            DataContext = new InitializationViewModel();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            InitializationViewModel vm = DataContext as InitializationViewModel;
            vm.Dispose();
        }
    }
}
