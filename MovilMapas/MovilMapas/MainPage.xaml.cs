using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;
using MovilMapas.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Mapsui.UI.Forms;
using Color = Mapsui.Styles.Color;
using Point = Mapsui.Geometries.Point;

namespace MovilMapas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        List<Lugar> lugares = Models.ServicioBD.ObtenerLugares();

        public MainPage()
        {
            InitializeComponent();
            var mapControl = new Mapsui.UI.Forms.MapControl();
            mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

            mapControl.Map.Widgets.Add(new ScaleBarWidget(mapControl.Map)

            {
                TextAlignment = Alignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            });
            mapControl.Map.Widgets.Add(new ZoomInOutWidget()
            {
                MarginX = 20,
                MarginY = 40
            });
            var gdl = lugares.First(x => x.Nombre == "Guadalajara");
            
            var coordenada = new Point(gdl.Longitud, gdl.Latitud);
            var coordenadaMercator = SphericalMercator.FromLonLat(coordenada.X, coordenada.Y);
            mapControl.Map.Home = n => n.NavigateTo(coordenadaMercator, mapControl.Map.Resolutions[9]);
            //mapControl.Map = CreateMap();
            var layer = GenerateIconLayer();
            layer.IsMapInfoLayer = true;
            mapControl.Map.Layers.Add(layer);
            
            var view = new MapView();

            var pin = new Pin(view)
            {
                Type = PinType.Pin,
                Label = "Some Pin",
                Position = new Position(-103.344,20.6736 ),
                Address = "Address...",
                Transparency =0.5f,
            };
            view.Pins.Add(pin);
            ContentGrid.Children.Add(view);
            //mapControl.Pins.Add(pin);

            ContentGrid.Children.Add(mapControl);
        }



        private ILayer GenerateIconLayer()
        {
            var layername = "Capa Local";

            return new Layer(layername)
            {
                Name = layername,
                //DataSource = new MemoryProvider(GetIconFeatures()),
                //Style = new SymbolStyle
                //{
                //    SymbolScale = 0.8,
                //    Fill = new Mapsui.Styles.Brush(Color.Green),
                //    Outline = { Color = Color.Black, Width = 5 }
                //}
            };
        }



    }
    }

