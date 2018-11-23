using System.Collections.Generic;
using System.Linq;
using ClassicComedians.Data;
using ClassicComedians.Models;
using ClassicComedians.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ClassicComedians.Controllers
{
    /// <summary>
    ///  Provides basic CRUD operations for manipulating
    /// </summary>
    public class ComedianController : Controller
    {
        public IActionResult Index()
        {
            IEnumerable<Comedian> comedians = Database.GetAllComedians();
            IEnumerable<Group> groups = Database.GetAllGroups();

            IEnumerable<ComedianListViewModel> viewModels = 
                from c in comedians
                join g in groups on c.GroupId equals g.Id
                select new ComedianListViewModel {
                    ComedianId = c.Id,
                    Name = $"{c.FirstName} {c.LastName}",
                    GroupName = g.Name
                };

            return View(viewModels);
        }

        public IActionResult Create()
        {
            IEnumerable<Group> groups = Database.GetAllGroups();
            ComedianCreateViewModel viewModel = new ComedianCreateViewModel { AllGroups = groups };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(Comedian comedian)
        {
            Database.AddComedian(comedian);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            Comedian comedian = Database.GetComedianById(id);
            if (comedian == null)
            {
                return NotFound();
            }
            return View(comedian);
        }

        public IActionResult Edit(int id)
        {
            Comedian comedian = Database.GetComedianById(id);
            if (comedian == null)
            {
                return NotFound();
            }

            IEnumerable<Group> groups = Database.GetAllGroups();
            ComedianEditViewModel viewModel = new ComedianEditViewModel
            {
                Comedian = comedian,
                AllGroups = groups
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(int id, ComedianEditViewModel viewModel)
        {
            Database.UpdateComedian(id, viewModel.Comedian);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            Comedian comedian = Database.GetComedianById(id);
            if (comedian == null)
            {
                return NotFound();
            }
            return View(comedian);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            Database.DeleteComedian(id);
            return RedirectToAction(nameof(Index));
        }
    }
}