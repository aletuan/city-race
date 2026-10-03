# City Race Quy ước kỹ thuật

Phiên bản 0.1 • 03/10/2026 • [English](ENGINEERING_GUIDELINES.en.md)

Đây là quy ước cho việc triển khai sau này, không khẳng định game hoặc benchmark đã tồn tại. Đọc [tech stack](TECH_STACK.vi.md) và [GDD](GDD.vi.md) trước. Ưu tiên cách triển khai nhỏ, rõ ràng, đáp ứng mốc hiện tại.

## 1 Áp dụng SOLID vào ranh giới thực tế

| Nguyên tắc | Quy ước City Race |
| --- | --- |
| Single responsibility | BikeMotor di chuyển xe; RaceRules xác nhận về đích; SessionGateway quản lý kết nối; BikeView hiển thị trạng thái. Tránh GameManager ôm mọi việc. |
| Open/closed | Mở rộng hành vi thực sự có biến thể bằng strategy nhỏ hoặc dữ liệu. Bắt đầu đơn giản; không dựng plugin system cho một ổ gà. |
| Liskov substitution | Các bản input/storage/session tuân cùng contract, bao gồm hủy và lỗi. Test hành vi quan sát được, không test cây kế thừa. |
| Interface segregation | Tách IRaceClock, IRideInput, IPreferencesStore khi bên dùng cần ranh giới đó. Tránh interface service chứa toàn bộ game. |
| Dependency inversion | Core định nghĩa contract cần dùng; infrastructure hiện thực. Luật chơi không tìm NetworkManager toàn cục hay đọc trạng thái UI. |

Ưu tiên composition hơn kế thừa sâu. Dùng interface ở ranh giới bên ngoài hoặc nơi có biến thể thật, không tạo cho mọi class. Inject đồng hồ và randomness khi cần tái lập. Luật domain không phụ thuộc scene Unity; truy vấn chuyển động đặc thù Unity đặt trong Gameplay adapter. Adapter mạng có thể bọc singleton của SDK bên trong nhưng không phát tán nó vào luật chơi.

## 2 Thư mục và quy ước C#

Khi có code, dùng cấu trúc này; không tạo thư mục rỗng chỉ để đủ bộ:

- Assets/CityRace/Runtime/Core, Gameplay, Infrastructure, Presentation, Bootstrap.
- Assets/CityRace/Editor cho code chỉ chạy trong editor.
- Assets/CityRace/Tests/EditMode và PlayMode.
- Assets/CityRace/Content cho scene, prefab, tuning, hình ảnh và âm thanh.
- docs/decisions cho quyết định kiến trúc có ngày.

Namespace theo phần sở hữu, ví dụ CityRace.Gameplay.Riding. Một type chính mỗi file; tên file trùng type. PascalCase cho type, method, property, thành viên enum; interface có tiền tố I; camelCase cho tham số/biến local; _camelCase cho private field. Dùng bốn dấu cách, UTF-8, LF và newline cuối file. Dùng dấu ngoặc khối nhất quán.

Dùng private serialized field cho cấu hình Inspector, property chỉ đọc cho bên dùng. Tránh public mutable field. Ghi đơn vị ở giá trị dễ nhầm, như speedMetersPerSecond hoặc stopDurationSeconds. Dùng ID ổn định khi lưu hoặc gửi mạng, không dùng tên hiển thị hoặc Unity instance ID.

Dùng tính năng C# được editor đã khóa hỗ trợ; không mặc định API .NET desktop có trên mọi nền tảng. Mặc định tránh đăng ký bằng reflection quá nhiều và sinh mã runtime. Nếu stripping/AOT cần metadata bảo toàn, giới hạn phạm vi và kiểm chứng release build.

Comment giải thích ý định và ràng buộc. Khi thêm UI, chuỗi hiển thị dùng localization key ổn định; không nối các mảnh câu đã dịch. Font TextMeshPro phải có ký tự tiếng Việt và được thử dấu.

## 3 Trạng thái thời gian và vòng đời

Mô hình hóa rõ Lobby → Countdown → Racing → Results cùng nhánh hủy/mất kết nối. Kiểm tra lệnh theo giai đoạn. Xử lý về đích và hoàn tất hình phạt phải idempotent để sự kiện mạng lặp không tính hai lần.

ScriptableObject đã soạn không thay đổi trong lúc chơi. Mỗi trận/người chơi có trạng thái riêng. Không giữ reference người chơi qua reload scene; reset static state khi vào Play Mode, kể cả khi tắt domain reload.

Đọc input qua Input System, đệm lệnh mới nhất, cập nhật chuyển động theo bước mô phỏng cố định. Không nhân chuyển động theo số frame render. Camera và làm mượt hình ảnh theo nhịp render; dùng LateUpdate khi phù hợp. Chỉ một bên điều khiển bước mô phỏng, không đồng thời chạy physics tự động và thủ công.

Dùng đồng hồ authority cho kết quả trận; đồng hồ presentation cho hiệu ứng/UI. Phân biệt fixed timestep và network tick, ghi quan hệ giữa chúng. Cùng random seed không khiến physics Unity deterministic trên các kiến trúc máy.

Subscribe/unsubscribe đối xứng. Hủy tác vụ async khi owner bị destroy, rời phòng hoặc tắt ứng dụng. Tránh async void trừ event handler bắt buộc; quan sát exception. Trở về main thread Unity trước khi thao tác Unity object. Không chặn main thread bằng Wait, Result hoặc network I/O đồng bộ.

Khi mất focus, nhả input đang giữ và áp dụng chính sách phanh/mất kết nối. Thử ngắt quãng, đưa app xuống nền và tab trình duyệt bị treo. Không chạy bù mô phỏng vô hạn khi quay lại.

## 4 Tính đúng của multiplayer

Chỉ authority đổi thứ tự về đích, đèn giao thông, hình phạt và chướng ngại chung. Kiểm tra quyền sender, khoảng input, độ cũ sequence, kích thước message và tần suất lệnh. Client không tự khai vị trí cuối hoặc thời gian trận đáng tin.

Tách snapshot chuyển động thường xuyên khỏi sự kiện reliable ít gặp. Chọn cách gửi theo transport; WSS dùng kênh reliable có thứ tự nên không hứa hành vi mất gói như UDP cho Web. Không gửi payload lớn hoặc asset trong message gameplay.

Giới hạn bộ đệm input/lịch sử; dùng phiên bản protocol/content rõ ràng và network ID ổn định. Từ chối phiên bản phòng không tương thích bằng thông báo dễ hiểu. Xử lý sự kiện lặp, cũ hoặc đến muộn mà không cộng lại phần thưởng/hình phạt.

Nội suy hình ảnh xe khác. Nếu làm prediction local, hiệu chỉnh theo snapshot authority với lịch sử/độ hiệu chỉnh có giới hạn; tách làm mượt trang trí khỏi trạng thái va chạm. Tránh nhiều component cùng ghi một transform.

Lợi thế host và gian lận vẫn là giới hạn của mô hình ban đầu. Mất host kết thúc trận, không âm thầm chuyển host. Authority xóa hoặc tắt va chạm của người mất kết nối. Log đủ thời gian để tìm lỗi lệch trạng thái nhưng không lưu token hoặc thông tin cá nhân không cần thiết.

## 5 Ngân sách và đo hiệu năng

Đây là **mục tiêu kỹ thuật tạm thời**, phải cập nhật từ đo đạc trên thiết bị tối thiểu được chỉ rõ, không phải cam kết quảng bá.

| Phần | Mục tiêu hoặc quy tắc ban đầu |
| --- | --- |
| Render native | Hướng tới 60 fps, khoảng 16,7 ms/frame; CPU và GPU đều cần dư địa |
| Web/mobile cấu hình thấp | Kiểm chứng 30 fps, khoảng 33,3 ms, nếu không giữ được 60; không đổi tốc độ hoặc luật chơi |
| Cấp phát gameplay | Hướng tới 0 B/frame trong vòng lặp gameplay nóng do ta viết sau warm-up; đo SDK/UI riêng |
| Mô phỏng | Bắt đầu 50 Hz; đo và ghi thay đổi, không tăng tần suất để che lỗi |
| Snapshot mạng | Thử từ 20 Hz; chỉnh theo độ trễ, chất lượng hiệu chỉnh và băng thông đo được |
| Thử dài | Ít nhất 10 phút và nhiều vòng chơi lại, không tăng đều bộ nhớ còn bị giữ |
| Bộ nhớ và tải Web | Đo peak memory và payload nén ban đầu ở build đầu có asset; chốt giới hạn số trước khi mở rộng asset |
| Tải đại diện | Ghi số xe giao thông hoạt động, số người chơi, hiệu ứng và quality tier ở mỗi lần so sánh |

FPS trung bình chưa đủ. Ghi phân bố frame time gồm p95 và spike lớn nhất, nút thắt CPU/GPU, GC allocation, bộ nhớ bị giữ, băng thông và sự kiện hiệu chỉnh nhìn thấy khi phù hợp. Dùng Unity Profiler trên development build của thiết bị để chẩn đoán, rồi xác nhận trải nghiệm ở non-development build. Số liệu Editor không phải số liệu thiết bị.

Tránh tìm scene, GetComponent, LINQ, format chuỗi, closure và collection tạm lặp lại trong vòng lặp nóng đã đo. Cache reference và dùng lại buffer khi có ích. Không cấm tuyệt đối các thao tác này trong setup/tooling.

Pool xe giao thông/hiệu ứng tái sử dụng thường xuyên, có trần số lượng và reset đầy đủ. Đo bộ nhớ pool; pool vô hạn là rò bộ nhớ. Reset ownership mạng, subscription và physics state khi dùng lại network object.

Dùng collider đơn giản và collision-layer matrix giới hạn. Xe xa có thể quyết định ít thường xuyên hơn nhưng không bỏ kiểm tra an toàn gần người chơi. Đặt trần xe/hiệu ứng hoạt động. Chỉ thêm jobs/ECS hoặc allocator riêng khi đã đo được nút thắt và kiểm chứng nền tảng.

Dùng chung material, hạn chế shader variant, transparency, realtime light và shadow. Đo draw call/overdraw trước khi đổi batching. Cấu hình kích thước/nén texture và audio import theo nền tảng. Quality tier chỉ đổi render, không đổi chướng ngại hoặc va chạm có thẩm quyền.

## 6 Quy tắc đa nền tảng

Một hiện thực gameplay cho mọi nền tảng. Giới hạn compilation symbol đặc thù nền tảng trong adapter, storage, transport và cấu hình build.

Chuyển cảm ứng và chuột thành cùng RideCommand. Xử lý safe area, thay đổi độ phân giải, pointer cancellation và ưu tiên UI trước input lái. Gameplay không phụ thuộc pixel màn hình; chuẩn hóa khoảng kéo và thử bố cục dọc với nhiều tỷ lệ.

Web phải xử lý yêu cầu kích hoạt âm thanh, mất focus, storage không dùng được và tải file lỗi. Không mặc định save local tồn tại mãi. Giữ C# trên trình duyệt trong giới hạn Unity công bố; không dùng background thread như cách sửa chung cho mọi nền tảng. [Giới hạn Unity Web](https://docs.unity3d.com/6000.3/Documentation/Manual/webgl-technical-overview.html).

Xem save là dữ liệu không đáng tin: có phiên bản, giới hạn giá trị, phục hồi dữ liệu hỏng, migrate hoặc reset an toàn. Native/browser dùng storage adapter. Settings lưu local; kết quả chính thức đến từ authority của trận.

Không nhúng secret trong client, kể cả Web. Signing material và service credential nằm ngoài Git. Hiển thị lỗi dịch vụ có thể phục hồi và cho lái offline khi dịch vụ phòng không hoạt động.

## 7 Git asset và dependency

Commit .meta cùng asset, giữ GUID khi di chuyển/đổi tên và ưu tiên thao tác có nhận biết Unity. Dùng Force Text serialization. Không tùy tiện sửa YAML scene/prefab hoặc tạo lại toàn bộ metadata để chữa merge.

Ignore Library, Temp, Obj, Logs, UserSettings, build sinh ra và credential local. Theo dõi manifest/lock, project settings, asset nguồn và metadata cần thiết. Dùng LFS cho asset nguồn nhị phân lớn; cấu hình attributes trước khi thêm. Không đưa code text hay .meta vào LFS.

Commit message kỹ thuật bằng tiếng Anh, ví dụ feat(riding): add braking input. Giữ phạm vi thay đổi gọn. Khóa dependency; giải thích package mới/nâng cấp bằng mục đích, giấy phép tương thích và bằng chứng nền tảng. Không tự nâng cấp trong công việc không liên quan.

Với thay đổi kiến trúc đáng kể, tạo quyết định song ngữ ngắn gồm bối cảnh, quyết định, đánh đổi, kiểm chứng và lựa chọn bị thay thế. Hai bản phải tương đương về nghĩa; không dịch identifier code.

## 8 Kiểm chứng và hoàn thành

| Thay đổi | Bằng chứng liên quan cần có |
| --- | --- |
| Luật trận | EditMode cho thứ tự, về đích lặp, hình phạt chưa xử lý, input/giai đoạn không hợp lệ |
| Lái xe hoặc va chạm | PlayMode cùng bằng chứng thiết bị/manual ở frame rate liên quan |
| Multiplayer | Hai client và host, kết quả thống nhất, mất kết nối/sự kiện lặp/đến muộn và native/Web chơi chung |
| Render/asset/UI | Ảnh target và profiler với tải đại diện; tiếng Việt và safe area |
| Nền tảng/package/build | Build nền tảng bị ảnh hưởng; compile Editor chưa đủ |
| Tài liệu | Kiểm tra link local, cặp ngôn ngữ, quyết định nhất quán và git diff --check |

Thử mạng từ baseline local, rồi khoảng 100 ms và 200 ms RTT mô phỏng với jitter được ghi lại. Thử mất gói ở đường datagram và nghẽn/ngắt ở WSS. Chỉ rõ giá trị simulator là trễ một chiều hay RTT. Quan sát phản hồi lái, độ hiệu chỉnh và kết quả về đích; kết nối được phòng chưa đủ xác nhận stack.

Test hành vi và ranh giới, không bám chi tiết private. Tránh test chỉ lặp phép gán. Thêm regression test cho lỗi thật; không chạy đi chạy lại bộ test không liên quan sau khi kiểm tra cần thiết đã đạt.

Trước khi bàn giao:

- Xác nhận phạm vi, chiều dependency và chủ thể có thẩm quyền.
- Review file thay đổi, metadata, serialized reference và package lock.
- Chạy test/build liên quan có sẵn, đọc kết quả.
- Ghi phiên bản thiết bị/trình duyệt/editor/package và giới hạn của kết luận hiệu năng.
- Cập nhật tài liệu tiếng Anh/Việt khi đổi kiến trúc hoặc gameplay.
- Báo trung thực kiểm tra chưa chạy. Không bịa lệnh, kết quả CI hoặc khả năng truy cập thiết bị.

Cho tới khi có Unity project và build script, repo này chỉ chạy được kiểm tra tài liệu. Bổ sung lệnh setup/build tái lập được khi các entry point đã được triển khai.
