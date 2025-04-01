using System.ComponentModel;

namespace backend.Models
{
    public enum ReportReason
    {
        [Description("Câu trả lời sai")]
        CâuTrảLờiSai = 1,

        [Description("Lỗi chính tả hoặc ngữ pháp")]
        LỗiChínhTảHoặcNgữPháp = 2,

        [Description("Câu hỏi không rõ ràng")]
        CâuHỏiKhôngRõRàng = 3,

        [Description("Câu hỏi thiếu thông tin")]
        CâuHỏiThiếuThôngTin = 4,

        [Description("Nội dung không phù hợp")]
        NộiDungKhôngPhùHợp = 5,

        [Description("Trùng lặp với câu hỏi khác")]
        TrùngLặpVớiCâuHỏiKhác = 6
    }
}
