# Bản lái thử — M1, đường tập (0.0.3)

5 tháng 10 năm 2026 • [English](RIDING_PROTOTYPE.en.md)

Cảnh hiện tại: `Assets/CityRace/Content/Scenes/Practice.unity`. Giữ lại Smoke và Riding đường thẳng cũ, nhưng bật Practice để build. Chỉ tạo cảnh chưa tồn tại bằng `python3 tools/unity_project.py practice`; công cụ không ghi đè cảnh đã có.

## Vòng chơi hiện tại

Rời nhà, theo vạch vàng qua bốn góc cua trái/phải và đoạn hẹp, rồi dừng trong ô vàng trước cổng công ty. Tuyến dài khoảng 107 m theo tim đường. Đi qua các checkpoint đúng thứ tự và giữ tốc độ dưới 0,4 m/s trong vùng đích 0,7 giây để hoàn thành. Phía trên có hướng đi và thời gian. Chọn Chơi lại bất cứ lúc nào để bắt đầu lượt mới mà không cần mở lại app.

Vẫn điều khiển bằng kéo tương đối một ngón. Khi rẽ gắt, ga mục tiêu giảm dần (còn 40% khi lệch hướng 100°), giới hạn rẽ 165°/s. Tốc độ thẳng tối đa vẫn 8 m/s; tăng tốc/phanh 5/16 m/s², mô phỏng 50 Hz. Đây là thông số thử nghiệm, chưa phải cảm giác lái đã được người chơi xác nhận.

Thoát kẹt: nhả tay rồi chạm giữ yên 1,2 giây khi xe dừng. Màn hình hiện tiến trình. Xe trở về checkpoint đã qua phía trước đó theo hành trình, về tốc độ không, phải nhả tay trước khi chạy tiếp; cộng 2 giây. Chiếu vị trí lên tuyến giúp giới hạn điểm hồi phục khi đi ngược, tránh nhảy tới phía trước trên hành trình (điểm xuất phát là giới hạn thấp nhất). Giữ ngắn hoặc gián đoạn không kích hoạt. Khi hoàn thành không thể hồi phục; Chơi lại xóa trạng thái lượt cũ. Camera mở rộng với orthographic size 12 và chuyển ngay sau lần hồi phục xa.

## Phân chia trách nhiệm và giới hạn

`PracticeProgress` giữ tiến độ theo thứ tự, điều kiện dừng và phạt hồi phục bằng C# thuần. `PracticeCourse` kết nối vị trí Unity, input, đặt lại vật lý và vòng đời. `PracticeHud` quan sát lượt thử và gửi thao tác Chơi lại. Hình học đường tạo một lần trong Editor, dùng chung vật liệu và static geometry; không tạo đường mỗi frame.

Đây vẫn là đường tập offline, chưa phải cuộc đua hoàn chỉnh. Chưa có ổ gà, xe buýt, multiplayer hay luật giao thông thật. HUD thử nghiệm nhỏ tiếp tục ngoại lệ tạm dùng uGUI/font có sẵn đã ghi nhận; còn TextMeshPro/khóa bản địa hóa và kiểm tra đủ dấu tiếng Việt trước HUD sản phẩm. Giảm ga khi cua cần phản hồi chơi thật. Chưa kết luận fps, nhiệt độ máy hay tương thích Android/Web của phần này.

## Kiểm chứng 0.0.3

Mười một test Editor PlayMode đã đạt trên Unity 6000.3.25f1, gồm bấm Chơi lại bằng con trỏ giả lập và lái tự động qua tuyến đã tạo. Dùng lệnh Unity đã ghim trong ghi chú lịch sử, đổi tên log thành `Logs/practice-tests.xml` và `Logs/practice-tests.log`. Phạm vi gồm chạy tuyến thực tế, dừng để về đích, Chơi lại qua sự kiện con trỏ, giữ yên để thoát kẹt, không hồi phục lặp khi vẫn giữ tay, giới hạn khi đi ngược và giảm tốc lúc rẽ gắt, cùng test hồi quy di chuyển/input cũ.

Đã xuất iOS 0.0.3 và build Debug có ký bằng Xcode 26.6. Kiểm tra chữ ký nghiêm ngặt, cài và khởi chạy trên iPhone 15 Pro Max (iOS 26.3 beta) đã đạt; tiến trình vẫn tồn tại sau khi mở. Điện thoại đang hiển thị ứng dụng khác lúc chụp kiểm tra, nên nghiệm thu hình ảnh/cảm giác lái native còn chờ phản hồi người dùng. Không lưu ảnh không liên quan vào bằng chứng dự án.

---

## Bằng chứng lịch sử 0.0.2

# Bản lái thử — M1, phần đầu

5 tháng 10 năm 2026 • [English](RIDING_PROTOTYPE.en.md)

M0 đã xác lập quy trình build. Phần này bắt đầu CR-010/011: một xe trên đoạn đường tập dài 120 m có ranh giới, dùng chung bộ di chuyển cho chuột và cảm ứng. M1 chưa hoàn tất.

## Cách thử

Mở `Assets/CityRace/Content/Scenes/Riding.unity` rồi vào Play Mode, hoặc chạy bản iOS development 0.0.2. Chạm trong 65% phía dưới vùng an toàn, kéo theo hướng muốn đi. Kéo xa hơn để tăng ga; nhả tay để phanh. Mỗi lần chạm mới bắt đầu ở mức ga bằng không. Hướng lên màn hình tương ứng hướng bắc của cảnh; camera đi theo nhưng không xoay theo xe.

Lề đường và hai đầu đường có va chạm. Chưa có thao tác thoát kẹt: khởi động lại ứng dụng/cảnh nếu xe mắc vào ranh giới. Ổ gà, xe buýt, đồng hồ đua, đích đến và multiplayer thuộc các phần tiếp theo.

## Triển khai và thông số thử nghiệm

- `DragRideInput`: đọc Input System; bán kính bằng 18% cạnh ngắn của vùng an toàn, vùng chết 12%. Hủy điều khiển khi nhả, mất focus, tạm dừng, đổi kích thước hoặc disable. Ngón thứ hai không kế thừa ga; sau khi hủy phải nhả hết các ngón.
- `BikeMotor`: Rigidbody trên mặt phẳng đường, mô phỏng cố định 50 Hz; tối đa 8 m/s, tăng tốc 5 m/s², phanh 16 m/s², rẽ tối đa 135°/s và giảm khả năng rẽ khi đi chậm. Không xoay tại chỗ hay có số lùi.
- `RidingCamera`: theo xe có làm mượt trong LateUpdate, góc nhìn cố định 58°, orthographic size 10. Camera và cảm giác lái vẫn cần playtest.
- `RidingHint`: hướng dẫn trong vùng an toàn, tiếng Anh/Việt theo ngôn ngữ hệ thống. Tạm dùng uGUI Text với font có sẵn để chưa phải nhập font asset; chuyển sang TextMeshPro với khóa bản địa hóa và kiểm tra dấu tiếng Việt trước khi mở rộng HUD.
- Assembly runtime phụ thuộc Input System và uGUI. Chưa thêm thư viện mạng/dịch vụ. Test nằm trong assembly riêng, không đi cùng bản build thông thường.
- Công cụ build dùng các cảnh được bật trong Build Settings; giữ Smoke, bật Riding. Công cụ tạo cảnh từ chối ghi đè Riding đã tồn tại.

## Đã kiểm chứng và còn chờ

Sáu test Editor PlayMode đã đạt trên Unity 6000.3.25f1: tăng tốc/giới hạn tốc độ/phanh, rẽ theo vòng cung có giới hạn, va chạm tường, kéo/nhả/hủy/chạm lại bằng chuột, cảm ứng chuẩn hóa/nhả tay, và nhấc ngón điều khiển khi còn ngón khác. Test dùng thiết bị Input System giả lập, chưa phải bằng chứng cảm ứng vật lý. Thiết lập focus riêng của test được khôi phục sau từng test. Kết quả nằm ở `Logs/riding-tests.xml`, không đưa vào Git.

iOS 0.0.2: đã xuất Unity, build Debug bằng Xcode 26.6, ký tự động, kiểm tra chữ ký nghiêm ngặt, cài và mở trên iPhone 15 Pro Max (iOS 26.3 beta). Ảnh chụp thiết bị qua Xcode xác nhận đường, xe và hướng dẫn tiếng Anh hiển thị; bằng chứng local là `Logs/riding-iphone.png`. Ảnh không chứng minh chất lượng điều khiển hoặc fps.

Cảm giác chạm thật, đưa app xuống nền/mở lại, so sánh 30/60 fps, các tỷ lệ màn hình dọc, dấu tiếng Việt và profiling trên thiết bị vẫn cần kiểm chứng. Chưa kiểm chứng build Android/Web của phần lái này. Bằng chứng Web của M0 chỉ áp dụng cho cảnh tĩnh cũ.

## Chạy kiểm thử

Đóng Editor đang mở dự án trước. Gọi đúng Unity đã ghim phiên bản:

```sh
"/Applications/Unity/Hub/Editor/6000.3.25f1/Unity.app/Contents/MacOS/Unity" -batchmode -projectPath "$PWD" -runTests -testPlatform PlayMode -testResults Logs/riding-tests.xml -logFile Logs/riding-tests.log
python3 tools/unity_project.py export-ios
```

Không thêm `-quit` vào lệnh test; test runner tự thoát khi xong. Xem [Setup](SETUP.vi.md) để biết điều kiện ký/chạy trên thiết bị. Chỉ chạy `python3 tools/unity_project.py riding` khi cần tạo cảnh chưa tồn tại, không dùng để cập nhật cảnh đã commit.

## Kiểm tra nghiệm thu tiếp theo

Trên iPhone: khởi hành chậm, rẽ trái/phải, quay đầu, nhả để dừng, chạm lại không tăng ga đột ngột, thêm/nhấc ngón thứ hai, rồi đưa app xuống nền/mở lại. Ghi nhận độ khó khi rẽ, quãng đường phanh và tình huống kẹt. Tinh chỉnh cảm giác lái và thêm thao tác thoát kẹt an toàn trước phần ổ gà/xe buýt CR-012. Sau đó tiếp tục camera/độ rõ CR-013, Android CR-014 và playtest CR-015.
