namespace backend.Models
{
    public enum PaymentIssueType
    {
        ChuyenDu,           // Không có vấn đề gì
        ChuyenThua,         // Người dùng chuyển thừa tiền
        ChuyenThieu,        // Chuyển thiếu tiền
        ChuyenMuon,         // Chuyển muộn nhưng đúng số tiền
        ChuyenMuon_Thua,    // Chuyển muộn và chuyển thừa
        ChuyenMuon_Thieu,    // Chuyển muộn và chuyển thiếu
        TuChoi                //Từ chối giao dịch do chuyển 2 lần vẫn thiếu
    }
}
