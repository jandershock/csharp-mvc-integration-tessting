using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassicComedians.Models.ViewModels
{
    public class ComedianCreateViewModel
    {
        public Comedian Comedian { get; set; }
        public IEnumerable<Group> AllGroups { get; set; }
        public IEnumerable<SelectListItem> GroupOptions
        {
            get
            {
                return AllGroups.Select(g => 
                    new SelectListItem {
                        Text = g.Name,
                        Value = g.Id.ToString()
                    }
                );
            }
        }
    }
}
