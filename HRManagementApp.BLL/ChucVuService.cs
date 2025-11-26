namespace HRManagementApp.BLL;
using HRManagementApp.DAL;
using HRManagementApp.models; 
public class ChucVuService
{
    private readonly ChucVuRepository _repo = new ChucVuRepository();
    public List<ChucVu> GetAllChucVu() => _repo.GetAll();

    public ChucVu GetChucVuByName(string chucVuName)
    {
        return _repo.GetChucVuByName(chucVuName);
    }

    public ChucVu GetChucVuById(int id)
    {
        return _repo.GetChucVuById(id);
    }
}