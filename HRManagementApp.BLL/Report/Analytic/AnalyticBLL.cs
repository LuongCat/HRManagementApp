using HRManagementApp.DAL;
using HRManagementApp.models;
namespace  HRManagementApp.BLL
{
    public class AnalyticsBLL
    {

        public AnalyticsModel GetDashboardAnalytics()
        {
            // Gọi DAL (giả sử bạn viết hàm trên trong AttendanceDAL hoặc ReportDAL)
            return new AnalyticsDAL().GetAnalyticsSummary();
        }
    }
};

