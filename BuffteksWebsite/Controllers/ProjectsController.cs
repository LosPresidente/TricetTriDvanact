using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BuffteksWebsite.Models;

/*ja sem velkej borec */

namespace BuffteksWebsite.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly BuffteksWebsiteContext _context;

        public ProjectsController(BuffteksWebsiteContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projects.ToListAsync());
        }

        // GET: Projects/Details/5 (what is the five?)
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .SingleOrDefaultAsync(m => m.ID == id);

            if (project == null)
            {
                return NotFound();
            }

            var clients = 
                from participant in _context.Clients
                join projectparticipant in _context.ProjectRoster
                on participant.ID equals projectparticipant.ProjectParticipantID
                where project.ID == projectparticipant.ProjectID
                select participant;

            var members = 
                from participant in _context.Members
                join projectparticipant in _context.ProjectRoster
                on participant.ID equals projectparticipant.ProjectParticipantID
                where project.ID == projectparticipant.ProjectID                
                select participant;

                

            var projectts = from man in _context.Projects
            select man;

            foreach(var guy in projectts){
                guy.ToString();
                Console.WriteLine(guy);
            }

            ProjectDetailViewModel pdvm = new ProjectDetailViewModel
            {
                TheProject = project,
                ProjectClients = clients.ToList() ?? null,
                ProjectMembers = members.ToList() ?? null
            };

            


            return View(pdvm);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProjectName,ProjectDescription")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.SingleOrDefaultAsync(m => m.ID == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,ProjectName,ProjectDescription")] Project project)
        {
            if (id != project.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/EditProjectParticipants/5
        public async Task<IActionResult> EditProjectParticipants(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.SingleOrDefaultAsync(m => m.ID == id);
            if (project == null)
            {
                return NotFound();
            }


            //var clients = await _context.Clients.ToListAsync();

            //CLIENTS
            //pull 'em into lists first

            /*
            var uniqueclients = 
                from participant in clients
                join projectparticipant in projectroster
                on participant.ID equals projectparticipant.ProjectParticipantID
                where participant.ID != projectparticipant.ProjectParticipantID
                select participant;
            */ 

var clients = await _context.Clients.ToListAsync();
            
            var projectroster = await _context.ProjectRoster.ToListAsync();

            var clientsOnProject=
                from participant in clients
                join projectparticipant in projectroster
                on participant.ID equals projectparticipant.ProjectParticipantID
                where participant.ID == projectparticipant.ProjectParticipantID
                select participant;

                var clientsNotOnProject = clients.Where(p => !clientsOnProject.Any(q2 => q2.ID == p.ID));
List<SelectListItem> clientsSelectList = new List<SelectListItem>();

foreach(var client in clientsNotOnProject)
            {
                clientsSelectList.Add(new SelectListItem { Value=client.ID, Text = client.FirstName + " " + client.LastName});
            }
//--------------------------------------------------------------------------------------------------------------------------------------------------------
            var members = await _context.Members.ToListAsync();
            
            var membersOnProject = 
                from participant in members
                join projectparticipant in projectroster
                on participant.ID equals projectparticipant.ProjectParticipantID
                where participant.ID == projectparticipant.ProjectParticipantID
                select participant;

            var membersNotOnProject = members.Where(p => !membersOnProject.Any(p2 => p2.ID == p.ID));
            //not contributing to a project
            foreach(var member in membersNotOnProject){
                member.ToString();
                Console.WriteLine(member);
            }
            /*
            var uniquemembers = 
                from participant in members
                join projectparticipant in projectroster
                on participant.ID equals projectparticipant.ProjectParticipantID
                where participant.ID != projectparticipant.ProjectParticipantID
                select participant;    
            */
List<SelectListItem> membersSelectList = new List<SelectListItem>();

foreach(var member in membersNotOnProject)
            {
                membersSelectList.Add(new SelectListItem { Value=member.ID, Text = member.FirstName + " " + member.LastName});
            }

            //this is the key
            //mozna kdyz to udelam manualne tak to muzu udelat i tady
            //tri vars jenom copirujou uqiuemembers etc.
            EditProjectDetailViewModel epdvm = new EditProjectDetailViewModel
            {
                ProjectID = project.ID,
                TheProject = project,
                ProjectClientsList = clientsSelectList,
                ProjectMembersList = membersSelectList
            };
            return View(epdvm);
        }        
//gives an error instead of doing nothing, still its an attempt
                [HttpPost, ActionName("EditProjectParticipants")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddConfirmed(EditProjectDetailViewModel EPDVMD)
        {
            var projectAddedTo = await _context.Projects.SingleOrDefaultAsync(Pro => Pro.ID == EPDVMD.ProjectID);
            //change the Members to Clients to add clients and switch back to add Members
            var participantToAdd = await _context.Members.SingleOrDefaultAsync(Mem => Mem.ID == EPDVMD.SelectedID);
            //Clients dont work 
           // var clientToAdd = await _context.Clients.SingleOrDefaultAsync(Cli => Cli.ID == EPDVMD.SelectedID);

            ProjectRoster dude = new ProjectRoster
            {
                ProjectID = projectAddedTo.ID,
                Project = projectAddedTo,
                ProjectParticipantID = participantToAdd.ID,
                ProjectParticipant = participantToAdd
                /* 
                ClientID = clientToAdd.ID,
                Client = clientToAdd
                */

                
            };

            //this writes a new record to the database
            await _context.ProjectRoster.AddAsync(dude);

            //this saves the  change from the write above
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
}
        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null){return NotFound();}

            var project = await _context.Projects.SingleOrDefaultAsync(m => m.ID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(EditProjectDetailViewModel EPDVMD)
        {
            var projectAddedTo = await _context.Projects.SingleOrDefaultAsync(Pro => Pro.ID == EPDVMD.ProjectID);
            var participantToAdd = await _context.Members.SingleOrDefaultAsync(Mem => Mem.ID == EPDVMD.SelectedID);
          var projectAddedToId = await _context.ProjectRoster.SingleOrDefaultAsync(Pro => Pro.ProjectID == EPDVMD.ProjectID);
        
        ProjectRoster pejsek = new ProjectRoster
            {
                ProjectID = projectAddedTo.ID,
                Project = projectAddedTo,
                ProjectParticipantID = participantToAdd.ID,
                ProjectParticipant = participantToAdd
            };
            _context.ProjectRoster.Remove (pejsek);
        
        _context.ProjectRoster.Remove(projectAddedToId);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(string id)
        {
            return _context.Projects.Any(e => e.ID == id);
        }
    }
}