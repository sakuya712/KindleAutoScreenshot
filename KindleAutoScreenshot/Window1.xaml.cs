using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace KindleAutoScreenshot
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class CaptureAreaWindow : Window
    {
        //選択用のThumb
        private Thumb SelectArea;
        //マウスカーソルの形を矢印に変更する範囲
        private const double PaddingRegion = 10;
        //直前の横幅変化量を記録
        private double ChangedHorizontal;
        //直前の縦幅変化量を記録
        private double ChangedVertical;
        public CaptureAreaWindow()
        {
            InitializeComponent();

            // 選択用のThumbを作成する
            SelectArea = new Thumb( );
            SelectArea.Width = 500;
            SelectArea.Height = 500;
            SelectArea.Background = Brushes.DarkRed;
            SelectArea.Opacity = 0.5;
            SelectArea.MouseMove += SelectArea_MouseMove;
            SelectArea.DragDelta += SelectArea_DragDelta;
            SelectArea.DragCompleted += SelectArea_DragCompleted;
            Canvas.SetLeft(SelectArea, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2 - SelectArea.Width / 2 );
            Canvas.SetTop(SelectArea, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2 - SelectArea.Height / 2);
            SelectAreaCanvas.Children.Add(SelectArea);

            //選択完了ボタンを作成
            Button SelectedButton = new Button();
            SelectedButton.Content = "選択範囲決定";
            SelectedButton.FontSize = 20;
            SelectedButton.Height = 100;
            SelectedButton.Width = 200;
            SelectedButton.Opacity = 1;
            Canvas.SetLeft(SelectedButton, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2 - SelectedButton.Width / 2);
            Canvas.SetTop(SelectedButton, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2 - SelectedButton.Height / 2);
            SelectedButton.Click += SelectedButton_Click;
            SelectAreaCanvas.Children.Add(SelectedButton);
        }

        //範囲大きさ
        public int XSize { get; private set; }
        public int YSize { get; private set; }
        //範囲の座標
        public int StartPosX { get; private set; }
        public int StartPosY { get; private set; }
        public int EndPosX { get; private set; }
        public int EndPosY { get; private set; }

        //選択完了ボタンを押したとき
        void SelectedButton_Click(Object sender, EventArgs e)
        {
            StartPosX = Convert.ToInt32(Canvas.GetLeft(SelectArea));
            StartPosY = Convert.ToInt32(Canvas.GetTop(SelectArea));
            EndPosX = StartPosX + Convert.ToInt32(SelectArea.Width);
            EndPosY = StartPosY + Convert.ToInt32(SelectArea.Height);
            XSize = Convert.ToInt32(SelectArea.Width);
            YSize = Convert.ToInt32(SelectArea.Height);
            this.Close();
        }

        //マウス移動しているときのイベント
        //ここでマウスカーソルのアイコンを変える
        private void SelectArea_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(SelectAreaCanvas);
            double left = Canvas.GetLeft(SelectArea);
            double top = Canvas.GetTop(SelectArea);
            double width = SelectArea.Width;
            double height = SelectArea.Height;
            if (p.X > left + width - PaddingRegion && p.Y > top + height - PaddingRegion)
            {
                //右下にあるときは斜め下の矢印
                SelectArea.Cursor = Cursors.SizeNWSE;
            }
            else if (p.X > left + width - PaddingRegion)
            {
                //右側にあるときは左右の矢印
                SelectArea.Cursor = Cursors.SizeWE;
            }
            else if (p.Y > top + height - PaddingRegion)
            {
                //下側にあるときは上下の矢印
                SelectArea.Cursor = Cursors.SizeNS;
            }
            else
            {
                //該当しない場合は普通のカーソル
                SelectArea.Cursor = Cursors.Arrow;
            }
        }

        //ドラッグ中のイベント
        //SelectArea_MouseMoveで変えたカーソルの種類に応じてドラッグの処理を行う
        private void SelectArea_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //カーソルの形で判定
            if (SelectArea.Cursor == Cursors.SizeWE)
            {
                //左右
                ChangeWidthSize();
            }
            else if (SelectArea.Cursor == Cursors.SizeNS)
            {
                //上下
                ChangeHeightSize();
            }
            else if (SelectArea.Cursor == Cursors.SizeNWSE)
            {
                //斜め
                ChangeWidthSize();
                ChangeHeightSize();
            }
            else if (SelectArea.Cursor == Cursors.Arrow)
            {
                //通常のカーソル
                //ドラッグ移動
                Canvas.SetLeft(SelectArea, Canvas.GetLeft(SelectArea) + e.HorizontalChange);
                Canvas.SetTop(SelectArea, Canvas.GetTop(SelectArea) + e.VerticalChange);
            }

            //左右のサイズを変化する処理
            void ChangeWidthSize()
            {
                double w = SelectArea.Width + e.HorizontalChange - ChangedHorizontal;
                //小さくなりすぎないようにする
                if(PaddingRegion > w)
                {
                    ChangedHorizontal = 0;
                }
                else
                {
                    SelectArea.Width = w;
                    //変化量を記録
                    ChangedHorizontal = e.HorizontalChange;
                }
            }
            //上下のサイズを変化する処理
            void ChangeHeightSize()
            {
                double h = SelectArea.Height + e.VerticalChange - ChangedVertical;
                //小さくなりすぎないようにする
                if (PaddingRegion > h)
                {
                    ChangedVertical = 0;
                }
                else
                {
                    SelectArea.Height = h;
                    //変化量を記録
                    ChangedVertical = e.VerticalChange;
                }
            }

        }
        //ドラッグが終了したときのイベント
        //変更量を0にする。
        private void SelectArea_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ChangedHorizontal = 0;
            ChangedVertical = 0;
        }
    }
}
