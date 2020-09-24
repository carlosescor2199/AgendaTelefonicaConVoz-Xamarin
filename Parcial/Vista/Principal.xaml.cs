using Parcial.Modelo;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;



namespace Parcial.Vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Detalle : ContentPage
    {
        private static ObservableCollection<DBContacto> tablaRegistros;
        public static ListView listaContacto;
        public static int llave;
        

        public Detalle()
        {
            InitializeComponent();
            try
            {
                ContactosList.ItemsSource = LlenarLista();
                listaContacto = ContactosList;
            }catch(Exception e)
            {

            }
            finally
            {
                ConvertirTextoAVoz("Bienvenido a la aplicacion de prueba");
            }            

        }

        //METODO PARA CONSULTAR TODOS LOS CONTACTOS Y LLENAR LA LISTA
        public static ObservableCollection<DBContacto> LlenarLista()
        {
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Contacto.db3");
            var db = new SQLiteConnection(dbpath);
            var res = db.Table<DBContacto>().ToList();
            var sorted = res.OrderBy(x => x.Nombre).ToList();
            return tablaRegistros = new ObservableCollection<DBContacto>(sorted);  
            
        }

        //METODO PARA SELECCIONAR UN CONTACTO DE LA LISTA Y ACCEDER A LA VISTA SECUNDARIA
        private void ContactosList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var obj = (DBContacto)e.SelectedItem;
            int item = obj.Id;
            llave = item;
            ConvertirTextoAVoz(obj.Nombre);            
            Navigation.PushAsync(new DetalleContacto(item));
            
        }

        //METODO PARA HACER UNA BUSQUEDA FILTRADA
        private void BuscarTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(BuscarTxt.Text))
            {
                listaContacto.ItemsSource = tablaRegistros;
            }
            var key = BuscarTxt.Text;
            listaContacto.ItemsSource = tablaRegistros.Where(x => x.Nombre.ToLower().Contains(key.ToLower()));

            if (!String.IsNullOrEmpty(key))
            {
                ConvertirTextoAVoz(key);
            }
        }

        //METODO PARA CONVERTIR A VOZ CUALQUIER TEXTO
        public static async void ConvertirTextoAVoz(string text)
        {          
            
            await TextToSpeech.SpeakAsync(text, new SpeechOptions
            {
                Volume = 1.0f                
            });
        }
    }
}