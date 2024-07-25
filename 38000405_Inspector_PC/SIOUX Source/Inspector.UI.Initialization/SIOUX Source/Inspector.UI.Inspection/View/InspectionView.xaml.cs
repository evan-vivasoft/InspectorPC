/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

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
            DataContext = new InspectionViewModel();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            InspectionViewModel vm = DataContext as InspectionViewModel;
            vm.Dispose();
        }

        private void btnStartContinuousMeasurement_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
