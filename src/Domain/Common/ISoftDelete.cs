namespace TP.Domain.Common;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
