using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HRManagementApp.UI.Views.Report
{
    public partial class EmployeeReportView : UserControl
    {
        public EmployeeReportView()
        {
            InitializeComponent();
        }

        // Search employee text box handler
        private void SearchBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                tb.Focus();
                tb.SelectAll();
                e.Handled = true;
            }
        }
    }
}