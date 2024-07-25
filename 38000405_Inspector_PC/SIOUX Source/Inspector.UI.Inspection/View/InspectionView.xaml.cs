/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Windows;
using System.Windows.Controls;
using Inspector.UI.Inspection.ViewModels;

namespace Inspector.UI.Inspection.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class InspectionView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionView"/> class.
        /// </summary>
        public InspectionView()
        {
            InitializeComponent();

            lstInitializationSteps.ItemContainerGenerator.ItemsChanged += (sender, args) => ScrollToBottom();
            
            DataContext = new InspectionViewModel();
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
            var vm = DataContext as InspectionViewModel;
            if (vm != null)
            {
                vm.Dispose();
            }
        }

        private void btnStartContinuousMeasurement_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
