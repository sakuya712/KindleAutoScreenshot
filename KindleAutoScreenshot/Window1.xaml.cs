using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KindleAutoScreenshot
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class CaptureAreaWindow : Window
    {
        public CaptureAreaWindow()
        {
            InitializeComponent();
            //カーソルを十字にする
            Cursor = Cursors.Cross;
        }
        //最初の座標
        private Point StartPos;
        //最後の座標
        private Point EndPos;
        //大きさ
        public int XSize { get; private set; }
        public int YSize { get; private set; }
        public int StartPosX { get { return Convert.ToInt32(StartPos.X); } }
        public int StartPosY { get { return Convert.ToInt32(StartPos.Y); } }
        public int EndPosX { get { return Convert.ToInt32(EndPos.X); } }
        public int EndPosY { get { return Convert.ToInt32(EndPos.Y); } }

        //ドラッグ判定
        private bool IsDrag;
        //前回の描写
        private UIElement LastElement;

        //左クリックしたときのイベント
        private void SelectAreaCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //現在の座標を取得
            Canvas c = sender as Canvas;
            this.StartPos = e.GetPosition(c);
            this.IsDrag = true;
        }
        //マウスが移動しているときのイベント
        private void SelectAreaCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDrag)
            {
                //マウスの現在位置を取得
                Point point = e.GetPosition(SelectAreaCanvas);
                //描写の処理
                //RectangleView(point);
            }
        }
        //左クリックを離したときのイベント
        private void SelectAreaCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsDrag)
            {
                //現在の座標を取得
                Canvas c = sender as Canvas;
                this.EndPos = e.GetPosition(c);
                this.IsDrag = false;
                XSize = Convert.ToInt32(Math.Abs(StartPos.X - EndPos.X));
                YSize = Convert.ToInt32(Math.Abs(StartPos.Y - EndPos.Y));
                this.Close();
            }
        }

        private void RectangleView(Point Pos)
        {
            //前回の描写を消す
            try
            {
                SelectAreaCanvas.Children.Remove(LastElement);
            }
            catch
            {
                //なにもしない
            }
            Rectangle rectangle = new Rectangle();
            //枠線
            rectangle.Stroke = new SolidColorBrush(Colors.Red);
            rectangle.StrokeThickness = 5;
            //座標
            rectangle.Width = Math.Abs(StartPos.X - Pos.X);
            rectangle.Height = Math.Abs(StartPos.Y - Pos.Y);

            //SelectAreaCanvasに追加する座標を決める
            if (StartPos.X < Pos.X)
            {
                Canvas.SetLeft(rectangle, StartPos.X);
            }
            else
            {
                Canvas.SetLeft(rectangle, Pos.X);
            }
            if (StartPos.Y < Pos.Y)
            {
                Canvas.SetTop(rectangle, StartPos.Y);
            }
            else
            {
                Canvas.SetTop(rectangle, Pos.Y);
            }
            //要素を追加する
            SelectAreaCanvas.Children.Add(rectangle);
            LastElement = rectangle;
        }

        private void SelectAreaCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsDrag = false;
        }
    }
}
