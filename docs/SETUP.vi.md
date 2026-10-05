# City Race Hướng dẫn thiết lập

Phiên bản 0.1 • 05/10/2026 • [English](SETUP.en.md)

## M0: mốc kỹ thuật đầu tiên

M0 là Milestone 0: thiết lập quy trình lặp lại được từ mã nguồn đến cảnh tối thiểu chạy trên thiết bị thật. Đây chưa phải bản thử lái xe (M1). Theo dõi [lộ trình](ROADMAP.vi.md) và [backlog](BACKLOG.vi.md).

Chỉ hoàn thành M0 khi project URP đã khóa phiên bản compile thành công, cảnh được quan sát trên iPhone thật, bản Web chạy qua HTTPS được kiểm tra, và phiên bản editor/package cùng kết quả thực tế được ghi lại. Chỉ xuất Xcode project chưa chứng minh game chạy được trên iPhone.

## Môi trường local — 05/10/2026

| Hạng mục | Trạng thái quan sát được |
| --- | --- |
| Kiến trúc Mac | Apple Silicon, arm64 |
| Repository | `/Volumes/T7-Workspace/Projects/Codex/city-race` |
| Đường dẫn cũ | `~/Workspace/Codex` là symlink tới `/Volumes/T7-Workspace/Projects/Codex` |
| Dung lượng trống | Internal khoảng 38 GiB; APFS dùng chung trên T7 khoảng 870 GiB; thay đổi theo thời điểm |
| Editor đã chọn | Unity 6000.3.25f1, ghi trong `.unity-version`; chưa cài |
| Unity Hub | Chưa thấy tại Applications hệ thống/user thông thường |
| Xcode | 26.6, build 17F113 |
| iPhone | iPhone 15 Pro Max đã từng pair; hiện unavailable trong `devicectl`; cần kết nối lại trước khi thử |
| Signing | Login Keychain có một Apple Development identity báo hết hạn (CSSMERR_TP_CERT_EXPIRED); 0 identity hợp lệ |
| Git LFS | Chưa cài; cần trước khi thêm asset nguồn nhị phân lớn, chưa cần cho các file text hiện tại |
| Project | Chưa có Assets, Packages hoặc ProjectSettings do Unity sinh |

Không commit định danh thiết bị, thông tin tài khoản, certificate hoặc provisioning profile. Người dùng đã cho phép cài phần mềm cần thiết cho M0. Đăng nhập, điều kiện license và signing Apple vẫn cần thông tin/lựa chọn của chính người dùng; không mua dịch vụ hoặc đoán signing team.

## Cài đặt và bố trí dữ liệu

1. Cài [Unity Hub](https://docs.unity.com/en-us/hub/install-hub) từ Unity. Cài editor Apple Silicon đúng `.unity-version`, kèm iOS Build Support. [Bản phát hành đã chọn](https://unity.com/releases/editor/whats-new/6000.3.25f1).
2. Đăng nhập và kích hoạt license Unity phù hợp; xác nhận mở được editor. Không âm thầm đổi patch.
3. Thêm Web Build Support cho CR-006 qua [module của Hub](https://docs.unity.com/en-us/hub/add-modules). Module Android thuộc M1.
4. Giữ bộ cài Unity/Xcode và cache toàn cục trên internal. Project, Library riêng của project và Builds nằm trên T7. Kiểm tra cả hai ổ khi cài; download và giải nén cần dung lượng tạm ngoài dung lượng cài xong. Không chuyển Docker, simulator, DerivedData hoặc cache toàn cục.
5. Giữ T7 kết nối khi Unity/IDE/build đang sử dụng. Đóng ứng dụng trước khi eject. Symlink không tạo bản offline hoặc backup tự động.

## Lệnh khởi tạo — đã chuẩn bị, chưa kiểm chứng bằng Unity

Chạy từ gốc repository. `preflight` chỉ đọc và trả mã khác 0 nếu thiếu editor hoặc ổ chứa project còn dưới 10 GiB. Đây không phải phép kiểm tra đầy đủ dung lượng cài đặt hay signing.

```sh
python3 tools/unity_project.py preflight
python3 tools/unity_project.py init
python3 tools/unity_project.py scene
python3 tools/unity_project.py export-ios
python3 tools/unity_project.py build-web
```

Chạy từng lệnh và kiểm tra từng kết quả. Đóng project đó trong Editor trước khi chạy batch. Executable mặc định là `/Applications/Unity/Hub/Editor/6000.3.25f1/Unity.app/Contents/MacOS/Unity`; dùng `--editor` nếu đã xác minh đường dẫn khác.

`init` cần URP blank template chính thức. Nếu không tìm thấy, lấy qua Hub rồi truyền `--template /absolute/path/to/com.unity.template.urp-blank-VERSION.tgz`. Không tự bịa template hoặc package lock. Script từ chối khi đã có thư mục project và giữ `.bootstrap-work/project` khi lỗi để kiểm tra. Không tùy tiện xóa phần khởi tạo dở hoặc chạy init đè lên nó.

Unity sinh settings/packages/metadata thật trong staging, sau đó script đưa vào repo và sao chép `tools/unity/CityRaceBootstrap.cs` tới `Assets/CityRace/Editor`. Sau khởi tạo, nếu sửa bootstrap thì giữ bản nguồn và bản editor đã sinh nhất quán. Kiểm tra compile, material/camera URP và `Assets/CityRace/Content/Scenes/Smoke.unity`. Cảnh chỉ có đường/nhà/xe dạng khối tĩnh, chưa có logic lái xe.

Commit asset nguồn kèm `.meta`, ProjectSettings, manifest và package lock sau khi xác minh. Logs, Library, Builds và staging được ignore. Log ở `Logs/`; đầu ra iOS ở `Builds/iOS`, Web ở `Builds/Web`. Kiểm tra đầu ra đã có trước khi build lại vì lệnh build có thể cập nhật chúng.

## Kiểm chứng thiết bị thật và trình duyệt

Mở Xcode project đã xuất, chọn signing team của người dùng và xác nhận bundle ID tạm `com.aletuan.cityrace.dev`. Kết nối/mở khóa iPhone, xác nhận trust và Developer Mode, rồi build/run. Ghi mẫu máy/OS, editor, commit, loại build, kết quả khởi chạy/render và lỗi; giữ kín credential. Compile thành công chưa phải kiểm tra hình ảnh.

Với Web, phục vụ build qua HTTPS cùng header nén đúng; kiểm tra tải, render và lỗi console trong trình duyệt/phiên bản được ghi rõ. Đích hosting/publishing phải được xác định rõ; tài liệu này không yêu cầu dịch vụ trả phí.

## Kiểm chứng hiện tại

Cú pháp Python, CLI help, chặn khi thiếu editor, link Markdown local và whitespace được kiểm tra trong bước 1. Compile Unity, tương thích template, cảnh sinh ra, xuất iOS, signing, chạy thiết bị và build Web chưa được kiểm chứng cho tới khi thực hiện từng task. Cập nhật mục này và backlog bằng bằng chứng thực tế, không ghi kết quả dự kiến như đã hoàn thành.

## Kiểm tra thêm signing — 05/10/2026

Project City Crew hiện dùng Expo/EAS, có cấu hình submit production và README ghi đã phát hành App Store. Các build profile không đổi nguồn credential mặc định. Vì vậy credential ký từ xa có thể do EAS quản lý; chưa kiểm tra tính hợp lệ của chúng. Việc đã phát hành App Store chưa chứng minh máy local hiện có development identity dùng được. Với City Race, xác minh Apple Developer team hiện có trong Xcode và cấu hình automatic development signing cho bundle ID riêng. Không thu hồi hoặc thay credential City Crew.
