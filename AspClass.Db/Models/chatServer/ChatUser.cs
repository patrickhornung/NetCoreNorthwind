using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AspClass.Db;

[Table("chatUser")]
public class ChatUser
{

    [Key]
    [Column("id")] public int Id { get; set; }

    [StringLength(50)]
    [Column("userName")]
    public string Name { get; set; }

    [StringLength(50)]
    [Column("currentStatus")]
    public string CurrentStatus { get; set; }
}
