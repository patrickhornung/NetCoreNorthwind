namespace AspClass.Db;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("chatChannel")]
public class ChatChannel
{
    [Key]
    [Column("id")] public int Id { get; set; }

    [Column("idChatUserSender")]
    public int IdChatUserSender { get; set; }

    [Column("idChatUserReceiver")]
    public int IdChatUserReceiver { get; set; }
}
