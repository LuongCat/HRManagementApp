using HRManagementApp.DAL;
using HRManagementApp.models;

namespace HRManagementApp.BLL;

public class KhauTruService
{
    public readonly KhauTruRepository _KhauTruRepository;

    public KhauTruService()
    {
        _KhauTruRepository = new KhauTruRepository();
    }

    public List<KhauTru> GetAllDeduction()
    {
        return _KhauTruRepository.GetAllDeduction();
    }

    public List<KhauTru> GetDeductionByMaNV(int maNV)
    {
        return _KhauTruRepository.GetDeductionByMaNV(maNV);
    }

    public bool AddDeduction(KhauTru kt)
    {
        return _KhauTruRepository.AddDeduction(kt);
    }

    public bool UpdateDeduction(KhauTru kt)
    {
        return _KhauTruRepository.UpdateDeduction(kt);
    }

    public bool DeleteDeduction(int maKT, int maNV)
    {
        return _KhauTruRepository.DeleteDeduction(maKT, maNV);
    }

    public bool Delete(int maKT)
    {
        return _KhauTruRepository.Delete(maKT);
    }
}