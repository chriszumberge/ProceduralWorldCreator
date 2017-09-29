using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProceduralWorldCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var creator = new WorldCreator()
            {
                Height = 100,
                Width = 100
            };

            creator.CreateWorld();

            Bitmap tectonicMap = BitmapGenerator.GetTectonicMap(creator.Width, creator.Height, creator.MapData.Tiles, creator.NumberOfPlates);

            Form form = new Form();
            form.Text = "Image Viewer";
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = tectonicMap;
            pictureBox.Dock = DockStyle.Fill;
            form.Controls.Add(pictureBox);
            Application.Run(form);

            var ids = creator.MapData.Tiles.AsList().Select(x => x.PlateId).Distinct();
        }
    }
}
