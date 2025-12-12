using System.Collections.Generic;
using System.Windows;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class DetailedPermissionWindow : Window
    {
        public DetailedPermissionWindow(string moduleName, List<PermissionItem> details)
        {
            InitializeComponent();
            this.Title = $"Chi tiết: {moduleName}";
            icDetails.ItemsSource = details; // Binding trực tiếp vào list tham chiếu
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}