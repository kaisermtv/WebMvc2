using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Application.Lib
{
    public class ImageMng
    {
        private string[] images;

        public ImageMng(string imgdata)
        {
            if (imgdata == null)
            {
                imgdata = "";
            }
            images = imgdata.Split('|');

        }

        public string First()
        {
            string img = "";
            if (images.Length > 0)
            {
                img = images[0];
            }

            return img;
        }

        public string FirstCrop( int size)
        {
            var sizeFormat = string.Format("?width={0}&crop=0,0,{0},{0}", size);
            return First() + sizeFormat;
        }

        public string FirstCrop(int xsize, int ysize)
        {
            var sizeFormat = string.Format("?width={0},height={1}&crop", xsize, ysize);
            return First() + sizeFormat;
        }
    }
}