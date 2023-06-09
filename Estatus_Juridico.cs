﻿using Microsoft.Office.Interop.Excel;
using Modulos.Clases;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modulos
{
    public partial class Estatus_Juridico : Form
    {
        EstatusJuridico_Controller est = new EstatusJuridico_Controller();
        List<(int, string)> list = new List<(int, string)> ();
        Conexion con;
        public Estatus_Juridico()
        {
            InitializeComponent();
            list = est.LawyerList(); 
            for(int i = 0; i < list.Count; i++)
            {
                comboLawyer.Items.Add (list[i].Item2);
            }
            con = new Conexion();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            buscar();
        }

        public void buscar()
        {
            txtTotalDemanda.Text = 0.ToString("C");
            int lawindex = comboLawyer.SelectedIndex + 1;
            EstatusJuridico_Controller ejc = new EstatusJuridico_Controller();
            adgvJuridico.Columns.Clear();
            adgvJuridico.DataSource = ejc.procesList(lawindex);
            txtTotalDemanda.Text = StorageClass.totalDemandas.ToString("C");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcesoJuridico pr = new ProcesoJuridico(1);
            pr.NoContrato.Text = adgvJuridico.CurrentRow.Cells["No. Credito"].Value.ToString();
            string fecha = adgvJuridico.CurrentRow.Cells["Fecha oficio"].Value.ToString();
            DateTime auxDate = DateTime.Parse(fecha);
            pr.fechaOficio.Value = auxDate;
            pr.totalDemanda.Text = adgvJuridico.CurrentRow.Cells["Moratorio"].Value.ToString();
            pr.listAbogado.Enabled = false;
            string status = adgvJuridico.CurrentRow.Cells["Estatus"].Value.ToString();
            string obs = adgvJuridico.CurrentRow.Cells["Observaciones"].Value.ToString();
            pr.listStatus.SelectedItem = status;
            pr.txtObsBox.Text = obs;
            pr.Visible = true;
            
        }

        private void adgvJuridico_FilterStringChanged(object sender, EventArgs e)
        {
            ((DataView)adgvJuridico.DataSource).RowFilter = adgvJuridico.FilterString;
        }


        private void adgvJuridico_SortStringChanged_1(object sender, EventArgs e)
        {
            ((DataView)adgvJuridico.DataSource).Sort = adgvJuridico.SortString;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            String credito = adgvJuridico.CurrentRow.Cells["CREDITO"].Value.ToString();

            var confirmResult = MessageBox.Show("¿Eliminar proceso juridico para el prestamo "+credito+"?",
                                     "¿Eliminar?",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                String query = "UPDATE estatus_juridico SET Activo = 0 WHERE IdPrestamo = " + credito + " AND Activo = 1;"; ;

                try
                {
                    con.crearConexion();
                    con.OpenConnection();

                    MySqlCommand command = new MySqlCommand(query, con.GetConnection());
                    command.CommandTimeout = 100000;

                    command.ExecuteNonQuery();

                    buscar();

                }
                catch (Exception error)
                {
                    Console.WriteLine("Error cancelando el deposito: " + error.Message);
                }
                finally
                {
                    con.CloseConnection();
                }
            }
                
        }

        private void adgvJuridico_EnabledChanged(object sender, EventArgs e)
        {
            
        }

        private void adgvJuridico_SortStringChanged(object sender, EventArgs e)
        {
            ((DataView)adgvJuridico.DataSource).Sort = adgvJuridico.SortString;
        }

        private void adgvJuridico_FilterStringChanged_1(object sender, EventArgs e)
        {
            ((DataView)adgvJuridico.DataSource).RowFilter = adgvJuridico.FilterString;
        }
    }
}
