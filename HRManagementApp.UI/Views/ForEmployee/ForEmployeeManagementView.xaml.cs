using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HRManagementApp.UI.Views.Leave;

namespace HRManagementApp.UI.Views;

public partial class ForEmployeeManagementView : UserControl
{
    
    private Button currentActiveButton = null!;
    
    public ForEmployeeManagementView()
    {
        InitializeComponent();
        SetActiveButton(btnLeave);
        LoadLeave();
    }

    private void BtnLeave_Click(object sender, RoutedEventArgs e)
    {
        SetActiveButton(sender as Button);
        LoadLeave();
    }

    private void BtnAttendance_Click(object sender, RoutedEventArgs e)
    {
        SetActiveButton(sender as Button);
        LoadAttendance();
    }
    
    private void LoadLeave()
    {
        ContentArea.Content = new MyLeaveView(1);
    }

    private void LoadAttendance()
    {
        ContentArea.Content = new MyAttendanceView(1);
    }
    
    private void SetActiveButton(Button activeButton)
    {
        if (currentActiveButton != null)
        {
            currentActiveButton.Background = new SolidColorBrush(Color.FromRgb(232, 232, 232));
            currentActiveButton.FontWeight = FontWeights.Normal;
        }

        if (activeButton != null)
        {
            activeButton.Background = Brushes.White;
            activeButton.FontWeight = FontWeights.SemiBold;
            currentActiveButton = activeButton;
        }
    }
}