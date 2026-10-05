# City Race Danh sách công việc

Phiên bản 0.1 • 05/10/2026 • [English](BACKLOG.en.md) • [Lộ trình](ROADMAP.vi.md)

P0 chặn mốc hiện tại; P1 đi sau nền tảng. Hai ngôn ngữ dùng chung ID và phụ thuộc. Trạng thái theo bằng chứng thực tế, không theo kết quả dự kiến.

| ID | Ưu tiên | Công việc và nghiệm thu | Phụ thuộc | Trạng thái |
| --- | --- | --- | --- | --- |
| CR-001 | P0 | Kiểm tra editor, Xcode, thiết bị, ổ đĩa, signing; ghi vào Setup | Không | Xong |
| CR-002 | P0 | Cài/kích hoạt Unity 6000.3.25f1 Apple Silicon và iOS support; xác nhận mở Editor | Đăng nhập/license Unity; đã cho phép cài | Xong; đã xác minh license Hub và executable Editor |
| CR-003 | P0 | Khởi tạo từ URP template đã cài; commit settings, package lock, metadata thật; compile sạch | CR-002 | Xong; đã sinh settings/lock/metadata, batch compile đạt |
| CR-004 | P0 | Sinh cảnh camera, đường, nhà, mô hình xe; quan sát URP render | CR-003 | Xong; đã quan sát Smoke trong Editor Play Mode |
| CR-005 | P0 | Xuất Xcode project, ký với team đã chọn, cài/chạy iPhone thật; quan sát cảnh | CR-004, signing Apple hợp lệ | Xong; đã ký, cài, chạy và kiểm tra ảnh iPhone |
| CR-006 | P1 | Cài Web support, build và phục vụ qua HTTPS; kiểm tra cảnh/tải trên trình duyệt | CR-004, dung lượng | Xong; ảnh người dùng xác nhận tải/render HTTPS; chưa kiểm tra console |
| CR-010 | P0 | Input pointer chuẩn hóa, phanh khi nhả/mất focus; chuột/cảm ứng nhất quán | M0 | Đang làm; đã có đường tập, thoát kẹt + 11 test PlayMode đạt; chờ cảm giác lái/kiểm tra fps trên máy thật |
| CR-011 | P0 | Bộ điều khiển xe có tăng tốc/rẽ/phanh; so sánh frame rate | CR-010 | Đang làm; đã có đường tập, thoát kẹt + 11 test PlayMode đạt; chờ cảm giác lái/kiểm tra fps trên máy thật |
| CR-012 | P0 | Phản ứng ổ gà/xe buýt/chướng ngại dễ hiểu; không ép thất bại vô cớ | CR-011 | Chưa làm |
| CR-013 | P1 | Chỉnh camera, safe area, tầm nhìn; ngón tay không che chướng ngại quan trọng | CR-011 | Chưa làm |
| CR-014 | P1 | Cài module Android, chạy trên thiết bị thật; ghi lỗi tương thích | CR-011, thiết bị và dung lượng | Chưa làm |
| CR-015 | P0 | Playtest 5–8 người theo GDD; đo hiệu năng thiết bị và ghi quyết định | CR-012, CR-013 | Chưa làm |
| CR-020 | P0 | Kiểm chứng phòng hai người, authority và độ trễ trước nội dung bốn người | M1 | Mốc dự kiến |
| CR-021 | P1 | Đo Relay mỗi giờ-người, gồm phần host; ước tính chi phí tháng | CR-020 | Mốc dự kiến |

## Trở ngại hiện tại

- Đã giải quyết trở ngại dung lượng: ngày 05/10 internal còn khoảng 21 GiB sau cài đặt, T7 khoảng 870 GiB. Tiếp tục kiểm tra dung lượng tạm khi cài.
- Đã cài Hub 3.22.2 và Editor 6000.3.25f1 ARM64 cùng iOS/Web. Đã xác minh license Personal và executable Editor. CR-003–006 đã đạt; đang làm phần lái M1.
- Có Xcode 26.6. Đã xác minh iPhone 15 Pro Max; giữ kết nối và mở khóa cho các lần chạy tiếp theo.
- Đã giải quyết signing sau đăng nhập lại Xcode: automatic signing và chạy iPhone thật đạt. Không sửa credential City Crew.
- Chưa có Git LFS; cài/cấu hình trước khi thêm asset nguồn nhị phân lớn. Công việc hiện tại chỉ có text.

Chi tiết môi trường và bước tiếp tục trong [Setup](SETUP.vi.md). Chưa nghiệm thu M1 trước khi ghi nhận kiểm tra thủ công trên thiết bị.

Phần lái M1: [triển khai, điều khiển và kiểm chứng](RIDING_PROTOTYPE.vi.md).
