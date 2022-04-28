using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuanBuy.Models.AppUtlity
{
    public static class GetBarColor
    {
        public static string GetColor(decimal? s)
        {
            var result = (int)s / 10;
            string color = "";
            switch (result)
            {
                case 1:
                    color = "#54fa2e";
                    break;
                case 2:
                    color = "#54fa2e";
                    break;
                case 3:
                    color = "#54fa2e";
                    break;
                case 4:
                    color = "#e8ec3c";
                    break;
                case 5:
                    color = "#e8ec3c";
                    break;
                case 6:
                    color = "#e8ec3c";
                    break;
                case 7:
                    color = "#ffae0f";
                    break;
                case 8:
                    color = "#ffae0f";
                    break;
                case 9:
                    color = "#ffae0f";
                    break;
                case 10:
                    color = "#ff0a1b";
                    break;
                default:
                    color = "#F5FFE8";
                    break;
            }

            return color;
        }
    }

}
