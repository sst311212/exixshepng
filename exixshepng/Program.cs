using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exixshepng
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
                return;

            int m_Width = 0, m_Height = 0;
            Bitmap paint = new Bitmap(10000, 10000, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(paint);

            byte[] data = File.ReadAllBytes(args[0]);
            byte[] segm = data.Take(64).ToArray();
            for (int i = 0; i < 8; i++)
            {
                int offset = BitConverter.ToInt32(segm, i * 4);
                byte[] buff = new byte[16];
                Buffer.BlockCopy(data, offset, buff, 0, buff.Length);
                for (int j = 0, k = 0; j < 16; j++, k += 0x77)
                    buff[j] -= (byte)k;
                buff = new byte[BitConverter.ToUInt32(buff, 4) + 8];
                Buffer.BlockCopy(data, offset, buff, 0, buff.Length);
                for (int j = 0, k = 0; j < 16; j++, k += 0x77)
                    buff[j] -= (byte)k;
                Bitmap webp = new Imazen.WebP.SimpleDecoder().DecodeFromBytes(buff, buff.Length);
                g.DrawImage(webp, 0, m_Height, webp.Width, webp.Height);
                if (m_Width < webp.Width)
                    m_Width = webp.Width;
                m_Height += webp.Height;
                webp.Dispose();
            }

            Bitmap image = new Bitmap(m_Width, m_Height, PixelFormat.Format32bppArgb);
            g = Graphics.FromImage(image);
            g.DrawImage(paint, 0, 0, new Rectangle(0, 0, m_Width, m_Height), GraphicsUnit.Pixel);
            image.Save(args[0]);
            image.Dispose();
            paint.Dispose();
        }
    }
}
