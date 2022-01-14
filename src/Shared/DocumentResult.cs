using System.Collections.Generic;
using System.Diagnostics;

namespace Demo.Shared.Models;

public class DocumentResult
{
    public string DocType { get; set; }

    public List<Fields> Fields { get; set; }

    public DocumentResult()
    {
        Fields = new List<Fields>();
    }
}

public class Fields 
{
    public string FieldName { get; set; }

    public string Content { get; set; }

    public float? Confidence { get; set; }
}