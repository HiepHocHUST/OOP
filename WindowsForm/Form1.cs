using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsForm
{
    // Kế thừa frmBase để có nền và header
    public class Form1 : frmBase
    {
        // ==========================================
        // 1. CẤU HÌNH GIAO DIỆN
        // ==========================================
        private readonly Color clrSurface = ColorTranslator.FromHtml("#fcf5e5");
        private readonly Color clrBorder = ColorTranslator.FromHtml("#5a4a42");
        private readonly Color clrText = ColorTranslator.FromHtml("#211a13");
        private readonly Color clrPrimary = ColorTranslator.FromHtml("#e67e22");
        private readonly Color clrInputBg = ColorTranslator.FromHtml("#ffffff");
        private readonly string handFont = "Segoe Print";

        // Các Control
        private SketchPanel panelMain;
        private Label lblFooter;
        private SketchTextBox txtUser;
        private SketchTextBox txtPass;
        private ComboBox cboSlot;

        public Form1()
        {
            // frmBase đã lo phần cửa sổ không viền, ta chỉ lo nội dung bên trong
            InitializeCustomControls();
            UpdateLayout();
        }

        // ==========================================
        // 2. KHỞI TẠO CONTROL
        // ==========================================
        private void InitializeCustomControls()
        {
            // --- PANEL ĐĂNG NHẬP ---
            panelMain = new SketchPanel(clrSurface, clrBorder);
            panelMain.Size = new Size(420, 500);
            this.Controls.Add(panelMain);
            panelMain.BringToFront();

            InitializeLoginContent();

            // --- FOOTER ---
            lblFooter = new Label();
            lblFooter.Text = "© 2025 Stickman Kingdom - Desktop Edition";
            lblFooter.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblFooter.ForeColor = Color.WhiteSmoke;
            lblFooter.AutoSize = true;
            lblFooter.BackColor = Color.Transparent;
            this.Controls.Add(lblFooter);
            lblFooter.BringToFront();
        }

        private void InitializeLoginContent()
        {
            // Header
            Label lblHeader = new Label() { Text = "Login Kingdom", Font = new Font(handFont, 22, FontStyle.Bold), ForeColor = clrText, AutoSize = true, Location = new Point(90, 25) };
            panelMain.Controls.Add(lblHeader);

            // Username
            Label lblUser = new Label() { Text = "Username", Font = new Font(handFont, 10, FontStyle.Bold), ForeColor = clrText, AutoSize = true, Location = new Point(35, 90) };
            panelMain.Controls.Add(lblUser);

            txtUser = new SketchTextBox(clrInputBg, clrBorder, clrText);
            txtUser.Location = new Point(35, 115);
            txtUser.Size = new Size(350, 45);
            panelMain.Controls.Add(txtUser);

            // Password
            Label lblPass = new Label() { Text = "Password", Font = new Font(handFont, 10, FontStyle.Bold), ForeColor = clrText, AutoSize = true, Location = new Point(35, 175) };
            panelMain.Controls.Add(lblPass);

            txtPass = new SketchTextBox(clrInputBg, clrBorder, clrText, true);
            txtPass.Location = new Point(35, 200);
            txtPass.Size = new Size(350, 45);
            panelMain.Controls.Add(txtPass);

            // Save Slot
            Label lblSlot = new Label() { Text = "Select Slot:", Font = new Font(handFont, 10, FontStyle.Bold), ForeColor = clrText, AutoSize = true, Location = new Point(35, 265) };
            panelMain.Controls.Add(lblSlot);

            cboSlot = new ComboBox();
            cboSlot.Font = new Font("Segoe UI", 11);
            cboSlot.BackColor = clrInputBg;
            cboSlot.FlatStyle = FlatStyle.Flat;
            cboSlot.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSlot.Items.AddRange(new object[] { "Profile 1", "Profile 2", "Profile 3" });
            cboSlot.SelectedIndex = 0;
            cboSlot.Location = new Point(180, 262);
            cboSlot.Size = new Size(205, 30);
            panelMain.Controls.Add(cboSlot);

            // Button Login
            SketchButton btnLogin = new SketchButton();
            btnLogin.Text = "ENTER WORLD";
            btnLogin.Font = new Font(handFont, 14, FontStyle.Bold);
            btnLogin.BackColor = clrPrimary;
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(35, 330);
            btnLogin.Size = new Size(350, 60);
            btnLogin.Click += BtnLogin_Click; // Gắn sự kiện Click
            panelMain.Controls.Add(btnLogin);

            // Links
            Label lblForgot = new Label() { Text = "Forgot?", Font = new Font(handFont, 9, FontStyle.Underline), ForeColor = Color.Gray, AutoSize = true, Cursor = Cursors.Hand, Location = new Point(35, 410) };
            panelMain.Controls.Add(lblForgot);
            Label lblCreate = new Label() { Text = "Register", Font = new Font(handFont, 9, FontStyle.Underline), ForeColor = clrPrimary, AutoSize = true, Cursor = Cursors.Hand, Location = new Point(290, 410) };
            panelMain.Controls.Add(lblCreate);
        }

        // ==========================================
        // 3. XỬ LÝ LOGIC (KHÔI PHỤC LOGIC CỦA BẠN)
        // ==========================================
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string u = txtUser.TextValue;
            string p = txtPass.TextValue;

            // [KHÔI PHỤC]: Sử dụng DatabaseHelper của bạn
            DatabaseHelper db = new DatabaseHelper();

            // Giả sử hàm CheckLogin trả về tên Role (VD: "Admin", "User") hoặc null nếu sai
            string role = db.CheckLogin(u, p);

            if (!string.IsNullOrEmpty(role))
            {
                MessageBox.Show($"Đăng nhập thành công! Xin chào {role}", "Thông báo");

                this.Hide();

                // Truyền Role và Slot sang Form Main như code cũ của bạn
                frmMain gameForm = new frmMain(role, cboSlot.SelectedItem.ToString());

                // Khi Form Main tắt thì tắt luôn app
                gameForm.FormClosed += (s, args) => this.Close();
                gameForm.Show();
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // 4. LAYOUT & CUSTOM CONTROLS (Giữ nguyên)
        // ==========================================
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (panelMain == null) return;
            int centerX = (this.ClientSize.Width - panelMain.Width) / 2;
            int centerY = (this.ClientSize.Height - panelMain.Height) / 2 + 20;
            panelMain.Location = new Point(centerX, centerY);

            if (lblFooter != null)
                lblFooter.Location = new Point((this.ClientSize.Width - lblFooter.Width) / 2, this.ClientSize.Height - 30);
        }

        // --- Custom Controls ---
        public class SketchPanel : Panel
        {
            private Color _bg, _bd;
            public SketchPanel(Color bg, Color bd) { _bg = bg; _bd = bd; DoubleBuffered = true; BackColor = Color.Transparent; }
            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle r = ClientRectangle; r.Inflate(-5, -5);
                using (SolidBrush s = new SolidBrush(Color.FromArgb(80, 0, 0, 0))) e.Graphics.FillRectangle(s, r.X + 8, r.Y + 8, r.Width, r.Height);
                using (SolidBrush b = new SolidBrush(_bg)) e.Graphics.FillRectangle(b, r);
                using (Pen p = new Pen(_bd, 4)) e.Graphics.DrawRectangle(p, r);
            }
        }

        public class SketchTextBox : Panel
        {
            private TextBox inner;
            public string TextValue { get { return inner.Text; } set { inner.Text = value; } }
            public SketchTextBox(Color bg, Color bd, Color txt, bool isPass = false)
            {
                BackColor = bg; Padding = new Padding(15, 10, 10, 10);
                inner = new TextBox() { BorderStyle = BorderStyle.None, BackColor = bg, ForeColor = txt, Font = new Font("Segoe Print", 12), Dock = DockStyle.Fill, UseSystemPasswordChar = isPass };
                Controls.Add(inner);
                Paint += (s, e) => { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using (Pen p = new Pen(bd, 2)) { Rectangle r = ClientRectangle; r.Inflate(-1, -1); e.Graphics.DrawRectangle(p, r); } };
                Click += (s, e) => inner.Focus();
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
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; Rectangle r = ClientRectangle; r.Inflate(-3, -3);
                if (!p) using (SolidBrush s = new SolidBrush(Color.FromArgb(100, 0, 0, 0))) e.Graphics.FillRectangle(s, r.X + 4, r.Y + 4, r.Width, r.Height);
                else r.Offset(2, 2);
                using (SolidBrush b = new SolidBrush(BackColor)) e.Graphics.FillRectangle(b, r);
                TextRenderer.DrawText(e.Graphics, Text, Font, r, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                using (Pen pen = new Pen(Color.FromArgb(100, 255, 255, 255), 2)) e.Graphics.DrawRectangle(pen, r);
            }
        }
    }
}