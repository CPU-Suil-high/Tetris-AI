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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TetrisGame;

namespace TetrisGA {
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window {

        private Image[] tetrisImages;
        public Image[] TetrisImages {
            get {
                return tetrisImages;
            }
            set {
                tetrisImages = value;
            }
        }

        private DispatcherTimer placeTimer;
        public DispatcherTimer PlaceTimer {
            get {
                return placeTimer;
            }
            set {
                placeTimer = value;
            }
        }

        private TimeSpan nextInterval;
        public TimeSpan NextInterval {
            get {
                return nextInterval;
            }
            set {
                nextInterval = value;
            }
        }

        private TetrisAIManager taManager;
        public TetrisAIManager TAManager {
            get {
                return taManager;
            }
            set {
                taManager = value;
            }
        }

        private int placeCount;
        public int PlaceCount {
            get {
                return placeCount;
            }
            set {
                placeCount = value;
                placeCountLabel.Content = $"Place Count : {value}";
            }
        }

        private Label placeCountLabel;
        public Label PlaceCountLabel {
            get {
                return placeCountLabel;
            }
            set {
                placeCountLabel = value;
            }
        }

        private Label generationLabel;
        public Label GenerationLabel {
            get {
                return generationLabel;
            }
            set {
                generationLabel = value;
            }
        }

        private Label[] geneLabels;
        public Label[] GeneLabels {
            get {
                return geneLabels;
            }
            set {
                geneLabels = value;
            }
        }

        public MainWindow() {
            Image[] tetrisImages = new Image[25];

            for (int i = 0; i < 25; i++) {
                tetrisImages[i] = new Image();
            }

            TetrisImages = tetrisImages;

            TAManager = new TetrisAIManager(25);

            PlaceTimer = new DispatcherTimer();
            PlaceTimer.Tick += PlaceTimer_Tick;
            PlaceTimer.Interval = TimeSpan.FromMilliseconds(250);

            GeneLabels = new Label[25];

            InitializeComponent();
        }

        private void NextGeneration() {
            TAManager.NextGeneration();
            PlaceCount = 0;
            GenerationLabel.Content = $"Generation : {TAManager.Generation}";

            UpdateGenerationInfo();
        }

        private void PlaceTimer_Tick(object sender, EventArgs e) {
            if (PlaceTimer.Interval != NextInterval) {
                PlaceTimer.Interval = NextInterval;
            }
            if (TAManager.CheckAllIsGameOver() || ++PlaceCount > 200) {
                NextGeneration();
                return;
            }

            TAManager.PlaceMino();
            UpdateTetrisImages();
        }

        private void UpdateGenerationInfo() {
            for (int i = 0; i < 25; i++) {
                int[] gene = TAManager.Genes[i];
                GeneLabels[i].Content = $"[{i, 2}] : ({gene[0], 4},{gene[1], 4},{gene[2], 4},{gene[3], 4},{gene[4], 4},{gene[5], 4},{gene[6], 4},{gene[7], 4},{gene[8], 4})";
            }
        }

        private void UpdateTetrisImages() {
            for (int i = 0; i < 25; i++) {
                Tetris tetris = TAManager.Tetrises[i];

                DrawingGroup dg = new DrawingGroup();

                RectangleGeometry background = new RectangleGeometry(new Rect(0, 0, 50, 100));
                GeometryDrawing backgroundDrawing = new GeometryDrawing();
                backgroundDrawing.Geometry = background;
                backgroundDrawing.Brush = Brushes.Black;

                dg.Children.Add(backgroundDrawing);

                for (int y = 0; y < 20; y++) {
                    for (int x = 0; x < 10; x++) {
                        if (tetris.PlayField[y, x] != MinoType.None) {
                            RectangleGeometry rect = new RectangleGeometry(new Rect(x * 5, y * 5, 5, 5));
                            
                            GeometryDrawing gd = new GeometryDrawing();
                            gd.Geometry = rect;

                            switch (tetris.PlayField[y, x]) {
                                case MinoType.I:
                                    gd.Brush = Brushes.DeepSkyBlue;
                                    break;
                                case MinoType.O:
                                    gd.Brush = Brushes.Yellow;
                                    break;
                                case MinoType.T:
                                    gd.Brush = Brushes.DarkViolet;
                                    break;
                                case MinoType.L:
                                    gd.Brush = Brushes.Orange;
                                    break;
                                case MinoType.J:
                                    gd.Brush = Brushes.Blue;
                                    break;
                                case MinoType.S:
                                    gd.Brush = Brushes.LawnGreen;
                                    break;
                                case MinoType.Z:
                                    gd.Brush = Brushes.Red;
                                    break;
                            }

                            gd.Pen = new Pen(Brushes.Black, 1);

                            dg.Children.Add(gd);
                        }
                    }
                }

                TetrisImages[i].Source = new DrawingImage(dg);
            }
        }

        private void Canvas_Initialized(object sender, EventArgs e) {
            Canvas canvas = (Canvas)sender;
           
            for (int i = 0; i < 25; i++) {
                TetrisImages[i].Width = 50;
                TetrisImages[i].Height = 100;

                RectangleGeometry rect = new RectangleGeometry(new Rect(0, 0, 50, 100));

                GeometryDrawing drawing = new GeometryDrawing();
                drawing.Geometry = rect;
                drawing.Brush = Brushes.Black;

                DrawingImage drawingImage = new DrawingImage(drawing);

                TetrisImages[i].Stretch = Stretch.None;
                TetrisImages[i].Source = drawingImage;

                Canvas.SetLeft(TetrisImages[i], (i % 5) * 55);
                Canvas.SetTop(TetrisImages[i], (i / 5) * 105);

                canvas.Children.Add(TetrisImages[i]);

            }
        }

        private void Start(object sender, RoutedEventArgs e) {
            Button button = (Button)sender;

            button.Content = "Pause";

            button.Click -= Start;
            button.Click += Pause;

            PlaceTimer.Start();
        }

        private void Pause(object sender, RoutedEventArgs e) {
            Button button = (Button)sender;

            button.Content = "Start";

            button.Click += Start;
            button.Click -= Pause;

            PlaceTimer.Stop();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
           Slider slider = (Slider)sender;

            NextInterval = TimeSpan.FromMilliseconds(500 - (int)slider.Value);
        }

        private void PlaceCountLabel_Initialized(object sender, EventArgs e) {
            PlaceCountLabel = (Label)sender;
        }

        private void GenerationLabel_Initialized(object sender, EventArgs e) {
            GenerationLabel = (Label)sender;
        }

        private void GenerationInfoGrid_Initialized(object sender, EventArgs e) {
            Grid grid = (Grid)sender;


            for (int i = 0; i < 25; i++) {
                Label geneLabel = new Label();

                geneLabel.Foreground = Brushes.White;
                geneLabel.FontFamily = new FontFamily("Consolas");

                geneLabel.Margin = new Thickness(10, i * 15 + 5, 0, 0);

                GeneLabels[i] = geneLabel;
                grid.Children.Add(geneLabel);
            }

            UpdateGenerationInfo();
        }
    }
}
