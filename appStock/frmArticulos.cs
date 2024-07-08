using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace appStock
{

    /*Deberá ser una aplicación de escritorio que contemple la administración de artículos.
     * Las funcionalidades que deberá tener la aplicación serán:

    Listado de artículos. ||
    Búsqueda de artículos por distintos criterios. ||
    Agregar artículos. ||
    Modificar artículos.||
    Eliminar artículos.||
    Ver detalle de un artículo.
    Toda ésta información deberá ser persistida en una base de datos ya existente (la cual se adjunta).

    Los datos mínimos con los que deberá contar el artículo son los siguientes:

    Código de artículo.||
    Nombre.||
    Descripción.||
    Marca (seleccionable de una lista desplegable).
    Categoría (seleccionable de una lista desplegable.
    Imagen.||
    Precio.||  */
    public partial class frmArticulos : Form
    {
        private List<Articulo> lista;
        public frmArticulos()
        {
            InitializeComponent();
            cargar();
            cboxCampo.Items.Add("Precio");
            cboxCampo.Items.Add("Nombre");
            cboxCampo.Items.Add("Marca");
            cboxCampo.Items.Add("Categoria");

        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.ImagenUrl);
                detalleDesactivado();
                
            }

        }

        private void detalleDesactivado()
        {
            lblNombre.Visible = false;
            lblPrecio.Visible = false;
            lblCategoria.Visible = false;
            lblDescripcion.Visible = false;
            lblId.Visible = false;
            lblMarca.Visible = false;
            linkLblDetalle.Visible = true;
            lblN.Visible = false;
            lblP.Visible = false;
            lblC.Visible = false;
            lblD.Visible = false;
            lblI.Visible = false;
            lblM.Visible = false;
            panel.Visible = false;
        }
        private void ocultarColumnas()
        {
            dgvArticulos.Columns["ImagenUrl"].Visible = false;
            dgvArticulos.Columns["Id"].Visible = false;
            btnActivar.Visible = false;
            btnInactivar.Visible = false;
            
        }

         private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                lista = negocio.listar();
                dgvArticulos.DataSource = lista;
                ocultarColumnas();
                pbxArticulo.Load(lista[0].ImagenUrl);
                
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
                pbxArticulo.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxArticulo.Load("https://png.pngtree.com/png-vector/20210604/ourmid/pngtree-gray-network-placeholder-png-image_3416659.jpg");
            }
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> listaFiltrada;
            //No inicializo el obj lista antes ya que la Lista se genera en el FindAll

            string filtro = txtFiltro.Text;
            if (filtro.Length >= 2)
                listaFiltrada = lista.FindAll(art => art.Nombre.ToUpper().Contains(filtro.ToUpper()) || art.Marca.Descripcion.ToUpper().Contains(filtro.ToUpper()) || art.Categoria.Descripcion.ToUpper().Contains(filtro.ToUpper()) || art.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            else
                listaFiltrada = lista;

            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = listaFiltrada;

            //validarCeldaSelec();
            ocultarColumnas();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaArticulo alta = new frmAltaArticulo();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

            frmAltaArticulo modificar = new frmAltaArticulo(seleccionado);
            modificar.ShowDialog();
            cargar();
        }
        /* no segui con el activar-desactivar ya que no era necesario para la entrega*/
        private void btnInactivar_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            eliminar();
        }

        private void eliminar(bool logico = false)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;
            try
            {
                DialogResult respuesta = MessageBox.Show("¿De verdad queres Eliminar?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

                    if (logico)
                        negocio.eliminarLogico(seleccionado.Id);
                    else
                        negocio.eliminar(seleccionado.Id);

                    cargar();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

        private bool validarFiltro()
        {
            if (cboxCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar.");
                return true;
            }
            if (cboxCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el criterio para filtrar.");
                return true;
            }
            if (cboxCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar el filtro para númericos");
                    return true;
                }
                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Solo números por favor para filtrar por un campo númerico...");
                    return true;
                }
            }

            return false;
        }

        

        private void btnFiltroAvanzado_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (validarFiltro())
                    return;
                string campo = cboxCampo.SelectedItem.ToString();
                string criterio = cboxCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvArticulos.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void cboxCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string campo = cboxCampo.SelectedItem.ToString();
            if (campo == "Precio")
            {
                cboxCriterio.Items.Clear();
                cboxCriterio.Items.Add("Mayor a");
                cboxCriterio.Items.Add("Menor a");
                cboxCriterio.Items.Add("Igual a");

            }
            else
            {
                cboxCriterio.Items.Clear();
                cboxCriterio.Items.Add("Comienza con");
                cboxCriterio.Items.Add("Termina con");
                cboxCriterio.Items.Add("Contiene");
            }
        }

        private void cboxCriterio_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void detalleActivado()
        {
            lblNombre.Visible = true;
            lblPrecio.Visible = true;
            lblCategoria.Visible = true;
            lblDescripcion.Visible = true;
            lblId.Visible = true;
            lblMarca.Visible = true;
            lblN.Visible = true;
            lblP.Visible = true;
            lblC.Visible = true;
            lblD.Visible = true;
            lblI.Visible = true;
            lblM.Visible = true;
            linkLblDetalle.Visible = false;
            panel.Visible = true;
        }

        private void linkLblDetalle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
            lblNombre.Text = seleccionado.Nombre;
            lblPrecio.Text = "Precio: $" + seleccionado.Precio.ToString() + ".";
            lblCategoria.Text = seleccionado.Categoria.ToString();
            lblDescripcion.Text = seleccionado.Descripcion;
            lblId.Text=seleccionado.Id.ToString();
            lblMarca.Text = seleccionado.Marca.ToString();

            detalleActivado();

        }

        private void btnActivar_Click(object sender, EventArgs e)
        {

        }
    }
    }

