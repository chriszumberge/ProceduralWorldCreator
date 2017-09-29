using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProceduralWorldCreator
{
    public static class BitmapHelpers
    {
        public static void ShowBitmap(Bitmap tectonicMap)
        {
            Form form = new Form();
            form.Text = "Image Viewer";
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = tectonicMap;
            pictureBox.Dock = DockStyle.Fill;
            form.Controls.Add(pictureBox);
            Application.Run(form);
        }
    }
}
