//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SportWeekManagementAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Playerteaminfo
    {
        public string player_regno { get; set; }
        public int sport_id { get; set; }
        public Nullable<int> team_id { get; set; }
    
        public virtual Player Player { get; set; }
        public virtual Sport Sport { get; set; }
        public virtual Team Team { get; set; }
    }
}
