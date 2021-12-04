using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reconocimientofacial
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            MemoryStream ms = new MemoryStream();
            pbfoto.Image.Save(ms, ImageFormat.Jpeg);
            byte[] aByte = ms.ToArray();

            Conexion conex = new Conexion();
            MySqlConnection conexionBD = conex.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand("insert into fotos (Nombre,Correo,Telefono,Foto) values ('"+txtnombre.Text+ "','" + txtcorreo.Text + "','" + txttelefono.Text + "', @Foto)",conexionBD);
                comando.Parameters.AddWithValue("Foto",aByte);
                comando.ExecuteNonQuery();
                MessageBox.Show("Imagen Guardada");
                pbfoto.Image = null;
                limpiar();
            }
            catch (MySqlException ex) {
                MessageBox.Show("Error al guardar "+ex.Message);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtid.Text);
            string sql = "Select * from fotos where Id='"+id+"'";

            Conexion conex = new Conexion();
            MySqlConnection conexionBD = conex.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql,conexionBD);
                MySqlDataReader reader = comando.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    MemoryStream ms = new MemoryStream((byte[])reader["Foto"]);
                    Bitmap bm = new Bitmap(ms);
                    pbfoto.Image = bm;
                    txtnombre.Text = reader["Nombre"].ToString();
                    txtcorreo.Text = reader["Correo"].ToString();
                    txttelefono.Text = reader["Telefono"].ToString();
                }
                else { MessageBox.Show("No se encontraron registros."); }
               
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al buscar " + ex.Message);
            }

        }

        private void btncargar_Click(object sender, EventArgs e)
        {
            OpenFileDialog opdcargar = new OpenFileDialog();
            opdcargar.Filter = "Imagenes|*.jpg; *.png";
            opdcargar.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            opdcargar.Title = "Seleccionar Imagen";

            if (opdcargar.ShowDialog() == DialogResult.OK)
            {
                pbfoto.Image = Image.FromFile(opdcargar.FileName);
            }
        }

        private void limpiar() {
            txtnombre.Text = "";
            txtcorreo.Text = "";
            txttelefono.Text = "";
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("Seguro que desea eliminar este usuario?","Salir",MessageBoxButtons.YesNoCancel);

            if(resultado == DialogResult.Yes)
            {
                int id = int.Parse(txtid.Text);
                string sql = "delete from fotos where Id='" + id + "'";

                Conexion conex = new Conexion();
                MySqlConnection conexionBD = conex.conexion();
                conexionBD.Open();

                try
                {
                    MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                    comando.ExecuteNonQuery();
                    txtid.Text = "";
                    limpiar();
                    pbfoto.Image = null;
                    MessageBox.Show("El registro se elimino con exito.");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error al eliminar " + ex.Message);
                }

            }
        }
    }
}
