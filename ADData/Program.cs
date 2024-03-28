using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;

namespace ADData
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ADService.SycnADUser();
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }
    }
}
