using HRManagementApp.DAL;
using HRManagementApp.models;
namespace HRManagementApp.BLL;

public class PhuCapService
{
    private readonly PhuCapNhanVienRepository _PhuCapNhanVienRepository;

    public PhuCapService()
    {
        _PhuCapNhanVienRepository = new PhuCapNhanVienRepository();
    }

    public List<PhuCapNhanVien> GetAllBenefit()
    {
        return _PhuCapNhanVienRepository.GetAllBenefit();
    }

    public bool AddPhuCap(PhuCapNhanVien pc)
    {
        return _PhuCapNhanVienRepository.AddPhuCap(pc);
    }

    public bool UpdatePhuCap(PhuCapNhanVien pc)
    {
        return _PhuCapNhanVienRepository.UpdatePhuCap(pc);
    }

    public bool DeletePhuCap(int id)
    {
        return _PhuCapNhanVienRepository.DeletePhuCap(id);
    }

    public List<PhuCapNhanVien> GetPhuCapByMaNV(int maNV)
    {
        return _PhuCapNhanVienRepository.GetPhuCapByMaNV(maNV);
    }
}