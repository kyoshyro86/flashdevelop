using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace ScintillaNet
{
	public class XPM
	{
		/// <summary>
        /// Converts Bitmap images to XPM data for use with ScintillaNet.
		/// Warning: images with more than (around) 50 colors will generate incorrect XPM
		/// tColor: specified transparent color in format: "#00FF00".
		/// </summary>
		static public string ConvertToXPM(Bitmap bmp, string tColor)
		{
			StringBuilder sb = new StringBuilder();
			ArrayList colors = new ArrayList();
			ArrayList chars  = new ArrayList();
			int width = bmp.Width;
			int height = bmp.Height;
			int index;
			sb.Append("/* XPM */static char * xmp_data[] = {\"").Append(width).Append(" ").Append(height).Append(" ? 1\"");
			int colorsIndex = sb.Length;
			string col;
			char c;
			for (int y = 0; y<height; y++)
			{
				sb.Append(",\"");
				for (int x = 0; x<width; x++)
				{
					col = ColorTranslator.ToHtml(bmp.GetPixel(x,y));
					index = colors.IndexOf(col);
					if (index < 0)
					{
						index = colors.Add(col)+65;
						if (index > 90) index += 6;
						c = Encoding.ASCII.GetChars(new byte[]{(byte)(index & 0xff)})[0];
						chars.Add(c);
						sb.Insert(colorsIndex, ",\""+c+" c "+col+"\"");
						colorsIndex += 14;
					}
					else c = (char)chars[index];
					sb.Append(c);
				}
				sb.Append("\"");
			}
			sb.Append("};");
			string result = sb.ToString();
			int p = result.IndexOf("?");
			string finalColor = result.Substring(0, p)+colors.Count+result.Substring(p+1).Replace(tColor.ToUpper(), "None");
			return finalColor;
		}
		
	}
	
}
