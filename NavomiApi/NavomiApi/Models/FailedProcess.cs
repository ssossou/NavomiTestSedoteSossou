using System;
using System.Collections.Generic;

namespace NavomiApi.Models
{
    public partial class FailedProcess
    {
        public string ProcessName { get; set; }
        public string FileName { get; set; }
        public string SheetName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public int Id { get; set; }
    }
}
