using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Entities.Enum;

namespace Entities.Model
{
    [Table("Task")]
    public class Tasks : Notifies
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Title")]
        [MaxLength(255)]
        public string Title { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("Status")]
        public Status Status { get; set; }

        [Column("Created")]
        public DateTime Created { get; set; }

        [ForeignKey("ApplicationUser")]
        [Column(Order = 1)]
        public string UserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
