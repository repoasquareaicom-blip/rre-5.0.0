using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory
{
    public enum DotMatrixPrintOption
    {
        Cancel,
        Print,
        Preview
    }

    public class DotMatrixPrintOptionForm : Form
    {
        private RadioButton radioPrint;
        private RadioButton radioPreview;

        public static DotMatrixPrintOption Show(string message)
        {
            using (DotMatrixPrintOptionForm form = new DotMatrixPrintOptionForm(message))
            {
                Form owner = Form.ActiveForm;
                DialogResult result;
                if (owner != null && owner.Visible && !owner.IsDisposed)
                {
                    result = form.ShowDialog(owner);
                }
                else
                {
                    result = form.ShowDialog();
                }

                if (result != DialogResult.OK)
                {
                    return DotMatrixPrintOption.Cancel;
                }

                if (form.radioPreview.Checked)
                {
                    return DotMatrixPrintOption.Preview;
                }

                return DotMatrixPrintOption.Print;
            }
        }

        public DotMatrixPrintOptionForm(string message)
        {
            this.Text = "Confirmation";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.ClientSize = new Size(420, 188);
            this.Font = new Font("Microsoft Sans Serif", 9F);

            Label lblMessage = new Label();
            lblMessage.Text = string.IsNullOrEmpty(message) ? "Do you want to Print?" : message;
            lblMessage.Location = new Point(16, 14);
            lblMessage.Size = new Size(388, 40);

            Label lblOption = new Label();
            lblOption.Text = "Print Option:";
            lblOption.Location = new Point(16, 60);
            lblOption.AutoSize = true;

            radioPrint = new RadioButton();
            radioPrint.Text = "Print";
            radioPrint.Location = new Point(32, 84);
            radioPrint.AutoSize = true;
            radioPrint.Checked = true;

            radioPreview = new RadioButton();
            radioPreview.Text = "Preview";
            radioPreview.Location = new Point(32, 108);
            radioPreview.AutoSize = true;

            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(228, 148);
            btnOk.Size = new Size(84, 28);
            btnOk.DialogResult = DialogResult.OK;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(320, 148);
            btnCancel.Size = new Size(84, 28);
            btnCancel.DialogResult = DialogResult.Cancel;

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
            this.Controls.Add(lblMessage);
            this.Controls.Add(lblOption);
            this.Controls.Add(radioPrint);
            this.Controls.Add(radioPreview);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }
    }
}
