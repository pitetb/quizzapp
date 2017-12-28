using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace QuizzApp.Core.Helpers.Converters
{
    public static class PackDifficultyToColor
    {
        #region IValueConverter Members

        public static Color GetPackColor(double difficulty, bool isLocked)
        {
            //if (difficulty <= 2) {
            //    [self setMainColor:UIColorFromRGB(0x99CC00)];
            //    [self setSecondColor:UIColorFromRGB(0x669900)];
            //} else if ((difficulty >= 2) && (difficulty < 4)) {
            //    [self setMainColor:UIColorFromRGB(0x33B5E5)];
            //    [self setSecondColor:UIColorFromRGB(0x0099CC)];
            //} else if ((difficulty >= 4) && (difficulty < 6)) {
            //    [self setMainColor:UIColorFromRGB(0xFFBB33)];
            //    [self setSecondColor:UIColorFromRGB(0xFF8800)];
            //} else if ((difficulty >= 6) && (difficulty < 8)) {
            //    [self setMainColor:UIColorFromRGB(0xFF4444)];
            //    [self setSecondColor:UIColorFromRGB(0xCC0000)];
            //} else if (difficulty >= 8) {
            //    [self setMainColor:UIColorFromRGB(0x1C1C1C)];
            //    [self setSecondColor:UIColorFromRGB(0x0b0b0b)];
            //}

            byte alpha = 255;
            Color color = Color.FromArgb(alpha, 81, 163, 81);

            if (isLocked)
            {
                alpha = 127;
            }

            if (difficulty <= 2)
            {
                color = Color.FromArgb(alpha, 153, 204 , 0);
            }
            else if (difficulty >= 2 && difficulty < 4)
            {
                color = Color.FromArgb(alpha, 51, 181 , 229);
            }
            else if (difficulty >= 4 && difficulty < 6)
            {
                color = Color.FromArgb(alpha, 255, 187, 51);
            }
            else if (difficulty >= 6 && difficulty < 8)
            {
                color = Color.FromArgb(alpha, 255, 68, 68);
            }
            else
            {
                color = Color.FromArgb(alpha, 28, 28, 28);
            }

            //switch (intDifficulty)
            //{
            //    case 0:
            //        color = Color.FromArgb(alpha, 91, 192, 222);
            //        break;
            //    case 1:
            //        color = Color.FromArgb(alpha, 0, 136, 204);
            //        break;
            //    case 2:
            //        color = Color.FromArgb(alpha, 98, 196, 98);
            //        break;
            //    case 3:
            //        color = Color.FromArgb(alpha, 81, 163, 81);
            //        break;
            //    case 4:
            //        color = Color.FromArgb(alpha, 251, 180, 80);
            //        break;
            //    case 5:
            //        color = Color.FromArgb(alpha, 248, 148, 6);
            //        break;
            //    case 6:
            //        color = Color.FromArgb(alpha, 238, 95, 91);
            //        break;
            //    case 7:
            //        color = Color.FromArgb(alpha, 189, 54, 47);
            //        break;
            //    case 8:
            //        color = Color.FromArgb(alpha, 68, 68, 68);
            //        break;
            //    case 9:
            //        color = Color.FromArgb(alpha, 34, 34, 34);
            //        break;
            //    case 10:
            //        color = Color.FromArgb(alpha, 0, 0, 0);
            //        break;
            //    default:
            //        color = Color.FromArgb(alpha, 81, 163, 81);
            //        break;
            //}

            Debug.WriteLine("Color : " + color);
            return color;
        }
                
        #endregion
    }

    
}
