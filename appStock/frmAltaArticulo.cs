using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using dominio;
using negocio;
using System.Configuration;
using System.IO;

namespace appStock
{
    public partial class frmAltaArticulo : Form
    {

        private Articulo articulo = null;
        private OpenFileDialog archivo = null;
        public frmAltaArticulo()
        {
            InitializeComponent();
        }

        public frmAltaArticulo(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modificar articulo";
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                //if ((char.IsLetter(caracter)) || (char.IsSymbol(caracter)) || (char.IsSeparator(caracter)) || (char.IsWhiteSpace(caracter)) || (char.IsControl(caracter)))
                   if(!(char.IsDigit(caracter)))
                    return false;
            }
            return true;
        }

        private bool validarCampos()
        {


            if (string.IsNullOrEmpty(txtboxNombre.Text) || string.IsNullOrEmpty(txtboxCodigo.Text) || string.IsNullOrEmpty(txtboxDescripcion.Text))
            {
                MessageBox.Show("Debes completar todos los campos");
                return true;
            }

            return false;
        }

            



        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //Pokemon poke = new Pokemon();
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (articulo == null)
                    articulo = new Articulo();

                articulo.Codigo = txtboxCodigo.Text;
                articulo.Nombre = txtboxNombre.Text;
                articulo.Descripcion = txtboxDescripcion.Text;
                articulo.ImagenUrl = txtBoxImagen.Text;
                if (!(soloNumeros(txtboxPrecio.Text)))
                {
                    MessageBox.Show("Solo números por favor para indicar el precio...");
                    return;
                }
                else if (string.IsNullOrEmpty(txtboxPrecio.Text)){
                    MessageBox.Show("Debe completar el campo de precio...");
                    return;
                }
                articulo.Precio = decimal.Parse(txtboxPrecio.Text);

                articulo.Marca = (Marca)cboMarca.SelectedItem;
                articulo.Categoria = (Categoria)cboCategoria.SelectedItem;

                if (validarCampos())
                    return;

                if (articulo.Id != 0)
                {   
                    negocio.modificar(articulo);
                    MessageBox.Show("Modificado exitosamente");

                }
                else
                {
                    negocio.agregar(articulo);
                    MessageBox.Show("Agregado exitosamente");
                }

                if (archivo != null && (txtBoxImagen.Text.ToUpper().Contains("HTTP")))
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images=folder"] + archivo.SafeFileName);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void frmAltaArticulo_Load(object sender, EventArgs e)
        {
             MarcaNegocio marcanegocio = new MarcaNegocio();
             CategoriaNegocio categorianegocio = new CategoriaNegocio();
             try
             {
                cboCategoria.DataSource = categorianegocio.listar();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";
                cboMarca.DataSource = marcanegocio.listar();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";


                if (articulo != null)
                 {
                     txtboxCodigo.Text = articulo.Codigo;
                     txtboxNombre.Text = articulo.Nombre;
                     txtboxDescripcion.Text = articulo.Descripcion;
                     txtBoxImagen.Text = articulo.ImagenUrl;
                     cargarImagen(articulo.ImagenUrl);
                     txtboxPrecio.Text = articulo.Precio.ToString();
                     
                     cboMarca.SelectedValue = articulo.Marca.Id;
                     cboCategoria.SelectedValue = articulo.Categoria.Id;
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.ToString());
             }
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pboxAlta.Load(imagen);
            }
            catch (Exception)
            {
                pboxAlta.Load("https://png.pngtree.com/png-vector/20210604/ourmid/pngtree-gray-network-placeholder-png-image_3416659.jpg");
            }
        }

        private void txtBoxImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtBoxImagen.Text);
        }

        private void btnImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";
            if (archivo.ShowDialog() == DialogResult.OK)
            {
                txtBoxImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);

                // guardo la img
                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images=folder"] + archivo.SafeFileName);
            }
        }
    }
}
