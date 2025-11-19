using System.Security.AccessControl;
using HRManagementApp.DAL;
using Mysqlx.Crud;
using HRManagementApp.models;
namespace HRManagementApp.BLL;

public class VaiTroNhanVienService
{
    private readonly VaiTroNhanVienReponsitory _vaiTroNhanVienReponsitory;

    public VaiTroNhanVienService()
    {
        var nhanVienRepo = new NhanVienRepository();
        _vaiTroNhanVienReponsitory = new VaiTroNhanVienReponsitory(nhanVienRepo);
        nhanVienRepo.vaiTroNhanVien = _vaiTroNhanVienReponsitory; 
    }

    public List<NhanVien> GetNhanVienOfPhongBan(int maPB)
    {
        return _vaiTroNhanVienReponsitory.GetNhanVienOfPhongBan(maPB);
    }

    public bool UpdateVaiTroNhanVien(VaiTroNhanVien vtnv)
    {
        return _vaiTroNhanVienReponsitory.UpdateVaiTroNhanVien(vtnv);
    }

    public bool InsertVaiTroNhanVien(VaiTroNhanVien vtnv)
    {
        return _vaiTroNhanVienReponsitory.InsertVaiTroNhanVien(vtnv);
    }

    public List<NhanVien> GetEmployeesNotInDepartment(int maPB)
    {
        return _vaiTroNhanVienReponsitory.GetEmployeesNotInDepartment(maPB);
    }

    public bool RemoveEmployeeFromDepartment(int MaNV, int MaPB)
    {
        return _vaiTroNhanVienReponsitory.RemoveEmployeeFromDepartment(MaNV, MaPB);
    }
}

