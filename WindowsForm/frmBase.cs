using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsForm
{
    // Class này dùng để làm FORM CHA cho các form khác kế thừa
    public class frmBase : Form
    {
        // ==========================================
        // 1. KHAI BÁO API WINDOWS (ĐỂ KÉO CỬA SỔ)
        // ==========================================
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        // ==========================================
        // 2. CẤU HÌNH GIAO DIỆN
        // ==========================================
        private readonly Color clrHeaderBg = Color.FromArgb(240, 30, 30, 40);
        private readonly string headerFont = "Segoe UI";

        protected Panel pnlHeader;
        private PictureBox picLogo; // <--- [MỚI] Khai báo PictureBox cho logo
        private Label lblTitle;
        private Label btnClose;
        private Label btnMinimize;

        public frmBase()
        {
            SetupBaseForm();
            InitializeHeader();
        }

        private void SetupBaseForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            this.Size = new Size(1200, 800);

            try
            {
                // [LƯU Ý]: Thay 'map_background' bằng tên ảnh nền map trong Resources của bạn
                object mapObj = Properties.Resources.anh_nen;
                if (mapObj != null)
                    this.BackgroundImage = (Image)mapObj;
                else
                    this.BackColor = ColorTranslator.FromHtml("#2b2b2b");
            }
            catch { this.BackColor = Color.Gray; }

            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        // ==========================================
        // 3. KHỞI TẠO HEADER (ĐÃ CẬP NHẬT LOGO)
        // ==========================================
        private void InitializeHeader()
        {
            // --- A. PANEL HEADER ---
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 45;
            pnlHeader.BackColor = clrHeaderBg;
            pnlHeader.MouseDown += Header_MouseDown;
            this.Controls.Add(pnlHeader);

            // --- B. CÁC NÚT ĐÓNG/HẠ (Bên phải) ---
            btnClose = CreateHeaderButton("✕", Color.Red);
            btnClose.Click += (s, e) => Application.Exit();
            pnlHeader.Controls.Add(btnClose);

            btnMinimize = CreateHeaderButton("─", Color.FromArgb(50, 255, 255, 255));
            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            pnlHeader.Controls.Add(btnMinimize);

            // --- C. LOGO NGƯỜI QUE (Bên trái) [PHẦN MỚI THÊM] ---
            picLogo = new PictureBox();
            picLogo.Size = new Size(35, 35); // Kích thước logo
            picLogo.SizeMode = PictureBoxSizeMode.Zoom; // Co giãn ảnh để không bị méo
            picLogo.Location = new Point(10, 5); // Cách lề trái 10px, lề trên 5px
            picLogo.BackColor = Color.Transparent; // Nền trong suốt để lộ màu header

            // [QUAN TRỌNG]: LOAD ẢNH TỪ RESOURCES
            try
            {
                // >>> HÃY SỬA CHỮ "stickman_logo" THÀNH TÊN FILE ẢNH BẠN ĐÃ THÊM Ở BƯỚC 1 <<<
                object logoObj = Properties.Resources.Screenshot_2025_12_13_183419;
                if (logoObj != null)
                {
                    picLogo.Image = (Image)logoObj;
                }
            }
            catch { /* Nếu chưa thêm ảnh thì thôi, không báo lỗi */ }

            // Cho phép kéo cửa sổ khi nắm vào logo
            picLogo.MouseDown += Header_MouseDown;
            pnlHeader.Controls.Add(picLogo);

            // --- D. TIÊU ĐỀ GAME (Đã dời vị trí) ---
            lblTitle = new Label();
            lblTitle.Text = "Stickman Kingdom";
            lblTitle.Font = new Font("Segoe Print", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            // [THAY ĐỔI VỊ TRÍ]: Dời sang phải (X=55) để nhường chỗ cho Logo
            lblTitle.Location = new Point(55, 8);

            lblTitle.MouseDown += Header_MouseDown;
            pnlHeader.Controls.Add(lblTitle);
        }

        // Hàm phụ để tạo nút header cho gọn code
        private Label CreateHeaderButton(string text, Color hoverColor)
        {
            Label btn = new Label();
            btn.Text = text;
            btn.Font = new Font(headerFont, 12, FontStyle.Bold);
            btn.ForeColor = Color.White;
            btn.AutoSize = false;
            btn.Size = new Size(45, 45);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.Dock = DockStyle.Right;
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
            return btn;
        }

        // ==========================================
        // 4. XỬ LÝ KÉO CỬA SỔ
        // ==========================================
        private void Header_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}