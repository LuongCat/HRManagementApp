namespace HRManagementApp.models
{
    public class KetQuaChamCong
    {
        public int SoNgayDiLam { get; set; } = 0; // Chỉ số 2: Số ngày đi làm thực tế
        public int DiemDiTre { get; set; } = 0;   // Chỉ số 1: Điểm phạt đi trễ
        
        public decimal SoGioDiLam { get; set; } = 0;
    }
}