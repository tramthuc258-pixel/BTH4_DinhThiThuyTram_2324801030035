# BTH5 - Task 1: PricingService

PricingService cung cấp `POST /api/Pricing` để tính phí từ sản phẩm, quyền lợi và câu trả lời. Phần triển khai được bổ sung để làm việc với API và các bộ test thầy cung cấp; các file test gốc được giữ nguyên.

## Các project

- `PricingService.Api`: command, kết quả, validation và chuyển đổi JSON cho các loại câu trả lời.
- `PricingService`: ASP.NET Core 8, controller, MediatR handler, domain tính phí và truy xuất biểu phí PostgreSQL.
- `PricingService.Test`: unit test gốc và test bổ sung về tăng phí, làm tròn, quyền lợi trùng/không hợp lệ và JSON.
- `PricingService.IntegrationTest`: hai test gốc gọi API với PostgreSQL container tạm.

Biểu phí chứa các biểu thức được xử lý bằng [DynamicExpresso](https://github.com/dynamicexpresso/DynamicExpresso). Hệ số `1.5` tương ứng nhân phí với `1.5`; các hệ số có điều kiện thỏa mãn được nhân lần lượt. Phí của mỗi quyền lợi được làm tròn hai chữ số thập phân trước khi cộng tổng.

## Build và kiểm thử

Chạy các lệnh dưới đây trong thư mục chứa `InsuranceMicroservices.slnx`:

```powershell
dotnet restore
dotnet build
dotnet test .\PricingService.Test\PricingService.Test.csproj
dotnet test .\PricingService.IntegrationTest\PricingService.IntegrationTest.csproj
```

Unit test không cần database. Integration test cần Docker Desktop đang chạy ở chế độ Linux containers và khả năng tải image PostgreSQL. Testcontainers tạo database riêng rồi dọn dẹp sau kiểm thử; không dùng database PostgreSQL của BTH4.

Kết quả kiểm tra Task 1: 22/22 unit test và 2/2 integration test đạt. Rebuild toàn bộ solution 28 project có 0 lỗi và 9 cảnh báo có sẵn ở Gateway/Blazor; phần PricingService không phát sinh cảnh báo trong lần Rebuild này. Các file test gốc đã được đối chiếu SHA256 để xác nhận không bị thay đổi.

## Chạy API trên máy

Khởi động PostgreSQL của BTH4, rồi chạy service:

```powershell
docker compose -f .\scripts\infra.yml up -d postgres
dotnet run --project .\PricingService\PricingService.csproj
```

Profile mặc định lắng nghe `http://localhost:5040`. API có endpoint `POST /api/Pricing`; chưa có trang chủ hoặc Swagger UI. Kết quả tính giá chứa `totalPrice` và `coverPrices`; dữ liệu không hợp lệ trả HTTP 400, sản phẩm chưa có biểu phí trả HTTP 404.

Chuỗi kết nối mặc định trong `appsettings.json` trỏ tới database `lab_netmicro_pricing` của BTH4, với tài khoản lab. Có thể thay bằng biến môi trường `ConnectionStrings__DefaultConnection`. Khi khởi động, service tạo bảng `pricing_tariffs` nếu chưa tồn tại và thêm bốn biểu phí mẫu `TRI`, `HSI`, `FAI`, `CAR`. Biểu phí đã tồn tại không bị ghi đè.

## Phạm vi

Task 1 hoàn thiện phần tính giá, API và kiểm thử. Chưa thêm đăng ký Eureka hay thay đổi các microservice khác; đó là phần làm tiếp trong Task 2. Chưa tự commit hoặc push lên GitHub.
