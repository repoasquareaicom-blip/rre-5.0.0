using Inventory.Masters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InvBal;
using InvDal;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;


namespace Inventory
{
    public partial class Login : Form
    {

        public static string connection = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        LoginBAL ObjLoginBAL = new LoginBAL();
        public Login()
        {
            InitializeComponent();
            SetLoginBranding();
            BindFloor();
            lblFloors.Visible = false;
            CmbFloorInchanger.Visible = false;
            Bindcompany();

            cmbcompany.SelectedIndex = 0;
            System.Reflection.FieldInfo versionField = typeof(Program).GetField("AppVersion");
            
            if (versionField != null)
            {
                object versionValue = versionField.GetValue(null);
                if (versionValue != null && versionValue.ToString().Trim() != "")
                {
                    lblVersionNumber.Text = versionValue.ToString();
                }
            }
        }






        private void SetLoginBranding()
        {
            try
            {
                string branchCode = ConfigurationManager.AppSettings["BranchCode"];
                if (string.IsNullOrEmpty(branchCode))
                    branchCode = "RR-SALEM";

                this.Text = "RR Electricals - Login";
                lblBrandTitle.Text = "RR\r\nElectricals";
                lblBrandSubTitle.Text = "Smart inventory access panel with secure branch login";
                lblWelcome.Text = "Secure Login";
                label1.Text = "RR Electricals inventory system";
                lblBranchInfo.Text = "Branch : " + branchCode;
                lblVersion.Text = "CloudEye Delight  •  Sync Enabled";
                lblFooter.Text = "Powered by CloudEye Delight";
                lblCloudIcon.Text = "";

                mainPanel.Paint -= MainPanel_Paint;
                mainPanel.Paint += MainPanel_Paint;
                brandPanel.Paint -= BrandPanel_Paint;
                brandPanel.Paint += BrandPanel_Paint;
                lblCloudIcon.Paint -= Badge_Paint;
                lblCloudIcon.Paint += Badge_Paint;
            }
            catch
            {
                // keep designer values if configuration is unavailable
            }
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(mainPanel.ClientRectangle,
                Color.FromArgb(245, 248, 255), Color.FromArgb(255, 246, 236), LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, mainPanel.ClientRectangle);
            }
        }

        private void BrandPanel_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(brandPanel.ClientRectangle,
                Color.FromArgb(42, 45, 145), Color.FromArgb(0, 150, 136), LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, brandPanel.ClientRectangle);
            }
        }

        private void Badge_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, lblCloudIcon.Width - 1, lblCloudIcon.Height - 1);
            using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                Color.FromArgb(255, 183, 77), Color.FromArgb(255, 112, 67), LinearGradientMode.ForwardDiagonal))
            using (Font badgeFont = new Font("Segoe UI Semibold", 30F, FontStyle.Bold))
            using (StringFormat sf = new StringFormat())
            {
                e.Graphics.FillEllipse(brush, rect);
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString("RR", badgeFont, Brushes.White, rect, sf);
            }
        }

        private void ButLogin_Click(object sender, EventArgs e)
        {
            if (Validationmenu())
            {
                userlogin();
            }



        }

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStrip1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void ChkFloorChanger_CheckedChanged(object sender, EventArgs e)
        {

            if (ChkFloorChanger.Checked)
            {
                lblFloors.Visible = true;
                CmbFloorInchanger.Visible = true;
            }

            else
            {
                lblFloors.Visible = false;
                CmbFloorInchanger.Visible = false;
            }
        }

        private static string EscapeJsonString(string s)
        {
            if (s == null)
                return string.Empty;
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public void BindFloor()
        {
            try
            {
                DataTable dt = LoginBAL.GetFloor();
                DataRow dr = dt.NewRow();
                dr["LocationID"] = "0";
                dr["LocationName"] = "--Select--";
                dt.Rows.InsertAt(dr, 0);
                CmbFloorInchanger.DataSource = dt;
                CmbFloorInchanger.ValueMember = "LocationID";
                CmbFloorInchanger.DisplayMember = "LocationName";
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message.ToString());
            }
        }


        public void Bindcompany()
        {
            try
            {
                DataTable dt = LoginBAL.Getcompanyname();
                DataRow dr = dt.NewRow();
                dr["CompanyName"] = "0";
                dr["CompanyName"] = "--Select--";
                dt.Rows.InsertAt(dr, 0);
                string com = dt.Rows[1][1].ToString();
                if (com == "R.R.LIGHTS")
                {
                    foreach (DataRow dr1 in dt.Rows)
                    {
                        if (dr1["CompanyName"].ToString() == "R.R. PIPES")
                            dr1.Delete();
                    }
                }
                else
                {

                }
                cmbcompany.DataSource = dt;

                cmbcompany.ValueMember = "CompanyName";
                cmbcompany.DisplayMember = "CompanyName";
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message.ToString());
            }
        }
        private bool Validationmenu()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(txtusername.Text))
            {
                i++;
                message = message + "Please Enter UserName" + "\n";
                if (i == 1)
                    this.ActiveControl = txtusername;
            }

            if (string.IsNullOrEmpty(txtpassword.Text))
            {
                i++;
                message = message + "Please Enter Password" + "\n";
                if (i == 1)
                    this.ActiveControl = txtpassword;
            }
            if (cmbcompany.SelectedIndex == 0)
            {
                i++;
                message = message + "Please Select Company" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbcompany;
            }

            if (ChkFloorChanger.Checked)
            {
                if (CmbFloorInchanger.SelectedIndex <= 0)
                {
                    i++;
                    message = message + "Please Select Floor" + "\n";
                    if (i == 1)
                        this.ActiveControl = CmbFloorInchanger;
                }

            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                status = false;
            }
            return status;

        }


        public void userlogin()
        {
            Program.Company = cmbcompany.Text;
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("[Getlogindetails]", con);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@UserName", txtusername.Text);
                cmd.Parameters.AddWithValue("@Password", txtpassword.Text);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                int count = dt.Rows.Count;
                if (count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["URole"]) == "Floor Incharge")
                    {
                        if (ChkFloorChanger.Checked)
                        {
                            string floor = Convert.ToString(dt.Rows[0]["FloorName"]);

                            string[] flr = floor.Split(',');
                            string FlrID = Convert.ToString(CmbFloorInchanger.SelectedValue);
                            if (flr.Contains(FlrID))
                            {
                                Program.Floor = FlrID;
                                Program.userid = Convert.ToString(dt.Rows[0]["UserId"]);
                                Program.UserName = Convert.ToString(dt.Rows[0]["UserName"]);
                                Program.Userrole = Convert.ToString(dt.Rows[0]["URole"]);
                                Program.Userfullname = Convert.ToString(dt.Rows[0]["UserFullName"]);
                                Program.ShopName = Convert.ToString(cmbcompany.SelectedIndex);
                                if (Program.objFrmMain == null)
                                {
                                    FrmMain cat = new FrmMain(Program.userid);
                                    Program.objFrmMain = cat;
                                }
                                else
                                {
                                    Program.objFrmMain.Dispose();
                                    FrmMain cat = new FrmMain(Program.userid);
                                    Program.objFrmMain = cat;
                                }
                                Program.objFrmMain.Show();
                                this.Hide();
                                // After successful login, fire presence sync in background
                                System.Threading.ThreadPool.QueueUserWorkItem(o => SendPresenceAsync(Program.userid, Program.UserName, 1));
                            }
                            else
                            {

                                MessageBox.Show("Select Valid Floor");
                            }

                        }
                        else
                        {
                            ChkFloorChanger.Checked = true;
                            MessageBox.Show("Please select floor");
                        }
                    }
                    else
                    {
                        // Program.UserName = txtusername.Text;
                        Program.userid = Convert.ToString(dt.Rows[0]["UserId"]);
                        Program.UserName = Convert.ToString(dt.Rows[0]["UserName"]);
                        Program.Userrole = Convert.ToString(dt.Rows[0]["URole"]);
                        Program.Userfullname = Convert.ToString(dt.Rows[0]["UserFullName"]);
                        Program.ShopName = Convert.ToString(cmbcompany.SelectedIndex);
                        if (Program.objFrmMain == null)
                        {
                            FrmMain cat = new FrmMain(Program.userid);
                            Program.objFrmMain = cat;
                        }
                        else
                        {
                            Program.objFrmMain.Dispose();
                            FrmMain cat = new FrmMain(Program.userid);
                            Program.objFrmMain = cat;
                        }
                        Program.objFrmMain.Show();
                        this.Hide();
                        // After successful login, fire presence sync in background
                        System.Threading.ThreadPool.QueueUserWorkItem(o => SendPresenceAsync(Program.userid, Program.UserName, 1));
                    }
                }
                else
                {
                    MessageBox.Show("Authentication failed");
                    return;
                }

            }


            //public void userlogin()
            //{
            //    DataTable dt = new DataTable();
            //    int Status = 0;
            //    if (string.IsNullOrEmpty(Convert.ToString(label4.Text)))
            //    {

            //        ObjLoginBAL.UserId = "";
            //    }
            //    else
            //    {
            //        ObjLoginBAL.UserId = label4.Text;
            //    }


            //    ObjLoginBAL.UserName = txtusername.Text;
            //    ObjLoginBAL.Password = txtpassword.Text;
            //    da.Fill(dt);
            //        int count = dt.Rows.Count;
            //        if (count > 0)
            //        {
            //            Program.username = userid.Text;
            //            Program.password = password.Text;
        }

        private void txtusername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (Validationmenu())
                {
                    userlogin();
                }
            }
        }

        private void txtpassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (Validationmenu())
                {
                    userlogin();
                }
            }
        }

        private void CmbFloorInchanger_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (Validationmenu())
                {
                    userlogin();
                }
            }
        }

        private void SendPresenceAsync(string externalId, string userName, int loginFlag)
        {
            try
            {
                // Check network availability
                if (!IsInternetAvailable())
                    return;

                string apiUrl = ConfigurationManager.AppSettings["PresenceApiUrl"];
                string branch = ConfigurationManager.AppSettings["BranchCode"];
                string apiKey = ConfigurationManager.AppSettings["ApiKey"];

                if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(branch) || string.IsNullOrEmpty(apiKey))
                    return;

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;


                string escUser = EscapeJsonString(userName);
                string escExternal = EscapeJsonString(externalId);
                string json = "{\"login_flag\":" + loginFlag.ToString() + ",\"external_id\":\"" + escExternal + "\",\"user_name\":\"" + escUser + "\"}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers["X-Branch-Code"] = branch;
                request.Headers["X-Api-Key"] = apiKey;

                using (var requestStream = request.GetRequestStream())
                using (var writer = new StreamWriter(requestStream))
                {
                    writer.Write(json);
                }

                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                {
                    // no-op: succeed silently
                }
            }
            catch
            {
                // swallow exceptions to avoid impacting login
            }
        }

        private bool IsInternetAvailable()
        {
            try
            {
                // Quick check: ping a reliable host
                using (var ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8", 1000);
                    return reply != null && reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (CmbFloorInchanger.Focused)
                {
                    ButLogin.Focus();
                }

            }

            if (keyData == Keys.Escape)
            {
                this.Close();
                Application.Exit();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);

        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
