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

namespace SpaceInvaderServidor
{
    public partial class Form1 : Form
    {
        Thread t1, t2, t3, t4, t5;
        int[] vecescudo;
        System.Windows.Forms.PictureBox[] Aliens;
        System.Windows.Forms.PictureBox Bala;
        int i = 0, cont = 0, salto = 0, band = 0, posy = 0, escudo = 0, impacto = 0, vidas = 0;
        TcpClient conectado;
        TcpListener listener = new TcpListener(IPAddress.Any, 5000);
        byte[] buffer = new byte[4096];

        public Form1()
        {
            InitializeComponent();
            vecescudo = new int[33];
            Aliens = new System.Windows.Forms.PictureBox[33];
            for (i = 0; i < 33; i++)
            {
                Aliens[i] = new System.Windows.Forms.PictureBox();
                Aliens[i].Location = new System.Drawing.Point(396, 0);
                vecescudo[i] = 0;
            }
            i = 0;
            t1 = new Thread(new ThreadStart(Tiempo));
            t1.Start();
            t2 = new Thread(new ThreadStart(Escudo));
            t2.Start();
            t3 = new Thread(new ThreadStart(Socket));
            t3.Start();
        }

        public void Alien()
        {
            int y = 0, j = 0;

            while (j < 33)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(
                        delegate()
                        {
                            Aliens[j].Left -= 36;
                        }
                        ));
                }
                else
                {
                    Aliens[j].Left -= 36;
                }

                j++;
            }

            while (i < cont)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(
                        delegate()
                        {
                            Aliens[i].BackgroundImage = global::SpaceInvaderServidor.Properties.Resources.alien;
                            Aliens[i].Location = new System.Drawing.Point(396, y);
                            Aliens[i].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                            Aliens[i].Margin = new System.Windows.Forms.Padding(0);
                            Aliens[i].Size = new System.Drawing.Size(36, 36);
                            Aliens[i].TabIndex = 0;
                            Aliens[i].TabStop = false;
                            this.Controls.Add(Aliens[i]);
                            Aliens[i].BringToFront();
                        }
                        ));
                }
                else
                {
                    Aliens[i].BackgroundImage = global::SpaceInvaderServidor.Properties.Resources.alien;
                    Aliens[i].Location = new System.Drawing.Point(396, y);
                    Aliens[i].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                    Aliens[i].Margin = new System.Windows.Forms.Padding(0);
                    Aliens[i].Size = new System.Drawing.Size(36, 36);
                    Aliens[i].TabIndex = 0;
                    Aliens[i].TabStop = false;
                    this.Controls.Add(Aliens[i]);
                    Aliens[i].BringToFront();
                }

                y += 36;
                i++;
            }

            if (cont < 33)
                cont = cont + 11;
        }

        public void disparo(int y)
        {
            if (band == 0)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(
                        delegate()
                        {
                            Bala = new System.Windows.Forms.PictureBox();
                            Bala.BackgroundImage = global::SpaceInvaderServidor.Properties.Resources.bala;
                            Bala.Location = new System.Drawing.Point(0, y);
                            Bala.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                            Bala.Margin = new System.Windows.Forms.Padding(0);
                            Bala.Size = new System.Drawing.Size(36, 36);
                            Bala.TabIndex = 0;
                            Bala.TabStop = false;
                            this.Controls.Add(Bala);
                            Bala.BringToFront();
                        }
                        ));
                }
                else
                {
                    Bala = new System.Windows.Forms.PictureBox();
                    Bala.BackgroundImage = global::SpaceInvaderServidor.Properties.Resources.bala;
                    Bala.Location = new System.Drawing.Point(0, y);
                    Bala.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                    Bala.Margin = new System.Windows.Forms.Padding(0);
                    Bala.Size = new System.Drawing.Size(36, 36);
                    Bala.TabIndex = 0;
                    Bala.TabStop = false;
                    this.Controls.Add(Bala);
                    Bala.BringToFront();
                }
                band = 1;
            }
        }

        public void MovimientoBala()
        {

            int j = 0;
            while (j < cont)
            {
                if (Bala.Location.X + 72 > Aliens[j].Location.X && vecescudo[j] == 1 && Aliens[j].Location.Y == Bala.Location.Y)
                {
                    escudo = 1;
                }
                else
                    if (Bala.Location.X + 72 > Aliens[j].Location.X && vecescudo[j] == 0 && Aliens[j].Location.Y == Bala.Location.Y)
                    {
                        vecescudo[j] = 2;
                        Aliens[j].BackgroundImage = null;
                        impacto = 1;
                        vidas ++;
                    }

                j++;
            }

            if (escudo == 1)
            {
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
            }
            else
                if (escudo == 0)
                {
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
                }

            if (Bala.Location.X < -36 && salto == 1)
            {
                salto = 0;
                escudo = 0;
                band = 0;
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
                if (conectado.Connected)
                    envia(2.ToString());
            }

            if (impacto == 1 || Bala.Location.X > 396)
            {
                salto = 0;
                escudo = 0;
                band = 0;
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
                if (conectado.Connected)
                    envia(3.ToString());

                impacto = 0;
            }

            if (vidas > 32)
                MessageBox.Show("Usted a salvado a el planeta Tierra de una invasion extraterrestre gracias =D");
            for (int k = 0; k < 33; k++)
            {
                if (vecescudo[k] != 2 && Aliens[k].Location.X < 0)
                    MessageBox.Show("Los extraterrestres han invadido la Tierra gracias a tu incompetencia");
            }
        }

        public void Escudo()
        {
            int j;
            while (true)
            {
                j = 0;
                while (j < cont)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(
                            delegate()
                            {
                                if (vecescudo[j] == 1)
                                {
                                    Aliens[j].BackgroundImage = global::SpaceInvaderServidor.Properties.Resources.alien;
                                    vecescudo[j] = 0;
                                }
                            }
                            ));
                    }
                    else
                    {
                        if (vecescudo[j] == 1)
                        {
                            Aliens[j].BackgroundImage = global::SpaceInvaderServidor.Properties.Resources.alien;
                            vecescudo[j] = 0;
                        }
                    }

                    j++;
                }
                Thread.Sleep(5000);

                j = 0;
                while (j < cont)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(
                            delegate()
                            {
                                if (vecescudo[j] == 0)
                                {
                                    Aliens[j].BackgroundImage = global::SpaceInvaderServidor.Properties.Resources.alienescudo;
                                    vecescudo[j] = 1;
                                }
                            }
                            ));
                    }
                    else
                    {
                        if (vecescudo[j] == 0)
                        {
                            Aliens[j].BackgroundImage = global::SpaceInvaderServidor.Properties.Resources.alienescudo;
                            vecescudo[j] = 1;
                        }
                    }

                    j++;
                }
                Thread.Sleep(2000);
            }
        }

        public void Tiempo()
        {
            Random r = new Random();
            while (true)
            {
                Alien();
                Thread.Sleep(r.Next(10000, 20000));
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t1.Abort();
            t2.Abort();
            t3.Abort();
            this.Close();
        }

        public void socketsin()
        {
            while (true)
            {
                recibe();
            }
        }

        public void recibe()
        {
            NetworkStream lee = conectado.GetStream();
            int data = lee.Read(buffer, 0, 4096);
            string str = Encoding.ASCII.GetString(buffer, 0, data);
            char[] seps = { ',' };
            string[] parts = str.Split(seps);
            salto = Convert.ToInt32(parts[0]);
            posy = Convert.ToInt32(parts[1]);
            if (salto == 1)
            {
                disparo(posy);
                while (salto == 1)
                {
                    MovimientoBala();
                    Thread.Sleep(200);
                }
            }
        }

        public void envia(String a)
        {
            byte[] data = Encoding.ASCII.GetBytes(a);
            NetworkStream stream = conectado.GetStream();
            stream.Write(data, 0, data.Length);
        }

        public void Socket()
        {
            listener.Start();
            conectado = listener.AcceptTcpClient();
            t4 = new Thread(new ThreadStart(socketsin));
            t4.Start();
        }
    }
}
