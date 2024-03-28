
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PLL.Helpers;
namespace PLL.Models
{
    public class FilesViewModel
    {
        public ViewDataUploadFilesResult[] Files { get; set; }
    }
}