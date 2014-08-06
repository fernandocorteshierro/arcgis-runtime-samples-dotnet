﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates how to calculate the area and perimiter of a polygon using the GeometryEngine.
    /// </summary>
    /// <title>Area</title>
    /// <category>Geometry</category>
    public sealed partial class AreaSample : Page
    {
        private const double toMilesConversion = 0.0006213700922;
        private const double toSqMilesConversion = 0.0000003861003;

        private GraphicsLayer graphicsLayer;

        public AreaSample()
        {
            InitializeComponent();

            graphicsLayer = MyMapView.Map.Layers["MyGraphicsLayer"] as GraphicsLayer;
			MyMapView.ExtentChanged += MyMapView_ExtentChanged; 
        }

		private async void MyMapView_ExtentChanged(object sender, System.EventArgs e)
		{
			MyMapView.ExtentChanged -= MyMapView_ExtentChanged;
			await DoCalculateAreaAndLengthAsync();
		}

        private async Task DoCalculateAreaAndLengthAsync()
        {
            try
            {
                //Wait for user to draw
                var geom = await MyMapView.Editor.RequestShapeAsync(DrawShape.Polygon);

                //show geometry on map
                graphicsLayer.Graphics.Clear();

                var g = new Graphic { Geometry = geom, Symbol = LayoutRoot.Resources["DefaultFillSymbol"] as Esri.ArcGISRuntime.Symbology.Symbol };
                graphicsLayer.Graphics.Add(g);

                //Calculate results
                var areaPlanar = GeometryEngine.Area(geom);
                ResultsAreaPlanar.Text = string.Format("{0} sq. miles", (areaPlanar * toSqMilesConversion).ToString("n3"));

                var perimPlanar = GeometryEngine.Length(geom);
                ResultsPerimeterPlanar.Text = string.Format("{0} miles", (perimPlanar * toMilesConversion).ToString("n3"));

                var areaGeodesic = GeometryEngine.GeodesicArea(geom);
                ResultsAreaGeodesic.Text = string.Format("{0} sq. miles", (areaGeodesic * toSqMilesConversion).ToString("n3"));

                var perimGeodesic = GeometryEngine.GeodesicLength(geom);
                ResultsPerimeterGeodesic.Text = string.Format("{0} miles", (perimGeodesic * toMilesConversion).ToString("n3"));

                Instructions.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Results.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                var dlg = new MessageDialog("Current sketch has been canceled.", "Task Canceled!");
                var _ = dlg.ShowAsync();
            }
        }

        private void ResetUI()
        {
            graphicsLayer.Graphics.Clear();
            Instructions.Visibility = Visibility.Visible;
            Results.Visibility = Visibility.Collapsed;
        }

        private async void CancelCurrent_Click(object sender, RoutedEventArgs e)
        {
            MyMapView.Editor.Cancel.Execute(null);
            ResetUI();
			await DoCalculateAreaAndLengthAsync();
        }

        private async void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
			await DoCalculateAreaAndLengthAsync();
        }
    }
}
