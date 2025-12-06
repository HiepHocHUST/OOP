using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsForm
{
    public class Form1 : Form
    {
        // ==========================================
        // 1. CẤU HÌNH CƠ BẢN
        // ==========================================
        private readonly Color clrBackground = ColorTranslator.FromHtml("#f5e9d3");
        private readonly Color clrSurface = ColorTranslator.FromHtml("#eaddc7");
        private readonly Color clrText = ColorTranslator.FromHtml("#211a13");
        private readonly Color clrBorder = ColorTranslator.FromHtml("#856f5a");
        private readonly Color clrPrimary = ColorTranslator.FromHtml("#3b82f6");
        private readonly string handFont = "Segoe Print";

        private SketchPanel panelMain;
        private Label lblFooter;
        private PictureBox picBackground; // <--- Đối tượng mới để chứa ảnh nền

        public Form1()
        {
            SetupForm();
            InitializeCustomControls();
            UpdateLayout();
        }

        private void SetupForm()
        {
            this.Text = "Stickman Adventures - Desktop Edition";
            this.Size = new Size(1200, 800);
            this.MinimumSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
        }

        // ==========================================
        // 2. KHỞI TẠO GIAO DIỆN (Đã sửa đổi)
        // ==========================================
        private void InitializeCustomControls()
        {
            // --- A. TẠO BACKGROUND (PICTUREBOX) ---
            picBackground = new PictureBox();
            picBackground.Dock = DockStyle.Fill; // Tràn đầy màn hình
            picBackground.SizeMode = PictureBoxSizeMode.StretchImage; // Co giãn ảnh cho vừa khung
            picBackground.BackColor = clrBackground; // Màu nền dự phòng nếu chưa có ảnh

            try
            {
                // [QUAN TRỌNG]: BẠN HÃY THAY ĐƯỜNG DẪN ẢNH CỦA BẠN VÀO DƯỚI ĐÂY
                // Ví dụ: @"C:\Users\Admin\Pictures\game_bg.jpg"
                picBackground.Image = Properties.Resources.trang_chủ_admin_game_stickman;
            }
            catch
            {
                // Nếu không tìm thấy ảnh thì thôi, giữ màu nền mặc định
            }

            this.Controls.Add(picBackground);


            // --- B. PANEL ĐĂNG NHẬP (Giữ nguyên thiết kế cũ) ---
            // Lưu ý: Add vào picBackground để nó nằm đè lên ảnh
            panelMain = new SketchPanel(clrSurface, clrBorder);
            panelMain.Size = new Size(400, 450);
            picBackground.Controls.Add(panelMain); // <--- Add vào PictureBox

            InitializeLoginPanelContent();

            // --- C. FOOTER ---
            lblFooter = new Label();
            lblFooter.Text = "© 2023 Stick Studios - Desktop Edition v2.0";
            lblFooter.Font = new Font("Arial", 10, FontStyle.Regular);
            lblFooter.ForeColor = Color.White; // Đổi màu chữ footer sang trắng cho dễ nhìn trên nền ảnh
            lblFooter.AutoSize = true;
            lblFooter.BackColor = Color.Transparent;

            // Add Footer vào PictureBox luôn để nền trong suốt đè lên ảnh
            picBackground.Controls.Add(lblFooter);
        }

        // --- Hàm này giữ nguyên nội dung bên trong, chỉ thay đổi vị trí Add controls ---
        private void InitializeLoginPanelContent()
        {
            Label lblHeader = new Label() { Text = "Welcome Back!", Font = new Font(handFont, 20, FontStyle.Bold), ForeColor = clrText, AutoSize = true, Location = new Point(30, 25) };
            panelMain.Controls.Add(lblHeader);

            Label lblUser = new Label() { Text = "Username", Font = new Font(handFont, 12, FontStyle.Bold), ForeColor = clrText, AutoSize = true, Location = new Point(30, 80) };
            panelMain.Controls.Add(lblUser);

            SketchTextBox txtUser = new SketchTextBox(clrBackground, clrBorder, clrText);
            txtUser.Location = new Point(30, 110);
            txtUser.Size = new Size(340, 45);
            panelMain.Controls.Add(txtUser);

            Label lblPass = new Label() { Text = "Password", Font = new Font(handFont, 12, FontStyle.Bold), ForeColor = clrText, AutoSize = true, Location = new Point(30, 170) };
            panelMain.Controls.Add(lblPass);

            SketchTextBox txtPass = new SketchTextBox(clrBackground, clrBorder, clrText, true);
            txtPass.Location = new Point(30, 200);
            txtPass.Size = new Size(340, 45);
            panelMain.Controls.Add(txtPass);

            // --- Save Slot ---
            Label lblSlot = new Label();
            lblSlot.Text = "Select Save Slot:";
            lblSlot.Font = new Font(handFont, 10, FontStyle.Bold);
            lblSlot.ForeColor = clrText;
            lblSlot.AutoSize = true;
            lblSlot.Location = new Point(30, 260);
            panelMain.Controls.Add(lblSlot);

            ComboBox cboSlot = new ComboBox();
            cboSlot.Font = new Font("Segoe UI", 11);
            cboSlot.BackColor = clrBackground;
            cboSlot.FlatStyle = FlatStyle.Flat;
            cboSlot.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSlot.Items.AddRange(new object[] { "Profile 1 (Lv.10)", "Profile 2 (New Game)", "Profile 3 (Empty)" });
            cboSlot.SelectedIndex = 0;
            cboSlot.Location = new Point(180, 258);
            cboSlot.Size = new Size(190, 30);
            panelMain.Controls.Add(cboSlot);

            // --- Button Login ---
            SketchButton btnLogin = new SketchButton();
            btnLogin.Text = "Login System";
            btnLogin.Font = new Font(handFont, 16, FontStyle.Bold);
            btnLogin.BackColor = clrPrimary;
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(30, 310);
            btnLogin.Size = new Size(340, 55);

            btnLogin.Click += (s, e) =>
            {
                string u = txtUser.TextValue;
                string p = txtPass.TextValue;
                string role = "";

                if (u == "admin" && p == "admin") role = "Admin";
                else if (u == "user" && p == "123") role = "User";
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!\n(Gợi ý: admin/admin)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Hide();
                frmMain gameForm = new frmMain(role, cboSlot.SelectedItem.ToString());
                gameForm.FormClosed += (sender, args) => this.Close();
                gameForm.Show();
            };
            panelMain.Controls.Add(btnLogin);

            // --- Links ---
            Label lblForgot = new Label() { Text = "Forgot password?", Font = new Font(handFont, 10, FontStyle.Underline), ForeColor = clrText, AutoSize = true, Cursor = Cursors.Hand, Location = new Point(30, 380) };
            panelMain.Controls.Add(lblForgot);

            Label lblSignUp = new Label() { Text = "Create Account", Font = new Font(handFont, 10, FontStyle.Underline), ForeColor = clrPrimary, AutoSize = true, Cursor = Cursors.Hand, Location = new Point(240, 380) };
            panelMain.Controls.Add(lblSignUp);
        }

        // ==========================================
        // 3. XỬ LÝ LAYOUT (CĂN GIỮA)
        // ==========================================
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (panelMain == null || picBackground == null) return;

            // Căn Panel Login lệch sang phải một chút cho đẹp (giống thiết kế landing page)
            // Hoặc nếu muốn ra chính giữa màn hình thì dùng công thức:
            // X = (this.ClientSize.Width - panelMain.Width) / 2

            int rightCenterX = (this.ClientSize.Width / 4) * 3; // Lệch phải 3/4 màn hình
            int centerY = (this.ClientSize.Height - panelMain.Height) / 2;

            // Nếu màn hình quá nhỏ thì đưa về chính giữa
            if (this.ClientSize.Width < 800)
            {
                rightCenterX = (this.ClientSize.Width - panelMain.Width) / 2;
            }

            panelMain.Location = new Point(rightCenterX - (panelMain.Width / 2), centerY);

            // Footer nằm dưới cùng chính giữa
            if (lblFooter != null)
            {
                lblFooter.Location = new Point((this.ClientSize.Width - lblFooter.Width) / 2, this.ClientSize.Height - 40);
            }
        }

        // ==========================================
        // 4. CÁC CLASS CUSTOM CONTROLS (Giữ nguyên để giao diện Login đẹp)
        // ==========================================
        public class SketchPanel : Panel
        {
            private Color _bgColor, _borderColor;
            public SketchPanel(Color bg, Color border) { _bgColor = bg; _borderColor = border; this.DoubleBuffered = true; this.BackColor = Color.Transparent; }
            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = this.ClientRectangle; rect.Inflate(-5, -5);
                // Vẽ bóng đổ nhẹ
                using (SolidBrush s = new SolidBrush(Color.FromArgb(50, 0, 0, 0))) g.FillRectangle(s, rect.X + 6, rect.Y + 6, rect.Width, rect.Height);
                // Vẽ nền
                using (SolidBrush b = new SolidBrush(_bgColor)) g.FillRectangle(b, rect);
                // Vẽ viền
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