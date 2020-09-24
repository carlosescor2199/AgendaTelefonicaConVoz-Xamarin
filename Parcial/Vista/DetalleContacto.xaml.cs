using Parcial.Modelo;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parcial.Vista;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Security.Principal;
using Parcial.Dependency;

namespace Parcial.Vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetalleContacto : ContentPage
    {
        public static Label id;
        public static Entry nombre;
        public static Entry apellido;
        public static Entry telefono;
        public static Entry email;
        
        public DetalleContacto()
        {
            InitializeComponent();
            BtnCrear.IsVisible = true;
            BtnActualizar.IsVisible = false;
            BtnEliminar.IsVisible = false;
            estadoVentanaLbl.Text = "Nuevo contacto";
            Detalle.ConvertirTextoAVoz("Accediendo a crear nuevo contacto");
        }

        //METODO QUE MUESTRA LOS DATOS DEL CONTACTO
        public DetalleContacto(int id)
        {
            InitializeComponent();
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Contacto.db3");
            var db = new SQLiteConnection(dbpath);
            var myquery = db.Table<DBContacto>()
                .Where(
                    consult => consult.Id == id
                ).FirstOrDefault();

            if (myquery!=null)
            {                
                NombresTxt.Text = myquery.Nombre;
                ApellidosTxt.Text = myquery.Apellido;
                TelefonoTxt.Text = myquery.Telefono;
                EmailTxt.Text = myquery.Email;
                BtnCrear.IsVisible = false;
                BtnActualizar.IsVisible = true;
                BtnEliminar.IsVisible = true;
                estadoVentanaLbl.Text = "Detalles del contacto";
                Detalle.ConvertirTextoAVoz("Accediendo a detalle contacto");
            }
        }

        //METODO PARA INSERTAR UN NUEVO CONTACTO
        private async void BtnCrear_Clicked(object sender, EventArgs e)
        {            
            if (String.IsNullOrEmpty(NombresTxt.Text) || String.IsNullOrEmpty(ApellidosTxt.Text) || String.IsNullOrEmpty(TelefonoTxt.Text) || String.IsNullOrEmpty(EmailTxt.Text))
            {
                var servicioM = DependencyService.Get<InterfazMensaje>().GetMensaje("Hay varios campos vacios");
                Detalle.ConvertirTextoAVoz(servicioM);               
                await DisplayAlert("Mensaje Dependency",servicioM, "Ok");
            }
            else
            {                
                var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Contacto.db3");
                var db = new SQLiteConnection(dbpath);
                db.CreateTable<DBContacto>();
                var contacto = new DBContacto()
                {
                    Nombre = NombresTxt.Text,
                    Apellido = ApellidosTxt.Text,
                    Telefono = TelefonoTxt.Text,
                    Email = EmailTxt.Text,
                };
                var res = db.Insert(contacto);
                if (res > 0)
                {
                    var servicioM = DependencyService.Get<InterfazMensaje>().GetMensaje("Contacto guardado");
                    Detalle.ConvertirTextoAVoz(servicioM);                    
                    await DisplayAlert("Mensaje Dependency", servicioM, "Ok");
                    await Navigation.PopAsync();
                }
                else
                {
                    var servicioM = DependencyService.Get<InterfazMensaje>().GetMensaje("Error al guardar");
                    Detalle.ConvertirTextoAVoz(servicioM);                    
                    await DisplayAlert("Mensaje Dependency", servicioM, "Ok");
                    await Navigation.PopAsync();
                }

                Detalle.listaContacto.ItemsSource = Detalle.LlenarLista();
            }
            
        }

        //METODO PARA ELIMINAR UN CONTACTO
        private async void BtnEliminar_Clicked(object sender, EventArgs e)
        {
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Contacto.db3");
            var db = new SQLiteConnection(dbpath);
            var myquery = db.Table<DBContacto>().Where(consult => consult.Id == Detalle.llave).FirstOrDefault();

            Detalle.ConvertirTextoAVoz("Seguro que quieres eliminar");
            bool opc = await DisplayAlert("Mensaje", "¿Seguro que quieres eliminar?", "Si", "Cancelar");
            

            if (opc==true)
            {
                db.Query<DBContacto>("DELETE FROM DBContacto WHERE Id = ?", myquery.Id);
                var servicioM = DependencyService.Get<InterfazMensaje>().GetMensaje("El contacto se ha eliminado");
                Detalle.ConvertirTextoAVoz(servicioM);                
                await DisplayAlert("Mensaje Dependency", servicioM, "Ok");
                await Navigation.PopAsync();
            }
            else
            {                
                await Navigation.PopAsync();
            }

           Detalle.listaContacto.ItemsSource = Detalle.LlenarLista();
            
        }

        //METODO PARA ACTUALIZAR UN CONTACTO
        private async void BtnActualizar_Clicked(object sender, EventArgs e)
        {
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Contacto.db3");
            var db = new SQLiteConnection(dbpath);
            var myquery = db.Table<DBContacto>().Where(consult => consult.Id == Detalle.llave).FirstOrDefault();

            bool opc = await DisplayAlert("Mensaje", "¿Seguro que quieres editar?", "Si", "Cancelar");

            if (opc == true)
            {
                string nombre = NombresTxt.Text;
                string apellido = ApellidosTxt.Text;
                string telefono = TelefonoTxt.Text;
                string mail = EmailTxt.Text;
                db.Query<DBContacto>("UPDATE DBContacto SET Nombre=?, Apellido=?, Telefono=?, Email=?   WHERE Id = ?", nombre, apellido, telefono, mail, myquery.Id);
                var servicioM = DependencyService.Get<InterfazMensaje>().GetMensaje("El contacto ha sido actualizado");
                Detalle.ConvertirTextoAVoz(servicioM);
                await DisplayAlert("Mensaje", servicioM, "Ok");                
                await Navigation.PopAsync();                
            }
            else
            {                
                await Navigation.PopAsync();                
            }
            Detalle.listaContacto.ItemsSource = Detalle.LlenarLista();
        }

        
    }
}