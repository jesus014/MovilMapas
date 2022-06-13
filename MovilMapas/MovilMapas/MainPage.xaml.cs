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
using Mapsui.UI;
using Plugin.Geolocator;
using Xamarin.Essentials;

namespace MovilMapas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        List<Lugar> lugares = Models.ServicioBD.ObtenerLugares();
        public Func<MapView, MapClickedEventArgs, bool> Clicker { get; set; }
        double latitud;
        double longitud;


        public MainPage(Func<MapView, MapClickedEventArgs, bool> c = null)
        {
            InitializeComponent();
            #region antes
            /*
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
            
            //mapControl.Map = CreateMap();
            var layer = GenerateIconLayer();
            layer.IsMapInfoLayer = true;
            mapControl.Map.Layers.Add(layer);
            
            //ContentGrid.Children.Add(view);
            //mapControl.Pins.Add(pin);
            Pin pin = new Pin();
            pin.Position=new Position(gdl.Latitud,gdl.Longitud);
            pin.Type = PinType.Pin;
            pin.Label = gdl.Nombre;
            pin.Address = gdl.Nombre;
            MapView mw = new MapView();
            mw.Pins.Add(pin);
            //mw.Map = mapControl;
            //ContentGrid.Children.Add(mapControl);
            */
            #endregion
            Localizar();

            var map = new Mapsui.Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
            var tileLayer = OpenStreetMap.CreateTileLayer();
            map.Layers.Add(tileLayer);
            mapView.Map = map;
            mapView.RotationLock = false;
            mapView.UnSnapRotationDegrees = 30;
            mapView.ReSnapRotationDegrees = 5;




            var ciudad = lugares.First();
            //mapView.MyLocationLayer.UpdateMyLocation(new Mapsui.UI.Forms.Position(ciudad.Latitud, ciudad.Longitud));
            var center = SphericalMercator.FromLonLat(-99.6675493, 20.3364713);//este centro deberia la posición GPS
            var resolution = 10;
            mapView.Map.Home = n => n.NavigateTo(center, resolution);//Actualizar cada que cambie la posición del gps

            mapView.PinClicked += OnPinClicked;

            foreach(var item in lugares)
            {
                mapView.Pins.Add(new Pin()
                {
                    Position = new Position(item.Latitud,item.Longitud),
                    Type = PinType.Pin,
                    Label = item.Nombre,
                    Address = item.Nombre
                });

            }



        }

        public  Lugar Localizar()
        {
            Lugar lugar = new Lugar();
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            if (locator.IsGeolocationAvailable)
            {
                if (locator.IsGeolocationEnabled)
                {
                    if (!locator.IsListening)
                    {
                         locator.StartListeningAsync(TimeSpan.FromSeconds(1), 5);
                    }
                    locator.PositionChanged += (cambio, args) =>
                    {
                        var loc = args.Position;
                        latitud = loc.Latitude;
                        longitud=loc.Longitude;
                        Console.WriteLine(loc);
                        Console.WriteLine(longitud);
                        Console.WriteLine(latitud);
                    };
                }
            }
             lugar.Longitud = longitud;
            lugar.Latitud = latitud;
            return lugar;
        }

        public async void OnPinClicked(object sender, PinClickedEventArgs e)
        {
            if (e.Pin != null)
            {
                if (e.NumOfTaps == 1)
                {
                    // Hide Pin when double click
                    await DisplayAlert($"Pin {e.Pin.Label}", $"está en la posición {e.Pin.Position}", "Ok");
                    //e.Pin.IsVisible = false;
                    Console.WriteLine("Estas haciendo click");
                }
            


            }

            e.Handled = true;
        }



        protected override void OnAppearing()
        {

            
        }

        //<mapsui:MapView x:Name= "selectMapControl"

        //        VerticalOptions= "FillAndExpand"

        //        HorizontalOptions= "Fill"

        //        BackgroundColor= "Gray" />

        private ILayer GenerateIconLayer()
        {
            var layername = "Capa Local";

            return new Layer(layername)
            {
                Name = layername,
                DataSource = new MemoryProvider(GetIconFeatures()),
                Style = new SymbolStyle
                {
                    SymbolScale = 0.8,
                    Fill = new Mapsui.Styles.Brush(Color.Green),
                    Outline = { Color = Color.Black, Width = 1 }
                }
            };
        }


        private Features GetIconFeatures()
        {
            var features = new Features();
            var map = new MapView();
            var ctz = lugares.First(x => x.Nombre == "Cortazar");
            var vil = lugares.First(x => x.Nombre == "Villagran");
            var val = lugares.First(x => x.Nombre == "Valle de Santiago");
            var pin = new Pin(map)
            {
                Label = $"PinType.Pin ",
                Address = "assas",
                Position = new Position(-103.344, 20.6736),
                Type = PinType.Pin,


            };
            var feature_Gto = new Feature
            {
                Geometry = new Mapsui.Geometries.Polygon(new LinearRing(new[]
                {
                    SphericalMercator.FromLonLat(ctz.Longitud, ctz.Latitud),
                    SphericalMercator.FromLonLat(vil.Longitud, vil.Latitud),
                    SphericalMercator.FromLonLat(val.Longitud, val.Latitud),
                    SphericalMercator.FromLonLat(ctz.Longitud, ctz.Latitud),
                })),
                ["Label"] = "Guanajuato",
                ["Type"] = "Estado"
            };

            var usa = lugares.Where(x => x.Nombre.Contains("Estados Unidos"));
            var Points = new List<Point>();

            foreach (var point in usa)
                Points.Add(SphericalMercator.FromLonLat(point.Longitud, point.Latitud));

            var feature_USA = new Feature
            {
                Geometry = new Mapsui.Geometries.Polygon(new LinearRing(Points)),
                ["Label"] = "USA",
                ["Type"] = "Pais"
            };

            features.Add(feature_Gto);
            features.Add(feature_USA);
            map.Pins.Add(pin);
            return features;
        }


        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if (!await Launcher.TryOpenAsync("fb://facewebmodal/f?href=https://www.facebook.com/jesus.santiagoguarneros.7"))
            {
                await Browser.OpenAsync("https://twitter.com/home");
            }
        }

    }
    }

