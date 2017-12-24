using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Media;
using System.Collections;

namespace cs_recoil_demo
{
    public partial class Form1 : Form
    {

        Thread thread = null;//= new Thread(new ThreadStart(recoil));
        static Boolean running = false;
        static Graphics g = null;
        static ArrayList pos_x = null, pos_y = null;






        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            running = true;
            thread = new Thread(new ParameterizedThreadStart(recoil));
            thread.IsBackground = true;
            thread.Start(this);
        }

        private static void recoil(Object e) {
            Random ran = new Random();
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = @"E:\git\cs_recoil_demo\gun.wav";
            player.Load();
            Pen p_big = new Pen(Color.Red, 2);
            Pen p_sml = new Pen(Color.Black, 1);
            Form1 me = (Form1)e;
            Point formPoint = me.PointToClient(Control.MousePosition);
            g.DrawEllipse(p_big, formPoint.X - 30, formPoint.Y - 30, 60, 60);
            pos_x = new ArrayList();
            pos_y = new ArrayList();

            
            while (running)
            {   
                
                //画图
                formPoint = me.PointToClient(Control.MousePosition);
                g.DrawEllipse(p_sml, formPoint.X - 3, formPoint.Y - 3, 6, 6);
                //保存当前鼠标位置
                Console.WriteLine(formPoint.X+","+formPoint.Y);
                pos_x.Add(formPoint.X);
                pos_y.Add(formPoint.Y);
                //响，发射
                player.Play();
                //后座
                int h_recoil = ran.Next(-5, 5);
                int v_recoil = ran.Next(8, 12);
                mouse_event(MOUSEEVENTF_MOVE, h_recoil, -v_recoil, 0, 0);
                //间隔
                Thread.Sleep(120);
            }
            
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            running = false;
            //计算命中率
            if (pos_x.Count > 1) {
                int x = (int)pos_x[0];
                int y = (int)pos_y[0];

                int all = pos_x.Count-1;
                int in_circle=0;

                for (int i = 1; i < pos_x.Count; i++) { 
                    int a_x=(int)pos_x[i],a_y=(int)pos_y[i];
                    if ((a_x - x) * (a_x - x) + (a_y - y) * (a_y - y) <= 30 * 30)
                        in_circle++;
                }
                if (label1.Text.Length > 30 * 5)
                    label1.Text = "";
                label1.Text += "\n\n总枪数:" + all + "\n命中枪数:" + in_circle + 
                    "\n命中率:" + string.Format("{0:0.00%}", (((double)in_circle) / ((double)all)));
            }
        }


        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        public static extern void mouse_event(
            int dwFlags,
            int dx,
            int dy,
            int cButtons,
            int dwExtraInfo
        );
        const int MOUSEEVENTF_MOVE = 0x0001;      //移动鼠标 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTUP = 0x0010; //模拟鼠标右键抬起 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;// 模拟鼠标中键抬起 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;//标示是否采用绝对坐标



    }
}
