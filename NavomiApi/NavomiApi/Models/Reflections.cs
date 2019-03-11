using System;
using System.Collections.Generic;

namespace NavomiApi.Models
{
    public partial class Reflections
    {
        public int ReflectionsId { get; set; }
        public string Osis { get; set; }
     
     
        public DateTime? ReflectionsDate { get; set; }
        public string ReflectionsValue { get; set; }
        public string FileName { get; set; }
        public string SheetName { get; set; }
      
        public DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedUser { get; set; }
    }
}
