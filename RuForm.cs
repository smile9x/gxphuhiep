using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.SqlClient;

namespace RuForm1
{

    public partial class RuForm1 : Form
    {


        public RuForm1()
        {
            InitializeComponent();
            lbKetQua.ValueMember = "Id";
            lbKetQua.DisplayMember = "Value";
            bLuu.Enabled = false;
        }

        /// <summary>
        /// Open file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    // Insert code to read the stream here.
                    myStream.Close();
                }
            }
        }

        /// <summary>
        /// Save file (false = SaveDialog=no, true=yes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(false, "C:\\", "FileName", "txt");
        }

        /// <summary>
        /// New object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Close Object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Save as
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(true, "C:\\", "FileName", "txt");
        }

        /// <summary>
        /// Exit Application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Aboutbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Aboutbox form
            RuAbout ruAbout = new RuAbout();

            ruAbout.Show();
        }
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="Modus false = SaveDialog=no, true=yes"></param>
        /// <param name="Directory"></param>
        /// <param name="FileName"></param>
        /// <param name="FileExtension"></param>
        void SaveFile(bool Modus, string Directory, string FileName, string FileExtension)
        {
            if (Modus == true)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = FileName;
                saveFileDialog1.DefaultExt = FileExtension;
                saveFileDialog1.InitialDirectory = Directory;
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    /*
                    if ((myStream = saveFileDialog1.) != null)
                    {
                        // Insert code to read the stream here.
                        myStream.Close();
                    }
                     * */
                }
            }
            else
            {

            }
        }

        private void RuForm1_Shown(object sender, EventArgs e)
        {
            String oldFilePath = getFilePath();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            String filePath = null;
            if (oldFilePath == null || oldFilePath == "")
            {
                filePath = openFileData(openFileDialog);
            }
            else
            {
                filePath = oldFilePath;
            }

            connection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;data source=" + filePath);
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    filePath = openFileData(openFileDialog);
                    connection.ConnectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + filePath;
                    connection.Open();
                }

                loadAllData();
                gbKetQuaTimDuoc.Text = "Kết quả tìm được: " + dataTable.Rows.Count;
                foreach (DataRow row in dataTable.Rows)
                {
                    lbKetQua.Items.Add(new Item(row["ID"].ToString(), row["HO"] + " " + row["TEN"]));
                }
            }
        }

        DataTable dataTable = new DataTable();

        private DataTable loadAllData()
        {
            String strGetAll = "select * from [TOAN XU]";
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(strGetAll, connection);            
            oleDbDataAdapter.Fill(dataTable);
            return dataTable;
        }

        private OleDbConnection connection;

        private String openFileData(OpenFileDialog fileData)
        {
            fileData.Filter = "Microsoft Access file(*.mdb)|*.mdb";
            String filePath = null;
            if (fileData.ShowDialog() == DialogResult.OK)
            {
                filePath = fileData.FileName;

                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                config.AppSettings.Settings["filePath"].Value = filePath;
                config.Save(ConfigurationSaveMode.Modified);
            }
            else
            {
                //
            }

            return filePath;
        }

        private String getFilePath()
        {
            return System.Configuration.ConfigurationManager.AppSettings["filePath"].ToString();
        }

        private void bTimKiem_Click(object sender, EventArgs e)
        {
            search(tbSearch.Text);
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                search(tbSearch.Text);
            }
        }

        private void search(String key)
        {
            if (key != null && key != "")
            {
                key = key.ToUpper();
                int type = getType();
                lbKetQua.Items.Clear();

                String strSearch = null;
                OleDbCommand command = null;

                switch (type)
                {
                    case 0:
                        {
                            strSearch = "select * from [TOAN XU] where [TEN] = @KEY";
                            command = new OleDbCommand(strSearch, connection);
                            command.Parameters.AddWithValue("@KEY", key);
                            break;
                        }

                    case 1:
                        {
                            strSearch = "select * from [TOAN XU] where [TEN CHA] like @KEY";
                            command = new OleDbCommand(strSearch, connection);
                            command.Parameters.AddWithValue("@KEY", '%' + key);
                            break;
                        }
                    case 2:
                        {
                            strSearch = "select * from [TOAN XU] where [TEN ME] like @KEY";
                            command = new OleDbCommand(strSearch, connection);
                            command.Parameters.AddWithValue("@KEY", '%' + key);
                            break;
                        }
                    case 3:
                        {
                            strSearch = "select * from [TOAN XU] where [NOI SINH] like @KEY";
                            command = new OleDbCommand(strSearch, connection);
                            command.Parameters.AddWithValue("@KEY", '%' + key + '%');
                            break;
                        }
                    case 4:
                        {
                            strSearch = "select * from [TOAN XU] where [NAM SINH] like @KEY";
                            command = new OleDbCommand(strSearch, connection);
                            command.Parameters.AddWithValue("@KEY", '%' + key + '%');
                            break;
                        }
                    case 5:
                        {
                            strSearch = "select * from [TOAN XU] where [TEN THANH] like @KEY";
                            command = new OleDbCommand(strSearch, connection);
                            command.Parameters.AddWithValue("@KEY", key);
                            break;
                        }
                }
                DataTable data = new DataTable();
                OleDbDataReader reader = command.ExecuteReader();
                data.Load(reader);
                gbKetQuaTimDuoc.Text = "Kết quả tìm được: " + data.Rows.Count;
                foreach (DataRow row in data.Rows)
                {
                    lbKetQua.Items.Add(new Item(row["ID"].ToString(), row["HO"] + " " + row["TEN"]));
                }
            }
            else
            {
                gbKetQuaTimDuoc.Text = "Kết quả tìm được: " + dataTable.Rows.Count;
                foreach (DataRow row in dataTable.Rows)
                {
                    lbKetQua.Items.Add(new Item(row["ID"].ToString(), row["HO"] + " " + row["TEN"]));
                }
            }

        }

        private int getType()
        {
            if (rdCha.Checked)
            {
                return 1;
            }
            else if (rdMe.Checked)
            {
                return 2;
            }
            else if (rdNoiSinh.Checked)
            {
                return 3;
            }
            else if (rdNamSinh.Checked)
            {
                return 4;
            }
            else if (rdTenThanh.Checked)
            {
                return 5;
            }
            return 0;
        }

        private void lbKetQua_SelectedIndexChanged(object sender, EventArgs e)
        {
            string id = ((Item)lbKetQua.SelectedItem).Id;
            String strGetData = "select * from [TOAN XU] where [ID] = @ID";
            //DataTable result = new DataTable();
            OleDbCommand oleDbCommand = new OleDbCommand(strGetData, connection);
            oleDbCommand.Parameters.AddWithValue("@ID", id);
            OleDbDataReader row = oleDbCommand.ExecuteReader();
            //foreach (DataRow row in result.Rows)
            while (row.Read())
            {
                //if (row["ID"].ToString().Equals(id))
                {
                    tbTenThanh.Text = row["TEN THANH"].ToString();
                    tbHoTen.Text = row["HO"] + " " + row["TEN"];
                    tbNamSinh.Text = row["NAM SINH"].ToString();
                    tbNoiSinh.Text = row["NOI SINH"].ToString();
                    tbNgayRuaToi.Text = row["NGAY RT"].ToString();
                    tbLinhMucRuaToi.Text = row["LM RT"].ToString();
                    tbDoDauRuaToi.Text = row["DODAU RT"].ToString();
                    tbNoiRuaToi.Text = row["NOI RT"].ToString();
                    tbNgayXungToiLanDau.Text = row["NGAY XTLD"].ToString();
                    tbNoiXungToiLanDau.Text = row["NOI XTLD"].ToString();
                    tbNgayThemSuc.Text = row["NGAY TS"].ToString();
                    tbNoiThemSuc.Text = row["NOI TS"].ToString();
                    tbLinhMucThemSuc.Text = row["LM TS"].ToString();
                    tbDoDauThemSuc.Text = row["DODAU TS"].ToString();
                    tbHoTenCha.Text = row["TEN CHA"].ToString();
                    tbHoTenMe.Text = row["TEN ME"].ToString();
                    tbGiaoHo.Text = row["HO GIAO"].ToString();
                    tbNgayKetHon.Text = row["NGAY HP"].ToString();
                    tbNoiKetHon.Text = row["NOI HP"].ToString();
                    tbKetHonVoi.Text = row["PHOI NGAU"].ToString();
                    tbLinhMucChungHon.Text = row["LM CHUNGHON"].ToString();
                    tbNguoiLamChung.Text = row["NGUOI LC"].ToString();
                    tbNgayChet.Text = row["NGAY CHET"].ToString().Trim();
                    tbChucVu.Text = row["CHUC VU"].ToString().Trim();
                    rtbGhiChu.Text = row["GHI CHU"].ToString();
                }
            }
            bLuu.Enabled = false;
            lbTrangThai.Text = "Thông tin: " + tbHoTen.Text;
            enableTaoMoi = false;
            enableAllTexbox();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (textBoxKeyDown)
            {
                bLuu.Enabled = true;
                enableSave = true;
                lbTrangThai.Text = "Trạng thái: Chưa được lưu.";
            }
        }
        bool enableSave = false;
        bool enableTaoMoi = false;

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            textBoxKeyDown = true;
        }
        bool textBoxKeyDown = false;

        private void bLuu_Click(object sender, EventArgs e)
        {
            if (enableSave)
            {
                OleDbCommand command = new OleDbCommand();

                command.CommandType = CommandType.Text;
                command.Connection = connection;

                if (enableTaoMoi)
                {
                    command.CommandText = @"insert into [TOAN XU]([TEN THANH], [HO], [TEN], [NAM SINH], [NOI SINH],
                                                                    [NGAY RT], [LM RT], [DODAU RT], [NOI RT], [NGAY XTLD],
                                                                    [NOI XTLD], [NGAY TS], [NOI TS], [LM TS], [DODAU TS], [TEN CHA],
                                                                    [TEN ME], [HO GIAO], [NGAY HP], [NOI HP], [PHOI NGAU], [LM CHUNGHON],
                                                                    [NGUOI LC], [NGAY CHET], [CHUC VU], [GHI CHU])
                                                        values(@TEN_THANH, @HO, @TEN, @NAM_SINH, @NOI_SINH, @NGAY_RT, @LM_RT, @DODAU_RT,
                                                                @NOI_RT, @NGAY_XTLD, @NOI_XTLD, @NGAY_TS, @NOI_TS, @LM_TS, @DODAU_TS, @TEN_CHA,
                                                                @TEN_ME, @HO_GIAO, @NGAY_HP, @NOI_HP, @PHOI_NGAU, @LM_CHUNGHON, @NGUOI_LC, @NGAY_CHET,
                                                                @CHUC_VU, @GHI_CHU)";
                }
                else
                {
                    command.CommandText = @"UPDATE [TOAN XU]
                                                    SET [TEN THANH] = @TEN_THANH,
                                                        [HO] = @HO,
                                                        [TEN] = @TEN,
                                                        [NAM SINH] = @NAM_SINH,
                                                        [NOI SINH] = @NOI_SINH,
                                                        [NGAY RT] = @NGAY_RT,
                                                        [LM RT] = @LM_RT,
                                                        [DODAU RT] = @DODAU_RT,
                                                        [NOI RT] = @NOI_RT,
                                                        [NGAY XTLD] = @NGAY_XTLD,
                                                        [NOI XTLD] = @NOI_XTLD,
                                                        [NGAY TS] = @NGAY_TS,
                                                        [NOI TS] = @NOI_TS,
                                                        [LM TS] = @LM_TS,
                                                        [DODAU TS] = @DODAU_TS,
                                                        [TEN CHA] = @TEN_CHA,
                                                        [TEN ME] = @TEN_ME,
                                                        [HO GIAO] = @HO_GIAO,
                                                        [NGAY HP] = @NGAY_HP,
                                                        [NOI HP] = @NOI_HP,
                                                        [PHOI NGAU] = @PHOI_NGAU,
                                                        [LM CHUNGHON] = @LM_CHUNGHON,
                                                        [NGUOI LC] = @NGUOI_LC,
                                                        [NGAY CHET] = @NGAY_CHET,
                                                        [CHUC VU] = @CHUC_VU,
                                                        [GHI CHU] = @GHI_CHU
                                                    WHERE [ID] = @ID";
                }

                command.Parameters.AddWithValue("@TEN_THANH", tbTenThanh.Text == "" ? " " : tbTenThanh.Text);
                if (tbHoTen.Text == "") tbHoTen.Text = "  "; //cheat code
                String ho = tbHoTen.Text.Substring(0, tbHoTen.Text.LastIndexOf(' '));
                String ten = tbHoTen.Text.Substring(tbHoTen.Text.LastIndexOf(' ') + 1);
                command.Parameters.AddWithValue("@HO", ho);
                command.Parameters.AddWithValue("@TEN", ten);
                command.Parameters.AddWithValue("@NAM_SINH", tbNamSinh.Text == "" ? " " : tbNamSinh.Text);
                command.Parameters.AddWithValue("@NOI_SINH", tbNoiSinh.Text == "" ? " " : tbNoiSinh.Text);
                command.Parameters.AddWithValue("@NGAY_RT", tbNgayRuaToi.Text == "" ? " " : tbNgayRuaToi.Text);
                command.Parameters.AddWithValue("@LM_RT", tbLinhMucRuaToi.Text == "" ? " " : tbLinhMucRuaToi.Text);
                command.Parameters.AddWithValue("@DODAU_RT", tbDoDauRuaToi.Text == "" ? " " : tbDoDauRuaToi.Text);
                command.Parameters.AddWithValue("@NOI_RT", tbNoiRuaToi.Text == "" ? " " : tbNoiRuaToi.Text);
                command.Parameters.AddWithValue("@NGAY_XTLD", tbNgayXungToiLanDau.Text == "" ? " " : tbNgayXungToiLanDau.Text);
                command.Parameters.AddWithValue("@NOI_XTLD", tbNoiXungToiLanDau.Text == "" ? " " : tbNoiXungToiLanDau.Text);
                command.Parameters.AddWithValue("@NGAY_TS", tbNgayThemSuc.Text == "" ? " " : tbNgayThemSuc.Text);
                command.Parameters.AddWithValue("@NOI_TS", tbNoiThemSuc.Text == "" ? " " : tbNoiThemSuc.Text);
                command.Parameters.AddWithValue("@LM_TS", tbLinhMucThemSuc.Text == "" ? " " : tbLinhMucThemSuc.Text);
                command.Parameters.AddWithValue("@DODAU_TS", tbDoDauThemSuc.Text == "" ? " " : tbDoDauThemSuc.Text);
                command.Parameters.AddWithValue("@TEN_CHA", tbHoTenCha.Text == "" ? " " : tbHoTenCha.Text);
                command.Parameters.AddWithValue("@TEN_ME", tbHoTenMe.Text == "" ? " " : tbHoTenMe.Text);
                command.Parameters.AddWithValue("@HO_GIAO", tbGiaoHo.Text == "" ? " " : tbGiaoHo.Text);
                command.Parameters.AddWithValue("@NGAY_HP", tbNgayKetHon.Text == "" ? " " : tbNgayKetHon.Text);
                command.Parameters.AddWithValue("@NOI_HP", tbNoiKetHon.Text == "" ? " " : tbNoiKetHon.Text);
                command.Parameters.AddWithValue("@PHOI_NGAU", tbKetHonVoi.Text == "" ? " " : tbKetHonVoi.Text);
                command.Parameters.AddWithValue("@LM_CHUNGHON", tbLinhMucChungHon.Text == "" ? " " : tbLinhMucChungHon.Text);
                command.Parameters.AddWithValue("@NGUOI_LC", tbNguoiLamChung.Text == "" ? " " : tbNguoiLamChung.Text);
                command.Parameters.AddWithValue("@NGAY_CHET", tbNgayChet.Text == "" ? " " : tbNgayChet.Text);
                command.Parameters.AddWithValue("@CHUC_VU", tbChucVu.Text == "" ? " " : tbChucVu.Text);
                command.Parameters.AddWithValue("@GHI_CHU", rtbGhiChu.Text == "" ? " " : rtbGhiChu.Text);

                if (!enableTaoMoi) {
                    command.Parameters.AddWithValue("@ID", ((Item)lbKetQua.SelectedItem).Id);
                }


                command.ExecuteNonQuery();

                lbTrangThai.Text = "Trạng thái: Lưu thành công.";
            }
            loadAllData();
        }

        private void RuForm1_FormClosing(object sender, FormClosingEventArgs e)
        {
            connection.Close();
        }

        private void bThemMoi_Click(object sender, EventArgs e)
        {
            tbTenThanh.Text = "";
            tbHoTen.Text = "";
            tbNamSinh.Text = "";
            tbNoiSinh.Text = "";
            tbNgayRuaToi.Text = "";
            tbLinhMucRuaToi.Text = "";
            tbDoDauRuaToi.Text = "";
            tbNoiRuaToi.Text = "";
            tbNgayXungToiLanDau.Text = "";
            tbNoiXungToiLanDau.Text = "";
            tbNgayThemSuc.Text = "";
            tbNoiThemSuc.Text = "";
            tbLinhMucThemSuc.Text = "";
            tbDoDauThemSuc.Text = "";
            tbHoTenCha.Text = "";
            tbHoTenMe.Text = "";
            tbGiaoHo.Text = "";
            tbNgayKetHon.Text = "";
            tbNoiKetHon.Text = "";
            tbKetHonVoi.Text = "";
            tbLinhMucChungHon.Text = "";
            tbNguoiLamChung.Text = "";
            tbNgayChet.Text = "";
            tbChucVu.Text = "";
            rtbGhiChu.Text = "";

            enableTaoMoi = true;
            tbTenThanh.Focus();
            lbTrangThai.Text = "Trạng thái: Đang tạo mới.";
            enableAllTexbox();
        }

        private void enableAllTexbox() 
        {
            tbTenThanh.Enabled = true;
            tbHoTen.Enabled = true;
            tbNamSinh.Enabled = true;
            tbNoiSinh.Enabled = true;
            tbNgayRuaToi.Enabled = true;
            tbLinhMucRuaToi.Enabled = true;
            tbDoDauRuaToi.Enabled = true;
            tbNoiRuaToi.Enabled = true;
            tbNgayXungToiLanDau.Enabled = true;
            tbNoiXungToiLanDau.Enabled = true;
            tbNgayThemSuc.Enabled = true;
            tbNoiThemSuc.Enabled = true;
            tbLinhMucThemSuc.Enabled = true;
            tbDoDauThemSuc.Enabled = true;
            tbHoTenCha.Enabled = true;
            tbHoTenMe.Enabled = true;
            tbGiaoHo.Enabled = true;
            tbNgayKetHon.Enabled = true;
            tbNoiKetHon.Enabled = true;
            tbKetHonVoi.Enabled = true;
            tbLinhMucChungHon.Enabled = true;
            tbNguoiLamChung.Enabled = true;
            tbNgayChet.Enabled = true;
            tbChucVu.Enabled = true;
            rtbGhiChu.Enabled = true;
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            if (tbSearch.Text == "")
                search(tbSearch.Text);
        }
    }



    public class Item
    {
        private String id;
        private String value;

        public string Id { get => id; set => id = value; }
        public string Value { get => value; set => this.value = value; }

        public Item(String id, String value)
        {
            this.Id = id;
            this.Value = value;
        }
    }
}
