using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Inventory
{
    public partial class Form1 : MaterialForm
    {
        private string thePath = "";

        public Form1()
        {
            InitializeComponent();
            initializeDatagrid();

            // Create a material theme manager and add the form to manage (this)
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            // Configure color schema
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue400, Primary.Blue500,
                Primary.Blue500, Accent.LightBlue200,
                TextShade.WHITE
            );
        }

        // Set the data grid up
        private void initializeDatagrid()
        {
            var dgv = dataGridView1;
            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.AllowUserToDeleteRows = true;
            dgv.AllowUserToResizeRows = true;
            dgv.AllowUserToResizeColumns = true;

            // Set the column header and row cell styles.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            DataGridViewCellStyle rowStyle = new DataGridViewCellStyle();
            rowStyle.Font = new Font("Verdana", 10);
            dataGridView1.RowsDefaultCellStyle = rowStyle;

            // Add cell value change event handler
            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
        }

        // Read the CSV file
        public DataTable readCSV(string filePath)
        {
            var dt = new DataTable();
            // Creating the columns
            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            // Adding the rows
            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split('\t'))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));
            return dt;
        }

        // Read the CSV file
        public DataTable readCSVwithCol(string filePath, string ColName)
        {
            var dt = new DataTable();
            // Creating the columns
            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            // Adding the rows
            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split('\t'))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));

            dt.Columns.Add(ColName);

            return dt;
        }

        // Not used currently
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        // Load the file
        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            // Load up the save file dialog with the default option as saving as a .csv or .txt file.
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"S:\Interface Group Documents\VictorApps\Inventory\Test Data";
            ofd.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // get the path to the selected file
                var myPath = ofd.FileName;

                // set the global path  
                thePath = ofd.FileName;

                // fill the data table
                var dt = readCSV(myPath);
                this.dataGridView1.Visible = true;

                // set the data source
                dataGridView1.DataSource = dt;

            //MessageBox.Show("CSV file loaded.");
            }

            dataGridView1.AutoResizeRows();
            dataGridView1.AutoResizeColumns();
        }

        // Save the file
        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
           var dgv = dataGridView1;

            // Don't save if no data is returned
            if (dgv.Rows.Count == 0)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();

            // Column headers
            string columnsHeader = "";
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                if(i == dgv.Columns.Count - 1)
                    columnsHeader += dgv.Columns[i].Name;
                else
                    columnsHeader += dgv.Columns[i].Name + ",";
            }
            sb.Append(columnsHeader + Environment.NewLine);

            var cnt = 0;
            // Go through each cell in the datagridview
            foreach (DataGridViewRow dgvRow in dgv.Rows)
            {
                cnt++;

                // Make sure it's not an empty row.
                if (!dgvRow.IsNewRow)
                {
                    for (int c = 0; c < dgvRow.Cells.Count; c++)
                    {
                        // Append the cells data followed by a comma to delimit.
                        if(c == dgvRow.Cells.Count - 1)
                            sb.Append(dgvRow.Cells[c].Value);
                        else
                            sb.Append(dgvRow.Cells[c].Value + ",");
                    }

                    // Add a new line in the text file.
                    if(cnt != dgv.Rows.Count-1)
                        sb.Append(Environment.NewLine);
                }
            }

            // Load up the save file dialog with the default option as saving as a .csv file.
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // If they've selected a save location...
                using (StreamWriter sw = new StreamWriter(sfd.FileName, false))
                {
                    // Write the stringbuilder text to the the file.
                    sw.WriteLine(sb.ToString());
                }
            }

            // Confirm to the user it has been completed.
            //MessageBox.Show("CSV file saved.");
        }

        // Delete rows
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in this.dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.RemoveAt(item.Index);
            }
        }

        // Event handler for cell changes
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Update the available column whenever the value of any cell changes.
            //UpdateAvailable();
        }

        // Update the number of items available
        private void UpdateAvailable()
        {
            int reserved;
            int total;
            int counter;

            // Iterate through the rows.
            for (counter = 0; counter < dataGridView1.Rows.Count;
                counter++)
            {
                reserved = 0;
                total = 0;

                // Check for a value in the Total column
                if (dataGridView1.Rows[counter].Cells["Total"].Value != null)
                {
                    // Verify that the cell value is not an empty string.
                    if (dataGridView1.Rows[counter]
                        .Cells["Total"].Value.ToString().Length != 0)
                    {
                        total = int.Parse(dataGridView1.Rows[counter]
                            .Cells["Total"].Value.ToString());
                    }
                }
                else
                    return;

                // Check for a value in the reserved column
                if (dataGridView1.Rows[counter].Cells["Reserved"].Value != null)
                {
                    if (dataGridView1.Rows[counter]
                        .Cells["Reserved"].Value.ToString().Length != 0)
                    {
                        reserved = int.Parse(dataGridView1.Rows[counter]
                            .Cells["Reserved"].Value.ToString());
                    }
                }
                else
                    return;

                dataGridView1.Rows[counter].Cells["Available"].Value =
                    (total - reserved).ToString();
            }
        }

        // Add a column button
        private void addCol_Click(object sender, EventArgs e)
        {
            string colName = Prompt.ShowDialog("Column Name", "Enter New Column Name");

            // Check that a file is selected
            if (thePath == "")
            {
                HelpPrompt.ShowDialog("No file selected!\n\n" +
                    "Please select a file before adding columns.\n\n" +
                    "To load a file, click the Load File button and navigate to your file.\n\n" +
                    "The file should be in a tab delimited format before attempting to load it.", "Warning");
                return;
            }

            // Reload data table with new column
            var dt = readCSVwithCol(thePath, colName);
            this.dataGridView1.Visible = true;
            dataGridView1.DataSource = dt;
        }

        // Help button
        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            string colName = HelpPrompt.ShowDialog("To use this program, first convert your data into a tab delimited file.\n\n" +
                "After loading the file, you can edit the data, add rows/columns,\nand save the data back to a tab delimited file.\n\n" +
                "The file can then be loaded into a spreadsheet or database.\n\n" +
                "Support: victor.brunell@tmh.org", "Help");
        }
    }

    // Prompt helper classes to display user prompts
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Width = 400 };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 80, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }

    public static class HelpPrompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 550,
                Height = 300,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Width = 700, Height = 700 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 200, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textLabel.Text : "";
        }
    }
}
