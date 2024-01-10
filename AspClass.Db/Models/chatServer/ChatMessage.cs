namespace AspClass.Db;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("chatMessage")]
public class ChatMessage
{
    [Key]
    [Column("id")] public int Id { get; set; }

    [Column("idChatChannel")]
    public int IdChatChannel { get; set; }

    [StringLength(50)]
    [Column("message")]
    public string Message { get; set; }

    [Column("sendDate")]
    public DateTime SendDate { get; set; }

    [Column("isDeleted")]
    public bool IsDeleted { get; set; }
}
