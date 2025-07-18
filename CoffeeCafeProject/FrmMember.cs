﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeCafeProject
{
    public partial class FrmMember : Form
    {
        public FrmMember()
        {
            InitializeComponent();
        }
        private void alertValidate(string msg)
        {
            MessageBox.Show(msg, "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        private void getAllMenuToListView()
        {
            //กำหนด connection string เพื่อติดต่อไปยัง Database
            //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
            string connectionString = @"Server=DESKTOP-HMDSMC8\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
            //สร้าง connection
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                    string strSQL = "select memberId, memberPhone, memberName, memberScore from member_tb";
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        //เอาข้อมูลที่ได้จาก strSQL ที่เป็นก้อน มาแปลงเป็น Table
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        //ตั้งค่า listViews
                        lvShowAllMember.Items.Clear();
                        lvShowAllMember.Columns.Clear();
                        lvShowAllMember.FullRowSelect = true;
                        lvShowAllMember.View = View.Details;



                        //กำหนดรายละเอียดของ column ใน listView
                        lvShowAllMember.Columns.Add("รหัสสมาชิก", 70, HorizontalAlignment.Left);
                        lvShowAllMember.Columns.Add("ชื่อสมาชิก", 150, HorizontalAlignment.Left);
                        lvShowAllMember.Columns.Add("เบอร์โทรสมาชิก", 100, HorizontalAlignment.Left);
                        lvShowAllMember.Columns.Add("คะแนนสะสม", 100, HorizontalAlignment.Left);

                        //loop ข้อมูลใส่ใน Table
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            ListViewItem item = new ListViewItem(dataRow["memberId"].ToString());

                            item.SubItems.Add(dataRow["memberName"].ToString());
                            item.SubItems.Add(dataRow["memberPhone"].ToString());
                            item.SubItems.Add(dataRow["memberScore"].ToString());

                            lvShowAllMember.Items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("พบข้อผิดพลาด :" + ex.Message);
                }
            }
        }

        private void lvShowAllMember_ItemActivate_1(object sender, EventArgs e)
        {

            var item = lvShowAllMember.SelectedItems[0];

            tbMemberId.Text = item.SubItems[0].Text;
            tbMemberName.Text = item.SubItems[1].Text;
            tbMemberPhone.Text = item.SubItems[2].Text;

            btSave.Enabled = false;
            btUpdate.Enabled = true;
            btDelete.Enabled = true;


        }

        private void tbMemberPhone_KeyPress_1(object sender, KeyPressEventArgs e)
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

        private void FrmMember_Load(object sender, EventArgs e)
        {
            tbMemberId.Clear();
            tbMemberPhone.Clear();
            tbMemberName.Clear();
            btSave.Enabled = true;
            btUpdate.Enabled = false;
            btDelete.Enabled = false;
            getAllMenuToListView();
        }

        private void btCancel_Click_1(object sender, EventArgs e)
        {
            tbMemberId.Clear();
            tbMemberPhone.Clear();
            tbMemberName.Clear();
            btSave.Enabled = true;
            btUpdate.Enabled = false;
            btDelete.Enabled = false;
            getAllMenuToListView();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btDelete_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบหรือไม่", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
                string connectionString = @"Server=DESKTOP-HMDSMC8\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open();
                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

                        string deleteSQL = "DELETE FROM member_tb WHERE memberId = @memberId";

                        using (SqlCommand command = new SqlCommand(deleteSQL, sqlConnection, sqlTransaction))
                        {
                            command.Parameters.AddWithValue("@memberId", int.Parse(tbMemberId.Text));
                            int rowsAffected = command.ExecuteNonQuery();
                            sqlTransaction.Commit();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("ลบเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tbMemberId.Clear();
                                tbMemberPhone.Clear();
                                tbMemberName.Clear();
                                btSave.Enabled = true;
                                btUpdate.Enabled = false;
                                btDelete.Enabled = false;
                                getAllMenuToListView();
                            }
                            else
                            {
                                MessageBox.Show("ไม่พบข้อมูลที่จะลบ");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด: " + ex.Message);
                    }
                }
            }
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (tbMemberPhone.Text.Length == 0)
            {
                alertValidate("กรุณาป้อนเบอร์มือถือ");
            }
            else if (tbMemberName.Text.Length == 0)
            {
                alertValidate("กรุณากรอกชื่อ");
            }


            //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
            string connectionString = @"Server=DESKTOP-HMDSMC8\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();



                    SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
                    string strSQL = "UPDATE member_tb " +
                                    "SET memberPhone = @memberPhone, " +
                                    "memberName= @memberName WHERE memberId = @memberId ";



                    using (SqlCommand command = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                    {
                        command.Parameters.Add("@memberPhone", SqlDbType.NVarChar, 50).Value = tbMemberPhone.Text;
                        command.Parameters.Add("@memberName", SqlDbType.NVarChar, 100).Value = tbMemberName.Text;
                        command.Parameters.Add("@memberId", SqlDbType.Int).Value = int.Parse(tbMemberId.Text);

                        int rowsAffected = command.ExecuteNonQuery();
                        sqlTransaction.Commit();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("แก้ไขเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            tbMemberId.Clear();
                            tbMemberPhone.Clear();
                            tbMemberName.Clear();
                            btSave.Enabled = true;
                            btUpdate.Enabled = false;
                            btDelete.Enabled = false;
                            getAllMenuToListView();

                        }
                        else
                        {
                            MessageBox.Show("ไม่พบข้อมูลที่จะอัปเดต");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("พบข้อผิดพลาด :" + ex.Message);
                }
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (tbMemberPhone.Text.Length == 0)
            {
                alertValidate("กรุณาป้อนเบอร์มือถือ");
            }
            else if (tbMemberName.Text.Length == 0)
            {
                alertValidate("กรุณากรอกชื่อ");
            }
            else
            {
                //บันทึกลง db

                //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
                string connectionString = @"Server=DESKTOP-HMDSMC8\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
                //สร้าง connection
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    try
                    {
                        sqlConnection.Open();
                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction(); //ใช้กับ CRUD


                        string strSQL = "INSERT INTO member_tb (memberPhone, memberName, memberScore)" +
                                        "VALUES (@memberPhone,@memberName, @memberScore)";


                        using (SqlCommand command = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {

                            //กำหนด Parameter
                            command.Parameters.Add("@memberPhone", SqlDbType.NVarChar, 100).Value = tbMemberPhone.Text;
                            command.Parameters.Add("@memberName", SqlDbType.NVarChar, 100).Value = tbMemberName.Text;
                            command.Parameters.Add("@memberScore", SqlDbType.Int).Value = 0;


                            //excute sql
                            command.ExecuteNonQuery();
                            sqlTransaction.Commit();

                            MessageBox.Show("บันทึกเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);


                            tbMemberId.Clear();
                            tbMemberName.Clear();
                            tbMemberPhone.Clear();
                            getAllMenuToListView();

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด :" + ex.Message);
                    }
            }
        }
    }
}
