using BE_56_PS;
using ClassLibrary2;
using ClassLibrary3;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using BLL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
namespace GUI_625NS.Administracion
{
    public partial class FormBitacoraEventos_625NS : Form //, IdiomaObserver_625NS
    {
        private List<BE_Evento_56_PS> eventos;
        private List<BE_Evento_56_PS> filtrados;


        //public void ActualizarIdioma_625NS()
        //{
        //    Traducir_625NS();
        //}
        //public void Traducir_625NS()
        //{
        //    var traductor = new BLL_Idioma_625NS();
        //    traductor.Traducir_625NS(this);
        //}

        public FormBitacoraEventos_625NS()
        {
            InitializeComponent();

            eventos = new BLL_BitacoraEvento_56_PS().obtenerEventos();
            filtrados = new List<BE_Evento_56_PS>(eventos);

            dataGridView1.DataSource = filtrados;

            var user = SessionManager_56_PS.getInstancia().getUsuarioActivo();

            BE_Evento_56_PS evento = new BE_Evento_56_PS(
                user.Dni,
                DateTime.Now,
                "Eventos",
                "Consultó el listado de eventos",
                BE_Evento_56_PS.Criticidad.Bajo
            );

            //SessionManager_625NS.getInstancia().Suscribir_625NS(this);

            //ActualizarIdioma_625NS();

        }

        private void FormBitacoraEventos_625NS_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(eventos.Select(ev => ev.modulo_625NS).Distinct().ToArray());

            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(Enum.GetNames(typeof(BE_Evento_56_PS.Criticidad)));

            dateTimePicker1.Value = eventos.Min(ev => ev.fecha_625NS);
            dateTimePicker2.Value = eventos.Max(ev => ev.fecha_625NS);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            var query = eventos.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(textBox1.Text))
                query = query.Where(ev => ev.dni_625NS.Contains(textBox1.Text));

            if (comboBox1.SelectedItem != null && !string.IsNullOrWhiteSpace(comboBox1.SelectedItem.ToString()))
                query = query.Where(ev => ev.modulo_625NS == comboBox1.SelectedItem.ToString());

            if (comboBox2.SelectedItem != null)
            {
                var crit = (BE_Evento_56_PS.Criticidad)Enum.Parse(typeof(BE_Evento_56_PS.Criticidad), comboBox2.SelectedItem.ToString());
                query = query.Where(ev => ev.criticidad_625NS == crit);
            }

            var desde = dateTimePicker1.Value.Date;
            var hasta = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1); // hasta fin del día

            query = query.Where(ev => ev.fecha_625NS >= desde && ev.fecha_625NS <= hasta);

            filtrados = query.ToList();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filtrados;
        }



        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;

            dateTimePicker1.Value = eventos.Min(ev => ev.fecha_625NS);
            dateTimePicker2.Value = eventos.Max(ev => ev.fecha_625NS);

            filtrados = new List<BE_Evento_56_PS>(eventos);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filtrados;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.");
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF (*.pdf)|*.pdf";
                saveFileDialog.FileName = "BitacoraEventos.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        iTextSharp.text.Font fontTitulo = new iTextSharp.text.Font(
                           iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD
                       );

                        iTextSharp.text.Font fontCabecera = new iTextSharp.text.Font(
                            iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD
                        );

                        iTextSharp.text.Font fontContenido = new iTextSharp.text.Font(
                            iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL
                        );

                        // Agregar título
                        Paragraph titulo = new Paragraph("Bitácora de Eventos", fontTitulo);
                        titulo.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(titulo);
                        pdfDoc.Add(new Paragraph("\n"));

                        PdfPTable pdfTable = new PdfPTable(dataGridView1.Columns.Count);
                        pdfTable.WidthPercentage = 100;

                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, fontCabecera));
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(cell);
                        }

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    pdfTable.AddCell(new Phrase(cell.Value?.ToString() ?? "", fontContenido));
                                }
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                        stream.Close();
                    }

                    MessageBox.Show("PDF exportado correctamente.");

                    string a = SessionManager_56_PS.getInstancia().getUsuarioActivo().Dni;

                    BE_Evento_56_PS ee = new BE_Evento_56_PS(a, DateTime.Now, "Eventos", "Exportacion a pdf de evento", BE_Evento_56_PS.Criticidad.Bajo);
                    new BLL_BitacoraEvento_56_PS().RegistrarEvento(ee);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar PDF: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
