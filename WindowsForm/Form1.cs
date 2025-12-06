using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

// SỬA: Đổi namespace từ StickmanLogin thành WindowsForm để khớp với Program
namespace WindowsForm
{
    public class Form1 : Form
    {
        // ==========================================
        // 1. CẤU HÌNH MÀU SẮC & FONT
        // ==========================================
        private readonly Color clrBackground = ColorTranslator.FromHtml("#f5e9d3");
        private readonly Color clrSurface = ColorTranslator.FromHtml("#eaddc7");
        private readonly Color clrText = ColorTranslator.FromHtml("#211a13");
        private readonly Color clrBorder = ColorTranslator.FromHtml("#856f5a");
        private readonly Color clrPrimary = ColorTranslator.FromHtml("#3b82f6");
        private readonly string handFont = "Segoe Print";

        private SketchPanel panelMain;
        private Label lblFooter;

        public Form1()
        {
            SetupForm();
            InitializeCustomControls();
            UpdateLayout();
        }

        private void SetupForm()
        {
            this.Text = "Stickman Adventures - Desktop Edition";
            this.Size = new Size(1200, 800); // Điều chỉnh size mặc định cho vừa màn hình laptop
            this.MinimumSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = clrBackground;
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }

        // ==========================================
        // 2. VẼ TOÀN BỘ GIAO DIỆN NỀN (GRID + STICKMAN + TEXT)
        // ==========================================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // --- A. Vẽ Grid (Giấy tập) ---
            using (Pen gridPen = new Pen(Color.FromArgb(30, clrBorder), 1))
            {
                int step = 40;
                for (int x = 0; x < this.Width; x += step) g.DrawLine(gridPen, x, 0, x, this.Height);
                for (int y = 0; y < this.Height; y += step) g.DrawLine(gridPen, 0, y, this.Width, y);
            }

            // --- B. Tính toán tọa độ ---
            float leftCenterX = this.Width * 0.3f; // Dời sang phải một chút cho cân
            float centerY = this.Height * 0.55f;
            float scale = Math.Min(this.Width, this.Height) / 600f; // Giảm tỉ lệ scale nhẹ để hình không quá to

            // --- C. VẼ CHỮ TRỰC TIẾP ---
            // 1. Vẽ chữ STICKMAN
            using (Font titleFont = new Font(handFont, 48 * scale, FontStyle.Bold))
            using (Brush brush = new SolidBrush(clrText))
            {
                string text = "STICKMAN";
                SizeF textSize = g.MeasureString(text, titleFont);
                g.DrawString(text, titleFont, brush, leftCenterX - (textSize.Width / 2), centerY - (250 * scale));
            }

            // 2. Vẽ chữ ADVENTURES
            using (Font titleFont = new Font(handFont, 45 * scale, FontStyle.Bold))
            using (Brush brush = new SolidBrush(clrPrimary))
            {
                string text = "ADVENTURES";
                SizeF textSize = g.MeasureString(text, titleFont);
                g.DrawString(text, titleFont, brush, leftCenterX - (textSize.Width / 3), centerY - (180 * scale));
            }

            // 3. Vẽ Slogan
            using (Font sloganFont = new Font(handFont, 14 * scale, FontStyle.Italic))
            using (Brush brush = new SolidBrush(clrBorder))
            {
                string text = "Join the journey on your desktop.";
                SizeF textSize = g.MeasureString(text, sloganFont);
                g.DrawString(text, sloganFont, brush, leftCenterX - (textSize.Width / 2), centerY - (100 * scale));
            }

            // --- D. Vẽ Stickman ---
            using (Pen stickPen = new Pen(clrText, 5 * scale))
            {
                stickPen.StartCap = LineCap.Round;
                stickPen.EndCap = LineCap.Round;

                // Đầu
                float headSize = 60 * scale;
                g.DrawEllipse(stickPen, leftCenterX - headSize / 2, centerY - 150 * scale, headSize, headSize);

                // Thân
                g.DrawLine(stickPen, leftCenterX, centerY - 90 * scale, leftCenterX, centerY + 60 * scale);

                // Tay
                g.DrawLine(stickPen, leftCenterX, centerY - 60 * scale, leftCenterX - 50 * scale, centerY + 20 * scale);
                g.DrawLine(stickPen, leftCenterX, centerY - 60 * scale, leftCenterX + 60 * scale, centerY - 100 * scale);

                // Chân
                g.DrawLine(stickPen, leftCenterX, centerY + 60 * scale, leftCenterX - 40 * scale, centerY + 160 * scale);
                g.DrawLine(stickPen, leftCenterX, centerY + 60 * scale, leftCenterX + 40 * scale, centerY + 160 * scale);
            }
        }

        // ==========================================
        // 3. KHỞI TẠO CONTROLS
        // ==========================================
        private void InitializeCustomControls()
        {
            // --- Panel Login ---
            panelMain = new SketchPanel(clrSurface, clrBorder);
            panelMain.Size = new Size(400, 450); // Resize nhẹ
            this.Controls.Add(panelMain);

            InitializeLoginPanelContent();

            // --- Footer ---
            lblFooter = new Label();
            lblFooter.Text = "© 2023 Stick Studios - Desktop Edition v2.0";
            lblFooter.Font = new Font("Arial", 10, FontStyle.Regular);
            lblFooter.ForeColor = clrBorder;
            lblFooter.AutoSize = true;
            lblFooter.BackColor = Color.Transparent;
            this.Controls.Add(lblFooter);
        }

        private void InitializeLoginPanelContent()
        {
            Label lblHeader = new Label();
            lblHeader.Text = "Welcome Back!";
            lblHeader.Font = new Font(handFont, 20, FontStyle.Bold);
            lblHeader.ForeColor = clrText;
            lblHeader.AutoSize = true;
            lblHeader.Location = new Point(30, 25);
            panelMain.Controls.Add(lblHeader);

            Label lblUser = new Label();
            lblUser.Text = "Username";
            lblUser.Font = new Font(handFont, 12, FontStyle.Bold);
            lblUser.ForeColor = clrText;
            lblUser.AutoSize = true;
            lblUser.Location = new Point(30, 80);
            panelMain.Controls.Add(lblUser);

            SketchTextBox txtUser = new SketchTextBox(clrBackground, clrBorder, clrText);
            txtUser.Location = new Point(30, 110);
            txtUser.Size = new Size(340, 45);
            panelMain.Controls.Add(txtUser);

            Label lblPass = new Label();
            lblPass.Text = "Password";
            lblPass.Font = new Font(handFont, 12, FontStyle.Bold);
            lblPass.ForeColor = clrText;
            lblPass.AutoSize = true;
            lblPass.Location = new Point(30, 170);
            panelMain.Controls.Add(lblPass);

            Label lblSlot = new Label();
            lblSlot.Text = "Select Save Slot:";
            lblSlot.Font = new Font(handFont, 10, FontStyle.Bold);
            lblSlot.ForeColor = clrText;
            lblSlot.AutoSize = true;
            lblSlot.Location = new Point(30, 260); // Vị trí dưới password
            panelMain.Controls.Add(lblSlot);

            ComboBox cboSlot = new ComboBox();
            cboSlot.Font = new Font("Segoe UI", 11);
            cboSlot.BackColor = clrBackground;
            cboSlot.FlatStyle = FlatStyle.Flat;
            cboSlot.DropDownStyle = ComboBoxStyle.DropDownList;
            // Thêm dữ liệu Slot
            cboSlot.Items.AddRange(new object[] { "Profile 1 (Lv.10)", "Profile 2 (New Game)", "Profile 3 (Empty)" });
            cboSlot.SelectedIndex = 0; // Chọn mặc định cái đầu
            cboSlot.Location = new Point(180, 258);
            cboSlot.Size = new Size(190, 30);
            panelMain.Controls.Add(cboSlot);

            SketchTextBox txtPass = new SketchTextBox(clrBackground, clrBorder, clrText, true);
            txtPass.Location = new Point(30, 200);
            txtPass.Size = new Size(340, 45);
            panelMain.Controls.Add(txtPass);

            SketchButton btnLogin = new SketchButton();
            btnLogin.Text = "Login System";
            btnLogin.Font = new Font(handFont, 16, FontStyle.Bold);
            btnLogin.BackColor = clrPrimary;
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(30, 310); // Dời xuống một chút
            btnLogin.Size = new Size(340, 55);

            btnLogin.Click += (s, e) =>
            {
                string u = txtUser.TextValue;
                string p = txtPass.TextValue;
                string role = "";

                // --- GIẢ LẬP CHECK DATABASE ---
                if (u == "admin" && p == "admin")
                {
                    role = "Admin"; // Quản trị viên: Full quyền
                }
                else if (u == "user" && p == "123")
                {
                    role = "User";  // Người chơi thường: Bị giới hạn
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!\n(Gợi ý: admin/admin hoặc user/123)", "Lỗi Đăng Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- ĐĂNG NHẬP THÀNH CÔNG ---
                this.Hide(); // Ẩn form Login

                // Truyền Role và Slot vừa chọn sang Form Main
                frmMain gameForm = new frmMain(role, cboSlot.SelectedItem.ToString());

                gameForm.FormClosed += (sender, args) => this.Close(); // Đóng app khi tắt game
                gameForm.Show();
            };
            panelMain.Controls.Add(btnLogin);


            Label lblForgot = new Label();
            lblForgot.Text = "Forgot password?";
            lblForgot.Font = new Font(handFont, 10, FontStyle.Underline);
            lblForgot.ForeColor = clrText;
            lblForgot.AutoSize = true;
            lblForgot.Cursor = Cursors.Hand;
            lblForgot.Location = new Point(30, 360);
            panelMain.Controls.Add(lblForgot);

            Label lblSignUp = new Label();
            lblSignUp.Text = "Create Account";
            lblSignUp.Font = new Font(handFont, 10, FontStyle.Underline);
            lblSignUp.ForeColor = clrPrimary;
            lblSignUp.AutoSize = true;
            lblSignUp.Cursor = Cursors.Hand;
            lblSignUp.Location = new Point(240, 360);
            panelMain.Controls.Add(lblSignUp);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (panelMain == null) return;

            int rightCenterX = (this.ClientSize.Width / 4) * 3;
            int centerY = this.ClientSize.Height / 2;

            panelMain.Location = new Point(rightCenterX - (panelMain.Width / 2), centerY - (panelMain.Height / 2));
            lblFooter.Location = new Point((this.ClientSize.Width - lblFooter.Width) / 2, this.ClientSize.Height - 40);
        }

        // --- Custom Controls ---
        public class SketchPanel : Panel
        {
            private Color _bgColor, _borderColor;
            public SketchPanel(Color bg, Color border) { _bgColor = bg; _borderColor = border; this.DoubleBuffered = true; this.BackColor = Color.Transparent; }
            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = this.ClientRectangle; rect.Inflate(-5, -5);
                using (SolidBrush s = new SolidBrush(Color.FromArgb(50, 0, 0, 0))) g.FillRectangle(s, rect.X + 6, rect.Y + 6, rect.Width, rect.Height);
                using (SolidBrush b = new SolidBrush(_bgColor)) g.FillRectangle(b, rect);
                using (Pen p = new Pen(_borderColor, 3)) g.DrawRectangle(p, rect);
            }
        }

        public class SketchTextBox : Panel
        {
            private TextBox inner;
            public string TextValue { get { return inner.Text; } set { inner.Text = value; } }
            public SketchTextBox(Color bg, Color border, Color text, bool isPass = false)
            {
                this.BackColor = bg; this.Padding = new Padding(10, 8, 10, 10);
                inner = new TextBox() { BorderStyle = BorderStyle.None, BackColor = bg, ForeColor = text, Font = new Font("Segoe Print", 12), Dock = DockStyle.Fill, UseSystemPasswordChar = isPass };
                this.Controls.Add(inner);
                this.Paint += (s, e) => {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (Pen p = new Pen(border, 2)) { Rectangle r = ClientRectangle; r.Inflate(-1, -1); e.Graphics.DrawRectangle(p, r); }
                };
                this.Click += (s, e) => inner.Focus();
            }
        }

        public class SketchButton : Button
        {
            private bool p = false;
            public SketchButton() { FlatStyle = FlatStyle.Flat; FlatAppearance.BorderSize = 0; Cursor = Cursors.Hand; }
            protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); p = true; Invalidate(); }
            protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); p = false; Invalidate(); }
            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle r = ClientRectangle; r.Inflate(-3, -3);
                if (!p) using (SolidBrush s = new SolidBrush(Color.FromArgb(60, 0, 0, 0))) g.FillRectangle(s, r.X + 3, r.Y + 3, r.Width, r.Height);
                else r.Offset(2, 2);
                using (SolidBrush b = new SolidBrush(BackColor)) g.FillRectangle(b, r);
                TextRenderer.DrawText(g, Text, Font, r, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                using (Pen pen = new Pen(Color.FromArgb(200, 255, 255, 255), 2)) g.DrawRectangle(pen, r);
            }
        }
    }
}