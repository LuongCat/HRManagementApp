namespace HRManagementApp.Services;
using HRManagementApp.Data;
using HRManagementApp.models; 
public class ChucVuService
{
    private readonly ChucVuRepository _repo = new ChucVuRepository();
    public List<ChucVu> GetAllChucVu() => _repo.GetAll();
}