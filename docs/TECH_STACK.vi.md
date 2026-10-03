# City Race Tech Stack

Phiên bản 0.1 • 03/10/2026 • [English](TECH_STACK.en.md)

## 1 Quyết định và trạng thái

**Định hướng đã chốt:** Unity + C#, ưu tiên iOS, tính đến Android và Web ngay từ đầu. Dùng chung logic và nội dung, nhưng tạo và kiểm chứng bản build riêng cho từng nền tảng.

**Mặc định triển khai:** các lựa chọn dưới đây là kiến trúc khởi đầu, chưa phải bằng chứng tích hợp thành công. Repo chưa có Unity project. Chưa chốt bản vá editor, phiên bản package, OS/trình duyệt/thiết bị tối thiểu, ngân sách hosting và số liệu hiệu năng.

| Thành phần | Lựa chọn khởi đầu | Lý do hoặc giới hạn |
| --- | --- | --- |
| Engine | Unity 6.3 LTS | Nền tảng ổn định cho dự án; khóa bản vá đã thử khi khởi tạo |
| Ngôn ngữ | C# được editor đã khóa hỗ trợ | Tránh tính năng ngôn ngữ/runtime không tương thích và dependency không cần thiết |
| Đồ họa | Universal Render Pipeline (URP), 3D đơn giản, camera từ trên xuống | Dùng chung cảnh mobile/web; ánh sáng và hiệu ứng vừa phải |
| Gameplay | GameObjects, MonoBehaviour adapter nhỏ và luật chơi bằng C# thuần | Độ phức tạp phù hợp cuộc đua nhỏ |
| Điều khiển | Unity Input System | Cảm ứng và chuột cho web/desktop cùng chuyển thành một loại lệnh |
| Giao diện | Unity UI (uGUI) với TextMeshPro | Menu/HUD nhỏ; font phải có đủ ký tự tiếng Việt |
| Thư viện mạng thử nghiệm | Netcode for GameObjects (NGO) + Unity Transport | Phải vượt qua thử nghiệm chuyển động và va chạm |
| Dịch vụ phòng | Multiplayer Services SDK thống nhất, Sessions và Relay | Tạo/vào phòng riêng; không triển khai trùng hai hệ quản lý vòng đời phòng |
| Danh tính | Unity Authentication đăng nhập ẩn danh khi chơi online | Không cần màn đăng ký; bản lái offline không phụ thuộc dịch vụ |
| Tùy chọn local | Dữ liệu nhỏ có phiên bản qua storage adapter | Nhân vật, xe, âm thanh, đồ họa; không quyết định kết quả trận |
| Kiểm thử | Unity Test Framework, EditMode, PlayMode và thử thiết bị/trình duyệt | Kiểm tra luật chơi và bằng chứng tích hợp thực tế |
| Quản lý mã nguồn | GitHub; Git LFS khi thêm tài nguyên nhị phân lớn | Lưu code, metadata, settings và package lock |

Unity xác định 6.3 là dòng LTS và cung cấp công cụ đồ họa/mobile. [Hướng dẫn bản phát hành](https://unity.com/blog/unity-6-3-lts-is-now-available), [nhóm công cụ Unity](https://docs.unity.com/en-us/engine/6000.6/manual/packages-list/feature-sets).

## 2 Cam kết nền tảng

| Nền tảng | Cam kết | Bằng chứng cần có ban đầu |
| --- | --- | --- |
| Ứng dụng iOS | Nền tảng phát hành chính | Build qua Xcode trên iPhone thật; kiểm tra cảm ứng, safe area, hiệu năng, vòng đời |
| Ứng dụng Android | Hướng mở rộng; thử tính tương thích sớm | Build trên máy Android thật, điều khiển và chơi chung phòng |
| Web desktop | Mục tiêu thử nghiệm và tương thích sớm; chưa chốt ngày phát hành | Build chạy qua HTTPS, chuột, tải game và chơi chung native/web |
| Web mobile | Thử tính khả thi sớm, không cam kết mọi thiết bị | Safari trên iPhone, Chrome trên Android; cảm ứng, bộ nhớ, mở âm thanh và chạy nền |

Unity Web chạy trên các trình duyệt hỗ trợ công nghệ đồ họa web và WebAssembly. Hỗ trợ trình duyệt không bảo đảm hiệu năng. Chọn danh sách thiết bị/trình duyệt cụ thể trước khi công bố tương thích. [Tương thích trình duyệt Unity](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/webgl/intro/browsercompatibility).

Dùng macOS/Xcode cho iOS và bộ công cụ Android tương thích editor. Ghi phiên bản thực tế vào hướng dẫn cài đặt khi đã có project. Hosting Web cần HTTPS, header đúng cho WebAssembly và file nén, cùng màn tải/lỗi; build thành công chưa chứng minh hosting hoạt động.

Đặt API chỉ có trên native sau adapter. Bộ nhớ lưu trữ của trình duyệt có giới hạn và có thể bị xóa; tùy chọn phải phục hồi được về mặc định. Không mặc định mọi nền tảng đều hỗ trợ filesystem tùy ý, chạy nền, managed thread hay sinh mã động. [Giới hạn Unity Web](https://docs.unity3d.com/6000.3/Documentation/Manual/webgl-technical-overview.html).

## 3 Ranh giới runtime

Dùng bốn ranh giới dependency nhỏ, chỉ tạo khi triển khai thực sự cần:

- **Core:** trạng thái trận, xác nhận về đích, hình phạt, schema settings, ID và lệnh. Không phụ thuộc UnityEngine, NGO, SDK dịch vụ hay UI.
- **Gameplay:** chuyển input thành lệnh, chuyển động theo bước thời gian cố định, adapter giao thông/va chạm; phụ thuộc Core và Unity.
- **Infrastructure:** hiện thực mạng, phòng chơi, lưu trữ và nền tảng; phụ thuộc contract của Core và SDK tương ứng.
- **Presentation:** camera, model, animation, âm thanh, HUD và menu; quan sát trạng thái và gửi lệnh.

Bootstrap là nơi ghép dependency cụ thể. Service C# thuần dùng constructor injection; MonoBehaviour dùng reference được serialize hoặc khởi tạo rõ ràng. Dùng assembly definition để giữ ranh giới khi module tồn tại; không dựng assembly rỗng hoặc framework dependency injection phức tạp trước nhu cầu.

Luồng điển hình: input → RideCommand → xử lý chuyển động/luật chơi có thẩm quyền → snapshot/sự kiện → hình ảnh và HUD. Phản hồi hình ảnh và dự đoán phía client không cho client quyền quyết định về đích hoặc hình phạt.

ScriptableObject chứa dữ liệu tuning do người phát triển soạn, sao chép sang trạng thái từng trận khi cần. Không dùng asset chung làm trạng thái trận đang thay đổi.

## 4 Thử nghiệm multiplayer

Bắt đầu bằng một người chơi iOS hoặc desktop native làm host, các client kết nối qua Relay. Host quyết định giao thông, thời gian, va chạm và kết quả. Relay chuyển tiếp dữ liệu; không chạy mô phỏng và không làm trận do người chơi host trở nên chống gian lận tuyệt đối.

Client trình duyệt dùng WebSocket bảo mật qua Unity Transport tương thích. Native dùng transport Relay được hỗ trợ. Kiểm chứng tổ hợp với package đã khóa. Trình duyệt làm host nằm ngoài phạm vi nghiệm thu đầu tiên; không mặc định có thể dùng socket nhận kết nối trực tiếp. [Tích hợp Relay và NGO](https://docs.unity.com/en-us/mps-sdk/tutorials/relay-and-ngo).

Dùng vòng đời Sessions làm đường tích hợp chính; ví dụ Relay cấp thấp là tài liệu tham khảo, không phải hệ quản lý phòng thứ hai. Đăng nhập ẩn danh đáp ứng “không cần tài khoản” ở giao diện nhưng vẫn tạo danh tính dịch vụ; xóa dữ liệu local/trình duyệt có thể làm mất danh tính này.

Client gửi input có giới hạn và sequence/tick. Authority kiểm tra chủ sở hữu, khoảng giá trị, tần suất và giai đoạn trận. Đồng bộ trạng thái chuẩn cùng sự kiện quan trọng; nội suy hình ảnh xe khác. Không gửi transform của mọi vật trang trí ở mỗi frame.

**Điều kiện kiểm chứng:** anticipation của NGO không phải hệ rollback/replay prediction đầy đủ. Phải chứng minh lái phản hồi nhanh và hiệu chỉnh chấp nhận được trên máy thật trước khi mở rộng nội dung multiplayer. Nếu cần, thử bộ dự đoán chuyển động nhỏ với hiệu chỉnh từ authority; cân nhắc giải pháp mạng khác nếu chi phí này quá lớn. Ghi quyết định thay thế, không âm thầm đưa thêm stack mạng thứ hai. [Tài liệu anticipation của NGO](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/advanced-topics/client-anticipation.html).

Chính sách vòng đời ban đầu: khóa vào trận sau countdown; mất host thì kết thúc với thông báo rõ; client mất kết nối không còn cản đường và được đánh dấu disconnected. Vào lại trận đang chạy, chuyển host, dedicated server, ghép trận công khai, xếp hạng và bảo đảm chống gian lận nằm ngoài bản đầu.

## 5 Chuyển động và nội dung

Bắt đầu bằng bộ điều khiển xe arcade giới hạn trên mặt đường, hình va chạm đơn giản và đồ thị làn/tuyến được thiết kế sẵn. Unity xử lý truy vấn va chạm; luật chơi không phụ thuộc góc nghiêng trang trí hoặc animation bánh xe. Làm mượt hình ảnh độc lập với tick mô phỏng.

Chưa bắt đầu bằng hệ treo thực, tuning WheelCollider, tìm đường toàn thành phố hoặc viết lại bằng ECS. Chỉ tăng độ phức tạp khi có nhu cầu gameplay được chứng minh. Giao thông đáng tin là yêu cầu thiết kế; mô phỏng xe máy chính xác tuyệt đối không phải yêu cầu.

Dùng hình học 3D đơn giản, material dùng chung và ánh sáng hạn chế trong URP. Góc nghiêng camera và lựa chọn orthographic/perspective vẫn cần thử. Khi thay greybox bằng asset, giữ khả năng đọc va chạm và nét Sài Gòn.

## 6 Phiên bản và khởi tạo project

Khi khởi tạo, chọn bản vá 6.3 ổn định cùng package tương thích qua Package Manager. Lưu editor trong ProjectSettings/ProjectVersion.txt, dependency trong Packages/manifest.json và Packages/packages-lock.json. Thử nâng cấp trên branch, kèm build các nền tảng bị ảnh hưởng.

Dùng các nhóm package: URP, Input System, uGUI/TextMeshPro theo editor, NGO, Unity Transport, Multiplayer Services, Authentication và Test Framework. Chốt phiên bản chính xác khi khởi tạo; chỉ thêm công cụ profiling/mô phỏng mạng khi dùng. Tài liệu này không khẳng định đã cài package nào.

Unity project đặt ở gốc repo: Assets/, Packages/, ProjectSettings/ cùng docs/ và AGENTS.md. Bật visible metadata và text asset serialization. Cấu hình ignore cho Unity và LFS trước khi thêm tài nguyên nhị phân lớn; giữ .meta đi cùng asset.

## 7 Trình tự và bằng chứng hoàn thành

1. Khởi tạo: khóa phiên bản, tạo project tối thiểu, ghi yêu cầu build, kiểm chứng build iPhone và Web đã host.
2. Lái xe: một xe, đường, ổ gà và xe buýt; kiểm tra cảm ứng/chuột, đo hiệu năng ban đầu. Thử build Android trong giai đoạn này.
3. Multiplayer: hai thiết bị, sau đó native/Web chơi chung; thử độ trễ, mất kết nối và kết quả về đích thống nhất.
4. Bản đầu: phòng riêng 2–4 người, bốn nhà, đi làm buổi sáng và sự kiện trong GDD. Đo lại hiệu năng với lượng giao thông đại diện.

Chỉ hoàn thành mốc khi có bằng chứng build/thiết bị, không chỉ compile trong Editor. Tài liệu này không triển khai cloud, CI hoặc code game.

## 8 Điểm còn mở

Thiết bị/OS tối thiểu; phiên bản package chính xác; mật độ giao thông và ngân sách tick mạng; chi phí hosting; chiến lược prediction cuối cùng; browser-host nếu sau này được yêu cầu; và Web phát hành dạng demo hay game đầy đủ.

Giải quyết bằng thử nghiệm tập trung và quyết định kiến trúc có ngày trong docs/decisions/. Đồng bộ quyết định tiếng Anh và tiếng Việt. Xem [quy ước kỹ thuật](ENGINEERING_GUIDELINES.vi.md) để triển khai và kiểm chứng.
