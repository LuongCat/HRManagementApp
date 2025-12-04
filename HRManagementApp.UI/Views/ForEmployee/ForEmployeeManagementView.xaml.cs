using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using HRManagementApp.UI.Views.Leave;
using HRManagementApp.models; // Thêm

namespace HRManagementApp.UI.Views;

public partial class ForEmployeeManagementView : UserControl
{
    private Button currentActiveButton = null!;
    
    public ForEmployeeManagementView()
    {
        InitializeComponent();
        SetActiveButton(btnAttendance);
        LoadAttendance();
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
        int maNV = UserSession.MaNV ?? 0;
        ContentArea.Content = new MyLeaveView(maNV);
    }

    private void LoadAttendance()
    {
        int maNV = UserSession.MaNV ?? 0;
        ContentArea.Content = new MyAttendanceView(maNV);
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