using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoSvc.Domain.Entities;

public class DailyTask {

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id {get; set;}

    public required string? Title {set; get;}
    public string? Description {set; get;}
    public PriorityLevel Priority {get; set;}
    
    public DailyTaskStatus Status {set; get;} = DailyTaskStatus.New;
    public DateTime? CompleteDate {set; get;}
    public DateTime? CreatedDate {set; get;} = DateTime.Now;
    public Guid? ObjectId {set; get;}
    public Guid? TodoItemId {set; get;} = Guid.Empty;

}