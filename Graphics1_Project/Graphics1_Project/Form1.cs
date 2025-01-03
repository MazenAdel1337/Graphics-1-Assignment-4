﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bezier;
using Lec_1_DDA;

namespace Graphics1_Project
{
    public class Cball
    {
        public PointF ball;
        public int color;
        public float tball=0;
        public Bitmap img;
        public DDALine Line;
    }

    public partial class Form1 : Form
    {
        Bitmap off, newImage, backGroundImg,img;
        Bitmap image = new Bitmap("zuma.png");
        Bitmap ball = new Bitmap("blue.png");
        Bitmap gameover = new Bitmap("gameover.png");

        bool flagPic = false;
        double angle;

        int mouseX, mouseY, zomaX = 100, zomaY = 310, val;
        int deltaX, deltaY;
        Point previousPosition;

        BezCurve mycurve = new BezCurve();
        PointF myball = new PointF(100, 310);
        bool goball = false;
        float tball = 0;

        List<Cball> zumaBalls = new List<Cball>();
        List<Cball> fireBalls = new List<Cball>();

        Random rnd = new Random();

        DDALine Line,LineCurs;
        int clickX, clickY;
        bool flagFire = false;

        double angleDegrees;

        bool flagDone = false,flagStart=false;
        int color;
        List<int> MatchR = new List<int>();
        List<int> MatchL = new List<int>();

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Paint += Form1_Paint;
            this.Load += Form1_Load;
            this.KeyDown += Form1_KeyDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseDown += Form1_MouseDown;
            Timer t = new Timer();
            t.Start();
            t.Tick += T_Tick;
            previousPosition = MousePosition;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            clickX = e.X;
            clickY = e.Y;
            Line = new DDALine(zomaX + (newImage.Width + 70) / 2, zomaY + (newImage.Height + 70) / 2, clickX, clickY);
            
            createFireBall(Line,color);
            flagFire = true;
            flagStart = true;
            color = rnd.Next(3);

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            LineCurs = new DDALine(zomaX, zomaY, mouseX, mouseY);

            Point mousePos = MousePosition;
            deltaX = Math.Abs(mousePos.X - previousPosition.X);
            deltaY = Math.Abs(mousePos.Y - previousPosition.Y);
            previousPosition = MousePosition;
            double angleRadians = Math.Atan2(deltaY, deltaX);
            angleDegrees = (angleRadians * 180) / Math.PI;
            /*if (deltaX > 0)
            {
                angle += angleDegrees / 10;
            }
            else
            {
                angle -= angleDegrees / 10;
            }*/
            flagPic = true;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            //zuma rotation
            if (flagPic)
            {
                if (mouseY > zomaY)
                {
                    val = deltaY;
                }
                else if (mouseY < zomaY)
                {
                    val = -deltaY;
                }
                angle += val*3;
            }

            if (goball)
            {
                tball += 0.0005f;/////
                for(int i=0;i<zumaBalls.Count();i++)
                {
                    if (i == 0)
                    {
                        zumaBalls[i].tball = tball;
                        zumaBalls[i].ball = mycurve.CalcCurvePointAtTime(zumaBalls[i].tball);
                    }
                    else
                    {
                        zumaBalls[i].tball = zumaBalls[i-1].tball - 0.015f;
                        zumaBalls[i].ball = mycurve.CalcCurvePointAtTime(zumaBalls[i].tball);
                    }
                }
            }

            if (flagFire)
            {
                for (int i = 0; i < fireBalls.Count(); i++)
                {
                    fireBalls[i].ball = fireBalls[i].Line.getnextpoint(fireBalls[i].ball.X, fireBalls[i].ball.Y);
                }

                if (flagStart)
                {
                    for (int i = 0; i < fireBalls.Count(); i++)
                    {
                        for (int j = 0; j < zumaBalls.Count(); j++)
                        {
                            if (fireBalls[i].ball.X > zumaBalls[j].ball.X && fireBalls[i].ball.X < zumaBalls[j].ball.X + zumaBalls[j].img.Width
                                && fireBalls[i].ball.Y < zumaBalls[j].ball.Y && fireBalls[i].ball.Y > zumaBalls[j].ball.Y - zumaBalls[j].img.Height
                                )
                            {
                                zumaBalls.Insert(j, fireBalls[i]);
                                fireBalls.RemoveAt(i);
                                checkForMatch(zumaBalls,j);
                                flagDone = true;
                                flagStart = false;
                                break;
                            }
                        }
                        if (flagDone)
                        {
                            flagDone = false;
                            break;
                        }
                    }
                }

            }
            DrawDubb(this.CreateGraphics());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            backGroundImg = new Bitmap("background.jpg");
            createCurve();
            createBalls();

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                goball = true;
            }
        }

        void DrawScene(Graphics g)
        {
            //rotation
            g.DrawImage(backGroundImg, 0, 0, 1536, 811);
            g.DrawImage(gameover, 230, 340,gameover.Width+50,gameover.Height+50);
            newImage = RotateImage1(image, (float)angle);

            //curve
            mycurve.DrawYourSelf(g);
            if (flagPic)
            {
                g.DrawImage(newImage, zomaX, zomaY, newImage.Width + 70, newImage.Height + 70);
                g.DrawLine(Pens.Blue, zomaX + (newImage.Width + 70)/2, zomaY + (newImage.Height + 70)/2, mouseX, mouseY);
            }

            if (goball == true)
            {
                if (color == 0) { img = new Bitmap("blue.png"); }
                else if (color == 1) { img = new Bitmap("yellow.jpg"); }
                else if (color == 2) { img = new Bitmap("red.jpg"); }
                else if (color == 3) { img = new Bitmap("green.jpg"); }
                g.DrawImage(img, myball.X - 25, myball.Y - 25, 50, 50);
                //fireballs
                for (int i = 0; i < fireBalls.Count(); i++)
                {
                    g.DrawImage(fireBalls[i].img, fireBalls[i].ball.X - 25, fireBalls[i].ball.Y - 25, 50, 50);
  
                }
                ball.MakeTransparent(ball.GetPixel(0, 0));

                //curve balls
                for(int i = 0; i < zumaBalls.Count(); i++)
                {
                    g.DrawImage(zumaBalls[i].img, zumaBalls[i].ball.X - 25, zumaBalls[i].ball.Y - 25, 50, 50);
                }   
            }
        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }

        public static Bitmap RotateImage1(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            PointF offset = new PointF((float)image.Width / 2, (float)image.Height / 2);

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }


        void checkForMatch(List<Cball> zumaBalls, int index)
        {
            for (int i = index; i < zumaBalls.Count(); i++)
            {
                if (zumaBalls[index].color == zumaBalls[i].color)
                {
                    MatchR.Add(i);
                }
                else
                {
                    break;
                }
            }

            ///
            for (int i = index - 1; i >= 0; i--)
            {
                if (zumaBalls[index].color == zumaBalls[i].color)
                {
                    MatchL.Add(i);
                }
                else
                {
                    break;
                }
            }
            if (MatchR.Count() + MatchL.Count() > 2)
            {
                for (int i = 0; i <MatchR.Count; i++)
                {
                    //MessageBox.Show("right:" + MatchR[i]);
                    zumaBalls.RemoveAt(MatchR[i]);
                }

                for (int i = 0; i < MatchL.Count; i++)
                {
                    //MessageBox.Show("left:" + MatchL[i]);
                    zumaBalls.RemoveAt(MatchL[i]);
                }
            }
            MatchR.Clear();
            MatchL.Clear();
        }

        void createBalls()
        {
            for (int i=0; i < 20; i++)
            {
                Cball pnn = new Cball();
                pnn.color = rnd.Next(3);
                if(pnn.color == 0) { pnn.img = new Bitmap("blue.png"); }
                else if (pnn.color == 1) { pnn.img = new Bitmap("yellow.jpg"); }
                else if (pnn.color == 2) { pnn.img = new Bitmap("red.jpg"); }
                else if (pnn.color == 3) { pnn.img = new Bitmap("green.jpg"); }
                pnn.img.MakeTransparent(pnn.img.GetPixel(0, 0));
                zumaBalls.Add(pnn);
            }
        }

        void createFireBall(DDALine Line,int color)
        {
            Cball pnn = new Cball();
            pnn.ball.X = zomaX + (newImage.Width + 70) / 2;
            pnn.ball.Y = zomaY + (newImage.Height + 70) / 2;
            pnn.Line = Line;
            pnn.color = color;
            if (pnn.color == 0) { pnn.img = new Bitmap("blue.png"); }
            else if (pnn.color == 1) { pnn.img = new Bitmap("yellow.jpg"); }
            else if (pnn.color == 2) { pnn.img = new Bitmap("red.jpg"); }
            else if (pnn.color == 3) { pnn.img = new Bitmap("green.jpg"); }
            pnn.img.MakeTransparent(pnn.img.GetPixel(0, 0));
            fireBalls.Add(pnn);
        }

        void createCurve()
        {
            mycurve.LCtrPts.Add(new PointF(0, 223));
            mycurve.LCtrPts.Add(new PointF(109, 181));
            mycurve.LCtrPts.Add(new PointF(185, 136));
            mycurve.LCtrPts.Add(new PointF(260, 142));
            mycurve.LCtrPts.Add(new PointF(346, 124));
            mycurve.LCtrPts.Add(new PointF(425, 79));
            mycurve.LCtrPts.Add(new PointF(516, 87));
            mycurve.LCtrPts.Add(new PointF(614, 75));
            mycurve.LCtrPts.Add(new PointF(691, 74));
            mycurve.LCtrPts.Add(new PointF(761, 47));
            mycurve.LCtrPts.Add(new PointF(830, 64));
            mycurve.LCtrPts.Add(new PointF(898, 36));
            mycurve.LCtrPts.Add(new PointF(960, 49));
            mycurve.LCtrPts.Add(new PointF(1034, 81));            
            mycurve.LCtrPts.Add(new PointF(1100, 67));
            mycurve.LCtrPts.Add(new PointF(1148, 109));
            mycurve.LCtrPts.Add(new PointF(1187, 123));
            mycurve.LCtrPts.Add(new PointF(1266, 112));
            mycurve.LCtrPts.Add(new PointF(1351, 169));
            mycurve.LCtrPts.Add(new PointF(1454, 203));
            mycurve.LCtrPts.Add(new PointF(1534, 267));
            mycurve.LCtrPts.Add(new PointF(1535, 359));
            mycurve.LCtrPts.Add(new PointF(1535, 478));
            mycurve.LCtrPts.Add(new PointF(1535, 479));
            mycurve.LCtrPts.Add(new PointF(1458, 523));
            mycurve.LCtrPts.Add(new PointF(1333, 541));          
            mycurve.LCtrPts.Add(new PointF(1243, 530));
            mycurve.LCtrPts.Add(new PointF(1192, 482));
            mycurve.LCtrPts.Add(new PointF(1236, 396));
            mycurve.LCtrPts.Add(new PointF(1254, 303));
            mycurve.LCtrPts.Add(new PointF(1193, 150));            
            mycurve.LCtrPts.Add(new PointF(1030, 163));
            mycurve.LCtrPts.Add(new PointF(906, 151));
            mycurve.LCtrPts.Add(new PointF(803, 161));
            mycurve.LCtrPts.Add(new PointF(709, 170));
        }
    }
}
