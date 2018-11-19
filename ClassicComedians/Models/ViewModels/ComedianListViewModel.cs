using System;
using System.ComponentModel.DataAnnotations;

namespace ClassicComedians.Models.ViewModels
{
    public class ComedianListViewModel
    {
        public int ComedianId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Group")]
        public string GroupName { get; set; }
    }
}
