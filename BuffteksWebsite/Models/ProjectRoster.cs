using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BuffteksWebsite.Models
{
    //tady se usidlily data
    public class ProjectRoster
    {
        public string ProjectParticipantID { get; set; }
        public ProjectParticipant ProjectParticipant { get; set; }
        public string ProjectID { get; set; }
        public Project Project { get; set; }
    }
}