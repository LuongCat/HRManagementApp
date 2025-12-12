using HRManagementApp.DAL;

namespace  HRManagementApp.models
{
    public class SynsPermissionBLL
    {
        private SystemDAL synsDAL;
        public SynsPermissionBLL()
        {
            synsDAL = new SystemDAL();
        }

        public void SynsPermissionSystem()
        {
            synsDAL.SyncPermissions();
        }
    }
}
