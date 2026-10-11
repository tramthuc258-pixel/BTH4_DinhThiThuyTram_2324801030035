# Web - giao diện Vue của bài thực hành 4

Web là giao diện Vue 2 và BootstrapVue, chạy bằng Node.js. Thư mục này không có file `.csproj`; thêm Web không thay đổi số lượng 27 project .NET của solution hiện tại.

## Chuẩn bị và cài package

Dùng Node.js 24 và npm. Mở Windows PowerShell hoặc Developer PowerShell trong Visual Studio, rồi chạy:

```powershell
Set-Location 'D:\KT_thietkephanmem\InsuranceMicroservices\InsuranceMicroservices\Web'
Copy-Item -LiteralPath '.env-example' -Destination '.env.local'
npm.cmd ci
```

Chỉ copy `.env-example` lần đầu. Nếu đã có `.env.local`, giữ các giá trị đã cấu hình. File `.env.local` được Git bỏ qua; các giá trị mẫu dùng để thiết lập máy khác nằm trong `.env-example`.

`npm.cmd` gọi npm trong PowerShell mà không phụ thuộc chính sách chạy script `.ps1`. `npm ci` cài đúng phiên bản từ `package-lock.json` và yêu cầu file này khớp `package.json`.

Mẫu đã thay `node-sass` bằng Dart Sass, dùng Vue CLI 3.12.1 và webpack 4.47.0 để phù hợp với Node hiện tại. Vue và `vue-template-compiler` cùng được ghim ở 2.7.16 để tránh tiến trình build bị giữ mở trên Node.js 24. Sau khi nhận source, dùng `npm ci` để tái tạo dependencies. Dự án dùng `package-lock.json` của npm; lockfile Yarn cũ đã được bỏ.

## Chạy giao diện

Trong cùng thư mục, chạy:

```powershell
npm.cmd run serve -- --host 127.0.0.1 --port 8080
```

Giữ cửa sổ PowerShell mở. Khi terminal báo đã biên dịch xong, mở địa chỉ `http://localhost:8080` hoặc địa chỉ được terminal in ra. Nhấn `Ctrl+C` khi muốn dừng.

Các địa chỉ backend trong `.env.local`:

| Biến | Giá trị mẫu | Chức năng |
| --- | --- | --- |
| `VUE_APP_BACKEND_URL` | `http://localhost:8099/api/` | Gateway cho dữ liệu nghiệp vụ |
| `VUE_APP_AUTH_URL` | `http://localhost:6060/api/` | Đăng nhập |
| `VUE_APP_CHAT_URL` | `http://localhost:4099` | Chat qua SignalR |
| `VUE_APP_BOT_URL` | URL Bot Framework mẫu | Chatbot, cần cấu hình bot thực tế |

Khởi động lại `npm run serve` sau khi sửa các biến môi trường. Các biến `VUE_APP_*` được đưa vào mã chạy trong trình duyệt, nên không đặt thông tin bí mật trong đó.

Backend nghiệp vụ của bộ khung hiện tại chưa được triển khai và khởi chạy đầy đủ. Đăng nhập, danh sách sản phẩm, hợp đồng, dashboard và chat chưa được kiểm thử kết nối backend; việc giao diện mở được không xác nhận các chức năng này đã hoạt động.

## Build giao diện

```powershell
npm.cmd run build
```

Kết quả được ghi vào thư mục `dist`. Build này tách biệt với **Build Solution** trong Visual Studio. `node_modules` và `dist` được Git bỏ qua.

Đã kiểm tra trên Node.js 24.21.0: `npm run build` kết thúc với mã 0, tạo `dist/index.html`; có 2 cảnh báo về kích thước bundle. Lệnh `serve` biên dịch thành công và trang chủ trả HTTP 200, hiển thị menu cùng tiêu đề Insurance Sales Portal. Các chức năng kết nối backend chưa được kiểm thử.

## Docker

Dockerfile dùng Node.js 24 để cài package bằng `npm ci`, build giao diện, rồi phục vụ `dist` bằng nginx. Cấu hình nginx giữ các đường dẫn `/api` và `/login` của mẫu; khi chạy trong Docker, cần có các backend với tên `dotnet-agent-portal-gateway` và `dotnet-auth-service` trên cùng mạng Docker.

Các file `.env.local` và `.env.*.local` được loại khỏi Docker build để cấu hình máy cá nhân không bị đưa vào image. Cấu hình backend mặc định của image đi qua các đường dẫn tương đối của nginx.
