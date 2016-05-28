// -----------------------------------------------------------------------
// <copyright file="CustomList.cs" company="xsDevelopment">
//   Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0)
//   All Rights Reserved - See License.txt for more details
// </copyright>
// -----------------------------------------------------------------------
namespace clientExample
{
    using System.Drawing;
    using System.Windows.Forms;

    public sealed class CustomList : UserControl
    {
        public CustomList()
        {
            HorizontalScroll.Visible = false;
            VerticalScroll.Visible = true;
            VScroll = true;
            //This is some style setting to prevent our control from flickering.
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.UserPaint, true);
        }

        private int _top = 0;

        public void Clear()
        {
            Controls.Clear();
            _top = 0;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            AutoScroll = false;
            if (Controls.Count == 1)
            {
                e.Control.Location = new Point(0, 0);
                if (VerticalScroll.Visible)
                    e.Control.Width = Width - SystemInformation.VerticalScrollBarWidth;
                else
                {
                    e.Control.Width = Width;
                }
            }
            else
            {
                _top += e.Control.Height;
                if (VerticalScroll.Visible)
                {
                    e.Control.Width = Width - SystemInformation.VerticalScrollBarWidth;
                    e.Control.Location = new Point(0, _top);
                }
                else
                {
                    e.Control.Location = new Point(0, _top);
                    e.Control.Width = Width;
                }
            }
            e.Control.Invalidate();
        }
    }

    public class CustomItem : Control
    {
        public string Nickname { get; set; }
        public string Status { get; set; }
        public Font NickFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        public Color NickColor = Color.Black;
        public Font StatusFont = new Font("Segoe UI", 7f, FontStyle.Regular);
        public Color StatusColor = Color.DarkGray;

        public CustomItem()
        {
            Height = 22;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
               ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
           e.Graphics.DrawString(Nickname, NickFont, new SolidBrush(NickColor), 2, 2);
            string statusLower = Status.ToLower();
            switch(statusLower)
            {
                case "admin":
                    StatusColor = Color.DarkRed;
                    break;
                case "mod":
                    StatusColor = Color.DarkGreen;
                    break;
                case "regular":
                    StatusColor = Color.DarkGray;
                    break;
                case "banned":
                    StatusColor = Color.DarkGray;
                    break;
            }
            e.Graphics.DrawString(Status, StatusFont, new SolidBrush(StatusColor), Width - e.Graphics.MeasureString(Status, StatusFont).Width-2 ,2);
        }
    }
}
