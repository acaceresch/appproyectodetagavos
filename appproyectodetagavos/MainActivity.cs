using System;
using System.Collections.Generic;

using System.Data;
using System.Data.SqlClient;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Widget;

//using System.IO;
using Java.IO;

using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Graphics.Drawables;


namespace AppProyectoDetagavos
{
    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }
    [Activity(Label = "AppProyectoDetagavos", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        string cultivo;
        private ImageView _imageView;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // make it available in the gallery
            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // display in ImageView. We will resize the bitmap to fit the display
            // Loading the full sized image will consume to much memory 
            // and cause the application to crash.
            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imageView.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it

            //llenar cultivos
            Spinner cultivos = FindViewById<Spinner>(Resource.Id.spinnercultivos);

            localhostdetagavos.detaservicio objservicio = new localhostdetagavos.detaservicio();
            string[] datos = objservicio.Llenarcultivos();

            ArrayAdapter<String> adaptador =
        new ArrayAdapter<String>(this,
            Android.Resource.Layout.SimpleSpinnerItem, datos);

            cultivos.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);

            adaptador.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            cultivos.Adapter = adaptador;



            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button button = FindViewById<Button>(Resource.Id.myButton);
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                if (App.bitmap != null)
                {
                    _imageView.SetImageBitmap(App.bitmap);
                    


                    //_imageView.BuildDrawingCache();
                    //Bitmap bmp = _imageView.GetDrawingCache(true);

                    //ByteArrayOutputStream stream = new ByteArrayOutputStream();
                    //System.IO.MemoryStream str = new System.IO.MemoryStream();
                    //bmp.Compress(Bitmap.CompressFormat.Png, 100, str);
                    ////bmp.CompressAsync(Bitmap.CompressFormat.Png, 100, stream);
                    //byte[] byteArray = str.ToArray();
                    ////localhostdetagavos.detaservicio objserviciom = new localhostdetagavos.detaservicio();
                    //objservicio.insertarsintoma("prueba app", byteArray, "1");
                }
                button.Click += TakeAPicture;
            }
            Button buttont = FindViewById<Button>(Resource.Id.myButtonp);
            buttont.Click += buttont_Click;

            Button buttontp = FindViewById<Button>(Resource.Id.myButtonsalir);
            buttontp.Click += buttontp_Click;


        }

        void buttontp_Click(object sender, EventArgs e)
        {
            App.bitmap = null;
            cultivo = "";
        }

        void buttont_Click(object sender, EventArgs e)
        {
            //try
            //{
            _imageView.BuildDrawingCache();
            Bitmap bmp = _imageView.GetDrawingCache(false);

            //ByteArrayOutputStream stream = new ByteArrayOutputStream();
            System.IO.MemoryStream str = new System.IO.MemoryStream();
            //bmp.Compress(Bitmap.CompressFormat.Jpeg, 100, str);

            App.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, str);
            //bmp.CompressAsync(Bitmap.CompressFormat.Png, 100, stream);
            byte[] byteArray = str.ToArray();
            localhostdetagavos.detaservicio objserviciom = new localhostdetagavos.detaservicio();
            //objserviciom.insertarsintoma("prueba app nu", byteArray, "1");
            TextView tt = FindViewById<TextView>(Resource.Id.textView1);
            tt.Text = objserviciom.algoritmodecomparacion(byteArray, cultivo);

            //Button buttont = FindViewById<Button>(Resource.Id.myButtonp);

            //}
            //catch (Exception ex)
            //{ }
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;

            string toast = string.Format("El cultivo es {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
            cultivo = string.Format("{0}", spinner.GetItemAtPosition(e.Position));

        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "AppDetagavos");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            App._file = new File(App._dir, String.Format("detagavos_{0}.jpg", Guid.NewGuid()));

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
           
            StartActivityForResult(intent, 0);




            //string ruta= String.Format("{0}",Uri.FromFile(App._file));
            ////
            //SqlConnection con = new SqlConnection("Data Source=ANTHONY-PC;Initial Catalog=basedetagavos;User ID=anthonycontrol;Password=shania");
            //SqlCommand com = new SqlCommand("insert into sintoma(id,nombre,imagen,id_plaga,estado) values(6,'juan',@Pic,2,'ACTIVO')", con);

            ////string ruta = "file://storage/emulated/0/Pictures/AppDetagavos/detagavos_" + Guid.NewGuid() + ".jpg";
            ////Se inicailiza un flujo de archivo con la imagen seleccionada desde el disco.
            ////System.IO.FileStream streamm = new System.IO.FileStream(@"C:\Users\antho_000\Pictures\template.jpg", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //System.IO.FileStream streamm = new System.IO.FileStream(@ruta, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            ////Se inicializa un arreglo de Bytes del tamaño de la imagen
            //byte[] binData = new byte[streamm.Length];
            ////Se almacena en el arreglo de bytes la informacion que se obtiene del flujo de archivos(foto)
            ////Lee el bloque de bytes del flujo y escribe los datos en un búfer dado.
            //streamm.Read(binData, 0, Convert.ToInt32(streamm.Length));

            ////Se muetra la imagen obtenida desde el flujo de datos
            ////pictureBox1.Image = Image.FromStream(stream);

            //com.Parameters.AddWithValue("@Pic", binData);
            //con.Open();
            ////Ejecuta una instrucción SQL en la conexión y devuelve el número de filas afectadas.
            //com.ExecuteNonQuery();
            //con.Close();

            //
        }
    }
}

