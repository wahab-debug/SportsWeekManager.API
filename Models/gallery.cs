//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SportsWeekManager.API.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public partial class gallery
    {
        public int id { get; set; }
        public string path { get; set; }
        public Nullable<int> match_id { get; set; }
        public Nullable<System.DateTime> Date { get; set; }

        [JsonIgnore]public virtual Match Match { get; set; }
    }
}
