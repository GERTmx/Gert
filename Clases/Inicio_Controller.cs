﻿using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Modulos.Clases
{
    public class Inicio_Controller
    {
        public Inicio_Controller()
        {

        }

        public string CalcularRFC(string nombre, string apellidoPaterno, string apellidoMaterno, string fecha)
        {
            //Cambiamos todo a mayúsculas
            nombre = nombre.ToUpper();
            apellidoPaterno = apellidoPaterno.ToUpper();
            apellidoMaterno = apellidoMaterno.ToUpper();

            //RFC que se regresará
            string rfc = string.Empty;

            //Quitamos los espacios al principio y final del nombre y apellidos
            nombre.Trim();
            apellidoPaterno = apellidoPaterno.Trim();
            apellidoMaterno = apellidoMaterno.Trim();

            //Quitamos los artículos de los apellidos
            apellidoPaterno = QuitarArticulos(apellidoPaterno);
            apellidoMaterno = QuitarArticulos(apellidoMaterno);

            //Agregamos el primer caracter del apellido paterno
            rfc = apellidoPaterno.Substring(0, 1);

            //Buscamos y agregamos al rfc la primera vocal del primer apellido
            foreach (char c in apellidoPaterno)
            {
                if (EsVocal(c))
                {
                    rfc += c;
                    break;
                }
            }

            //Agregamos el primer caracter del apellido materno
            rfc += apellidoMaterno.Substring(0, 1);

            //Agregamos el primer caracter del primer nombre
            rfc += nombre.Substring(0, 1);

            //agregamos la fecha yymmdd (por ejemplo: 680825, 25 de agosto de 1968 )
            rfc += fecha;

            //Le agregamos la homoclave al rfc 
            CalcularHomoclave(apellidoPaterno + " " + apellidoMaterno + " " + nombre, fecha, ref rfc);

            return rfc;
        }

        /// <summary>
        /// Calcula la homoclave
        /// </summary>
        /// <param name="nombreCompleto">El nombre completo de la persona en el formato "ApellidoPaterno ApellidoMaterno Nombre(s)"</param>
        /// <param name="fecha">fecha en el formato "dd/MM/yy"</param>
        /// <param name="rfc">rfc sin homoclave, esta se pasa con ref y después de la función tendrá la homoclave</param>
        static private void CalcularHomoclave(string nombreCompleto, string fecha, ref string rfc)
        {
            //Guardara el nombre en su correspondiente numérico
            StringBuilder nombreEnNumero = new StringBuilder(); ;
            //La suma de la secuencia de números de nombreEnNumero
            long valorSuma = 0;

            #region Tablas para calcular la homoclave
            //Estas tablas realmente no se porque son como son
            //solo las copie de lo que encontré en internet

            #region TablaRFC 1
            Hashtable tablaRFC1 = new Hashtable();
            tablaRFC1.Add("&", 10);
            tablaRFC1.Add("Ñ", 40);
            tablaRFC1.Add("A", 11);
            tablaRFC1.Add("B", 12);
            tablaRFC1.Add("C", 13);
            tablaRFC1.Add("D", 14);
            tablaRFC1.Add("E", 15);
            tablaRFC1.Add("F", 16);
            tablaRFC1.Add("G", 17);
            tablaRFC1.Add("H", 18);
            tablaRFC1.Add("I", 19);
            tablaRFC1.Add("J", 21);
            tablaRFC1.Add("K", 22);
            tablaRFC1.Add("L", 23);
            tablaRFC1.Add("M", 24);
            tablaRFC1.Add("N", 25);
            tablaRFC1.Add("O", 26);
            tablaRFC1.Add("P", 27);
            tablaRFC1.Add("Q", 28);
            tablaRFC1.Add("R", 29);
            tablaRFC1.Add("S", 32);
            tablaRFC1.Add("T", 33);
            tablaRFC1.Add("U", 34);
            tablaRFC1.Add("V", 35);
            tablaRFC1.Add("W", 36);
            tablaRFC1.Add("X", 37);
            tablaRFC1.Add("Y", 38);
            tablaRFC1.Add("Z", 39);
            tablaRFC1.Add("0", 0);
            tablaRFC1.Add("1", 1);
            tablaRFC1.Add("2", 2);
            tablaRFC1.Add("3", 3);
            tablaRFC1.Add("4", 4);
            tablaRFC1.Add("5", 5);
            tablaRFC1.Add("6", 6);
            tablaRFC1.Add("7", 7);
            tablaRFC1.Add("8", 8);
            tablaRFC1.Add("9", 9);
            #endregion

            #region TablaRFC 2
            Hashtable tablaRFC2 = new Hashtable();
            tablaRFC2.Add(0, "1");
            tablaRFC2.Add(1, "2");
            tablaRFC2.Add(2, "3");
            tablaRFC2.Add(3, "4");
            tablaRFC2.Add(4, "5");
            tablaRFC2.Add(5, "6");
            tablaRFC2.Add(6, "7");
            tablaRFC2.Add(7, "8");
            tablaRFC2.Add(8, "9");
            tablaRFC2.Add(9, "A");
            tablaRFC2.Add(10, "B");
            tablaRFC2.Add(11, "C");
            tablaRFC2.Add(12, "D");
            tablaRFC2.Add(13, "E");
            tablaRFC2.Add(14, "F");
            tablaRFC2.Add(15, "G");
            tablaRFC2.Add(16, "H");
            tablaRFC2.Add(17, "I");
            tablaRFC2.Add(18, "J");
            tablaRFC2.Add(19, "K");
            tablaRFC2.Add(20, "L");
            tablaRFC2.Add(21, "M");
            tablaRFC2.Add(22, "N");
            tablaRFC2.Add(23, "P");
            tablaRFC2.Add(24, "Q");
            tablaRFC2.Add(25, "R");
            tablaRFC2.Add(26, "S");
            tablaRFC2.Add(27, "T");
            tablaRFC2.Add(28, "U");
            tablaRFC2.Add(29, "V");
            tablaRFC2.Add(30, "W");
            tablaRFC2.Add(31, "X");
            tablaRFC2.Add(32, "Y");
            tablaRFC2.Add(33, "Z");
            #endregion

            #region TablaRFC 3
            Hashtable tablaRFC3 = new Hashtable();
            tablaRFC3.Add("A", 10);
            tablaRFC3.Add("B", 11);
            tablaRFC3.Add("C", 12);
            tablaRFC3.Add("D", 13);
            tablaRFC3.Add("E", 14);
            tablaRFC3.Add("F", 15);
            tablaRFC3.Add("G", 16);
            tablaRFC3.Add("H", 17);
            tablaRFC3.Add("I", 18);
            tablaRFC3.Add("J", 19);
            tablaRFC3.Add("K", 20);
            tablaRFC3.Add("L", 21);
            tablaRFC3.Add("M", 22);
            tablaRFC3.Add("N", 23);
            tablaRFC3.Add("&", 24);
            tablaRFC3.Add("O", 25);
            tablaRFC3.Add("P", 26);
            tablaRFC3.Add("Q", 27);
            tablaRFC3.Add("R", 28);
            tablaRFC3.Add("S", 29);
            tablaRFC3.Add("T", 30);
            tablaRFC3.Add("U", 31);
            tablaRFC3.Add("V", 32);
            tablaRFC3.Add("W", 33);
            tablaRFC3.Add("X", 34);
            tablaRFC3.Add("Y", 35);
            tablaRFC3.Add("Z", 36);
            tablaRFC3.Add("0", 0);
            tablaRFC3.Add("1", 1);
            tablaRFC3.Add("2", 2);
            tablaRFC3.Add("3", 3);
            tablaRFC3.Add("4", 4);
            tablaRFC3.Add("5", 5);
            tablaRFC3.Add("6", 6);
            tablaRFC3.Add("7", 7);
            tablaRFC3.Add("8", 8);
            tablaRFC3.Add("9", 9);
            tablaRFC3.Add(" ", 37);
            tablaRFC3.Add("Ñ", 38);
            #endregion

            #endregion

            //agregamos un cero al inicio de la representación númerica del nombre
            nombreEnNumero.Append("0");

            //Recorremos el nombre y vamos convirtiendo las letras en 
            //su valor numérico
            foreach (char c in nombreCompleto)
            {
                if (tablaRFC1.ContainsKey(c.ToString()))
                    nombreEnNumero.Append(tablaRFC1[c.ToString()].ToString());
                else
                    nombreEnNumero.Append("00");
            }

            //Calculamos la suma de la secuencia de números 
            //calculados anteriormente
            //la formula es:
            //( (el caracter actual multiplicado por diez)
            //mas el valor del caracter siguiente )
            //(y lo anterior multiplicado por el valor del caracter siguiente)
            for (int i = 0; i < nombreEnNumero.Length - 1; i++)
            {
                valorSuma += (Convert.ToInt32(nombreEnNumero[i].ToString()) * 10 + Convert.ToInt32(nombreEnNumero[i + 1].ToString())) * Convert.ToInt32(nombreEnNumero[i + 1].ToString());
            }

            //Lo siguiente no se porque se calcula así, es parte del algoritmo.
            //Los magic numbers que aparecen por ahí deben tener algún origen matemático
            //relacionado con el algoritmo al igual que el proceso mismo de calcular el 
            //digito verificador.
            //Por esto no puedo añadir comentarios a lo que sigue, lo hice por acto de fe.

            int div = 0, mod = 0;
            div = Convert.ToInt32(valorSuma) % 1000;
            mod = div % 34;
            div = (div - mod) / 34;

            int indice = 0;
            string hc = string.Empty;  //los dos primeros caracteres de la homoclave
            while (indice <= 1)
            {
                if (tablaRFC2.ContainsKey(indice == 0 ? div : mod))
                    hc += tablaRFC2[indice == 0 ? div : mod];
                else
                    hc += "Z";
                indice++;
            }

            //Agregamos al RFC los dos primeros caracteres de la homoclave
            rfc += hc;

            //Aqui empieza el calculo del digito verificador basado en lo que tenemos del RFC
            //En esta parte tampoco conozco el origen matemático del algoritmo como para dar 
            //una explicación del proceso, así que ¡tengamos fe hermanos!.
            int rfcAnumeroSuma = 0, sumaParcial = 0;
            for (int i = 0; i < rfc.Length; i++)
            {
                if (tablaRFC3.ContainsKey(rfc[i].ToString()))
                {
                    rfcAnumeroSuma = Convert.ToInt32(tablaRFC3[rfc[i].ToString()]);
                    sumaParcial += rfcAnumeroSuma * (14 - (i + 1));
                }
            }

            int moduloVerificador = sumaParcial % 11;
            if (moduloVerificador == 0)
                rfc += "0";
            else
            {
                sumaParcial = 11 - moduloVerificador;
                if (sumaParcial == 10)
                    rfc += "A";
                else
                    rfc += sumaParcial.ToString();
            }

            //en este punto la variable rfc pasada ya debe tener la homoclave
            //recuerda que la variable rfc se paso como "ref string" lo cual
            //hace que se modifique la original.
        }

        /// <summary>
        /// Verifica si el caracter pasado es una vocal
        /// </summary>
        /// <param name="letra">Caracter a comprobar</param>
        /// <returns>Regresa true si es vocal, de lo contrario false</returns>
        static private bool EsVocal(char letra)
        {
            //Aunque para el caso del RFC cambié todas las letras a mayúsculas
            //igual agregé las minúsculas.
            if (letra == 'A' || letra == 'E' || letra == 'I' || letra == 'O' || letra == 'U' ||
                letra == 'a' || letra == 'e' || letra == 'i' || letra == 'o' || letra == 'u')
                return true;
            else
                return false;
        }

        /// <summary>
        /// Remplaza los artículos comúnes en los apellidos en México con caracter vacío (String.Empty).
        /// </summary>
        /// <param name="palabra">Palabra que se le quitaran los artículos</param>
        /// <returns>Regresa la palabra sin los artículos</returns>
        static private string QuitarArticulos(string palabra)
        {
            return palabra.Replace("DEL ", string.Empty).Replace("LAS ", string.Empty).Replace("DE ", string.Empty).Replace("LA ", string.Empty).Replace("Y ", string.Empty).Replace("A ", string.Empty);
        }

        public void cargarExcelJuridico()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XLSX (*.xlsx)|*.xlsx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;
                var workbook = new XLWorkbook(fileName);
                var ws1 = workbook.Worksheet("DEMANDAS");
                IXLCell fechaOficio;

                String fechaOficioS;
                String contrato;
                String empresa;
                String moraS;
                double mora;
                String abogado;
                int iAbogado = 0;
                String estatus;
                String obs;

                int lastIndex = ws1.LastRowUsed().RowNumber();
                for (int i = 3; i <= lastIndex; i++)
                {

                    empresa = ws1.Cell(i, 3).Value.ToString();

                    if (empresa.Equals("GERT"))
                    {
                        contrato = ws1.Cell(i, 2).Value.ToString();
                        if (!contrato.Equals(""))
                        {
                            if (!ws1.Cell(i, 11).IsEmpty())
                            {
                                fechaOficio = ws1.Cell(i, 1);
                                fechaOficioS = $"{fechaOficio.GetDateTime().Year}-{fechaOficio.GetDateTime().Month.ToString("00")}-{fechaOficio.GetDateTime().Day.ToString("00")}";
                                mora = ws1.Cell(i, 11).GetDouble();
                                abogado = ws1.Cell(i, 14).Value.ToString();
                                estatus = ws1.Cell(i, 15).Value.ToString();
                                obs = ws1.Cell(i, 17).Value.ToString();

                                if (abogado.Contains("VILLELA"))
                                {
                                    iAbogado = 1;
                                }
                                else if (abogado.Contains("ENRIQUE"))
                                {
                                    iAbogado = 2;
                                }
                                else if (abogado.Contains("GALLEGOS"))
                                {
                                    iAbogado = 3;
                                }
                                else if (abogado.Contains("JOSE"))
                                {
                                    iAbogado = 4;
                                }
                                else if (abogado.Contains("JUAN"))
                                {
                                    iAbogado = 5;
                                }
                                else if (abogado.Contains("GRANADOS"))
                                {
                                    iAbogado = 6;
                                }
                                else if (abogado.Contains("NOE"))
                                {
                                    iAbogado = 7;
                                }

                                insertJuridico(contrato, fechaOficioS, mora.ToString(), iAbogado, estatus, obs);
                            }
                        }
                    }
                }
            }
        }

        public void insertJuridico(String NoContrato, String fechaOficio, String mora, int abogado, String status, String obs)
        {
            Conexion con = new Conexion();

            con.crearConexion();
            con.OpenConnection();

            try
            {
                string query = ("INSERT INTO estatus_juridico(IdPrestamo, FechaOficio, Moratorios, Abogado, Estatus, Liquido_con, Observacion, Activo) VALUES" +
                    "('" + NoContrato + "', '" + fechaOficio + "', '" + mora + "', '" + abogado + "', '" + status + "' , 0, '" + obs + "', 1);");
                MySqlCommand cmdJuridico = new MySqlCommand(query, con.GetConnection());
                cmdJuridico.CommandTimeout = 100000;
                cmdJuridico.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                con.CloseConnection();
            }
        }
    }
}
