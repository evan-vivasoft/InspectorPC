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

            lstInitializationSteps.ItemContainerGenerator.ItemsChanged += (sender, args) => ScrollToBottom();

            DataContext = new InitializationViewModel();
        }

        private void ScrollToBottom()
        {
            var selectedIndex = lstInitializationSteps.Items.Count - 1;

            if (selectedIndex < 0) return;

            lstInitializationSteps.SelectedIndex = selectedIndex;
            lstInitializationSteps.UpdateLayout();
            lstInitializationSteps.ScrollIntoView(lstInitializationSteps.SelectedItem);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as InitializationViewModel;
            if (vm != null)
            {
                vm.Dispose();
            }
        }
    }
}
