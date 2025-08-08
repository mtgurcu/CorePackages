using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorePackages.Persistance.Entity
{
    public interface IBaseEntity { }
    public class BaseEntity : IBaseEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
    }
}
