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

namespace SaveImage
{
    public partial class Form1 : Form
    {
        string fileName;
        List<MyPic> list;

        public Form1()
        {
            InitializeComponent();
        }

        Image ConvertBinaryToImage(byte[] data)
        {
            using(MemoryStream ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.FocusedItem != null)
            {
                pictureBox1.Image = ConvertBinaryToImage(list[listView1.FocusedItem.Index].Data);
                lblFileName.Text = listView1.FocusedItem.SubItems[0].Text;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "JPEG|*.jpg", ValidateNames = true, Multiselect = false })
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    fileName = ofd.FileName;
                    lblFileName.Text = fileName;
                    pictureBox1.Image = Image.FromFile(fileName);
                }
            }

        }

        byte[] ConvertImageToBinary(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveImageEntities db = new SaveImageEntities())
            {
                MyPic pic = new MyPic() { FileName = fileName, Data = ConvertImageToBinary(pictureBox1.Image) };
                db.MyPics.Add(pic);
                await db.SaveChangesAsync();//task will run with no interference
                MessageBox.Show("Successfully Saved.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            using(SaveImageEntities db = new SaveImageEntities())
            {
                list = db.MyPics.ToList();
                foreach(MyPic pic in list)
                {
                    ListViewItem item = new ListViewItem(pic.FileName);
                    listView1.Items.Add(item);
                }
            }
        }
    }
}
