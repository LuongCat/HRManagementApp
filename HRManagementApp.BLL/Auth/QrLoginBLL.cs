using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HRManagementApp.Models;
using System.IO;

namespace HRManagementApp.BLL
{
    public class QrLoginBLL
    {
        private HttpListener _listener;
        private QrLoginSession _currentSession;
        private bool _isRunning = false;
        private string _localIpAddress;

        // Sự kiện trả về username người vừa quét
        public event Action<string , string> OnLoginSuccess;

        public QrLoginBLL()
        {
            _localIpAddress = GetLocalIPAddress();
        }

        public string GenerateSessionUrl()
        {
            _currentSession = new QrLoginSession
            {
                SessionId = Guid.NewGuid().ToString(),
                IsConfirmed = false,
                CreatedTime = DateTime.Now
            };
            if (!_isRunning) StartWebServer();
            return $"http://{_localIpAddress}:5000/scan?id={_currentSession.SessionId}";
        }

        private void StartWebServer()
        {
            Task.Run(() =>
            {
                try
                {
                    _listener = new HttpListener();
                    _listener.Prefixes.Add("http://+:5000/");
                    _listener.Start();
                    _isRunning = true;
                    System.Windows.MessageBox.Show("Server đã khởi động thành công tại Port 5000!");
                    while (_isRunning)
                    {
                        var context = _listener.GetContext();
                        ProcessRequest(context);
                    }
                    
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Lỗi Server (Mã {ex.Message}): {ex.Message}\n\nHãy chạy Visual Studio bằng 'Run as Administrator'.");
                    System.Diagnostics.Debug.WriteLine("Server Error: " + ex.Message);
                }
            });
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            string responseString = "";

            string id = request.QueryString["id"];

            // 1. Nếu điện thoại vừa quét mã (Truy cập /scan)
            if (request.Url.AbsolutePath == "/scan")
            {
                if (_currentSession != null && id == _currentSession.SessionId)
                {
                    // Trả về giao diện giả lập App Mobile
                    // Cho phép nhập User (hoặc chọn User có sẵn trên điện thoại)
                    responseString = GetMobileHtmlPage(id); 
                }
                else
                {
                    responseString = "<h1>Mã QR không hợp lệ hoặc đã hết hạn!</h1>";
                }
            }
            else if (request.Url.AbsolutePath == "/confirm")
            {
                string username = request.QueryString["user"]; // Lấy username từ điện thoại gửi lên
                string password = request.QueryString["password"];
                if (_currentSession != null && id == _currentSession.SessionId && !string.IsNullOrEmpty(username))
                {
                    _currentSession.IsConfirmed = true;
                    _currentSession.Username = username;

                    // Báo giao diện điện thoại thành công
                    responseString = "<h1 style='color:green; text-align:center; margin-top:50px;'>Đăng nhập thành công!</h1><p style='text-align:center'>Bạn có thể xem trên máy tính.</p>";

                    // Báo về WPF App xử lý tiếp
                    OnLoginSuccess?.Invoke(username , password);
                }
                else
                {
                    responseString = "<h1>Lỗi xác thực!</h1>";
                }
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        // HTML giả lập giao diện Mobile App
        private string GetMobileHtmlPage(string sessionId)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='vi'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Đăng nhập QR</title>
                    <style>
                        body {{ font-family: 'Segoe UI', sans-serif; padding: 20px; text-align: center; background-color: #f0f2f5; display: flex; justify-content: center; align-items: center; min-height: 100vh; margin: 0; }}
                        .card {{ background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); width: 100%; max-width: 350px; }}
                        h2 {{ color: #1F2937; margin-bottom: 5px; }}
                        p {{ color: #6B7280; margin-bottom: 25px; font-size: 14px; }}
                        label {{ display: block; text-align: left; margin-bottom: 5px; font-weight: 600; color: #374151; font-size: 14px; }}
                        input {{ width: 100%; padding: 12px; margin-bottom: 15px; border: 1px solid #D1D5DB; border-radius: 8px; box-sizing: border-box; font-size: 16px; outline: none; }}
                        input:focus {{ border-color: #1ABC9C; }}
                        button {{ width: 100%; padding: 14px; background: #1ABC9C; color: white; border: none; border-radius: 8px; font-weight: bold; font-size: 16px; cursor: pointer; transition: 0.2s; }}
                        button:active {{ transform: scale(0.98); }}
                        .loading {{ display: none; color: #1ABC9C; font-weight: bold; }}
                        .switch-account {{ margin-top: 15px; font-size: 13px; color: #6B7280; text-decoration: underline; cursor: pointer; display: none; }}
                    </style>
                </head>
                <body>
                    <div class='card'>
                        <div id='autoLoginParams' class='loading'>
                            <p>🔄 Đang tự động đăng nhập...</p>
                            <p id='welcomeUser' style='color:#333'></p>
                            <button style='background:#ccc; color:#333' onclick='cancelAutoLogin()'>Hủy & Đăng nhập khác</button>
                        </div>

                        <div id='loginForm'>
                            <h2>Xác thực đăng nhập</h2>
                            <p>Hệ thống Quản lý Nhân sự</p>
                            
                            <form onsubmit='return handleLogin()'>
                                <label>Tên đăng nhập:</label>
                                <input type='text' id='userInput' name='user' placeholder='Nhập username' required />
                                
                                <label>Mật khẩu:</label>
                                <input type='password' id='passInput' name='password' placeholder='Nhập mật khẩu' required />
                                
                                <button type='submit'>ĐĂNG NHẬP NGAY</button>
                            </form>
                        </div>
                    </div>

                    <script>
                        const sessionId = '{sessionId}';
                        let autoLoginTimer;

                        // Khi trang vừa load xong
                        window.onload = function() {{
                            // Kiểm tra bộ nhớ trình duyệt
                            const savedUser = localStorage.getItem('hrm_user');
                            const savedPass = localStorage.getItem('hrm_pass');

                            if (savedUser && savedPass) {{
                                // Nếu có dữ liệu cũ => Chuyển sang chế độ Auto Login
                                document.getElementById('loginForm').style.display = 'none';
                                document.getElementById('autoLoginParams').style.display = 'block';
                                document.getElementById('welcomeUser').innerText = 'Xin chào, ' + savedUser;

                                // Đợi 2 giây rồi tự gửi lệnh (để user kịp bấm hủy nếu muốn)
                                autoLoginTimer = setTimeout(() => {{
                                    window.location.href = `/confirm?id=${{sessionId}}&user=${{savedUser}}&password=${{savedPass}}`;
                                }}, 1500); 
                            }}
                        }};

                        // Hàm xử lý khi bấm nút Đăng nhập thủ công
                        function handleLogin() {{
                            const u = document.getElementById('userInput').value;
                            const p = document.getElementById('passInput').value;

                            // Lưu vào bộ nhớ trình duyệt
                            localStorage.setItem('hrm_user', u);
                            localStorage.setItem('hrm_pass', p);

                            // Chuyển hướng
                            window.location.href = `/confirm?id=${{sessionId}}&user=${{u}}&password=${{p}}`;
                            return false; // Chặn form submit mặc định để dùng JS redirect
                        }}

                        // Hàm hủy tự động đăng nhập
                        function cancelAutoLogin() {{
                            clearTimeout(autoLoginTimer); // Dừng đếm ngược
                            localStorage.removeItem('hrm_user'); // Xóa dữ liệu cũ
                            localStorage.removeItem('hrm_pass');
                            
                            // Hiện lại form
                            document.getElementById('autoLoginParams').style.display = 'none';
                            document.getElementById('loginForm').style.display = 'block';
                        }}
                    </script>
                </body>
                </html>";
        }

        public void StopServer()
        {
            _isRunning = false;
            _listener?.Stop();
        }

        private string GetLocalIPAddress()
        {
            return "192.168.1.6";
        }
    }
}