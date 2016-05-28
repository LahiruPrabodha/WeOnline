// -----------------------------------------------------------------------
// <copyright file="FormClient.cs" company="xsDevelopment">
//   Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0)
//   All Rights Reserved - See License.txt for more details
// </copyright>
// -----------------------------------------------------------------------
namespace clientExample
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public partial class FormMain : Form
    {
        public static Client Client = new Client();

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern uint ScrollText(IntPtr hwnd, uint wMsg, uint wParam, uint lParam);
        private const int WM_VSCROLL = 0x115;
        private const int SB_BOTTOM = 7;

        public static string IPADDRESS = "127.0.0.1";
        public const int PORT = 1987;
        public static string CurrentNickname = "";
        public static int CurrentUserId = 0;

        public FormMain()
        {
            InitializeComponent();
            AcceptButton = buttonConnect;
            Client.OnDataRecieved += OnDataRecieved;
            Client.OnDisconnect += (()=> ButtonLogoutClick(null, null));
            Closing += FormMain_Closing;
            messageBox.TextChanged += MessageBoxTextChanged;
            Shown += FormMainShown;
        }
        private void ButtonCreateClick(object sender, EventArgs e)
        {
            if (Sets.RegisterInstance != null)
                if (!Sets.RegisterInstance.IsDisposed)
                {
                    Sets.RegisterInstance.Close();
                }
            Sets.RegisterInstance = new FormRegister();
            Sets.RegisterInstance.ShowDialog();
        }
        private void ButtonConnectClick(object sender, EventArgs e)
        {
            groupConnect.Enabled = false;
            if (!Client.Connect(IPADDRESS, PORT))
            {
                groupConnect.Enabled = true;
                textBoxEmail.Select();
                MessageBox.Show("Failed to connect to server.");
                return;
            }
            Client.WritePacket(
                new Packet(
                    "LOGIN REQUEST",
                    DataMap.Serialize(
                        new List<string>
                            {
                                textBoxEmail.Text, textBoxPassword.Text
                            })));
        }
        private void ButtonChangeNickClick(object sender, EventArgs e)
        {
            string newNickname = CurrentNickname;
            var result = InputBox.Show("Change your nickname", "Edit your nickname, then press submit to save changes", ref newNickname,false);
            if (result == DialogResult.OK)
            {
                Client.WritePacket(Packets.UpdateNickname(newNickname));
                CurrentNickname = newNickname;
            }
        }
        private void ButtonSendClick(object sender, EventArgs e)
        {
            var msgPack = new Packet("MESSAGE REQUEST", textBoxMessage.Text);
            Client.WritePacket(msgPack);
            textBoxMessage.Clear();
            textBoxMessage.Select();
        }
        private void ButtonLogoutClick(object sender, EventArgs e)
        {
            try
            {
                Client.TcpClient.Close();
            }
            catch
            {
            }
            messageBox.Clear();
            textBoxMessage.Clear();
            groupConnect.Enabled = true;
            groupBox.Enabled = false;
            onlineList.Clear();
        }
        private void ButtonChangePassClick(object sender, EventArgs e)
        {
            string newPass = "";
            var result = InputBox.Show("Change your password", "Edit your password, then press submit to save changes", ref newPass, true);
            if (result == DialogResult.OK)
            {
                Client.WritePacket(Packets.UpdatePassword(newPass));
            }
        }
        private void MessageBoxTextChanged(object sender, EventArgs e)
        {
            ScrollText(messageBox.Handle, WM_VSCROLL, SB_BOTTOM, 0);
        }
        private void TextBoxIpTextChanged(object sender, EventArgs e)
        {
            IPADDRESS = textBoxIp.Text;
        }
        private void FormMainShown(object sender, EventArgs e)
        {
            textBoxEmail.Select();
        }
        private void FormMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void OnDataRecieved(string data)
        {
            var packet = new Packet(data);
            string command = packet.Command.ToUpper();
            if (command == "LOGIN SUCCESS")
            {
                Invoke(new Action(() =>
                                      {
                                          groupBox.Enabled = true;
                                          AcceptButton = buttonSend;
                                          var map = packet.Carriage.Deserialize();
                                          int uid = Convert.ToInt32(map[0]);
                                          string nick = map[1];
                                          CurrentNickname = nick;
                                          CurrentUserId = uid;
                                      }));
            }
            else if (command == "LOGIN FAILED")
            {
                Invoke(new Action(() =>
                                      {
                                          Disconnect();
                                          MessageBox.Show(packet.Carriage, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                          groupConnect.Enabled = true;
                                          groupBox.Enabled = false;
                                          AcceptButton = buttonConnect;
                                      }));
            }
            else if (command == "USER LIST")
            {
                Invoke(new Action(() => UserListUpdate(packet.Carriage)));
            }
            else if (command == "MESSAGE")
            {
                Invoke(new Action(() => MessageRecieved(packet.Carriage)));
            }
            else if (command == "CREATE RESPONSE")
            {
                if (Sets.RegisterInstance == null) return;
                if (Sets.RegisterInstance.IsDisposed) return;
                Sets.RegisterInstance.Invoke(new Action(() =>
                                                            {
                                                                textBoxPassword.Text = Sets.RegisterInstance.Password;
                                                                textBoxEmail.Text = Sets.RegisterInstance.Email;
                                                                Sets.RegisterInstance.labelRegister.Visible = false;
                                                                Sets.RegisterInstance.Close();
                                                                Disconnect();
                                                                MessageBox.Show("Account created, you may now login.", "Create Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                                groupConnect.Enabled = true;
                                                                groupBox.Enabled = false;
                                                                AcceptButton = buttonConnect;
                                                            }));
            }
            else if (command == "CREATE FAILED")
            {
                if (Sets.RegisterInstance == null) return;
                if (Sets.RegisterInstance.IsDisposed) return;
                Sets.RegisterInstance.Invoke(new Action(() =>
                                                            {
                                                                Disconnect();
                                                                Sets.RegisterInstance.labelRegister.Visible = false;
                                                                MessageBox.Show(packet.Carriage, "Create Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                                groupConnect.Enabled = true;
                                                                groupBox.Enabled = false;
                                                                AcceptButton = buttonConnect;
                                                            }));
            }
        }
        private void MessageRecieved(string messageData)
        {
            var data = messageData.Deserialize();
            string nickname = data[0];
            string msg = data[1];
            messageBox.AppendText(string.Format("<{0}> {1}\r\n", nickname, msg));
        }
        private void UserListUpdate(string rawdata)
        {
            var data = rawdata.Deserialize();
            onlineList.Clear();

            foreach (var user in data)
            {
                var attr = user.Deserialize();
                string nick = attr[0];
                string role = attr[1];
                bool isbanned = attr[2].ToLower() == "true";
                string userid = attr[3];

                var contextMenu = new ContextMenu();
                var ban = new MenuItem("Ban", (a, b) => Client.WritePacket(Packets.UpdateRole(userid, "banned")));
                var unban = new MenuItem("Unban", (a, b) => Client.WritePacket(Packets.UpdateRole(userid, "unbanned")));
                var admin = new MenuItem("Admin", (a, b) => Client.WritePacket(Packets.UpdateRole(userid, Sets.UserRole.Admin)));
                var reg = new MenuItem("Regularize", (a, b) => Client.WritePacket(Packets.UpdateRole(userid, Sets.UserRole.Regular)));
                var kill = new MenuItem("Kill", (a, b) => Client.WritePacket(Packets.Kill(userid)));
                contextMenu.MenuItems.AddRange(new[]{admin, reg,GetSep(), ban, unban,GetSep(),kill});

                    onlineList.Controls.Add(
                        new CustomItem()
                            {
                                Nickname = nick,
                                Status = (isbanned) ? "banned" : role,
                                ContextMenu = contextMenu
                            });
            }
        }
        private MenuItem GetSep()
        {
            return new MenuItem("-");
        }

        private void Disconnect()
        {
            try
            {
                Client.TcpClient.Close();
            }
            catch
            {
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }



    }
}
