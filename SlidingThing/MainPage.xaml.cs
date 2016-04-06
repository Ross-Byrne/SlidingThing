using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SlidingThing
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private double _ellipseHeight, _ellipseWidth, _gridSize;
        Image img = new Image();
        public MainPage()
        {
            this.InitializeComponent();

        }

        private void rbSize_Checked(object sender, RoutedEventArgs e) {
            RadioButton curr = (RadioButton)sender;
            // get the number from contect 4 x 4

            int gridSize = Convert.ToInt32(curr.Content.ToString().Substring(0,
                                            Convert.ToInt32(curr.Content.ToString().IndexOf(" "))));

            _gridSize = gridSize;
            createGameGrid(gridSize);

            // add picture
            
            img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Pictures/tree.png"));
            img.Width = 600;
            img.Height = 600;

           // MainGrid.Children.Add(img);


            /*  img.HorizontalAlignment = HorizontalAlignment.Center;
              img.VerticalAlignment = VerticalAlignment.Center;
              img.SetValue(Grid.RowProperty, 0);
              img.SetValue(Grid.ColumnProperty, 0);


              contentGrid.Children.Add(img);*/

        }

        private void createGameGrid(int gridSize) {
            int cols, rows;
            // create row and col definitions
            for(cols = 0; cols < gridSize; cols++) {
                contentGrid.RowDefinitions.Add(new RowDefinition());
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // add canvas to the grid squares
            // make them a little smaller
            Canvas myC;
            for(cols = 0; cols < gridSize; cols++) {
                for(rows = 0; rows < gridSize; rows++) {
                    myC = new Canvas();
                    myC.Name = "c" + cols.ToString() + "_" + rows.ToString(); // r0_1
                    myC.Width = (contentGrid.Width / gridSize) - 4;
                    myC.Height = (contentGrid.Height / gridSize) - 4;
                    myC.HorizontalAlignment = HorizontalAlignment.Center;
                    myC.VerticalAlignment = VerticalAlignment.Center;
                    myC.SetValue(Grid.RowProperty, rows);
                    myC.SetValue(Grid.ColumnProperty, cols);

                    Image other = new Image();

                    other.Source = new BitmapImage(new Uri("ms-appx:///Assets/Pictures/tree.png"));
                    other.Width = 600;
                    other.Height = 600;

                    RectangleGeometry r = new RectangleGeometry();
                    r.Rect = new Rect(rows, cols, (contentGrid.Width / gridSize) - 4, (contentGrid.Height / gridSize) - 4);
                    other.Clip = r;

                    

                    myC.Children.Add(other);

                    myC.Tapped += MyR_Tapped;
                    contentGrid.Children.Add(myC);
                }
            }

            Ellipse myE;

            //TransformGroup myTransformGroup = new TransformGroup();
            //TranslateTransform transTransForm;
            //transTransForm = new TranslateTransform();
            //transTransForm.X = contentGrid.Width * ((gridSize - 1) / gridSize);
            //transTransForm.Y = contentGrid.Height* ((gridSize - 1) / gridSize);
            //transTransForm.SetValue(NameProperty, "myTranslateTransform");
            //myTransformGroup.Children.Add(transTransForm);


            for(cols = 0; cols < 2; cols++) {
                myE = new Ellipse();
                myE.Name = "piece" + cols.ToString();
                myE.Height = (contentGrid.Height / gridSize) - 6;
                myE.Width = (contentGrid.Width / gridSize) - 6;
                myE.Stroke = new SolidColorBrush(Colors.Red);
                myE.StrokeThickness = 0;
                myE.Fill = new SolidColorBrush(Colors.Yellow);

                myE.SetValue(Grid.RowProperty, cols);
                myE.SetValue(Grid.ColumnProperty, cols);
                myE.Tapped += MyE_Tapped;

                //myE.RenderTransform = myTransformGroup;

                _ellipseHeight = myE.Height;
                _ellipseWidth = myE.Width;

                contentGrid.Children.Add(myE);
            }


        }

        private void MyR_Tapped(object sender, TappedRoutedEventArgs e) {
            if(_current == null) return;


            Canvas canNew = (Canvas)sender;
            _newCanvas = canNew;

            // run the disappear storyboard.
            dblAniHeight.SetValue(Storyboard.TargetNameProperty, _current.Name.ToString());
            dblAniHeight.From = _current.Height;
            dblAniHeight.To = 0;

            dblAniwidth.SetValue(Storyboard.TargetNameProperty, _current.Name.ToString());
            dblAniwidth.From = _current.Width;
            dblAniwidth.To = 0;


            sbNowYouSeeIt.Begin();

        }

        private bool animationRunning;
        private Ellipse _current;
        private Canvas _newCanvas;
        private void MyE_Tapped(object sender, TappedRoutedEventArgs e) {
            // set off the animation using the sender as the target
            Ellipse curr = (Ellipse)sender;


            if(animationRunning == false) {
                dblAniStrokeT.SetValue(Storyboard.TargetNameProperty, curr.Name.ToString());
                sbStrokeAnimation.Begin();
                _current = curr;
                animationRunning = true;
            } else {
                sbStrokeAnimation.Stop();
                _current = null;
                animationRunning = false;
            }



        }

        private bool _hasRunOnce;
        private void sbNowYouSeeIt_Completed(object sender, object e) {
            // runs after the storyboard finishes
            sbStrokeAnimation.Stop();

            _current.SetValue(Grid.RowProperty, _newCanvas.GetValue(Grid.RowProperty));
            _current.SetValue(Grid.ColumnProperty, _newCanvas.GetValue(Grid.ColumnProperty));

            sbNowYouSeeIt.Stop();

            // run the disappear storyboard.
            dblAniHeight.SetValue(Storyboard.TargetNameProperty, _current.Name.ToString());
            dblAniHeight.To = _current.Height;
            //dblAniHeight.From = 0;

            dblAniwidth.SetValue(Storyboard.TargetNameProperty, _current.Name.ToString());
            dblAniwidth.To = _current.Width;
            //dblAniwidth.From = 0;

            if(_hasRunOnce == false) {
                _hasRunOnce = true;
                sbNowYouSeeIt.Begin();
            } else {
                _hasRunOnce = false;
                //_current = null;
            }

        }
    }
}
