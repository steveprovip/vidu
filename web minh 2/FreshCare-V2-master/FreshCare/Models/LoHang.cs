namespace FreshCare.Models
{
    /// <summary>
    /// Bảng Lô hàng (Batch) - BẢNG LÕI CỦA HỆ THỐNG
    /// Mỗi lần nhập hàng tạo 1 lô mới gắn NSX + HSD
    /// Luật #7: Tự động phân loại trạng thái: Quá hạn (Đỏ), Cận date <14 ngày (Cam), An toàn (Xanh)
    /// Luật #6: Số ngày còn lại hiển thị >= 0
    /// </summary>
    public class LoHang
    {
        public int MaLo { get; set; }
        public int MaSP { get; set; }
        public decimal SoLuongNhap { get; set; }
        public decimal SoLuongTon { get; set; }
        public DateTime NgaySanXuat { get; set; }
        public DateTime HanSuDung { get; set; }
        public DateTime NgayNhapKho { get; set; }
        public string TrangThai { get; set; } = "An Toàn"; // 'An Toàn','Cận Date','Quá Hạn','Đã Hủy'

        // === Thuộc tính tính toán (không lưu DB) ===
        public string? TenSP { get; set; }
        public string? DonViTinh { get; set; }
        public string? TenDanhMuc { get; set; }
        public decimal GiaBanGoc { get; set; }
        public decimal PhanTramSale { get; set; }

        /// <summary>
        /// Luật #6: Số ngày còn lại, không cho phép giá trị âm
        /// </summary>
        public int SoNgayConLai
        {
            get
            {
                int days = (HanSuDung.Date - DateTime.Now.Date).Days;
                return days < 0 ? 0 : days;
            }
        }

        /// <summary>
        /// Tổng phần trăm sale: 
        /// Base sale + Giá trị cộng dồn nếu cực sát ngày hết hạn
        /// </summary>
        public decimal TongPhanTramSale
        {
            get
            {
                if (TrangThai == "Cận Date")
                {
                    decimal extraSale = 0;
                    int days = SoNgayConLai;

                    // Càng gần ngày hết hạn cộng thêm sale càng lớn
                    if (days <= 3) extraSale = 40;
                    else if (days <= 7) extraSale = 20;

                    return Math.Min(PhanTramSale + extraSale, 90);
                }
                return PhanTramSale;
            }
        }

        /// <summary>
        /// Luật #8 mới: Giá thực tế (Sale lũy tiến)
        /// Tối đa giảm 90% để tránh giá bằng 0.
        /// </summary>
        public decimal GiaThucTe
        {
            get
            {
                if (TrangThai == "Cận Date")
                {
                    return GiaBanGoc * (100 - TongPhanTramSale) / 100;
                }
                return GiaBanGoc;
            }
        }

        /// <summary>
        /// Màu sắc CSS cho Dashboard (Đỏ/Cam/Xanh)
        /// </summary>
        public string MauCanhBao
        {
            get
            {
                return TrangThai switch
                {
                    "Quá Hạn" => "danger",    // Đỏ
                    "Cận Date" => "warning",   // Cam
                    "Đã Hủy" => "secondary",  // Xám
                    _ => "success"             // Xanh
                };
            }
        }
    }
}
