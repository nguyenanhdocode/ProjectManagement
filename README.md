# ProjectManagement
Đồ án website quản lý dự án nhóm.

## Công nghệ sử dụng
1. .NET 6, ASP.NET CORE API
2. SQL SERVER
3. ReactJS (Material UI)

## Hình ảnh một số chức năng
### Màn hình danh mục dự án
![image](https://github.com/user-attachments/assets/f096340d-041f-49d7-bb7f-9b7f32d1f393)

Màn hình danh mục dự án hiển thị danh sách các dự án mà người dùng đang quản lý hoặc đang tham gia với vai trò là thành viên, cùng với bộ lọc để giúp người dùng lọc các dự án theo các tiêu chí khác nhau như: vai trò tham gia, ngày bắt đầu, ngày kết thúc. Mỗi ô vuông nhỏ đại diện cho một dự án, thể hiện tóm lược thông tin quan trọng của một dự án.
### Màn hình chi tiết dự án
![image](https://github.com/user-attachments/assets/44ab9f7f-ffae-4f5d-bad2-9e03e11ba941)

Tại màn hình xem chi tiết sẽ hiển thị các thông tin của một dự án theo 4 nhóm khác nhau.

Nhóm 1. Tổng quan dự án, hiển thị các thông tin cơ bản của một dự án, trong đó có tiến độ dự án (tính bằng đơn vị phần trăm), thời gian còn lại của dự án. Tại đây có các nút để người dùng có thể cập nhật thông tin của dự các thông tin dự án như: tên, mô tả, ngày bắt đầu, ngày kết thúc.

Nhóm 2. Công việc, các công việc của dự án được hiển thị ở dạng timeline, mỗi ô trên timeline đại diện cho một công việc, màu sắc của ô phụ thuộc vào trạng thái của công việc. Khi click chuột phải vào các công việc trên timeline sẽ có một menu hiện ra với các lựa chọn như: tổng quan, xem chi tiết, cập nhật và xóa. Tại đây người dùng có thể tạo mới công việc, tìm kiếm và lọc danh sách các công việc.

Nhóm 3. Thành viên, hiển thị các thành viên đang tham gia vào dự án. Tại đây người trưởng dự án có thể thêm hoặc xóa thành viên khỏi dự án.

Nhóm 4. Tài nguyên, hiển thị danh sách các tài nguyên được thêm vào dự án và nút thao tác xóa. Tại đây người dùng có thể chọn hoặc upload tài nguyên để thêm vào dự án.

### Màn hình thống kê công việc hôm nay
![image](https://github.com/user-attachments/assets/6858be86-e9d1-4d74-b1f6-15d42887bc6e)

Chức năng thống kê công việc hôm nay hiển thị danh sách các công việc mà người dùng được giao hoặc những công việc trong dự án mà người dùng phải quản lý cần thực hiện trong hôm nay. Chức năng cho phép người dùng lọc ra chỉ những công việc mà hình phải phụ trách.

Khi nhìn trên bảng danh sách các công việc, chúng ta sẽ thấy các dòng có 3 màu khác nhau:

•	Màu xám đậm: là những dòng hiển thị thông tin của dự án, các dòng sau đó là các dòng thông tin công việc. Người dùng có thể nháy chuột vào dòng này để di chuyển đến màn hình xem chi tiết dự án.

•	Màu xám nhạt: là những dòng hiển thị thông tin công việc cha.

•	Màu trắng: là những dòng hiển thị thông tin công việc con.

Ngoài ra, cột cuối cùng sẽ hai nút nhấn xem tổng quan và xem chi tiết công việc.

### Màn hình thống kê công việc trễ
![image](https://github.com/user-attachments/assets/7df1b9da-eff6-48e9-a67e-7415a1dd6f2b)

Chức năng thống kê công việc trễ hiển thị danh sách các công việc bắt đầu hoặc kết thúc trễ của các công việc mà người dùng được giao hoặc thuộc dự án mà người dùng đang quản lý.
