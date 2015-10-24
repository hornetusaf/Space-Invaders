using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.Net.Sockets;
using System.Net;

namespace SpaceInvaderCliente
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.PictureBox Bala;
        int balaviva, posy = 0, salto = 0, vidas = 0;
        Thread t1, t2, t3, t4;
        TcpClient cliente;
        byte[] buffer = new byte[4096];

        public Form1()
        {
            InitializeComponent();
            balaviva = 0;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PresionaTecla);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Disparo);
            t1 = new Thread(new ThreadStart(Socket));
            t1.Start();
        }

        private void PresionaTecla(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && this.pictureBox1.Location.Y > 0)
            {
                this.pictureBox1.Top -= 36;
            }
            if (e.KeyCode == Keys.Down && this.pictureBox1.Location.Y < 326)
            {
                this.pictureBox1.Top += 36;
            }
        }

        private void Disparo(object sender, KeyEventArgs e)
        {
            if (balaviva == 0)
                if (e.KeyCode == Keys.Space)
                {
                    Bala = new System.Windows.Forms.PictureBox();
                    Bala.BackgroundImage = global::SpaceInvaderCliente.Properties.Resources.bala;
                    Bala.Location = new System.Drawing.Point(this.pictureBox1.Location.X + 36, this.pictureBox1.Location.Y);
                    Bala.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                    Bala.Margin = new System.Windows.Forms.Padding(0);
                    Bala.Size = new System.Drawing.Size(36, 36);
                    Bala.TabIndex = 0;
                    Bala.TabStop = false;
                    this.Controls.Add(Bala);
                    Bala.BringToFront();
                    balaviva = 1;
                    t2 = new Thread(new ThreadStart(MovimientoBala));
                    t2.Start();
                }
        }

        public void MovimientoBala()
        {
            int i = 0;
            if (salto == 0)
            {
                i = 0;
                while (i < 10)
                {
                    Thread.Sleep(200);
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(
                            delegate()
                            {
                                Bala.Left += 36;
                            }
                            ));
                    }
                    else
                    {
                        Bala.Left += 36;
                    }
                    i++;
                }

                salto = 1;
                posy = Bala.Location.Y;
                if (cliente.Connected)
                {
                    envia(salto.ToString(), posy.ToString());
                }
            }

            if (salto == 2)
            {
                i = 0;
                while (i < 12)
                {
                    Thread.Sleep(200);
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(
                            delegate()
                            {
                                Bala.Left -= 36;
                            }
                            ));
                    }
                    else
                    {
                        Bala.Left -= 36;
                    }
                    i++;
                }
                salto = 0;
                t2.Abort();
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(
                        delegate()
                        {
                            Bala.Dispose();
                        }
                        ));
                }
                else
                {
                    Bala.Dispose();
                }
                balaviva = 0;
                vidas += 1;
            }

            if (vidas > 9)
                MessageBox.Show("Los extraterrestres han invadido la Tierra gracias a tu incompetencia");

        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t1.Abort();
            t2.Abort();
            t3.Abort();
            t4.Abort();
            this.Close();
        }

        public void socketsin()
        {
            while (cliente.Connected)
            {
                recibe();
            }
        }

        public void envia(String s, String y)
        {
            NetworkStream stream = cliente.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(s + "," + y);
            stream.Write(data, 0, data.Length);
        }

        public void recibe()
        {

            NetworkStream stream = cliente.GetStream();
            int data = stream.Read(buffer, 0, 4096);
            salto = Convert.ToInt32(Encoding.ASCII.GetString(buffer, 0, data));
            while (salto == 2)
            {
                MovimientoBala();
                Thread.Sleep(200);
            }
            if (salto == 3)
            {
                salto = 0;
                balaviva = 0;
            }
        }

        public void Socket()
        {
            cliente = new TcpClient();
            cliente.Connect("127.0.0.1", 5000);
            t3 = new Thread(new ThreadStart(socketsin));
            t3.Start();
        }
    }
}
