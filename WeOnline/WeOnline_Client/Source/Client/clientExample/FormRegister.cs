// -----------------------------------------------------------------------
// <copyright file="Packet.cs" company="xsDevelopment">
//   Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0)
//   All Rights Reserved - See License.txt for more details
// </copyright>
// -----------------------------------------------------------------------
namespace clientExample
{
    using System;
    using System.Windows.Forms;

    public partial class FormRegister : Form
    {
        public FormRegister()
        {
            InitializeComponent();
        }

        public string Email { get { return textEmail.Text; } }
        public string Password { get { return textPass.Text; }}

        private void LinkNickHelpLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Your nickname is the name that will appear to other users.");
        }

        private void ButtonRegisterClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textEmail.Text))
            {
                MessageBox.Show("Please enter an email");
                return;
            }
            if (string.IsNullOrEmpty(textNick.Text))
            {
                MessageBox.Show("Please enter a nickname");
                return;
            }
            if (string.IsNullOrEmpty(textPass.Text))
            {
                MessageBox.Show("Please enter a password");
                return;
            }
            if (textPass.Text != textConfirmPass.Text)
            {
                MessageBox.Show("Please ensure the password match");
                return;
            }
            FormMain.Client.Connect(FormMain.IPADDRESS, FormMain.PORT);
            var creq = Packets.CreateRequest(textEmail.Text, textPass.Text, textNick.Text);
            FormMain.Client.WritePacket(creq);
            labelRegister.Visible = true;
        }
    }
}
