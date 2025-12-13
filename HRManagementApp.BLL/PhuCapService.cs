using System.ComponentModel;
using System.Linq.Expressions;
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

    public List<RewardEvaluation> GetRewardEvaluation(int maNV, int month, int year)
    {
        var result = new List<RewardEvaluation>();

        ChamCongRepository _chamCongRepo = new ChamCongRepository();

        // 1. Ngày công
        int workingDays = _chamCongRepo.GetWorkingDays(maNV, month, year);

        result.Add(new RewardEvaluation
        {
            TieuChi = "Ngày công (+10/lần)",
            SoLan = workingDays,
            Diem = workingDays * 10
        });

        // 2. Đi trễ
        int lateCount = _chamCongRepo.GetLateCount(maNV, month, year);

        result.Add(new RewardEvaluation
        {
            TieuChi = "Đi trễ (-10/lần)",
            SoLan = lateCount,
            Diem = lateCount * -10
        });

        // 3. OT
        double otHours = _chamCongRepo.GetOTHours(maNV, month, year);

        result.Add(new RewardEvaluation
        {
            TieuChi = "Tăng ca (+1/h): ",
            SoLan = (int)Math.Round(otHours),
            Diem = (int)Math.Round(otHours) * 1
        });

        return result;
    }
}