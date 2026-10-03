# City Race GDD v0.2 Tiếng Việt

Tài liệu thiết kế game GDD v0.2 • Ngày 03/10/2026 • Bản nháp để phát triển bản chơi thử.

Bản 0.2 ghi nhận công nghệ đã chốt và việc kiểm chứng Web/Android sớm. Phạm vi gameplay giữ nguyên. Xem [tech stack](TECH_STACK.vi.md) và [quy ước kỹ thuật](ENGINEERING_GUIDELINES.vi.md).

Game đua xe máy tới đích qua giao thông giờ cao điểm ở Sài Gòn. Mỗi người xuất phát từ một nhà trong khu phố, trực tiếp lái bằng một ngón tay và gặp nhau trong dòng xe. Niềm vui đến từ quan sát, chọn đường, xử lý sự cố và những khoảnh khắc bất ngờ cùng bạn bè.

Tài liệu ghi lại định hướng đã thống nhất và đề xuất phạm vi triển khai đầu tiên. Các con số về thời lượng, số lượng nội dung và thông số chơi dưới đây là mục tiêu thử nghiệm, chưa phải kết quả đã kiểm chứng. Tên tạm thời của dự án là “City Race”.

[Bản tiếng Anh](GDD.en.md)

## 1 Định hướng đã thống nhất

- Phát hành iOS trước, dự kiến mở rộng Android và stack đã chọn hỗ trợ Web. Kiểm chứng Android/Web sớm; chưa chốt thời điểm phát hành hai nền tảng này.

- Chơi nhiều người cùng lúc, thi tới đích; người chơi trực tiếp điều khiển xe bằng một ngón tay.

- Bối cảnh đầu tiên là TP.HCM, gọi là Sài Gòn trong game.

- Mỗi người khởi hành từ một nhà riêng trong cùng khu phố rồi hội tụ.

- Hình ảnh tối giản nhưng không gian, phương tiện và hành vi cần giống thực tế.

- Cho chọn diện mạo hoặc giới tính, nhóm tuổi, phong cách ăn mặc và một số loại xe máy quen thuộc ở Việt Nam. Phạm vi lựa chọn ban đầu nhỏ.

- Tình huống đời thường gồm đường bị chắn, xe buýt ép sát, ổ gà, vi phạm bị cảnh sát thổi còi, thủng săm và mưa.

- Bối cảnh chuyến đi có thể gồm đi làm công sở hoặc cơ quan, giao hàng và chở người yêu.

**Mục tiêu trải nghiệm:** dễ bắt đầu, cảm nhận được việc lái xe, có cơ hội thể hiện kỹ năng và có chuyện để kể sau mỗi trận. Hỗn loạn phải có nguyên nhân và dấu hiệu; không liên tục phạt người chơi bằng sự cố ngẫu nhiên.

**Người chơi dự kiến:** người thích game ngắn, dễ chơi cùng bạn bè và thích sự gần gũi của đời sống đô thị Việt Nam. Đây là giả thuyết cần kiểm chứng khi thử game.

## 2 Một trận chơi

**Đề xuất cho bản đầu:** phòng riêng 2–4 người, cùng chơi kịch bản đi làm buổi sáng, mỗi trận khoảng 2–4 phút. Thử kỹ thuật trước với hai người; chưa cần ghép trận công khai.

1. Vào phòng, chọn nhanh nhân vật và xe; dùng lại lựa chọn ở trận sau.

2. Mỗi người nhận một nhà xuất phát khác nhau; cùng nhìn thấy đích đến.

3. Đếm ngược chung rồi khởi hành đồng thời.

4. Ra khỏi hẻm, nhập dòng xe, chọn tuyến và xử lý chướng ngại.

5. Tới vùng cổng cơ quan, giảm tốc và dừng để được ghi nhận hoàn thành.

6. Xem thứ hạng, thời gian và chọn chơi lại.

**Luật thắng đề xuất:** người hoàn thành đầu tiên thắng. Va chạm và xử lý vi phạm làm mất thời gian trực tiếp, không cộng thêm hệ điểm phức tạp sau trận. Chấm công là bối cảnh gây cảm giác vội; trễ giờ không lập tức loại người chơi.

Sau người đầu tiên về đích, cho những người còn lại thêm 30 giây; trận có giới hạn tổng 4 phút. Người chưa tới được ghi “Chưa hoàn thành”, không xếp trên người đã về đích. Đây là thông số thử, cần điều chỉnh theo chiều dài bản đồ.

Để tới đích hợp lệ, người chơi phải vào vùng cổng ở tốc độ thấp và không còn lượt xử lý vi phạm đang chờ. Chưa có đường tắt qua nhà, xuyên xe hay bỏ qua va chạm.

**Ví dụ:** bạn chọn hẻm để vượt một ô tô chắn đường, nhưng phải giảm tốc trước ổ gà. Người bạn chọn đường lớn gặp xe buýt đang ghé trạm. Hai người hội tụ ở ngã tư cuối; quyết định trước đó tạo chênh lệch mà cả hai đều hiểu được.

## 3 Điều khiển và thông tin trên màn hình

**Đã chốt:** một ngón tay điều khiển trực tiếp. **Cần thử:** cơ chế kéo tương đối, màn hình dọc và góc nhìn từ trên xuống hơi nghiêng.

Điểm chạm ban đầu làm tâm điều khiển tạm thời. Hướng kéo chỉ hướng lái; độ dài kéo quyết định mức ga. Đưa ngón tay về gần tâm để giảm tốc; nhấc tay để phanh nhanh. Chạm lại tạo tâm mới, không làm xe nhảy vị trí hoặc tăng tốc đột ngột.

Xe tiến và rẽ theo cung, có giới hạn tăng tốc, quay đầu và quãng phanh; không trượt ngang theo ngón tay. Bản đầu chưa có nút lùi. Nếu mắc kẹt hoàn toàn, có thao tác giữ để đặt lại xe gần vị trí vừa đi qua, chịu mất thời gian và không tiến gần đích hơn.

Vùng chạm ưu tiên ở phần dưới màn hình. Camera cho nhìn xa phía trước xe, không bị ngón tay che những chướng ngại cần phản ứng. Không cần nút còi hoặc nút tương tác riêng trong bản đầu; dừng đúng vùng sẽ kích hoạt tương tác.

Thông tin thiết yếu gồm thời gian, vị trí của mình, hướng tới đích và tên bạn bè gần đó. Khi chưa xác định được thứ hạng trên các đường nhánh, ưu tiên hiển thị vị trí bạn bè thay vì thứ hạng nhảy liên tục. Cảnh báo cần có hình ảnh đi cùng âm thanh.

## 4 Khu phố và giao thông Sài Gòn

**Đề xuất bản đồ đầu:** khu phố hư cấu có bốn nhà xuất phát, hẻm nhánh, đường lớn, một ngã tư có đèn và cổng cơ quan. Có 2–3 tuyến khả dụng. Các nhánh đầu hội tụ sau khoảng 15–20 giây để người chơi sớm thấy nhau.

Cân bằng vị trí xuất phát bằng thời gian và độ khó di chuyển, không chỉ bằng khoảng cách. Nhà gần đường lớn có thể khó nhập dòng hơn. Khi thử nghiệm, đổi nhà giữa các lượt để phát hiện tuyến có lợi thế cố định.

Nhà phố, cửa cuốn, mái hiên, quán ăn sáng, biển tiếng Việt, cây và xe dựng ven đường tạo bản sắc. Chi tiết trang trí phải tách bạch với phần có thể va chạm. Hình dáng và tỷ lệ tương đối giúp phân biệt xe số, tay ga, ô tô và xe buýt.

Giao thông do máy điều khiển cần dừng trước đèn đỏ, giữ khoảng cách, nhập từ hẻm và phản ứng khi đường bị chắn. Xe buýt báo rẽ rồi ghé trạm. Xe không được sinh ra ngay trước mặt hoặc đổi hướng tức thời.

Giữ mật độ tạo nhiều quyết định nhưng luôn có đường xử lý: chờ ngắn, đi chậm hoặc đổi tuyến. Nếu dòng xe bế tắc kéo dài, hệ thống phải có cách giải tỏa tự nhiên.

Âm thanh dự kiến gồm động cơ, còi ngắn, tiếng phố và mưa ở bản sau. Hài hước đến từ diễn biến và phản ứng nhỏ của nhân vật; không dựa vào hình thể phóng đại hoặc thương tích.

## 5 Tình huống và hậu quả

Các cơ chế trong bảng là đề xuất thiết kế. “Bản đầu” là màn chơi hoàn chỉnh nhỏ sau khi kiểm chứng điều khiển và multiplayer.

| Tình huống | Dấu hiệu và ứng phó | Hậu quả đề xuất | Phạm vi |
| --- | --- | --- | --- |
| Ổ gà | Nhìn thấy mặt đường xấu; né hoặc giảm tốc | Cán nhanh gây chao đảo và mất tốc độ ngắn, vẫn có thể điều khiển | Bản đầu |
| Xe buýt ghé trạm | Báo rẽ và chuyển dần vào lề; phanh hoặc vòng khi an toàn | Chiếm lối tạm thời; va quẹt làm chậm | Bản đầu |
| Ô tô chắn lối | Thấy xe đang dừng hoặc xoay trở; chờ, luồn nếu đủ chỗ hoặc đổi tuyến | Mất thời gian theo quyết định; không xuyên qua xe | Bản đầu |
| Cảnh sát thổi còi | Bản đầu chỉ thử lỗi vượt đèn đỏ tại ngã tư có kiểm soát; chỉ rõ lỗi và vùng dừng | Tấp vào xử lý ngắn; chưa xử lý thì chưa được hoàn thành cuộc đua | Bản đầu |
| Mưa | Mây tối và hạt mưa xuất hiện trước khi độ bám giảm; đi chậm hơn | Phanh dài hơn, tầm nhìn giảm vừa phải; ảnh hưởng chung cả trận | Sau |
| Thủng săm | Đề xuất gắn với việc cán đoạn nguy hiểm có dấu hiệu thay vì rút thăm vô cớ | Xe chậm, tìm tiệm vá; rút ngắn thời gian sửa | Sau |

Không áp dụng thêm hình phạt chồng lên một va chạm khi người chơi vừa mất kiểm soát. Va quẹt nhẹ giữa người chơi làm chậm và lệch hướng ít; cơ chế thoát kẹt cần ngăn việc đứng chắn mãi một người.

Các sự kiện ảnh hưởng chung phải thống nhất cho mọi người trong trận. Không tạo ổ gà riêng ngay trước người đang dẫn đầu để ép thay đổi kết quả.

Cơ chế vi phạm là luật chơi giản lược. Chưa mô phỏng đầy đủ nghiệp vụ hoặc mức phạt thực tế. Khi bổ sung lỗi mới, phải kiểm tra tính đúng và diễn đạt rõ trong game.

## 6 Nhân vật và xe

**Đã chốt:** có cá nhân hóa tối giản. **Đề xuất số lượng để thử:**

| Nhóm | Lựa chọn ban đầu |
| --- | --- |
| Diện mạo và giới tính | Hai mẫu cơ bản, không giới hạn trang phục theo mẫu |
| Nhóm tuổi | Trưởng thành trẻ và trung niên |
| Trang phục | Công sở, bụi bặm, đi chơi |
| Xe | Xe số phổ thông, tay ga nhỏ, tay ga lớn |
| Màu | Ba màu áo và ba màu xe; mũ bảo hiểm phối sẵn |

Các lựa chọn dùng chung bộ phận để hạn chế lượng nội dung phải làm. Ưu tiên dáng người, áo, túi và màu xe nhìn được từ camera; chưa chỉnh chi tiết khuôn mặt. Nhân vật luôn đội mũ bảo hiểm.

Bản đầu đề xuất các xe có hiệu năng và vùng va chạm tương đương để kiểm chứng cuộc đua công bằng. Đây là giản lược tạm thời; hình dáng vẫn cần nhận ra từng nhóm xe. Nếu khác biệt kích thước hình ảnh làm va chạm khó hiểu, điều chỉnh mẫu xe trước khi thêm chỉ số.

Phong cách ăn mặc không khóa nghề nghiệp hay kịch bản. Chưa dùng logo thương hiệu hoặc nền tảng giao hàng cụ thể. Nhà là vị trí khởi hành được chọn trong phòng, chưa phải tài sản cần mua hoặc nâng cấp.

## 7 Phạm vi thực hiện

**Bản thử cảm giác lái:** một người trên iPhone, một xe, một đoạn đường, vài xe do máy điều khiển, một ổ gà và một xe buýt. Dùng hình khối đơn giản. Mục tiêu là xác nhận điều khiển dễ hiểu và vui.

**Bản thử multiplayer:** hai người, hai nhà, chung dòng xe và một đích. Kiểm tra đồng bộ vị trí, chướng ngại, va chạm và kết quả. Nếu mất kết nối, thông báo rõ; xe mất kết nối không được chắn người khác vô thời hạn.

**Bản đầu chơi được hoàn chỉnh:** một khu phố, kịch bản đi làm buổi sáng, phòng riêng 2–4 người, bốn tình huống trong phạm vi bản đầu, cá nhân hóa cơ bản, kết quả và chơi lại. Không bắt buộc tài khoản để bắt đầu thử nghiệm.

**Để sau:** mưa và ngập, thủng săm, tan tầm, giao hàng, chở người yêu, bản đồ mới, khác biệt hiệu năng xe, trang trí nhà, ghép trận công khai, xếp hạng mùa, kinh tế trong game và kiếm tiền. Phát hành Android là hướng mở rộng; kiểm chứng kỹ thuật Android và Web thuộc các bản thử sớm.

Đã chọn Unity + C#. Kiến trúc khởi đầu dùng Unity 6.3 LTS, URP và Input System, với NGO, Unity Transport và Multiplayer Services Sessions/Relay là bộ thư viện mạng thử nghiệm đầu tiên. Xem tech stack để biết điều kiện kiểm chứng, gồm độ trễ và tương thích Web. Ngân sách, lịch sản xuất, phiên bản package chính xác và thiết bị tối thiểu vẫn chưa chốt.

## 8 Giả thuyết cần kiểm chứng

Các tiêu chí dưới đây là ngưỡng quyết định nội bộ đề xuất cho nhóm thử đầu tiên gồm 5–8 người, không phải bằng chứng đại diện thị trường.

| Giả thuyết | Cách kiểm tra và tín hiệu mong muốn |
| --- | --- |
| Một ngón tay đủ dễ dùng | Ít nhất 4/5 người có thể xuất phát, rẽ và dừng trong một phút sau hướng dẫn ngắn |
| Điều khiển tạo cảm giác công bằng | Sau phần lớn va chạm, người chơi giải thích được nguyên nhân và biết lần sau nên làm gì |
| Cảm giác lái đủ vui | Đa số người thử tự muốn chơi thêm một lượt; hỏi rõ họ thích hoặc khó chịu ở đâu |
| Nhà xuất phát không quyết định người thắng | Đổi nhà qua nhiều lượt, đo thời gian với giao thông cố định; sửa nhánh có lợi thế lặp lại |
| Multiplayer tăng sự hấp dẫn | Hai máy nhìn thấy cùng sự kiện và cùng kết quả; không có va chạm lệch rõ làm thay đổi người thắng |
| Cá nhân hóa đủ nhận diện | Người chơi nhận ra xe của mình và phân biệt bạn bè khi đang di chuyển |

Nếu người chơi liên tục mất dấu xe, điều khiển khó hoặc không hiểu vì sao bị phạt, sửa các điểm đó trước khi thêm nội dung.

Những quyết định cần chốt sau bản thử: cơ chế kéo và phanh, góc camera, mức va chạm giữa người chơi, thời lượng trận, cách thoát kẹt và độ khác biệt giữa các xe.

**Bước tiếp theo:** dựng bản thử cảm giác lái trên iPhone theo phạm vi ở mục 7, thêm kiểm tra Android/Web sớm theo tech stack, ghi kết quả theo mục 8 và cập nhật GDD v0.3.
