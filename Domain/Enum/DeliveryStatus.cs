using System.Collections.Generic;

namespace Domain.DTO;

public enum Deliveriestatus
{
    Waiting,
    Assigned,
    Delivered
}

public static class DeliveriestatusExtensions
{
    private static readonly Dictionary<Deliveriestatus, string> StatusToStringDict 
        = new()
    {
        {Deliveriestatus.Waiting, "Ready"},
        {Deliveriestatus.Assigned, "Assigned"},
        {Deliveriestatus.Delivered, "Delivered"}
    };

    public static string ToString(this Deliveriestatus status)
    {
        return StatusToStringDict[status];
    }
}