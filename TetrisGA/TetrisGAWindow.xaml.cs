using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
    public partial class TetrisGAWindow : Window {

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

        private Slider slider;
        public Slider Slider {
            get {
                return slider;
            }
            set {
                slider = value;
            }
        }

        private Button startAndPauseButton;
        public Button StartAndPauseButton {
            get {
                return startAndPauseButton;
            }
            set {
                startAndPauseButton = value;
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

        private int[] previousBestGene;
        public int[] PreviousBestGene {
            get {
                return previousBestGene;
            }
            set {
                previousBestGene = value;
            }
        }

        private Label previousBestGeneLabel;
        public Label PreviousBestGeneLabel {
            get {
                return previousBestGeneLabel;
            }
            set {
                previousBestGeneLabel = value;
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

        public TetrisGAWindow() {
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
            PreviousBestGene = TAManager.GetBestGenes(1)[0];

            TAManager.NextGeneration();
            PlaceCount = 0;

            UpdateGenerationInfo();
        }

        private void PlaceTimer_Tick(object sender, EventArgs e) {
            if (PlaceTimer.Interval != NextInterval) {
                PlaceTimer.Interval = NextInterval;
            }
            if (TAManager.CheckAllIsGameOver() || ++PlaceCount > 1000) {
                NextGeneration();
                return;
            }

            TAManager.PlaceMino();
            UpdateTetrisImages();
        }

        private void UpdateGenerationInfo() {
            GenerationLabel.Content = $"Generation : {TAManager.Generation}";

            if (PreviousBestGene == null) {
                PreviousBestGeneLabel.Content = "[Previous Best Gene] : ()";
            } else {
                object[] args = new object[9];

                for (int i = 0; i < 9; i++) {
                    args[i] = (object)PreviousBestGene[i];
                }
                PreviousBestGeneLabel.Content = string.Format("[Previous Best Gene] : ({0, 4},{1, 4},{2, 4},{3, 4},{4, 4},{5, 4},{6, 4},{7, 4},{8, 4} )", args);
            }

            for (int i = 0; i < 25; i++) {
                int[] gene = TAManager.Genes[i];

                object[] args = new object[9];

                for (int j = 0; j < 9; j++) {
                    args[j] = (object)gene[j];
                }
                GeneLabels[i].Content = $"[{i,2}]" + string.Format(" : ({0, 4},{1, 4},{2, 4},{3, 4},{4, 4},{5, 4},{6, 4},{7, 4},{8, 4} )", args);
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

                FormattedText num = new FormattedText($"{i}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Consolas"), 10, Brushes.Gray, 120);

                Geometry numGeomtry = num.BuildGeometry(new Point(0, 0));
                GeometryDrawing numDrawing = new GeometryDrawing();
                numDrawing.Geometry = numGeomtry;
                numDrawing.Brush = Brushes.Gray;

                dg.Children.Add(backgroundDrawing);
                dg.Children.Add(numDrawing);

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

        private void SaveGeneration() {
            Pause();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "TetrisGeneration File (*.bin)|*.bin";
            string path = "";

            if (saveFileDialog.ShowDialog() == true) {
                path = saveFileDialog.FileName;

                SaveData saveData = new SaveData(this);

                Stream ws = new FileStream(path, FileMode.Create);
                BinaryFormatter serializer = new BinaryFormatter();

                serializer.Serialize(ws, saveData);

                ws.Close();
            }
        }

        private void LoadGeneration() {
            Pause();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TetrisGeneration File (*.bin)|*.bin";
            string path = "";

            if (openFileDialog.ShowDialog() == true) {
                path = openFileDialog.FileName;

                Stream rs = new FileStream(path, FileMode.Open);
                BinaryFormatter deserializer = new BinaryFormatter();

                SaveData saveData = (SaveData)deserializer.Deserialize(rs);

                rs.Close();

                NextInterval = saveData.nextInterval;
                TAManager = saveData.tetrisAIManager;
                PlaceCount = saveData.placeCount;
                PreviousBestGene = saveData.previousBestGene;

                Slider.Value = 500 - NextInterval.Milliseconds;

                UpdateGenerationInfo();
                UpdateTetrisImages();
            }
        }

        private void Canvas_Initialized(object sender, EventArgs e) {
            Canvas canvas = (Canvas)sender;
           
            for (int i = 0; i < 25; i++) {
                TetrisImages[i].Width = 50;
                TetrisImages[i].Height = 100;

                TetrisImages[i].Stretch = Stretch.None;

                Canvas.SetLeft(TetrisImages[i], (i % 5) * 55);
                Canvas.SetTop(TetrisImages[i], (i / 5) * 105);

                canvas.Children.Add(TetrisImages[i]);
            }

            UpdateTetrisImages();
        }

        private void Slider_Initialized(object sender, EventArgs e) {
            Slider = (Slider)sender;
        }

        private void Start() {
            StartAndPauseButton.Content = "Pause";

            PlaceTimer.Start();
        }

        private void Pause() {
            StartAndPauseButton.Content = "Start";

            PlaceTimer.Stop();
        }

        private void Button_Initialized(object sender, EventArgs e) {
            StartAndPauseButton = (Button)sender;
        }

        private void StartAndPauseButton_Click(object sender, RoutedEventArgs e) {
            if (PlaceTimer.IsEnabled) {
                Pause();
            } else {
                Start();
            }
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

            PreviousBestGeneLabel = new Label();
            PreviousBestGeneLabel.Foreground = Brushes.White;
            PreviousBestGeneLabel.FontFamily = new FontFamily("Consolas");
            PreviousBestGeneLabel.Margin = new Thickness(10, 5, 0, 0);

            grid.Children.Add(PreviousBestGeneLabel);


            for (int i = 0; i < 25; i++) {
                Label geneLabel = new Label();

                geneLabel.Foreground = Brushes.White;
                geneLabel.FontFamily = new FontFamily("Consolas");

                geneLabel.Margin = new Thickness(10, i * 15 + 40, 0, 0);

                GeneLabels[i] = geneLabel;
                grid.Children.Add(geneLabel);
            }

            UpdateGenerationInfo();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            SaveGeneration();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e) {
            LoadGeneration();
        }
    }
}
