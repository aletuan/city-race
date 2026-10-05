# City Race Lộ trình phát triển

Phiên bản 0.1 • 05/10/2026 • [English](ROADMAP.en.md)

Theo [GDD](GDD.vi.md), [tech stack](TECH_STACK.vi.md) và [quy ước kỹ thuật](ENGINEERING_GUIDELINES.vi.md). Mốc là điều kiện kiểm chứng, không phải cam kết lịch. Tập trung rủi ro chưa chứng minh tiếp theo; không thêm nội dung để bù điều khiển hoặc mạng chưa tốt.

| Mốc | Đầu ra | Bằng chứng hoàn thành | Trạng thái |
| --- | --- | --- | --- |
| M0 Môi trường và khởi tạo | Unity project khóa phiên bản, cảnh URP tối thiểu, quy trình build tái lập | Cảnh chạy trên iPhone thật; Web smoke test đã host; ghi editor/package | Đang làm; đã cài Editor; còn khởi tạo project và signing |
| M1 Prototype lái | Lái một ngón tay, đường, ổ gà, xe buýt, chướng ngại hình khối | Người mới xuất phát/rẽ/dừng được; phản hồi muốn chơi lại; hiệu năng thiết bị; Android smoke build | Dự kiến |
| M2 Prototype multiplayer | Hai nhà, hai người, chung giao thông/đích; sau đó bốn người | Native/Web chung phòng, lái phản hồi tốt, kết quả thống nhất, mất kết nối, số byte Relay mỗi giờ-người | Dự kiến |
| M3 Một màn nhỏ hoàn chỉnh | Bốn nhà, đi làm sáng, sự kiện trong phạm vi, cá nhân hóa đơn giản, âm thanh, kết quả/chơi lại | Nhóm ngoài hoàn thành luồng và tự muốn chơi lại; hiệu năng với tải đại diện | Dự kiến |
| M4 Sản xuất và hoàn thiện | Hoàn thiện nội dung đã chứng minh, sửa lỗi, khả năng tiếp cận và tối ưu | Danh sách thiết bị đạt, phân loại lỗi còn lại, đạt ngân sách hiệu năng/mạng | Dự kiến |
| M5 Kiểm chứng phát hành | TestFlight, metadata, trang hỗ trợ/quyền riêng tư, cấu hình dịch vụ | Xử lý phản hồi beta, review checklist và build phát hành được duyệt | Dự kiến |
| M6 Vận hành | Theo dõi crash/chi phí và phản hồi | Ưu tiên thay đổi từ dữ liệu sử dụng, ổn định và chi phí thực | Dự kiến |

## Trình tự hiện tại

1. Hoàn tất điều kiện môi trường M0 và chạy smoke scene trên iPhone thật.
2. Kiểm chứng Web build và tạo package lock.
3. Chỉ triển khai điều khiển/cảm giác lái M1 trong [backlog](BACKLOG.vi.md).
4. Playtest nhỏ; tạo biên bản có ngày khi thực sự diễn ra.
5. Chỉ vào M2 khi cảm giác lái đạt tiêu chí GDD.

Smoke scene native của M0 kiểm tra đường build, chưa phải prototype lái. Kiểm tra Android trong M1, không để tới phát hành.

## Theo dõi và bằng chứng

Dùng Backlog theo dõi task tới khi quyết định chuyển sang GitHub Issues. Trạng thái: Chưa làm, Đang làm, Bị chặn, Xong. Xong cần bằng chứng nghiệm thu. Ghi ngày, commit, thiết bị/OS/trình duyệt, editor, loại build, kết quả quan sát và giới hạn. Mặc định không đưa log/binary vào Git.

Chỉ tạo docs/playtests/YYYY-MM-DD-topic.en.md và .vi.md sau buổi thử thật. Gồm setup, người tham gia không kèm dữ liệu cá nhân thừa, nhiệm vụ, quan sát, lỗi, quyết định và thử nghiệm tiếp theo. Tách quan sát khỏi suy luận. Không tạo biên bản khiến người đọc hiểu rằng đã thử khi chưa thử.

Đổi công nghệ cần quyết định song ngữ ngắn. Nếu thử nghiệm thất bại, xem lại phạm vi mốc, không đánh dấu xong chỉ để theo lịch.
