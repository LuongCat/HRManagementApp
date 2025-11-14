using System;
using HRManagementApp.DAL;
using HRManagementApp.models;
namespace HRManagementApp.BLL
{
    public class AccountBLL
    {   
        private AccountDAL accDAL;
        public AccountBLL()
        {
            accDAL = new AccountDAL();
        }
        public List<AccountModel> getAllAccountModelBLL()
        {
            return accDAL.getAllAccountModel();
        }

    }
}