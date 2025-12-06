using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsForm
{
    public partial class frmMain : Form
    {
        // --- 1. CẤU HÌNH GIAO DIỆN ---
        private readonly Color clrBackground = ColorTranslator.FromHtml("#f5e9d3");
        private readonly Color clrSurface = ColorTranslator.FromHtml("#eaddc7");
        private readonly Color clrText = ColorTranslator.FromHtml("#211a13");
        private readonly Color clrAccent = ColorTranslator.FromHtml("#e74c3c"); // Màu đỏ cam cho nút Start
        private readonly string handFont = "Segoe Print";

        private MenuStrip menuStrip1;
        private StatusStrip statusStrip1;
        private PictureBox picBackground;
        private Panel pnlLauncher; // <--- Panel mới để chọn màn và bắt đầu game

        private string _userRole; // Vai trò người dùng (Admin, Moderator, User)
        private string _saveSlot; // Tên người dùng đăng nhập


        public frmMain(string role, string slot)
        {
            _userRole = role;
            _saveSlot = slot;

            this.Text = $"Hệ thống Game - [{_userRole}] - Đang chơi: {_saveSlot}";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ColorTranslator.FromHtml("#f5e9d3");
            this.IsMdiContainer = true;

            InitializeCustomComponents();
            ApplyPermissions(); // <--- Gọi hàm phân quyền
        }
        private void ApplyPermissions()
        {
            // Cập nhật thanh trạng thái phía dưới
            statusStrip1.Items.Clear();
            statusStrip1.Items.Add(new ToolStripStatusLabel($"User: {_userRole} | Slot: {_saveSlot} | Trạng thái: Online"));

            // LOGIC PHÂN QUYỀN:
            // Nếu là "User" thường -> Ẩn menu "Hệ thống" và "Quản lý"
            if (_userRole == "User")
            {
                foreach (ToolStripItem item in menuStrip1.Items)
                {
                    if (item.Text == "Hệ thống" || item.Text == "Quản lý")
                    {
                        item.Visible = false; // Ẩn đi
                    }
                }

                // Nếu User thường, đổi màu nút Start sang màu xanh cho thân thiện hơn
                // (Optional - tùy chỉnh giao diện)
            }
        }

        private void InitializeCustomComponents()
        {
            // --- A. MENUSTRIP (Giữ nguyên menu đẹp như cũ) ---
            menuStrip1 = new MenuStrip();
            menuStrip1.Dock = DockStyle.Top;
            menuStrip1.Font = new Font(handFont, 11F, FontStyle.Bold);
            menuStrip1.BackColor = clrSurface;
            menuStrip1.ForeColor = clrText;
            menuStrip1.Renderer = new SketchMenuRenderer(clrSurface, ColorTranslator.FromHtml("#856f5a"));

            ToolStripMenuItem mnuSystem = new ToolStripMenuItem("Hệ thống");
            mnuSystem.DropDownItems.Add("Cấu hình", null, (s, e) => MessageBox.Show("Mở Cấu hình"));
            mnuSystem.DropDownItems.Add("Thoát", null, (s, e) => Application.Exit());

            ToolStripMenuItem mnuManage = new ToolStripMenuItem("Quản lý");
            mnuManage.DropDownItems.Add("Danh sách Tướng");
            mnuManage.DropDownItems.Add("Kho Quái vật");

            ToolStripMenuItem mnuShop = new ToolStripMenuItem("Cửa hàng");
            ToolStripMenuItem mnuReport = new ToolStripMenuItem("Báo cáo");
            menuStrip1.Items.AddRange(new ToolStripItem[] { mnuSystem, mnuManage, mnuShop });
            this.Controls.Add(menuStrip1);

            // --- B. STATUSSTRIP (Giữ nguyên) ---
            statusStrip1 = new StatusStrip();
            statusStrip1.Dock = DockStyle.Bottom;
            statusStrip1.BackColor = clrSurface;
            statusStrip1.Font = new Font(handFont, 9F);
            statusStrip1.Items.Add(new ToolStripStatusLabel("Admin: SuperUser | Trạng thái: Sẵn sàng chiến đấu"));
            this.Controls.Add(statusStrip1);

            // --- C. ẢNH NỀN ---
            picBackground = new PictureBox();
            picBackground.Dock = DockStyle.Fill;
            picBackground.SizeMode = PictureBoxSizeMode.Zoom;
            picBackground.BackColor = clrBackground;
            try
            {
                // Thay đường dẫn ảnh của bạn vào đây
                picBackground.Image = Image.FromFile(@"D:\New folder (2)\trang_chủ_admin_game_stickman.png");
            }
            catch { }
            this.Controls.Add(picBackground);
            picBackground.BringToFront();


            // --- D. BATTLE LAUNCHER (KHU VỰC BẮT ĐẦU TRẬN ĐẤU) ---
            // Đây là cái mới bạn cần
            InitializeLauncher();
        }

        private void InitializeLauncher()
        {
            // 1. Tạo Panel chứa (Nổi lên trên ảnh nền)
            pnlLauncher = new Panel();
            pnlLauncher.Size = new Size(400, 350);
            // Căn giữa màn hình bằng công thức toán
            pnlLauncher.Location = new Point((this.ClientSize.Width - 400) / 2, (this.ClientSize.Height - 350) / 2);
            pnlLauncher.BackColor = Color.FromArgb(240, 255, 255, 255); // Màu trắng trong suốt nhẹ (Alpha=240)
            pnlLauncher.BorderStyle = BorderStyle.FixedSingle;

            // Sự kiện resize để panel luôn ở giữa khi phóng to thu nhỏ
            this.Resize += (s, e) => {
                pnlLauncher.Location = new Point((this.ClientSize.Width - pnlLauncher.Width) / 2, (this.ClientSize.Height - pnlLauncher.Height) / 2);
            };

            // Add Panel vào trong PictureBox để hỗ trợ trong suốt tốt hơn (hoặc add vào Form và BringToFront)
            picBackground.Controls.Add(pnlLauncher);


            // 2. Tiêu đề "TRẬN ĐẤU MỚI"
            Label lblTitle = new Label();
            lblTitle.Text = "BATTLE SETUP";
            lblTitle.Font = new Font(handFont, 20, FontStyle.Bold);
            lblTitle.ForeColor = clrAccent;
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(400, 50);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Location = new Point(0, 20);
            pnlLauncher.Controls.Add(lblTitle);


            // 3. Chọn Level (ComboBox)
            Label lblMap = new Label() { Text = "Chọn Bản đồ:", Location = new Point(50, 90), Font = new Font(handFont, 10), AutoSize = true };
            pnlLauncher.Controls.Add(lblMap);

            ComboBox cboMap = new ComboBox();
            cboMap.Font = new Font("Segoe UI", 12);
            cboMap.Location = new Point(50, 120);
            cboMap.Size = new Size(300, 30);
            cboMap.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMap.Items.AddRange(new object[] { "Map 1: Rừng Nguyên Sinh", "Map 2: Sa Mạc Chết", "Map 3: Đỉnh Núi Tuyết" });
            cboMap.SelectedIndex = 0;
            pnlLauncher.Controls.Add(cboMap);


            // 4. Chọn Độ khó (Radio Button)
            Label lblDiff = new Label() { Text = "Độ khó:", Location = new Point(50, 170), Font = new Font(handFont, 10), AutoSize = true };
            pnlLauncher.Controls.Add(lblDiff);

            RadioButton rdoEasy = new RadioButton() { Text = "Dễ", Location = new Point(50, 200), Font = new Font(handFont, 10), Checked = true, AutoSize = true };
            RadioButton rdoHard = new RadioButton() { Text = "Khó (x2 Vàng)", Location = new Point(150, 200), Font = new Font(handFont, 10, FontStyle.Bold), AutoSize = true, ForeColor = Color.Red };
            pnlLauncher.Controls.Add(rdoEasy);
            pnlLauncher.Controls.Add(rdoHard);


            // 5. NÚT START GAME (Button to đùng)
            Button btnStart = new Button();
            btnStart.Text = "VÀO TRẬN NGAY!";
            btnStart.Font = new Font(handFont, 16, FontStyle.Bold);
            btnStart.BackColor = clrAccent;
            btnStart.ForeColor = Color.White;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Size = new Size(300, 60);
            btnStart.Location = new Point(50, 260);
            btnStart.Cursor = Cursors.Hand;

            // Sự kiện bấm nút
            btnStart.Click += (s, e) => {
                string map = cboMap.SelectedItem.ToString();
                string mode = rdoHard.Checked ? "HARD" : "EASY";

                // Sau này code gọi Unity sẽ nằm ở đây
                MessageBox.Show($"Đang khởi động Unity Game...\nMap: {map}\nMode: {mode}", "SYSTEM LAUNCHER");
            };

            pnlLauncher.Controls.Add(btnStart);
        }


        // --- CLASS RENDERER CHO MENU (Giữ nguyên để menu đẹp) ---
        private class SketchMenuRenderer : ToolStripProfessionalRenderer
        {
            public SketchMenuRenderer(Color bg, Color border) : base(new SketchColors(bg, border)) { }
        }

        private class SketchColors : ProfessionalColorTable
        {
            private Color _bg, _border;
            public SketchColors(Color bg, Color border) { _bg = bg; _border = border; }
            public override Color MenuStripGradientBegin => _bg;
            public override Color MenuStripGradientEnd => _bg;
            public override Color ToolStripDropDownBackground => _bg;
            public override Color MenuItemSelected => Color.FromArgb(20, 0, 0, 0);
            public override Color MenuItemBorder => Color.Transparent;
            public override Color MenuItemPressedGradientBegin => _bg;
            public override Color MenuItemPressedGradientEnd => _bg;
            public override Color ImageMarginGradientBegin => _bg;
            public override Color ImageMarginGradientMiddle => _bg;
            public override Color ImageMarginGradientEnd => _bg;
        }
    }
}