# FU Mini Hotel System - Hướng Dẫn Sử Dụng

## Giới Thiệu
FU Mini Hotel System là hệ thống quản lý khách sạn được phát triển bằng C# WPF (.NET 8.0) với kiến trúc MVVM. Hệ thống cho phép quản lý phòng, khách hàng, đặt phòng và báo cáo một cách hiệu quả.

## Tính Năng Chính

### 🔐 Hệ Thống Xác Thực
- Đăng nhập với tài khoản Admin hoặc Customer
- Phân quyền truy cập theo vai trò
- Bảo mật thông tin người dùng

### 👥 Quản Lý Khách Hàng (Admin)
- Thêm, sửa, xóa thông tin khách hàng
- Tìm kiếm khách hàng theo tên, email
- Xem lịch sử đặt phòng của khách hàng
- Quản lý trạng thái tài khoản khách hàng

### 🏨 Quản Lý Phòng (Admin)
- Thêm, sửa, xóa thông tin phòng
- Phân loại phòng theo loại (Standard, Deluxe, Suite)
- Quản lý giá phòng và trạng thái phòng
- Tìm kiếm phòng theo số phòng, loại phòng

### 📅 Quản Lý Đặt Phòng (Admin)
- Tạo đặt phòng mới cho khách hàng
- Chỉnh sửa thông tin đặt phòng
- Hủy đặt phòng
- Lọc đặt phòng theo loại (Online/Offline)
- Tìm kiếm đặt phòng theo khách hàng, phòng, ngày

### 📊 Báo Cáo (Admin)
- Thống kê đặt phòng theo thời gian
- Báo cáo doanh thu
- Phân tích loại phòng được đặt nhiều nhất

### 👤 Giao Diện Khách Hàng
- Xem và cập nhật thông tin cá nhân
- Đổi mật khẩu
- Đặt phòng trực tuyến
- Xem lịch sử đặt phòng của mình

## Cài Đặt và Chạy Dự Án

### Yêu Cầu Hệ Thống
- Windows 10/11
- .NET 8.0 SDK
- SQL Server (LocalDB, Express, hoặc Full)
- Visual Studio 2022 (khuyến nghị) hoặc VS Code

### Bước 1: Cài Đặt Dependencies
```bash
# Cài đặt .NET 8.0 SDK từ Microsoft
# Tải từ: https://dotnet.microsoft.com/download/dotnet/8.0

# Cài đặt Entity Framework tools
dotnet tool install --global dotnet-ef
```

### Bước 2: Cấu Hình Database
1. **Cập nhật Connection String** trong các file:
   - `BusinessObjects/appsettings.json`
   - `TranTuanKietWPF/appsettings.json`

```json
{
  "ConnectionStrings": {
    "HotelDB": "Server=localhost;Database=HotelManagementDB;User Id=SA;Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

2. **Tạo Database**:
```bash
cd BusinessObjects
dotnet ef database update
```

### Bước 3: Build và Chạy
```bash
# Build toàn bộ dự án
dotnet build TranTuanKietWPF/TranTuanKietWPF.csproj

# Chạy ứng dụng
dotnet run --project TranTuanKietWPF/TranTuanKietWPF.csproj
```

## Hướng Dẫn Sử Dụng

### 🔑 Đăng Nhập Hệ Thống

#### Tài Khoản Admin
- **Email:** `admin@FUMiniHotelSystem.com`
- **Password:** `@@abc123@@`

#### Tài Khoản Khách Hàng
- Sử dụng email và mật khẩu đã đăng ký
- Hoặc đăng ký tài khoản mới thông qua Admin

### 🖥️ Giao Diện Admin Dashboard

#### 1. Quản Lý Khách Hàng
- **Thêm khách hàng mới**: Click "Add" → Điền thông tin → "Save"
- **Sửa thông tin**: Chọn khách hàng → Click "Edit" → Cập nhật → "Save"
- **Xóa khách hàng**: Chọn khách hàng → Click "Delete" → Xác nhận
- **Tìm kiếm**: Nhập tên hoặc email vào ô Search

#### 2. Quản Lý Phòng
- **Thêm phòng mới**: Click "Add" → Điền thông tin phòng → "Save"
- **Chỉnh sửa phòng**: Chọn phòng → Click "Edit" → Cập nhật → "Save"
- **Xóa phòng**: Chọn phòng → Click "Delete" → Xác nhận
- **Lọc theo loại phòng**: Chọn loại phòng từ dropdown

#### 3. Quản Lý Đặt Phòng
- **Tạo đặt phòng**: Click "Add" → Chọn khách hàng và phòng → Chọn ngày → "Save"
- **Chỉnh sửa**: Chọn đặt phòng → Click "Edit" → Cập nhật → "Save"
- **Hủy đặt phòng**: Chọn đặt phòng → Click "Delete" → Xác nhận
- **Lọc theo loại**: Sử dụng các nút "Online", "Offline", "All"
- **Tìm kiếm theo ngày**: Chọn khoảng thời gian → Click "Filter by Date"

#### 4. Báo Cáo
- Xem thống kê đặt phòng theo thời gian
- Phân tích doanh thu
- Báo cáo loại phòng phổ biến

### 👤 Giao Diện Khách Hàng

#### 1. Thông Tin Cá Nhân
- Xem thông tin cá nhân
- Cập nhật thông tin liên hệ
- Thay đổi mật khẩu

#### 2. Đặt Phòng
- Chọn ngày check-in và check-out
- Chọn loại phòng mong muốn
- Xem danh sách phòng có sẵn
- Đặt phòng và xác nhận

#### 3. Lịch Sử Đặt Phòng
- Xem tất cả đặt phòng đã thực hiện
- Kiểm tra trạng thái đặt phòng
- Xem chi tiết từng đặt phòng

## Cấu Trúc Dự Án

```
TranTuanKiet_SE18D07/
├── BusinessObjects/          # Models và DbContext
├── DataAccessObjects/        # Data Access Layer
├── Repositories/            # Repository Pattern
├── Services/                # Business Logic Layer
└── TranTuanKietWPF/         # WPF UI Application
    ├── ViewModels/          # MVVM ViewModels
    ├── Views/               # WPF Views
    └── Converters/          # Data Converters
```

## Xử Lý Lỗi Thường Gặp

### 1. Lỗi Kết Nối Database
- Kiểm tra SQL Server đã chạy chưa
- Xác minh connection string trong appsettings.json
- Đảm bảo database đã được tạo

### 2. Lỗi Build
- Kiểm tra .NET 8.0 SDK đã cài đặt
- Restore NuGet packages: `dotnet restore`
- Clean và rebuild project

### 3. Lỗi Đăng Nhập
- Kiểm tra email và password chính xác
- Đảm bảo tài khoản có trạng thái hoạt động
- Kiểm tra kết nối database

## Tính Năng Nâng Cao

### Validation
- Kiểm tra định dạng email
- Xác minh mật khẩu (tối thiểu 6 ký tự)
- Validate ngày tháng hợp lệ
- Kiểm tra trùng lặp email

### Business Logic
- Tự động tính giá phòng theo thời gian
- Kiểm tra phòng có sẵn khi đặt
- Quản lý trạng thái đặt phòng
- Soft delete cho dữ liệu quan trọng

### UI/UX
- Giao diện hiện đại với Material Design
- Responsive layout
- Real-time validation
- User-friendly error messages

## Hỗ Trợ

Nếu gặp vấn đề hoặc cần hỗ trợ:
1. Kiểm tra log lỗi trong console
2. Xác minh cấu hình database
3. Đảm bảo tất cả dependencies đã cài đặt
4. Liên hệ developer để được hỗ trợ

---

**Phiên bản:** 1.0  
**Tác giả:** Tran Tuan Kiet - SE18D07  
**Môn học:** PRN212 - .NET Programming  
**Trường:** FPT University




