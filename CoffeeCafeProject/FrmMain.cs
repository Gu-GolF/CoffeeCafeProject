using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CoffeeCafeProject
{
    public partial class FrmMain : Form
    {
        //ตัวแปรเก็ยราคาเมนู
        float[] menuPrice = new float[10];
        //ตัวแปรเก็บรหัสสมาชิก
        int memberId = 0;
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btMenu_Click(object sender, EventArgs e)
        {
            FrmMenu frmMenu = new FrmMenu();
            frmMenu.ShowDialog();
            resetForm();
        }

        private void btMember_Click(object sender, EventArgs e)
        {
            FrmMember frmMember = new FrmMember();
            frmMember.ShowDialog();
        }

        //เมธอด resetForm

        private void resetForm()
        {
            //ตั้งค่า memberIb เป็น 0
            memberId = 0;
            //ให้ rd memberNo, rememberYes ไม่ถูกเลือก
            rdMenberNo.Checked = false;
            rdMemberYes.Checked = false;
            //ให้ tbMemberPhone ว่างและใช้งานไม่ได้
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = false;
            //ให้ tbMemberName เป็นข้อความ (ฃื่อสมาชิก)
            tbMemberName.Text = "(ชื่อสมาชิก)";
            //ให้ lbMemberscore เป็น 0
            lbMemberScore.Text = "0";
            //ให้ lbOrderPay เป็น 0.00
            lbOrderPay.Text = "0.00";
            //เคลีย lvOrederMenu
            lvOrderMenu.Items.Clear();
            lvOrderMenu.Columns.Clear();
            lvOrderMenu.FullRowSelect = true;
            lvOrderMenu.View = View.Details;
            lvOrderMenu.Columns.Add("ชื่อเมนู", 150, HorizontalAlignment.Left);
            lvOrderMenu.Columns.Add("ราคา", 80, HorizontalAlignment.Left);
            //ดึงข้อมูลรายการเมนูจาก Database มาแสดงที่หน้าจอ และเก็บไว้ใช้กับตอนที่ผู็ใช้เลือกเสั่งเมนู
            //กำหนด connection string เพื่อติดต่อไปยัง Database
            //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
            string connectionString = @"Server=DESKTOP-HMDSMC8\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
            //สร้าง connection
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                    string strSQL = "select menuName, menuPrice, menuImage FROM menu_tb";

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        //เอาข้อมูลที่ได้จาก strSQL ที่เป็นก้อน มาแปลงเป็น Table
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        //สร้างตัวแปรอ้างวถึง PictureBox และ Button ที่จะเอารูปและชื่อเมนูไปแสดง
                        PictureBox[] pbMenuImage = { pbMenu1, pbMenu2, pbMenu3, pbMenu4, pbMenu5, pbMenu6, pbMenu7, pbMenu8, pbMenu9, pbMenu10 };
                        Button[] btMenuName = { btMenu1, btMenu2, btMenu3, btMenu4, btMenu5, btMenu6, btMenu7, btMenu8, btMenu9, btMenu10 };

                        //เคลีย pbMenuImage และ btMenuName ก่อนที่จะใส่ลงไปใหม่
                        for (int i = 0; i < 10; i++)
                        {
                            pbMenuImage[i].Image = Properties.Resources.menu;
                            btMenuName[i].Text = "Menu";
                        }

                        //วนลูปเอาข้อมูลที่อยู่ใน Datatable กำหนดให้กับ pbMenuImage, tMenuName, menuPrice
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            btMenuName[i].Text = dataTable.Rows[i]["menuName"].ToString();
                            menuPrice[i] = float.Parse(dataTable.Rows[i]["menuPrice"].ToString());
                            //เอารูปไปกำหนดให้กับ pbMenuImage
                            if (dataTable.Rows[i]["menuImage"] != DBNull.Value)
                            {
                                //กรณีมีรูปต้องแปลงจาก Binary ให้เป็นรูป
                                byte[] imgByte = (byte[])dataTable.Rows[i]["menuImage"];
                                using (var ms = new System.IO.MemoryStream(imgByte))
                                {
                                    pbMenuImage[i].Image = System.Drawing.Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                pbMenuImage[i].Image = Properties.Resources.menu; ;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT:" + ex.Message);
                }
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            resetForm();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            resetForm();
        }

        private void rdMenberNo_CheckedChanged(object sender, EventArgs e)
        {
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = false;
            tbMemberName.Text = "(ชื่อสมาชิก)";
            lbMemberScore.Text = "0";
            memberId = 0;
        }

        private void rdMemberYes_CheckedChanged(object sender, EventArgs e)
        {
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = true;
            tbMemberName.Text = "(ชื่อสมาชิก)";
            lbMemberScore.Text = "0";
        }

        private void tbMemberPhone_KeyUp(object sender, KeyEventArgs e)
        {
            //ตรวจสอบว่าปุ้มที่กดแล้วปล่อยใช่ปุ่ม Enter หรือไม่
            //ถ้าไม่ใช่ก็ไม่ต้องทำอะไร แต่ถ้าใช่ให้เอา เบอร์โทรไปค้นใน Database
            //แล้วชื่อกับแต้มมาโชว์ ส่วนรหัสเอาไว้ใช้บันทึกลง Database
            if (e.KeyCode == Keys.Enter)
            {
                //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
                string connectionString = @"Server=DESKTOP-HMDSMC8\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
                //สร้าง connection
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open();

                        string strSQL = "select memberId, memberName, memberScore FROM member_tb WHERE memberPhone=@memberPhone";

                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection))
                        {
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.VarChar, 50).Value = tbMemberPhone.Text;

                            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand))
                            {
                                //เอาข้อมูลที่ได้จาก strSQL ที่เป็นก้อน มาแปลงเป็น Table
                                DataTable dataTable = new DataTable();
                                dataAdapter.Fill(dataTable);

                                if (dataTable.Rows.Count == 1)
                                {
                                    tbMemberName.Text = dataTable.Rows[0]["memberName"].ToString();
                                    lbMemberScore.Text = dataTable.Rows[0]["memberScore"].ToString();
                                    memberId = int.Parse(dataTable.Rows[0]["memberId"].ToString());

                                }
                                else
                                {
                                    MessageBox.Show("เบอร์โทรนี้ไม่มี กรุณาป้อนเบอร์โทรใหม่อีกรอบ...!");
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT:" + ex.Message);
                    }

                }
            }
        }

        private void btMenu1_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu1.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu1.Text);
                item.SubItems.Add(menuPrice[0].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวยสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }



                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[0]).ToString();
            }
        }

        private void btMenu2_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu2.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu2.Text);
                item.SubItems.Add(menuPrice[1].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวยสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }



                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[1]).ToString();
            }
        }

        private void btMenu3_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu3.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu3.Text);
                item.SubItems.Add(menuPrice[2].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวยสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }



                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[2]).ToString();
            }
        }

        private void btMenu4_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu4.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu4.Text);
                item.SubItems.Add(menuPrice[3].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวยสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }



                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[3]).ToString();
            }
        }

        private void btMenu5_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu5.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu5.Text);
                item.SubItems.Add(menuPrice[4].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวยสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }



                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[4]).ToString();
            }
        }

        private void btMenu6_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu6.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu6.Text);
                item.SubItems.Add(menuPrice[5].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวยสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }



                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[5]).ToString();
            }
        }

        private void btMenu7_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu7.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu7.Text);
                item.SubItems.Add(menuPrice[6].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวยสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }



                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[6]).ToString();
            }
        }

        private void btMenu8_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu8.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu8.Text);
                item.SubItems.Add(menuPrice[7].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวยสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }



                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[7]).ToString();
            }
        }

        private void btMenu9_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu9.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu9.Text);
                item.SubItems.Add(menuPrice[8].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวยสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }



                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[8]).ToString();
            }
        }

        private void btMenu10_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu10.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu10.Text);
                item.SubItems.Add(menuPrice[9].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวยสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }



                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[9]).ToString();
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            //ตรวจสอบก่อนว่ารวมเป็นเงินมีค่า 0.00 หรือเปล่า
            if (lbOrderPay.Text == "0.00")
            {
                MessageBox.Show("เลือกเมนูที่จะสั่งด้วย...!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (rdMemberYes.Checked != true && rdMenberNo.Checked != true)
            {
                MessageBox.Show("เลือกสถานะการเป็นสมาชิกด้วย!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (rdMemberYes.Checked == true && tbMemberName.Text == "(ชื่อสมาชิก)")
            {
                MessageBox.Show("กรุณาค้นหาสมาชิกด้วย!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                //ผ่านตรวจสอบมาได้ ต้องทำ 3 อย่าง
                //1. บันทึกเพิ่มลง order_tb (INSERT INTO......)
                //2. บันทึกเพิ่มลง order_detail_tb (INSERT INTO......)
                //3. บันทึกแก้ไขแต้มคะแนนของสมาชิก member_tb กรณีเป็นสมาชิก (UPDATE... SET...)
                //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
                string connectionString = @"Server=DESKTOP-HMDSMC8\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
                //สร้าง connection
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    try
                    {
                        sqlConnection.Open();
                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction(); //ใช้กับ CRUD

                        //บันทึกลง order_tb ----------------------
                        string strSQL1 = "INSERT INTO order_tb (memberId, orderPay, createAt, updateAt) " +
                                         "VALUES (@memberId, @orderPay, @createAt, @updateAt) " +
                                         "SELECT CAST(SCOPE_IDENTITY() AS INT )";
                        //ตัวแปรเก็บ orderId
                        int orderId;

                        using (SqlCommand sqlCommand = new SqlCommand(strSQL1, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;
                            sqlCommand.Parameters.Add("@orderPay", SqlDbType.Float).Value = float.Parse(lbOrderPay.Text);
                            sqlCommand.Parameters.Add("@createAt", SqlDbType.Date).Value = DateTime.Now;
                            sqlCommand.Parameters.Add("@updateAt", SqlDbType.Date).Value = DateTime.Now;

                            orderId = (int)sqlCommand.ExecuteScalar();

                        }


                        //บันทึกลง order_detail_tb ------------------
                        foreach (ListViewItem item in lvOrderMenu.Items)
                        {
                            string strSQL2 = "INSERT INTO order_detail_tb (orderId, menuName, menuPrice) " +
                                         "VALUES (@orderId, @menuName, @menuPrice) ";
                            using (SqlCommand sqlCommand = new SqlCommand(strSQL2, sqlConnection, sqlTransaction))
                            {
                                sqlCommand.Parameters.Add("@orderId", SqlDbType.Int).Value = orderId;
                                sqlCommand.Parameters.Add("@menuName", SqlDbType.VarChar, 100).Value = item.SubItems[0].Text;
                                sqlCommand.Parameters.Add("@menuPrice", SqlDbType.Float).Value = float.Parse(item.SubItems[1].Text);

                                sqlCommand.ExecuteNonQuery();

                            }

                        }

                        //แก้ไข MemberScore ที่ member_tb --------------
                        if (rdMemberYes.Checked == true)
                        {
                            string strSQL3 = "UPDATE member_tb SET memberScore = @memberScore WHERE memberId = @memberId";

                            using (SqlCommand sqlCommand = new SqlCommand(strSQL3, sqlConnection, sqlTransaction))
                            {
                                sqlCommand.Parameters.Add("@memberScore", SqlDbType.Int).Value = int.Parse(lbMemberScore.Text);
                                sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;


                                sqlCommand.ExecuteNonQuery();

                            }
                        }


                        //-----------------------------------------
                        sqlTransaction.Commit();
                        MessageBox.Show("บันทึกเรียบร้อยแล้ว...^_^", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        resetForm();


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT:" + ex.Message);
                    }
            }
        }

        private void lvOrderMenu_ItemActivate(object sender, EventArgs e)
        {
            // ตรวจสอบว่ามีรายการถูกเลือกอยู่หรือไม่
            if (lvOrderMenu.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvOrderMenu.SelectedItems[0];

                // ดึงราคาสินค้าจาก SubItem ที่ 1 (คอลัมน์ที่สอง)
                float itemPrice = float.Parse(selectedItem.SubItems[1].Text);

                // ลบรายการออกจาก ListView
                lvOrderMenu.Items.Remove(selectedItem);

                // ลดแต้ม ถ้าเป็นสมาชิก
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    int point = int.Parse(lbMemberScore.Text);
                    if (point > 0)
                        lbMemberScore.Text = (point - 1).ToString();
                }

                // ลดยอดรวมราคา
                float currentTotal = float.Parse(lbOrderPay.Text);
                lbOrderPay.Text = (currentTotal - itemPrice).ToString("0.00");
            }

        }

        private void tbMemberPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (char.IsDigit(e.KeyChar) && ((sender as TextBox).Text.Length >= 10))
            {
                e.Handled = true;
            }
        }
    }
}